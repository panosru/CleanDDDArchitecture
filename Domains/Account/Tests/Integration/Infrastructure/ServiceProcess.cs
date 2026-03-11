using System.Diagnostics;
using System.Net;

namespace CleanDDDArchitecture.Domains.Account.Tests.Integration.Infrastructure;

internal sealed class ServiceProcess : IAsyncDisposable
{
    private readonly string _displayName;
    private readonly string _dllPath;
    private readonly string _workingDirectory;
    private readonly Uri _healthUri;
    private readonly IReadOnlyDictionary<string, string> _environmentVariables;
    private readonly Action<string>? _log;
    private readonly List<string> _output = new();
    private readonly object _outputLock = new();
    private Process? _process;

    public ServiceProcess(
        string displayName,
        string dllPath,
        string workingDirectory,
        Uri healthUri,
        IReadOnlyDictionary<string, string> environmentVariables,
        Action<string>? log = null)
    {
        _displayName = displayName;
        _dllPath = dllPath;
        _workingDirectory = workingDirectory;
        _healthUri = healthUri;
        _environmentVariables = environmentVariables;
        _log = log;
    }

    public async Task StartAsync(TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        ProcessStartInfo startInfo = new("dotnet", $"\"{_dllPath}\"")
        {
            WorkingDirectory = _workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        foreach (var (key, value) in _environmentVariables)
        {
            startInfo.Environment[key] = value;
        }

        _process = new Process
        {
            StartInfo = startInfo,
            EnableRaisingEvents = true
        };

        _process.OutputDataReceived += (_, args) =>
        {
            if (args.Data is not null)
            {
                AppendOutput($"[{_displayName}:out] {args.Data}");
            }
        };
        _process.ErrorDataReceived += (_, args) =>
        {
            if (args.Data is not null)
            {
                AppendOutput($"[{_displayName}:err] {args.Data}");
            }
        };

        if (!_process.Start())
        {
            throw new InvalidOperationException($"Failed to start {_displayName}.");
        }

        _log?.Invoke(
            $"{_displayName}: started process {_process.Id} using '{_dllPath}' in '{_workingDirectory}'");

        _process.BeginOutputReadLine();
        _process.BeginErrorReadLine();

        try
        {
            await WaitForHealthAsync(timeout, cancellationToken).ConfigureAwait(false);
        }
        catch
        {
            await DisposeAsync().ConfigureAwait(false);
            throw;
        }
    }

    public string GetLogs()
    {
        lock (_outputLock)
        {
            return string.Join(Environment.NewLine, _output.TakeLast(200));
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_process is null)
        {
            return;
        }

        try
        {
            if (!_process.HasExited)
            {
                _process.Kill(entireProcessTree: true);
                await _process.WaitForExitAsync().ConfigureAwait(false);
            }
        }
        catch
        {
            // best effort disposal
        }
        finally
        {
            _process.Dispose();
            _process = null;
        }
    }

    private async Task WaitForHealthAsync(TimeSpan timeout, CancellationToken cancellationToken)
    {
        using HttpClient client = new() { Timeout = TimeSpan.FromSeconds(5) };
        var deadline = DateTimeOffset.UtcNow + timeout;
        Exception? lastException = null;

        while (DateTimeOffset.UtcNow < deadline)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (_process?.HasExited == true)
            {
                throw new InvalidOperationException(
                    $"{_displayName} exited before becoming healthy.{Environment.NewLine}{GetLogs()}");
            }

            try
            {
                using var response = await client.GetAsync(_healthUri, cancellationToken).ConfigureAwait(false);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return;
                }
            }
            catch (Exception exception)
            {
                lastException = exception;
            }

            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken).ConfigureAwait(false);
        }

        throw new TimeoutException(
            $"{_displayName} did not become healthy at '{_healthUri}' within {timeout}.{Environment.NewLine}" +
            $"{lastException}{Environment.NewLine}{GetLogs()}");
    }

    private void AppendOutput(string line)
    {
        lock (_outputLock)
        {
            _output.Add(line);
        }

        _log?.Invoke(line);
    }
}

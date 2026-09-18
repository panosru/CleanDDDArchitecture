using System.Net;
using System.Text.Json;
using Aviant.Application.Exceptions;
using AwesomeAssertions;
using CleanDDDArchitecture.Hosts.ServiceDefaults.Core.Errors;
using FluentValidation.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace CleanDDDArchitecture.Hosts.RestApi.Tests.Behaviour;

/// <summary>
///     The one error pipeline every host uses: exceptions become RFC 9457 problem details.
/// </summary>
public sealed class ErrorPipelineTests
{
    [Fact]
    public async Task ValidationExceptionBecomes400WithTheFieldErrors()
    {
        using var response = await SendAsync(
            new ValidationException([new ValidationFailure("Title", "Title must not be empty.")]));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/problem+json");
        using var body = await ReadAsync(response);
        body.RootElement.GetProperty("errors").GetProperty("Title")[0].GetString()
           .Should().Be("Title must not be empty.");
    }

    [Fact]
    public async Task NotFoundExceptionBecomes404()
    {
        using var response = await SendAsync(new NotFoundException("TodoList", 42));

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        using var body = await ReadAsync(response);
        body.RootElement.GetProperty("detail").GetString().Should().Contain("42");
    }

    [Fact]
    public async Task UnexpectedExceptionBecomes500WithoutInternalDetailOutsideDevelopment()
    {
        using var response = await SendAsync(new InvalidOperationException("connection string leaked here"));

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        var text = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        text.Should().NotContain("connection string leaked here");
    }

    [Fact]
    public async Task UnexpectedExceptionShowsItsMessageInDevelopment()
    {
        using var response = await SendAsync(new InvalidOperationException("helpful detail"), Environments.Development);

        using var body = await ReadAsync(response);
        body.RootElement.GetProperty("detail").GetString().Should().Be("helpful detail");
    }

    [Fact]
    public async Task ARefusedDomainRuleBecomes400WithItsMessage()
    {
        using var response = await SendAsync(new Aviant.Core.Exceptions.DomainRuleException("An archived list cannot take new items."));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        using var body = await ReadAsync(response);
        body.RootElement.GetProperty("detail").GetString().Should().Be("An archived list cannot take new items.");
    }

    private static async Task<HttpResponseMessage> SendAsync(Exception toThrow, string environment = "Production")
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions { EnvironmentName = environment });
        builder.WebHost.UseTestServer();
        builder.Services.AddApiErrorHandling();

        var app = builder.Build();
        app.UseApiErrorHandling();
        app.MapGet("/boom", (HttpContext _) => throw toThrow);
        await app.StartAsync(TestContext.Current.CancellationToken);

        try
        {
            return await app.GetTestClient().GetAsync(new Uri("/boom", UriKind.Relative), TestContext.Current.CancellationToken);
        }
        finally
        {
            await app.StopAsync(TestContext.Current.CancellationToken);
        }
    }

    private static async Task<JsonDocument> ReadAsync(HttpResponseMessage response) =>
        await JsonDocument.ParseAsync(
            await response.Content.ReadAsStreamAsync(TestContext.Current.CancellationToken),
            cancellationToken: TestContext.Current.CancellationToken);
}

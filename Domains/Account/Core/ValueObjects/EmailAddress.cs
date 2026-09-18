using System.Globalization;
using Aviant.Core.Exceptions;

namespace CleanDDDArchitecture.Domains.Account.Core.ValueObjects;

/// <summary>
///     An email address, normalised (trimmed, lower case) and checked for a plausible shape.
/// </summary>
/// <remarks>
///     Deliverability is proven by the confirmation email, not here; this only keeps obviously
///     malformed values out of the domain model.
/// </remarks>
public sealed record EmailAddress
{
    public const int MaxLength = 254;

    private EmailAddress(string value) => Value = value;

    public string Value { get; }

    public static EmailAddress From(string value)
    {
        var normalised = (value ?? string.Empty).Trim().ToLower(CultureInfo.InvariantCulture);

        if (!IsPlausible(normalised))
            throw new DomainRuleException($"\"{value}\" is not a valid email address.");

        return new EmailAddress(normalised);
    }

    public override string ToString() => Value;

    private static bool IsPlausible(string email)
    {
        if (email.Length is 0 or > MaxLength)
            return false;

        var at = email.IndexOf('@', StringComparison.Ordinal);

        if (at <= 0 || at != email.LastIndexOf('@') || at == email.Length - 1)
            return false;

        var domain = email[(at + 1)..];

        return domain.Contains('.', StringComparison.Ordinal)
            && !domain.StartsWith('.')
            && !domain.EndsWith('.')
            && !email.Any(char.IsWhiteSpace);
    }
}

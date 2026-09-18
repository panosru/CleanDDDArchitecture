using Aviant.Core.Exceptions;

namespace CleanDDDArchitecture.Domains.Account.Core.ValueObjects;

/// <summary>
///     A person's first and last name, each trimmed, non-blank and bounded.
/// </summary>
public sealed record PersonName
{
    public const int MaxLength = 100;

    private PersonName(string first, string last)
    {
        First = first;
        Last  = last;
    }

    public string First { get; }

    public string Last { get; }

    public static PersonName From(string first, string last) =>
        new(Part(first, "first name"), Part(last, "last name"));

    public override string ToString() => $"{First} {Last}";

    private static string Part(string value, string what)
    {
        var trimmed = (value ?? string.Empty).Trim();

        if (trimmed.Length == 0)
            throw new DomainRuleException($"The {what} must not be empty.");

        if (trimmed.Length > MaxLength)
            throw new DomainRuleException($"The {what} can be at most {MaxLength} characters.");

        return trimmed;
    }
}

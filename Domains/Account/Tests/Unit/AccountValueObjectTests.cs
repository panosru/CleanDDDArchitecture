using Aviant.Core.Exceptions;
using AwesomeAssertions;
using CleanDDDArchitecture.Domains.Account.Core.ValueObjects;
using Xunit;

namespace CleanDDDArchitecture.Domains.Account.Tests.Unit;

public sealed class AccountValueObjectTests
{
    [Fact]
    public void EmailAddressIsTrimmedAndLowerCased()
    {
        var email = EmailAddress.From("  Ada.Lovelace@Example.COM ");

        email.Value.Should().Be("ada.lovelace@example.com");
        email.ToString().Should().Be("ada.lovelace@example.com");
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-an-email")]
    [InlineData("@example.com")]
    [InlineData("ada@")]
    [InlineData("ada@example")]
    [InlineData("ada@@example.com")]
    public void EmailAddressRefusesMalformedInput(string input)
    {
        var act = () => EmailAddress.From(input);

        act.Should().Throw<DomainRuleException>();
    }

    [Fact]
    public void EmailAddressesWithTheSameValueAreEqual() =>
        EmailAddress.From("ada@example.com").Should().Be(EmailAddress.From("ADA@example.com"));

    [Fact]
    public void PersonNameIsTrimmed()
    {
        var name = PersonName.From("  Ada ", " Lovelace  ");

        name.First.Should().Be("Ada");
        name.Last.Should().Be("Lovelace");
    }

    [Theory]
    [InlineData("", "Lovelace")]
    [InlineData("Ada", " ")]
    public void PersonNameRefusesBlankParts(string first, string last)
    {
        var act = () => PersonName.From(first, last);

        act.Should().Throw<DomainRuleException>();
    }

    [Fact]
    public void PersonNameRefusesPartsLongerThanTheLimit()
    {
        var act = () => PersonName.From(new string('a', PersonName.MaxLength + 1), "Lovelace");

        act.Should().Throw<DomainRuleException>();
    }
}

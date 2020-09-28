namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.UpdateDetails
{
    using System;
    using Aviant.DDD.Application.UseCases;

    public sealed class UpdateDetailsInput : UseCaseInput
    {
        public UpdateDetailsInput(
            Guid   id,
            string firstName,
            string lastName,
            string email)
        {
            Id        = id;
            FirstName = firstName;
            LastName  = lastName;
            Email     = email;
        }

        internal Guid Id { get; }

        internal string FirstName { get; }

        internal string LastName { get; }

        internal string Email { get; }
    }
}
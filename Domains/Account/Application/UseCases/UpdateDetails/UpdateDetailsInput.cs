namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.UpdateDetails
{
    using System;
    using Aviant.DDD.Application.UseCases;

    public class UpdateDetailsInput : UseCaseInput
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

        public Guid Id { get; }

        public string FirstName { get; }

        public string LastName { get; }

        public string Email { get; }
    }
}
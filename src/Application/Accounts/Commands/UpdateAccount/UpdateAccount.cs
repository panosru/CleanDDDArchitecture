namespace CleanDDDArchitecture.Application.Accounts.Commands.UpdateAccount
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Commands;
    using Aviant.DDD.Domain.Aggregates;
    using Aviant.DDD.Domain.Services;
    using Domain.Entities;
    using MediatR;

    public class UpdateAccount : Command<AccountEntity, AccountId>
    {
        public AccountId Id { get; }
        
        public string FirstName { get; }
        
        public string LastName { get; }
        
        public string Email { get; }

        public UpdateAccount(
            AccountId id, 
            string firstName, 
            string lastName,
            string email)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
        }
    }

    public class UpdateAccountHandler : CommandHandler<UpdateAccount, AccountEntity, AccountId>
    {
        public override async Task<AccountEntity> Handle(UpdateAccount command, CancellationToken cancellationToken)
        {
            var account = await EventsService.RehydrateAsync(command.Id);
            if (account is null)
                throw new ArgumentOutOfRangeException(nameof(UpdateAccount.Id), "Invalid account id");
            
            account.ChangeDetails(command.FirstName, command.LastName, command.Email);

            return account;
        }
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CleanDDDArchitecture.Domains.Account.Infrastructure.Persistence.Contexts;

public sealed class AccountDbContextWriteFactory : IDesignTimeDbContextFactory<AccountDbContextWrite>
{
    public AccountDbContextWrite CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("ConnectionStrings__PGSQLConnection")
         ?? Environment.GetEnvironmentVariable("PGSQLConnection")
         ?? "Server=localhost;Port=15432;Database=cleandddarchitecture;User Id=postgres;Password=change-me;Include Error Detail=true";

        var optionsBuilder = new DbContextOptionsBuilder<AccountDbContextWrite>();
        optionsBuilder.UseNpgsql(
            connectionString,
            builder => builder.MigrationsAssembly(typeof(AccountDbContextWrite).Assembly.FullName));

        return new AccountDbContextWrite(optionsBuilder.Options);
    }
}

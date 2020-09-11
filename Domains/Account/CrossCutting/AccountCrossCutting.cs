namespace CleanDDDArchitecture.Domains.Account.CrossCutting
{
    using System.Collections.Generic;
    using System.Reflection;
    using Application.UseCases.Create;
    using AutoMapper;

    public static class AccountCrossCutting
    {
        public static IEnumerable<Profile> AccountAutoMapperProfiles()
        {
            return new List<Profile>();
        }
        
        public static IEnumerable<Assembly> AccountValidatorAssemblies()
        {
            return new List<Assembly>();
        }

        public static IEnumerable<Assembly> AccountMediatorAssemblies()
        {
            return new List<Assembly>
            {
                typeof(CreateAccount).Assembly
            };
        }
    }
}
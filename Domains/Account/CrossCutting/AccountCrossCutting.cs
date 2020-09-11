namespace CleanDDDArchitecture.Domains.Account.CrossCutting
{
    #region

    using System.Collections.Generic;
    using System.Reflection;
    using Application.UseCases.Create;
    using AutoMapper;

    #endregion

    public static class AccountCrossCutting
    {
        public static IEnumerable<Profile> AccountAutoMapperProfiles() => new List<Profile>();

        public static IEnumerable<Assembly> AccountValidatorAssemblies() => new List<Assembly>();

        public static IEnumerable<Assembly> AccountMediatorAssemblies() => new List<Assembly>
        {
            typeof(CreateAccount).Assembly
        };
    }
}
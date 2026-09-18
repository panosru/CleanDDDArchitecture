/// <summary>
///     JWT settings for local development, the same public dev values docker/compose.yaml uses.
/// </summary>
/// <remarks>
///     These keys protect nothing and must never be used outside a developer machine. Deployed
///     environments supply their own through secrets (see docker/.env.example).
/// </remarks>
internal static class DevelopmentJwt
{
    private static readonly Dictionary<string, string> Validation = new()
    {
        ["Jwt__Issuer"]             = "cleandddarchitecture",
        ["Jwt__Audience"]           = "cleandddarchitecture-clients",
        ["Jwt__Access__Key256Bit"]  = "0123456789abcdef0123456789abcdef",
        ["Jwt__Access__Key512Bit"]  = "0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef",
        ["Jwt__ClockSkewInMinutes"] = "1"
    };

    private static readonly Dictionary<string, string> Issuance = new(Validation)
    {
        ["Jwt__Access__ExpirationDurationInMinutes"]  = "15",
        ["Jwt__Refresh__Key256Bit"]                   = "fedcba9876543210fedcba9876543210",
        ["Jwt__Refresh__Key512Bit"]                   = "fedcba9876543210fedcba9876543210fedcba9876543210fedcba9876543210",
        ["Jwt__Refresh__ExpirationDurationInMinutes"] = "10080"
    };

    /// <summary>For hosts that issue tokens (the monolith, the account service).</summary>
    public static IResourceBuilder<ProjectResource> WithTokenIssuance(this IResourceBuilder<ProjectResource> project) =>
        Apply(project, Issuance);

    /// <summary>For hosts that only validate tokens.</summary>
    public static IResourceBuilder<ProjectResource> WithTokenValidation(this IResourceBuilder<ProjectResource> project) =>
        Apply(project, Validation);

    private static IResourceBuilder<ProjectResource> Apply(
        IResourceBuilder<ProjectResource> project,
        Dictionary<string, string>        settings)
    {
        foreach ((string name, string value) in settings)
            project.WithEnvironment(name, value);

        return project;
    }
}

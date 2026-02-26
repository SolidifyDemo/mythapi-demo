namespace MythApi.Common;

public static class AuthConstants
{
    // This key is used for development and testing only.
    // In production, this MUST be overridden via configuration.
    public const string DevelopmentKey = "ThisIsASecretKeyForDevelopmentPurposesOnly-ChangeInProduction-MustBeAtLeast32Characters";
    public const string DefaultIssuer = "MythApi";
    public const string DefaultAudience = "MythApiUsers";
}

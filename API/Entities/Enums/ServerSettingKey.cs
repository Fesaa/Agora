namespace API.Entities.Enums;


public enum ServerSettingKey
{
    /// <summary>
    /// The open id authority to connect to for authentication
    /// </summary>
    OpenIdAuthority = 0,
    // Do NOT use 1,2; these have been removed
    /// <summary>
    /// At which level Agora will log
    /// </summary>
    LoggingLevel = 3,
    /// <summary>
    /// The provider used as open id authority.
    /// </summary>
    /// <remarks>This is used to register the correct RoleClaimTransformer</remarks>
    OpenIdConnectProviders = 4,
    /// <summary>
    /// Which CalenderProvider your users can connect with
    /// </summary>
    CalenderSyncProvider = 5,
}
namespace API.Entities.Enums;


public enum ServerSettingKey
{
    /// <summary>
    /// The open id authority to connect to for authentication
    /// </summary>
    OpenIdAuthority = 0,
    /// <summary>
    /// The configured client id
    /// </summary>
    OpenIdClientId = 1,
    /// <summary>
    /// The configured client secret
    /// </summary>
    OpenIdClientSecret = 2,
    /// <summary>
    /// At which level Agora will log
    /// </summary>
    LoggingLevel = 3,
}
using System.ComponentModel.DataAnnotations;

namespace API.Entities;

public class UserPreferences
{
    /// <summary>
    /// The id returned by OpenId Connect
    /// </summary>
    [Key]
    public required string ExternalId { get; set; } = null!;

    public string Language { get; set; } = "en";
}
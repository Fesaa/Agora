using System.ComponentModel.DataAnnotations;

namespace API.Entities;

public class UserEmail
{
    [Key]
    public string ExternalId { get; set; }
    
    public string Email { get; set; }
}
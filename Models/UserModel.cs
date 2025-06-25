using System.ComponentModel.DataAnnotations;

public class UserModel
{
    [Required]
    public string Username { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    public string? Role { get; set; } 
}

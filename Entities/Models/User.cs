using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models;

[Table("User")]
public class User
{
    [Key]
    public int ID { get; set; }

    [Required(ErrorMessage = "Username is a required field.")]
    [StringLength(20, MinimumLength = 3, ErrorMessage = "The username must be between 3-20 characters long.")]
    public string Username { get; set; }

    [Required(ErrorMessage = "FirstName is a required field.")]
    [MaxLength(60, ErrorMessage = "Maximum length for FirstName is 60 characters.")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "LastName is a required field.")]
    [MaxLength(60, ErrorMessage = "Maximum length for FirstName is 60 characters.")]
    public string LastName { get; set; }

    [Required]
    public decimal Balance { get; set; } = 1000.00m;

    [Required(ErrorMessage = "Password is a required field.")]
    [MaxLength(256, ErrorMessage = "Maximum length for Password is 8 characters.")]
    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set; }
}

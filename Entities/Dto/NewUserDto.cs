using System.ComponentModel.DataAnnotations;

namespace Entities.Dto;

public class NewUserDto
{

    [Required(ErrorMessage = "Username is a required field.")]
    [StringLength(20, MinimumLength = 3, ErrorMessage = "The username must be between 3-20 characters long.")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "FirstName is a required field.")]
    [MaxLength(60, ErrorMessage = "Maximum length for FirstName is 60 characters.")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "LastName is a required field.")]
    [MaxLength(60, ErrorMessage = "Maximum length for FirstName is 60 characters.")]
    public string LastName { get; set; }

    [Required]
    public string Password { get; set; }
}

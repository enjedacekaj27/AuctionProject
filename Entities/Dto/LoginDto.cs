using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dto;

public class LoginDto
{

    [Required(ErrorMessage = "Username is a required field.")]
    public string Username { get; set; }
    [Required]
    public string Password { get; set; }
}

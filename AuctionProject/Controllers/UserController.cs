using AuctionProject.Validation;
using Contracts;
using Entities.Dto;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuctionProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly IRepositoryManager _repositoryManager;
        public UserController(IRepositoryManager repositoryManager, IConfiguration configuration)
        {
            _repositoryManager = repositoryManager;
            _configuration = configuration;

        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(NewUserDto newUser)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var checkPassword = AccountValidation.isValidPassword(newUser.Password);

            if (!checkPassword)
            {
                return BadRequest("Sorry password is not valid");
            }


            AccountValidation.CreatePasswordHash(newUser.Password,
                out byte[] passwordHash, out byte[] passwordSalt);

            var user = new User
            {
                FirstName = newUser.FirstName,
                LastName = newUser.LastName,
                Username = newUser.UserName,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt

            };

            _repositoryManager.UserRepository.Create(user);
            _repositoryManager.Save();
            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginDto loginDTO)
        {
            var user = _repositoryManager.UserRepository.FindByCondition(u => u.Username == loginDTO.Username, false).FirstOrDefault();
            if (user == null)
            {
                return BadRequest("User not found");
            }


            if (!AccountValidation.VerifyPasswordHash(loginDTO.Password, user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest("Wrong password");
            }


            if (AccountValidation.VerifyPasswordHash(loginDTO.Password, user.PasswordHash, user.PasswordSalt) == true && user.Username == loginDTO.Username)
            {
                List<Claim> claims = new List<Claim>
            {
                new Claim("ID", user.ID.ToString()),

                new Claim(ClaimTypes.NameIdentifier, user.Username),


            };
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("AppSettings:Token"));
                var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
                var tokeOptions = new JwtSecurityToken(
                    issuer: "https://localhost:7124",
                    audience: "https://localhost:7124",
                      claims: claims,
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: signinCredentials);
                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
                return Ok(tokenString);
            }


            return Unauthorized();
        }



    }



}

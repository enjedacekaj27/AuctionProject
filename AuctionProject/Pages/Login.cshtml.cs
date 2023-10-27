using AuctionProject.Validation;
using Contracts;
using Entities.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using NuGet.Protocol.Core.Types;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuctionProject.Pages
{
    public class LoginModel : PageModel
    {
        private IConfiguration _configuration;
        private readonly IRepositoryManager _repositoryManager;

        public LoginModel(IRepositoryManager repositoryManager, IConfiguration configuration)
        {
            _repositoryManager = repositoryManager;
            _configuration = configuration;

        }
        public ActionResult OnGet()
        {
            if (this.User.Identity.IsAuthenticated)
            {
                // Home Page.  
                return this.RedirectToPage("Index");
            }
            else
            {
                return this.Page();
            }
        }

        public ActionResult OnPost([FromForm] LoginDto loginDTO)
        {
            var username = Request.Form["username"];
            var password = Request.Form["password"];

            var user = _repositoryManager.UserRepository.FindByCondition(u => u.Username == loginDTO.Username, false).FirstOrDefault();
            if (user == null)
            {

                ModelState.AddModelError(string.Empty, "User not found!");
                return this.Page();
            }



            var loginResult = AccountValidation.VerifyPasswordHash(loginDTO.Password, user.PasswordHash, user.PasswordSalt) == true && user.Username == loginDTO.Username;


            if (loginResult)
            {
                List<Claim> claims = new List<Claim>
                {
                    new Claim("ID", user.ID.ToString()),
                    new Claim("FullName", $"{user.FirstName} {user.LastName}"),
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
                HttpContext.Session.SetString("Token", tokenString);
                return RedirectToPage("Index");

            }
            else
            {
                ModelState.AddModelError(string.Empty, "Wrong credentials!");
                return this.Page();
            }
        }

        public ActionResult OnGetLogout()
        {
            Response.Headers.Remove("Authorization");
            
            HttpContext.Session.Remove("Token");
            HttpContext.Session.Clear();
            return RedirectToPage("Login");
            
        }
    }
}

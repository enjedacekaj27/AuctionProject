using AuctionProject.Validation;
using Contracts;
using Entities.Dto;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuctionProject.Pages
{
    public class RegisterModel : PageModel
    {
        private IConfiguration _configuration;
        private readonly IRepositoryManager _repositoryManager;
        public RegisterModel(IRepositoryManager repositoryManager, IConfiguration configuration)
        {
            _repositoryManager = repositoryManager;
            _configuration = configuration;

        }
        public void OnGet()
        {
        }

        public ActionResult OnPost(NewUserDto newUser)
        {
            if (!ModelState.IsValid)
            {
                return this.Page();
            }

            var checkPassword = AccountValidation.isValidPassword(newUser.Password);

            if (!checkPassword)
            {
                ModelState.AddModelError(string.Empty, "Sorry password is not valid");
                return this.Page();
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
            return RedirectToPage("Login");
        }
    }
}

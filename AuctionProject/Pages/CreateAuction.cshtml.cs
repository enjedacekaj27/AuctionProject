using AutoMapper;
using Contracts;
using Entities.Dto;
using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuctionProject.Pages
{
    [Authorize]
    public class CreateAuctionModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private IConfiguration _configuration;
        private readonly IRepositoryManager _repositoryManager;
        private readonly IMapper _mapper;
        public CreateAuctionModel(ILogger<IndexModel> logger, IRepositoryManager repositoryManager, IConfiguration configuration, IMapper mapper)
        {
            _repositoryManager = repositoryManager;
            _configuration = configuration;
            _logger = logger;
            _mapper = mapper;
        }

        public ActionResult OnGet()
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type == "ID");
            var userData = _repositoryManager.UserRepository.GetUserById(Convert.ToInt32(userId.Value));
            ViewData["userData"] = userData;
            if (userData == null)
            {

                return RedirectToPage("Login");
            };

            var Auctions = _repositoryManager.AuctionRepository.GetAllAuctions(new Entities.Helper.PagesParameter { PageNumber = 1, PageSize = 100 }).ToList();
            ViewData["Auctions"] = Auctions;
            return this.Page();
        }

        public ActionResult OnPost([FromForm] NewAuctionDto newAuctionDto)
        {

            if (!ModelState.IsValid)
            {
               return this.Page();
            }

            var currentUser = HttpContext?.User.Claims.FirstOrDefault()?.Value ?? "1";
            var auction = _mapper.Map<Auction>(newAuctionDto);
            {
                auction.ProductName = newAuctionDto.ProductName;
                auction.Description = newAuctionDto.Description;
                auction.StartingBid = newAuctionDto.StartingBid;
                auction.StartDate = DateTime.Now;
                auction.EndDate = newAuctionDto.EndDate;
                auction.UserID = Convert.ToInt32(currentUser);
            };
            _repositoryManager.AuctionRepository.Create(auction);

            _repositoryManager.Save();
            return RedirectToPage("Index");
        }
    }
}

using AutoMapper;
using Contracts;
using Entities.Dto;
using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NuGet.Protocol.Core.Types;

namespace AuctionProject.Pages
{
    [Authorize]
    public class AuctionDetailsModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private IConfiguration _configuration;
        private readonly IRepositoryManager _repositoryManager;
        private readonly IMapper _mapper;

        public AuctionDetailsModel(ILogger<IndexModel> logger, IRepositoryManager repositoryManager, IConfiguration configuration, IMapper mapper)
        {
            _repositoryManager = repositoryManager;
            _configuration = configuration;
            _logger = logger;
            _mapper = mapper;
        }

        public ActionResult OnGet(int ID)
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type == "ID");
            var userData = _repositoryManager.UserRepository.GetUserById(Convert.ToInt32(userId.Value));
            ViewData["userData"] = userData;
            if (userData == null)
            {

                return RedirectToPage("Login");
            };

            var Auction = _repositoryManager.AuctionRepository.GetAuctionById(ID);
            var HighestBid = _repositoryManager.BidRepository.GetHighestBidForAuction(ID);
            if (HighestBid == null)
            {
                HighestBid = new Entities.Models.Bid
                {
                    AuctionID = ID,
                    BidAmount = Auction.StartingBid,
                    UserID = Auction.UserID,
                    ID = Auction.ID

                };
            }
            ViewData["Auction"] = Auction;
            ViewData["HighestBid"] = HighestBid;
            ViewData["User"] = userData;
            return this.Page();
        }

        public ActionResult OnPost([FromForm]NewBidDto newBidDto)
        {
            var currentUser = HttpContext?.User.Claims.FirstOrDefault()?.Value ?? "1";
            var userId = Convert.ToInt32(currentUser);

            // Retrieve the previous highest bid for the auction
            var previousBid = _repositoryManager.BidRepository.GetHighestBidForAuction(newBidDto.AuctionID);
            var auction = _repositoryManager.AuctionRepository.GetAuctionById(newBidDto.AuctionID);
            if (auction == null)
            {
                ModelState.AddModelError(string.Empty, "Auction not found.");
                return this.Page();

            }
            var user = _repositoryManager.UserRepository.GetUserById(userId);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User not found.");
                return this.Page();

            }
            if (previousBid == null)
            {
                previousBid = new Entities.Models.Bid
                {
                    AuctionID = auction.ID,
                    BidAmount = auction.StartingBid,
                    UserID = auction.UserID,
                    ID = auction.ID

                };
            }
            ViewData["Auction"] = auction;
            ViewData["HighestBid"] = previousBid;
            ViewData["User"] = user;


            if (previousBid != null && newBidDto.BidAmount <= previousBid.BidAmount)
            {
                ModelState.AddModelError(string.Empty, "The new bid amount must be higher than the previous bid.");
                return this.Page();
            }

            // Check the user's balance
            

            if (userId == auction.UserID)
            {
                ModelState.AddModelError(string.Empty, "Cannot place bid for own auctions!");
                return this.Page();
            }

            if (newBidDto.BidAmount > user.Balance)
            {
                ModelState.AddModelError(string.Empty, "The bid amount exceeds your current balance.");
                return this.Page();
            }

            // Create and save the new bid
            var bid = _mapper.Map<Bid>(newBidDto);
            bid.UserID = userId;

            _repositoryManager.BidRepository.Create(bid);
            _repositoryManager.Save();

            return RedirectToPage("Index");
        }

    }
}

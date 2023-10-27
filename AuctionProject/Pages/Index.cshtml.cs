using Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Repository;
using System.Security.Claims;

namespace AuctionProject.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private IConfiguration _configuration;
        private readonly IRepositoryManager _repositoryManager;

        public IndexModel(ILogger<IndexModel> logger,IRepositoryManager repositoryManager, IConfiguration configuration)
        {
            _repositoryManager = repositoryManager;
            _configuration = configuration;
            _logger = logger;
        }


        public ActionResult OnGet()
        {
            var userId = User.Claims.FirstOrDefault(x=> x.Type == "ID");
            var userData = _repositoryManager.UserRepository.GetUserById(Convert.ToInt32(userId.Value));
            ViewData["userData"] = userData;
            if (userData == null) {

                return RedirectToPage("Login");
            };

            var Auctions = _repositoryManager.AuctionRepository.GetAllAuctions(new Entities.Helper.PagesParameter { PageNumber = 1, PageSize = 100 }).ToList();
            Auctions.ForEach(x =>
            {
                x.Bids = _repositoryManager.BidRepository.GetBidForAucton(x.ID);
            });
            ViewData["Auctions"] = Auctions;
            return this.Page();

        }

  
        public ActionResult OnGetDelete(int ID)
        {
         

            var userId = User.Claims.FirstOrDefault(x => x.Type == "ID");
            var userData = _repositoryManager.UserRepository.GetUserById(Convert.ToInt32(userId.Value));
            ViewData["userData"] = userData;
            if (userData == null)
            {

                return RedirectToPage("Login");
            };

            var Auctions = _repositoryManager.AuctionRepository.GetAllAuctions(new Entities.Helper.PagesParameter { PageNumber = 1, PageSize = 100 }).ToList();
            Auctions.ForEach(x =>
            {
                x.Bids = _repositoryManager.BidRepository.GetBidForAucton(x.ID);
            });
            ViewData["Auctions"] = Auctions;

            var auction = _repositoryManager.AuctionRepository.GetAuctionById(ID);

            if (auction == null)
            {
                ModelState.AddModelError(string.Empty, $"Auction with id {ID} not found on db");
                return this.Page();
            }

            //// Calculate the time remaining in days
            //var currentDateTime = DateTime.Now;
            //var timeRemaining = (int)(auction.EndDate - currentDateTime).TotalDays;

            //if (timeRemaining == 0)
            //{

            // Determine the winning bid
            var winningBid = _repositoryManager.BidRepository.GetHighestBidForAuction(ID);

            if (winningBid == null)
            {
                ModelState.AddModelError(string.Empty, "No winning bid found for this auction.");
                return this.Page();
            }

            if (auction.UserID != Convert.ToInt32(userId.Value))
            {
                ModelState.AddModelError(string.Empty, "You are not the owner of this auction.");
                return this.Page();
            }

            // Get the auction owner
            var auctionOwner = _repositoryManager.UserRepository.GetUserById(auction.UserID);

            if (auctionOwner == null)
            {
                ModelState.AddModelError(string.Empty, "Auction owner not found.");
                return this.Page();
            }

            //get the highest bidder

            var highestBidder = _repositoryManager.UserRepository.GetUserById(winningBid.UserID);

            if (highestBidder == null)
            {
                ModelState.AddModelError(string.Empty, "Highest bidder not found.");
                return this.Page();
            }

            // Update the balances
            auctionOwner.Balance += winningBid.BidAmount;
            highestBidder.Balance -= winningBid.BidAmount;
            //mark the auction as deleted
            auction.IsDeleted = true;

            // Update the database
            _repositoryManager.UserRepository.Update(auctionOwner);
            _repositoryManager.Save();
            _repositoryManager.UserRepository.Update(highestBidder);
            _repositoryManager.Save();
            _repositoryManager.AuctionRepository.Update(auction);
            _repositoryManager.Save();




            return RedirectToPage("Index");
            //}
            //else
            //{
            //    // If time remaining is not zero, you can handle this case accordingly
            //    return BadRequest("Auction cannot be deleted as it still has time remaining.");
            //}
        }
    }
}
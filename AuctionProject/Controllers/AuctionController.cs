using AutoMapper;
using Contracts;
using Entities.Dto;
using Entities.Helper;
using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AuctionProject.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AuctionController : ControllerBase
{
    private readonly IMapper _mapper;
    private IRepositoryManager _repository;

    public AuctionController(
        IMapper mapper,
        IRepositoryManager repositoryManager
       )
    {
        _mapper = mapper;
        _repository = repositoryManager;
    }


    /// <summary>
    /// Create new Auction for the current user
    /// </summary>
    /// <param name="newAuctionDto"> Dto with new auction parameters</param>
    /// <returns></returns>
    [HttpPost("AddNewAuction")]
    [Authorize]
    public async Task<ActionResult> AddNewAuction([FromBody] NewAuctionDto newAuctionDto)
    {

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
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
        _repository.AuctionRepository.Create(auction);

        _repository.Save();
        return Ok(auction);
    }
    /// <summary>
    /// Get All Auction for the current user
    /// </summary>
    /// <param name="pagesParameter"></param>
    /// <returns></returns>

    [HttpGet("GetAllAuctions")]
    public ActionResult GetAllAuctions([FromQuery] PagesParameter pagesParameter)
    {
        var currentUser = HttpContext?.User.Claims.FirstOrDefault()?.Value ?? "1";
        var auctions = _repository.AuctionRepository.GetAllAuctions(pagesParameter);
        var currentDateTime = DateTime.Now;
        var auctionsWithTimeRemaining = auctions.Select(auction =>
        {
            var timeRemaining = (int)(auction.EndDate - currentDateTime).TotalDays;
            return new
            {
                Auction = auction,
                TimeRemainingInDays = timeRemaining
            };
        }).OrderBy(a => a.TimeRemainingInDays);

        var metadata = new
        {
            auctions.TotalCount,
            auctions.PageSize,
            auctions.CurrentPage,
            auctions.TotalPages,
            auctions.HasNext,
            auctions.HasPrevious
        };
        if (auctions == null)
        {
            return NotFound("You dont have any auctions on db");
        }
        Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));



        var auctionDto = _mapper.Map<IEnumerable<AllAuctionDto>>(auctionsWithTimeRemaining.Select(a => new AllAuctionDto
        {
            Title = a.Auction.ProductName,
            UserId = Convert.ToInt32(currentUser),
            Description = a.Auction.Description,
            TopBid = a.Auction.StartingBid,
            isDeleted = a.Auction.IsDeleted,
            TimeRemainingInDays = a.TimeRemainingInDays
        }));

        return Ok(auctionDto);

    }
    /// <summary>
    /// Get Auction by id
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>

    //Lexojme notes sipas id 
    [HttpGet("GetAuctionById/{ID}")]

    public IActionResult GetAuctionById(int ID)
    {
        var auctionObject = _repository.AuctionRepository.GetAuctionById(ID);

        if (auctionObject == null)
        {
            return NotFound($"Auction with id {ID} not found on db");
        }
        var auctionObjectDto_id = _mapper.Map<AllAuctionDto>(auctionObject);
        return Ok(auctionObjectDto_id);
    }

/// <summary>
/// Close auction and add winning bid to the Auction owner
/// </summary>
/// <param name="ID"></param>
/// <returns></returns>
    [HttpDelete("CloseAuctionAndAddWinningBidToTheOwner/{ID}")]
    public IActionResult CloseAuctionAndAddWinningBidToTheOwner(int ID)
    {
        var currentUser = HttpContext?.User.Claims.FirstOrDefault()?.Value;
        if (string.IsNullOrEmpty(currentUser))
        {
            return Unauthorized("User not authenticated.");
        }

        var userId = Convert.ToInt32(currentUser);

        var auction = _repository.AuctionRepository.GetAuctionById(ID);

        if (auction == null)
        {
            return NotFound($"Auction with id {ID} not found on db");
        }

        // Calculate the time remaining in days
        var currentDateTime = DateTime.Now;
        var timeRemaining = (int)(auction.EndDate - currentDateTime).TotalDays;

        if (timeRemaining == 0)
        {
           
            // Determine the winning bid
            var winningBid = _repository.BidRepository.GetHighestBidForAuction(ID);

            if (winningBid == null)
            {
                return NotFound("No winning bid found for this auction.");
            }

            if (winningBid.UserID != userId)
            {
                return BadRequest("You are not the winning bidder for this auction.");
            }

            // Get the auction owner
            var auctionOwner = _repository.UserRepository.GetUserById(auction.UserID);

            if (auctionOwner == null)
            {
                return NotFound("Auction owner not found.");
            }

            // Update the auction owner's balance
            auctionOwner.Balance += winningBid.BidAmount;

            // Update the database
            _repository.UserRepository.Update(auctionOwner);
            _repository.Save();

            var response = new
            {
                WinningBidAmount = winningBid.BidAmount,
                IsDeleted = true
            };

            return Ok(response);
        }
        else
        {
            // If time remaining is not zero, you can handle this case accordingly
            return BadRequest("Auction cannot be deleted as it still has time remaining.");
        }

    }

   
}

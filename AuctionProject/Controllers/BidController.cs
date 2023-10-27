using AutoMapper;
using Contracts;
using Entities.Dto;
using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuctionProject.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class BidController : ControllerBase
{
    private readonly IMapper _mapper;
    private IRepositoryManager _repository;

    public BidController(
        IMapper mapper,
        IRepositoryManager repositoryManager
       )
    {
        _mapper = mapper;
        _repository = repositoryManager;
    }

    [HttpPost("AddNewBid")]
    [Authorize]
    public async Task<ActionResult> AddNewBid([FromBody] NewBidDto newBidDto)
    {
        var currentUser = HttpContext?.User.Claims.FirstOrDefault()?.Value ?? "1";
        var userId = Convert.ToInt32(currentUser);

        // Retrieve the previous highest bid for the auction
        var previousBid = _repository.BidRepository.GetHighestBidForAuction(newBidDto.AuctionID);

        if (previousBid != null && newBidDto.BidAmount <= previousBid.BidAmount)
        {
            return BadRequest("The new bid amount must be higher than the previous bid.");
        }

        // Check the user's balance
        var user = _repository.UserRepository.GetUserById(userId);

        if (user == null)
        {
            return NotFound("User not found.");
        }

        if (newBidDto.BidAmount > user.Balance)
        {
            return BadRequest("The bid amount exceeds your current balance.");
        }

        // Create and save the new bid
        var bid = _mapper.Map<Bid>(newBidDto);
        bid.UserID = userId;

        _repository.BidRepository.Create(bid);
        _repository.Save();

        return Ok(bid);
    }

  
}

namespace Entities.Dto;

public class AllAuctionDto
{
    public string Title { get; set; }
    public int UserId { get; set; }
    public string Description { get; set; }
    public decimal TopBid { get; set; }
    public bool isDeleted { get; set; }
    public int TimeRemainingInDays { get; set; }
}

using Entities.AttributeValidation;
using System.ComponentModel.DataAnnotations;

namespace Entities.Dto;

public class NewAuctionDto
{
    [Required]
    [StringLength(64,MinimumLength = 3, ErrorMessage = "The ProductName must be between 3-64 characters long.")]
    public string ProductName { get; set; }
    [Required]
    [StringLength(256, MinimumLength = 10, ErrorMessage = "The Description must be between 3-64 characters long.")]
    public string Description { get; set; }

    [Required]
    [DateLaterThanOrEqualToToday]
    public DateTime EndDate { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "StartingBid must be greater than 0")]
    public decimal StartingBid { get; set; }
}

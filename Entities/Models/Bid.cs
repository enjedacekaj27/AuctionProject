using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models;

public class Bid
{
    [Key]

    public int ID { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "BidAmount must be greater than 0")]
    public decimal BidAmount { get; set; }
    
    [ForeignKey("UserID")]
    public int UserID { get; set; }
   
    [ForeignKey("AuctionID")]
    public int AuctionID { get; set; }
}

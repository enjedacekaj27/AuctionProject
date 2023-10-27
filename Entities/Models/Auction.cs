using Entities.AttributeValidation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models;

public class Auction
{
    [Key]

    public int ID { get; set; }

    [Required(ErrorMessage = "ProductName is a required field.")]
    [MaxLength(60, ErrorMessage = "Maximum length for ProductName is 60 characters.")]
    public string ProductName { get; set; }


    [Required(ErrorMessage = "Description is a required field.")]
    [MaxLength(256, ErrorMessage = "Maximum length for Description is 256 characters.")]
    public string Description { get; set; }
  
    [Required]
    [DateLaterThanOrEqualToToday]
    public DateTime StartDate { get; set; }

    [Required]
    [DateLaterThanOrEqualToToday]
    public DateTime EndDate { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "StartingBid must be greater than 0")]
    public decimal StartingBid { get; set; }

    [Required]
    public bool IsDeleted { get; set; }

    [ForeignKey("UserID")]
    public int UserID { get; set; }

    public IEnumerable<Bid> Bids { get; set; }  
}

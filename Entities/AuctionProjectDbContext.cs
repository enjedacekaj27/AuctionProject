using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Entities;

public class AuctionProjectDbContext : DbContext
{
    public AuctionProjectDbContext(DbContextOptions options): base(options)
    {

    }
    public AuctionProjectDbContext()
    {

    }
    public DbSet<User> User { get; set; }
    public DbSet<Auction> Auction { get; set; }
    public DbSet<Bid> Bid { get; set; }
}

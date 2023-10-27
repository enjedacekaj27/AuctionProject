using Contracts;
using Entities;
using Entities.Models;

namespace Repository
{
    public class BidRepository : RepositoryBase<Bid>,IBidRepository
    {
        public BidRepository(AuctionProjectDbContext repositoryContext)
            : base(repositoryContext)
        { }

        public Bid GetBidById(int ID)
        {
            var bid = this.FindByCondition(x => x.ID == ID, false).FirstOrDefault();
            return bid;
        }

        public Bid GetHighestBidForAuction(int auctionId)
        {
            var bid= this.FindByCondition(b => b.AuctionID == auctionId,false)
            .OrderByDescending(b => b.BidAmount)
            .FirstOrDefault();
            return bid;
        }

        IEnumerable<Bid> IBidRepository.GetBidForAucton(int id)
        {
            var bid = this.FindByCondition(b => b.AuctionID == id, false).ToList();
            return bid;
        }
    }
}

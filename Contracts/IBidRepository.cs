
using Entities.Models;

namespace Contracts;

public interface IBidRepository : IRepositoryBase<Bid>
{
    Bid GetBidById(int ID);
    Bid GetHighestBidForAuction(int Id);
    IEnumerable<Bid> GetBidForAucton(int id);
}

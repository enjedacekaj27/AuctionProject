using Entities.Helper;
using Entities.Models;

namespace Contracts;

public interface IAcutionRepository : IRepositoryBase<Auction>
{
    PagedList<Auction> GetAllAuctions(PagesParameter pagesParameter);

    Auction GetAuctionById(int ID);
}

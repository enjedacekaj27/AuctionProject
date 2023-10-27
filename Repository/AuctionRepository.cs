using Contracts;
using Entities;
using Entities.Helper;
using Entities.Models;

namespace Repository
{
    public class AuctionRepository : RepositoryBase<Auction>, IAcutionRepository
    {
        public AuctionRepository(AuctionProjectDbContext repositoryContext)
            : base(repositoryContext)
        { }

        public PagedList<Auction> GetAllAuctions(PagesParameter pagesParameter)
        {

            return PagedList<Auction>.ToPagedList(FindAll(false).Where(x=> x.IsDeleted == false),
         pagesParameter.PageNumber,
         pagesParameter.PageSize);
        }

        public Auction GetAuctionById(int ID)
        {
            var auction = base.FindByCondition(x => x.ID == ID && x.IsDeleted == false, false).FirstOrDefault();
            return auction;
        }
    }
}
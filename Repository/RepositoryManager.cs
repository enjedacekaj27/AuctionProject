using Contracts;
using Entities;

namespace Repository;

public class RepositoryManager : IRepositoryManager
{
    private AuctionProjectDbContext _repositoryContext;
    private IUserRepository  _userRepository;
    private IAcutionRepository _acutionRepository;
    private IBidRepository _bidRepository;

    public RepositoryManager(AuctionProjectDbContext auctionProjectDbContext)
    {
        _repositoryContext = auctionProjectDbContext;
    }
    public IAcutionRepository AuctionRepository 
    { 
        get
        {
            if (_acutionRepository == null)
                _acutionRepository = new AuctionRepository(_repositoryContext);
            return _acutionRepository;
        }
    
     }

    public IUserRepository UserRepository
    {
        get
        {
            if (_userRepository == null)
                _userRepository = new UserRepository(_repositoryContext);
            return _userRepository;
        }

    }

    public IBidRepository BidRepository
    {
        get
        {
            if (_bidRepository == null)
                _bidRepository = new BidRepository(_repositoryContext);
            return _bidRepository;
        }

    }

    public void Save() => _repositoryContext.SaveChanges();
}

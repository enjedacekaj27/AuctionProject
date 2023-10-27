namespace Contracts;

public interface IRepositoryManager
{
    IAcutionRepository AuctionRepository { get; }
    IUserRepository UserRepository { get; }
    IBidRepository BidRepository { get; }
    void Save();
}

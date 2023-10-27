using Contracts;
using Entities;
using Entities.Models;

namespace Repository;

public class UserRepository : RepositoryBase<User>,IUserRepository
{
    public UserRepository(AuctionProjectDbContext repositoryContext)
        : base(repositoryContext) 
    { }

    public User GetUserById(int ID)
    {
        var user = base.FindByCondition(x => x.ID == ID, false).FirstOrDefault();
        return user;
    }
}

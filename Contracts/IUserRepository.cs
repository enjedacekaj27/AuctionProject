using Entities.Models;

namespace Contracts;

public interface IUserRepository : IRepositoryBase<User>
{
    User GetUserById(int ID);
}

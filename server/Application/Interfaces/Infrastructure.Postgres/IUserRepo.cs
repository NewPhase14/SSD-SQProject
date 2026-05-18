using Core.Domain.Entities;

namespace Application.Interfaces.Infrastructure.Postgres;

public interface IUserRepo
{
    User? GetUserOrNull(string email);
    User AddUser(User user);
    User UpdateUser(User user);
}
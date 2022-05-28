using TripBooker.WebApi.Models;

namespace TripBooker.WebApi.Repositories
{
    public interface IUserRepository
	{
		User? GetUserByUsername(string username);
		IEnumerable<User> GetUsers();
	}
}
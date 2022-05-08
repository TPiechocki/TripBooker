using WebApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApi.Repositories
{
	public interface IUserRepository
	{
		Task<User> GetUserByUsername(string username);
		Task<IEnumerable<User>> GetUsers();
	}
}
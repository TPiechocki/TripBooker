using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Models;

namespace WebApi.Repositories
{
	/// <summary>
	/// NOTE: DO NOT USE THIS IMPLEMENTATION. THIS IS FOR DEMO PURPOSE ONLY
	/// </summary>
	public class InMemoryUserRepository : IUserRepository
	{
		private List<User> _users = new List<User>
		{
			new User { Username = "TestUser1", Password = "1234" },
			new User { Username = "TestUser2", Password = "1234" },
			new User { Username = "TestUser3", Password = "1234" },
			new User { Username = "TestUser4", Password = "1234" },
			new User { Username = "TestUser5", Password = "1234" },
			new User { Username = "TestUser6", Password = "1234" },
			new User { Username = "TestUser7", Password = "1234" },
			new User { Username = "TestUser8", Password = "1234" },
			new User { Username = "TestUser9", Password = "1234" },
			new User { Username = "TestUser10", Password = "1234" },
			new User { Username = "TestUser11", Password = "1234" },
			new User { Username = "TestUser12", Password = "1234" },
			new User { Username = "TestUser13", Password = "1234" },
		};


		public Task<User> GetUserByUsername(string username)
		{
			return Task.FromResult(_users.FirstOrDefault(u => u.Username == username));
		}

		public Task<IEnumerable<User>> GetUsers()
		{
			return Task.FromResult<IEnumerable<User>>(_users);
		}
	}
}

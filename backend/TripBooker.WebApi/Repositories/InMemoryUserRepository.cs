using TripBooker.WebApi.Models;

namespace TripBooker.WebApi.Repositories
{
    public class InMemoryUserRepository : IUserRepository
	{
		private readonly List<User> _users = new()
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

		public User? GetUserByUsername(string username)
		{
			return _users.FirstOrDefault(u => u.Username == username);
		}

		public IEnumerable<User> GetUsers()
		{
			return _users;
		}
	}
}

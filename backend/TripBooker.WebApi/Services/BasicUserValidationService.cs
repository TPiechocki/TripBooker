using AspNetCore.Authentication.Basic;
using TripBooker.WebApi.Repositories;

namespace TripBooker.WebApi.Services
{
    internal class BasicUserValidationService : IBasicUserValidationService
	{
		private readonly ILogger<BasicUserValidationService> _logger;
		private readonly IUserRepository _userRepository;

		public BasicUserValidationService(ILogger<BasicUserValidationService> logger, IUserRepository userRepository)
		{
			_logger = logger;
			_userRepository = userRepository;
		}

		public Task<bool> IsValidAsync(string username, string password)
		{
			try
			{
				var user = _userRepository.GetUserByUsername(username);
				var isValid = user != null && user.Password == password;
                return Task.FromResult(isValid);
			}
			catch (Exception e)
			{
				_logger.LogError(e, e.Message);
				throw;
			}
		}
	}
}
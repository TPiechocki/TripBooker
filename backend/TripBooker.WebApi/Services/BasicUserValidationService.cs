using AspNetCore.Authentication.Basic;
using Microsoft.Extensions.Logging;
using WebApi.Repositories;
using System;
using System.Threading.Tasks;

namespace WebApi.Services
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

		public async Task<bool> IsValidAsync(string username, string password)
		{
			try
			{
				var user = await _userRepository.GetUserByUsername(username);
				var isValid = user != null && user.Password == password;
				return isValid;
			}
			catch (Exception e)
			{
				_logger.LogError(e, e.Message);
				throw;
			}
		}
	}
}
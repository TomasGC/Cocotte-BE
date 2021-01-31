using System;
using System.Reflection;
using Cocotte.APIs;
using Cocotte.Managers;
using Cocotte.MongoDb.Types;
using Cocotte.Types.Responses;
using Common.Request;
using CoreLib.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Cocotte.Controllers {
	/// <summary>
	/// The controller for the users.
	/// </summary>
	[Produces("application/json")]
	[ApiController]
	[Route("users/api")]
	public class UsersController : ControllerBase {
		static readonly ILogger log = Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

		/// <summary>
		/// Authentication infos.
		/// </summary>
		AuthenticationResponse auth;

		/// <summary>
		/// Controller to get the auth infos.
		/// </summary>
		/// <param name="httpContextAccessor"></param>
		public UsersController(IHttpContextAccessor httpContextAccessor) => auth = Authentication.AuthenticateSession(httpContextAccessor.HttpContext.Request);

		/// <summary>
		/// Log in the user.
		/// </summary>
		/// <tags>Users</tags>
		/// <param name="request">The info to log in.</param>
		/// <returns>The authentication response.</returns>
		[HttpPost("login")]
		public AuthenticationResponse Login(LoginRequest request) {
			try {
				return Authentication.Login(request.Login, request.Password);
			}
			catch (Exception e) {
				log.Error($"Error [{e.Message}] - Stack [{e.StackTrace}].");
				return new AuthenticationResponse() { Success = false, Rescode = -1, Message = e.Message };
			}
		}

		/// <summary>
		/// Create a new user.
		/// </summary>
		/// <tags>Users</tags>
		/// <param name="login">Login.</param>
		/// <param name="password">Password.</param>
		/// <returns>Success of the user's creation.</returns>
		[HttpPost("user/create/{login}/{password}")]
		public BaseResponse Create(string login, string password) {
			try {
				UsersApi.Create(login, password);
				return new BaseResponse { Success = true };
			}
			catch (Exception e) {
				log.Error($"Error [{e.Message}] - Stack [{e.StackTrace}].");
				return new BaseResponse { Success = false, Rescode = -1, Message = e.Message };
			}
		}

		/// <summary>
		/// Modify the user.
		/// </summary>
		/// <tags>Users</tags>
		/// <param name="user">User infos.</param>
		/// <returns>Success of the user's update.</returns>
		[HttpPut("user/update")]
		public BaseResponse Update(Users user) {
			if (!auth.Success)
				return new BaseResponse() { Success = false, Rescode = -100, Message = "Bad authentication !" };

			try {
				UsersApi.Update(user);
				return new BaseResponse { Success = true };
			}
			catch (Exception e) {
				log.Error($"Error [{e.Message}] - Stack [{e.StackTrace}].");
				return new BaseResponse { Success = false, Rescode = -1, Message = e.Message };
			}
		}
	};
}

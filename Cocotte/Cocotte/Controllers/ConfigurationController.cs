using System;
using System.Reflection;
using Cocotte.APIs;
using Cocotte.Managers;
using Cocotte.MongoDb.Types;
using Cocotte.Types.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Cocotte.Controllers {
	/// <summary>
	/// The controller for the configuration.
	/// </summary>
	[Produces("application/json")]
	[ApiController]
	[Route("configuration/api")]
	public class ConfigurationController : ControllerBase {
		static readonly ILogger log = Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

		/// <summary>
		/// Authentication infos.
		/// </summary>
		AuthenticationResponse auth;

		/// <summary>
		/// Controller to get the auth infos.
		/// </summary>
		/// <param name="httpContextAccessor"></param>
		public ConfigurationController(IHttpContextAccessor httpContextAccessor) => auth = Authentication.AuthenticateSession(httpContextAccessor.HttpContext.Request);

		/// <summary>
		/// Get the default colors.
		/// </summary>
		/// <tags>Users</tags>
		/// <returns>The session key.</returns>
		[HttpGet("colors")]
		public UIColors GetDefaultColors() {
			if (!auth.Success)
				return null;

			try {
				return ConfigurationApi.GetDefaultColors();
			}
			catch (Exception e) {
				log.Error($"Error [{e.Message}] - Stack [{e.StackTrace}].");
				return null;
			}
		}
	};
}

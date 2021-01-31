using System.Reflection;
using Cocotte.MongoDb.Types;
using CoreLib.Managers;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Serilog;

namespace Cocotte.APIs {
	/// <summary>
	/// Api class that communicates directly with MongoDB, to treat the configation.
	/// </summary>
	public static class ConfigurationApi {
		static readonly ILogger log = Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

		/// <summary>
		/// Get default colors.
		/// </summary>
		/// <returns>AuthenticationResponse</returns>
		public static UIColors GetDefaultColors() {
			return MongoManager.GetQueryable<Configurations>().Select(x => x.DefaultUIColors).First();
		}
	};
}

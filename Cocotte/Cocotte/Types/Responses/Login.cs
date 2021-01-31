using Cocotte.MongoDb.Types;
using CoreLib.Types;

namespace Cocotte.Types.Responses {
	/// <summary>
	/// Response for the login.
	/// </summary>
	public class AuthenticationResponse : BaseResponse {
		/// <summary>
		/// The User info.
		/// </summary>
		public Users User { get; set; }
		/// <summary>
		/// The Session info.
		/// </summary>
		public Sessions Session { get; set; }
	};
}

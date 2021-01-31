namespace Common.Request {
	/// <summary>
	/// Information for log in.
	/// </summary>
	public class LoginRequest {
		/// <summary>
		/// The login.
		/// </summary>
		public string Login { get; set; }
		/// <summary>
		/// The password.
		/// </summary>
		public string Password { get; set; }
	};
}

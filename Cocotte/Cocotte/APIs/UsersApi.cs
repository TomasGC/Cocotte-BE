using System;
using System.Reflection;
using Cocotte.MongoDb.Types;
using Cocotte.Types.Responses;
using CoreLib.Managers;
using MongoDB.Bson;
using Serilog;

namespace Cocotte.APIs {
	/// <summary>
	/// Api class that communicates directly with MongoDB, to treat the Users.
	/// </summary>
	public static class UsersApi {
		static readonly ILogger log = Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

		/// <summary>
		/// Authentication by session key.
		/// </summary>
		/// <param name="sessionKey">The session.</param>
		/// <returns>AuthenticationResponse</returns>
		public static AuthenticationResponse AuthenticateSession(string sessionKey) {
			Sessions session = MongoManager.FirstOrDefault<Sessions>(x => x.Key == sessionKey);

			if(session == null)
				throw new Exception("The session key is invalid.");

			log.Information($"Authentication succeed.");

			Users user = MongoManager.First<Users>(x => x._id == session.UserId);

			return new AuthenticationResponse { Success = true, User = user, Session = session };
		}

		/// <summary>
		/// Get the user by a given user id.
		/// </summary>
		/// <param name="userId"></param>
		public static Users GetUser(ObjectId userId) => MongoManager.First<Users>(x => x._id == userId);

		/// <summary>
		/// Update the user infos.
		/// </summary>
		/// <param name="login"></param>
		/// <param name="password"></param>
		public static void Create(string login, string password) => MongoManager.AddItemInCollection(new Users(login, password, ConfigurationApi.GetDefaultColors()));

		/// <summary>
		/// Update the user infos.
		/// </summary>
		/// <param name="user"></param>
		public static void Update(Users user) => MongoManager.UpdateItem(user);
	};
}

using System;
using System.Linq;
using System.Reflection;
using Cocotte.MongoDb.Types;
using Cocotte.Types.Responses;
using CoreLib.Managers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Serilog;

namespace Cocotte.Managers {
	/// <summary>
	/// Authentication class.
	/// </summary>
	public class Authentication {
		/// <summary>
		/// Logger
		/// </summary>
		static readonly ILogger log = Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

		/// <summary>
		/// Check session and returns some related objects.
		/// </summary>
		/// <param name="httpRequest"></param>
		/// <returns></returns>
		public static AuthenticationResponse AuthenticateSession(HttpRequest httpRequest) {
            // Check the Barear Key
            StringValues keys;
			string key;

            if (httpRequest.Method == "OPTIONS")
                return new AuthenticationResponse() { Success = false, Rescode = -1001, Message = "OPTIONS Cors received." };

            try {
                // Check header barear Token
                if (!httpRequest.Headers.ContainsKey("Authorization")) {
                    log.Information("Http header does not contains Authorization key, skipped check session.");
                    return new AuthenticationResponse() { Success = false, Rescode = -1002, Message = "No Authorization header key." };
                }

                // Key is type of: "Basic {key}"
                httpRequest.Headers.TryGetValue("Authorization", out keys);
				key = keys.First().Replace("{", "").Replace("}", "");
                log.Debug($"Check Session => Key = [{key}]");
            }
            catch (Exception e) {
                log.Error($"Auth FAILED, BAD Http headers. Error [{e.Message}] - Stack [{e.StackTrace}]");
                return new AuthenticationResponse() { Success = false, Rescode = -2, Message = "Bad authentication (bad http headers)" };
            }

			try {
				return AuthenticateSession(key);
			}
			catch (Exception e) {
				log.Error($"Auth FAILED, impossible to retrieve the Session Key. Error [{e.Message}] - Stack [{e.StackTrace}]");
				return new AuthenticationResponse() { Success = false, Rescode = -2, Message = "Bad authentication (bad http headers)" };
			}
        }

		/// <summary>
		/// Check session and returns some related objects.
		/// </summary>
		/// <param name="sessionKey"></param>
		/// <returns></returns>
		static AuthenticationResponse AuthenticateSession(string sessionKey) {
			// Get session
			Sessions session = MongoManager.FirstOrDefault<Sessions>(s => s.Key == sessionKey);

			if (session == null) {
				// BAD auth
				log.Error("Auth FAILED, fail to get the fetched database row.");
				return new AuthenticationResponse() { Success = false, Rescode = -1, Message = "Bad authentication" };
			}

			// OK, session existing
			log.Information($"Auth OK, for userId = [{session.UserId}].");

			return new AuthenticationResponse() {
				Success = true,
				Rescode = 0,
				User = MongoManager.First<Users>(x => x._id == session.UserId),
				Session = session
			};
		}

		/// <summary>
		/// Login operation
		/// Create a session for user if auth is Ok.
		/// </summary>
		/// <param name="login"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public static AuthenticationResponse Login(string login, string password) {
			login = login.Trim();
			password = password.Trim();
			Users user = MongoManager.FirstOrDefault<Users>(x => x.Login == login);
			if (user == null)
				throw new Exception($"There is no user {login} in the Database.");

			if (user.Password != password)
				throw new Exception("The password is wrong.");

			Sessions session = MongoManager.SingleOrDefault<Sessions>(x => x.UserId == user._id);
			if (session == null) {
				session = new Sessions {
					CreationDate = DateTime.Now,
					UserId = user._id,
					Key = "Here you put the code with the session key."
				};

				MongoManager.AddItemInCollection(session);
			}

			log.Information($"Added session {session._id}.");

			return new AuthenticationResponse { Success = true, User = user, Session = session };
		}
	};
}

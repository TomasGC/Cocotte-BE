using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Cocotte.APIs;
using Cocotte.Types.Responses;
using Microsoft.AspNetCore.SignalR;
using Serilog;

namespace Cocotte.Hubs {
	/// <summary>
	/// Base Hub.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class BaseHub<T> : Hub where T : BaseHubHandler<T> {
		static readonly ILogger log = Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

		/// <summary>
		/// Banned auth keys.
		/// </summary>
		public Dictionary<string, int> BannedAuthKeys { get; set; }
		/// <summary>
		/// User connection ids.
		/// </summary>
		public Dictionary<AuthenticationResponse, List<string>> UserConnectionIds { get; set; }
		/// <summary>
		/// Class name.
		/// </summary>
		public string ClassName { get; set; }

		/// <summary>
		/// Hub manages all Events or Changes.
		/// </summary>
		public BaseHub() {
			Type type = typeof(T);

			if (type == typeof(EventHubHandler)) {
				BannedAuthKeys = EventHubHandler.BannedAuthKeys;
				UserConnectionIds = EventHubHandler.UserConnectionIds;
				ClassName = EventHubHandler.ClassName;
			}

			log.Debug($"{ClassName} EventHub created.");
		}

		/// <summary>
		/// Event: on new connection.
		/// </summary>
		/// <returns></returns>
		public async override Task OnConnectedAsync() {
			log.Information($"{ClassName} OnConnectedAsync() called");

			// Get the auth key token.
			var test = Context.GetHttpContext();
			string accessAuthKey = test.Request.Query["access_token"];

			// Check banned auth keys.
			if (BannedAuthKeys.Count(x => x.Key == accessAuthKey) == 1 && BannedAuthKeys[accessAuthKey] >= BaseHubHandler<T>.MaxAuthKeyAttempts) {
				Context.GetHttpContext().Abort();
				return;
			}

			// Check auth token.
			AuthenticationResponse auth = UsersApi.AuthenticateSession(accessAuthKey);

			if (auth.Success) {
				try {
					// Add user into connection pool.
					BaseHubHandler<T>.AddUserConnectionId(auth, Context.ConnectionId);
				}
				catch (Exception e) {
					log.Error($"{ClassName} Error referencing User in connection pool. Exception = [{e.Message}] - StackTrace = {e.StackTrace}.");

					Context.GetHttpContext().Abort();
					return;
				}
			}
			else {
				log.Information($"{ClassName} Connection failed, bad auth for authKey = [{accessAuthKey}]");

				// Add to banned auth keys to avoid potential flood.
				if (BannedAuthKeys.Count(x => x.Key == accessAuthKey) == 0)
					BannedAuthKeys.Add(accessAuthKey, 1);
				else {
					BannedAuthKeys[accessAuthKey] += 1;

					if (BannedAuthKeys[accessAuthKey] >= BaseHubHandler<T>.MaxAuthKeyAttempts)
						log.Warning($"{ClassName} - BANNED authKey because too many attemps ({BaseHubHandler<T>.MaxAuthKeyAttempts}) = [{accessAuthKey}]");
				}

				Context.GetHttpContext().Abort();
				return;
			}

			log.Information($"{ClassName} User = [{auth.User._id}, {auth.User.Login}] successfully connected.");

			await base.OnConnectedAsync();
		}

		/// <summary>
		/// Event: disconnected.
		/// </summary>
		/// <param name="exception"></param>
		/// <returns></returns>
		public override Task OnDisconnectedAsync(Exception exception) {
			string connectionId = Context.ConnectionId;

			try {
				if (UserConnectionIds.Count(x => x.Value.Contains(connectionId)) != 0)
					BaseHubHandler<T>.RemoveUserConnectionId(connectionId);
			}
			catch (Exception e) {
				log.Warning($"{ClassName} Error disconnecting #1! StackTrace = {e.StackTrace}.");
			}

			try {
				if (UserConnectionIds.Count(x => x.Value.Contains(connectionId)) == 0)
					return base.OnDisconnectedAsync(exception);
			}
			catch (Exception e) {
				log.Warning($"{ClassName} Error disconnecting #2! StackTrace = {e.StackTrace}.");
			}

			return base.OnDisconnectedAsync(exception);
		}
	};
}

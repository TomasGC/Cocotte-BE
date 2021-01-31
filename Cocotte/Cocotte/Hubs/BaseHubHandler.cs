using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;
using Cocotte.MongoDb.Types;
using Cocotte.Types.Responses;
using CoreLib.Managers;
using CoreLib.Types;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using Serilog;

namespace Cocotte.Hubs {
	/// <summary>
	/// Base Hub Handler.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class BaseHubHandler<T> where T : BaseHubHandler<T> {
		protected static readonly ILogger log = Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);
		/// <summary>
		/// Log timer.
		/// </summary>
		protected static Timer logTimer;
		//readonly IHubContext<BaseHub> hubContext;
		/// <summary>
		/// Configuration for the hub.
		/// </summary>
		protected static IConfiguration configuration { get; set; }

		/// <summary>
		/// Users connected.
		/// </summary>
		public static Dictionary<AuthenticationResponse, List<string>> UserConnectionIds { get; protected set; }

		/// <summary>
		/// Users banned.
		/// </summary>
		public static Dictionary<string, int> BannedAuthKeys { get; protected set; }
		/// <summary>
		/// Max auth key attempts.
		/// </summary>
		public static readonly int MaxAuthKeyAttempts = 20;
		/// <summary>
		/// Class name.
		/// </summary>
		public static readonly string ClassName = typeof(T).Name;

		/// <summary>
		/// Constructor of the EventHubHandler.
		/// </summary>
		public BaseHubHandler() {
			MongoManager.OnDbModified -= OnDbModified;
			MongoManager.OnDbModified += OnDbModified;

			// Start a keep alive timer.
			logTimer = new Timer(30000);
			logTimer.Elapsed += new ElapsedEventHandler(KeepAlive);
			logTimer.Start();

			log.Information("BaseHubHandler created.");
		}

		/// <summary>
		/// Configuration method.
		/// </summary>
		/// <param name="configurationSettings"></param>
		public virtual void Config(IConfiguration configurationSettings) {
			configuration = configurationSettings;
			UserConnectionIds = new Dictionary<AuthenticationResponse, List<string>>();
			BannedAuthKeys = new Dictionary<string, int>();

			log.Information("BaseHubHandler has been configured!");
		}

		/// <summary>
		/// When we receive a event from DbMongoManager when the db is modified.
		/// </summary>
		/// <param name="operationType"></param>
		/// <param name="type"></param>
		/// <param name="obj"></param>
		public virtual void OnDbModified(ChangeStreamOperationType operationType, Type type, BaseType obj) { }

		/// <summary>
		/// Update the user in the cache.
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		protected Task UpdateUsers(Users user) {
			log.Debug($"{ClassName}.UpdateUsers(): called for user: {user._id}.");

			try {
				KeyValuePair<AuthenticationResponse, List<string>> pair = UserConnectionIds.FirstOrDefault(x => x.Key.User._id == user._id);

				if (!pair.Equals(default(KeyValuePair<AuthenticationResponse, List<string>>)))
					pair.Key.User = user;
			}
			catch (Exception e) {
				log.Error($"{ClassName} Failed to update user! Exception = {e.Message} - Stack = {e.StackTrace}.");
			}

			return Task.CompletedTask;
		}

		/// <summary>
		/// Add a connection Id to a user and save it in the DB.
		/// </summary>
		/// <param name="auth"></param>
		/// <param name="connectionId"></param>
		public static void AddUserConnectionId(AuthenticationResponse auth, string connectionId) {
			try {
				if (UserConnectionIds == null) {
					UserConnectionIds = new Dictionary<AuthenticationResponse, List<string>>();
					log.Warning($"{ClassName} ----- User connection list was null, newly created!");
				}

				if (UserConnectionIds.Count == 0 || UserConnectionIds.Count(x => x.Key.User._id == auth.User._id) == 0) {
					UserConnectionIds.Add(auth, new List<string>());
					log.Debug($"{ClassName} userId = [{auth.User._id}] successfully referenced into connection list.");
				}

				UserConnectionIds.First(x => x.Key.User._id == auth.User._id).Value.Add(connectionId);
			}
			catch (Exception e) {
				log.Warning($"{ClassName} Unable to add auth. Error [{e.Message}] - Stack [{e.StackTrace}].");
			}

			try {
				SaveUserConnectionIds();
			}
			catch (Exception e) {
				log.Warning($"{ClassName} Unable to add user count from database! Exception = [{e.Message}]");
			}
		}

		/// <summary>
		/// Save WebSocket user connections on hubs.
		/// </summary>
		public static void SaveUserConnectionIds() {
			Configurations configurations = MongoManager.First<Configurations>();
			SignalRServers server = configurations.SignalRServers;
			SignalRHub hub = configurations.SignalRServers.Hubs.First(x => x.Type == SignalRHubType.Events);

			hub.ConnectedClients = UserConnectionIds.Sum(x => x.Value.Count);

			configurations.SignalRServers.Hubs[configurations.SignalRServers.Hubs.IndexOf(hub)] = hub;

			UpdateDefinition<Configurations> update = Builders<Configurations>.Update.Set(x => x.SignalRServers, server);
			MongoManager.UpdateItems(new List<ObjectId>() { configurations._id }, update);
		}

		/// <summary>
		/// Remove a connectionId from a user and save it in the DB.
		/// </summary>
		/// <param name="connectionId"></param>
		public static void RemoveUserConnectionId(string connectionId) {
			AuthenticationResponse auth = UserConnectionIds.Where(x => x.Value.Contains(connectionId)).Select(x => x.Key).FirstOrDefault();

			if (auth == null)
				return;

			if (UserConnectionIds[auth].Count(x => x == connectionId) == 1)
				UserConnectionIds[auth].Remove(connectionId);

			int connectionsLeft = UserConnectionIds[auth].Count;

			log.Information($"{ClassName} ConnectionId [{connectionId}] from user [{auth.User._id}, {auth.User.Login}] disconnected, user has {connectionsLeft} connections left.");

			if (connectionsLeft == 0) {
				UserConnectionIds[auth].Clear();
				UserConnectionIds.Remove(auth);
			}

			try {
				SaveUserConnectionIds();
			}
			catch (Exception e) {
				log.Warning($"{ClassName} Unable to remove use count from database! Exception = [{e.Message}]");
			}
		}

		/// <summary>
		/// Timer event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected static void KeepAlive(object sender, ElapsedEventArgs e) => log.Information($"{ClassName} is alive. Currently, there are {UserConnectionIds.Count} connected users. userIds => {string.Concat(UserConnectionIds.Select(x => x.Key.User._id), ", ")}");
	};
}

using System;
using System.Threading.Tasks;
using Cocotte.MongoDb.Types;
using CoreLib.Managers;
using CoreLib.Types;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;

namespace Cocotte.Hubs {
	/// <summary>
	/// Class that handles all the events.
	/// </summary>
	public class EventHubHandler : BaseHubHandler<EventHubHandler> {
		readonly IHubContext<BaseHub<EventHubHandler>> hubContext;

		static EventHubHandler instance;

		/// <summary>
		/// Constructor of the EventHubHandler.
		/// </summary>
		/// <param name="hubContext"></param>
		public EventHubHandler(IHubContext<BaseHub<EventHubHandler>> hubContext) {
			if (instance == null)
				instance = this;

			this.hubContext = hubContext;

			// Start to watch database collections to get any changes.
			Task ingredientsTask = Task.Run(() => MongoManager.Watch<Ingredients>());
			Task recipesTask = Task.Run(() => MongoManager.Watch<Recipes>());
			Task weeksTask = Task.Run(() => MongoManager.Watch<Weeks>());
			Task weekIngredientsTask = Task.Run(() => MongoManager.Watch<WeekIngredients>());

			log.Information("EventHubHandler created.");
		}

		/// <summary>
		/// Send changes.
		/// </summary>
		/// <param name="operationType"></param>
		/// <param name="request"></param>
		/// <returns></returns>
		public Task SendMessage<T>(EventNotifierOperation operationType, T request) where T : BaseType {
			if (request == null)
				return Task.CompletedTask;

			log.Debug($"SendRequestMessage(id={request._id}): called with operationType = [{operationType}].");

			try {
				return hubContext.Clients.All.SendAsync(typeof(T).Name, new EventNotifierNotification<T> {
					Operation = operationType,
					Data = request
				});
			}
			catch (Exception e) {
				log.Warning($"SendRequestMessage(id={request._id}): Failed to send notification! Exception = {e.Message} - Stack = {e.StackTrace}.");
			}

			return Task.CompletedTask;
		}

		/// <summary>
		/// When we receive a event from DbMongoManager when the db is modified.
		/// </summary>
		/// <param name="operationType"></param>
		/// <param name="type"></param>
		/// <param name="obj"></param>
		public override void OnDbModified(ChangeStreamOperationType operationType, Type type, BaseType obj) {
			log.Debug("EventHubHandler.OnDbModified() called.");

			EventNotifierOperation? notifierOperationType = null;
			switch (operationType) {
				case ChangeStreamOperationType.Delete:
					notifierOperationType = EventNotifierOperation.Delete;
					break;

				case ChangeStreamOperationType.Insert:
					notifierOperationType = EventNotifierOperation.Create;
					break;

				case ChangeStreamOperationType.Update:
				case ChangeStreamOperationType.Replace:
					notifierOperationType = EventNotifierOperation.Update;
					break;
			}

			if (notifierOperationType == null)
				return;

			// Users connection updates.
			if (type == typeof(Users))
				UpdateUsers(obj as Users);

			// Events based Tasks.
			else if (type == typeof(Ingredients))
				SendMessage(notifierOperationType.Value, (Ingredients)obj);
			else if (type == typeof(Recipes))
				SendMessage(notifierOperationType.Value, (Recipes)obj);
			else if (type == typeof(Weeks))
				SendMessage(notifierOperationType.Value, (Weeks)obj);
			else if (type == typeof(WeekIngredients))
				SendMessage(notifierOperationType.Value, (WeekIngredients)obj);
		}
	};
}

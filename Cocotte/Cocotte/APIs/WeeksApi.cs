using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cocotte.MongoDb.Types;
using CoreLib.Managers;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Serilog;

namespace Cocotte.APIs {
	/// <summary>
	/// Api class that communicates directly with MongoDB, to treat the Ingredients, Recipes and Weeks.
	/// </summary>
	public static class WeeksApi {
		static readonly ILogger log = Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);
		static readonly int weeksIndex = 1;
		static readonly int weekIngredientsIndex = 2;

		static List<ObjectId> userGeneratingList = new List<ObjectId>();

		/// <summary>
		/// Generate a week of recipes. Can only generate a week if the current one is ended or about to end.
		/// </summary>
		/// <param name="userId">Id of the user.</param>
		public static void Generate(ObjectId userId) {
			if (userGeneratingList.Contains(userId))
				return;

			userGeneratingList.Add(userId);
			try {
				Users user = UsersApi.GetUser(userId);

				Weeks week = MongoManager.FirstOrDefault<Weeks>();
				if (week == null) {
					week = new Weeks();
					MongoManager.AddItemInCollection(week);
				}

				if (week != null && week.StartTime.AddDays(7) > DateTime.Now)
					return;

				// Get the number of reciepes we need.
				DateTime minimumWeeksPassed = DateTime.Now.AddDays(-user.TimeBetweenMeals);
				int countMeals = user.DayMealsSchedule.Sum(x => x.Meals.Count(y => y.Value));
				List<Recipes> recipes = MongoManager.Where<Recipes>(x => x.UserId == userId && x.Type == RecipeType.Dish && (!x.LastCooked.HasValue || (x.LastCooked.Value < minimumWeeksPassed)), 0, countMeals)
											.OrderBy(_ => Guid.NewGuid()).ToList();

				while (recipes.Count < countMeals) {
					int diffMealsCount = countMeals - recipes.Count;
					recipes.Add(MongoManager.Where<Recipes>(x => x.UserId == userId && x.Type == RecipeType.Dish && (!x.LastCooked.HasValue || (x.LastCooked.Value < minimumWeeksPassed)), 0, diffMealsCount).OrderBy(_ => Guid.NewGuid()).First());
				}

				countMeals = 0;
				week.Days = new List<Day>();

				int countDays = 0;
				for (int i = 0; i < 7; ++i) {
					week.Days.Add(new Day {
						Position = countDays,
						Date = DateTime.Now.AddDays(++countDays),
						Meals = new List<Meal>()
					});
				}

				foreach (var i in user.DayMealsSchedule) {
					int index = week.Days.FindIndex(x => x.Date.DayOfWeek == i.Day);

					foreach (var j in i.Meals) {
						if (!j.Value)
							continue;

						week.Days[index].Meals.Add(new Meal {
							NumberOfPeople = 2,
							Type = j.Key,
							RecipeId = recipes.First()._id
						});

						recipes.RemoveAt(0);
					}
				}

				MongoManager.UpdateItem(week);
				userGeneratingList.Remove(userId);
				log.Information($"Week {week._id} generated.");
			}
			catch (Exception e) {
				userGeneratingList.Remove(userId);
				log.Error($"Error on generating week: Exception = {e.Message}, Stack = {e.StackTrace}.");
			}
        }

		/// <summary>
		/// Modify the day of a week.
		/// </summary>
		/// <param name="userId">The user id.</param>
		/// <param name="day">Modified day.</param>
		/// <returns>The modified week.</returns>
		public static Weeks UpdateDay(ObjectId userId, Day day) {
            Weeks week = MongoManager.First<Weeks>(x => x.UserId == userId);
			int dayIndex = week.Days.FindIndex(x => x._id == day._id);
            week.Days[dayIndex] = day;

            MongoManager.UpdateItem(week);

            log.Information($"Week {week._id} modified.");
			return week;
        }

		/// <summary>
		/// Modify the week.
		/// </summary>
		/// <param name="week">Modified week.</param>
		/// <returns>The modified week.</returns>
		public static Weeks Update(Weeks week) {
            MongoManager.UpdateItem(week);

            log.Information($"Week {week._id} modified.");
			return week;
        }

		/// <summary>
		/// Delete a week.
		/// </summary>
		/// <param name="id">Week's id.</param>
		public static void Delete(ObjectId id) => MongoManager.DeleteItem<Weeks>(id);

		/// <summary>
		/// Update the list of all the ingredients of the week.
		/// </summary>
		/// <param name="weekIngredients"></param>
		public static void UpdateWeekIngredients(WeekIngredients weekIngredients) => MongoManager.UpdateItem(weekIngredients);

		/// <summary>
		/// Get the list of all the ingredients of the week.
		/// </summary>
		/// <param name="userId"></param>
		public static WeekIngredients GetWeekIngredients(ObjectId userId) => MongoManager.GetAggregate<WeekIngredients>(weekIngredientsIndex, x => x.UserId == userId).First();

		/// <summary>
		/// Retrieve the current week.
		/// </summary>
		/// <param name="userId">Id of the user.</param>
		/// <returns></returns>
		public static Weeks GetWeek(ObjectId userId) {
			Weeks week = MongoManager.GetAggregate<Weeks>(weeksIndex, x => x.UserId == userId).First();
			week.Days = week.Days.OrderBy(x => x.Position).ToList();
			return week;
        }
	};
}
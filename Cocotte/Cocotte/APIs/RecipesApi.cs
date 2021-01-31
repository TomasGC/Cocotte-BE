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
	public static class RecipesApi {
		static readonly ILogger log = Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);
        static readonly int recipeIndex = 0;

        /// <summary>
        /// Add the recipe.
        /// </summary>
        /// <param name="recipe">Recipe.</param>
        public static void Create(Recipes recipe) {
            if (MongoManager.ItemExists<Recipes>(x => x.Name == recipe.Name))
                throw new Exception($"There is already a reciepe [{recipe.Name}].");

            MongoManager.AddItemInCollection(recipe);

            log.Information($"Added recipe {recipe._id}.");
        }

        /// <summary>
        /// Modify the recipe.
        /// </summary>
        /// <param name="recipe">Modified recipe.</param>
        public static void Update(Recipes recipe) {
            if (!MongoManager.ItemExists<Recipes>(x => x._id == recipe._id))
                throw new Exception($"There is no recipe [{recipe._id}] - {recipe.Name}.");

            MongoManager.UpdateItem(recipe);

            log.Information($"Updated recipe {recipe._id}.");
        }

		/// <summary>
		/// Modify a list of recipes.
		/// </summary>
		/// <param name="recipes">Modified recipes.</param>
		public static void Update(List<Recipes> recipes) {
			foreach (Recipes recipe in recipes) {
				if (!MongoManager.ItemExists<Recipes>(x => x._id == recipe._id))
					throw new Exception($"There is no recipe [{recipe._id}] - {recipe.Name}.");

				MongoManager.UpdateItem(recipe);

				log.Information($"Updated recipe {recipe._id}.");
			}
        }

        /// <summary>
        /// Delete the recipe.
        /// </summary>
        /// <param name="id">Recipe's id.</param>
        public static void Delete(ObjectId id) {
            MongoManager.DeleteItem<Recipes>(id);

            List<Weeks> weeks = MongoManager.Where<Weeks>(x => x.Days.Any(y => y.Meals.Any(z => z.RecipeId == id)));
            foreach (var week in weeks) {
                foreach (var day in week.Days)
                    day.Meals.RemoveAll(x => x.RecipeId == id);

                MongoManager.UpdateItem(week);
            }

            log.Information($"Deleted recipe {id}.");
        }

		/// <summary>
		/// Retrieve all the recipes.
		/// </summary>
		/// <param name="userId">Id of the user.</param>
		/// <returns>List of the recipes.</returns>
		public static List<Recipes> GetAll(ObjectId userId) => MongoManager.GetAggregate<Recipes>(recipeIndex, x => x.UserId == userId).ToList().OrderBy(x => x.Name).ToList();

		/// <summary>
		/// Count the available recipes for a given user id.
		/// </summary>
		/// <param name="userId">Id of the user.</param>
		/// <returns>Number of available recipes.</returns>
		public static long Count(ObjectId userId) => MongoManager.Count<Recipes>(x => x.UserId == userId);
    };
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cocotte.MongoDb.Types;
using CoreLib.Managers;
using MongoDB.Bson;
using Serilog;

namespace Cocotte.APIs {
	/// <summary>
	/// Api class that communicates directly with MongoDB, to treat the Ingredients, Recipes and Weeks.
	/// </summary>
	public static class IngredientsApi {
		static readonly ILogger log = Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Add the ingredient.
        /// </summary>
        /// <param name="ingredient">The ingredient to add.</param>
        public static void Create(Ingredients ingredient) {
            if (MongoManager.ItemExists<Ingredients>(x => x.Name == ingredient.Name))
                throw new Exception($"There is already an ingredient [{ingredient.Name}] {ingredient.Unit.ToString()}.");

            MongoManager.AddItemInCollection(ingredient);

            log.Information($"Added ingredient {ingredient._id}.");
        }

        /// <summary>
        /// Modify the existing ingredient in the DB.
        /// </summary>
        /// <param name="ingredient">Modify ingredient to replace.</param>
        public static void Update(Ingredients ingredient) {
            if (!MongoManager.ItemExists<Ingredients>(x => x._id == ingredient._id))
                throw new Exception($"There is no ingredient [{ingredient._id}] - {ingredient.Name} - {ingredient.Unit.ToString()}.");

            MongoManager.UpdateItem(ingredient);

            log.Information($"Updated ingredient {ingredient._id}.");
        }

        /// <summary>
        /// Delete the ingredient.
        /// </summary>
        /// <param name="id">Id of the ingredient to remove.</param>
        public static void Delete(ObjectId id) {
            MongoManager.DeleteItem<Ingredients>(id);

            List<Recipes> recipes = MongoManager.Where<Recipes>(x => x.IngredientIds.Any(x => x.Key == id));
            foreach (var recipe in recipes) {
                recipe.IngredientIds.RemoveAll(x => x.Key == id);

                MongoManager.UpdateItem(recipe);
            }

            log.Information($"Deleted ingredient {id}.");
        }

		/// <summary>
		/// Retrieve all the ingredients.
		/// </summary>
		/// <param name="userId">Id of the user.</param>
		/// <returns>List of the ingredients.</returns>
		public static List<Ingredients> GetAll(ObjectId userId) => MongoManager.Where<Ingredients>(x => x.UserId == userId).OrderBy(x => x.Name).ToList();
	};
}
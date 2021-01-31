using System;
using System.Collections.Generic;
using CoreLib.Types;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace Cocotte.MongoDb.Types
{
	/// <summary>
	/// Type of recipe.
	/// </summary>
	public enum RecipeType {
        Starter,
        Dish,
        Dessert
    };

	/// <summary>
	/// Type of recipe.
	/// </summary>
	public enum Season {
		Automn,
        Winter,
		Spring,
		Summer
    };

    /// <summary>
    /// Used to store the infos of the recipe.
    /// </summary>
    public class Recipes : SerializationBaseType {
        /// <summary>
        /// Name of the recipe.
        /// </summary>
        public string Name { get; set; }
		/// <summary>
		/// List of the ids of the ingredients with the associated quantity needed.
		/// </summary>
		public List<KeyValuePair<ObjectId, int>> IngredientIds { get; set; } = new List<KeyValuePair<ObjectId, int>>();
		/// <summary>
		/// How many times it has been cooked.
		/// </summary>
		public int TimesCooked { get; set; }
        /// <summary>
        /// Last time it has been cooked.
        /// </summary>
        public DateTime? LastCooked { get; set; }
        /// <summary>
        /// Type of recipe.
        /// </summary>
        public RecipeType Type { get; set; }
		/// <summary>
		/// Seasons when to cook this recipe.
		/// </summary>
		public List<Season> Seasons { get; set; } = new List<Season>();
		/// <summary>
		/// The id of the user.
		/// </summary>
		public ObjectId UserId { get; set; }

		#region Not Serialized
		/// <summary>
		/// Price of the recipe, according to the price of the ingredients.
		/// </summary>
		public double Price { get; set; }
		/// <summary>
		/// List of the ingredients used for the recipe.
		/// </summary>
		public List<Ingredients> Ingredients { get; set; } = new List<Ingredients>();
        #endregion

        /// <summary>
        /// Class serialization.
        /// </summary>
        public override void Serialization() {
            log.Information("Serialization of Recipes");

            _ = BsonClassMap.RegisterClassMap<Recipes>(cm => {
                cm.AutoMap();

				cm.SetIgnoreExtraElements(true);
                _ = cm.MapMember(x => x.Ingredients).SetShouldSerializeMethod(x => false);
                _ = cm.MapMember(x => x.Price).SetShouldSerializeMethod(x => false);
            });
        }
    };
}

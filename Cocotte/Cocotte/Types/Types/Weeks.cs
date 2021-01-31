using System;
using System.Collections.Generic;
using CoreLib.Types;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace Cocotte.MongoDb.Types {
	/// <summary>
	/// Type of meals in a day.
	/// </summary>
	public enum MealType {
        Breakfast,
        Lunch,
        Dinner
    };

    /// <summary>
    /// Used to store the type of a meal and the recipe.
    /// </summary>
    public class Meal : SerializationBaseType {
        /// <summary>
        /// Type of meal.
        /// </summary>
        public MealType Type { get; set; }
        /// <summary>
        /// Id of the recipe.
        /// </summary>
        public ObjectId RecipeId { get; set; }
		/// <summary>
		/// Number of people eating the meal.
		/// </summary>
		public int NumberOfPeople { get; set; }

        #region Not Serialized
        /// <summary>
        /// Recipe itself.
        /// </summary>
        public Recipes Recipe { get; set; }
        #endregion

        /// <summary>
        /// Class serialization.
        /// </summary>
        public override void Serialization()
        {
            log.Information("Serialization of Meal");

            _ = BsonClassMap.RegisterClassMap<Meal>(cm => {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
                _ = cm.MapMember(x => x.Recipe).SetShouldSerializeMethod(x => false);
            });
        }
    };

    /// <summary>
    /// Used to store the recipes by lunch type.
    /// </summary>
    public class Day : SerializationBaseType {
        /// <summary>
        /// The position of the day.
        /// </summary>
        public int Position { get; set; }
        /// <summary>
        /// Date of the day.
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// List of all the meals in the day.
        /// </summary>
        public List<Meal> Meals { get; set; }

        #region Not Serialized
        /// <summary>
        /// Total price for all the meal in the day.
        /// </summary>
        public double Price { get; set; }
        #endregion

        /// <summary>
        /// Class serialization.
        /// </summary>
        public override void Serialization() {
            log.Information("Serialization of Day");

            _ = BsonClassMap.RegisterClassMap<Day>(cm => {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
                _ = cm.MapMember(x => x.Price).SetShouldSerializeMethod(x => false);
            });
        }
    };

    /// <summary>
    /// Used to store all the meals of the week.
    /// </summary>
    public class Weeks : SerializationBaseType {
        /// <summary>
        /// List of the days in the week.
        /// </summary>
        public List<Day> Days { get; set; }
        /// <summary>
        /// When the week starts.
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// Total price for all the meals in the week.
        /// </summary>
        public double TotalPrice { get; set; }
		/// <summary>
		/// The id of the user.
		/// </summary>
		public ObjectId UserId { get; set; }

		/// <summary>
		/// Class serialization.
		/// </summary>
		public override void Serialization() {
            log.Information("Serialization of Weeks");

            _ = BsonClassMap.RegisterClassMap<Weeks>(cm => {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
                _ = cm.MapMember(x => x.TotalPrice).SetShouldSerializeMethod(x => false);
            });
        }
    };
}
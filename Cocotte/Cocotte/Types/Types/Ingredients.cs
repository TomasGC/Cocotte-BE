using System;
using System.Collections.Generic;
using CoreLib.Types;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace Cocotte.MongoDb.Types
{
	/// <summary>
	/// The unit for the ingredients.
	/// </summary>
	public enum IngredientUnit {
		Grammes,
		Centiliters,
		Pinch,
		Slices,
        Pieces,
		Spoon,
		Teaspoon,
		None = 100
    };

    /// <summary>
    /// Used to store the ingredients for the recipes.
    /// </summary>
	[Serializable]
    public class Ingredients : SerializationBaseType {
        /// <summary>
        /// Name of the ingredient.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Base price of the ingredient.
        /// </summary>
        public double BasePrice { get; set; }
        /// <summary>
        /// Base quantity of the ingredient.
        /// </summary>
        public int BaseQuantity { get; set; }
		/// <summary>
		/// How many or much it is needed.
		/// </summary>
		public List<int> Quantities { get; set; } = new List<int>();
        /// <summary>
        /// The unit for the number of this ingredient.
        /// </summary>
        public IngredientUnit Unit { get; set; }
		/// <summary>
		/// The id of the user.
		/// </summary>
		public ObjectId UserId { get; set; }

		#region Not Serialized
		/// <summary>
		/// The selected quantity.
		/// </summary>
		public int SelectedQuantity { get; set; }
		#endregion

		/// <summary>
		/// Class serialization.
		/// </summary>
		public override void Serialization() {
			log.Information("Serialization of Ingredients");

			_ = BsonClassMap.RegisterClassMap<Ingredients>(cm => {
				cm.AutoMap();
				cm.SetIgnoreExtraElements(true);
				_ = cm.MapMember(x => x.SelectedQuantity).SetShouldSerializeMethod(x => false);
			});
		}
	};
}

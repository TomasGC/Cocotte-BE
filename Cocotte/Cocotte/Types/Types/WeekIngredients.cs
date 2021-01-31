using System.Collections.Generic;
using CoreLib.Types;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace Cocotte.MongoDb.Types {
	public class WeekIngredient : SerializationBaseType {
		public bool Checked { get; set; }

		#region Not Serialized
		public Ingredients Ingredient { get; set; }
		public int TotalQuantity { get; set; }
		#endregion

		/// <summary>
		/// Class serialization.
		/// </summary>
		public override void Serialization() {
			log.Information("Serialization of WeekIngredient");

			_ = BsonClassMap.RegisterClassMap<WeekIngredient>(cm => {
				cm.AutoMap();
				cm.SetIgnoreExtraElements(true);
				_ = cm.MapMember(x => x.Ingredient).SetShouldSerializeMethod(x => false);
				_ = cm.MapMember(x => x.TotalQuantity).SetShouldSerializeMethod(x => false);
			});
		}
	};

	public class WeekIngredients : SerializationBaseType {
		public List<WeekIngredient> Ingredients { get; set; }
		public ObjectId UserId { get; set; }

		#region Not Serialized
		public double TotalPrice { get; set; }
		#endregion

		/// <summary>
		/// Class serialization.
		/// </summary>
		public override void Serialization() {
			log.Information("Serialization of WeekIngredients");

			_ = BsonClassMap.RegisterClassMap<WeekIngredients>(cm => {
				cm.AutoMap();
				cm.SetIgnoreExtraElements(true);
				_ = cm.MapMember(x => x.TotalPrice).SetShouldSerializeMethod(x => false);
			});
		}
	};
}

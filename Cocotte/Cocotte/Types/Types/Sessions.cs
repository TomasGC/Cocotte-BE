using System;
using CoreLib.Types;
using MongoDB.Bson;

namespace Cocotte.MongoDb.Types {
	/// <summary>
	/// Used to store connection keys.
	/// </summary>
	public class Sessions : BaseType {
		/// <summary>
		/// Key.
		/// </summary>
		public string Key { get; set; }
		/// <summary>
		/// UserId.
		/// </summary>
		public ObjectId UserId { get; set; }
		/// <summary>
		/// CreationDate.
		/// </summary>
		public DateTime CreationDate { get; set; }
	};
}

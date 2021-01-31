using System.Collections.Generic;
using Cocotte.MongoDb.Types;
using CoreLib.Types;

namespace Cocotte.Types.Responses {
	/// <summary>
	/// Response for a list of ingredients.
	/// </summary>
	public class GetIngredientsResponse : BaseResponse {
		/// <summary>
		/// The requested ingredients.
		/// </summary>
		public List<Ingredients> Ingredients { get; set; }
	};
}

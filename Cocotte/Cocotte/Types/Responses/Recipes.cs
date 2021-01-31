using System.Collections.Generic;
using Cocotte.MongoDb.Types;
using CoreLib.Types;

namespace Cocotte.Types.Responses {
	/// <summary>
	/// Response for a list of recipes.
	/// </summary>
	public class GetRecipesResponse : BaseResponse {
		/// <summary>
		/// The requested recipes.
		/// </summary>
		public List<Recipes> Recipes { get; set; }
	};
}

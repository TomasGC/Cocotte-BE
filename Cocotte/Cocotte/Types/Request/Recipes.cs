using System.Collections.Generic;
using Cocotte.MongoDb.Types;
using CoreLib.Types;

namespace Common.Request {
	public class UpdateRecipesRequest : BaseRequest {
		public List<Recipes> Recipes { get; set; }
	};
}

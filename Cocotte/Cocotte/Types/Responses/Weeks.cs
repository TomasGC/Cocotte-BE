using Cocotte.MongoDb.Types;
using CoreLib.Types;

namespace Cocotte.Types.Responses {
	/// <summary>
	/// Response for a week.
	/// </summary>
	public class GetWeek : BaseResponse {
		/// <summary>
		/// The requested week.
		/// </summary>
		public Weeks Week { get; set; }
	};

	/// <summary>
	/// Response for a week ingredients.
	/// </summary>
	public class GetWeekIngredients : BaseResponse {
		/// <summary>
		/// The requested week ingredients.
		/// </summary>
		public WeekIngredients WeekIngredients { get; set; }
	};
}

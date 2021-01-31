using Cocotte.MongoDb.Types;
using CoreLib.Types;

namespace Common.Request {
	public class UpdateDayRequest : BaseRequest {
		public Day Day { get; set; }
	};
}

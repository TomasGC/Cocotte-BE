using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cocotte.Core {
	public sealed class Settings {
		public static string ApiName { get; set; }
		public static string ApiVersion { get; set; }
		public static string SwaggerBaseUrl { get; set; }
	};
}

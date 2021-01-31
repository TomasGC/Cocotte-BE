using System;
using System.Collections.Generic;
using CoreLib.Types;

namespace Cocotte.MongoDb.Types {
	/// <summary>
	/// Type of SignalR hubs.
	/// </summary>
	public enum	SignalRHubType {
		Events
	};

	/// <summary>
	/// SignalR Hub class.
	/// </summary>
	public class SignalRHub {
		/// <summary>
		/// Type of hub.
		/// </summary>
		public SignalRHubType Type { get; set; }
		/// <summary>
		/// Name of the Hub.
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Route for the hub.
		/// </summary>
		public string Route { get; set; }
		/// <summary>
		/// How many clients are connected.
		/// </summary>
		public long ConnectedClients { get; set; }
	};

	/// <summary>
	/// Class to Register the config of the SignalR Servers.
	/// </summary>
	public class SignalRServers {
		/// <summary>
		/// Hostname.
		/// </summary>
		public string HostName { get; set; }
		/// <summary>
		/// Public uri.
		/// </summary>
		public string PublicUri { get; set; }
		/// <summary>
		/// When the server started.
		/// </summary>
		public DateTime StartedAt { get; set; }
		/// <summary>
		/// List of hubs attached to the server.
		/// </summary>
		public List<SignalRHub> Hubs { get; set; }
	};

	/// <summary>
	/// The default colors for the UI.
	/// </summary>
	public class UIColors {
		/// <summary>
		/// Color of the Header.
		/// </summary>
		public string Headers { get; set; }
		/// <summary>
		/// Color of the Background.
		/// </summary>
		public string Backgrounds { get; set; }
		/// <summary>
		/// Color of the Icons.
		/// </summary>
		public string Icons { get; set; }
		/// <summary>
		/// Color of the Texts.
		/// </summary>
		public string Texts { get; set; }
		/// <summary>
		/// Color of the Main Menu Buttons.
		/// </summary>
		public string MainMenuButtons { get; set; }
		/// <summary>
		/// Color of the Tabs.
		/// </summary>
		public string Tabs { get; set; }
		/// <summary>
		/// Color of the Exit Buttons.
		/// </summary>
		public string ExitButtons { get; set; }
		/// <summary>
		/// Color of the Validate Buttons.
		/// </summary>
		public string ValidateButtons { get; set; }
		/// <summary>
		/// Color of the Inputs.
		/// </summary>
		public string Inputs { get; set; }
		/// <summary>
		/// Color of the Modify Buttons.
		/// </summary>
		public string ModifyButtons { get; set; }
		/// <summary>
		/// Color of the Delete Buttons.
		/// </summary>
		public string DeleteButtons { get; set; }
		/// <summary>
		/// Color of the Add Buttons.
		/// </summary>
		public string AddButtons { get; set; }
		/// <summary>
		/// Color of the Checks.
		/// </summary>
		public string Checks { get; set; }
	};

	/// <summary>
	/// Class containing the configurations for the applications.
	/// </summary>
	public class Configurations : BaseType {
		/// <summary>
		/// Configs for Signal R.
		/// </summary>
		public SignalRServers SignalRServers { get; set; }
		/// <summary>
		/// The default colors of the UI.
		/// </summary>
		public UIColors DefaultUIColors { get; set; }
	};
}

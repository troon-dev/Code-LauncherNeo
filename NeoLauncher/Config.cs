using System;
using System.IO;

namespace NeoLauncher;

public static class Config
{
	public const string BackendScheme = "https";

	public const string BackendHost = "neofn.dev";

	public const string BackendUri = "https://neofn.dev";

	public const string XmppUri = "wss://xmpp-service-prod.neofn.dev";

	public const string XmppDomain = "xmpp-service-prod.neofn.dev";

	public static readonly string DataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NeoLauncherData");
}

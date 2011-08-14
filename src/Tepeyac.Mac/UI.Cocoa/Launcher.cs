using MonoMac.AppKit;
using MonoMac.Foundation;

namespace Tepeyac.UI.Cocoa
{
	public class Launcher : ILauncher
	{
		public void Launch(string url)
		{
			NSWorkspace.SharedWorkspace.OpenUrl(NSUrl.FromString(url));
		}
	}
}


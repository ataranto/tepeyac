using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Tepeyac.UI.MonoTouch
{
	public class Launcher : ILauncher
	{
		public void Launch(string url)
		{
			UIApplication.SharedApplication.OpenUrl(NSUrl.FromString(url));
		}
	}
}


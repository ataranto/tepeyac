using MonoMac.AppKit;
using MonoMac.Foundation;
using Ninject;

namespace Tepeyac.Mac
{
	public partial class AppDelegate : NSApplicationDelegate
	{
		public override void FinishedLaunching (NSObject notification)
		{
			var kernel = new StandardKernel(
				new Tepeyac.Core.Module(),
			    //new Tepeyac.Core.Mac.Module(),
			    new Tepeyac.UI.Module(),
			    new Tepeyac.UI.Cocoa.Module()
			);
			                  
			kernel.Get<Tepeyac.UI.MainPresenter>();
		}
	}
}
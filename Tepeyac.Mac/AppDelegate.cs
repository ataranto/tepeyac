using MonoMac.AppKit;
using MonoMac.Foundation;
using Ninject;

namespace Tepeyac.Mac
{
	public partial class AppDelegate : NSApplicationDelegate
	{
		MainWindowController mainWindowController;

		public override void FinishedLaunching (NSObject notification)
		{
			mainWindowController = new MainWindowController ();
			mainWindowController.Window.MakeKeyAndOrderFront (this);
						
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


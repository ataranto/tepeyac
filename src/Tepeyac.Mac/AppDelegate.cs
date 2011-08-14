using MonoMac.AppKit;
using MonoMac.Foundation;
using Ninject;
using Tepeyac.Mac.Integration;

namespace Tepeyac.Mac
{
	public partial class AppDelegate : NSApplicationDelegate
	{
		public override void FinishedLaunching (NSObject notification)
		{
			try
			{
				LSSharedFileList.InsertLoginItem(NSBundle.MainBundle.BundlePath);
			}
			catch 	
			{
				
			}
			
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
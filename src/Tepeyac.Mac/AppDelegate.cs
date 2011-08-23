using Funq;
using MonoMac.AppKit;
using MonoMac.Foundation;
using Retlang.Core;
using Retlang.Fibers;
using Tepeyac.Core;
using Tepeyac.Mac.Integration;
using Tepeyac.UI;
using Tepeyac.UI.Cocoa;

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

			var container = new Container();
			Tepeyac.Funq.Registry.Register(container);
			
			container.Register<IFiber>("GuiFiber", c =>
			{
				var executor =
					c.Resolve<IExecutor>() ??
					new Executor();
				var fiber = new CocoaFiber(executor);
				fiber.Start();
				
				return fiber;
			});

			container.Register<ILauncher>(c =>
				new Tepeyac.UI.Cocoa.Launcher());
			container.Register<IBurritoDayView>(c =>
				new StatusItemBurritoDayView(c));
			
			container.Resolve<IBurritoDayView>();
		}
	}
}
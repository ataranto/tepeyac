using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Retlang.Core;
using Retlang.Fibers;
using Tepeyac.Core;
using Tepeyac.iPhone.UI;
using Tepeyac.UI.MonoTouch;

namespace Tepeyac.iPhone
{
	public class Application
	{
		static void Main (string[] args)
		{
			UIApplication.Main (args);
		}
	}
	
	// The name AppDelegate is referenced in the MainWindow.xib file.
	public partial class AppDelegate : UIApplicationDelegate
	{
		// This method is invoked when the application has loaded its UI and its ready to run
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			var container = new Funq.Container();
			Tepeyac.Core.Registry.Register(container);
			
			container.Register<IFiber>("GuiFiber", c =>
			{
				var executor =
					c.Resolve<IExecutor>() ??
					new Executor();
				var fiber = new MonoTouchFiber(executor);
				fiber.Start();
				
				return fiber;
			});

			var view = new BurritoDayView(container);
			window.AddSubview(view.View);
			window.MakeKeyAndVisible();
	
			return true;
		}
	
		// This method is required in iPhoneOS 3.0
		public override void OnActivated (UIApplication application)
		{
			
		}
	}
}


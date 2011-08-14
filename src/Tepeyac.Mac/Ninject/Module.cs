using Ninject.Modules;
using Retlang.Fibers;
using Tepeyac.Core;
using Tepeyac.UI;
using Tepeyac.UI.Cocoa;

namespace Tepeyac.Mac.Ninject
{
	public class Module : NinjectModule
	{
		public override void Load()
		{
			var guiFiber = new CocoaFiber(new Executor());
			guiFiber.Start();
			this.Bind<IFiber>().ToConstant(guiFiber).
				WhenTargetHas<GuiFiberAttribute>();
			
			this.Rebind<ILauncher>().To<Tepeyac.UI.Cocoa.Launcher>().InSingletonScope();
			this.Bind<IBurritoDayView>().To<StatusItemBurritoDayView>().InSingletonScope();
		}
	}
}
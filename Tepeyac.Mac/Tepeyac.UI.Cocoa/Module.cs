using Ninject.Modules;
using Tepeyac.Core;
using Retlang.Fibers;

namespace Tepeyac.UI.Cocoa
{
	public class Module : NinjectModule
	{
		public override void Load()
		{
			var guiFiber = new CocoaFiber(new Executor());
			guiFiber.Start();
			this.Bind<IFiber>().ToConstant(guiFiber).
				WhenTargetHas<GuiFiberAttribute>();
			
			this.Rebind<ILauncher>().To<Launcher>().InSingletonScope();
			
			this.Bind<IBurritoDayView>().To<StatusItemBurritoDayView>().InSingletonScope();
		}
	}
}
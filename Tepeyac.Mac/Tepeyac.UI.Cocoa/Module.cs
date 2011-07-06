using Ninject.Modules;

namespace Tepeyac.UI.Cocoa
{
	public class Module : NinjectModule
	{
		public override void Load()
		{
			this.Bind<IBurritoDayView>().To<StatusItemBurritoDayView>().InSingletonScope();	
		}
	}
}


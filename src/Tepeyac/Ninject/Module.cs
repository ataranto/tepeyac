using Ninject.Modules;
using Retlang.Fibers;
using Tepeyac.Core;
using Tepeyac.UI;

namespace Tepeyac.Ninject
{
	public class Module : NinjectModule
	{
		public override void Load()
		{
			var executor = new Executor();
			base.Bind<IFiber>().ToMethod(c =>
			{
				var fiber = new PoolFiber(executor);
				fiber.Start();
				
				return fiber;
			});
			
			base.Bind<IWebClient>().To<WebClient>();
			base.Bind<IBurritoDayModel>().To<BurritoDayModel>().InSingletonScope();
			
			base.Bind<ILauncher>().To<Launcher>().InSingletonScope();
		}
	}
}


using System;
using Ninject.Modules;
using Retlang.Fibers;

namespace Tepeyac.Core
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
		}
	}
}


using Funq;
using Retlang.Fibers;
using Retlang.Core;
using Tepeyac.UI;

namespace Tepeyac.Core
{
	public static class Registry
	{
		public static void Register(Container container)
		{
			var executor = new Executor();
			
			container.Register<IExecutor>(c =>
				executor);
			container.Register<IFiber>(c =>
			{
				var fiber = new PoolFiber(executor);
				fiber.Start();
				
				return fiber;
			}).ReusedWithin(ReuseScope.None);
			
			container.Register<IWebClient>(c =>
				new WebClient()).
				ReusedWithin(ReuseScope.None);
			container.Register<ILauncher>(c =>
				new Launcher());
			container.Register<IBurritoDayModel>(c =>
			{
				var fiber = c.Resolve<IFiber>();
				var client = c.Resolve<IWebClient>();
				
				return new BurritoDayModel(fiber, client);
			});

			container.Register<BurritoDayPresenter, IBurritoDayView>((c, view) =>
			{
				var model = c.Resolve<IBurritoDayModel>();
				var fiber = c.ResolveNamed<IFiber>("GuiFiber");
				
				return new BurritoDayPresenter(model, view, fiber);
			});
			
			container.Register<UrlActivationPresenter, IUrlActivationView>((c, view) =>
			{
				var model = c.Resolve<ILauncher>();
				
				return new UrlActivationPresenter(model, view);
			});
		}
	}
}
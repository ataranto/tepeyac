using Ninject.Modules;

namespace Tepeyac.UI
{
	public class Module : NinjectModule
	{
		public override void Load()
		{
			base.Bind<ILauncher>().To<Launcher>().InSingletonScope();
		}
	}
}


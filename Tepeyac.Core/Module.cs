using System;
using Ninject.Modules;

namespace Tepeyac.Core
{
	public class Module : NinjectModule
	{
		public override void Load()
		{
			this.Bind<IPlatform>().To<Platform>().InSingletonScope();	
		}
	}
}


using Ninject;

namespace Tepeyac.Mac
{
	class MainClass
	{
		static void Main (string[] args)
		{
			var kernel = new StandardKernel(
				new Tepeyac.Core.Module(),
			    //new Tepeyac.Core.Mac.Module(),
			    new Tepeyac.UI.Module(),
			    new Tepeyac.UI.Cocoa.Module()
			);
			                  
			kernel.Get<Tepeyac.UI.MainPresenter>();
			
			/*
			NSApplication.Init ();
			NSApplication.Main (args);
			*/
		}
	}
}


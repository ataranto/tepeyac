using MonoMac.AppKit;

namespace Tepeyac.Mac
{
	class MainClass
	{
		static void Main (string[] args)
		{
			NSApplication.Init();
			NSApplication.Main(args);
		}
	}
}


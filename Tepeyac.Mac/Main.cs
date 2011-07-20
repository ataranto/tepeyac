using System;
using System.IO;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;

namespace Tepeyac.Mac
{
	class MainClass
	{
		static void Main (string[] args)
		{
			var contents = AppDomain.CurrentDomain.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar);
			var sparkle = Path.Combine(Directory.GetParent(contents).ToString(), "Frameworks", "Sparkle.framework", "Sparkle");
			Dlfcn.dlopen(sparkle, 0);
			
			NSApplication.Init();
			NSApplication.Main(args);
		}
	}
}


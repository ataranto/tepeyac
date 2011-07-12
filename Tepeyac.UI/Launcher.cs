using System.Diagnostics;

namespace Tepeyac.UI
{
	public class Launcher : ILauncher
	{
		public void Launch(string url)
		{
			Process.Start(url);
		}
	}
}


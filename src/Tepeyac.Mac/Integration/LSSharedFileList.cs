using System.Runtime.InteropServices;

namespace Tepeyac.Mac.Integration
{
	public static class LSSharedFileList
	{
		private const string path = "@executable_path/../Resources/libIntegration.dylib";
		
		[DllImport(path)]
		public static extern void InsertLoginItem(string path);
	}
}


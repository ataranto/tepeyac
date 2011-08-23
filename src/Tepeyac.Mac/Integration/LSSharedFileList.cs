using System.Runtime.InteropServices;

namespace Tepeyac.Mac.Integration
{
	public static class LSSharedFileList
	{
		[DllImport("Integration")]
		public static extern void InsertLoginItem(string path);
	}
}
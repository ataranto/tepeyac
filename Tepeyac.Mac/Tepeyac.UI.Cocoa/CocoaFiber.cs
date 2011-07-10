using MonoMac.Foundation;
using Retlang.Fibers;
using Retlang.Core;

namespace Tepeyac.UI.Cocoa
{
	public class CocoaFiber : GuiFiber
	{
		public CocoaFiber (IExecutor executor)
			: base(new CocoaAdapter(), executor)
		{
			
		}
	}
}


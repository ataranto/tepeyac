using System;
using Retlang.Fibers;
using Retlang.Core;

namespace Tepeyac.UI.MonoTouch
{
	public class MonoTouchFiber : GuiFiber
	{
		public MonoTouchFiber(IExecutor executor)
			: base(new MonoTouchAdapter(), executor)
		{
			
		}
	}
}


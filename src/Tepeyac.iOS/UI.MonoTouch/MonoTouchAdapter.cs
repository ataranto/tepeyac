using System;
using MonoTouch.Foundation;
using Retlang.Core;

namespace Tepeyac.UI.MonoTouch
{
	public class MonoTouchAdapter : NSObject, IExecutionContext
	{
		public void Enqueue(Action action)
		{
			base.BeginInvokeOnMainThread(() => action());
		}
	}
}


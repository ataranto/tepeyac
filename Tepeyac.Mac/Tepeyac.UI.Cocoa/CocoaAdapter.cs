using System;
using MonoMac.Foundation;
using Retlang.Core;

namespace Tepeyac.UI.Cocoa
{
	public class CocoaAdapter : NSObject, IExecutionContext
	{
		public void Enqueue(Action action)
		{
			base.BeginInvokeOnMainThread(() => action());
		}
	}
}


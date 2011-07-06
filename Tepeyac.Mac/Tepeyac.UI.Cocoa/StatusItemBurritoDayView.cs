using System;
using MonoMac.AppKit;
using Ninject;
using Tepeyac.Core;
using Ninject.Parameters;

namespace Tepeyac.UI.Cocoa
{
	public class StatusItemBurritoDayView : IBurritoDayView
	{
		private readonly NSStatusItem si;
		private readonly IDisposable presenter;
		
		public StatusItemBurritoDayView (IKernel kernel)
		{
			this.si = NSStatusBar.SystemStatusBar.CreateStatusItem(28);
			this.si.HighlightMode = true;
			
			var parameter = new ConstructorArgument("view", this);
			this.presenter = kernel.Get<BurritoDayPresenter>(parameter);
		}
		
		public void Dispose()
		{
			
		}
		
		void IBurritoDayView.SetState(BurritoDayState state)
		{
			System.Console.WriteLine("state: {0}", state);
		}
	}
}


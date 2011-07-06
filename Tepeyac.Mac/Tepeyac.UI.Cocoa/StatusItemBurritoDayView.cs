using System;
using System.Drawing;
using MonoMac.AppKit;
using MonoMac.Foundation;
using Ninject;
using Ninject.Parameters;
using Tepeyac.Core;

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
			this.presenter.Dispose();
		}
		
		void IBurritoDayView.SetState(BurritoDayState state)
		{
			string name = state.ToString().ToLower();
			var path = NSBundle.MainBundle.PathForResource(name, "png") ??
				NSBundle.MainBundle.PathForResource("no", "png");

			this.si.Image = new NSImage(path);
			this.si.Image.Size = new SizeF(16, 16);
		}
	}
}


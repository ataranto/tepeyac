using System;
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
		
		private readonly NSMenuItem RefreshMenuItem = new NSMenuItem("Refresh");
		private readonly NSMenuItem LaunchMenuItem = new NSMenuItem("Launch Burrito Website");
		private readonly NSMenuItem QuitMenuItem = new NSMenuItem("Quit Tepeyac");
		
		public StatusItemBurritoDayView (IKernel kernel)
		{
			this.QuitMenuItem.Activated += (sender, e) => NSApplication.SharedApplication.Terminate(sender as NSObject);
			
			this.si = NSStatusBar.SystemStatusBar.CreateStatusItem(28);
			this.si.HighlightMode = true;
			this.si.Menu = new NSMenu();
			
			this.si.Menu.AddItem(this.RefreshMenuItem);
			this.si.Menu.AddItem(this.LaunchMenuItem);
			this.si.Menu.AddItem(NSMenuItem.SeparatorItem);
			this.si.Menu.AddItem(this.QuitMenuItem);
			
			var parameter = new ConstructorArgument("view", this);
			this.presenter = kernel.Get<BurritoDayPresenter>(parameter);
		}
		
		public void Dispose()
		{
			this.presenter.Dispose();
		}
		
		#region IBurritoDayView
		
		void IBurritoDayView.SetState(BurritoDayState state)
		{
			var name = state.ToString().ToLower();
			var path = NSBundle.MainBundle.PathForResource(name, "png") ??
				NSBundle.MainBundle.PathForResource("no", "png");

			this.si.Image = new NSImage(path);
		}
		
		#endregion
	}
}


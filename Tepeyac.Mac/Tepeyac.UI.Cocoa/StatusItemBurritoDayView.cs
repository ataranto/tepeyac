using System;
using System.Collections.Generic;
using MonoMac.AppKit;
using MonoMac.Foundation;
using Ninject;
using Ninject.Parameters;
using Tepeyac.Core;

namespace Tepeyac.UI.Cocoa
{
	public class StatusItemBurritoDayView : IBurritoDayView, IUrlActivationView, IActivationView<IPreferencesView>
	{
		private readonly NSStatusItem si;
		private readonly ICollection<IDisposable> presenters;
		
		private readonly NSMenuItem RefreshMenuItem = new NSMenuItem("Refresh");
		private readonly NSMenuItem LaunchMenuItem = new NSMenuItem("Launch Burrito Website");
		private readonly NSMenuItem PreferencesMenuItem = new NSMenuItem("Preferences...");
		private readonly NSMenuItem QuitMenuItem = new NSMenuItem("Quit Tepeyac");
		
		public StatusItemBurritoDayView (IKernel kernel)
		{
			this.LaunchMenuItem.Activated += delegate {
				var handler = this.urlActivated;
				if (handler != null)
				{
					handler(this, "http://isitburritoday.com");
				}
			};
			this.QuitMenuItem.Activated += (sender, e) => NSApplication.SharedApplication.Terminate(sender as NSObject);
			
			this.si = NSStatusBar.SystemStatusBar.CreateStatusItem(28);
			this.si.HighlightMode = true;
			this.si.Menu = new NSMenu();
			
			this.si.Menu.AddItem(this.RefreshMenuItem);
			this.si.Menu.AddItem(this.LaunchMenuItem);
			this.si.Menu.AddItem(NSMenuItem.SeparatorItem);
			this.si.Menu.AddItem(this.PreferencesMenuItem);
			this.si.Menu.AddItem(NSMenuItem.SeparatorItem);
			this.si.Menu.AddItem(this.QuitMenuItem);
			
			var parameter = new ConstructorArgument("view", this);
			this.presenters = new IDisposable[]
			{
				kernel.Get<BurritoDayPresenter>(parameter),
				kernel.Get<UrlActivationPresenter>(parameter),
			};
		}
		
		public void Dispose()
		{
			foreach (var presenter in this.presenters)
			{
				presenter.Dispose();
			}
		}
		
		#region IBurritoDayView
		
		event EventHandler IBurritoDayView.RefreshActivated
		{
			add { this.RefreshMenuItem.Activated += value; }
			remove { this.RefreshMenuItem.Activated -= value; }
		}
		
		void IBurritoDayView.SetState(BurritoDayState state)
		{
			var name = state.ToString().ToLower();
			var path = NSBundle.MainBundle.PathForResource(name, "png") ??
				NSBundle.MainBundle.PathForResource("no", "png");

			this.si.Image = new NSImage(path);
		}
		
		#endregion
		
		#region IUrlActivationView
		
		private event Action<object, string> urlActivated;
		event Action<object, string> IUrlActivationView.Activated
		{
			add { this.urlActivated += value; }
			remove { this.urlActivated -= value; }
		}
		
		#endregion
		
		#region IActivationView<IPreferencesView>
		
		event EventHandler IActivationView<IPreferencesView>.Activated
		{
			add {  this.PreferencesMenuItem.Activated += value; }
			remove { this.PreferencesMenuItem.Activated -= value; }
		}
		
		#endregion
	}
}


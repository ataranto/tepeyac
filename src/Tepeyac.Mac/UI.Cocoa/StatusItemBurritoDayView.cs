using System;
using System.Collections.Generic;
using MonoMac.AppKit;
using MonoMac.Foundation;
using Ninject;
using Ninject.Parameters;
using Tepeyac.Core;

namespace Tepeyac.UI.Cocoa
{
	public class StatusItemBurritoDayView : IBurritoDayView, IUrlActivationView
	{
		private NSStatusItem si;
		private readonly ICollection<IDisposable> presenters;
		
		private readonly NSMenu menu = new NSMenu();
		private readonly NSMenuItem refreshMenuItem = new NSMenuItem("Refresh");
		private readonly NSMenuItem launchMenuItem = new NSMenuItem("Launch Burrito Website");
		private readonly NSMenuItem dismissMenuitem = new NSMenuItem("Dismiss");
		private readonly NSMenuItem quitMenuItem = new NSMenuItem("Quit Tepeyac");
		
		public StatusItemBurritoDayView (IKernel kernel)
		{
			this.menu = new NSMenu();
			
			this.menu.AddItem(this.refreshMenuItem);
			this.menu.AddItem(this.launchMenuItem);
			this.menu.AddItem(NSMenuItem.SeparatorItem);
			this.menu.AddItem(this.dismissMenuitem);
			//this.Menu.AddItem(this.preferencesMenuItem);
			this.menu.AddItem(NSMenuItem.SeparatorItem);
			this.menu.AddItem(this.quitMenuItem);
			
			this.launchMenuItem.Activated += delegate {
				var handler = this.urlActivated;
				if (handler != null)
				{
					handler(this, "http://isitburritoday.com");
				}
			};
			this.quitMenuItem.Activated += (sender, e) =>
				NSApplication.SharedApplication.Terminate(sender as NSObject);
			
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
		
		private void InstantiateStatusItem ()
		{
			this.si = NSStatusBar.SystemStatusBar.CreateStatusItem(28);
			this.si.HighlightMode = true;
			this.si.Menu = this.menu;
		}
		
		#region IView
		
		bool IView.Visible
		{
			get { return this.si != null; }
			
			set
			{
				if (!value && this.si != null)
				{
					this.si.Dispose();
					this.si = null;
				}
				else if (value && this.si == null)
				{
					this.InstantiateStatusItem();		
				}
			}
		}
		
		#endregion
		
		#region IBurritoDayView
		
		event EventHandler IBurritoDayView.RefreshActivated
		{
			add { this.refreshMenuItem.Activated += value; }
			remove { this.refreshMenuItem.Activated -= value; }
		}
		
		event EventHandler IBurritoDayView.DismissActivated
		{
			add { this.dismissMenuitem.Activated += value; }
			remove { this.dismissMenuitem.Activated -= value; }
		}
		
		void IBurritoDayView.SetState(BurritoDayState state, string description)
		{
			if (this.si == null)
			{
				return;
			}
			
			var name = state.ToString().ToLower();
			var path = NSBundle.MainBundle.PathForResource(name, "png") ??
				NSBundle.MainBundle.PathForResource("no", "png");

			this.si.Image = new NSImage(path);
			this.si.ToolTip = description;
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
	}
}
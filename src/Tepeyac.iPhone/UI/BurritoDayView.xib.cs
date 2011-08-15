using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Tepeyac.Core;
using Tepeyac.UI;
using Funq;
using System.Collections.Generic;

namespace Tepeyac.iPhone.UI
{
	public partial class BurritoDayView : UIViewController, IBurritoDayView, IUrlActivationView
	{
		private readonly Container container;
		private ICollection<IDisposable> presenters;

		#region Constructors
		
		// The IntPtr and initWithCoder constructors are required for items that need 
		// to be able to be created from a xib rather than from managed code
		
		public BurritoDayView (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		[Export ("initWithCoder:")]
		public BurritoDayView (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		public BurritoDayView (Container container) : base ("BurritoDayView", null)
		{
			this.container = container;
			Initialize ();
		}
		
		void Initialize ()
		{
			if (this.container == null)
			{	
				return;
			}
			
			this.presenters = new IDisposable[]
			{
				this.container.Resolve<BurritoDayPresenter, IBurritoDayView>(this),
				this.container.Resolve<UrlActivationPresenter, IUrlActivationView>(this),
			};
		}
		
		#endregion
		
		#region IView
		
		bool IView.Visible
		{
			get { return true; }
			set { }
		}
		
		#endregion
		
		#region IBurritoDayView
		
		private event EventHandler refreshActivated;
		event EventHandler IBurritoDayView.RefreshActivated
		{
			add { this.refreshActivated += value; }
			remove { this.refreshActivated -= value; }
		}
		
		event EventHandler IBurritoDayView.DismissActivated
		{
			add { }
			remove { }
		}
		
		void IBurritoDayView.SetState(BurritoDayState state, string description)
		{
			Console.WriteLine("set state: {0}", state.ToString());
			this.label.Text = state.ToString();
		}
		
		#endregion
		
		#region IUrlActivationView
		
		event Action<object, string> IUrlActivationView.Activated
		{
			add { }
			remove { }
		}
		
		#endregion
		
		
	}
}


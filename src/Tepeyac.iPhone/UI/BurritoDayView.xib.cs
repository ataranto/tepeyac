using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Tepeyac.Core;
using Tepeyac.UI;
using Funq;

namespace Tepeyac.iPhone.UI
{
	public partial class BurritoDayView : UIViewController, IBurritoDayView, IUrlActivationView
	{
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
			Initialize ();
		}
		
		void Initialize ()
		{
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
		
		event EventHandler IBurritoDayView.RefreshActivated
		{
			add { }
			remove { }
		}
		
		event EventHandler IBurritoDayView.DismissActivated
		{
			add { }
			remove { }
		}
		
		void IBurritoDayView.SetState(BurritoDayState state, string description)
		{
			
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


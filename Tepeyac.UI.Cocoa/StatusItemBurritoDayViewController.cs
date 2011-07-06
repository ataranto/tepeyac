
using System;
using MonoMac.AppKit;
using MonoMac.Foundation;
using Ninject;
using Tepeyac.Core;

namespace Tepeyac.UI.Cocoa
{
	public partial class StatusItemBurritoDayViewController : NSViewController, IBurritoDayView
	{
		#region Constructors

		// Called when created from unmanaged code
		public StatusItemBurritoDayViewController (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export("initWithCoder:")]
		public StatusItemBurritoDayViewController (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		// Call to load from the XIB/NIB file
		[Inject]
		public StatusItemBurritoDayViewController () : base("StatusItemBurritoDayView", NSBundle.MainBundle)
		{
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
			
		}

		#endregion
		
		/*
		//strongly typed view accessor
		public new StatusItemBurritoDayView View {
			get { return (StatusItemBurritoDayView)base.View; }
		}
		*/
		
		#region IBurritoDayView
		
		void IBurritoDayView.SetState(BurritoDayState state)
		{
			
		}
		
		#endregion
	}
}


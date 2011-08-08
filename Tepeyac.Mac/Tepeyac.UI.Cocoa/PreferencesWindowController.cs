using System;
using MonoMac.AppKit;
using MonoMac.Foundation;
using Ninject;

namespace Tepeyac.UI.Cocoa
{
	public partial class PreferencesWindowController : NSWindowController, IPreferencesView
	{
		#region Constructors
		
		// Called when created from unmanaged code
		public PreferencesWindowController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public PreferencesWindowController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		[Inject]
		public PreferencesWindowController () : base ("PreferencesWindow")
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}
		
		#endregion
		
		//strongly typed window accessor
		public new PreferencesWindow Window {
			get {
				return (PreferencesWindow)base.Window;
			}
		}
		
		#region IView
		
		bool IView.Visible
		{
			get { return this.Window.IsVisible; }
			set { this.Window.IsVisible = value; }
		}
		
		#endregion
	}
}


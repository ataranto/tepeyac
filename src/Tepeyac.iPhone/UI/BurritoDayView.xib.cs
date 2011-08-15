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
			
			base.BeginInvokeOnMainThread(() =>
			{
				this.launchButton.TouchUpInside += delegate {
					var handler = this.urlActivated;
					if (handler != null)
					{
						handler(this, "http://isitburritoday.com");
					}
				};
			
				this.presenters = new IDisposable[]
				{
					this.container.Resolve<BurritoDayPresenter, IBurritoDayView>(this),
					this.container.Resolve<UrlActivationPresenter, IUrlActivationView>(this),
				};
			});
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
			add { this.refreshButton.TouchUpInside += value; }
			remove { this.refreshButton.TouchUpInside -= value; }
		}
		
		event EventHandler IBurritoDayView.DismissActivated
		{
			add { }
			remove { }
		}
		
		void IBurritoDayView.SetState(BurritoDayState state, string description)
		{
			var name = "Images/" + state.ToString().ToLower() + ".png";
			var image =
				UIImage.FromFile(name) ??
				UIImage.FromFile("Images/no.png");
			
			this.image.Image = image;
			this.label.Text = description;
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


using System;
using Retlang.Fibers;
using Tepeyac.Core;

namespace Tepeyac.UI
{
	public class BurritoDayPresenter : GuiPresenter<IBurritoDayModel, IBurritoDayView>
	{
		public BurritoDayPresenter(IBurritoDayModel model, IBurritoDayView view, IFiber guiFiber)
			: base(model, view, guiFiber)
		{
			base.model.Changed += this.OnModelChanged;
			base.view.RefreshActivated += this.OnViewRefreshActivated;
			base.view.DismissActivated += this.OnViewDismissActivated;
			
			this.ViewUpdate();
		}
		
		public override void Dispose()
		{
			base.model.Changed -= this.OnModelChanged;
			base.view.RefreshActivated -= this.OnViewRefreshActivated;
			this.view.DismissActivated -= this.OnViewDismissActivated;
		}
		
		private void OnModelChanged(object sender, EventArgs e)
		{
			this.ViewUpdate ();
		}
		
		private void OnViewRefreshActivated(object sender, EventArgs e)
		{
			base.model.Refresh();	
		}
		
		private void OnViewDismissActivated(object sender, EventArgs e)
		{
			base.view.Visible = false;
		}

		private void ViewUpdate ()
		{
			var state = base.model.State;
			var description = this.GetDescription(state);
			var duration = String.Format("ETA: {0} minutes",
				Math.Ceiling(base.model.Duration.TotalMinutes));
			
			base.Invoke(() =>
			{
				base.view.Visible = true;
				base.view.SetState(state, description);
				base.view.SetLocation(state == BurritoDayState.Transit,
					base.model.Location, duration);
			});
		}
		
		private string GetDescription(BurritoDayState state)
		{
			switch (state)
			{
				case BurritoDayState.No:
					return "Today is not burrito day";
				case BurritoDayState.Tomorrow:
					return "Tomorrow is burrito day";
				case BurritoDayState.Yes:
					return "Today is burrito day";
				case BurritoDayState.Transit:
					return String.Format("Burritos are about {0} minutes away",
						Math.Ceiling(base.model.Duration.TotalMinutes));
				case BurritoDayState.Arrived:
					return "Burritos have arrived";
				default:
					return "Unable to determine burrito status";
			}
		}
	}
}
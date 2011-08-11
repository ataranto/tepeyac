using System;
using Retlang.Fibers;
using Tepeyac.Core;

namespace Tepeyac.UI
{
	public class BurritoDayPresenter : GuiPresenter<IBurritoDayModel, IBurritoDayView>
	{
		public BurritoDayPresenter(IBurritoDayModel model, IBurritoDayView view, [GuiFiber] IFiber guiFiber)
			: base(model, view, guiFiber)
		{
			base.model.Changed += this.OnModelChanged;
			base.view.RefreshActivated += this.OnViewRefreshActivated;
			base.view.DismissActivated += this.OnViewDismissActivated;

			this.ViewSetState(base.model.State);
		}
		
		public override void Dispose()
		{
			base.model.Changed -= this.OnModelChanged;
			base.view.RefreshActivated -= this.OnViewRefreshActivated;
			this.view.DismissActivated -= this.OnViewDismissActivated;
		}
		
		private void OnModelChanged(object sender, EventArgs e)
		{
			this.ViewSetState (base.model.State);
		}
		
		private void OnViewRefreshActivated(object sender, EventArgs e)
		{
			base.model.Refresh();	
		}
		
		private void OnViewDismissActivated(object sender, EventArgs e)
		{
			base.view.Visible = false;
		}

		private void ViewSetState (BurritoDayState state)
		{
			base.Invoke(() =>
			{
				base.view.Visible = true;
				base.view.SetState(state);	
			});
		}
	}
}
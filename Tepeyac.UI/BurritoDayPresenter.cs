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
			base.model.StateChanged += this.OnModelStateChanged;
			
			this.ViewSetState();
		}
		
		public override void Dispose()
		{
			base.model.StateChanged -= this.OnModelStateChanged;
		}
		
		private void OnModelStateChanged(object sender, EventArgs e)
		{
			this.ViewSetState();
		}
		
		private void ViewSetState()
		{
			var state = base.model.State;
			base.Invoke(() => base.view.SetState(state));
		}
	}
}
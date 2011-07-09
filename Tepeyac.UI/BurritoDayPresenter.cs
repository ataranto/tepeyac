using System;
using Tepeyac.Core;

namespace Tepeyac.UI
{
	public class BurritoDayPresenter : Presenter<IBurritoDayModel, IBurritoDayView>
	{
		public BurritoDayPresenter(IBurritoDayModel model, IBurritoDayView view)
			: base(model, view)
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
			Console.WriteLine("set state: {0}", base.model.State);
			
			// XXX: gui fiber
			//var state = base.model.State;
			//base.view.SetState(state);
		}
	}
}
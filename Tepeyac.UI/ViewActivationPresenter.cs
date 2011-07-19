using System;
using Ninject;

namespace Tepeyac.UI
{
	public class ViewActivationPresenter<T> : Presenter<IKernel, IActivationView<T>> where T : IView
	{
		public ViewActivationPresenter (IKernel model, IActivationView<T> view)
			: base(model, view)
		{
			base.view.Activated += this.OnViewActivated;
		}
		
		public override void Dispose()
		{
			base.view.Activated -= this.OnViewActivated;
		}
		
		private void OnViewActivated(object sender, EventArgs e)
		{
			base.model.Get<T>().Present();	
		}
	}
}


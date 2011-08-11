using System;

namespace Tepeyac.UI
{
	public class UrlActivationPresenter : Presenter<ILauncher, IUrlActivationView>
	{
		public UrlActivationPresenter (ILauncher model, IUrlActivationView view)
			: base(model, view)

		{
			base.view.Activated += this.OnViewActivated;
		}
		
		public override void Dispose()
		{
			base.view.Activated -= this.OnViewActivated;
		}
		
		private void OnViewActivated(object sender, string url)
		{
			base.model.Launch(url);	
		}
	}
}


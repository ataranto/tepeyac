using System;
using Ninject;

namespace Tepeyac.UI
{
	public class ViewActivationPresenter<T> : Presenter<IKernel, IActivationView<T>> where T : IView
	{
		public ViewActivationPresenter (IKernel model, IActivationView<T> view)
			: base(model, view)
		{
			
		}
		
		public override void Dispose()
		{
			
		}
	}
}


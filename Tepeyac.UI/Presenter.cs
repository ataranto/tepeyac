using System;

namespace Tepeyac.UI
{
	public abstract class Presenter<TModel, TView> : IDisposable where TView : IView
	{
		protected TModel model;
		protected TView view;
		
		public Presenter(TModel model, TView view)
		{
			this.model = model;
			this.view = view;
		}
		
		public abstract void Dispose();
	}
}


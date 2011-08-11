using System;
using Retlang.Fibers;

namespace Tepeyac.UI
{
	public abstract class GuiPresenter<TModel, TView> : Presenter<TModel, TView> where TView : IView
	{
		protected readonly IFiber fiber;
		
		public GuiPresenter (TModel model, TView view, IFiber fiber)
			: base(model, view)
			
		{
			this.fiber = fiber;
		}
		
		protected void Invoke(Action action)
		{
			this.fiber.Enqueue(action);	
		}
	}
}
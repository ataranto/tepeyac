using Tepeyac.Core;

namespace Tepeyac.UI
{
	public class BurritoDayPresenter : Presenter<IBurritoDayModel, IBurritoDayView>
	{
		public BurritoDayPresenter(IBurritoDayModel model, IBurritoDayView view)
			: base(model, view)
		{
		
		}
		
		public override void Dispose()
		{
			
		}
	}
}
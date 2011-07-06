using Ninject;

namespace Tepeyac.UI
{
	public class MainPresenter
	{
		public MainPresenter(IKernel kernel)
		{
			kernel.Get<IBurritoDayView>();
		}
	}
}


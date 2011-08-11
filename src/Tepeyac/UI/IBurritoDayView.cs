using System;
using Tepeyac.Core;

namespace Tepeyac.UI
{
	public interface IBurritoDayView : IView
	{
		event EventHandler RefreshActivated;
		event EventHandler DismissActivated;
		
		void SetState(BurritoDayState state, string description);
	}
}
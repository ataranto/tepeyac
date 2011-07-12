using System;
using Tepeyac.Core;

namespace Tepeyac.UI
{
	public interface IBurritoDayView : IView
	{
		event EventHandler RefreshActivated;
		
		void SetState(BurritoDayState state);
	}
}
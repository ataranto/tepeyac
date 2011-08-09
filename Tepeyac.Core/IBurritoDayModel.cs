using System;

namespace Tepeyac.Core
{
	public interface IBurritoDayModel
	{
		event EventHandler StateChanged;
		
		BurritoDayState State { get; }
		Uri Latitude { get; }
		
		void Refresh();
	}
}
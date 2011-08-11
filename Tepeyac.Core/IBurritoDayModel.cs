using System;

namespace Tepeyac.Core
{
	public interface IBurritoDayModel
	{
		event EventHandler Changed;
		
		BurritoDayState State { get; }
		TimeSpan Duration { get; }
		string Location { get; }
		
		void Refresh();
	}
}
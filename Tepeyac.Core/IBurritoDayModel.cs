using System;

namespace Tepeyac.Core
{
	public interface IBurritoDayModel
	{
		event EventHandler StateChanged;
		
		BurritoDayState State { get; }
	}
}
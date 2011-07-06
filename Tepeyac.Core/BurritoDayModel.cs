using System;
namespace Tepeyac.Core
{
	public class BurritoDayModel : IBurritoDayModel
	{
		public event EventHandler StateChanged;
		
		public BurritoDayState State
		{
			get { return BurritoDayState.Unknown; }	
		}
	}
}


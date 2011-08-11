using Retlang.Core;
using System;
using System.Collections.Generic;

namespace Tepeyac.Core
{
	public class Executor : IExecutor
	{
		public void Execute(Action action)
		{
			try
			{
				action();
			}
			catch
			{
				
			}
		}
		
		public void Execute(List<Action> actions)
		{
			actions.ForEach(this.Execute);
		}
	}
}


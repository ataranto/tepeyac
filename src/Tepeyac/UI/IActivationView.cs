using System;

namespace Tepeyac.UI
{
	public interface IActivationView<T> : IView where T : IView
	{
		event EventHandler Activated;
	}
}


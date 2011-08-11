using System;

namespace Tepeyac.UI
{
	public interface IUrlActivationView : IView
	{
		event Action<object, string> Activated;
	}
}
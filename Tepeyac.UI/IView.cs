using System;

namespace Tepeyac.UI
{
	public interface IView : IDisposable
	{
		void Present();
	}
}


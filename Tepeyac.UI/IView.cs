using System;

namespace Tepeyac.UI
{
	public interface IView : IDisposable
	{
		bool Visible { get; set; }
	}
}


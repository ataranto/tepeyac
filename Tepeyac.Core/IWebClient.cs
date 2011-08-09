using System;

namespace Tepeyac.Core
{
	public interface IWebClient
	{
		event Action<bool, Exception, string> Completed;
		
		void DownloadStringAsync(Uri uri);
		void CancelAsync();
	}
}


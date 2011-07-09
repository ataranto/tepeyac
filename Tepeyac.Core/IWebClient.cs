using System;
namespace Tepeyac.Core
{
	public interface IWebClient
	{
		event Action<bool, Exception, string> Completed;
		
		void DownloadDataAsync(Uri uri);
	}
}


using System;
namespace Tepeyac.Core
{
	public interface IWebClient
	{
		event Action<bool, Exception, byte[]> Completed;
		
		void DownloadDataAsync(Uri uri);
	}
}


using System;

namespace Tepeyac.Core
{
	public interface IWebClient
	{	
		string DownloadString(Uri uri);
	}
}


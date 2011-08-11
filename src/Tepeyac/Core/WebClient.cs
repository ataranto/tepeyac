using System;
using System.Net;

namespace Tepeyac.Core
{
	public class WebClient : System.Net.WebClient, IWebClient
	{	
		public WebClient ()
		{

		}
		
		public string Download(Uri uri)
		{
			try
			{
				return base.DownloadString(uri);
			}
			catch
			{
				return null;	
			}
		}
	}
}
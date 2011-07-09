using System;
using System.Net;

namespace Tepeyac.Core
{
	public class WebClient : System.Net.WebClient, IWebClient
	{
		public event Action<bool, Exception, string> Completed;
		
		public WebClient ()
		{
			base.DownloadStringCompleted += this.OnDownloadStringCompleted;
		}
		
		private void OnDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
		{
			var handler = this.Completed;
			if (handler != null)
			{
				var success = !e.Cancelled && e.Error == null;
				handler(success, e.Error, e.Result);
			}
		}
	}
}
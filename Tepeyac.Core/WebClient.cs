using System;
using System.Net;

namespace Tepeyac.Core
{
	public class WebClient : System.Net.WebClient, IWebClient
	{
		public event Action<bool, Exception, byte[]> Completed;
		
		public WebClient ()
		{
			base.DownloadDataCompleted += this.OnDownloadDataCompleted;
		}
		
		private void OnDownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
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
using System;
using Retlang.Fibers;
using System.Text;
namespace Tepeyac.Core
{
	public class BurritoDayModel : IBurritoDayModel
	{
		public event EventHandler StateChanged;
		
		private readonly Uri uri = new Uri("http://isitburritoday.com");
		private readonly IWebClient client;
		
		private TimeSpan interval = TimeSpan.FromHours(1);
		
		public BurritoDayModel(IFiber fiber, IWebClient client)
		{
			this.client = client;
			this.client.Completed += this.OnClientCompleted;
			
			this.Poll();
		}
		
		public BurritoDayState State
		{
			get { return BurritoDayState.Unknown; }	
		}
		
		private void OnClientCompleted(bool success, Exception error, byte[] data)
		{
			var xml = Encoding.UTF8.GetString(data);
			Console.WriteLine(xml);
		}
		
		private void Poll()
		{
			this.client.DownloadDataAsync(this.uri);
		}
	}
}


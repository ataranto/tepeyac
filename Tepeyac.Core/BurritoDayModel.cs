using System;
using System.Text;
using System.Xml;
using System.Timers;

namespace Tepeyac.Core
{
	public class BurritoDayModel : IBurritoDayModel
	{
		public event EventHandler StateChanged;
		
		private readonly Uri uri = new Uri("http://isitburritoday.com");
		private readonly Timer timer = new Timer();
		private readonly IWebClient client;
		
		private TimeSpan interval = TimeSpan.FromSeconds(10);
		private BurritoDayState state = BurritoDayState.Unknown;
		
		public BurritoDayModel(IWebClient client)
		{
			this.client = client;
			this.client.Completed += this.OnClientCompleted;
			
			this.timer.AutoReset = false;
			this.timer.Elapsed += delegate { this.Poll(); };
			
			this.Poll();
		}
		
		public BurritoDayState State
		{
			get { return this.state; }
			
			private set
			{
				if (this.state == value)
				{
					return;
				}
				
				this.state = value;
				
				var handler = this.StateChanged;
				if (handler != null)
				{
					handler(this, EventArgs.Empty);
				}
			}
		}
		
		private void OnClientCompleted(bool success, Exception error, string data)
		{
			try
			{
				var doc = new XmlDocument();
				//doc.LoadXml(xml);
				
				Console.WriteLine(data);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
			finally
			{
				this.timer.Interval = this.interval.TotalMilliseconds;
				timer.Enabled = true;
			}
		}
		
		private void Poll()
		{
			this.client.DownloadDataAsync(this.uri);
		}
	}
}


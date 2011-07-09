using System;
using System.Timers;
using HtmlAgilityPack;

namespace Tepeyac.Core
{
	public class BurritoDayModel : IBurritoDayModel
	{
		public event EventHandler StateChanged;
		
		private readonly Uri uri = new Uri("http://isitburritoday.com");
		private readonly Timer timer = new Timer();
		private readonly IWebClient client;
		
		private TimeSpan interval = TimeSpan.FromHours(1);
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
				var doc = new HtmlDocument();
				doc.LoadHtml(data);
				
				var node = doc.DocumentNode.SelectSingleNode("//div[@id='answer']");
				if (node == null || node.InnerText == null)
				{
					this.State = BurritoDayState.Unknown;
				}
				else
				{
					var text = node.InnerText.Trim().ToLower();
					if (text == "yes")
					{
						this.State = BurritoDayState.Yes;
					}
					else if (text == "no")
					{
						this.State = BurritoDayState.No;	
					}
				}
			}
			finally
			{
				this.timer.Interval = this.interval.TotalMilliseconds;
				timer.Enabled = true;
			}
		}
		
		private void Poll()
		{
			this.client.DownloadStringAsync(this.uri);
		}
	}
}


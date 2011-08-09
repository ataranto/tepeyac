using System;
using System.Collections.Generic;
using System.Timers;
using HtmlAgilityPack;

namespace Tepeyac.Core
{
	public class BurritoDayModel : IBurritoDayModel
	{
		public event EventHandler StateChanged;
		
		private readonly IDictionary<string, BurritoDayState> keywords = new Dictionary<string, BurritoDayState>()
		{
			{ "no", BurritoDayState.No },
			{ "yes", BurritoDayState.Yes },
			{ "tomorrow", BurritoDayState.Tomorrow },
		};
		
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
			this.timer.Elapsed += delegate { this.Refresh(); };

			this.Refresh();
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
		
		public void Refresh()
		{
			this.timer.Enabled = false;
			
			try
			{
				this.client.CancelAsync();
			}
			finally
			{
				this.client.DownloadStringAsync(this.uri);
			}
		}
		
		private void OnClientCompleted(bool success, Exception error, string data)
		{
			try
			{
				this.State = this.GetState(data);
			}
			finally
			{
				this.timer.Interval = this.interval.TotalMilliseconds;
				timer.Enabled = true;
			}
		}
			
		private BurritoDayState GetState(string data)
		{
			var doc = new HtmlDocument();
			doc.LoadHtml(data);
			
			var node =
				doc.DocumentNode.SelectSingleNode("//div[@id='hooray']") ??
				doc.DocumentNode.SelectSingleNode("//div[@id='answer']");
			
			if (node != null && !String.IsNullOrEmpty(node.InnerText))
			{
				var text = node.InnerText.ToLower();
				
				foreach (var pair in this.keywords)
				{
					if (text.Contains(pair.Key))
					{
						return pair.Value;	
					}
				}
			}
			
			return BurritoDayState.Unknown;
		}
	}
}


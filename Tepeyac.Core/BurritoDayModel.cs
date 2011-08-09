using System;
using System.Collections.Generic;
using System.Timers;
using HtmlAgilityPack;
using Retlang.Fibers;

namespace Tepeyac.Core
{
	public class BurritoDayModel : IBurritoDayModel
	{
		public event EventHandler StateChanged;
		
		private readonly TimeSpan interval = TimeSpan.FromHours(1);
		private readonly Uri uri = new Uri("http://isitburritoday.com");

		private readonly IFiber fiber;
		private readonly IWebClient client;
		
		private BurritoDayState state = BurritoDayState.Unknown;
		private Uri latitude = null;
		
		public BurritoDayModel(IFiber fiber, IWebClient client)
		{
			this.fiber = fiber;
			this.client = client;
			
			this.fiber.ScheduleOnInterval(this.PollState, 0,
				(long)this.interval.TotalMilliseconds);
		}
		
		public Uri Latitude
		{
			get { return this.latitude; }
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
			this.fiber.Enqueue(this.PollState);	
		}
		
		private void PollState()
		{
			try
			{
				var data = this.client.DownloadString(this.uri);
				var doc = new HtmlDocument();
				doc.LoadHtml(data);
				
				var state = BurritoDayModel.GetState(doc);
				this.latitude = BurritoDayModel.GetLatitudeUri(doc);
				
				this.State = state;
				
				/*
				if (state == BurritoDayState.Yes && this.latitude != null)
				{
					this.fiber.Enqueue(this.PollLocation);
				}
				else
				{
					this.State = state;
				}
				*/
			}
			catch
			{
				this.State = BurritoDayState.Unknown;
			}
		}
		
		private static IDictionary<string, BurritoDayState> StateKeywords = new Dictionary<string, BurritoDayState>()
		{
			{ "no", BurritoDayState.No },
			{ "yes", BurritoDayState.Yes },
			{ "tomorrow", BurritoDayState.Tomorrow },
		};
			
		private static BurritoDayState GetState(HtmlDocument doc)
		{
			var node =
				doc.DocumentNode.SelectSingleNode("//div[@id='hooray']") ??
				doc.DocumentNode.SelectSingleNode("//div[@id='answer']");
			
			if (node != null && !String.IsNullOrEmpty(node.InnerText))
			{
				var text = node.InnerText.ToLower();
				
				foreach (var pair in BurritoDayModel.StateKeywords)
				{
					if (text.Contains(pair.Key))
					{
						return pair.Value;	
					}
				}
			}
			
			return BurritoDayState.Unknown;
		}
		
		private static Uri GetLatitudeUri(HtmlDocument doc)
		{
			var node = doc.DocumentNode.SelectSingleNode("//iframe");
			Uri uri = null;
			
			if (node != null)
			{
				Uri.TryCreate(node.GetAttributeValue("src", null),
					UriKind.Absolute, out uri);
			}
			
			return uri;
		}
	}
}


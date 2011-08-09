using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Timers;
using System.Web;
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
		
		public BurritoDayModel(IFiber fiber, IWebClient client)
		{
			this.fiber = fiber;
			this.client = client;
			
			this.fiber.ScheduleOnInterval(this.PollState, 0,
				(long)this.interval.TotalMilliseconds);
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
				var latitude = BurritoDayModel.GetLatitudeUri(doc);
				
				if (latitude != null)
				{
					data = this.client.DownloadString(latitude);
					var location = BurritoDayModel.GetLocation(data);
				}
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
		
		private static Regex Regex = new Regex("\".*\"", RegexOptions.Compiled);
		
		private static Location GetLocation(string data)
		{
			var location = new Location();
			
			foreach (var match in BurritoDayModel.Regex.Matches(data))
			{
				var center = HttpUtility.ParseQueryString(match.ToString())["center"];
				if (!String.IsNullOrEmpty(center))
				{
					double latitude, longitude;
					var tokens = center.Split(',');
					if (tokens.Length == 2 &&
						Double.TryParse(tokens[0], out latitude) &&
						Double.TryParse(tokens[1], out longitude))
					{
						location.Latitude = latitude;
						location.Longitute = longitude;
						
						break;
					}
				}
			}
			
			return location;
		}
	}
}
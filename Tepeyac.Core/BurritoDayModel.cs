using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using HtmlAgilityPack;
using Retlang.Fibers;

namespace Tepeyac.Core
{
	public class BurritoDayModel : IBurritoDayModel
	{
		public event EventHandler StateChanged;
		
		private readonly TimeSpan interval = TimeSpan.FromHours(1);
		private readonly Uri uri = new Uri("http://isitburritoday.com");
		private readonly string api = "http://maps.googleapis.com/maps/api/distancematrix/xml?sensor=false&units=imperial&destinations=62+1st+St+San+Francisco+CA&origins=";

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
			var data = this.client.DownloadString(this.uri);
			var doc = new HtmlDocument();
			doc.LoadHtml(data);
			
			BurritoDayState state;
			if (!BurritoDayModel.TryGetState(doc, out state))
			{
				this.State = BurritoDayState.Unknown;
				return;
			}
			
			Uri uri;
			if (state == BurritoDayState.Yes &&
				BurritoDayModel.TryGetLatitudeUri(doc, out uri))
			{
				data = this.client.DownloadString(uri);
				double latitude, longitude;
				
				if (BurritoDayModel.TryGetLocation(data, out latitude, out longitude))
				{
					uri = new Uri(this.api + latitude + "," + longitude);
					data = this.client.DownloadString(uri);
					
					Distance distance;
					var xml = new XmlDocument();
					xml.LoadXml(data);
					
					if (BurritoDayModel.TryGetDistance(xml, out distance))
					{
						state = distance.Duration < TimeSpan.FromMinutes(5) ?
							BurritoDayState.Arrived :
							BurritoDayState.Transit;
					}
				}
			}
			
			this.State = state;
		}

		private static IDictionary<string, BurritoDayState> StateKeywords = new Dictionary<string, BurritoDayState>()
		{
			{ "no", BurritoDayState.No },
			{ "yes", BurritoDayState.Yes },
			{ "tomorrow", BurritoDayState.Tomorrow },
		};
		private static bool TryGetState(HtmlDocument doc, out BurritoDayState state)
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
						state = pair.Value;	
						return true;
					}
				}
			}

			state = BurritoDayState.Unknown;
			return false;
		}
		
		private static bool TryGetLatitudeUri(HtmlDocument doc, out Uri uri)
		{
			uri = null;
			
			var node = doc.DocumentNode.SelectSingleNode("//iframe");
			return
				node != null &&
				Uri.TryCreate(node.GetAttributeValue("src", null),
					UriKind.Absolute, out uri);
		}
		
		private static Regex Regex = new Regex("\".*\"", RegexOptions.Compiled);
		private static bool TryGetLocation(string data,
			out double latitude, out double longitude)
		{
			foreach (var match in BurritoDayModel.Regex.Matches(data))
			{
				var center = HttpUtility.ParseQueryString(match.ToString())["center"];
				if (!String.IsNullOrEmpty(center))
				{
					var tokens = center.Split(',');
					if (tokens.Length == 2 &&
						double.TryParse(tokens[0], out latitude) &&
						double.TryParse(tokens[1], out longitude))
					{
						return true;
					}
				}
			}
			
			latitude = longitude = default(double);
			return false;
		}
		
		private static bool TryGetDistance(XmlDocument doc, out Distance distance)
		{
			distance = new Distance();
			
			var node = doc.SelectSingleNode("//origin_address");
			if (node != null && !String.IsNullOrEmpty(node.InnerText))
			{
				distance.LocationDescription = node.InnerText;
			}
			
			node = doc.SelectSingleNode("//duration/value");
			double seconds;
			if (node != null && !String.IsNullOrEmpty(node.InnerText) &&
				double.TryParse(node.InnerText, out seconds))
			{
				distance.Duration = TimeSpan.FromSeconds(seconds);
			}
			
			node = doc.SelectSingleNode("//distance/value");
			ulong meters;
			if (node != null && !String.IsNullOrEmpty(node.InnerText) &&
				ulong.TryParse(node.InnerText, out meters))
			{
				distance.Meters = meters;
			}

			return !String.IsNullOrEmpty(distance.LocationDescription) &&
				(distance.Duration > TimeSpan.Zero || distance.Meters > 0);
		}
	}
}
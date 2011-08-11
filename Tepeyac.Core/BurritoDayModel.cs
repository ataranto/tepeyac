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
		private readonly Uri burrito_day_uri = new Uri("http://isitburritoday.com");
		private readonly Uri latitude_uri = new Uri("http://www.google.com/latitude/apps/badge/api?user=797967215506697296&type=iframe&maptype=roadmap");
		private readonly string distance_api = "http://maps.googleapis.com/maps/api/distancematrix/xml?sensor=false&units=imperial&destinations=62+1st+St+San+Francisco+CA&origins=";

		private readonly IFiber fiber;
		private readonly IWebClient client;
		
		private BurritoDayState state = BurritoDayState.Unknown;
		private IDisposable location_scheduled = null;
		
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
			var data = this.client.Download(this.burrito_day_uri);
			
			BurritoDayState state;
			if (!BurritoDayModel.TryParseState(data, out state))
			{
				this.State = BurritoDayState.Unknown;
			}
			else if (state == BurritoDayState.Yes)
			{
				this.StartLocationPolling(TimeSpan.FromMinutes(10));
			}
			else
			{
				this.StopLocationPolling();
				this.State = state;
			}
		}
		
		private void PollLocation()
		{
			state = BurritoDayState.Yes;
			var data = this.client.Download(this.latitude_uri);
			
			double latitude, longitude;
			if (BurritoDayModel.TryParseLocation(data, out latitude, out longitude))
			{
				var uri = new Uri(this.distance_api + latitude + "," + longitude);
				data = this.client.Download(uri);
			
				Distance distance;
				if (BurritoDayModel.TryParseDistance(data, out distance))
				{
					state = distance.Duration < TimeSpan.FromMinutes(5) ?
							BurritoDayState.Arrived :
							BurritoDayState.Transit;
				}
			}
			
			this.State = state;
		}
		
		private void StartLocationPolling(TimeSpan interval)
		{
			this.StopLocationPolling();
			
			var ms = (long)interval.TotalMilliseconds;
			this.location_scheduled =
				this.fiber.ScheduleOnInterval(this.PollLocation, 0, ms);
		}
		
		private void StopLocationPolling()
		{
			using (this.location_scheduled)
			{
				this.location_scheduled = null;
			}
		}

		private static IDictionary<string, BurritoDayState> StateKeywords = new Dictionary<string, BurritoDayState>()
		{
			{ "no", BurritoDayState.No },
			{ "yes", BurritoDayState.Yes },
			{ "tomorrow", BurritoDayState.Tomorrow },
		};
		private static bool TryParseState(string data, out BurritoDayState state)
		{
			var doc = new HtmlDocument();
			doc.LoadHtml(data ?? String.Empty);
			
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

			state = default(BurritoDayState);
			return false;
		}
		
		private static Regex Regex = new Regex("\".*\"", RegexOptions.Compiled);
		private static bool TryParseLocation(string data,
			out double latitude, out double longitude)
		{
			latitude = longitude = default(double);
			
			if (String.IsNullOrEmpty(data))
			{
				return false;
			}
			
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
			
			return false;
		}
		
		private static bool TryParseDistance(string data, out Distance distance)
		{
			distance = new Distance();
			var doc = new XmlDocument();
			doc.LoadXml(data ?? String.Empty);
			
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
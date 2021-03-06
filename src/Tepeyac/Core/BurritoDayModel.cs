using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using HtmlAgilityPack;
using Retlang.Fibers;
using System.Xml.Schema;

namespace Tepeyac.Core
{
	public class BurritoDayModel : IBurritoDayModel
	{
		public event EventHandler Changed;
		
		private readonly object sync = new object();
		private readonly TimeSpan interval = TimeSpan.FromHours(1);
		private readonly Uri burrito_day_uri = new Uri("http://isitburritoday.com");
		private readonly Uri latitude_uri = new Uri("http://www.google.com/latitude/apps/badge/api?user=797967215506697296&type=iframe&maptype=roadmap");
		private readonly string distance_api = "http://maps.googleapis.com/maps/api/distancematrix/xml?sensor=false&units=imperial&destinations=1550+Bryant+St+San+Francisco+CA&origins=";
		
		private readonly int origin_threshold = 39000; // ~24 miles
		private readonly int destination_threshold = 1600; // ~1 mile
		
		private readonly IFiber fiber;
		private readonly IWebClient client;
		
		private BurritoDayState state = BurritoDayState.Unknown;
		private IDisposable location_scheduled = null;
		private string location = null;
		private TimeSpan duration = TimeSpan.Zero;
		private int meters = 0;
		
		public BurritoDayModel(IFiber fiber, IWebClient client)
		{
			this.fiber = fiber;
			this.client = client;
			
			this.fiber.ScheduleOnInterval(this.PollState, 0,
				(long)this.interval.TotalMilliseconds);
		}
		
		public BurritoDayState State
		{
			get
			{
				lock (this.sync)
				{
					return this.state;
				}
			}
			
			private set
			{
				if (value != BurritoDayState.Transit)
				{
					if (value == this.state)
					{
						return;
					}
					
					this.location = null;
					this.duration = TimeSpan.Zero;
				}
				
				lock (this.sync)
				{
					this.state = value;
				}
				
				var handler = this.Changed;
				if (handler != null)
				{
					handler(this, EventArgs.Empty);
				}
			}
		}
		
		public TimeSpan Duration
		{
			get
			{
				lock (this.sync)
				{
					return this.state == BurritoDayState.Transit ?
						this.duration :
						TimeSpan.Zero;
				}
			}
		}
		
		public string Location
		{
			get
			{
				lock (this.sync)
				{
					return this.state == BurritoDayState.Transit ?
						this.location :
						null;
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
			
			BurritoDayState new_state;
			if (!BurritoDayModel.TryParseState(data, out new_state))
			{
				this.State = BurritoDayState.Unknown;
			}
			else if (new_state != BurritoDayState.Yes)
			{
				this.StopLocationPolling();
				this.State = new_state;
			}
			else
			{
				this.StartLocationPolling(TimeSpan.FromMinutes(10));
			}
		}
		
		private void PollLocation()
		{
			if (this.state == BurritoDayState.Arrived)
			{
				return;
			}

			var new_state = BurritoDayState.Yes;
			var data = this.client.Download(this.latitude_uri);
			
			double latitude, longitude;
			if (BurritoDayModel.TryParseLocation(data, out latitude, out longitude))
			{
				var uri = new Uri(this.distance_api + latitude + "," + longitude);
				data = this.client.Download(uri);
				
				bool success;
				
				lock (this.sync)
				{
					success = BurritoDayModel.TryParseDistance(data,
						out this.location, out this.duration, out this.meters);
				}
				
				if (success)
				{
					if (meters >= this.destination_threshold &&
						meters <= this.origin_threshold)
					{
						new_state = BurritoDayState.Transit;
						this.StartLocationPolling(TimeSpan.FromMinutes(2));
					}
					else if (this.state == BurritoDayState.Transit &&
						meters <= this.destination_threshold)
					{
						new_state = BurritoDayState.Arrived;
						this.StopLocationPolling();
						
						// force a transition out of the Arrived state
						// around midnight in case of back to back
						// burrito days. a delicious special case.
						this.fiber.Schedule(() =>
						{
							this.state = BurritoDayState.Unknown;
							this.PollState();
						}, (long)TimeSpan.FromHours(12).TotalMilliseconds);
					}
				}
			}
			
			this.State = new_state;
		}
		
		private void StartLocationPolling(TimeSpan interval)
		{
			this.StopLocationPolling();
			
			var regular = (long)interval.TotalMilliseconds;
			var first = this.state == BurritoDayState.Unknown ?
                0 :
                regular;
			this.location_scheduled =
				this.fiber.ScheduleOnInterval(this.PollLocation, first, regular);
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
		
		private static Regex Regex = new Regex("\".*\"", RegexOptions.None);
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
				string center =  null;
				
				var tokens = match.ToString().Split('?');
				if (tokens.Length == 2)
				{
					tokens = tokens[1].Split('&');
					foreach (var token in tokens)
					{
						var pair = token.Split('=');
						if (pair.Length == 2 && pair[0] == "center")
						{
							center = pair[1];
						}
					}
				}
				
				if (!String.IsNullOrEmpty(center))
				{
					tokens = center.Split(',');
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
		
		private static bool TryParseDistance(string data,
			out string location, out TimeSpan duration, out int meters)
		{
			var doc = new XmlDocument();
			doc.LoadXml(data ?? String.Empty);
			
			var node = doc.SelectSingleNode("//origin_address");
			location = node != null ?
				node.InnerText :
				null;
			
			node = doc.SelectSingleNode("//duration/value");
			double seconds;
			duration = node != null && double.TryParse(node.InnerText, out seconds) ?
				TimeSpan.FromSeconds(seconds) :
				TimeSpan.Zero;
			
			node = doc.SelectSingleNode("//distance/value");
			if (node == null || !int.TryParse(node.InnerText, out meters))
			{
				meters = 0;
			}

			return
				!String.IsNullOrEmpty(location) &&
				duration > TimeSpan.Zero &&
				meters > 0;
		}
	}
}
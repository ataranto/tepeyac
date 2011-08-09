using System;

namespace Tepeyac.Core
{
	public class Location
	{
		public readonly double Latitude;
		public readonly double Longitude;
		
		public Location(double latitude, double longitude)
		{
			this.Latitude = latitude;
			this.Longitude = longitude;
		}
		
		public override string ToString()
		{
			return String.Format("Latitude: {0}, Longitude: {1}",
				this.Latitude, this.Longitude);	
		}
	}
}


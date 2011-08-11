using System;

namespace Tepeyac.Core
{
	public class Platform : IPlatform
	{
		public string Name
		{
			get { return Environment.OSVersion.Platform.ToString(); }
		}
		
		public Version Version
		{
			get { return Environment.OSVersion.Version; }	
		}
	}
}


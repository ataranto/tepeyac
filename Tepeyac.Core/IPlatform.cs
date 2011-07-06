using System;

namespace Tepeyac.Core
{
	public interface IPlatform
	{
		string Name { get; }
		Version Version { get; }
	}
}
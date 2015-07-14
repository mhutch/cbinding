using System.Collections.Generic;

namespace CBinding.Parser
{
	public class OverloadCandidate
	{
		public string Returns { get; }

		public string Name { get; }

		public List<string> Parameters { get; }

		public OverloadCandidate (string returns, string name, List<string> parameters)
		{
			Returns = returns;
			Name = name;
			Parameters = parameters;
		}

	}

}

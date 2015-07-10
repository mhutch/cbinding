using System.Collections.Generic;

namespace CBinding.Parser
{
	public class OverloadCandidate
	{
		public string Returns {
			get;
			set;
		}

		public string Name {
			get;
			set;
		}

		public List<string> Parameters {
			get;
			set;
		}

		public OverloadCandidate(string returns, string name, List<string> parameters){
			Returns = returns;
			Name = name;
			Parameters = parameters;
		}
	}
}

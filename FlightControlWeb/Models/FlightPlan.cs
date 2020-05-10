using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
	public class FlightPlan
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public bool IsComplete { get; set; }
		public int Passengers { get; set; }
		public int Longitude { get; set; }
		public int Latitude { get; set; }
		public string DataTime { get; set; }
		// list of segments.
	}
}

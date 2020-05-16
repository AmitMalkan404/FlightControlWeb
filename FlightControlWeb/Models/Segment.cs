using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
	public class Segment
	{
		public long Id { get; set; }
		public double Longitude { get; set; }
		public double Latitude { get; set; }
		public double TimespanSeconds { get; set; }
		public string FlightId { get; set; }

	}
}

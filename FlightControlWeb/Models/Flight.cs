using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
	public class Flight
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public bool IsComplete { get; set; }
		public long FlightId { get; set; }
		public int Longitude { get; set; }
		public int Latitude { get; set; }
		public int Passengers { get; set; }
		public string CompanyName { get; set; }
		public string DataTime { get; set; }
		public string IsExternal { get; set; }
	}
}

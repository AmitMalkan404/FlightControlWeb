using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
	public class FlightPlanFullData
	{
		[JsonPropertyName("flight_id")]
		public string FlightId { get; set; }
		[JsonPropertyName("passengers")]
		public int Passengers { get; set; }
		[JsonPropertyName("comapny_name")]
		public string CompanyName { get; set; }
		[JsonPropertyName("initial_location")]
		public InitialLocation InitialLocation { get; set; }

		[JsonPropertyName("segments")]
		public List<Segment> Segments { get; set; }
		[JsonPropertyName("is_external")]
		public bool IsExternal { get; set; }
	}
}

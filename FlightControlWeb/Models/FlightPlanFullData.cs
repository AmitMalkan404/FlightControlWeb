using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
//using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
	public class FlightPlanFullData
	{
		[JsonProperty("flight_id")]
		public string FlightId { get; set; }
		[JsonProperty("passengers")]
		public int Passengers { get; set; }
		[JsonProperty("company_name")]
		public string CompanyName { get; set; }
		[JsonProperty("initial_location")]
		public InitialLocation InitialLocation { get; set; }

		[JsonProperty("segments")]
		public List<Segment> Segments { get; set; }
		[JsonProperty("is_external")]
		public bool IsExternal { get; set; }
	}
}

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
//using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
	public class FlightPlan
	{
		[Key]
		public long Id { get; set; }
		[JsonProperty("flight_id")]
		public string FlightId { get; set; }
		[JsonProperty("passengers")]
		[Range(0, Double.MaxValue, ErrorMessage = "Amount of passengers must be possitive")]
		public int Passengers { get; set; }
		[JsonProperty("company_name")]
		
		[Required(ErrorMessage = "Flight plan has to have a company_name")]
		public string CompanyName { get; set; }
		[JsonProperty("initial_location")]
		public InitialLocation InitialLocation{ get; set; }
		[JsonProperty("segments")]
		[Required(ErrorMessage = "A flight has to have a have route")]
		public List<Segment> SegmentsList{ get; set; }

		[JsonProperty("is_external")]
		public bool IsExternal { get; set; }
	}
}

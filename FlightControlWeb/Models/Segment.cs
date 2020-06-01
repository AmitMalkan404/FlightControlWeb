using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
//using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
	public class Segment
	{
		public long Id { get; set; }
		[JsonProperty("longitude")]
		[Required]
		[Range(-180.000001, 180, ErrorMessage = "{0} has to be between {1} to {2}")]
		public double Longitude { get; set; }
		[JsonProperty("latitude")]
		[Required]
		[Range(-90.000001, 90, ErrorMessage = "{0} has to be between {1} to {2}")]
		public double Latitude { get; set; }
		[JsonProperty("timespan_seconds")]
		[Range(0, Double.MaxValue, ErrorMessage = "TimeSpan must be possitive")]

		public double TimespanSeconds { get; set; }
		public string FlightId { get; set; }

	}
}

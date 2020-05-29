using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
//using System.Text.Json.Serialization;
using System.Threading.Tasks;
using JsonIgnoreAttribute = Newtonsoft.Json.JsonIgnoreAttribute;

namespace FlightControlWeb.Models
{
	public class InitialLocation
	{
		[JsonIgnore]
		public long Id { get; set; }
		[JsonProperty("longitude")]
		[Required]
		[Range(-180.000001, 180, ErrorMessage = "{0} has to be between {1} to {2}")]
		public double Longitude { get; set; }
		[JsonProperty("latitude")]
		[Required]
		[Range(-90.000001, 90, ErrorMessage = "{0} has to be between {1} to {2}")]
		public double Latitude { get; set; }
		[JsonProperty("date_time")]
		public DateTime DateTime { get; set; }
	}
}

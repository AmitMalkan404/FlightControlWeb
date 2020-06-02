using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
//using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
	public class Flight
	{
		public long Id { get; set; }
		[JsonProperty("flight_id")]
		public string FlightId { get; set; }
		[JsonProperty("longitude")]
		[Required]
		[Range(-180.000001, 180, ErrorMessage = "{0} has to be between {1} to {2}")]
		public double Longitude { get; set; }
		[JsonProperty("latitude")]
		[Required]
		[Range(-90.000001, 90, ErrorMessage = "{0} has to be between {1} to {2}")]
		public double Latitude { get; set; }
		[JsonProperty("passengers")]
		[Range(0, Double.MaxValue, ErrorMessage = "Amount of passengers must be possitive")]
		public int Passengers { get; set; }
		[JsonProperty("company_name")]

		[Required(ErrorMessage = "Flight plan has to have a company_name")]
		public string CompanyName { get; set; }
		[JsonProperty("date_time")]
		public DateTime DateTime { get; set; }
		[JsonProperty("is_external")]
		public bool IsExternal { get; set; }
	}
}

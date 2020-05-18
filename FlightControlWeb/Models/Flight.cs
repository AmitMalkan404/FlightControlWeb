using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
	public class Flight
	{
		public long Id { get; set; }
		[JsonPropertyName("flight_id")]
		public string FlightId { get; set; }
		public double Longitude { get; set; }
		public double Latitude { get; set; }
		public int Passengers { get; set; }
		[JsonPropertyName("company_name")]
		public string CompanyName { get; set; }
		[JsonPropertyName("date_time")]
		public DateTime DateTime { get; set; }
		[JsonPropertyName("is_external")]
		public bool IsExternal { get; set; }
	}
}

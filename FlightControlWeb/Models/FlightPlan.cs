using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
	public class FlightPlan
	{
		public long Id { get; set; }
		public string FlightId { get; set; }
		public int Passengers { get; set; }
		[JsonPropertyName("comapny_name")]
		public string CompanyName { get; set; }
		public double Longitude { get; set; }
		public double Latitude { get; set; }
		[JsonPropertyName("date_time")]
		public DateTime DateTime { get; set; }
		public bool IsExternal { get; set; }
	}
}

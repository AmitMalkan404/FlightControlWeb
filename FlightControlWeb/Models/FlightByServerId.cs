using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
	public class FlightByServerId
	{
		public long id { get; set; }
		public string ServerId { get; set; }
		public string FlightId { get; set; }
	}
}

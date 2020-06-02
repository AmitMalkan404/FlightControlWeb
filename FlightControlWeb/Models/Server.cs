using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
	public class Server
	{
		public long id { get; set; }
		[Required]
		public string ServerId { get; set; }
		[Required]
		public string ServerURL { get; set; }

	}
}

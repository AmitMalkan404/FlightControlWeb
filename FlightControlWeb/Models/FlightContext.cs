using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FlightControlWeb.Models;


namespace FlightControlWeb.Models
{
	public class FlightContext : DbContext
	{
        public FlightContext(DbContextOptions<FlightContext> options) : base(options)
        {
        }

        public DbSet<FlightPlan> FlightItems { get; set; }

        public DbSet<FlightControlWeb.Models.Flight> Flight { get; set; }
        public DbSet<FlightControlWeb.Models.Server> Server { get; set; }
        public DbSet<FlightControlWeb.Models.Segment> Segment { get; set; }
        public DbSet<FlightControlWeb.Models.FlightByServerId> FlightByServerIds { get; set; }


    }
}

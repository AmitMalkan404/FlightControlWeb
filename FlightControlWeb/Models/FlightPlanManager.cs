using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace FlightControlWeb.Models
{
    public class FlightPlanManager
    {
        private readonly FlightContext _context;
        public FlightPlanManager(FlightContext context)
        {
            _context = context;
        }
    }

}

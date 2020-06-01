using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlightControlWeb.Models;
//using System.Text.Json;
using Newtonsoft.Json;
using System.Net.Http;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Newtonsoft.Json.Serialization;

namespace FlightControlWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightPlansController : ControllerBase
    {
        private readonly FlightContext _context;
        private static int _flightsNumber=0;


        public FlightPlansController(FlightContext context)
        {
            _context = context;
        }

        //GET: api/FlightPlans/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FlightPlan>> GetFlightPlan(string id)
        {
            string request = Request.QueryString.Value;
            FlightPlan flightPlan = await _context.FlightItems.Include(x=>x.SegmentsList).Include(x => x.InitialLocation).Where(x => x.FlightId == id).FirstOrDefaultAsync();
            // Check in our server DB.
            if (flightPlan != null)
            {
                return Ok(flightPlan);
            }
            else
            {
                var flightPlan2 = await CheckFlightPlanInServers(id);
                if (flightPlan2 != null)
                {
                    return flightPlan2;
                }
                return NotFound();
            }
        }
        // POST: api/FlightPlans
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<FlightPlan>> PostFlightPlan([FromBody] FlightPlan jsonFlight)
        {
            // string stringJsonFlight = jsonFlight.ToString();
            //dynamic jsonObj = JsonConvert.DeserializeObject(stringJsonFlight);
            //FlightPlan fp = JsonConvert.DeserializeObject<FlightPlan>(stringJsonFlight);
            jsonFlight.FlightId = SetFlightId(jsonFlight.CompanyName);
            _context.FlightItems.Add(jsonFlight);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetFlightPlan", new { id = jsonFlight.Id }, jsonFlight); 
        }
        private bool FlightPlanExists(long id)
        {
            return _context.FlightItems.Any(e => e.Id == id);
        }
        public string SetFlightId(string flightName)
        {
            string flightId = flightName.Substring(0, 3);
            int flightIdlen = flightId.Length;
            for (int i = flightIdlen; i < 10; i++)
            {
                flightId += _flightsNumber;
                _flightsNumber++;
                i++;
            }
            return flightId;
        }

        public async Task<ActionResult<FlightPlan>> CheckFlightPlanInServers(string id)
        {
            FlightByServerId serverUrl = await _context.FlightByServerIds.Where(x => x.FlightId == id).FirstAsync();
            if(serverUrl == null)
            {
                return NotFound();
            }
            //var flightByServerId = from flight in _context.FlightByServerIds where flight.FlightId == id select flight;
            var flightPlan = await GetExternalFlightFromServer(serverUrl.ServerId, id);
            return flightPlan;
            //catch
            //{
            //    throw new ArgumentException("This is an external flightPlan. Something went wrong with its server.");
            //}
            //return null;
        }
        public async Task<ActionResult<FlightPlan>> GetExternalFlightFromServer(string myServerUrl,string id)
        {
            //string url = "https://";
            string url = "";
            url += myServerUrl;
            url += "/api/FlightPlans/" + id;
            HttpClient client = new HttpClient();
            var response = await client.GetStringAsync(url);
            if (response == null)
            {
                throw new ArgumentException("This is an external flightPlan. Something went wrong with its server.");
            }
            string stringJsonFlight = response.ToString();
            FlightPlan fp = JsonConvert.DeserializeObject<FlightPlan>(stringJsonFlight);
            //dynamic json = JsonConvert.DeserializeObject(stringJsonFlight);
            return fp;
        }
    }
}

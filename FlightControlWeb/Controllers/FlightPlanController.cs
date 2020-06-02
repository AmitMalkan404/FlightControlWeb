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
    public class FlightPlanController : ControllerBase
    {
        private readonly FlightContext _context;
        private static int _flightsNumber=0;


        public FlightPlanController(FlightContext context)
        {
            _context = context;
        }

        //GET: api/FlightPlan/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FlightPlan>> GetFlightPlan(string id)
        {
            // Getting the request.
            string request = Request.QueryString.Value;
            // Checking in the DB if we have a flight plan with the same ID that we got.
            FlightPlan flightPlan = await _context.FlightItems.Include(x => x.SegmentsList).Include(x => x.InitialLocation).Where(x => x.FlightId == id).FirstOrDefaultAsync();
            // Check in our server DB.
            if (flightPlan != null)
            {
                return Ok(flightPlan);
            }
            else
            {
                // If we didnt find the flightplan at our DB, we will check from which server we got it.
                var flightPlan2 = await CheckFlightPlanInServers(id);
                if (flightPlan2 != null)
                {
                    return flightPlan2;
                }
                return NotFound();
            }
        }
        // POST: api/FlightPlan
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<FlightPlan>> PostFlightPlan([FromBody] FlightPlan jsonFlight)
        {
            // Give the new flight plan an ID.
            jsonFlight.FlightId = SetFlightId(jsonFlight.CompanyName);
            // Add it to the DB.
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
            // Giving the new flight plan an ID using chars and numbers.
            string flightId = flightName.Substring(0, 3);
            int flightIdlen = flightId.Length;
            for (int i= flightIdlen+1; i<10;i++)
            {
                flightId += _flightsNumber;
                _flightsNumber++;
                i++;
            }
            return flightId;
        }

        public async Task<ActionResult<FlightPlan>> CheckFlightPlanInServers(string id)
        {
            // We know that the flight isnt from our DB, and we got it from some where. 
            // We have a DB that maps flights from out side servers, and the servers that we got them from.
            // We will search from which sever we got the flight from, according to the flight id.
            FlightByServerId serverUrl = await _context.FlightByServerIds.Where(x => x.FlightId == id).FirstAsync();
            if(serverUrl == null)
            {
                return NotFound();
            }
            var flightPlan = await GetExternalFlightFromServer(serverUrl.ServerId, id);
            return flightPlan;
        }
        public async Task<ActionResult<FlightPlan>> GetExternalFlightFromServer(string myServerUrl,string id)
        {
            // Creating the server url and send the GET request.
            string url = "";
            url += myServerUrl;
            url += "/api/FlightPlan/" + id;
            HttpClient client = new HttpClient();
            var response = await client.GetStringAsync(url);
            if (response == null)
            {
                // Throw exception if some error happened.
                throw new ArgumentException("This is an external flightPlan. Something went wrong with its server.");
            }
            string stringJsonFlight = response.ToString();
            FlightPlan fp = JsonConvert.DeserializeObject<FlightPlan>(stringJsonFlight);
            return fp;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlightControlWeb.Models;
using System.Text.Json;
using Newtonsoft.Json;

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

        // GET: api/FlightPlans
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FlightPlan>>> GetFlightItems()
        {
            return await _context.FlightItems.ToListAsync();
        }

        // GET: api/FlightPlans/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FlightPlan>> GetFlightPlan(long id)
        {
            var flightPlan = await _context.FlightItems.FindAsync(id);

            if (flightPlan == null)
            {
                return NotFound();
            }

            return flightPlan;
        }

        // PUT: api/FlightPlans/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFlightPlan(long id, FlightPlan flightPlan)
        {
            if (id != flightPlan.Id)
            {
                return BadRequest();
            }

            _context.Entry(flightPlan).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FlightPlanExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/FlightPlans
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<FlightPlan>> PostFlightPlan([FromBody] JsonElement jsonFlight)
        {
            FlightPlan flightPlan = new FlightPlan();
            string stringJsonFlight = jsonFlight.ToString();
            dynamic jsonObj = JsonConvert.DeserializeObject(stringJsonFlight);
            //long id;
            int passengers = jsonObj["passengers"];
            string companyName = jsonObj["company_name"];
            string flightId = SetFlightId(companyName);
            double longitude = jsonObj["initial_location"]["longitude"];
            double latitude = jsonObj["initial_location"]["latitude"];
            DateTime dateTime = jsonObj["initial_location"]["date_time"];
            bool isExternal = false;
            flightPlan.Passengers = passengers;
            flightPlan.CompanyName = companyName;
            flightPlan.Longitude = longitude;
            flightPlan.Latitude = latitude;
            flightPlan.IsExternal = isExternal;
            flightPlan.DateTime = dateTime;
            flightPlan.FlightId = flightId;
            dynamic segments = jsonObj["segments"];
            List<Segment> segmentList = new List<Segment>();
            foreach (var seg in segments)
            {
                Segment newSeg = new Segment();
                newSeg.Longitude = seg["longitude"];
                newSeg.Latitude = seg["latitude"];
                newSeg.TimespanSeconds = seg["timespan_seconds"];
                newSeg.FlightId = flightId;
                //{
                //    Longitude = seg["longitude"],
                //    Latitude = seg["latitude"],
                //    TimespanSeconds = seg["timespan_seconds"],
                //};
                _context.Add(newSeg);
            }
            _context.FlightItems.Add(flightPlan);
            await _context.SaveChangesAsync();

            //return CreatedAtAction("GetFlightPlan", new { id = flightPlan.Id }, flightPlan);
            return CreatedAtAction(nameof(GetFlightPlan), new { id = flightPlan.Id }, flightPlan);
        }

        // DELETE: api/FlightPlans/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<FlightPlan>> DeleteFlightPlan(string flightId)
        {
            var flightPlan = await _context.FlightItems.Where(x => x.FlightId == flightId).FirstAsync();
            if (flightPlan == null)
            {
                return NotFound();
            }

            _context.FlightItems.Remove(flightPlan);
            await _context.SaveChangesAsync();

            return flightPlan;
        }

        private bool FlightPlanExists(long id)
        {
            return _context.FlightItems.Any(e => e.Id == id);
        }
        public string SetFlightId(string flightName)
        {
            string flightId = flightName.Substring(0, 2);
            for(int i=0; i<3;i++)
            {
                flightId += _flightsNumber;
                _flightsNumber++;
                i++;
            }
            return flightId;
        }
    }
}

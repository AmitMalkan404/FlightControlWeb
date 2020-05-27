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
using System.Net.Http;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

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

        //// GET: api/FlightPlans
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<FlightPlan>>> GetFlightItems()
        //{
        //    return await _context.FlightItems.ToListAsync();
        //}

        // GET: api/FlightPlans/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FlightPlanFullData>> GetFlightPlan(string id)
        {
            string request = Request.QueryString.Value;
            var flightPlan = await _context.FlightItems.Where(x => x.FlightId == id).FirstOrDefaultAsync();
            // Check in our server DB.
            if (flightPlan != null)
            {
                List<Segment> segmentList = new List<Segment>();
                bool isExternal = false;
                FlightPlanFullData flightPlanFullData = CreateFlightPlanFullData(flightPlan.CompanyName, flightPlan.DateTime,
                    flightPlan.Latitude, flightPlan.Longitude, flightPlan.Passengers, flightPlan.FlightId,isExternal,segmentList);
                return flightPlanFullData;
            }
            try
            {
                var flightPlan2 = await CheckFlightPlanInServers(id);

            }
            catch
            {
                throw;
            }
            //catch
            //{
            //    throw new ArgumentException("This is an external flightPlan. Something went wrong with its server.");
            //}
            //if (flightPlan2 != null)
            //{
            //    return flightPlan2;
            //}
            //else
            //{
            //    return NotFound();
            //}
            return NotFound();
        }

        //// PUT: api/FlightPlans/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to, for
        //// more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutFlightPlan(long id, FlightPlan flightPlan)
        //{
        //    if (id != flightPlan.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(flightPlan).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        return NotFound();
        //        //if (!FlightPlanExists(id))
        //        //{
        //        //    return NotFound();
        //        //}
        //        //else
        //        //{
        //        //    throw;
        //        //}
        //    }

        //    return NoContent();
        //}

        // POST: api/FlightPlans
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<FlightPlan>> PostFlightPlan([FromBody] JsonElement jsonFlight)
        {
            try
            {
                string stringJsonFlight = jsonFlight.ToString();
                dynamic jsonObj = JsonConvert.DeserializeObject(stringJsonFlight);
                //long id;
                FlightPlan flightPlan = CheckValidFlightPlan(jsonObj, false);
                //int passengers = jsonObj["passengers"];
                //string companyName = jsonObj["company_name"];
                //string flightId = SetFlightId(companyName);
                //double longitude = jsonObj["initial_location"]["longitude"];
                //double latitude = jsonObj["initial_location"]["latitude"];
                //DateTime dateTime = jsonObj["initial_location"]["date_time"];
                //bool isExternal = false;
                //flightPlan.Passengers = passengers;
                //flightPlan.CompanyName = companyName;
                //flightPlan.Longitude = longitude;
                //flightPlan.Latitude = latitude;
                //flightPlan.IsExternal = isExternal;
                //flightPlan.DateTime = dateTime;
                //flightPlan.FlightId = flightId;
                dynamic segments = jsonObj["segments"];
                List<Segment> segmentList = CheckValidSegments(segments, flightPlan.FlightId, false);
                //foreach (var seg in segments)
                //{
                //    Segment newSeg = new Segment();
                //    newSeg.Longitude = seg["longitude"];
                //    newSeg.Latitude = seg["latitude"];
                //    newSeg.TimespanSeconds = seg["timespan_seconds"];
                //    newSeg.FlightId = flightId;
                //    //{
                //    //    Longitude = seg["longitude"],
                //    //    Latitude = seg["latitude"],
                //    //    TimespanSeconds = seg["timespan_seconds"],
                //    //};
                //    _context.Add(newSeg);
                //}
                _context.FlightItems.Add(flightPlan);
                await _context.SaveChangesAsync();

                //return CreatedAtAction("GetFlightPlan", new { id = flightPlan.Id }, flightPlan); 
                return CreatedAtAction(nameof(GetFlightPlan), new { id = flightPlan.Id }, flightPlan);
            }
            catch
            {
                throw;
            }
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

        public async Task<ActionResult<FlightPlanFullData>> CheckFlightPlanInServers(string id)
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
        public async Task<ActionResult<FlightPlanFullData>> GetExternalFlightFromServer(string myServerUrl,string id)
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
            dynamic json = JsonConvert.DeserializeObject(stringJsonFlight);

            FlightPlanFullData flightPlanFullData = new FlightPlanFullData();
            FlightPlan fp = CheckValidFlightPlan(json,true);
            int passengers = json["passengers"];
            string companyName = json["company_name"];
            string flightId = json["flight_id"];
            double longitude = json["initial_location"]["longitude"];
            double latitude = json["initial_location"]["latitude"];
            DateTime dateTime = json["initial_location"]["date_time"];
            bool isExternal = true;
            dynamic segments = json["segments"];
            List<Segment> segmentList = CheckValidSegments(segments, flightId,true);
            flightPlanFullData = CreateFlightPlanFullData(companyName, dateTime, latitude, longitude, passengers, flightId, isExternal, segmentList);
            return flightPlanFullData;
        }

        public FlightPlan CheckValidFlightPlan(dynamic json,bool isExternal)
        {
            FlightPlan flightPlan = new FlightPlan();
            string companyName = json["company_name"];
            double longitude = json["initial_location"]["longitude"];
            double latitude = json["initial_location"]["latitude"];
            int passengers = json["passengers"];
            string flightId = SetFlightId(companyName);
            DateTime dateTime = json["initial_location"]["date_time"];
            if (companyName == "")
            {
                //return false;
                throw new ArgumentException("Missing company name.");
            }
            if (longitude < -90 || longitude > 90)
            {
                //return false;
                throw new ArgumentException("initial_location:Longitude value is not valid.");
            }
            if (latitude < -180 || latitude > 180)
            {
                //return false;
                throw new ArgumentException("initial_location:Latitude value is not valid.");
            }
            
            if (!isExternal)
            {
                flightPlan.Passengers = passengers;
                flightPlan.CompanyName = companyName;
                flightPlan.Longitude = longitude;
                flightPlan.Latitude = latitude;
                flightPlan.IsExternal = isExternal;
                flightPlan.DateTime = dateTime;
                flightPlan.FlightId = flightId;
            }
            return flightPlan;
        }

        // if something gets stuck - change back to async task...
        public FlightPlanFullData CreateFlightPlanFullData(string companyName, DateTime dateTime,
                    double latitude, double longitude, int passengers, string flightId, bool isExternal, List<Segment> segmentList)
        {
            InitialLocation initialLocation = new InitialLocation()
            {
                Longitude = longitude,
                Latitude = latitude,
                DateTime = dateTime,
            };
            List<Segment> segments = new List<Segment>();
            if (!isExternal)
            {
                var varSegments = from segment in _context.Segment where segment.FlightId == flightId select segment;
                segments = varSegments.ToList();
            }
            else
            {
                segments = segmentList;
            }
            
            FlightPlanFullData flightPlanFullData = new FlightPlanFullData()
            {
                Passengers = passengers,
                CompanyName = companyName,
                InitialLocation = initialLocation,
                Segments = segments,
            };

            return flightPlanFullData;
        }
        public List<Segment> CheckValidSegments(dynamic checkSegmentsList, string flightId,bool isExternal)
        {
            List<Segment> segmentList = new List<Segment>();
            int i = 0;
            foreach (var seg in checkSegmentsList)
            {
                Segment newSeg = CheckEachSegment(seg, flightId, isExternal, i);
                //newSeg.Longitude = seg["longitude"];
                //newSeg.Latitude = seg["latitude"];
                //newSeg.TimespanSeconds = seg["timespan_seconds"];
                //newSeg.FlightId = flightId;
                segmentList.Add(newSeg);
                i++;
            }
            return segmentList;
            //try
            //{
            //    List<Segment> segmentList = new List<Segment>();
            //    int i = 0;
            //    foreach (var seg in checkSegmentsList)
            //    {
            //        Segment newSeg = CheckEachSegment(seg, flightId, isExternal, i);
            //        //newSeg.Longitude = seg["longitude"];
            //        //newSeg.Latitude = seg["latitude"];
            //        //newSeg.TimespanSeconds = seg["timespan_seconds"];
            //        //newSeg.FlightId = flightId;
            //        segmentList.Add(newSeg);
            //        i++;
            //    }
            //    return segmentList;
            //}
            //catch
            //{
            //    throw;
            //}

        }
        public Segment CheckEachSegment(dynamic seg,string flightId,bool isExternal, int numSegment)
        {
            
            double longitude = seg["longitude"];
            double latitude = seg["latitude"];
            double timeSpan = seg["timespan_seconds"];
            if (longitude < -90 || longitude > 90)
            {
                //return false;
                throw new ArgumentException("Longitude value is not valid in segment" + numSegment);
            }
            if (latitude < -180 || latitude > 180)
            {
                //return false;
                throw new ArgumentException("Latitude value is not valid.");
            }
            if (timeSpan < 0)
            {
                //return false;
                throw new ArgumentException("TimeSpan Seconds value is lower then 0.");
            }
            Segment newSeg = new Segment();
            {
                newSeg.Longitude = longitude;
                newSeg.Latitude = latitude;
                newSeg.TimespanSeconds = timeSpan;
                newSeg.FlightId = flightId;
            }
            if (!isExternal)
            {
                _context.Segment.Add(newSeg);
            }
            return newSeg;
        }
    }
}

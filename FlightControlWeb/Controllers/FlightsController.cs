using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlightControlWeb.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FlightControlWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightsController : ControllerBase
    {
        private readonly FlightContext _context;

        public FlightsController(FlightContext context)
        {
            _context = context;
        }

        // GET: api/Flights
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Flight>>> GetAllFlight(string relative_to)
        {
            string request = Request.QueryString.Value;
            bool isExternal = request.Contains("sync_all");
            DateTime myTime = DateTime.Parse(relative_to);
            myTime = TimeZoneInfo.ConvertTimeToUtc(myTime);
            // Get all non extrernal flights.
            List<FlightPlan> allNotExternalFlights = await _context.FlightItems.Include(x => x.SegmentsList).Include(x => x.InitialLocation).ToListAsync();
            // Get Servers flights.
            List<Server> allServers = await _context.Server.ToListAsync();
            List<Flight> resultAllFlights = new List<Flight>();
            // This is for all the non external flight.
            foreach (FlightPlan flightPlan in allNotExternalFlights)
            {
                List<Segment> segmentList = flightPlan.SegmentsList;
                string flightId = flightPlan.FlightId;
                Segment resultSegment = GetMyLocation(flightPlan.InitialLocation.Longitude, flightPlan.InitialLocation.Longitude, flightPlan.InitialLocation.DateTime, 
                    flightPlan.FlightId, myTime, segmentList);
                // The result can be null which means the flight isnt relevant any more.
                if (resultSegment == null)
                {
                    continue;
                }
                Flight myflight = CreateFlight(flightPlan.CompanyName, flightPlan.FlightId, flightPlan.Passengers, flightPlan.IsExternal, resultSegment, myTime);
                resultAllFlights.Add(myflight);
            }
            // This is all the external flights.
            if (isExternal)
            {
                List<Flight> getAllExternalFlight = await GetExternalFlights(allServers, relative_to);
                resultAllFlights.AddRange(getAllExternalFlight);
            }
            return resultAllFlights;
        }
        // DELETE: api/Flights/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<FlightPlan>> DeleteFlightPlan(string id)
        {
            var flightPlan = await _context.FlightItems.Where(x => x.FlightId == id).FirstAsync();
            if (flightPlan == null)
            {
                return NotFound();
            }

            _context.FlightItems.Remove(flightPlan);
            await _context.SaveChangesAsync();

            return flightPlan;
        }

        private bool FlightExists(long id)
        {
            return _context.Flight.Any(e => e.Id == id);
        }


        public Flight CreateFlight(string companyName, string flightId, int passengers, bool isExternal, Segment location, DateTime relativeTo)
        {
            Flight myFlight = new Flight()
            {
                CompanyName = companyName,
                FlightId = flightId,
                Passengers = passengers,
                DateTime = relativeTo,
                IsExternal = isExternal,
                Latitude = location.Latitude,
                Longitude = location.Longitude,
            };
            return myFlight;
        }

        // Get all flights from external servers.
        public async Task<List<Flight>> GetExternalFlights(List<Server> allServers, string dateTime)
        {
            List<Flight> allExternalFlights = new List<Flight>();
            for (int i = 0; i < allServers.Count; i++)
            {
                List<Flight> tempFlights = await GetExternalFlightsFromServer(allServers[i], dateTime);
                if(tempFlights == null)
                {
                    continue;
                }
                AddAllExternalFlightToDb(tempFlights, allServers[i].ServerURL);
                allExternalFlights.AddRange(tempFlights);
            }
            return allExternalFlights;
        }

        public  async Task<List<Flight>> GetExternalFlightsFromServer(Server myServer, string dateTime)
        {
            try
            {
                List<Flight> allExternalFlights = new List<Flight>();
                string url = "";
                url += myServer.ServerURL;
                url += "/api/Flights?relative_to=";
                string date = dateTime.ToString();
                url += date;
                HttpClient client = new HttpClient();
                var response = await client.GetStringAsync(url);
                string stringJsonFlight = response.ToString();
                List<Flight> flightPlanList = JsonConvert.DeserializeObject<List<Flight>>(stringJsonFlight);
                foreach(Flight flight in flightPlanList)
                {
                    flight.IsExternal = true;
                }
                return flightPlanList;
            }
            catch
            {
                return null;
            }

        }

        public Segment GetMyLocation(double longitude, double latitude, DateTime initialDateTime, string flightId, DateTime relativeTo, List<Segment> segmentList)
        {
            Segment resultSegment = new Segment();
            bool isFuture = true;
            string id = flightId;
            double thisFlightLatitude = latitude;
            double thisFlightLongitude = latitude;
            DateTime thisFlightTime = initialDateTime;
            TimeSpan secondsPassedSpanTillNow = relativeTo - thisFlightTime;
            double secondsPassedTillNow = secondsPassedSpanTillNow.TotalSeconds;
            // compare relative time and start time.
            int isOver = DateTime.Compare(relativeTo, thisFlightTime);
            // Check if the now date is before even the flight time started.
            if (isOver < 0)
            {
                return null;
            }
            int i = 0;
            Segment currSegment = new Segment();
            // Find the segment we need.
            Segment prevSegment = new Segment()
            {
                Latitude = latitude,
                Longitude = longitude,
                TimespanSeconds = 0,
            };
            foreach (var segment in segmentList)
            {
                if ((secondsPassedTillNow - segment.TimespanSeconds) < 0)
                {
                    currSegment = segment;
                    isFuture = false;
                    break;
                }
                secondsPassedTillNow -= segment.TimespanSeconds;

                prevSegment = segment;
            }
            if (isFuture)
            {
                return null;
            }
            double proportionalTime = secondsPassedTillNow / currSegment.TimespanSeconds;
            resultSegment = FindMyLocationMath(prevSegment,currSegment, proportionalTime);
            return resultSegment;
        }
        Segment FindMyLocationMath(Segment prevSegment, Segment currSegment,double proportionalTime)
        {
            double distLatitude = currSegment.Latitude -prevSegment.Latitude;
            double distLongitude = currSegment.Longitude - prevSegment.Longitude;
            double latitudeResult = prevSegment.Latitude + (proportionalTime * distLatitude);
            double longitudeResult = prevSegment.Longitude + (proportionalTime * distLongitude);
            Segment myLocation = new Segment
            {
                Latitude = latitudeResult,
                Longitude = longitudeResult,
            };
            return myLocation;
        }
        public async void AddAllExternalFlightToDb(List<Flight> allExternalFlight, string severUrl)
        {
            if(allExternalFlight == null)
            {
                return;
            }
            foreach (Flight flight in allExternalFlight)
            {
                FlightByServerId flightToDb = new FlightByServerId
                {
                    ServerId = severUrl,
                    FlightId = flight.FlightId,
                };
                _context.FlightByServerIds.Add(flightToDb);
                await _context.SaveChangesAsync();
            }
        }
    }
}
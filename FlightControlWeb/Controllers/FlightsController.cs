﻿using System;
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
            string timePattern = "yyyy-MM-ddTHH:mm:ssZ";
            DateTime myTime = DateTime.Parse(relative_to);
            myTime = TimeZoneInfo.ConvertTimeToUtc(myTime);
            //myTime = myTime.AddHours(-2);
            //DateTime relativeTo = DateTime.ParseExact(relative_to, timePattern, System.Globalization.CultureInfo.InvariantCulture);
            // Get all non extrernal flights.
            List<FlightPlan> allNotExternalFlights = await _context.FlightItems.ToListAsync();
            // Get Servers flights.
            List<Server> allServers = await _context.Server.ToListAsync();
            List<Flight> resultAllFlights = new List<Flight>();
            // This is for all the non external flight.
            foreach (var flightPlan in allNotExternalFlights)
            {
                string flightId = flightPlan.FlightId;
                Segment resultSegment = GetMyLocation(_context.Segment, flightPlan.Longitude, flightPlan.Latitude, flightPlan.DateTime, flightPlan.FlightId, myTime);
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


        // GET: api/Flights/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Flight>> GetFlight(string id)
        {
            FlightPlan flightPlan = await _context.FlightItems.Where(x => x.FlightId == id).FirstAsync();
            Segment resultSegment;
            DateTime relativeTo = new DateTime(2009, 8, 1, 0, 0, 0);
            //var flightPlan = await _context.FlightItems.FindAsync(id);

            if (flightPlan == null)
            {
                return NotFound();
            }
            // need to use the func and get a segment with the longitude and the latitude.
            resultSegment = GetMyLocation(_context.Segment, flightPlan.Longitude, flightPlan.Latitude, flightPlan.DateTime, flightPlan.FlightId, relativeTo);
            Flight myflight = CreateFlight(flightPlan.CompanyName, flightPlan.FlightId, flightPlan.Passengers, flightPlan.IsExternal, resultSegment, relativeTo);
            //myflight.Longitude = resultSegment.Latitude;
            //myflight.Latitude = resultSegment.Latitude;
            return myflight;
        }

        //// PUT: api/Flights/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to, for
        //// more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutFlight(long id, Flight flight)
        //{
        //    if (id != flight.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(flight).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!FlightExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        //// POST: api/Flights
        //// To protect from overposting attacks, enable the specific properties you want to bind to, for
        //// more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        //[HttpPost]
        //public async Task<ActionResult<Flight>> PostFlight(Flight flight)
        //{
        //    _context.Flight.Add(flight);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetFlight", new { id = flight.Id }, flight);
        //}

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
                AddAllExternalFlightToDb(tempFlights, allServers[i].ServerURL);
                allExternalFlights.AddRange(tempFlights);
            }
            return allExternalFlights;
        }

        public async Task<List<Flight>> GetExternalFlightsFromServer(Server myServer, string dateTime)
        {
            List<Flight> allExternalFlights = new List<Flight>();
            //string url = "https://";
            string url = "";
            url += myServer.ServerURL;
            //url += "/api/Flights?relative_to=";
            url += "Flights?relative_to=";
            //dateTime = dateTime.AddHours(2);
            //dateTime.ToString("yyyy-MM-dd HH':'mm':'ss");
            string date = dateTime.ToString();
            url += date;
            HttpClient client = new HttpClient();
            var response = await client.GetStringAsync(url);
            string stringJsonFlight = response.ToString();
            dynamic dJson = JsonConvert.DeserializeObject(stringJsonFlight);
            foreach(var flight in dJson)
            {
                JToken json = flight;
                Segment segmentFlight = new Segment
                {
                    Latitude = flight["latitude"],
                    Longitude = flight["longitude"],
                };
                int passengers = flight["passengers"];
                string companyName = flight["company_name"];
                string flightId = flight["flight_id"];
                DateTime dateTimeFlight = flight["date_time"];
                bool isExternalFlight = true;
                Flight flightcreateFlight = CreateFlight(companyName, flightId, passengers, isExternalFlight, segmentFlight, dateTimeFlight);
                allExternalFlights.Add(flightcreateFlight);
            }
            return allExternalFlights;
        }

        public Segment GetMyLocation(DbSet<Segment> contextSegment, double longitude, double latitude, DateTime initialDateTime, string flightId, DateTime relativeTo)
        {
            Segment resultSegment = new Segment();
            bool isFuture = true;
            string id = flightId;
            double thisFlightLatitude = latitude;
            double thisFlightLongitude = latitude;
            DateTime thisFlightTime = initialDateTime;
            TimeSpan secondsPassedSpanTillNow = relativeTo - thisFlightTime;
            double secondsPassedTillNow = secondsPassedSpanTillNow.TotalSeconds;
            var segmentList = from segment in contextSegment where segment.FlightId == id select segment;
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


            //double distX = currSegment.Latitude -prevSegment.Latitude;
            //double distY = currSegment.Longitude - prevSegment.Longitude;
            //double distance = Math.Sqrt(Math.Pow(distX, 2) + Math.Pow(distY, 2));
            //double proportionalDistance = proportionalTime * distance;
            //double latitudeResult = prevSegment.Latitude + (prevSegment.Latitude * proportionalTime);
            //double longitudeResult = prevSegment.Longitude + (prevSegment.Longitude * proportionalTime);
            //double latitudeResult = prevSegment.Latitude - ((proportionalDistance * (prevSegment.Latitude - currSegment.Longitude)) / distance);
            //double longitudeResult = prevSegment.Longitude - ((proportionalDistance * (prevSegment.Longitude - currSegment.Longitude)) / distance);
            Segment myLocation = new Segment
            {
                Latitude = latitudeResult,
                Longitude = longitudeResult,
            };
            return myLocation;
        }
        public async void AddAllExternalFlightToDb(List<Flight> allExternalFlight, string severUrl)
        {
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
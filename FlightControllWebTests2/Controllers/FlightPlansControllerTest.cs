using Microsoft.VisualStudio.TestTools.UnitTesting;
using FlightControlWeb.Controllers;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using FlightControlWeb.Models;
using FlightControlWeb;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;

namespace FlightControllWeb.Controllers
{
    [TestClass()]
    public class FlightsControllerTests
    {
        // Creating the DBContext and the Controller
        FlightContext _FlightDBContextMock;
        private readonly FlightPlanController fpc;
        public FlightsControllerTests()
        {
            string[] args = { };
            var d = new DbContextOptionsBuilder<FlightContext>();
            var h = Program.CreateHostBuilder(args);
            d.UseInMemoryDatabase("DBName");
            _FlightDBContextMock = new FlightContext(d.Options);
            fpc = new FlightPlanController(_FlightDBContextMock);
        }

        [TestMethod()]
        public async Task PostFlightPlanTest()
        {

            // Assign

            InitialLocation location = new InitialLocation();
            DateTime dateTime = new DateTime(2020, 5, 30, 18, 0, 0);
            location.DateTime = dateTime;
            location.Longitude = 30.522;
            location.Latitude = 34.943;
            Segment seg1 = new Segment();
            seg1.FlightId = "EL-012345";
            seg1.Longitude = 33.567;
            seg1.Latitude = 31.865;
            seg1.TimespanSeconds = 180;

            Segment seg2 = new Segment();
            seg2.FlightId = "EL-012345";
            seg2.Longitude = 35.674;
            seg2.Latitude = 30.651;
            seg2.TimespanSeconds = 720;

            List<Segment> segList = new List<Segment>();
            segList.Add(seg1);
            segList.Add(seg2);

            FlightPlan flightPlan1 = new FlightPlan();
            flightPlan1.Id = 1255;
            flightPlan1.FlightId = "EL-012345";
            flightPlan1.Passengers = 168;
            flightPlan1.CompanyName = "EL-AL";
            flightPlan1.InitialLocation = location;
            flightPlan1.SegmentsList = segList;
            flightPlan1.IsExternal = false;


            // Act

            double initialLatitude = 34.943;
            Task<ActionResult<FlightPlan>> apiFlight = fpc.PostFlightPlan(flightPlan1);
            var contextFlights = await _FlightDBContextMock.FlightItems.ToListAsync();
            var contextFlight = contextFlights.Where(a => a.FlightId.CompareTo("EL-012345") == 0).First();


            //Assert

            Assert.IsNotNull(contextFlight);
            Assert.IsTrue(contextFlight.FlightId.CompareTo("EL-012345") ==0);
            Assert.IsTrue(contextFlight.Passengers == 168);
            Assert.AreEqual(segList, contextFlight.SegmentsList);
            Assert.AreEqual(contextFlight.InitialLocation.Latitude, initialLatitude);
        }
    }

}
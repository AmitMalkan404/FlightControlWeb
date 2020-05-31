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

namespace FlightControllWeb.Tests
{
    [TestClass()]
    public class FlightsControllerTests
    {
        FlightContext _FlightDBContextMock;
        private readonly FlightPlansController fpc;
        public FlightsControllerTests()
        {
            string[] args = { };
            var d = new DbContextOptionsBuilder<FlightContext>();
            var h = Program.CreateHostBuilder(args);
            d.UseInMemoryDatabase("DBName");
            _FlightDBContextMock = new FlightContext(d.Options);
            fpc = new FlightPlansController(_FlightDBContextMock);
        }

        [TestMethod()]
        public async Task<ActionResult<FlightPlan>> GetFlightPlanTest()
        {
            FlightsControllerTests controllerTests = new FlightsControllerTests();


            InitialLocation location = new InitialLocation();
            DateTime dateTime = new DateTime(2020, 5, 31, 18, 0, 0);
            location.DateTime = dateTime;
            location.Longitude = 30.522;
            location.Latitude = 34.943;
            Segment seg1 = new Segment();
            seg1.FlightId = "LY1255";
            seg1.Longitude = 33.567;
            seg1.Latitude = 31.865;
            seg1.TimespanSeconds = 180;

            Segment seg2 = new Segment();
            seg2.FlightId = "LY1255";
            seg2.Longitude = 35.674;
            seg2.Latitude = 30.651;
            seg2.TimespanSeconds = 720;

            List<Segment> segList = new List<Segment>();
            segList.Add(seg1);
            segList.Add(seg2);

            //FlightPlan flightPlan1 = new FlightPlan("LY1255", 168, "EL-AL", location, segList, false);
            FlightPlan flightPlan1 = new FlightPlan();
            flightPlan1.FlightId = "LY1255";
            flightPlan1.Passengers = 168;
            flightPlan1.CompanyName = "EL-AL";
            flightPlan1.InitialLocation = location;
            flightPlan1.SegmentsList = segList;
            flightPlan1.IsExternal = false;


            controllerTests._FlightDBContextMock.Add(flightplan1);
            controllerTests.fpc.PostFlightPlan(flightPlan1);


            //FlightPlan flightPlan = await _context.FlightItems.Include(x => x.SegmentsList).Include(x => x.InitialLocation).Where(x => x.FlightId == id).FirstOrDefaultAsync();
            //Assert.Fail();
        }
    }

}
//using Microsoft.Extensions.Caching.Memory;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//namespace FlightControlWeb.Models
//{
//	public class FlightPlanManager
//	{
//		private readonly FlightContext _context;
//		public FlightPlanManager(FlightContext context)
//		{
//			_context = context;
//		}


//        public async Task<ActionResult<IEnumerable<FlightPlan>>> GetFlightItems()
//        {
//            return await _context.FlightItems.ToListAsync();
//        }

//        public async Task<ActionResult<FlightPlan>> GetFlightPlan(long id)
//        {
//            var flightPlan = await _context.FlightItems.FindAsync(id);

//            return flightPlan;
//        }

//        // POST: api/FlightPlans
//        // To protect from overposting attacks, enable the specific properties you want to bind to, for
//        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
//        [HttpPost]
//        public async void PostFlightPlan(FlightPlan flightPlan)
//        {
//            await _context.SaveChangesAsync();

//        }

//        // DELETE: api/FlightPlans/5
//        [HttpDelete("{id}")]
//        public async Task<ActionResult<FlightPlan>> DeleteFlightPlan(long id)
//        {
//            var flightPlan = await _context.FlightItems.FindAsync(id);
//            if (flightPlan == null)
//            {
//                return NotFound();
//            }

//            _context.FlightItems.Remove(flightPlan);
//            await _context.SaveChangesAsync();

//            return flightPlan;
//        }

//        private bool FlightPlanExists(long id)
//        {
//            return _context.FlightItems.Any(e => e.Id == id);
//        }

//        public void PutEntryState(FlightPlan flightPlan)
//        {
//            _context.Entry(flightPlan).State = EntityState.Modified;
//        }
//        public void SaveChanges()
//        {
//            _context.SaveChangesAsync();
//        }
//    }
//}

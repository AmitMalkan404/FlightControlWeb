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
using Newtonsoft.Json.Linq;

namespace FlightControlWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServersController : ControllerBase
    {
        private readonly FlightContext _context;

        public ServersController(FlightContext context)
        {
            _context = context;
        }

        // GET: api/Servers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Server>>> GetServer()
        {
            List<Server> serverList = new List<Server>();
            var serverFromDb =  await _context.Server.ToListAsync();
            foreach (var item in serverFromDb)
            {
                Server server = new Server
                {
                    ServerId = item.ServerId,
                    ServerURL= item.ServerURL,
                };
                serverList.Add(server);
            };
            return serverList;
        }

        //// GET: api/Servers/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<Server>> GetServer(string id)
        //{
        //    var server = await _context.Server.FindAsync(id);

        //    if (server == null)
        //    {
        //        return NotFound();
        //    }

        //    return server;
        //}

        // PUT: api/Servers/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.

        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutServer(string id, Server server)
        //{
        //    if (id != server.ServerId)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(server).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!ServerExists(id))
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

        // POST: api/Servers
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Server>> PostServer([FromBody] JsonElement jsonFlight)
        {
            string stringJsonFlight = jsonFlight.ToString();
            dynamic jsonObj = JsonConvert.DeserializeObject(stringJsonFlight);
            Server server = CreateServer(jsonObj);
            //Server server = new Server()
            //{
            //    ServerId = jsonObj["ServerId"],
            //    ServerURL = jsonObj["ServerURL"],
            //};

            try
            {

                _context.Server.Add(server);
            }
            catch
            {
                throw new ArgumentException("Failed to add the server. Check if the properties are written correctly.");
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ServerExists(server.ServerId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetServer", new { id = server.ServerId }, server);
        }

        // DELETE: api/Servers/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Server>> DeleteServer(string id)
        {
            try
            {
                var server = await _context.Server.Where(x => x.ServerId == id).FirstAsync();
                _context.Server.Remove(server);
                await _context.SaveChangesAsync();
                return server;
            } 
            catch
            {
                throw new ArgumentException("Failed to remove the server. Check the server id again.");
            }
            //await _context.SaveChangesAsync();
        }

        private bool ServerExists(string id)
        {
            return _context.Server.Any(e => e.ServerId == id);
        }

        Server CreateServer(JToken inupt)
        {

            Server server = new Server()
            {
                ServerId = (string)inupt["ServerId"],
                ServerURL = (string)inupt["ServerURL"],
            };
            if (server.ServerId == "" || server.ServerURL == "")
            {
                return null;
            }
            return server;
        }
    }
}

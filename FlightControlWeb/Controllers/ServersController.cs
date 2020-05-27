﻿using System;
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
            return await _context.Server.ToListAsync();
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
            Server server = new Server()
            {
                ServerId = jsonObj["ServerId"],
                ServerURL = jsonObj["ServerURL"],
            };
            try
            {
                bool validServer = CheckValidServer(server);
                _context.Server.Add(server);
            }
            catch
            {
                throw new ArgumentException("One of the server details is not in a correct format. Please try again.");
            }
            _context.Server.Add(server);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return Conflict();
                //else
                //{
                //    throw;
                //}
            }

            return CreatedAtAction("GetServer", new { id = server.ServerId }, server);
        }

        // DELETE: api/Servers/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Server>> DeleteServer(string id)
        {
            var server = await _context.Server.Where(x => x.ServerId == id).FirstAsync();
            if (server == null)
            {
                return NotFound();
            }

            _context.Server.Remove(server);
            await _context.SaveChangesAsync();

            return server;
        }

        private bool ServerExists(string id)
        {
            return _context.Server.Any(e => e.ServerId == id);
        }
        public bool CheckValidServer(Server server)
        {
            if ((server.ServerURL == "") || server.ServerId == "")
            {
                throw new ArgumentException("One of the server details is not in a correct format. Please try again.");
            }
            return true;
        }
    }
}

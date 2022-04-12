﻿#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using E_proc.Models;
using E_proc.Repositories.Interfaces;
using E_proc.Models.StatusModel;
using E_proc.MyHub;
using Microsoft.AspNetCore.SignalR;

namespace E_proc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressesController : ControllerBase
    {
        private readonly AuthContext _context;
        private readonly IAddressRepository _reposAddress;
        private readonly IHubContext<NotificationHub> _notificationHubContext;

        public AddressesController(AuthContext context,IAddressRepository reposAddress, IHubContext<NotificationHub> notificationHubContext)
        {
            _context = context;
            _reposAddress=reposAddress;
            _notificationHubContext = notificationHubContext;

        }

        // GET: api/Addresses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Address>>> GetAddress()
        {
          
            return await _context.Address.ToListAsync();
        }

        // GET: api/Addresses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Address>> GetAddress(int id)
        {

            await _notificationHubContext.Clients.All.SendAsync("Send","f");

            var address = await _context.Address.FindAsync(id);
           
            if (address == null)
            {
                return NotFound();
            }

            return address;
        }

        // PUT: api/Addresses/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAddress(int id, Address address)
        {

           var adr= await _reposAddress.UpdateAsync(id,address);
            if (adr == null) return new Success(false, "message.notFound");
            return new Success(true, "message.Address Updated",adr);


        }

        // POST: api/Addresses
        [HttpPost]
        public async Task<ActionResult<Address>> PostAddress(Address address)
        {
            _context.Address.Add(address);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAddress", new { id = address.Id }, address);
        }

        // DELETE: api/Addresses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var address = await _context.Address.FindAsync(id);
            if (address == null)
            {
                return NotFound();
            }

            _context.Address.Remove(address);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AddressExists(int id)
        {
            return _context.Address.Any(e => e.Id == id);
        }
    }
}

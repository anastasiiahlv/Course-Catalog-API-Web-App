using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CourseCatalogAPIWebApp.Models;

namespace CourseCatalogAPIWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly CourseCatalogAPIContext _context;

        public RolesController(CourseCatalogAPIContext context)
        {
            _context = context;
        }

        private IEnumerable<object> FormResult(List<Role> roles)
        {
            var result = roles.Select(r => new
            {
                roleId = r.Id,
                name = r.Name,
                participants = r.Participants?.Select(participant => new
                {
                    participantId = participant?.Id,
                    FirstName = participant?.FirstName,
                    LastName = participant?.LastName,
                    Email = participant?.Email,
                    PhoneNumber = participant?.PhoneNumber,
                })
            }).ToList();

            return result;
        }

        private object FormResponse(string m, int c)
        {
            object response = new
            {
                code = c,
                message = m
            };

            return response;
        }

        // GET: api/Roles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Role>>> GetRoles()
        {
            var roles = await _context.Roles
                .Include(c => c.Participants)
                .ToListAsync();

            var result = FormResult(roles);

            return Ok(new
            {
                code = 200,
                data = result
            });
        }

        // GET: api/Roles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Role>> GetRole(int id)
        {
            var roles = await _context.Roles
                .Include(c => c.Participants)
                .ToListAsync();

            var role = await _context.Roles.FindAsync(id);

            if (role == null)
            {
                return NotFound(FormResponse("There is no role with this ID.", 404));
            }

            var result = FormResult(new List<Role> { role });
            return Ok(new
            {
                code = 200,
                data = result
            });
        }

        // PUT: api/Roles/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRole(int id, Role role)
        {
            if (id != role.Id)
            {
                return BadRequest(FormResponse("The role update request contains an invalid identifier.", 400));
            }

            _context.Entry(role).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoleExists(id))
                {
                    return NotFound(FormResponse("There is no role with this ID.", 404));
                }
                else
                {
                    throw;
                }
            }

            return Ok(FormResponse("Updated successfully.", 200));
        }

        // POST: api/Roles
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Role>> PostRole(Role role)
        {
            if (_context.Roles.Any(c => c.Name == role.Name))
            {
                return Conflict(FormResponse("A role with this name and info already exists.", 409));
            }

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            var result = new
            {
                code = 201,
                data = role
            };

            return CreatedAtAction("GetRole", new { id = role.Id }, result);
        }

        // DELETE: api/Roles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return NotFound(FormResponse("There is no role with this ID.", 404));
            }

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RoleExists(int id)
        {
            return _context.Roles.Any(e => e.Id == id);
        }
    }
}

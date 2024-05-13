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
    public class ParticipantsController : ControllerBase
    {
        private readonly CourseCatalogAPIContext _context;

        public ParticipantsController(CourseCatalogAPIContext context)
        {
            _context = context;
        }

        private IEnumerable<object> FormResult(List<Participant> participants)
        {
            var result = participants.Select(p => new
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Email = p.Email,
                PhoneNumber = p.PhoneNumber,
                Role =  p.Role?.Name,
                Courses = p.Courses?.Select(course => new
                {
                    courseId = course?.Id,
                    courseName = course?.Name,
                    courseInfo = course?.Info,
                    level = course?.Level?.Name
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

        // GET: api/Participants
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Participant>>> GetParticipants()
        {
            var participants = await _context.Participants
                .Include(p => p.Role)
                .Include(p => p.Courses)
                .ThenInclude(course => course.Level)
                .ToListAsync();

            var result = FormResult(participants);

            return Ok(new
            {
                code = 200,
                data = result
            });
        }

        // GET: api/Participants/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Participant>> GetParticipant(int id)
        {
            var participants = await _context.Participants
                .Include(p => p.Role)
                .Include(p => p.Courses)
                .ThenInclude(course => course.Level)
                .ToListAsync();

            var participant = await _context.Participants.FindAsync(id);

            if (participant == null)
            {
                return NotFound(FormResponse("There is no participant with this ID.", 404));
            }

            var result = FormResult(new List<Participant> { participant });
            return Ok(new
            {
                code = 200,
                data = result
            });
        }

        // PUT: api/Participants/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutParticipant(int id, Participant participant)
        {
            if (id != participant.Id)
            {
                return BadRequest(FormResponse("The participant update request contains an invalid identifier.", 400));
            }

            _context.Entry(participant).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ParticipantExists(id))
                {
                    return NotFound(FormResponse("There is no participant with this ID.", 404));
                }
                else
                {
                    throw;
                }
            }

            return Ok(FormResponse("Updated successfully.", 200));
        }

        // POST: api/Participants
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Participant>> PostParticipant(Participant participant)
        {
            if (_context.Participants.Any(p => p.Email == participant.Email))
            {
                return Conflict(FormResponse("A participant with this email already exists.", 409));
            }

            _context.Participants.Add(participant);
            await _context.SaveChangesAsync();

            var result = new
            {
                code = 201,
                data = participant
            };

            return CreatedAtAction("GetParticipant", new { id = participant.Id }, result);
        }

        // DELETE: api/Participants/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteParticipant(int id)
        {
            var participant = await _context.Participants.FindAsync(id);
            if (participant == null)
            {
                return NotFound(FormResponse("There is no participant with this ID.", 404));
            }

            _context.Participants.Remove(participant);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ParticipantExists(int id)
        {
            return _context.Participants.Any(e => e.Id == id);
        }
    }
}

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
                participantId = p.Id,
                firstName = p.FirstName,
                lastName = p.LastName,
                email = p.Email,
                phoneNumber = p.PhoneNumber,
                //role = p.Role != null ? p.Role.Name : null,
                roleId = p.RoleId,
                courses = p.Courses?.Select(course => new
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
            var participant = await _context.Participants
                .Include(p => p.Role)
                .Include(p => p.Courses)
                .ThenInclude(course => course.Level)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (participant == null)
            {
                return NotFound(new { message = "Немає користувача з таким ID.", code = 404 });
            }

            return Ok(new
            {
                code = 200,
                data = participant
            });
        }

        // PUT: api/Participants/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutParticipant(int id, Participant participant)
        {
            if (id != participant.Id)
            {
                return BadRequest(FormResponse("Запит на оновлення користувача містить невірний ідентифікатор.", 400));
            }
            var existingParticipant = await _context.Participants
                    .FirstOrDefaultAsync(p => p.Email == participant.Email && p.Id != id);
            if (existingParticipant != null)
            {
                return Conflict(FormResponse("Користувач з такою електронною поштою вже існує.", 409));
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
                    return NotFound(FormResponse("Немає користувача з таким ID.", 404));
                }
                else
                {
                    throw;
                }
            }

            return Ok(FormResponse("Оновлення успішне.", 200));
        }

        // POST: api/Participants
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Participant>> PostParticipant(Participant participant)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                               .Select(e => e.ErrorMessage)
                                               .ToList();
                return BadRequest(new { message = "Валідація не пройшла успішно.", errors });
            }

            if (_context.Participants.Any(p => p.Email == participant.Email))
            {
                return Conflict(FormResponse("Користувач з такою електронною поштою вже існує.", 409));
            }

            _context.Participants.Add(participant);
            await _context.SaveChangesAsync();

            var result = new
            {
                code = 201,
                data = new
                {
                    participantId = participant.Id,
                    firstName = participant.FirstName,
                    lastName = participant.LastName,
                    email = participant.Email,
                    phoneNumber = participant.PhoneNumber,
                    roleId = participant.RoleId,
                    courses = participant.Courses
                }
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
                return NotFound(FormResponse("Немає користувача з таким ID.", 404));
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

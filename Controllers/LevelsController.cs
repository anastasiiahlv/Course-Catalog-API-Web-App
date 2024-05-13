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
    public class LevelsController : ControllerBase
    {
        private readonly CourseCatalogAPIContext _context;

        public LevelsController(CourseCatalogAPIContext context)
        {
            _context = context;
        }

        private IEnumerable<object> FormResult(List<Level> levels)
        {
            var result = levels.Select(l => new
            {
                Id = l.Id,
                Name = l.Name,
                Courses = l.Courses?.Select(course => new
                {
                    courseId = course?.Id,
                    courseName = course?.Name,
                    courseInfo = course?.Info,
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

        // GET: api/Levels
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Level>>> GetLevels()
        {
            var levels = await _context.Levels
                .Include(c => c.Courses)
                .ToListAsync();

            var result = FormResult(levels);

            return Ok(new
            {
                code = 200,
                data = result
            });
        }

        // GET: api/Levels/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Level>> GetLevel(int id)
        {
            var levels = await _context.Levels
                 .Include(c => c.Courses)
                 .ToListAsync();

            var level = await _context.Levels.FindAsync(id);

            if (level == null)
            {
                return NotFound(FormResponse("There is no level with this ID.", 404));
            }

            var result = FormResult(new List<Level> { level });
            return Ok(new
            {
                code = 200,
                data = result
            });
        }

        // PUT: api/Levels/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLevel(int id, Level level)
        {
            if (id != level.Id)
            {
                return BadRequest(FormResponse("The level update request contains an invalid identifier.", 400));
            }

            _context.Entry(level).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LevelExists(id))
                {
                    return NotFound(FormResponse("There is no level with this ID.", 404));
                }
                else
                {
                    throw;
                }
            }

            return Ok(FormResponse("Updated successfully.", 200));
        }

        // POST: api/Levels
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Level>> PostLevel(Level level)
        {
            if (_context.Levels.Any(l => l.Name == level.Name))
            {
                return Conflict(FormResponse("A level with this name already exists.", 409));
            }

            _context.Levels.Add(level);
            await _context.SaveChangesAsync();

            var result = new
            {
                code = 201,
                data = level
            };

            return CreatedAtAction("GetLevel", new { id = level.Id }, result);
        }

        // DELETE: api/Levels/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLevel(int id)
        {
            var level = await _context.Levels.FindAsync(id);
            if (level == null)
            {
                return NotFound(FormResponse("There is no level with this ID.", 404));
            }

            _context.Levels.Remove(level);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LevelExists(int id)
        {
            return _context.Levels.Any(e => e.Id == id);
        }
    }
}

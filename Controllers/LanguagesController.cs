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
    public class LanguagesController : ControllerBase
    {
        private readonly CourseCatalogAPIContext _context;

        public LanguagesController(CourseCatalogAPIContext context)
        {
            _context = context;
        }

        private IEnumerable<object> FormResult(List<Language> languages)
        {
            var result = languages.Select(l => new
            {
                Id = l.Id,
                Name = l.Name,
                Courses = l.Courses?.Select(course => new
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

        // GET: api/Languages
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Language>>> GetLanguages()
        {
            var languages = await _context.Languages
                .Include(c => c.Courses)
                .ThenInclude(course => course.Level)
                .ToListAsync();

            var result = FormResult(languages);

            return Ok(new
            {
                code = 200,
                data = result
            });
        }

        // GET: api/Languages/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Language>> GetLanguage(int id)
        {
            var languages = await _context.Languages
                .Include(c => c.Courses)
                .ThenInclude(course => course.Level)
                .ToListAsync();

            var language = await _context.Languages.FindAsync(id);

            if (language == null)
            {
                return NotFound(FormResponse("There is no language with this ID.", 404));
            }

            var result = FormResult(new List<Language> { language });
            return Ok(new
            {
                code = 200,
                data = result
            });
        }

        // PUT: api/Languages/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLanguage(int id, Language language)
        {
            if (id != language.Id)
            {
                return BadRequest(FormResponse("The language update request contains an invalid identifier.", 400));
            }

            _context.Entry(language).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LanguageExists(id))
                {
                    return NotFound(FormResponse("There is no language with this ID.", 404));
                }
                else
                {
                    throw;
                }
            }

            return Ok(FormResponse("Updated successfully.", 200));
        }

        // POST: api/Languages
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Language>> PostLanguage(Language language)
        {
            if (_context.Languages.Any(l => l.Name == language.Name))
            {
                return Conflict(FormResponse("A language with this name already exists.", 409));
            }

            _context.Languages.Add(language);
            await _context.SaveChangesAsync();

            var result = new
            {
                code = 201,
                data = language
            };

            return CreatedAtAction("GetLanguage", new { id = language.Id }, result);
        }

        // DELETE: api/Languages/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLanguage(int id)
        {
            var language = await _context.Languages.FindAsync(id);
            if (language == null)
            {
                return NotFound(FormResponse("There is no category with this ID.", 404));
            }

            _context.Languages.Remove(language);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LanguageExists(int id)
        {
            return _context.Languages.Any(e => e.Id == id);
        }
    }
}

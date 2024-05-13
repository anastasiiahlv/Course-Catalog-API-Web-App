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
    public class CoursesController : ControllerBase
    {
        private readonly CourseCatalogAPIContext _context;

        public CoursesController(CourseCatalogAPIContext context)
        {
            _context = context;
        }

        private IEnumerable<object> FormResult(List<Course> courses)
        {
            var result = courses.Select(c => new
            {
                Id = c.Id,
                Name = c.Name,
                Info = c.Info,
                Level = c.Level?.Name,
                Categories = c.Categories?.Select(category => new
                {
                    categoryId = category?.Id,
                    categoryName = category?.Name
                }),
                Languages = c.Languages?.Select(language => new
                {
                    languageId = language?.Id,
                    languageName = language?.Name
                }),
                Participants = c.Participants?.Select(participant => new
                {
                    participantId = participant?.Id,
                    FirstName = participant?.FirstName,
                    LastName = participant?.LastName,
                    Email = participant?.Email,
                    PhoneNumber = participant?.PhoneNumber,
                    Role = participant?.Role?.Name
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

        // GET: api/Courses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Course>>> GetCourses()
        {
            var courses = await _context.Courses
                .Include(c => c.Categories)
                .Include(c => c.Languages)
                .Include(c => c.Level)
                .Include(c => c.Participants)
                .ThenInclude(p => p.Role)
                .ToListAsync();

            var result = FormResult(courses);

            return Ok(new
            {
                code = 200,
                data = result
            });
        }

        // GET: api/Courses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Course>> GetCourse(int id)
        {
            var courses = await _context.Courses
                .Include(c => c.Categories)
                .Include(c => c.Languages)
                .Include(c => c.Level)
                .Include(c => c.Participants)
                .ThenInclude(p => p.Role)
                .ToListAsync();

            var course = await _context.Courses.FindAsync(id);

            if (course == null)
            {
                return NotFound(FormResponse("There is no course with this ID.", 404));
            }

            var result = FormResult(new List<Course> { course });
            return Ok(new
            {
                code = 200,
                data = result
            });
        }

        // PUT: api/Courses/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCourse(int id, Course course)
        {
            if (id != course.Id)
            {
                return BadRequest(FormResponse("The course update request contains an invalid identifier.", 400));
            }

            _context.Entry(course).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseExists(id))
                {
                    return NotFound(FormResponse("There is no course with this ID.", 404));
                }
                else
                {
                    throw;
                }
            }

            return Ok(FormResponse("Updated successfully.", 200));
        }

        // POST: api/Courses
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Course>> PostCourse(Course course)
        {
            if (_context.Courses.Any(c => c.Name == course.Name) && _context.Courses.Any(c => c.Info == course.Info))
            {
                return Conflict(FormResponse("A course with this name and info already exists.", 409));
            }

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            var result = new
            {
                code = 201,
                data = course
            };

            return CreatedAtAction("GetCourse", new { id = course.Id }, result);
        }

        // DELETE: api/Courses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound(FormResponse("There is no course with this ID.", 404));
            }

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.Id == id);
        }
    }
}

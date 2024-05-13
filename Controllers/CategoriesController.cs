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
    public class CategoriesController : ControllerBase
    {
        private readonly CourseCatalogAPIContext _context;

        public CategoriesController(CourseCatalogAPIContext context)
        {
            _context = context;
        }

        private IEnumerable<object> FormResult(List<Category> categories)
        {
            var result = categories.Select(c => new
            {
                Id = c.Id,
                Name = c.Name,
                Courses = c.Courses?.Select(course => new
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

        // GET: api/Categories
        [HttpGet]
        public async Task<ActionResult<Category>> GetCategories()
        {
            var categories = await _context.Categories
                .Include(c => c.Courses)
                .ThenInclude(course => course.Level)
                .ToListAsync();

            var result = FormResult(categories);

            return Ok(new
            {
                code = 200,
                data = result
            });
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            var categories = await _context.Categories
                .Include(c => c.Courses)
                .ThenInclude(course => course.Level)
                .ToListAsync();

            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound(FormResponse("There is no category with this ID.", 404));
            }

            var result = FormResult(new List<Category> { category });
            return Ok(new
            {
                code = 200,
                data = result
            });
        }

        // PUT: api/Categories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(int id, Category category)
        {
            if (id != category.Id)
            {
                return BadRequest(FormResponse("The category update request contains an invalid identifier.", 400));
            }

            _context.Entry(category).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
                {
                    return NotFound(FormResponse("There is no category with this ID.", 404));
                }
                else
                {
                    throw;
                }
            }

            return Ok(FormResponse("Updated successfully.", 200));
        }

        // POST: api/Categories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Category>> PostCategory(Category category)
        {
            if (_context.Categories.Any(c => c.Name == category.Name))
            {
                return Conflict(FormResponse("A category with this name already exists.", 409));
            }

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            var result = new
            {
                code = 201,
                data = category
            };

            return CreatedAtAction("GetCategory", new { id = category.Id }, result);
        }

        // DELETE: api/Categories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound(FormResponse("There is no category with this ID.", 404));
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}

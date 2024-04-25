using Microsoft.EntityFrameworkCore;
using System.Net;

namespace CourseCatalogAPIWebApp.Models
{
    public class CourseCatalogAPIContext: DbContext
    {
        public CourseCatalogAPIContext()
        {
            Database.EnsureCreated();
        }

        public CourseCatalogAPIContext(DbContextOptions<CourseCatalogAPIContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Course> Courses { get; set; }
        public virtual DbSet<Language> Languages { get; set; }
        public virtual DbSet<Level> Levels { get; set; }
        public virtual DbSet<Participant> Participants { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
    }
}

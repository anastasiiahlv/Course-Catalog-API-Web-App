using System.ComponentModel.DataAnnotations;

namespace CourseCatalogAPIWebApp.Models
{
    public class Language
    {
        public Language()
        {
            Courses = new List<Course>();
        }
        [Key]
        public int Id { get; set; }

        [Display(Name = "Назва")]
        [Required(ErrorMessage = "Поле не повинно бути  порожнім")]
        [StringLength(30, ErrorMessage = "Назва не має перевищувати 30 символів")]
        public string Name { get; set; } = null!;

        public virtual ICollection<Course> Courses { get; set; } 
    }
}

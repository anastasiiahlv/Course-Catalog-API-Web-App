using System.ComponentModel.DataAnnotations;

namespace CourseCatalogAPIWebApp.Models
{
    public class Course
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Назва")]
        [Required(ErrorMessage = "Поле не повинно бути  порожнім")]
        [StringLength(100, ErrorMessage = "Назва не має перевищувати 100 символів")]
        public string Name { get; set; } = null!;

        [Display(Name = "Опис")]
        [Required(ErrorMessage = "Поле не повинно бути  порожнім")]
        public string Info { get; set; } = null!;

        [Display(Name = "Рівень")]
        public int LevelId { get; set; } 
        public virtual Level? Level { get; set; }

        public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
        public virtual ICollection<Language> Languages { get; set; } = new List<Language>();
        public virtual ICollection<Participant> Participants { get; set; } = new List<Participant>();
    }
}

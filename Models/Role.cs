using System.ComponentModel.DataAnnotations;

namespace CourseCatalogAPIWebApp.Models
{
    public class Role
    {
        public Role()
        {
            Participants = new List<Participant>();
        }
        [Key]
        public int Id { get; set; }

        [Display(Name = "Назва")]
        [Required(ErrorMessage = "Поле не повинно бути  порожнім")]
        [StringLength(30, ErrorMessage = "Назва не має перевищувати 30 символів")]
        public string Name { get; set; } = null!;
        public virtual ICollection<Participant> Participants { get; set; }
    }
}

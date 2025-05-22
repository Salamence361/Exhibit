using System.ComponentModel.DataAnnotations;


namespace fly.Models
{
    public class Category
    {
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Название категории обязательно.")]
        [StringLength(255, ErrorMessage = "Название не должно превышать 255 символов.")]
        [Display(Name = "Название категории")]
        public string? Name { get; set; }

        public string? Description { get; set; }

        public ICollection<Exhibit>? Exhibit { get; set; }
    }
}




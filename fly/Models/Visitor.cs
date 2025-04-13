using System.ComponentModel.DataAnnotations;

namespace fly.Models
{
    public class Visitor
    {
        public int VisitorId { get; set; }


        [StringLength(50, MinimumLength = 2)]
        [Display(Name = "Имя посетителя")]
        public string VisitorFirstName { get; set; }

        [StringLength(50, MinimumLength = 2)]
        [Display(Name = "Фамилия посетителя")]
        public string VisitorLastName { get; set; }

        [Range(1, 120)]
        [Display(Name = "Возраст")]
        public int? Age { get; set; }
        
        [Display(Name = "Дата посещения")]
        public DateTime? VisitDate { get; set; }

        public virtual ICollection<Visit>? Visits { get; set; }
    }



}

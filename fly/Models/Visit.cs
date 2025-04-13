using System.ComponentModel.DataAnnotations;

namespace fly.Models
{
    public class Visit
    {
        public int VisitId { get; set; }

        
        public int? VisitorId { get; set; }
        [Display(Name = "Посетитель")]
        public virtual Visitor? Visitor { get; set; }

        
        public int? ExhibitionId { get; set; }
        [Display(Name = "Выставка")]
        public virtual Exhibition? Exhibition { get; set; }

        [Display(Name = "Дата посещения")]
        public DateTime? VisitDate { get; set; }
    }

}

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace fly.Models
{

    public class ExhibitInExhibition
    {
        public int ExhibitInExhibitionId { get; set; }
       
        public int ExhibitId { get; set; }
        [Display(Name = "Экспонат")]
        public virtual Exhibit? Exhibit { get; set; }

        
        public int ExhibitionId { get; set; }
        [Display(Name = "Выставка")]
        public virtual Exhibition? Exhibition { get; set; }
       
        [Display(Name = "Дата размещения")]
        public DateTime? PlacementDate { get; set; }
    }


}

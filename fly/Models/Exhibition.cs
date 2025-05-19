using System.ComponentModel.DataAnnotations;

namespace fly.Models
{
    public class Exhibition
    {
        public int ExhibitionId { get; set; }

        [StringLength(50, MinimumLength = 5)]
        [Display(Name = "Название выставки")]
        public string ExhibitionName { get; set; }

        [StringLength(250)]
        [Display(Name = "Описание выставки")]
        public string ExhibitionDescription { get; set; }

        [Display(Name = "Дата начала")]
        public DateTime? StartDate { get; set; }
        [Display(Name = "Дата окончания")]
        public DateTime? EndDate { get; set; }

        [StringLength(100)]
        [Display(Name = "Место проведения")]
        public string? Venue { get; set; }

        public virtual ICollection<ExhibitInExhibition>? ExhibitInExhibitions { get; set; }
        
    }



}

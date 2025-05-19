using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;


namespace fly.Models
{
    public class Restoration
    {
        public int RestorationId { get; set; }

        [Display(Name = "Экспонат")]
        public int ExhibitId { get; set; }

        [Display(Name = "Дата реставрации")]
        public DateTime RestorationDate { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Display(Name = "Рестовратор")]
        public string RestorerName { get; set; }

        [Display(Name = "Утверждено")]
        public bool IsApproved { get; set; }

        [Display(Name = "Дата начала")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Дата окончания")]
        public DateTime EndDate { get; set; }

        [BindNever]
        public Exhibit? Exhibit { get; set; }
    }
}
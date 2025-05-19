using System.ComponentModel.DataAnnotations;

namespace fly.Models
{
    public class Insurance
    {
        public int InsuranceId { get; set; }

        [Display(Name = "Экспонат")]
        public int ExhibitId { get; set; }

        [Display(Name = "Страховая компания")]
        public string InsuranceCompany { get; set; }

        [Display(Name = "Номер полиса")]
        public string PolicyNumber { get; set; }

        [Display(Name = "Дата начала")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Дата окончания")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Сумма покрытия")]
        public decimal CoverageAmount { get; set; }

        public Exhibit Exhibit { get; set; }
    }
}
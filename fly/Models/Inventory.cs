using System.ComponentModel.DataAnnotations;

namespace fly.Models
{
    public class Inventory
    {
        public int InventoryId { get; set; }

        [Display(Name = "Дата поступления")]
        public DateTime? поступления { get; set; }

        [Display(Name = "Дата удаления")]
        public DateTime? списания { get; set; }

        public int? ExhibitId { get; set; }

        [Display(Name = "Экспонат")]
        public Exhibit? Exhibit { get; set; }

        [Display(Name = "Наименование экспоната")]
        public string? ExhibitName { get; set; } 
    }
}
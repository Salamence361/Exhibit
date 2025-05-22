using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace fly.Models
{
    public class Movement
    {
        public int MovementId { get; set; }

        [Display(Name = "Экспонат")]
        public int ExhibitId { get; set; }

        [Display(Name = "Откуда")]
        public int FromStorageLocationId { get; set; }

        [Display(Name = "Куда")]
        public int ToStorageLocationId { get; set; }

        [Display(Name = "Дата перемещения")]
        public DateTime MovementDate { get; set; }

        [BindNever]
        public Exhibit? Exhibit { get; set; }
        [BindNever]
        public StorageLocation? FromStorageLocation { get; set; }
        [BindNever]
        public StorageLocation? ToStorageLocation { get; set; }
    }
}
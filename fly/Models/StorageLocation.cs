using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace fly.Models
{
    public class StorageLocation
    {
        public int StorageLocationId { get; set; }

        [Display(Name = "Название места хранения")]
        public string Name { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Display(Name = "Адрес")]
        public string? Address { get; set; }

        // Навигационные свойства
        public List<Movement> FromMovements { get; set; } // Перемещения, где это исходное место
        public List<Movement> ToMovements { get; set; }   // Перемещения, где это конечное место
    }
}
﻿using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace fly.Models
{
    public class Inventory
    {
        public int InventoryId { get; set; }

        [Required(ErrorMessage = "Количество обязательно.")]
        [Range(0, int.MaxValue, ErrorMessage = "Количество должно быть неотрицательным.")]
        [Display(Name = "Количество")]
        public int Quantity { get; set; }

        [Display(Name = "Дата поступления")]
        public DateTime? поступления { get; set; }

        [Display(Name = "Дата перемещения")]
        public DateTime? списания { get; set; }
        // Связи
        public int PartId { get; set; } 
        [Display(Name = "Запчасть")]
        public Exhibit? Exhibit { get; set; } 
    }
}

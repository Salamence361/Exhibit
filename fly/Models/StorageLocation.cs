﻿using System.ComponentModel.DataAnnotations;

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

        public List<Movement> FromMovements { get; set; }
        public List<Movement> ToMovements { get; set; }

        public List<Exhibit> Exhibits { get; set; }
    }
}
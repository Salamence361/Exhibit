﻿using System.ComponentModel.DataAnnotations;

namespace fly.Models
{
    public class Podrazdelenie
    {
        public int PodrazdelenieId { get; set; }
        [Display(Name = "Подразделение")]
        public string? PodrazdelenieName { get; set; }
        public virtual ICollection<CustomUser> CustomUser { get; set; }
    }
}

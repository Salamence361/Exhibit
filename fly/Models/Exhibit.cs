
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using OfficeOpenXml;


namespace fly.Models
{
    public class Exhibit
    {
        public int ExhibitId { get; set; }


       
        public int CategoryId { get; set; }
        
        public Category? Category { get; set; }

        [Required(ErrorMessage = "Укажите название экспоната")]
        [Display(Name = "Название экспоната")]
        [StringLength(50, MinimumLength = 5)]

        public string ExhibitName { get; set; }

        
        [Required(ErrorMessage = "Укажите описание экспоната")]
        [Display (Name = "Описание экспоната" )]
        [StringLength(250)]
        public string ExhibitDescription { get; set; }

        [Required(ErrorMessage = "Укажите дату создания")]
        [Display(Name = "Дата создания")]
        public DateTime? CreationDate { get; set; }

        [Required(ErrorMessage = "Укажите материал")]
        [Display(Name = "Материал")]
        [StringLength(50)]
        public string? Material { get; set; }

        [Required(ErrorMessage = "Укажите размер")]
        [Display(Name = "Размер")]
        [StringLength(50)]
        
        public string? Size { get; set; }

        [Display(Name = "Вес")]
        [Required(ErrorMessage = "Укажите вес")]
        public float? Weight { get; set; }

        [Display(Name = "Логотип")]
        public string? LogoPath { get; set; }

        public virtual ICollection<ExhibitInExhibition>? ExhibitInExhibitions { get; set; }


    }



}

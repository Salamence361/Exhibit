
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using OfficeOpenXml;


namespace fly.Models
{
    public class Museum
    {
        public int MuseumId { get; set; }


        [Required(ErrorMessage = "Укажите название музея")]
        [Display(Name = "Название музея")]
        [StringLength(50, MinimumLength = 5)]
        public string MuseumName { get; set; }


        [Required(ErrorMessage = "Укажите описание музея")]
        [Display(Name = "Адресс музея")]
        [StringLength(250)]
        public string MuseumAddress { get; set; }

        [Display(Name = "Логотип")]
        public string? LogoPath { get; set; }


        public virtual ICollection<Exhibit>? Exhibit { get; set; }
    }



}

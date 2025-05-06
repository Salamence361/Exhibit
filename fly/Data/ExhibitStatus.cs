using System.ComponentModel.DataAnnotations;

namespace fly.Data
{
    public enum ExhibitStatus
    {
        [Display(Name = "На выставке")]
        Заказано,

        [Display(Name = "В музее")]
        ЗаказВыполнен
    }
}
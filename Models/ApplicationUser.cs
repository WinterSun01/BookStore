using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [Display(Name = "Имя")]
        public string FirstName { get; set; } = string.Empty;

        [Display(Name = "Фамилия")]
        public string? LastName { get; set; }

        [Display(Name = "Отчество")]
        public string? MiddleName { get; set; }

        [Display(Name = "Дата рождения")]
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        [Display(Name = "Телефон")]
        [Phone]
        public string? Phone { get; set; }

        [Display(Name = "Полное имя")]
        public string FullName => $"{LastName} {FirstName} {MiddleName}".Trim();
    }
}

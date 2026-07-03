using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookStore.Models;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public RegisterModel(UserManager<ApplicationUser> userManager,
                             SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public class InputModel
        {
            [Required(ErrorMessage = "Имя обязательно")]
            [Display(Name = "Имя")]
            public string FirstName { get; set; } = string.Empty;

            [Display(Name = "Фамилия")]
            public string? LastName { get; set; }

            [Display(Name = "Отчество")]
            public string? MiddleName { get; set; }

            [Required(ErrorMessage = "Email обязателен")]
            [EmailAddress(ErrorMessage = "Некорректный формат email")]
            [Display(Name = "Электронная почта")]
            public string Email { get; set; } = string.Empty;

            [Display(Name = "Дата рождения")]
            [DataType(DataType.Date)]
            public DateTime? BirthDate { get; set; }

            [Phone(ErrorMessage = "Некорректный формат телефона")]
            [Display(Name = "Телефон")]
            public string? Phone { get; set; }

            [Required(ErrorMessage = "Пароль обязателен")]
            [StringLength(100, ErrorMessage = "Пароль должен содержать минимум {2} символов", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Пароль")]
            [RegularExpression(@"^(?=.*[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]).+$",
            ErrorMessage = "Пароль должен содержать хотя бы один специальный символ (например: !@#$%).")]
            public string Password { get; set; } = string.Empty;

            [Required(ErrorMessage = "Подтверждение пароля обязательно")]
            [DataType(DataType.Password)]
            [Display(Name = "Подтверждение пароля")]
            [Compare("Password", ErrorMessage = "Пароли не совпадают")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    FirstName = Input.FirstName,
                    LastName = Input.LastName,
                    MiddleName = Input.MiddleName,
                    BirthDate = Input.BirthDate,
                    PhoneNumber = Input.Phone
                };

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect("~/");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return Page();
        }
    }
}
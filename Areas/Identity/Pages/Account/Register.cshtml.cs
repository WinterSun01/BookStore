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

        public RegisterModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public class InputModel
        {
            [Required(ErrorMessage = "Email обязателен")]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Пароль обязателен")]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;

            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "Пароли не совпадают")]
            public string ConfirmPassword { get; set; } = string.Empty;

            [Required(ErrorMessage = "Имя обязательно")]
            [Display(Name = "Имя")]
            public string FirstName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Фамилия обязательна")]
            [Display(Name = "Фамилия")]
            public string LastName { get; set; } = string.Empty;

            [Display(Name = "Отчество")]
            public string? MiddleName { get; set; }

            [Display(Name = "Дата рождения")]
            [DataType(DataType.Date)]
            public DateTime? BirthDate { get; set; }

            [Phone]
            [Display(Name = "Телефон")]
            public string? Phone { get; set; }
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
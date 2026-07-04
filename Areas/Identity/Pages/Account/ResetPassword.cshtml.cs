using BookStore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BookStore.Areas.Identity.Pages.Account
{
    public class ResetPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ResetPasswordModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            [Required]
            public string Email { get; set; } = string.Empty;

            [Required]
            public string Code { get; set; } = string.Empty;

            [Required(ErrorMessage = "Введите пароль")]
            [StringLength(100, MinimumLength = 6, ErrorMessage = "Пароль должен содержать минимум 6 символов.")]
            [DataType(DataType.Password)]
            [RegularExpression(@"^(?=.*[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]).+$",
                ErrorMessage = "Пароль должен содержать хотя бы один специальный символ (например: !@#$%).")]
            public string Password { get; set; } = string.Empty;

            [Required(ErrorMessage = "Подтвердите пароль")]
            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "Пароли не совпадают")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        public IActionResult OnGet(string code = null, string email = null)
        {
            if (code == null || email == null)
                return BadRequest("Ошибка ссылки");

            Input = new InputModel
            {
                Email = email,
                Code = code
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var user = await _userManager.FindByEmailAsync(Input.Email);

            if (user == null)
                return RedirectToPage("./ResetPasswordConfirmation");

            var passwordHasher = new PasswordHasher<ApplicationUser>();
            var verificationResult = passwordHasher.VerifyHashedPassword(
                user, user.PasswordHash!, Input.Password);

            if (verificationResult == PasswordVerificationResult.Success)
            {
                ModelState.AddModelError(string.Empty,
                    "Новый пароль не должен совпадать со старым");
                return Page();
            }

            var decodedCode = Encoding.UTF8.GetString(
                WebEncoders.Base64UrlDecode(Input.Code));

            var result = await _userManager.ResetPasswordAsync(
                user,
                decodedCode,
                Input.Password);

            if (result.Succeeded)
                return RedirectToPage("./ResetPasswordConfirmation");

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return Page();
        }
    }
}
using BookStore.Models;
using BookStore.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Text;

namespace BookStore.Areas.Identity.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly EmailService _emailService;

        public ForgotPasswordModel(
            UserManager<ApplicationUser> userManager,
            EmailService emailService)
        {
            _userManager = userManager;
            _emailService = emailService;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var user = await _userManager.FindByEmailAsync(Input.Email);

            if (user == null)
            {
                return RedirectToPage("ForgotPasswordConfirmation");
            }

            var token =
                await _userManager.GeneratePasswordResetTokenAsync(user);

            token = WebEncoders.Base64UrlEncode(
                System.Text.Encoding.UTF8.GetBytes(token));

            var callbackUrl = Url.Page(
                "/Account/ResetPassword",
                null,
                new
                {
                    area = "Identity",
                    code = token,
                    email = user.Email
                },
                Request.Scheme);

            try
            {
                await _emailService.SendEmailAsync(
                    user.Email!,
                    "Восстановление пароля",
                    $"""
            <h2>BookStore</h2>
            <p>Для восстановления пароля нажмите кнопку ниже.</p>
            <p>
                <a href="{HtmlEncoder.Default.Encode(callbackUrl!)}">
                    Восстановить пароль
                </a>
            </p>
            """);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка отправки письма: {ex.Message}");
            }

            return RedirectToPage("ForgotPasswordConfirmation");
        }
    }
}
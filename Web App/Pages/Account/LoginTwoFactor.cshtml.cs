using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Web_App.Data.Account;
using Web_App.Services;

namespace Web_App.Pages.Account
{
    public class LoginTwoFactorModel : PageModel
    {
        private readonly UserManager<User> userManager;
        private readonly IEmailService _emailService;
        private readonly SignInManager<User> signInManager;

        [BindProperty]
        public EmailMFA EmailMFA { get; set; } = new();

        public LoginTwoFactorModel(UserManager<User> userManager, IEmailService emailService, SignInManager<User> signInManager)
        {
            this.userManager = userManager;
            this._emailService = emailService;
            this.signInManager = signInManager;
        }

        public async Task OnGetAsync(string email, bool rememberMe)
        {
            var user = await userManager.FindByEmailAsync(email);

            this.EmailMFA.SecurityCode = string.Empty;
            this.EmailMFA.RememberMe = rememberMe;

            var securityCode = await userManager.GenerateTwoFactorTokenAsync(user, "Email");

            await _emailService.SendConfirmationAsync(email, "Token", $"Use this as the token: {securityCode}");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var result = await signInManager.TwoFactorSignInAsync("Email",
                this.EmailMFA.SecurityCode,
                this.EmailMFA.RememberMe,
                false);

            if (result.Succeeded)
            {
                return RedirectToPage("/Index");
            }
            else
            {
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("Login2FA", "You are locked out.");
                }
                else
                {
                    ModelState.AddModelError("Login2FA", "Failed to login.");
                }

                return Page();
            }
        }
    }

    public class EmailMFA
    {
        [Required]
        [Display(Name = "Security Code")]
        public string SecurityCode { get; set; }
        public bool RememberMe { get; set; }
    }
}

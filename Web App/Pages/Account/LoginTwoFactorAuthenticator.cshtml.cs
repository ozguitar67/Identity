using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Web_App.Data.Account;
using Web_App.Services;

namespace Web_App.Pages.Account
{
    public class LoginTwoFactorAuthenticatorModel : PageModel
    {
        private readonly UserManager<User> userManager;        
        private readonly SignInManager<User> signInManager;

        [BindProperty]
        public AuthenticatorMFA AuthenticatorMFA { get; set; } = new();

        public LoginTwoFactorAuthenticatorModel(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        public async Task OnGetAsync(bool rememberMe)
        {
            this.AuthenticatorMFA.SecurityCode = string.Empty;
            this.AuthenticatorMFA.RememberMe = rememberMe;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var result = await signInManager.TwoFactorAuthenticatorSignInAsync(this.AuthenticatorMFA.SecurityCode,
                this.AuthenticatorMFA.RememberMe,
                false);

            if (result.Succeeded)
            {
                return RedirectToPage("/Index");
            }
            else
            {
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("Authenticator2FA", "You are locked out.");
                }
                else
                {
                    ModelState.AddModelError("Authenticator2FA", "Failed to login.");
                }

                return Page();
            }
        }
    }

    public class AuthenticatorMFA
    {
        [Required]
        [Display(Name = "Security Code")]
        public string SecurityCode { get; set; }
        public bool RememberMe { get; set; }
    }
}

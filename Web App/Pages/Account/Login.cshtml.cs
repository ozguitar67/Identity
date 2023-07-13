using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Web_App.Data.Account;

namespace Web_App.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<User> signInManager;
        private readonly UserManager<User> userManager;

        public LoginModel(SignInManager<User> signInManager, UserManager<User> userManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        [BindProperty]
        public CredentialViewModel CredentialViewModel { get; set; }

        [BindProperty]
        public IEnumerable<AuthenticationScheme> ExternalLoginProviders { get; set; }
        public async Task OnGetAsync()
        {
            this.ExternalLoginProviders = await signInManager.GetExternalAuthenticationSchemesAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var result = await signInManager.PasswordSignInAsync(this.CredentialViewModel.Email, this.CredentialViewModel.Password, this.CredentialViewModel.RememberMe, false);

            if (result.Succeeded)
            {
                return RedirectToPage("/Index");
            }
            else
            {
                if (result.RequiresTwoFactor)
                {
                    var user = await userManager.FindByEmailAsync(this.CredentialViewModel.Email);
                    if (string.IsNullOrWhiteSpace(await userManager.GetAuthenticatorKeyAsync(user))) 
                    {
                        return RedirectToPage("/Account/LoginTwoFactor", new
                        {
                            Email = this.CredentialViewModel.Email,
                            RememberMe = this.CredentialViewModel.RememberMe
                        });
                    }
                    else
                    {
                        return RedirectToPage("/Account/LoginTwoFactorAuthenticator", new
                        {
                            RememberMe = this.CredentialViewModel.RememberMe
                        });
                    }
                }
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("Login", "You are locked out");
                }
                else
                {
                    ModelState.AddModelError("Login", "Failed to login");
                }
                return Page();
            }
        }

        public IActionResult OnPostLoginExternally(string provider)
        {
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, null);
            properties.RedirectUri = Url.Action("ExternalLoginCallback", "Account");

            return Challenge(properties, provider);
        }

    }

    public class CredentialViewModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        [DataType(dataType: DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }

    }
}

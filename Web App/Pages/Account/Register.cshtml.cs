using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;
using Web_App.Data;
using Web_App.Services;

namespace Web_App.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IEmailService _emailService;

        public RegisterModel(UserManager<IdentityUser> userManager, IEmailService emailService)
        {
            this.userManager = userManager;
            _emailService = emailService;
        }

        [BindProperty]
        public RegisterViewModel RegisterViewModel { get; set; }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Validating Email address (optional) due to options in Program.cs

            //Create user
            var user = new IdentityUser
            {
                Email = RegisterViewModel.Email,
                UserName = RegisterViewModel.Email
            };

            var result = await this.userManager.CreateAsync(user, RegisterViewModel.Password);
            if (result.Succeeded)
            {
                var confirmationToken = await this.userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = Url.PageLink(pageName: "/Account/ConfirmEmail", values: new { userId = user.Id, token = confirmationToken });

                bool emailSuccess = await _emailService.SendConfirmationAsync(user.Email, "Please Confirm Your Email", $"Please click this link to confirm your email address: {confirmationLink}");

                if (emailSuccess)
                {
                    return RedirectToPage("/Account/Login");
                }
                else
                {
                    ModelState.AddModelError("Register", "Registration Error");
                    return Page();
                }
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("Register", error.Description);
                }

                return Page();
            }
        }

    }

    public class RegisterViewModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = string.Empty;
        [Required]
        [DataType(dataType: DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}

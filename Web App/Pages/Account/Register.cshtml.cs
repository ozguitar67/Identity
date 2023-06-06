using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;
using Web_App.Data;

namespace Web_App.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly ApplicationDbContext dbContext;

        public RegisterModel(UserManager<IdentityUser> userManager, ApplicationDbContext dbContext)
        {
            this.userManager = userManager;
            this.dbContext = dbContext;
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

                var message = new MailMessage("ozguitar67@comcast.net", user.Email,
                    "Please Confirm Your Email",
                    $"Please click on this link to confirm your email address: {confirmationLink}");

                Credentials? smtp = await dbContext.Credentials.AsNoTracking().Where(c => c.CredentialId == 1).FirstOrDefaultAsync();
                if (smtp is not null)
                {
                    using var emailClient = new SmtpClient("smtp.comcast.net")
                    {
                        Port = 587,
                        Credentials = new NetworkCredential(smtp.Username, smtp.Password),
                        EnableSsl = true
                    };                    

                    await emailClient.SendMailAsync(message);

                    return RedirectToPage("/Account/Login");
                }
                ModelState.AddModelError("Register", "Registration Error");
                return Page();
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
        public string Email { get; set; }
        [Required]
        [DataType(dataType: DataType.Password)]
        public string Password { get; set; }
    }
}

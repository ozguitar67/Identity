using Identity.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public Credential Credential { get; set; }
        public void OnGet()
        {
            this.Credential = new Credential
            {
                UserName = "admin"
            };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            else
            {
                //Verify credential
                if (Credential.UserName == "admin" && Credential.Password == "password")
                {
                    //Create security context
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, Credential.UserName),
                        new Claim(ClaimTypes.Email, "admin@mywebsite.com"),
                        new Claim("Department", "HR"),
                        new Claim("Admin", "true"),
                        new Claim("Manager", "true"),
                        new Claim("EmploymentDate", "2021-05-01")
                    };
                    var identity = new ClaimsIdentity(claims, "MyCookieAuth");
                    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = Credential.RememberMe
                    };

                    await HttpContext.SignInAsync("MyCookieAuth", claimsPrincipal, authProperties);

                    return RedirectToPage("/Index");
                }
                else
                {
                    return Page();
                }
            }
        }
    }
}

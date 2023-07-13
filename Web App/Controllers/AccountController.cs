using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Web_App.Data.Account;

namespace Web_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<User> signInManager;

        public AccountController(SignInManager<User> signInManager)
        {
            this.signInManager = signInManager;
        }

        public async Task<IActionResult> ExternalLoginCallback()
        {
            var loginInfo = await signInManager.GetExternalLoginInfoAsync();
            var emailClaim = loginInfo.Principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
            var userClaim = loginInfo.Principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name);

            if (emailClaim != null && userClaim != null)
            {
                var user = new User { UserName = emailClaim.Value, Email = userClaim.Value };
                await signInManager.SignInAsync(user, isPersistent: false);
            }

            return RedirectToPage("/Index");
        }
    }
}

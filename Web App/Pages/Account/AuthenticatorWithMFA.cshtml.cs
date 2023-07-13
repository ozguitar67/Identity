using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRCoder;
using Web_App.Data.Account;

namespace Web_App.Pages.Account
{
    [Authorize]
    public class AuthenticatorWithMFAModel : PageModel
    {
        private readonly UserManager<User> userManager;

        [BindProperty]
        public SetupMFAViewModel MFAViewModel { get; set; } = new();
        public bool SetupSuccess { get; set; }

        public AuthenticatorWithMFAModel(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }
        public async Task OnGetAsync()
        {
            var user = await userManager.GetUserAsync(base.User);
            await userManager.ResetAuthenticatorKeyAsync(user);
            this.MFAViewModel.Key = await userManager.GetAuthenticatorKeyAsync(user);
            this.MFAViewModel.QrCodeBytes = GenerateQrCodeBytes("Identity", this.MFAViewModel.Key, user.Email);
        }

        public async Task OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(base.User);
                if (await userManager.VerifyTwoFactorTokenAsync(user, userManager.Options.Tokens.AuthenticatorTokenProvider, this.MFAViewModel.SecurityCode))
                {
                    await userManager.SetTwoFactorEnabledAsync(user, true);
                    this.SetupSuccess = true;
                }
                else
                {
                    ModelState.AddModelError("AuthenticatorSetup", "Error with Authenticator set up");
                }
            }
        }

        private Byte[] GenerateQrCodeBytes(string Provider, string Key, string UserEmail)
        {
            var qrCodeGenerator = new QRCodeGenerator();
            //qrCodeGenerator.CreateQrCode($"otpauth://totp/{provider}:{UserEmail}?secret={Key}&issuer={Provider}&digits=6")
            var qrData = qrCodeGenerator.CreateQrCode($"otpauth://totp/{Provider}:{UserEmail}?secret={Key}&issuer={Provider}", QRCodeGenerator.ECCLevel.Q);
            var qrPng = new PngByteQRCode(qrData);
            return qrPng.GetGraphic(20);
        }
    }

    public class SetupMFAViewModel
    {
        public string Key { get; set; }
        public string SecurityCode { get; set; }
        public Byte[] QrCodeBytes { get; set; }
    }
}

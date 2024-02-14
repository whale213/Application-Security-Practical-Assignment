using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MimeKit;
using Newtonsoft.Json;
using Google.Authenticator;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using AppSecPracticalAssignment_223981B.Models;
using AppSecPracticalAssignment_223981B.Service;
using AppSecPracticalAssignment_223981B.ViewModels;



namespace AppSecPracticalAssignment_223981B.Views
{
    public class LoginModel : PageModel
    {

        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;

        private readonly AuditLogService auditLogService;

        public LoginModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, AuditLogService auditLogService)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.auditLogService = auditLogService;
        }

        [BindProperty]
        public Login LModel { get; set; }

        //public string QrCodeUrl { get; set; }

        //public string ManualEntryCode { get; set; }

        public async Task<IActionResult> OnPostAsync([FromServices] AuthDbContext db)
        {
            var recaptchaResponse = HttpContext.Request.Form["g-recaptcha-response"];

            var recaptchaClient = new HttpClient();
            var recaptchaResult = await recaptchaClient.GetStringAsync($"https://www.google.com/recaptcha/api/siteverify?secret=6Ld4ZWQpAAAAABabbcPEkiVUoko3SX8Od9OtQRB_&response={recaptchaResponse}");
            var recaptchaData = JsonConvert.DeserializeObject<Recaptcha>(recaptchaResult);

            if (!recaptchaData.Success)
            {
                ModelState.AddModelError(string.Empty, "reCAPTCHA verification failed. Please try again.");
                return Page();
            } 

            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(LModel.Email, LModel.Password, LModel.RememberMe, lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    HttpContext.Session.SetString("Username", LModel.Email);
                    SessionManager.AddSession(LModel.Email, HttpContext.Session.Id);

                    auditLogService.Log(LModel.Email, "Logging In", "User has logged in");


                    string key = GenerateRandomString(10);

                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress("App Sec Assignment", "appsecassignment@gmail.com"));
                    message.To.Add(new MailboxAddress("User", LModel.Email));
                    message.Subject = "Two-Factor Authentication Code";

                    message.Body = new TextPart("plain")
                    {
                        Text = $"Your two-factor authentication code is: {key}"
                    };

                    using (var client = new SmtpClient())
                    {
                        client.Connect("smtp.gmail.com", 587, false);
                        client.Authenticate("appsecassignment@gmail.com", "fnfn jhxi gsys resx");
                        client.Send(message);
                        client.Disconnect(true);
                    }

                    db.TwoFactorAuthentication.Add(new TwoFA
                    {
                        Id = Guid.NewGuid(),
                        Email = LModel.Email,
                        Key = key
                    });
                    db.SaveChanges();


                    // FOR 2FA



                    //TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();

                    //SetupCode setupInfo = tfa.GenerateSetupCode("Test Two Factor", LModel.Email, key, false, 3);
                    //ManualEntryCode = setupInfo.ManualEntryKey;


                    return RedirectToPage("TwoFactorAuthentication");
                }
                else if (result.IsLockedOut)
                {
                    ModelState.AddModelError("LModel.Email", "Account is locked out. Please try again later.");
                    auditLogService.Log(LModel.Email, "Logging In", "User has been locked out");

                }
                else if (!result.Succeeded)
                {
                    var user = await userManager.FindByEmailAsync(LModel.Email);
                    if (user != null)
                    {
                        user.AccessFailedCount++;

                        if (user.AccessFailedCount >= userManager.Options.Lockout.MaxFailedAccessAttempts)
                        {
                            user.LockoutEnd = DateTimeOffset.UtcNow.Add(userManager.Options.Lockout.DefaultLockoutTimeSpan);
                        }

                        await userManager.UpdateAsync(user);
                    }

                    ModelState.AddModelError("LModel.Email", "Email or password is incorrect.");
                }
            }

            return Page();
        }

        public static string GenerateRandomString(int length, string allowableChars = null)
        {
            if (string.IsNullOrEmpty(allowableChars))

                allowableChars = @"ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var rnd = new byte[length];

            using (var rng = new RNGCryptoServiceProvider())
                rng.GetBytes(rnd);

            var allowable = allowableChars.ToCharArray();
            var l = allowable.Length;
            var chars = new char[length];

            for (var i = 0; i < length; i++)
                chars[i] = allowable[rnd[i] % l];

            return new string(chars);
        }

    }
}

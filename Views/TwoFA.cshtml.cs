using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AppSecPracticalAssignment_223981B.Models;
using AppSecPracticalAssignment_223981B.Service;
using AppSecPracticalAssignment_223981B.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;


namespace AppSecPracticalAssignment_223981B.Views
{
    public class TwoFactorAuthenticationModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;


        public TwoFactorAuthenticationModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        [BindProperty]
        public TwoFA TwoFAModel { get; set; }


        public async Task<IActionResult> OnPostAsync([FromServices] AuthDbContext db)
        {
            var loggedUser = await userManager.GetUserAsync(User);

            if (loggedUser != null)
            {
                var user = await db.TwoFactorAuthentication.FirstOrDefaultAsync(u => u.Email == loggedUser.Email);

                if (user != null)
                {
                    string userTwoFAKey = user.Key;

                    string twoFACode = Request.Form["TwoFACode"];

                    if (twoFACode == userTwoFAKey)
                    {

                        db.TwoFactorAuthentication.Remove(user);
                        await db.SaveChangesAsync();
                        return RedirectToPage("Index");
                    }
                    else
                    {
                        ViewData["TwoFACodeError"] = "Invalid Two-Factor Authentication code.";
                    }
                }
                else
                {
                    ViewData["TwoFACodeError"] = "Two-Factor Authentication is not enabled for this user.";
                }
            }
            else
            {
                return RedirectToPage("Login");
            }

            return Page();
        }
    }
}

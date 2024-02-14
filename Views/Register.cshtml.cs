using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Cryptography;
using AppSecPracticalAssignment_223981B.Models;
using AppSecPracticalAssignment_223981B.Service;
using AppSecPracticalAssignment_223981B.ViewModels;


namespace AppSecPracticalAssignment_223981B.Views
{
    public class RegisterModel : PageModel
    {

        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly AuditLogService auditLogService;

        public RegisterModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, AuditLogService auditLogService)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.auditLogService = auditLogService;
        }

        [BindProperty]
        public Register RModel { get; set; }



        public void OnGet()
        {
        }



        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var existingUser = await userManager.FindByEmailAsync(RModel.Email);

                if (existingUser != null)
                {
                    ModelState.AddModelError("RModel.Email", "Email address is already registered");
                    return Page();
                }
                var user = new ApplicationUser()
                {
                    FullName = RModel.FullName,
                    CreditCardNumber = Encrypt(RModel.CreditCardNumber),
                    Gender = RModel.Gender,
                    MobileNumber = RModel.MobileNumber,
                    DeliveryAddress = RModel.DeliveryAddress,
                    Password = RModel.Password,
                    ConfirmPassword = RModel.ConfirmPassword,
                    AboutMe = RModel.AboutMe,
                    UserName = RModel.Email,
                    Email = RModel.Email,
                    Key = EncryptionSettings.Instance.Key,
                    IV = EncryptionSettings.Instance.IV
                };

                if (RModel.Photo != null && RModel.Photo.Length > 0)
                {
                    string extension = Path.GetExtension(RModel.Photo.FileName);

                    using (var memoryStream = new MemoryStream())
                    {
                        await RModel.Photo.CopyToAsync(memoryStream);
                        user.Photo = memoryStream.ToArray();
                    }
                }

                var result = await userManager.CreateAsync(user, RModel.Password);

                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, false);

                    HttpContext.Session.SetString("Username", RModel.Email);

                    auditLogService.Log(RModel.Email, "Register", "User has successfully registered.");


                    return RedirectToPage("Index");
                }
            }

            return Page();
        }


        private static string Encrypt(string input)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = EncryptionSettings.Instance.Key;
                aesAlg.IV = EncryptionSettings.Instance.IV;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using MemoryStream msEncrypt = new MemoryStream();
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(input);
                    }
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }
    }
}

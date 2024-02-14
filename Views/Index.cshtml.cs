using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AppSecPracticalAssignment_223981B.Models;
using AppSecPracticalAssignment_223981B.Service;
using AppSecPracticalAssignment_223981B.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography;


namespace AppSecPracticalAssignment_223981B.Views
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> userManager;

        private readonly AuditLogService auditLogService;



        public IndexModel(UserManager<ApplicationUser> userManager, AuditLogService auditLogService)
        {

            this.userManager = userManager;
            this.auditLogService = auditLogService;

        }

        public ApplicationUser UserInfo { get; set; }

        public void OnGet()
        {
            var user = userManager.GetUserAsync(User).Result;



            UserInfo = user;

            var fullName = user.FullName;


            var encryptedCreditCard = user.CreditCardNumber;
            var key = user.Key;
            var IV = user.IV;
            var decryptedCreditCard = Decrypt(encryptedCreditCard, key, IV);

            ViewData["FullName"] = fullName;
            ViewData["DecryptedCreditCard"] = decryptedCreditCard;

            auditLogService.Log(user.Email, "View Details", "User is viewing details successfully.");

        }


        private string Decrypt(string encryptedText, byte[] key, byte[] IV)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = IV;

                // Create a decryptor to perform the stream transform
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption
                using MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(encryptedText));
                using CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
                using StreamReader srDecrypt = new StreamReader(csDecrypt);

                // Read the decrypted bytes from the decrypting stream and return as string
                return srDecrypt.ReadToEnd();
            }
        }
    }
}

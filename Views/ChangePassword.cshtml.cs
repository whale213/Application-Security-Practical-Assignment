using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AppSecPracticalAssignment_223981B.Models;
using AppSecPracticalAssignment_223981B.Service;
using AppSecPracticalAssignment_223981B.ViewModels;


namespace AppSecPracticalAssignment_223981B.Views
{
    public class ChangePasswordModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;

        private readonly AuditLogService auditLogService;

        public ChangePasswordModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, AuditLogService auditLogService)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.auditLogService = auditLogService;
        }


        [BindProperty]
        public ChangePassword CPModel { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            var changePasswordResult = await userManager.ChangePasswordAsync(user, CPModel.ExistingPassword, CPModel.UpdatedPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(CPModel.ExistingPassword, "Wrong password. Try Again,");
                }
                return Page();
            }

            await signInManager.RefreshSignInAsync(user);

            auditLogService.Log(userManager.GetUserId(User), "Changing password", "User has changed password successfully");

            return RedirectToPage("/Login");
        }
    }
}

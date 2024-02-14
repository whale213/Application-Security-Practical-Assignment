using System.ComponentModel.DataAnnotations;

namespace AppSecPracticalAssignment_223981B.ViewModels
{
    public class ChangePassword
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Existing password")]
        public string ExistingPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Updated password")]
        public string UpdatedPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm updated password")]
        [Compare("UpdatedPassword", ErrorMessage = "The passwords do not match.")]
        public string ConfirmUpdatedPassword { get; set; }
    }
}

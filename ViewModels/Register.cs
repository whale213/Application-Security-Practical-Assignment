using System.ComponentModel.DataAnnotations;

namespace AppSecPracticalAssignment_223981B.ViewModels
{
    public class Register
    {
        [Required(ErrorMessage = "Please enter your full name.")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Please enter a valid credit card number.")]
        [RegularExpression(@"^\d{16}$", ErrorMessage = "Credit card number must be 16 digits.")]
        [Display(Name = "Credit Card No")]
        [DataType(DataType.CreditCard)]
        public string CreditCardNumber { get; set; }

        [Required(ErrorMessage = "Please select your gender.")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Please enter your mobile number.")]
        [RegularExpression(@"^[0-9]{8}$", ErrorMessage = "Mobile number must be 8 digits.")]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Mobile No")]
        public string MobileNumber { get; set; }

        [Required(ErrorMessage = "Please enter your delivery address.")]
        [Display(Name = "Delivery Address")]
        public string DeliveryAddress { get; set; }

        [Required(ErrorMessage = "Please enter a valid email address.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter a password.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please confirm your password.")]
        [Compare(nameof(Password), ErrorMessage = "The passwords do not match.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

        public IFormFile Photo { get; set; }

        [Display(Name = "About Me")]
        public string AboutMe { get; set; }
    }
}

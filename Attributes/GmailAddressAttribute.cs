using System.ComponentModel.DataAnnotations;

namespace _2022_CS_668.Attributes
{
    public class GmailAddressAttribute : ValidationAttribute
    {
        public GmailAddressAttribute()
        {
            ErrorMessage = "Email must be a valid Gmail address ending with @gmail.com";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult("Email is required");
            }

            string email = value.ToString()!.Trim().ToLower();

            // Check if it's a valid email format first
            if (!email.Contains("@"))
            {
                return new ValidationResult("Email must be in a valid format");
            }

            // Check if it ends with @gmail.com
            if (!email.EndsWith("@gmail.com"))
            {
                return new ValidationResult(ErrorMessage);
            }

            // Validate the part before @gmail.com
            var localPart = email.Substring(0, email.IndexOf("@"));
            if (string.IsNullOrWhiteSpace(localPart))
            {
                return new ValidationResult("Email must have characters before @gmail.com");
            }

            // Check for valid characters (alphanumeric, dots, underscores)
            if (!System.Text.RegularExpressions.Regex.IsMatch(localPart, @"^[a-zA-Z0-9._]+$"))
            {
                return new ValidationResult("Email contains invalid characters");
            }

            return ValidationResult.Success;
        }
    }
}

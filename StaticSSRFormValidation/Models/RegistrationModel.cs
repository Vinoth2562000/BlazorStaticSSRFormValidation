using System.ComponentModel.DataAnnotations;

namespace StaticSSRFormValidation.Models
{
    /// <summary>
    /// Model for registration form validation testing.
    /// Tests both client-side (browser) and server-side validation in static SSR.
    /// </summary>
    public class RegistrationModel
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
        [ReservedName(ErrorMessage = "'{0}' is a reserved name and cannot be used")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Email must be a valid email address")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Age is required")]
        [Range(18, 120, ErrorMessage = "Age must be between 18 and 120")]
        public int? Age { get; set; }
    }

    /// <summary>
    /// Custom validation attribute that only validates on the server side.
    /// No client-side rule provider - tests server-only validation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ReservedNameAttribute : ValidationAttribute
    {
        // List of reserved names that cannot be used
        private static readonly HashSet<string> ReservedNames = new(StringComparer.OrdinalIgnoreCase)
        {
            "admin",
            "administrator",
            "system",
            "root",
            "test"
        };

        public override bool IsValid(object? value)
        {
            if (value is not string name || string.IsNullOrWhiteSpace(name))
            {
                return true; // Let Required attribute handle this
            }

            return !ReservedNames.Contains(name);
        }
    }
}

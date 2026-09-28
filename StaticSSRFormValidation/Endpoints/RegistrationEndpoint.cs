using System.ComponentModel.DataAnnotations;
using StaticSSRFormValidation.Models;

namespace StaticSSRFormValidation.Endpoints
{
    /// <summary>
    /// Endpoints for registration form submission.
    /// Tests server-side validation including reserved name checking.
    /// </summary>
    public static class RegistrationEndpoint
    {
        public static void MapRegistrationEndpoints(this WebApplication app)
        {
            app.MapPost("/api/register", HandleRegistration)
                .WithName("RegisterUser")
                .Produces<RegistrationResponse>(200)
                .Produces(400);
        }

        private static IResult HandleRegistration(RegistrationModel model)
        {
            // Validate model
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            if (!Validator.TryValidateObject(model, context, results, true))
            {
                // Build error response from validation results
                var errors = new Dictionary<string, string[]>();
                foreach (var result in results)
                {
                    var key = result.MemberNames.FirstOrDefault() ?? "general";
                    if (!errors.ContainsKey(key))
                    {
                        errors[key] = Array.Empty<string>();
                    }
                    errors[key] = errors[key].Append(result.ErrorMessage ?? "Validation failed").ToArray();
                }

                return Results.BadRequest(new { errors });
            }

            // If all validations passed, return success
            var response = new RegistrationResponse
            {
                Name = model.Name,
                Timestamp = DateTime.UtcNow
            };

            return Results.Ok(response);
        }

        public class RegistrationResponse
        {
            public string? Name { get; set; }
            public DateTime Timestamp { get; set; }
        }
    }
}

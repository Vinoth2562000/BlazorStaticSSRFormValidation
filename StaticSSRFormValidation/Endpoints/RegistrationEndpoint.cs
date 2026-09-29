using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
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

        private static IResult HandleRegistration(HttpContext context, IFormCollection form)
        {
            // Extract form data - Blazor posts with "Model." prefix when using FormName
            var model = new RegistrationModel
            {
                Name = form["Model.Name"].ToString(),
                Email = form["Model.Email"].ToString(),
                Age = int.TryParse(form["Model.Age"].ToString(), out var age) ? age : 0
            };

            // Validate model
            var validationContext = new ValidationContext(model);
            var results = new List<ValidationResult>();

            if (!Validator.TryValidateObject(model, validationContext, results, true))
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

            // If all validations passed, redirect back to form with success message
            return Results.Redirect($"/registration?success=true&name={Uri.EscapeDataString(model.Name ?? "")}");

        }

        public class RegistrationResponse
        {
            public string? Name { get; set; }
            public DateTime Timestamp { get; set; }
        }
    }
}

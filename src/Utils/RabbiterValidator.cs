using System.ComponentModel.DataAnnotations;

namespace RabbitR.Utils;

/// <summary>
/// Provides helper methods for object validation.
/// </summary>
internal static class RabbiterValidator
{
    /// <summary>
    /// Performs validation using the specified annotations of object, also <see cref="IValidatableObject"/>.
    /// </summary>
    /// <exception cref="ValidationException">The exception that is thrown if there are validation errors.</exception>
    internal static void ThrowIfNotValid(object obj)
    {
        var context = new ValidationContext(obj, null, null);
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(obj, context, validationResults, true);
        if (!isValid)
        {
            throw new ValidationException(string.Join("\n", validationResults.Select(r => r.ErrorMessage)));
        }
    }
}
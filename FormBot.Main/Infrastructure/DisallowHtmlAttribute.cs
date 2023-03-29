using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace FormBot.Main.Infrastructure
{
    public class DisallowHtmlAttribute : ValidationAttribute
    {
        /// <summary>
        /// Checks whether any control value contains HTML tags
        /// </summary>
        /// <param name="value">Property value to be validated</param>
        /// <param name="validationContext">The context information about the validation operation.</param>
        /// <returns>
        /// An instance of the <see cref="T:System.ComponentModel.DataAnnotations.ValidationResult" /> class.
        /// </returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            var tagWithoutClosingRegex = new Regex(@"[<\[^>\]>]");
            var hasTags = false;
            if (!value.ToString().Contains("Generic.List"))
            {
                hasTags = tagWithoutClosingRegex.IsMatch(value.ToString());
            }
            if (!hasTags)
                return ValidationResult.Success;

            return new ValidationResult("The field cannot contain symbols like < > ^ [ ].");
        }
    }
}
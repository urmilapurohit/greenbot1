using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Entity
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// class RequiredIfAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class RequiredIfAttribute : ValidationAttribute
    {
        /// <summary>
        /// The default error message format string
        /// </summary>
        private const string DefaultErrorMessageFormatString = "The {0} field is required.";

        /// <summary>
        /// The _dependent property name
        /// </summary>
        private readonly string prpdependentPropertyName;

        /// <summary>
        /// The _dependent property comparison
        /// </summary>
        private readonly Operator prpdependentPropertyComparison;

        /// <summary>
        /// The _dependent property value
        /// </summary>
        private readonly object prpdependentPropertyValue;

        /// <summary>
        /// The _dependent property value
        /// </summary>
        private readonly object[] targetValue;

        /// <summary>
        /// The default error message format string
        /// </summary>
        private RequiredAttribute innerAttribute = new RequiredAttribute();

        /// <summary>
        /// Initializes a new instance of the <see cref="RequiredIfAttribute"/> class.
        /// </summary>
        /// <param name="dependentPropertyName">Name of the dependent property.</param>
        /// <param name="dependentPropertyComparison">The dependent property comparison.</param>
        /// <param name="dependentPropertyValue">The dependent property value.</param>
        /// <param name="targetValue">The dependent target value.</param>
        public RequiredIfAttribute(string dependentPropertyName, Operator dependentPropertyComparison, object dependentPropertyValue, params object[] targetValue)
        {
            this.prpdependentPropertyName = dependentPropertyName;
            this.prpdependentPropertyComparison = dependentPropertyComparison;
            this.prpdependentPropertyValue = dependentPropertyValue;
            this.targetValue = targetValue;
            this.ErrorMessage = DefaultErrorMessageFormatString;
        }

        /// <summary>
        /// ENUM Operator
        /// </summary>
        public enum Operator
        {
            /// <summary>
            /// The equal to
            /// </summary>
            EqualTo,

            /// <summary>
            /// The not equal to
            /// </summary>
            NotEqualTo,

            /// <summary>
            /// The Contains
            /// </summary>
            Contains
        }

        /// <summary>
        /// Validates the specified value with respect to the current validation attribute.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="validationContext">The context information about the validation operation.</param>
        /// <returns>
        /// An instance of the <see cref="T:System.ComponentModel.DataAnnotations.ValidationResult" /> class.
        /// </returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                var dependentProperty = validationContext.ObjectInstance.GetType().GetProperty(this.prpdependentPropertyName);

                var dependentPropertyValue = dependentProperty.GetValue(validationContext.ObjectInstance, null);
                if (this.targetValue != null && this.targetValue.Length > 0)
                {
                    // get a reference to the property this validation depends upon
                    var containerType = validationContext.ObjectInstance.GetType();
                    var field = containerType.GetProperty(this.prpdependentPropertyName);

                    if (field != null)
                    {
                        // get the value of the dependent property
                        var dependentvalue = field.GetValue(validationContext.ObjectInstance, null);

                        foreach (var obj in this.targetValue)
                        {
                            // compare the value against the target value
                            if ((dependentvalue == null && this.targetValue == null) ||
                                (dependentvalue != null && dependentvalue.Equals(obj)) ||
                                (dependentvalue != null && Convert.ToString(dependentvalue).Contains(Convert.ToString(obj))))
                            {
                                // match => means we should try validating this field
                                if (!this.innerAttribute.IsValid(value))
                                {
                                    // validation failed - return an error
                                    return new ValidationResult(string.Format(ErrorMessageString, validationContext.DisplayName));
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (this.ValidateDependentProperty(dependentPropertyValue))
                    {
                        return new ValidationResult(string.Format(ErrorMessageString, validationContext.DisplayName));
                    }
                }
            }

            return ValidationResult.Success;
        }

        /// <summary>
        /// Validates the dependent property.
        /// </summary>
        /// <param name="actualPropertyValue">The actual property value.</param>
        /// <returns>return BOOL</returns>
        private bool ValidateDependentProperty(object actualPropertyValue)
        {
            switch (this.prpdependentPropertyComparison)
            {
                case Operator.Contains:
                    return actualPropertyValue == null ? this.prpdependentPropertyValue != null : !Convert.ToString(actualPropertyValue).Contains(Convert.ToString(this.prpdependentPropertyValue));
                case Operator.NotEqualTo:
                    return actualPropertyValue == null ? this.prpdependentPropertyValue != null : !actualPropertyValue.Equals(this.prpdependentPropertyValue);
                default:
                    return actualPropertyValue == null ? this.prpdependentPropertyValue == null : actualPropertyValue.Equals(this.prpdependentPropertyValue);
            }
        }
    }

}

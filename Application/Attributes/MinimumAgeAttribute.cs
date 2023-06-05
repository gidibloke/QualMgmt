using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Attributes
{
    public class MinimumAgeAttribute : ValidationAttribute
    {
        private readonly int _minimumAge;

        public MinimumAgeAttribute(int minimumAge)
        {
            _minimumAge = minimumAge;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime dateOfBirth)
            {
                var currentDate = DateTime.Now;
                var smallestDOB = currentDate.AddYears(-_minimumAge);

                if (dateOfBirth > smallestDOB)
                {
                    return new ValidationResult($"The minimum age requirement is {_minimumAge} years.");
                }
            }

            return ValidationResult.Success;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WS_HyJ.Helpers
{
    public class StringRangeAttribute : ValidationAttribute
    {
        public string[] AllowableValues { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (AllowableValues?.Contains(value?.ToString()) == true)
            {
                return ValidationResult.Success;
            }

            var msg = $"Por favor ingresar los valores permitidos: {string.Join(", ", (AllowableValues ?? new string[] { "No se encontraron valores permitidos." }))}.";
            return new ValidationResult(msg);
        }
    }
}

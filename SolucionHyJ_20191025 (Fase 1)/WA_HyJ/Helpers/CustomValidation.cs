using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace WA_HyJ.Helpers
{
    public class CustomValidation
    {
        public sealed class RUCValidatorAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                string ruc = value.ToString();

                if (Regex.IsMatch(ruc, @"[z1-2][0]", RegexOptions.IgnoreCase) && ruc.Length == 11)
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult("Por favor ingresa un RUC válido.");
                }
            }
        }

        public sealed class EmailValidatorAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                string email = value.ToString();

                if (Regex.IsMatch(email, @"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", RegexOptions.IgnoreCase))
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult("Por favor ingrese un email válido.");
                }
            }
        }

        /// <summary>  
        /// File extensions attribute class  
        /// </summary>  
        [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
        public class AllowExtensionsAttribute : ValidationAttribute
        {
            #region Public / Protected Properties  

            /// <summary>  
            /// Gets or sets extensions property.  
            /// </summary>  
            public string Extensions { get; set; } = "png,jpg,jpeg,gif";

            #endregion

            #region Is valid method  

            /// <summary>  
            /// Is valid method.  
            /// </summary>  
            /// <param name="value">Value parameter</param>  
            /// <returns>Returns - true is specify extension matches.</returns>  
            public override bool IsValid(object value)
            {
                // Initialization  
                HttpPostedFileBase file = value as HttpPostedFileBase;
                bool isValid = true;

                // Settings.  
                List<string> allowedExtensions = this.Extensions.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                // Verification.  
                if (file != null)
                {
                    // Initialization.  
                    var fileName = file.FileName;

                    // Settings.  
                    isValid = allowedExtensions.Any(y => fileName.EndsWith(y));
                }

                // Info  
                return isValid;
            }

            #endregion
        }
    }
}
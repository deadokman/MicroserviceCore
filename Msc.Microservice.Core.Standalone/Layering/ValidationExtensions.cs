// ------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidationExtensions.cs" company="Konyshev A.V., Stepanov N.O.">
//   Copyright (c) Konyshev A.V., Stepanov N.O.
// </copyright>
// <summary>
//   Расширение для валидации конфигурации
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Msc.Microservice.Core.Standalone.Layering
{
    /// <summary>
    /// Расширение для валидации конфигурации.
    /// </summary>
    public static class ValidationExtensions
    {
        /// <summary>
        /// Валидация ошибок конфигурации.
        /// </summary>
        /// <param name="this">Объект для проверки конфигурации.</param>
        /// <returns>Список ошибок валидации.</returns>
        public static IEnumerable<string> ValidateErrors(this object @this)
        {
            if (@this == null)
            {
                yield return "Конфигурация отсутствует (value == null)";
            }
            else
            {
                var context = new ValidationContext(@this, serviceProvider: null, items: null);
                var results = new List<ValidationResult>();
                Validator.TryValidateObject(@this, context, results, true);
                foreach (var validationResult in results)
                {
                    yield return validationResult.ErrorMessage;
                }
            }
        }
    }
}

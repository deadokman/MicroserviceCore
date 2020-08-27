// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Postgres.cs" company="Konyshev A.V., Stepanov N.O.">
//   Copyright (c) Konyshev A.V., Stepanov N.O.
// </copyright>
// <summary>
//   Конфигурация для посгреса
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using System.Configuration;

namespace Msc.Microservice.Layer.Postgres.Configuration
{
    /// <summary>
    /// Конфигурация для посгреса.
    /// </summary>
    public class Postgres
    {
        /// <summary>
        /// Строка поключения
        /// Обязательное свойство
        /// В конфигурационном файле или переменной окружения указывается как connectionString;
        /// Значение по умолчанию null;.
        /// </summary>
        [Required]
        [ConfigurationProperty("connectionString")]
        public string ConnectionString { get; set; }

        /// <summary>
        /// Интервал переподключения
        /// В конфигурационном файле указывается как reconnectInterval;
        /// Значение по умолчанию 1;.
        /// </summary>
        [ConfigurationProperty("reconnectInterval", DefaultValue = 1)]
        public int? ReconnectInterval { get; set; }
    }
}

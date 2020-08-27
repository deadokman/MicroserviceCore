// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RedisEndpoint.cs" company="Konyshev A.V., Stepanov N.O.">
//   Copyright (c) Konyshev A.V., Stepanov N.O.
// </copyright>
// <summary>
//   Конечная точка для подключения Redis
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Msc.Microservice.Layer.Redis.Configuration
{
    /// <summary>
    /// Конечная точка для подключения Redis
    /// </summary>
    public class RedisEndpoint
    {
        /// <summary>
        /// Хост подключения
        /// </summary>
        [Required]
        public string Host { get; set; }

        /// <summary>
        /// Порт подключения
        /// </summary>
        [Required]
        public int Port { get; set; }
    }
}

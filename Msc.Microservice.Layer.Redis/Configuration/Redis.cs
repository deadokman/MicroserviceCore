// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Redis.cs" company="Konyshev A.V., Stepanov N.O.">
//   Copyright (c) Konyshev A.V., Stepanov N.O.
// </copyright>
// <summary>
//   Конфигурация для Redis
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Msc.Microservice.Layer.Redis.Configuration
{
    /// <summary>
    /// Конфигурация для Redis
    /// </summary>
    public class Redis
    {
        /// <summary>
        /// Имя экземпляра слушателя
        /// </summary>
        [Required]
        public string InstanceName { get; set; }

        /// <summary>
        /// Подключение
        /// </summary>
        [Required]
        public List<RedisEndpoint> Endpoints { get; set; }

        /// <summary>
        /// время в сек, через которое требуется производить ping подключения
        /// Для того чтобы удостовериться что подключение открыто
        /// </summary>
        public int KeepAlive { get; set; } = 1;

        /// <summary>
        /// Тайминг синхронизации
        /// </summary>
        public int SyncTimeoutMs { get; set; } = 20000;

        /// <summary>
        /// Количество попыток переподключения
        /// </summary>
        public int ConnectRetry { get; set; } = 20;

        /// <summary>
        /// Пароль
        /// </summary>
        [Required]
        public string Password { get; set; }
    }
}

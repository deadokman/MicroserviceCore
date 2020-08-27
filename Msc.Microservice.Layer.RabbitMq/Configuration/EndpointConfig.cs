// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EndpointConfig.cs" company="Konyshev A.V., Stepanov N.O.">
//   Copyright (c) Konyshev A.V., Stepanov N.O.
// </copyright>
// <summary>
//   Конфигурация конечных точек в рамках очереди
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Msc.Microservice.Layer.RabbitMq.Configuration
{
    /// <summary>
    /// Конфигурация конечных точек.
    /// </summary>
    public class EndpointConfig
    {
        /// <summary>
        /// Наименование конечной точки получателя.
        /// </summary>
        public string EndpointName { get; set; }

        /// <summary>
        /// Автоподтверждение получения.
        /// </summary>
        public bool AutoAck { get; set; } = false;
    }
}

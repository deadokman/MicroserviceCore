// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RabbitMessageQueueEndpointType.cs" company="Konyshev A.V., Stepanov N.O.">
//   Copyright (c) Konyshev A.V., Stepanov N.O.
// </copyright>
// <summary>
//   Тип конечной точки подключения клиента RabbitMQ
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Msc.Microservice.Layer.RabbitMq
{
    /// <summary>
    /// Тип конечной точки подключения клиента RabbitMQ.
    /// </summary>
    public enum RabbitMessageQueueEndpointType
    {
        /// <summary>
        /// Конечной точкой является очередь
        /// </summary>
        Queue = 0,

        /// <summary>
        /// Конечной точкой является Эксчейнджер
        /// </summary>
        Exchange = 1,
    }
}

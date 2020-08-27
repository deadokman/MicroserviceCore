// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageProcessPhase.cs" company="Konyshev A.V., Stepanov N.O.">
//   Copyright (c) Konyshev A.V., Stepanov N.O.
// </copyright>
// <summary>
//  Фаза обработки сообщения
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Msc.Microservice.Layer.RabbitMq
{
    /// <summary>
    /// Фаза обработки сообщения.
    /// </summary>
    public enum MessageProcessPhase
    {
        /// <summary>
        /// Перед обработкой
        /// </summary>
        BeforeProcess,

        /// <summary>
        /// После обработки сообщения
        /// </summary>
        AfterProcess,

        /// <summary>
        /// После возникновения ошибки
        /// </summary>
        OnError,
    }
}

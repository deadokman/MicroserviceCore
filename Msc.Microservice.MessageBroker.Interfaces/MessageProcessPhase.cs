// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageProcessPhase.cs" company="ООО ИТСК">
//   Copyright (c) ООО ИТСК. All rights reserved.
// </copyright>
// <summary>
//  Фаза обработки сообщения
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Itsk.Microservice.Layer.RabbitMq
{
    /// <summary>
    /// Фаза обработки сообщения
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

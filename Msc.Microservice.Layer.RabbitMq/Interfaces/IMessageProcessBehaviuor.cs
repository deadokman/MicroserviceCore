// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessageProcessBehaviuor.cs" company="Konyshev A.V., Stepanov N.O.">
//   Copyright (c) Konyshev A.V., Stepanov N.O.
// </copyright>
// <summary>
//   Интерфейс модели поведения клиента
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Msc.Microservice.Layer.RabbitMq.Interfaces
{
    /// <summary>
    /// Интерфейс модели поведения клиента.
    /// </summary>
    public interface IMessageProcessBehaviuor
    {
        /// <summary>
        /// Вызвать обработку поведенческой модели.
        /// </summary>
        /// <param name="messageBytes">Байты сообщения.</param>
        void InvokeBehaviour(ReadOnlyMemory<byte> messageBytes);

        /// <summary>
        /// Фаза обработки сообщения.
        /// </summary>
        MessageProcessPhase ProcessPhase { get; set; }

        /// <summary>
        /// Установить ссылку на клиент.
        /// </summary>
        /// <param name="messageQueueClient">Клиент очереди.</param>
        void SetClient(IMessageQueueClient messageQueueClient);
    }
}

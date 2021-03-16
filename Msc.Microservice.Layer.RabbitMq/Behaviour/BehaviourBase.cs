// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BehaviourBase.cs" company="Konyshev A.V., Stepanov N.O.">
//   Copyright (c) Konyshev A.V., Stepanov N.O.
// </copyright>
// <summary>
//   Базовый класс поведенческой модели
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using Msc.Microservice.Layer.RabbitMq.Interfaces;

namespace Msc.Microservice.Layer.RabbitMq.Behaviour
{
    /// <summary>
    /// Базовый класс поведенческой модели.
    /// </summary>
    public abstract class BehaviourBase : IMessageProcessBehaviuor
    {
        /// <summary>
        /// Клиент очереди.
        /// </summary>
        protected IMessageQueueClient QueueClient;

        /// <summary>
        /// Вызвать обработку сообщения согласно модели поведения.
        /// </summary>
        /// <param name="messageBytes">Набор байт сообщения.</param>
        public abstract void InvokeBehaviour(ReadOnlyMemory<byte> messageBytes);

        /// <summary>
        /// Фаза обработки сообщения.
        /// </summary>
        public MessageProcessPhase ProcessPhase { get; set; }

        /// <summary>
        /// Установить ссылку на клиент.
        /// </summary>
        /// <param name="messageQueueClient">Клиент очереди.</param>
        public void SetClient(IMessageQueueClient messageQueueClient)
        {
            QueueClient = messageQueueClient;
        }
    }
}

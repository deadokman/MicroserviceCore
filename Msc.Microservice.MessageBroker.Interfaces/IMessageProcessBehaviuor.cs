// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessageProcessBehaviuor.cs" company="ООО ИТСК">
//   Copyright (c) ООО ИТСК. All rights reserved.
// </copyright>
// <summary>
//   Интерфейс модели поведения клиента
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Itsk.Microservice.Layer.RabbitMq.Interfaces
{
    /// <summary>
    /// Интерфейс модели поведения клиента
    /// </summary>
    public interface IMessageProcessBehaviuor
    {
        /// <summary>
        /// Вызвать обработку поведенческой модели
        /// </summary>
        /// <param name="messageBytes">Байты сообщения</param>
        void InvokeBehaviour(byte[] messageBytes);

        /// <summary>
        /// Фаза обработки сообщения
        /// </summary>
        MessageProcessPhase ProcessPhase { get; set; }

        /// <summary>
        /// Установить ссылку на клиент
        /// </summary>
        /// <param name="messageQueueClient">Клиент очереди</param>
        void SetClient(IMessageQueueClient messageQueueClient);
    }
}

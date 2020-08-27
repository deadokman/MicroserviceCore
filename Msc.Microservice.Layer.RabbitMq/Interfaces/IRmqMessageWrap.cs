// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRmqMessageWrap.cs" company="Konyshev A.V., Stepanov N.O.">
//   Copyright (c) Konyshev A.V., Stepanov N.O.
// </copyright>
// <summary>
//  Обертка над базовым сообщением RabbitMq
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Msc.Microservice.Layer.RabbitMq.Interfaces
{
    /// <summary>
    /// Интерфейс обертки над сообщением Rmq.
    /// </summary>
    /// <typeparam name="T">Тип полезной нагрузки.</typeparam>
    public interface IRmqMessageWrap<T>
    {
        /// <summary>
        /// Клиент MQ.
        /// </summary>
        IMessageQueueClient Client { get; }

        /// <summary>
        /// Полезная нагрузка.
        /// </summary>
        T Payload { get; }

        /// <summary>
        /// Подтвердить обработку сообщения.
        /// </summary>
        void Ack();

        /// <summary>
        /// Отметить сообщение как "не обработанное".
        /// </summary>
        /// <param name="requeue">true - сообщение помещается обратно в очередь с тем же самым DeliveryTag.</param>
        void Nack(bool requeue = false);
    }
}

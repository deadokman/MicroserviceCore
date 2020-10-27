// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RmqMessageBase.cs" company="Konyshev A.V., Stepanov N.O.">
//   Copyright (c) Konyshev A.V., Stepanov N.O.
// </copyright>
// <summary>
//  Обертка над базовым сообщением RabbitMq
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Msc.Microservice.Layer.RabbitMq.Interfaces;

namespace Msc.Microservice.Layer.RabbitMq
{
    /// <summary>
    /// Базовая обертка сообщения.
    /// </summary>
    /// <typeparam name="T">Payload сообщения.</typeparam>
    internal sealed class RmqMessageBase<T> : IRmqMessageWrap<T>
    {

        /// <summary>
        /// Подтверждение сообщения.
        /// </summary>
        public Acknoledge Acknoledge { get; set; }

        /// <summary>
        /// Payload.
        /// </summary>
        public T Payload { get; set; }

        /// <summary>
        /// Клиент очереди.
        /// </summary>
        public IMessageQueueClient Client { get; set; }

        private bool _notCompleted = true;

        /// <summary>
        /// Подтвердить обработку сообщения.
        /// </summary>
        public void Ack()
        {
            if (_notCompleted)
            {
                Acknoledge.Invoke(true, false);
            }

            _notCompleted = false;
        }

        /// <summary>
        /// Отметить сообщение как "не обработанное".
        /// </summary>
        /// <param name="requeue">true - сообщение помещается обратно в очередь с тем же самым DeliveryTag.</param>
        public void Nack(bool requeue = false)
        {
            if (_notCompleted)
            {
                Acknoledge.Invoke(false, requeue);
            }

            _notCompleted = false;
        }
    }
}

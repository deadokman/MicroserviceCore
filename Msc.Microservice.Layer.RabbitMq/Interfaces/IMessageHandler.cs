// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessageHandler.cs" company="Konyshev A.V., Stepanov N.O.">
//   Copyright (c) Konyshev A.V., Stepanov N.O.
// </copyright>
// <summary>
//   Выполняет распределение сообщений по хендлерам
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Msc.Microservice.Layer.RabbitMq.Interfaces
{
    /// <summary>
    /// Хэндлер сообщения очереди с нагрузкой T.
    /// </summary>
    /// <typeparam name="T">Тип обрабатываемого сообщения.</typeparam>
    public interface IMessageHandler<T>
    {
        /// <summary>
        /// Выполнить обработку сообщения в соответствии с его типом.
        /// </summary>
        /// <param name="msg">Сообщение.</param>
        void HandleMessage(IRmqMessageWrap<T> msg);
    }
}

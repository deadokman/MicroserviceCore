// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessageHandler.cs" company="ООО ИТСК">
//   Copyright (c) ООО ИТСК. All rights reserved.
// </copyright>
// <summary>
//   Выполняет распределение сообщений по хендлерам
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Itsk.Microservice.Layer.RabbitMq.Interfaces
{
    /// <summary>
    /// Хэндлер сообщения очереди с нагрузкой T
    /// </summary>
    /// <typeparam name="T">Тип обрабатываемого сообщения</typeparam>
    public interface IMessageHandler<T>
    {
        /// <summary>
        /// Выполнить обработку сообщения в соответствии с его типом
        /// </summary>
        /// <param name="msg">Сообщение</param>
        void HandleMessage(IRmqMessageWrap<T> msg);
    }
}

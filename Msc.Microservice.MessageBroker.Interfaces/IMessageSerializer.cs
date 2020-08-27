// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessageSerializer.cs" company="ООО ИТСК">
//   Copyright (c) ООО ИТСК. All rights reserved.
// </copyright>
// <summary>
//  Интерфейс сериализатора сообщений очереди
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Itsk.Microservice.Layer.RabbitMq.Interfaces
{
    /// <summary>
    /// Интерфейс сериализотора сообщений очереди
    /// </summary>
    public interface IMessageSerializer
    {
        /// <summary>
        /// Преобразовать сообщение которое подразумевает только передачу данных в набор байт для отправки
        /// </summary>
        /// <param name="message"> Сообщение </param>
        /// <param name="messageType"> Тип сообщения </param>
        /// <returns>Сериализованный байт-массв</returns>
        byte[] SerializeTransferMessage(object message, Type messageType);

        /// <summary>
        /// Преобразовать в объект сообщение, которое подразумевает только передачу данных
        /// </summary>
        /// <param name="body"> Тело сообщения </param>
        /// <param name="requestArgType"> Тип сообщения </param>
        /// <returns>Возвращает диссериализованную инстанцию объекта</returns>
        object DeserializeRpcMessage(byte[] body, Type requestArgType);

        /// <summary>
        /// Преобразовать в объект сообщение, которое подразумевает только передачу данных
        /// </summary>
        /// <param name="body"> Тело сообщения </param>
        /// <param name="contentType"> Тип сообщения </param>
        /// <returns>Возвращает диссериализованную инстанцию объекта</returns>
        object DeserializeTransferMessage(byte[] body, out Type contentType);
    }
}

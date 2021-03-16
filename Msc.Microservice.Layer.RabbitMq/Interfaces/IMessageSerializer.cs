// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessageSerializer.cs" company="Konyshev A.V., Stepanov N.O.">
//   Copyright (c) Konyshev A.V., Stepanov N.O.
// </copyright>
// <summary>
//  Интерфейс сериализатора сообщений очереди
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Msc.Microservice.Layer.RabbitMq.Interfaces
{
    /// <summary>
    /// Интерфейс сериализотора сообщений очереди.
    /// </summary>
    public interface IMessageSerializer
    {
        /// <summary>
        /// Преобразовать сообщение которое подразумевает только передачу данных в набор байт для отправки.
        /// </summary>
        /// <param name="message"> Сообщение. </param>
        /// <param name="messageType"> Тип сообщения. </param>
        /// <returns>Сериализованный байт-массв.</returns>
        byte[] SerializeTransferMessage(object message, Type messageType);

        /// <summary>
        /// Преобразовать payload в json для передачи.
        /// </summary>
        /// <param name="payload">Данные сообщения.</param>
        /// <returns>JSON - объект сообщения.</returns>
        string SerializePayload(object payload);

        /// <summary>
        /// Преобразовать в объект сообщение, которое подразумевает только передачу данных.
        /// </summary>
        /// <param name="body"> Тело сообщения. </param>
        /// <param name="requestArgType"> Тип сообщения. </param>
        /// <returns>Возвращает диссериализованную инстанцию объекта.</returns>
        object DeserializeRpcMessage(ReadOnlyMemory<byte> body, Type requestArgType);

        /// <summary>
        /// Диссриализовать сообщения из байт-массива.
        /// </summary>
        /// <param name="body">Тело сообщения.</param>
        /// <param name="contentType">Тип объектной модели сообщения.</param>
        /// <param name="contractTypes">Делегат на получение типа контракта по алиасу.</param>
        /// <returns>Возвращает диссериализованный объект содержимого сообщения.</returns>
        object DeserializeTransferMessage(ReadOnlyMemory<byte> body, out Type contentType, Func<string, Type> contractTypes = null);
    }
}

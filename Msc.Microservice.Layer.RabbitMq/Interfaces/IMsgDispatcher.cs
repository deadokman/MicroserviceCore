// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMsgDispatcher.cs" company="Konyshev A.V., Stepanov N.O.">
//   Copyright (c) Konyshev A.V., Stepanov N.O.
// </copyright>
// <summary>
//   Выполняет распределение сообщений по хендлерам
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Msc.Microservice.Layer.RabbitMq.Interfaces
{
    /// <summary>
    /// Интерфейс диспетчера сообщеий очереди.
    /// </summary>
    public interface IMsgDispatcher
    {
        /// <summary>
        /// Попытаться апоулчить тип по алиасу.
        /// </summary>
        /// <param name="alias">Алиас.</param>
        /// <returns>Возвращаемый тип.</returns>
        Type TryGetByAlias(string alias);

        /// <summary>
        /// Выполинть обработку сообщения очереди через диспетчер.
        /// </summary>
        /// <param name="client">Клиент очереди.</param>
        /// <param name="acknowledge">Делегат, отвечающий за ручное подтверждение или откат сообщения.</param>
        /// <param name="message">Сообщение.</param>
        /// <param name="payloadType">Тип сообщения.</param>
        void DispatchMessage(IMessageQueueClient client, Acknoledge acknowledge, object message, Type payloadType);

        /// <summary>
        /// Обработать RPC запрос.
        /// </summary>
        /// <param name="client">Клиент очереди.</param>
        /// <param name="message">Сообщение.</param>
        /// <param name="acknowledge">Делегат ручного подтверждения или отката обработки сообщения.</param>
        /// <param name="argumentType">Тип входного аргумента.</param>
        /// <param name="responseType">Тип ответа.</param>
        /// <returns>Возвращает результат RPC запроса.</returns>
        object HandleRpcMessage(IMessageQueueClient client, object message, Acknoledge acknowledge, Type argumentType, Type responseType);
    }

    /// <summary>
    /// Делегат подтверждения (или отката) обработки сообшения при ручной обработке.
    /// </summary>
    /// <param name="ack">true - подтверждение обработки false - unack.</param>
    /// <param name="requeue">Если true и ack = false сообщение ставится в начало очереди.</param>
    public delegate void Acknoledge(bool ack, bool requeue);
}

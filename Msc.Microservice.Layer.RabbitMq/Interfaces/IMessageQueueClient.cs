// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessageQueueClient.cs" company="Konyshev A.V., Stepanov N.O.">
//   Copyright (c) Konyshev A.V., Stepanov N.O.
// </copyright>
// <summary>
//   Клиент брокера сообщений
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Msc.Microservice.Layer.RabbitMq.Configuration;
using RabbitMQ.Client;

namespace Msc.Microservice.Layer.RabbitMq.Interfaces
{
    /// <summary>
    /// Клиент брокера очереди.
    /// </summary>
    public interface IMessageQueueClient : IDisposable
    {
        /// <summary>
        /// Опубликовать сообщение в брокере в конкретной очереди.
        /// </summary>
        /// <param name="queue">Наименование очереди.</param>
        /// <param name="msg">Сообщение.</param>
        /// <param name="props">Свосйства запроса.</param>
        /// <param name="messageType">Тип отправляемого сообщения.</param>
        void PublishMessage(string queue, object msg, IBasicProperties props = null, Type messageType = null);

        /// <summary>
        /// Подключено
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Отправить байты сообщеия.
        /// </summary>
        /// <param name="queue">Конечная точка.</param>
        /// <param name="bytes">Набор байт.</param>
        /// <param name="props">Свойства.</param>
        void SendBytes(string queue, ReadOnlyMemory<byte> bytes, IBasicProperties props = null);

        /// <summary>
        /// Опубликовать сообщение в брокере в конкретной очереди.
        /// </summary>
        /// <param name="exchanger">Эксчейнджер.</param>
        /// <param name="queue">Наименование очереди.</param>
        /// <param name="msg">Сообщение.</param>
        /// <param name="props">Свосйства запроса.</param>
        /// <param name="messageType">Тип отправляемого сообщения.</param>
        void PublishMessage(string exchanger, string queue, object msg, IBasicProperties props = null, Type messageType = null);

        /// <summary>
        /// Начать получение сообщений.
        /// </summary>
        void BeginConsume();

        /// <summary>
        /// Выполнить RPC запрос.
        /// </summary>
        /// <typeparam name="TArgs">Аргумент.</typeparam>
        /// <typeparam name="TResp">Результат запроса.</typeparam>
        /// <param name="reqArgs">Экземпляр аргумента.</param>
        /// <param name="queue">Очередь.</param>
        /// <param name="timeout">Задержка ожидания запроса.</param>
        /// <returns>Асинхронный результат запроса.</returns>
        Task<TResp> MakeRpcCallAsync<TArgs, TResp>(TArgs reqArgs, string queue, TimeSpan? timeout = null);

        /// <summary>
        /// Настроить клиент.
        /// </summary>
        void SetUpClient();

        /// <summary>
        /// Добавить конфигурацию для конечной точки
        /// </summary>
        /// <param name="ep">Конечная точка</param>
        void AppendEndpoint(EndpointConfig ep);

        /// <summary>
        /// Создать очередь
        /// </summary>
        /// <param name="queue">Очередь</param>
        /// <param name="durable">Durable</param>
        /// <param name="autoDelete">AUto deltet</param>
        /// <param name="vhost">Vhost</param>
        void QueueDeclare(string queue, bool durable = false, bool autoDelete = true, string vhost = "");

        /// <summary>
        /// Удаление очереди
        /// </summary>
        /// <param name="queueName">Имя очереди</param>
        void DeleteQueue(string queueName);

        /// <summary>
        /// Добавить биндинг между эксчейнджером и очередью
        /// </summary>
        /// <param name="exchanger">Эксчейнджер</param>
        /// <param name="queue">Очередь</param>
        /// <param name="routingKey">Routing key</param>
        void CreateQueueBinding(string exchanger, string queue, string routingKey = "");
    }
}

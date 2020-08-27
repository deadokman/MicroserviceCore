// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RabbitClientBase.cs" company="Konyshev A.V., Stepanov N.O.">
//   Copyright (c) Konyshev A.V., Stepanov N.O.
// </copyright>
// <summary>
//   Реализация клиента RabbitMQ для ед. окна
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Msc.Microservice.Layer.RabbitMq.Configuration;
using Msc.Microservice.Layer.RabbitMq.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Msc.Microservice.Layer.RabbitMq
{
    /// <summary>
    /// Базовый класс клиента RMQ.
    /// </summary>
    public abstract class RabbitClientBase : IDisposable
    {
        /// <summary>
        /// Диспетчер сообщений текущего клиента.
        /// </summary>
        protected IMsgDispatcher Dispatcher;

        /// <summary>
        /// Логгер.
        /// </summary>
        protected ILogger Logger;

        /// <summary>
        /// Текущая реализация сериализатора сообщений.
        /// </summary>
        protected IMessageSerializer Serializer;

        /// <summary>
        /// Конфигурация текущего клиента.
        /// </summary>
        protected QueuesConfig Configuration;

        /// <summary>
        /// Подключение RabbitMq.
        /// </summary>
        protected IConnection Connection;

        /// <summary>
        /// Модель данных.
        /// </summary>
        protected IModel Model;

        /// <summary>
        /// Список зарегестрированных источников потребления данных.
        /// </summary>
        protected Dictionary<string, EventingBasicConsumer> RegistratedConsumers { get; set; }

        /// <summary>
        /// Фабрика подключений рэббита.
        /// </summary>
        protected ConnectionFactory ConnectionFactory { get; set; }

        /// <summary>
        /// Счетчик доставок сообщений.
        /// </summary>
        private Dictionary<ulong, int> DeliveryCount { get; set; }

        /// <summary>
        /// Справочник поведений.
        /// </summary>
        public Dictionary<MessageProcessPhase, List<IMessageProcessBehaviuor>> BehavioursDict;

        /// <summary>
        /// Клиент RabbitMq.
        /// </summary>
        /// <param name="configuration"> Конфигурация клиента. </param>
        /// <param name="dispatcher"> Диспетчер сообщений. </param>
        /// <param name="logger"> Логгер. </param>
        /// <param name="serializer"> Сериализатор сообщений. </param>
        public RabbitClientBase(QueuesConfig configuration, IMsgDispatcher dispatcher, ILogger<IMessageQueueClient> logger, IMessageSerializer serializer)
        {
            Configuration = configuration ?? throw new ArgumentNullException($"Конфигурация клиента очереди не найдена или содержит ошибки");
            Dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            Logger = logger ?? throw new ArgumentException(nameof(logger));
            Serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));

            DeliveryCount = new Dictionary<ulong, int>();
            RegistratedConsumers = new Dictionary<string, EventingBasicConsumer>();
            ConnectionFactory = new ConnectionFactory
            {
                RequestedHeartbeat = 5,
            };
        }

        /// <summary>
        /// Очистить занимаеме очередью ресурсы.
        /// </summary>
        public abstract void Dispose();

        /// <summary>
        /// Изменить состояние счетчиков доставки если сообщение не доставлено.
        /// </summary>
        /// <param name="deliveryTag">Тег сообщения.</param>
        /// <returns>Возвращает true если сообщение следует попробовать доставить еще раз и false если оно считаетя окончательно сломанным.</returns>
        protected bool UndeliveredRetries(ulong deliveryTag)
        {
            if (!DeliveryCount.ContainsKey(deliveryTag))
            {
                DeliveryCount.Add(deliveryTag, 1);
                return true;
            }

            if (DeliveryCount[deliveryTag] > Configuration.RedeliveryRetries.Value)
            {
                DeliveryCount.Remove(deliveryTag);
                return false;
            }

            DeliveryCount[deliveryTag]++;
            return true;
        }

        /// <summary>
        /// Создать подключение к RabbitMq.
        /// </summary>
        /// <param name="connectionFactory">Фабрика подключений.</param>
        /// <returns>Подключение.</returns>
        protected IConnection CreateConnection(ConnectionFactory connectionFactory)
        {
            var waitTime = Configuration.PauseBetweenAttemts ?? 15;
            var maxAttempts = Configuration.ConnectionAttempts ?? 3;
            IConnection conn = null;
            for (var attempt = 1; attempt <= maxAttempts && conn == null; attempt++)
            {
                try
                {
                    Logger.LogTrace($"Попытка подключения клиента {Configuration.ClientName} по адресу {Configuration.HostName}:{Configuration.Port} № - {attempt}");
                    conn = connectionFactory.CreateConnection();
                }
                catch (RabbitMQ.Client.Exceptions.BrokerUnreachableException ex)
                {
                    Logger.LogWarning($"Не удалось подключиться клиенту {Configuration.ClientName} к RabbitMq попытка № - {attempt} клиент засыпает на {waitTime} сек.");
                    if (attempt >= maxAttempts)
                    {
                        var msg = $"Клиенту {Configuration.ClientName} не удалось подключиться к шине в результате {maxAttempts} попыток";
                        Logger.LogError(msg);
                        throw new Exception(msg, ex);
                    }

                    System.Threading.Thread.Sleep(waitTime * 1000);
                }

                Logger.LogTrace($"Клиент {Configuration.ClientName} установил соединение по адресу {Configuration.HostName}:{Configuration.Port} попыток: {attempt}");
            }

            return conn;
        }

        /// <summary>
        /// Инициализировать клиент.
        /// </summary>
        protected void Initialize()
        {
            ConnectionFactory.Configure(Configuration);
            Connection = CreateConnection(ConnectionFactory);
            Model = Connection.CreateModel();
            Model.Configure(Configuration);
        }
    }
}

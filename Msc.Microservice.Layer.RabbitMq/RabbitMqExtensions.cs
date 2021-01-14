// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RabbitMqExtensions.cs" company="Konyshev A.V., Stepanov N.O.">
//   Copyright (c) Konyshev A.V., Stepanov N.O.
// </copyright>
// <summary>
//  Методы расширений для RabbitMq
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Msc.Microservice.Layer.RabbitMq.Configuration;
using RabbitMQ.Client;

namespace Msc.Microservice.Layer.RabbitMq
{
    /// <summary>
    /// Класс расширений для RabbitMQ.
    /// </summary>
    public static class RabbitMqExtensions
    {
        /// <summary>
        /// Конфигурация фабрики подключений.
        /// </summary>
        /// <param name="connectionFactory"> Фабрика подключений. </param>
        /// <param name="config"> Конфигурация. </param>
        public static void Configure(this ConnectionFactory connectionFactory, QueuesConfig config)
        {
            connectionFactory.UserName = config.UserName;
            connectionFactory.Password = config.Password;
            connectionFactory.VirtualHost = config.VirtualHost;
            connectionFactory.Protocol = Protocols.DefaultProtocol;
            connectionFactory.HostName = config.HostName;
            connectionFactory.Port = config.Port;
            connectionFactory.RequestedHeartbeat = 30;
            connectionFactory.AutomaticRecoveryEnabled = true;
        }

        /// <summary>
        /// Сконфигурировать модель.
        /// </summary>
        /// <param name="model"> Модель данных. </param>
        /// <param name="config"> Конфигурация. </param>
        public static void Configure(this IModel model, QueuesConfig config)
        {
            if (config.PrefetchCount.HasValue)
            {
                model.BasicQos(0, config.PrefetchCount.Value, false);
            }
            else
            {
                model.BasicQos(0, 1, false);
            }
        }
    }
}

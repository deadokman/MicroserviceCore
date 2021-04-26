// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueuesConfig.cs" company="Konyshev A.V., Stepanov N.O.">
//   Copyright (c) Konyshev A.V., Stepanov N.O.
// </copyright>
// <summary>
//   Конфигурация клиента RabbitMq
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.ComponentModel.DataAnnotations;

namespace Msc.Microservice.Layer.RabbitMq.Configuration
{
    /// <summary>
    /// Класс описывающий конфигурацию текущего RabbitMQ клиента.
    /// </summary>
    public class QueuesConfig
    {
        /// <summary>
        /// Мапинг неймспейсов.
        /// </summary>
        public MessageNamespaceMapItem[] NamespaceMap { get; set; }

        /// <summary>
        /// Конфигурация конечных точек.
        /// </summary>
        public EndpointConfig[] Endpoints { get; set; }

        /// <summary>
        /// Модель поведений при обработке сообщения.
        /// </summary>
        public BehaviourConfig[] BehaviourConfigurations { get; set; }

        /// <summary>
        /// Наименование клиента (для отображения в логгировании).
        /// </summary>
        [Required]
        public string ClientName { get; set; }

        /// <summary>
        /// Имя хоста-подключения.
        /// </summary>
        [Required(ErrorMessage = "Требуется указать имя хоста")]
        public string HostName { get; set; }

        /// <summary>
        /// Порт подключения.
        /// </summary>
        [Required(ErrorMessage ="Требуется указать порт")]
        [RegularExpression("[0-9]+")]
        public int Port { get; set; }

        /// <summary>
        /// Имя пользователя.
        /// </summary>
        [Required(ErrorMessage = "Требуется указать имя пользователя")]
        public string UserName { get; set; }

        /// <summary>
        /// Пароль подключения.
        /// </summary>
        [Required(ErrorMessage = "Требуется указать пароль")]
        public string Password { get; set; }

        /// <summary>
        /// Число попыток подключения.
        /// </summary>
        public int? ConnectionAttempts { get; set; } = 10;

        /// <summary>
        /// Пауза между попытками (сек.)
        /// </summary>
        public int? PauseBetweenAttemts { get; set; } = 60;

        /// <summary>
        /// Интервал подключения к очереди
        /// </summary>
        public int ConnectHeartbeatSec { get; set; } = 60;

        /// <summary>
        /// Интервал подключения к очереди
        /// </summary>
        public int ContinuationTimeoutSec { get; set; } = 25;

        /// <summary>
        /// HandshakeContinuationTimeoutSec
        /// </summary>
        public int HandshakeContinuationTimeoutSec { get; set; } = 15;

        /// <summary>
        /// RequestedConnectionTimeoutSec
        /// </summary>
        public int RequestedConnectionTimeoutSec { get; set; } = 30;

        /// <summary>
        /// Количество попыток поворной доставки после провала.
        /// </summary>
        public int? RedeliveryRetries { get; set; } = 10;

        /// <summary>
        /// Использовать RPC. True по умолчанию.
        /// </summary>
        public bool UseRpc { get; set; } = true;

        /// <summary>
        /// Таймаут RPC операции, 1 минута по умолчанию.
        /// </summary>
        public TimeSpan RpcTimeout { get; set; } = new TimeSpan(0, 0, 5, 0);

        /// <summary>
        /// Тип конечной точки подключения клиента (EndpointName или Exchanger).
        /// </summary>
        public RabbitMessageQueueEndpointType EndpointType { get; set; }

        /// <summary>
        /// Максимальное количество сообщений, которое принимает клиент до подтверждения предыдущих в очереди
        /// </summary>
        public ushort? PrefetchCount { get; set; } = 1;

        /// <summary>
        /// Виртуальный сегмент
        /// </summary>
        public string VirtualHost { get; set; } = "/";
    }
}

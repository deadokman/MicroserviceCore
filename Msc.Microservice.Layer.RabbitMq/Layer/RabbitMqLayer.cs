// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RabbitMqLayer.cs" company="Konyshev A.V., Stepanov N.O.">
//   Copyright (c) Konyshev A.V., Stepanov N.O.
// </copyright>
// <summary>
//   Представляет собой интерфейс слоя микросервиса, наделяющий его той или иной функциональностью
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Msc.Microservice.Core.Standalone.Interfaces;
using Msc.Microservice.Core.Standalone.Layering;
using Msc.Microservice.Layer.RabbitMq.Configuration;
using Msc.Microservice.Layer.RabbitMq.Dispatcher;
using Msc.Microservice.Layer.RabbitMq.Interfaces;

namespace Msc.Microservice.Layer.RabbitMq.Layer
{
    /// <summary>
    /// Слой доступа и получения данных из RabbitMQ.
    /// </summary>
    public class RabbitMqLayer : IRunnableLayer
    {
        private readonly Type[] _handlerTypes;
        private readonly bool _beginConsumeAuto = false;
        private readonly QueuesConfig _queuesConfig = null;

        /// <summary>
        /// Конструктор слоя.
        /// </summary>
        /// <param name="runConsumeAuto">Запустить получение сообщений в Runnable слое.</param>
        /// <param name="handlerTypes">Хэндлеры для регистрации.</param>
        public RabbitMqLayer(bool runConsumeAuto, params Type[] handlerTypes)
        {
            _handlerTypes = handlerTypes;
            _beginConsumeAuto = runConsumeAuto;
        }

        /// <summary>
        /// Выполнить регистрацию и валидацию конфигураций.
        /// </summary>
        /// <param name="configurationRoot">Валидация конфигураций.</param>
        public void RegisterConfiguration(IConfigurationBuilder configurationRoot)
        {
            return;
        }

        /// <summary>
        /// Выполнить регистрацию и валидацию конфигураций.
        /// </summary>
        /// <param name="configurationRoot">Валидация конфигураций.</param>
        /// <param name="serviceCollection">Коллекция служб.</param>
        /// <returns>Список ошибок во время конфигурирования.</returns>
        public IEnumerable<string> RegisterLayer(IConfigurationRoot configurationRoot, IServiceCollection serviceCollection)
        {
            var configuration = _queuesConfig ?? configurationRoot.GetSection(nameof(QueuesConfig)).Get<QueuesConfig>();
            if (configuration == null)
            {
                throw new Exception("Не удалось произвести конфигурацию слоя доступа к RabbitMQ т.к. не задана конфигурация QueuesConfig. \n " +
                                    "Убедитесь в наличии секции QueuesConfig в конфигурационном файле или используйте перегрузку конструктора слоя с конфигурацией");
            }

            var errors = configuration.ValidateErrors().ToList();
            if (errors.Any())
            {
                return errors;
            }

            errors = new List<string>();
            var handlerType = typeof(IMessageHandler<>);
            var rpcHandlerType = typeof(IRpcMessageHandler<,>);

            serviceCollection.Configure<QueuesConfig>(configurationRoot.GetSection(nameof(QueuesConfig)));

            // Зарегистрировать клиент
            serviceCollection.AddSingleton<IMessageQueueClient, RabbitMqClient>();
            serviceCollection.AddTransient<IMessageSerializer, OwJsonSerializer>();
            var acceptableHandlers = new List<Type>();
            if (_handlerTypes != null)
            {
                foreach (var handlerInstanceType in _handlerTypes)
                {
                    // Проверить каждый тип, что он реализует хендлер.
                    var handlerIfaces = handlerInstanceType.GetInterfaces()
                        .Where(iface => iface.IsGenericType).Select(iface => iface.GetGenericTypeDefinition());
                    if (handlerIfaces.Any(iface => iface == rpcHandlerType || iface == handlerType))
                    {
                        acceptableHandlers.Add(handlerInstanceType);
                    }
                    else
                    {
                        errors.Add($"Переданный тип хендлера {handlerInstanceType} не реализует ни один из интерфейсов");
                        continue;
                    }

                    serviceCollection.AddTransient(handlerInstanceType);
                }
            }

            serviceCollection.AddTransient<IMsgDispatcher>((p) =>
            {
                var handlers = acceptableHandlers.Select(ht => p.GetService(ht)).ToArray();
                return new MessageDispatcher(p.GetService<ILogger<IMsgDispatcher>>(), handlers);
            });
            return errors;
        }

        /// <summary>
        /// Запустить выполннение операций в слое асинхронно.
        /// </summary>
        /// <param name="serviceProvider">Провайдер служб.</param>
        public void RunAsync(IServiceProvider serviceProvider)
        {
            var rmqClient = serviceProvider.GetService<IMessageQueueClient>();
            rmqClient.SetUpClient();
            if (_beginConsumeAuto)
            {
                try
                {
                    rmqClient.BeginConsume();
                }
                catch (Exception e)
                {
                    throw new Exception($"Ошибка во время начала получения сообщений через RabbitMq в слое {this}", e);
                }
            }
        }

        /// <summary>
        /// Отключить работу службы.
        /// </summary>
        /// <param name="serviceProvider">Провайдер служб.</param>
        public void Shutdown(IServiceProvider serviceProvider)
        {
            var rmqClient = serviceProvider.GetService<IMessageQueueClient>();
            rmqClient.Dispose();
        }
    }
}

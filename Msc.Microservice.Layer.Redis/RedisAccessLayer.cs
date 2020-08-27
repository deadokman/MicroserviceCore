// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RedisAccessLayer.cs" company="Konyshev A.V., Stepanov N.O.">
//   Copyright (c) Konyshev A.V., Stepanov N.O.
// </copyright>
// <summary>
//   Слой доступа к Redis
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Msc.Microservice.Core.Standalone.Interfaces;
using Msc.Microservice.Core.Standalone.Layering;
using RedLockNet;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;

namespace Msc.Microservice.Layer.Redis
{
    /// <summary>
    /// Слой доступа к Redis
    /// </summary>
    public class RedisAccessLayer : IRunnableLayer
    {
        private static Lazy<ConnectionMultiplexer> _lazyConnection;

        private RedLockFactory _lockFactory;

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
        /// <param name="config">Валидация конфигураций.</param>
        /// <param name="services">Коллекция служб.</param>
        /// <returns>Список ошибок во время конфигурирования.</returns>
        public IEnumerable<string> RegisterLayer(IConfigurationRoot config, IServiceCollection services)
        {
            var redisConfiguration = config.GetSection(nameof(Configuration.Redis)).Get<Configuration.Redis>();
            var configurationErrors = redisConfiguration.ValidateErrors();

            var configureationOptions = new ConfigurationOptions()
            {
                KeepAlive = redisConfiguration.KeepAlive,
                Password = redisConfiguration.Password,
                ConnectRetry = redisConfiguration.ConnectRetry,
                SyncTimeout = redisConfiguration.SyncTimeoutMs,
            };
            foreach (var ep in redisConfiguration.Endpoints)
            {
                configureationOptions.EndPoints.Add(ep.Host, ep.Port);
            }

            services.Configure<RedisCacheOptions>((options) =>
                {
                    options.InstanceName = redisConfiguration.InstanceName;
                    options.ConfigurationOptions = configureationOptions;
                });

            services.AddSingleton<IDistributedLockFactory>(
                (sp) =>
                    {
                        if (_lockFactory == null)
                        {
                            var endpoints = configureationOptions.EndPoints.Select(ep =>
                                    new RedLockEndPoint(ep)
                                    {
                                        SyncTimeout = configureationOptions.SyncTimeout,
                                        Password = configureationOptions.Password,
                                    })
                                .ToList();
                            var redLockConfiguration = new RedLockConfiguration(
                                endpoints,
                                sp.GetService<ILoggerFactory>());
                            _lockFactory = new RedLockFactory(redLockConfiguration);
                        }

                        return _lockFactory;
                    });

            services.AddSingleton<IDistributedCache, RedisCache>();
            return configurationErrors;
        }

        /// <summary>
        /// Запустить выполннение операций в слое асинхронно.
        /// </summary>
        /// <param name="serviceProvider">Провайдер служб.</param>
        public void RunAsync(IServiceProvider serviceProvider)
        {
            return;
        }

        /// <summary>
        /// Отключить работу службы.
        /// </summary>
        /// <param name="serviceProvider">Провайдер служб.</param>
        public void Shutdown(IServiceProvider serviceProvider)
        {
            _lockFactory.Dispose();
        }
    }
}

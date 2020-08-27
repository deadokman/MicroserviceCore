// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GrpcServerAccessLayer.cs" company="Konyshev A.V., Stepanov N.O.">
//   Copyright (c) Konyshev A.V., Stepanov N.O.
// </copyright>
// <summary>
//   Слой доступа к серверу, протокол взаимодействия Grpc
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Grpc.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Msc.Microservice.Core.Standalone.Interfaces;
using Msc.Microservice.GrpcCore.Configuration;

namespace Msc.Microservice.Layer.GrpcServer
{
    /// <summary>
    /// Слой доступа к серверу, протокол взаимодействия Grpc.
    /// </summary>
    public class GrpcServerAccessLayer : IRunnableLayer
    {
        private readonly Func<IServiceProvider, ServerServiceDefinition>[] _services;

        /// <summary>
        /// Конструктор, принимает коллекцию определений сервисов. Регистрирует обработчики для взяимодействия с методами сервисов.
        /// </summary>
        /// <param name="services">Коллекция определений сервисов.</param>
        public GrpcServerAccessLayer(params Func<IServiceProvider, ServerServiceDefinition>[] services)
        {
            _services = services;
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
            var errors = new List<string>();
            var section = configurationRoot.GetSection("GrpcServer");
            var serverConfig = section.Get<GrpcConfig>();

            if (serverConfig == null)
            {
                errors.Add("Отсутствует секция GrpcServer в конфигурационном файле");
                return errors;
            }

            errors = serverConfig.Validate();
            if (errors.Count != 0)
            {
                return errors;
            }

            serviceCollection.Configure<GrpcConfig>(section);
            serviceCollection.AddSingleton<IGrpcServer, BaseGrpcServer>();

            return errors;
        }

        /// <summary>
        /// Запустить выполннение операций в слое асинхронно.
        /// </summary>
        /// <param name="serviceProvider">Провайдер служб.</param>
        public void RunAsync(IServiceProvider serviceProvider)
        {
            try
            {
                var server = serviceProvider.GetService<IGrpcServer>();
                var grpcServices = _services.Select(s => s.Invoke(serviceProvider));
                server.Start(grpcServices);
            }
            catch (Exception e)
            {
                throw new Exception($"Ошибка во время запуска сервера Grpc {this}", e);
            }
        }

        /// <summary>
        /// Отключить работу службы.
        /// </summary>
        /// <param name="serviceProvider">Провайдер служб.</param>
        public void Shutdown(IServiceProvider serviceProvider)
        {
            var server = serviceProvider.GetService<IGrpcServer>();
            server.Shutdown();
        }
    }
}

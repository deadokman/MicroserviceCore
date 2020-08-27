// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GrpcClientAccessLayer.cs" company="Konyshev A.V., Stepanov N.O.">
//   Copyright (c) Konyshev A.V., Stepanov N.O.
// </copyright>
// <summary>
//   Слой регистрирует канал подключения к серверу Grpc
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using Grpc.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Msc.Microservice.Core.Standalone.Layering;
using Msc.Microservice.GrpcCore.Configuration;

namespace Msc.Microservice.Layer.GrpcClient
{
    /// <summary>
    /// Слой регистрирует канал подключения к серверу Grpc.
    /// </summary>
    public class GrpcClientAccessLayer : MicroserviceLayerBase
    {
        private readonly ClientCache _cache;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="GrpcClientAccessLayer"/>.
        /// </summary>
        /// <param name="cache">Кеш клиентов.</param>
        public GrpcClientAccessLayer(ClientCache cache)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        /// <summary>
        /// Зарегистрировать конфигурационный файл.
        /// </summary>
        /// <param name="configurationBuilder">Построитель конфигурации.</param>
        public override void RegisterConfiguration(IConfigurationBuilder configurationBuilder)
        {
        }

        /// <summary>
        /// Выполнить регистрацию и валидацию конфигураций.
        /// </summary>
        /// <param name="configurationRoot">Валидация конфигураций.</param>
        /// <param name="serviceCollection">Коллекция служб.</param>
        /// <returns>Список ошибок конфигурации.</returns>
        public override IEnumerable<string> RegisterLayer(IConfigurationRoot configurationRoot, IServiceCollection serviceCollection)
        {
            var errors = new List<string>();
            var section = configurationRoot.GetSection("GrpcClient");
            var config = section.Get<GrpcConfig>();

            if (config == null)
            {
                errors.Add("Отсутствует секция GrpcServer в конфигурационном файле");
                return errors;
            }

            errors = config.Validate();
            if (errors.Count != 0)
            {
                return errors;
            }

            if (config.Ssl != null)
            {
                var cacert = File.ReadAllText(Path.Combine(config.Ssl.Path, config.Ssl.CaCert));
                var clientCert = File.ReadAllText(Path.Combine(config.Ssl.Path, config.Ssl.CertificateChain));
                var clientKey = File.ReadAllText(Path.Combine(config.Ssl.Path, config.Ssl.PrivateKey));
                var ssl = new SslCredentials(cacert, new KeyCertificatePair(clientCert, clientKey));

                var options = new List<ChannelOption>
                {
                    new ChannelOption(ChannelOptions.SslTargetNameOverride, "KONYSHEVS-PC"),
                    new ChannelOption(ChannelOptions.MaxMessageLength, int.MaxValue),
                };

                serviceCollection.AddTransient(p => new Channel($"{config.Host.Hostname}:{config.Host.Port}", ssl, options));
            }
            else
            {
                serviceCollection.AddTransient(p => new Channel($"{config.Host.Hostname}:{config.Host.Port}", ChannelCredentials.Insecure));
            }

            try
            {
                var compileErrors = _cache.Compile(serviceCollection);
                errors.AddRange(compileErrors);
            }
            catch (Exception e)
            {
                errors.Add($"Ошибка регистрации клиентов Grpc. Ex:{e}");
            }

            return errors;
        }
    }
}

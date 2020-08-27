// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseGrpcServer.cs" company="Konyshev A.V., Stepanov N.O.">
//   Copyright (c) Konyshev A.V., Stepanov N.O.
// </copyright>
// <summary>
//   Реализация сервера Grpc, предоставляет управление инициализация/запуск/остановка.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Msc.Microservice.GrpcCore.Configuration;

namespace Msc.Microservice.Layer.GrpcServer
{
    /// <summary>
    /// Реализация сервера Grpc, предоставляет управление инициализация/запуск/остановка.
    /// </summary>
    public class BaseGrpcServer : IGrpcServer
    {
        private readonly GrpcConfig _config;
        private readonly ILogger<IGrpcServer> _logger;
        private readonly Guid _identity = Guid.NewGuid();

        private Server _server;

        /// <summary>
        /// Конструктор с параметрами.
        /// </summary>
        /// <param name="config">Конфигурация запуска.</param>
        /// <param name="logger">Логгер.</param>
        public BaseGrpcServer(IOptions<GrpcConfig> config, ILogger<IGrpcServer> logger)
        {
            var cfg = config ?? throw new ArgumentNullException(nameof(config));
            _config = cfg.Value ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public void Start(IEnumerable<ServerServiceDefinition> services)
        {
            _logger.LogTrace($"[Grpc Server Layer. {_identity}] Запускается Grpc Server с конфигурацией: Host:{_config.Host.Hostname}; Port: {_config.Host.Port}; SSL: {_config.Ssl != null}");

            var credentials = ServerCredentials.Insecure;
            if (_config.Ssl != null)
            {
                var cacert = File.ReadAllText(Path.Combine(_config.Ssl.Path, _config.Ssl.CaCert));
                var servercert = File.ReadAllText(Path.Combine(_config.Ssl.Path, _config.Ssl.CertificateChain));
                var serverkey = File.ReadAllText(Path.Combine(_config.Ssl.Path, _config.Ssl.PrivateKey));
                var keypair = new KeyCertificatePair(servercert, serverkey);
                credentials = new SslServerCredentials(
                    new List<KeyCertificatePair> { keypair },
                    cacert,
                    SslClientCertificateRequestType.DontRequest);
            }

            _server = new Server(new[] { new ChannelOption(ChannelOptions.SoReuseport, 0) })
            {
                Ports = { new ServerPort(_config.Host.Hostname, _config.Host.Port, credentials) },
            };

            foreach (var definition in services)
            {
                _server.Services.Add(definition);
                _logger.LogTrace($"[Grpc Server Layer. {_identity}] Добавляется конечная точка. {definition}");
            }

            _server.Start();
            _logger.LogTrace($"[Grpc Server Layer. {_identity}] Grpc Server запущен.");
        }

        /// <inheritdoc />
        public void Shutdown()
        {
            _server?.ShutdownAsync().RunSynchronously();
            _logger.LogTrace($"[Grpc Server Layer. {_identity}] Grpc Server остановлен.");
        }
    }
}

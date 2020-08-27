// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GrpcConfig.cs" company="Konyshev A.V., Stepanov N.O.">
//   Copyright (c) Konyshev A.V., Stepanov N.O.
// </copyright>
// <summary>
//   Конфигурация для Grpc-сервера. Настройки Endpoint.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;

namespace Msc.Microservice.GrpcCore.Configuration
{
    /// <summary>
    /// Конфигурация для Grpc-сервера.
    /// </summary>
    public class GrpcConfig
    {
        /// <summary>
        /// Настройки Endpoint.
        /// </summary>
        public GrpcHostConfig Host { get; set; }

        /// <summary>
        /// SSL credentials.
        /// </summary>
        public GrpcSslConfig Ssl { get; set; }

        /// <summary>
        /// Проверить конфигурацию.
        /// </summary>
        /// <returns>Список ошибок.</returns>
        public List<string> Validate()
        {
            var errors = new List<string>();

            if (Host == null)
            {
                errors.Add("Отсутствует секция GrpcServer.Host в конфигурационном файле");
            }
            else
            {
                if (string.IsNullOrEmpty(Host.Hostname))
                {
                    errors.Add("Отсутствует Hostname в секция GrpcServer.Host");
                }

                if (Host.Port == default)
                {
                    errors.Add("Отсутствует Port в секция GrpcServer.Host");
                }
            }

            if (Ssl == null)
            {
                return errors;
            }

            if (string.IsNullOrEmpty(Ssl.Path))
            {
                errors.Add("Отсутствует Path в секция GrpcServer.Ssl");
            }
            else
            {
                if (string.IsNullOrEmpty(Ssl.CaCert))
                {
                    errors.Add("Отсутствует CaCert в секция GrpcServer.Ssl");
                }
                else if (!File.Exists(Path.Combine(Ssl.Path, Ssl.CaCert)))
                {
                    errors.Add(
                        $"Не существует файла {Ssl.CaCert} по указанному пути {Ssl.Path}");
                }

                if (string.IsNullOrEmpty(Ssl.CertificateChain))
                {
                    errors.Add("Отсутствует CertificateChain в секция GrpcServer.Ssl");
                }
                else if (!File.Exists(Path.Combine(Ssl.Path, Ssl.CertificateChain)))
                {
                    errors.Add($"Не существует файла {Ssl.CertificateChain} по указанному пути {Ssl.Path}");
                }

                if (string.IsNullOrEmpty(Ssl.PrivateKey))
                {
                    errors.Add("Отсутствует PrivateKey в секция GrpcServer.Ssl");
                }
                else if (!File.Exists(Path.Combine(Ssl.Path, Ssl.PrivateKey)))
                {
                    errors.Add($"Не существует файла {Ssl.PrivateKey} по указанному пути {Ssl.Path}");
                }
            }

            return errors;
        }
    }
}
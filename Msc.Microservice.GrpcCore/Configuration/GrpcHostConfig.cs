// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GrpcHostConfig.cs" company="Konyshev A.V., Stepanov N.O.">
//   Copyright (c) Konyshev A.V., Stepanov N.O.
// </copyright>
// <summary>
//   Настройки Endpoint.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Msc.Microservice.GrpcCore.Configuration
{
    /// <summary>
    /// Настройки Endpoint.
    /// </summary>
    public class GrpcHostConfig
    {
        /// <summary>
        /// Имя хоста.
        /// </summary>
        public string Hostname { get; set; }

        /// <summary>
        /// Номер порта.
        /// </summary>
        public int Port { get; set; }
    }
}
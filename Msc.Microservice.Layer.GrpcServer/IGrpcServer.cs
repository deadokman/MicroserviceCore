// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IGrpcServer.cs" company="Konyshev A.V., Stepanov N.O.">
//   Copyright (c) Konyshev A.V., Stepanov N.O.
// </copyright>
// <summary>
//   Слой доступа к серверу, протокол взаимодействия Grpc
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using Grpc.Core;

namespace Msc.Microservice.Layer.GrpcServer
{
    /// <summary>
    /// Интерфейс сервера Grpc.
    /// </summary>
    public interface IGrpcServer
    {
        /// <summary>
        /// Запустить сервер.
        /// </summary>
        /// <param name="services">Коллекция определений сервисов.</param>
        void Start(IEnumerable<ServerServiceDefinition> services);

        /// <summary>
        /// Остановить сервер.
        /// </summary>
        void Shutdown();
    }
}
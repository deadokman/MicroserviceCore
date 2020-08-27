// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IClientService.cs" company="Konyshev A.V., Stepanov N.O.">
//   Copyright (c) Konyshev A.V., Stepanov N.O.
// </copyright>
// <summary>
//   Хранилище экземпляров клиентов.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using Grpc.Core;

namespace Msc.Microservice.Layer.GrpcClient
{
    /// <summary>
    /// Хранилище экземпляров клиентов.
    /// </summary>
    public interface IClientService
    {
        /// <summary>
        /// DI.
        /// </summary>
        IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// Получить экземпляр по типу.
        /// </summary>
        /// <typeparam name="T">Тип клиента.</typeparam>
        /// <returns>Экземпляр клиента.</returns>
        T GetInstance<T>()
            where T : ClientBase<T>;
    }
}
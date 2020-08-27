// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DoWorkArguments.cs" company="Konyshev A.V., Stepanov N.O.">
//   Copyright (c) Konyshev A.V., Stepanov N.O.
// </copyright>
// <summary>
//   Обобщенный контракт для запуска бэкграуд треда в микросрвисе.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Threading;

namespace Msc.Microservice.Core.Standalone.Implementations
{
    /// <summary>
    /// Обобщенный контракт для запуска бэкграуд треда в микросрвисе.
    /// </summary>
    public class DoWorkArguments
    {
        /// <summary>
        /// Провайдер микросервисов.
        /// </summary>
        public IServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// Токен синхронизации останова службы.
        /// </summary>
        public CancellationToken CancellationToken { get; set; }
    }
}

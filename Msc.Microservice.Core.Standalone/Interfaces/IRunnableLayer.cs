// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRunnableLayer.cs" company="Konyshev A.V., Stepanov N.O.">
//   Copyright (c) Konyshev A.V., Stepanov N.O.
// </copyright>
// <summary>
//   Представляет собой интерфейс слоя микросервиса, наделяющий его той или иной функциональностью
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Msc.Microservice.Core.Standalone.Interfaces
{
    /// <summary>
    /// Слой, который может выполнять некоторые действия.
    /// </summary>
    public interface IRunnableLayer : IMicroserviceLayer
    {
        /// <summary>
        /// Запустить выполннение операций в слое асинхронно.
        /// </summary>
        /// <param name="serviceProvider">Провайдер служб.</param>
        void RunAsync(IServiceProvider serviceProvider);

        /// <summary>
        /// Отключить работу службы.
        /// </summary>
        /// <param name="serviceProvider">Провайдер служб.</param>
        void Shutdown(IServiceProvider serviceProvider);
    }
}

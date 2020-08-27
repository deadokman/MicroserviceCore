// -------// -------------------------------------------------------------------------------------------------------------------
// <copyright file="PrepareExecutionDelegate.cs" company="Konyshev A.V., Stepanov N.O.">
//   Copyright (c) Konyshev A.V., Stepanov N.O.
// </copyright>
// <summary>
//   Делегат для конфигурирования микросервиса.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Msc.Microservice.Core.Standalone.Implementations
{
    /// <summary>
    /// Делегат для конфигурирования микросервиса.
    /// </summary>
    /// <param name="serviceCollection">Коллекция служб.</param>
    /// <param name="configuration">Конфиг.</param>
    public delegate void PrepareExecutionDelegate(IServiceCollection serviceCollection, IConfigurationRoot configuration);
}

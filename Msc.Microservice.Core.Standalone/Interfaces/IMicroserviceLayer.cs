// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMicroserviceLayer.cs" company="Konyshev A.V., Stepanov N.O.">
//   Copyright (c) Konyshev A.V., Stepanov N.O.
// </copyright>
// <summary>
//   Представляет собой интерфейс слоя микросервиса, наделяющий его той или иной функциональностью
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Msc.Microservice.Core.Standalone.Interfaces
{
    /// <summary>
    /// Представляет собой интерфейс слоя микросервиса, наделяющий его той или иной функциональностью.
    /// </summary>
    public interface IMicroserviceLayer
    {
        /// <summary>
        /// Выполнить регистрацию и валидацию конфигураций.
        /// </summary>
        /// <param name="configurationRoot">Валидация конфигураций.</param>
        void RegisterConfiguration(IConfigurationBuilder configurationRoot);

        /// <summary>
        /// Выполнить регистрацию и валидацию конфигураций.
        /// </summary>
        /// <param name="configurationRoot">Валидация конфигураций.</param>
        /// <param name="serviceCollection">Коллекция служб.</param>
        /// <returns>Список ошибок во время конфигурирования.</returns>
        IEnumerable<string> RegisterLayer(IConfigurationRoot configurationRoot, IServiceCollection serviceCollection);
    }
}

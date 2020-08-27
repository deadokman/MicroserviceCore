// ------------------------------------------------------------------------------------------------------------------
// <copyright file="MicroserviceLayerBase.cs" company="Konyshev A.V., Stepanov N.O.">
//   Copyright (c) Konyshev A.V., Stepanov N.O.
// </copyright>
// <summary>
// Базовый класс для слоя микросервиса.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Msc.Microservice.Core.Standalone.Interfaces;

namespace Msc.Microservice.Core.Standalone.Layering
{
    /// <summary>
    /// Базовый класс для слоя микросервиса.
    /// </summary>
    public abstract class MicroserviceLayerBase : IMicroserviceLayer
    {
        /// <summary>
        /// Зарегистрировать конфигурационный файл.
        /// </summary>
        /// <param name="configurationBuilder">Построитель конфигурации.</param>
        public abstract void RegisterConfiguration(IConfigurationBuilder configurationBuilder);

        /// <summary>
        /// Выполнить регистрацию и валидацию конфигураций.
        /// </summary>
        /// <param name="configurationRoot">Валидация конфигураций.</param>
        /// <param name="serviceCollection">Коллекция служб.</param>
        /// <returns>Список ошибок конфигурации.</returns>
        public abstract IEnumerable<string> RegisterLayer(IConfigurationRoot configurationRoot, IServiceCollection serviceCollection);
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMicroserviceCore.cs" company="Konyshev A.V., Stepanov N.O.">
//   Copyright (c) Konyshev A.V., Stepanov N.O.
// </copyright>
// <summary>
//   Интерфейс ядра микросервиса
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Msc.Microservice.Core.Standalone.Implementations;
using Msc.Microservice.Core.Standalone.Implementations;

namespace Msc.Microservice.Core.Standalone.Interfaces
{
    /// <summary>
    /// Интерфейс ядра микросервиса.
    /// </summary>
    public interface IMicroserviceCore
    {
        /// <summary>
        /// Остановить работу службы.
        /// </summary>
        void Stop();

        /// <summary>
        /// ЗАпустить службу асинхронно.
        /// </summary>
        /// <returns>Асинхронный запуск службы.</returns>
        Task RunAsync();

        /// <summary>
        /// Запустить службу на выполнение.
        /// </summary>
        void Run();

        /// <summary>
        /// Добавить слой.
        /// </summary>
        /// <param name="layer">Слой.</param>
        void AddLayer(IMicroserviceLayer layer);

        /// <summary>
        /// Начать выполнение задачи службой
        /// </summary>
        event DoWorkDelegate DoWork;

        /// <summary>
        /// Выполняется перед началом выполнения тела микросервиса
        /// </summary>
        event PrepareExecutionDelegate PrepareExecution;

        /// <summary>
        /// Нотификация о необработанной исключительной ситуации
        /// </summary>
        event EventHandler<UnhandledExceptionEventArgs> GotUnhandledException;

        /// <summary>
        /// Произвести действия перед остановкой службы
        /// </summary>
        event PrepareShutdownDelegate PrepareShutdown;

        /// <summary>
        /// Выполнить регистрацию конфигураций
        /// </summary>
        event EventHandler<IConfigurationBuilder> RegisterConfigurations;
    }
}

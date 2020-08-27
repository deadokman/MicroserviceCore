// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MicroserviceStartupException.cs" company="Konyshev A.V., Stepanov N.O.">
//   Copyright (c) Konyshev A.V., Stepanov N.O.
// </copyright>
// <summary>
//   Обобщенный контракт для запуска бэкграуд треда в микросрвисе
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Msc.Microservice.Core.Standalone.Exceptions
{
    /// <summary>
    /// Ошибка выбрасываемая в момент старта и конфигурации миеросервиса.
    /// </summary>
    public class MicroserviceStartupException : Exception
    {
        /// <summary>
        /// Конструктор ошибки инициализации службы.
        /// </summary>
        /// <param name="message">Сообщение об ошибке.</param>
        public MicroserviceStartupException(string message)
            : base(message)
        {
        }
    }
}

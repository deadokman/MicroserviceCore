// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResponseType.cs" company="Konyshev A.V., Stepanov N.O.">
//   Copyright (c) Konyshev A.V., Stepanov N.O.
// </copyright>
// <summary>
//  Тип ответа (успешно или ошибка)
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Msc.Microservice.Layer.RabbitMq.Rpc
{
    /// <summary>
    /// Тип ответа (успешно или ошибка).
    /// </summary>
    public enum ResponseType
    {
        /// <summary>
        /// Выполнено успешно
        /// </summary>
        Ok = 0,

        /// <summary>
        /// Выполнено с ошибками
        /// </summary>
        Error = 1,
    }
}

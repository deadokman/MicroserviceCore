// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResponseType.cs" company="ООО ИТСК">
//   Copyright (c) ООО ИТСК. All rights reserved.
// </copyright>
// <summary>
//  Тип ответа (успешно или ошибка)
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Itsk.Microservice.Layer.RabbitMq.Rpc
{
    /// <summary>
    /// Тип ответа (успешно или ошибка)
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

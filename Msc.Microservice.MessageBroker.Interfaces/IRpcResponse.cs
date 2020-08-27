// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRpcResponse.cs" company="ООО ИТСК">
//   Copyright (c) ООО ИТСК. All rights reserved.
// </copyright>
// <summary>
//  Интерфейс ответа
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Itsk.Microservice.Layer.RabbitMq.Rpc;

namespace Itsk.Microservice.Layer.RabbitMq.Interfaces
{
    /// <summary>
    /// Интерфейс ответа
    /// </summary>
    /// <typeparam name="TResponse">Тип ответа</typeparam>
    public interface IRpcResponse<TResponse>
    {
        /// <summary>
        /// Тип ответа (ошибка или успех)
        /// </summary>
        ResponseType ResponseType { get; }

        /// <summary>
        /// Ответ на запрос
        /// </summary>
        TResponse Response { get; set; }

        /// <summary>
        /// Сообщение
        /// </summary>
        string Message { get; }
    }
}

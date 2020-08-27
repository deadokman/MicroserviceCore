// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRpcResponse.cs" company="Konyshev A.V., Stepanov N.O.">
//   Copyright (c) Konyshev A.V., Stepanov N.O.
// </copyright>
// <summary>
//  Интерфейс ответа
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Msc.Microservice.Layer.RabbitMq.Rpc;

namespace Msc.Microservice.Layer.RabbitMq.Interfaces
{
    /// <summary>
    /// Интерфейс ответа.
    /// </summary>
    /// <typeparam name="TResponse">Тип ответа.</typeparam>
    public interface IRpcResponse<TResponse>
    {
        /// <summary>
        /// Тип ответа (ошибка или успех).
        /// </summary>
        ResponseType ResponseType { get; }

        /// <summary>
        /// Ответ на запрос.
        /// </summary>
        TResponse Response { get; set; }

        /// <summary>
        /// Сообщение.
        /// </summary>
        string Message { get; }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RpcResponse.cs" company="ООО ИТСК">
//   Copyright (c) ООО ИТСК. All rights reserved.
// </copyright>
// <summary>
//   RPC ответ
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Itsk.Microservice.Layer.RabbitMq.Interfaces;

namespace Itsk.Microservice.Layer.RabbitMq.Rpc
{
    /// <summary>
    /// RPC ответ
    /// </summary>
    public class RpcResponse : IRpcResponse<object>
    {
        /// <summary>
        /// RPC ответ
        /// </summary>
        /// <param name="responseType">Тип ответа (ошибка или данные)</param>
        /// <param name="response">Объект ответа</param>
        /// <param name="message">Сообщение</param>
        public RpcResponse(ResponseType responseType, object response, string message)
        {
            ResponseType = responseType;
            Response = response;
            Message = message;
        }

        /// <summary>
        /// Тип RPC ответа
        /// </summary>
        public ResponseType ResponseType { get; private set; }

        /// <summary>
        /// RPC ответ
        /// </summary>
        public object Response { get; set; }

        /// <summary>
        /// Сообщение
        /// </summary>
        public string Message { get; private set; }
    }
}

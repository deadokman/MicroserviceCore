// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRpcMessageHandler.cs" company="Konyshev A.V., Stepanov N.O.">
//   Copyright (c) Konyshev A.V., Stepanov N.O.
// </copyright>
// <summary>
//   Обработчик сообщений RPC
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Msc.Microservice.Layer.RabbitMq.Interfaces
{
    /// <summary>
    /// Обработчик сообщений типа RPC.
    /// </summary>
    /// <typeparam name="TArg">Тип входного аргумента.</typeparam>
    /// <typeparam name="TResp">Тип ответа.</typeparam>
    public interface IRpcMessageHandler<TArg, TResp>
    {
        /// <summary>
        /// Выполнить обработку RPC запроса.
        /// </summary>
        /// <param name="arg"> Параметр запроса. </param>
        /// <returns> Тип ответа. </returns>
        TResp HandleRpc(IRmqMessageWrap<TArg> arg);
    }
}

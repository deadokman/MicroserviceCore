// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RepublishMessageBehaviour.cs" company="Konyshev A.V., Stepanov N.O.">
//   Copyright (c) Konyshev A.V., Stepanov N.O.
// </copyright>
// <summary>
//   Поведение для переотправки недоставленного сообщения в другую очередь
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Msc.Microservice.Layer.RabbitMq.Behaviour
{
    /// <summary>
    /// Поведенческая модель для определения действия клиента при возникновении ошибок обработки сообшения.
    /// </summary>
    public class RepublishMessageBehaviour : BehaviourBase
    {
        /// <summary>
        /// Целевая очередь для переотправки сообщения.
        /// </summary>
        public string TargetEndpoint { get; set; }

        /// <summary>
        /// Вызвать отработку поведенческой модели.
        /// </summary>
        /// <param name="messageBytes">Набор байт сообщения.</param>
        public override void InvokeBehaviour(ReadOnlyMemory<byte> messageBytes)
        {
            if (QueueClient != null)
            {
                QueueClient.SendBytes(TargetEndpoint, messageBytes);
            }
        }
    }
}

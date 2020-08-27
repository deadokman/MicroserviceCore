// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OwMessageCommon.cs" company="Konyshev A.V., Stepanov N.O.">
//   Copyright (c) Konyshev A.V., Stepanov N.O.
// </copyright>
// <summary>
//  Общий класс сообщения OW
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Msc.Microservice.Layer.RabbitMq
{
    /// <summary>
    /// Общий класс сообщения OW.
    /// </summary>
    public class OwMessageCommon
    {
        /// <summary>
        /// Конструктор класса.
        /// </summary>
        /// <param name="customMessageType">Кастомный тип сообщения.</param>
        public OwMessageCommon(string customMessageType)
        {
            MessageType = customMessageType;
        }

        /// <summary>
        /// Установить тип сообщения.
        /// </summary>
        /// <param name="type">Тип сообщения.</param>
        public void SetMessageType(Type type)
        {
            MessageType = type.AssemblyQualifiedName;
        }

        /// <summary>
        /// Установить кастомное имя контаркта.
        /// </summary>
        /// <param name="customMessageType">Кастомное имя контракта.</param>
        public void SetCustomMessageType(string customMessageType)
        {
            MessageType = customMessageType;
        }

        /// <summary>
        /// Добавить полезную нагрузку сообщения.
        /// </summary>
        /// <param name="payload">Полезная нагрузка.</param>
        public void SetPayload(string payload)
        {
            Payload = payload;
        }

        /// <summary>
        /// Заголовок типа сообщения.
        /// </summary>
        public string MessageType { get; private set; }

        /// <summary>
        /// Полезная нагрузка сообщения.
        /// </summary>
        public string Payload { get; private set; }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RabbitContractAttribute.cs" company="Konyshev A.V., Stepanov N.O.">
//   Copyright (c) Konyshev A.V., Stepanov N.O.
// </copyright>
// <summary>
//   Реализация клиента RabbitMQ для ед. окна
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Msc.Microservice.Layer.RabbitMq
{
    /// <summary>
    /// Аттрибут используется для декларации контракта для RabbitMq
    /// Не является обязательным аттнрибутом. Используется при необходимости кроссплатформенной передачи контрактов внутри шины данных.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class RabbitContractAttribute : Attribute
    {
        /// <summary>
        /// Основной конструктор тиап.
        /// </summary>
        /// <param name="alias">Алиас контракта.</param>
        public RabbitContractAttribute(string @alias)
        {
            Alias = alias;
        }

        /// <summary>
        /// Возвращает алиас контракта.
        /// </summary>
        public string Alias { get; private set; }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeNameFromFullQualified.cs" company="Konyshev A.V., Stepanov N.O.">
//   Copyright (c) Konyshev A.V., Stepanov N.O.
// </copyright>
// <summary>
//  Методы расширений для RabbitMq
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Msc.Microservice.Layer.RabbitMq
{
    /// <summary>
    /// Класс парсинга названия типов.
    /// </summary>
    public class TypeNameFromFullQualified
    {
        /// <summary>
        /// Получить экземпляр Typename.
        /// </summary>
        /// <param name="typeFullQualified">Имя типа.</param>
        /// <returns>Вернуть представление типа по имени.</returns>
        public static TypeNameFromFullQualified GetTypeName(string typeFullQualified)
        {
            return new TypeNameFromFullQualified(typeFullQualified);
        }

        /// <summary>
        /// Класс парсинга названия типов.
        /// </summary>
        /// <param name="name">Имя таблицы.</param>
        private TypeNameFromFullQualified(string name)
        {
            var pairs = name.Split(',');
            if (pairs.Length != 5)
            {
                throw new Exception($"{name} is not full Qualified");
            }

            TypeName = pairs[0].Trim();
            AssemblyName = pairs[1].Trim();
            PublicToken = pairs[2].Trim();
            TypeNameUnqualified = $"{TypeName}, {AssemblyName}";
        }

        /// <summary>
        /// Имя типа.
        /// </summary>
        public string TypeName { get; private set; }

        /// <summary>
        /// Сборка.
        /// </summary>
        public string AssemblyName { get; private set; }

        /// <summary>
        /// Сборка.
        /// </summary>
        public string PublicToken { get; private set; }

        /// <summary>
        /// Не полное название типа.
        /// </summary>
        public string TypeNameUnqualified { get; private set; }
    }
}

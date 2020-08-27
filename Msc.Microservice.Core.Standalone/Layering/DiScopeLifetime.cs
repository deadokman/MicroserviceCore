// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiScopeLifetime.cs" company="Konyshev A.V., Stepanov N.O.">
//   Copyright (c) Konyshev A.V., Stepanov N.O.
// </copyright>
// <summary>
// Переисление.Уровень жизни типа в DI контейнере.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Msc.Microservice.Core.Standalone.Layering
{
    /// <summary>
    /// Переисление. Уровень жизни типа в DI контейнере.
    /// </summary>
    public enum DiScopeLifetime
    {
        /// <summary>
        /// Новый экземпляр на каждый инстанс
        /// </summary>
        Tranisent,

        /// <summary>
        /// Один экземлпяр в рамках области видимости вызова
        /// </summary>
        Scoped,

        /// <summary>
        /// Один экземпляр
        /// </summary>
        SingleInstance,
    }
}

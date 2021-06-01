// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IExtendDistributedCache.cs" company="Konyshev A.V., Stepanov N.O.">
//   Copyright (c) Konyshev A.V., Stepanov N.O.
// </copyright>
// <summary>
//   Интерфейс расширение для кеша Redis
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Extensions.Caching.Distributed;

namespace Msc.Microservice.Layer.Redis
{
    /// <summary>
    /// Интерфейс расширение для кеша Redis
    /// </summary>
    public interface IExtendDistributedCache : IDistributedCache
    {
        /// <summary>
        /// Установить начальное значение инкремента
        /// </summary>
        /// <param name="key">Название ключа</param>
        /// <param name="val">Начальное значение ключа</param>
        void ResetIncr(string key, long val = 0);

        /// <summary>
        /// Получить текущее значение инкремента
        /// </summary>
        /// <param name="key">Название ключа</param>
        /// <returns>Значение</returns>
        long GetIncr(string key);

        /// <summary>
        /// Увеличить инкремент на заданное значение
        /// </summary>
        /// <param name="key">Название ключа</param>
        /// <param name="val">Начальное значение ключа</param>
        /// <returns>Результат изменения</returns>
        long SetIncr(string key, long val = 1);
    }
}
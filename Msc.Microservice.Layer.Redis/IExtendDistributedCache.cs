// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IExtendDistributedCache.cs" company="Konyshev A.V., Stepanov N.O.">
//   Copyright (c) Konyshev A.V., Stepanov N.O.
// </copyright>
// <summary>
//   Интерфейс расширение для кеша Redis
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

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

        /// <summary>
        /// Hash set
        /// </summary>
        /// <param name="hashKey">Hash key</param>
        /// <param name="values">Values</param>
        /// <returns> representing the asynchronous operation.</returns>
        Task SetHValuesAsync(string hashKey, params (string key, string value)[] values);

        /// <summary>
        /// Получить все значения из хэша
        /// </summary>
        /// <param name="hashKey">Ключ хэша=бакета</param>
        /// <returns>пары ключ-значение</returns>
        Task<(string key, string value)[]> GetAllHValuesAsync(string hashKey);

        /// <summary>
        /// Установить значение хэш-сета
        /// </summary>
        /// <param name="hashKey">Ключ хэш сета</param>
        /// <param name="key">Ключ данных</param>
        /// <param name="value">Ключ значения</param>
        /// <param name="hashSetKeyLifetime">Время жизни ключа хэш-сета</param>
        /// <returns>Асинхронная операция</returns>
        Task SetHValueAsync(string hashKey, string key, string value, TimeSpan? hashSetKeyLifetime = null);

        /// <summary>
        /// Получить значение для ключа из хэша
        /// </summary>
        /// <param name="hashKey">Ключ хэша</param>
        /// <param name="hashValueName">Имя значения внутри хэша</param>
        /// <returns>Получить </returns>
        Task<string> GetHValueAsync(string hashKey, string hashValueName);

        /// <summary>
        /// Удалить хэш
        /// </summary>
        /// <param name="hashKey">Хэш-ключ</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task DelHAsync(string hashKey);
    }
}
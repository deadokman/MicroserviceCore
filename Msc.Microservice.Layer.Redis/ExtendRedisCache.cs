// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtendRedisCache.cs" company="Konyshev A.V., Stepanov N.O.">
//   Copyright (c) Konyshev A.V., Stepanov N.O.
// </copyright>
// <summary>
//   Класс расширение для кеша Redis
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Msc.Microservice.Layer.Redis
{
    /// <summary>
    /// Класс расширение для кеша Redis
    /// </summary>
    public class ExtendRedisCache : RedisCache, IExtendDistributedCache
    {
        private readonly string _instance;

        /// <summary>
        /// Конструктор с параметрами
        /// </summary>
        /// <param name="optionsAccessor">Опции подключения</param>
        public ExtendRedisCache(IOptions<RedisCacheOptions> optionsAccessor)
            : base(optionsAccessor)
        {
            _instance = optionsAccessor.Value.InstanceName ?? string.Empty;
        }

        /// <summary>
        /// Установить начальное значение инкремента
        /// </summary>
        /// <param name="key">Название ключа</param>
        /// <param name="val">Начальное значение ключа</param>
        public void ResetIncr(string key, long val = 0)
        {
            var cache = GetCache();
            cache.StringSet($"{_instance}{key}", val);
        }

        /// <summary>
        /// Hash set
        /// </summary>
        /// <param name="hashKey">Hash key</param>
        /// <param name="values">Values</param>
        /// <returns> representing the asynchronous operation.</returns>
        public async Task SetHValuesAsync(string hashKey, params (string key, string value)[] values)
        {
            var cache = GetCache();
            await cache.HashSetAsync(hashKey, values.Select(v => new HashEntry(v.key, v.value)).ToArray());
        }

        /// <summary>
        /// Hash set
        /// </summary>
        /// <param name="hashKey">Hash key</param>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="hashSetKeyLifetime">Время жизни для всего хэш сета</param>
        /// <returns> representing the asynchronous operation.</returns>
        public async Task SetHValueAsync(string hashKey, string key, string value, TimeSpan? hashSetKeyLifetime = null)
        {
            var cache = GetCache();
            await cache.HashSetAsync(hashKey, key, value, When.Always);

            if (hashSetKeyLifetime.HasValue)
            {
                cache.KeyExpire(hashKey, hashSetKeyLifetime);
            }
        }

        /// <summary>
        /// Получить все значения из хэша
        /// </summary>
        /// <param name="hashKey">Ключ хэша=бакета</param>
        /// <returns>пары ключ-значение</returns>
        public async Task<(string key, string value)[]> GetAllHValuesAsync(string hashKey)
        {
            var cache = GetCache();
            var vals = await cache.HashGetAllAsync(hashKey);
            return vals.Select(v => (key: v.Name.ToString(), value: v.Value.ToString())).ToArray();
        }

        /// <summary>
        /// Получить значение для ключа из хэша
        /// </summary>
        /// <param name="hashKey">Ключ хэша</param>
        /// <param name="hashValueName">Имя значения внутри хэша</param>
        /// <returns>Получить </returns>
        public async Task<string> GetHValueAsync(string hashKey, string hashValueName)
        {
            var cache = GetCache();
            var vals = await cache.HashGetAsync(hashKey, hashValueName);
            return vals.ToString();
        }

        /// <summary>
        /// Удалить хэш
        /// </summary>
        /// <param name="hashKey">Хэш-ключ</param>
        /// <returns>representing the asynchronous operation.</returns>
        public async Task DelHAsync(string hashKey)
       {
            var cache = GetCache();
            await cache.KeyDeleteAsync(hashKey);
        }

        /// <summary>
        /// Получить текущее значение инкремента
        /// </summary>
        /// <param name="key">Название ключа</param>
        /// <returns>Значение</returns>
        public long GetIncr(string key)
        {
            var cache = GetCache();
            var keyValue = cache.StringGet($"{_instance}{key}");
            return (long)keyValue;
        }

        /// <summary>
        /// Увеличить инкремент на заданное значение
        /// </summary>
        /// <param name="key">Название ключа</param>
        /// <param name="val">Начальное значение ключа</param>
        /// <returns>Результат изменения</returns>
        public long SetIncr(string key, long val = 1)
        {
            var cache = GetCache();
            return cache.StringIncrement($"{_instance}{key}", val);
        }

        private void InternalConnect()
        {
            var connectMethod = typeof(RedisCache).GetMethod("Connect", BindingFlags.NonPublic | BindingFlags.Instance);
            if (connectMethod == null)
            {
                throw new NullReferenceException("Connect");
            }

            connectMethod.Invoke(this, null);
        }

        private IDatabase GetCache()
        {
            InternalConnect();
            var cacheField = typeof(RedisCache).GetField("_cache", BindingFlags.NonPublic | BindingFlags.Instance);
            if (cacheField == null)
            {
                throw new NullReferenceException("Connect");
            }

            return (IDatabase)cacheField.GetValue(this);
        }
    }
}
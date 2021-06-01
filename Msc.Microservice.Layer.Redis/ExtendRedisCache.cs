// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtendRedisCache.cs" company="Konyshev A.V., Stepanov N.O.">
//   Copyright (c) Konyshev A.V., Stepanov N.O.
// </copyright>
// <summary>
//   Класс расширение для кеша Redis
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Reflection;
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
// <copyright file="ClientCache.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Msc.Microservice.Layer.GrpcClient
{
    /// <summary>
    /// Регистрация клиентов Grpc для регистрации в ClientLayer.
    /// </summary>
    public class ClientCache : IClientService
    {
        /// <summary>
        /// Экземпляр класса.
        /// </summary>
        private static ClientCache _instance;

        /// <summary>
        /// Статический экземпляр класса.
        /// </summary>
        public static ClientCache Instance => _instance ??= new ClientCache();

        /// <summary>
        /// Коллекция типов, которые необходимо зарегистрировать.
        /// </summary>
        public HashSet<Type> Types { get; } = new HashSet<Type>();

        /// <summary>
        /// DI.
        /// </summary>
        public IServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// Зарегистрировать тип клиента.
        /// </summary>
        /// <typeparam name="T">Тип сгенерированного клиента Grpc.</typeparam>
        public void Add<T>()
            where T : ClientBase<T>
        {
            Types.Add(typeof(T));
        }

        /// <summary>
        /// Скомпилировать и зарегистрировать клиенты в DI.
        /// </summary>
        /// <param name="serviceCollection">Коллекция служб.</param>
        /// <returns>Список ошибок конфигурации.</returns>
        public List<string> Compile(IServiceCollection serviceCollection)
        {
            var errors = new List<string>();
            foreach (var clientType in Types)
            {
                var channelType = typeof(ChannelBase);
                var ctorInfo = clientType.GetConstructor(new[] { channelType });
                if (ctorInfo != null)
                {
                    // Вариант без компиляции
                    // serviceCollection.AddTransient(p => (T)ctorInfo.Invoke(new object[] { p.GetService<Channel>() }));
                    var funcDelegate = typeof(Func<,>).MakeGenericType(channelType, clientType);
                    var channelParameter = Expression.Parameter(channelType);
                    var ctor = Expression.New(ctorInfo, channelParameter);

                    var memberInit = Expression.MemberInit(ctor);
                    var lambdaExpression = Expression.Lambda(funcDelegate, memberInit, channelParameter);
                    var compiledInvoke = lambdaExpression.Compile();

                    serviceCollection.AddTransient(clientType, p => compiledInvoke.DynamicInvoke(p.GetService<Channel>()));
                }
                else
                {
                    errors.Add("Не найден конструктор для создания клиента Grpc");
                }
            }

            serviceCollection.AddSingleton<IClientService>(p =>
            {
                ServiceProvider = p;
                return this;
            });

            return errors;
        }

        /// <summary>
        /// Получить экземпляр по типу.
        /// </summary>
        /// <typeparam name="T">Тип клиента.</typeparam>
        /// <returns>Экземпляр клиента.</returns>
        public T GetInstance<T>()
            where T : ClientBase<T>
        {
            var clientType = typeof(T);
            if (Types.Any(x => x == clientType))
            {
                return (T)ServiceProvider.GetService(clientType);
            }

            return null;
        }
    }
}
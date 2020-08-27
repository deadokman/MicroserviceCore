// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageDispatcher.cs" company="Konyshev A.V., Stepanov N.O.">
//   Copyright (c) Konyshev A.V., Stepanov N.O.
// </copyright>
// <summary>
//   Выполняет распределение сообщений по хендлерам
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using Msc.Microservice.Layer.RabbitMq.Interfaces;

namespace Msc.Microservice.Layer.RabbitMq.Dispatcher
{
    /// <summary>
    /// Выполняет распределение сообщений по хендлерам.
    /// </summary>
    public class MessageDispatcher : IMsgDispatcher
    {
        private readonly Dictionary<string, Type> _commonContractTypes;

        private readonly Dictionary<Type, List<Func<IMessageQueueClient, object, object>>> _dispatcherInvocators;

        private readonly Dictionary<BigInteger, Tuple<Type, Func<object, object>>> _rpcHandlers;

        private readonly ILogger _logger;

        private MD5 _md5;

        private Dictionary<Type, Func<object, Acknoledge, IMessageQueueClient, object>> _messageWrapperInitiators;

        /// <summary>
        /// Конструктор класса.
        /// </summary>
        /// <param name="logger">Логгер.</param>
        /// <param name="handlers">Массив хендлеров сообщений.</param>
        public MessageDispatcher(ILogger<IMsgDispatcher> logger, object[] handlers)
        {
            if (handlers == null)
            {
                throw new ArgumentNullException(nameof(handlers));
            }

            _logger = logger;
            _md5 = MD5.Create();
            _commonContractTypes = new Dictionary<string, Type>();
            _dispatcherInvocators = BuildHandlers(handlers, out _rpcHandlers);

            _messageWrapperInitiators = new Dictionary<Type, Func<object, Acknoledge, IMessageQueueClient, object>>();

            // Построить компиллированные лямбды для создания оберток сообщений
            BuildWrapperInitiators();
        }

        /// <summary>
        /// Попытаться апоулчить тип по алиасу.
        /// </summary>
        /// <param name="alias">Алиас.</param>
        /// <returns>Возвращаемый тип.</returns>
        public Type TryGetByAlias(string alias)
        {
            if (_commonContractTypes.TryGetValue(alias, out var type))
            {
                return type;
            }

            return default;
        }

        /// <summary>
        /// Выполинть обработку сообщения очереди через диспетчер.
        /// </summary>
        /// <param name="client">Клиент очереди.</param>
        /// <param name="acknowledge">Делегат, отвечающий за ручное подтверждение или откат сообщения.</param>
        /// <param name="message">Сообщение.</param>
        /// <param name="payloadType">Тип сообщения.</param>
        public void DispatchMessage(IMessageQueueClient client, Acknoledge acknowledge, object message, Type payloadType)
        {
            if (_dispatcherInvocators != null && _dispatcherInvocators.TryGetValue(payloadType, out var invokers))
            {
                if (_messageWrapperInitiators.TryGetValue(payloadType, out var messageWrapperInitiarot))
                {
                    var wrapperObject = messageWrapperInitiarot.Invoke(message, acknowledge, client);
                    foreach (var invoke in invokers)
                    {
                        invoke.Invoke(client, wrapperObject);
                    }
                }
                else
                {
                    throw new KeyNotFoundException($"Не найдено ни одного инициализатора обертки сообщения для типа: {payloadType}");
                }
            }
            else
            {
                throw new KeyNotFoundException($"Не найдено ни одного обработчика для входного типа сообщения {payloadType}");
            }
        }

        /// <summary>
        /// Обработать RPC запрос.
        /// </summary>
        /// <param name="client">Клиент rmq.</param>
        /// <param name="message">Сообщение.</param>
        /// <param name="acknowledge">Делегат ручного подтверждения или отката обработки сообщения.</param>
        /// <param name="argumentType">Тип входного аргумента.</param>
        /// <param name="responseType">Тип ответа.</param>
        /// <returns>Возвращает результат RPC запроса.</returns>
        public object HandleRpcMessage(IMessageQueueClient client, object message, Acknoledge acknowledge, Type argumentType, Type responseType)
        {
            var hash = new BigInteger(_md5.ComputeHash(Encoding.UTF8.GetBytes(argumentType.AssemblyQualifiedName + responseType.AssemblyQualifiedName)));
            if (_rpcHandlers.TryGetValue(hash, out var invoke))
            {
                if (_messageWrapperInitiators.TryGetValue(argumentType, out var messageWrapperInitiarot))
                {
                    var wrapperObject = messageWrapperInitiarot.Invoke(message, acknowledge, client);
                    return invoke.Item2.Invoke(wrapperObject);
                }
                else
                {
                    throw new KeyNotFoundException($"Не найдено ни одного инициализатора обертки сообщения для типа: {argumentType}");
                }
            }

            throw new NotSupportedException($"Не поддерживается обработка комбинации типов входного: {argumentType.AssemblyQualifiedName} и выходного: {responseType.AssemblyQualifiedName}");
        }

        /// <summary>
        /// Подготовить динамические инвокаторы для оберток сообщений.
        /// </summary>
        private void BuildWrapperInitiators()
        {
            // Построить инициаторы для всех типов содержащиъся в хендлере, без учета RPС или обычный тип
            foreach (var pairType in _dispatcherInvocators)
            {
                if (!_messageWrapperInitiators.ContainsKey(pairType.Key))
                {
                    var typeCreator = CreateDynamicClassInitiator(pairType.Key);
                    _messageWrapperInitiators[pairType.Key] = typeCreator;
                }
            }

            // Построить инициаторы для всех типов содержащиъся в хендлере, без учета RPС или обычный тип
            foreach (var hndlrType in _rpcHandlers)
            {
                var arg = hndlrType.Value.Item1;
                if (arg != null && !_messageWrapperInitiators.ContainsKey(arg))
                {
                    var typeCreator = CreateDynamicClassInitiator(arg);
                    _messageWrapperInitiators[arg] = typeCreator;
                }
            }
        }

        private Func<object, Acknoledge, IMessageQueueClient, object> CreateDynamicClassInitiator(Type payloadType)
        {
            var createdType = typeof(RmqMessageBase<>).MakeGenericType(payloadType);
            var delegateType = typeof(Acknoledge);
            var messageQueueClientType = typeof(IMessageQueueClient);
            var funcDelegate = typeof(Func<,,,>).MakeGenericType(payloadType, delegateType, messageQueueClientType, createdType);

            // Создать параметры для передачи в класс
            var displayPayloadValueParam = Expression.Parameter(payloadType, "payload");
            var displayAcknoledgeValueParam = Expression.Parameter(delegateType, "acknoledge");
            var displaymqClientValueParam = Expression.Parameter(messageQueueClientType, "client");

            var ctor = Expression.New(createdType);
            var payloadProperty = createdType.GetProperty("Payload");
            var acknoledgeProperty = createdType.GetProperty("Acknoledge");
            var mqclientProperty = createdType.GetProperty("Client");
            var binds = new[]
            {
                Expression.Bind(payloadProperty, displayPayloadValueParam),
                Expression.Bind(acknoledgeProperty, displayAcknoledgeValueParam),
                Expression.Bind(mqclientProperty, displaymqClientValueParam),
            };

            var memberInit = Expression.MemberInit(ctor, binds);
            var compiledInvoke = Expression.Lambda(funcDelegate, memberInit, displayPayloadValueParam, displayAcknoledgeValueParam, displaymqClientValueParam).Compile();
            Func<object, Acknoledge, IMessageQueueClient, object> createInvoke = (o, a, c) =>
            {
                return compiledInvoke.DynamicInvoke(o, a, c);
            };

            return createInvoke;
        }

        /// <summary>
        /// Построить обычные хендлеры сообщений.
        /// </summary>
        /// <param name="handlers">Коллекция обработчиков.</param>
        /// <param name="rpcHandlers">Хендлеры RPC.</param>
        /// <returns>Справочник хендлеров для TransferMessage.</returns>
        private Dictionary<Type, List<Func<IMessageQueueClient, object, object>>> BuildHandlers(object[] handlers, out Dictionary<BigInteger, Tuple<Type, Func<object, object>>> rpcHandlers)
        {
            var res = new Dictionary<Type, List<Func<IMessageQueueClient, object, object>>>();
            rpcHandlers = new Dictionary<BigInteger, Tuple<Type, Func<object, object>>>();
            var transferMessageInterface = typeof(IMessageHandler<>);
            var rpcInterface = typeof(IRpcMessageHandler<,>);
            foreach (var handler in handlers)
            {
                var handlerType = handler.GetType();
                var handlerInterfaces = handlerType.GetInterfaces();
                foreach (var handlerInterface in handlerInterfaces)
                {
                    if (!handlerInterface.IsGenericType)
                    {
                        continue;
                    }

                    var genericHandlerInterface = handlerInterface.GetGenericTypeDefinition();

                    // Если интерфейс обычного сообщения
                    if (genericHandlerInterface == transferMessageInterface)
                    {
                        var messageType = handlerInterface.GetGenericArguments().First();

                        // Заполнить справочник из аттрибутов контракта, если имеются.
                        var attribute = messageType.GetCustomAttributes<RabbitContractAttribute>().FirstOrDefault();
                        if (attribute != null)
                        {
                            if (_commonContractTypes.TryGetValue(attribute.Alias, out var existingType)
                                && existingType != messageType)
                            {
                                throw new ArgumentException($"Алиас {attribute.Alias} задан для двух разных контрактов {existingType} и {messageType}, алиас должен быть уникален для контракта");
                            }

                            _commonContractTypes.Add(attribute.Alias, messageType);
                        }

                        if (!res.TryGetValue(messageType, out var innerList))
                        {
                            innerList = new List<Func<IMessageQueueClient, object, object>>();
                            res.Add(messageType, innerList);
                        }

                        var methodInfo = handlerInterface.GetMethod("HandleMessage");
                        innerList.Add((c, o) =>
                        {
                            try
                            {
                                return methodInfo.Invoke(handler, new[] { o });
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError($"Ошибка во время обработки сообщения {messageType} диспетчером {handler} \n {ex}. ");
                                throw;
                            }
                        });
                    }
                    else if (genericHandlerInterface == rpcInterface)
                    {
                        // Сформировать хэшкод аргументов
                        var messageTypes = handlerInterface.GetGenericArguments();
                        var hash = new BigInteger(_md5.ComputeHash(Encoding.UTF8.GetBytes(messageTypes.Select(v => v.AssemblyQualifiedName).Aggregate(string.Empty, (a, b) => a + b))));
                        if (rpcHandlers.ContainsKey(hash))
                        {
                            throw new Exception($"Не удалось добавить обработчик RPC вызова: [{handler}], т.к. обработчик для указанного входного и выходного типа уже был добавлен ранее.");
                        }

                        var methodInfo = handlerInterface.GetMethod("HandleRpc");
                        rpcHandlers.Add(hash, new Tuple<Type, Func<object, object>>(messageTypes.First(), (o) =>
                        {
                            try
                            {
                                return methodInfo.Invoke(handler, new[] { o });
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError($"Ошибка в обработчике {handler} \n {ex}");
                                throw;
                            }
                        }));
                    }
                }
            }

            return res;
        }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RabbitMqClient.cs" company="Konyshev A.V., Stepanov N.O.">
//   Copyright (c) Konyshev A.V., Stepanov N.O.
// </copyright>
// <summary>
//   Реализация клиента RabbitMQ для ед. окна
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Msc.Microservice.Layer.RabbitMq.Behaviour;
using Msc.Microservice.Layer.RabbitMq.Configuration;
using Msc.Microservice.Layer.RabbitMq.Interfaces;
using Msc.Microservice.Layer.RabbitMq.Rpc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Msc.Microservice.Layer.RabbitMq
{
    /// <summary>
    /// Клиент RabbitMQ.
    /// </summary>
    public class RabbitMqClient : RabbitClientBase, IMessageQueueClient
    {
        /// <summary>
        /// Метка RPC запроса в шапке сообщения.
        /// </summary>
        public static readonly string RpcRequestHeaderName = "RpcRequestMessage";

        /// <summary>
        /// Метка RPC ответа в шапке сообщения.
        /// </summary>
        public static readonly string RpcResponseHeaderName = "RpcResponseMessage";

        /// <summary>
        /// Метка RPC запроса в шапке сообщения.
        /// </summary>
        public static readonly string RpcArgType = "RpcArgType";

        /// <summary>
        /// Метка типа RPC запроса в шапке сообщения.
        /// </summary>
        public static readonly string RpcRespType = "RpcRespType";

        private readonly object _lockDictionary = new object();

        /// <summary>
        /// Список запросов, ожидающие ответа.
        /// </summary>
        private readonly Dictionary<string, RpcResposeWaitHandle> _rpcWaits;

        private readonly Dictionary<Type, string> _typeAliasCache;

        /// <summary>
        /// Наименование очереди для ответов по RPC.
        /// </summary>
        private string _rpcCallbackQueue;

        private IModel _rpcModel;

        private EventingBasicConsumer _rpcConsumer;

        /// <summary>
        /// Клиент RabbitMq.
        /// </summary>
        /// <param name="config"> Конфигурация клиента. </param>
        /// <param name="dispatcher"> Диспетчер сообщений. </param>
        /// <param name="logger"> Логгер. </param>
        /// <param name="serializer"> Сериализатор сообщений. </param>
        public RabbitMqClient(IOptions<QueuesConfig> config, IMsgDispatcher dispatcher, ILogger<IMessageQueueClient> logger, IMessageSerializer serializer)
            : base(config?.Value, dispatcher, logger, serializer)
        {
            _typeAliasCache = new Dictionary<Type, string>();
            PrepareBehaviours(Configuration);
            _rpcWaits = new Dictionary<string, RpcResposeWaitHandle>();
        }

        /// <summary>
        /// Выполнить RPC запрос.
        /// </summary>
        /// <typeparam name="TArgs">Аргумент запроса.</typeparam>
        /// <typeparam name="TResp">Ответ.</typeparam>
        /// <param name="reqArgs">Экземпляр аргумента запроса.</param>
        /// <param name="queue">Очередь, в которую следует опубликровать сообщение.</param>
        /// <param name="timeout">Таймаут ожидания операции.</param>
        /// <returns>Возвращает ответ.</returns>
        public async Task<TResp> MakeRpcCallAsync<TArgs, TResp>(TArgs reqArgs, string queue, TimeSpan? timeout = null)
        {
            var requestId = Guid.NewGuid();
            var ev = new RpcResposeWaitHandle();

            // Подготовить RPC сообщение
            var resetWaitFunc = new Func<RpcResposeWaitHandle, object>((mre) =>
            {
                var basicProps = Model.CreateBasicProperties();
                basicProps.Headers = new Dictionary<string, object>();
                basicProps.Type = RpcRequestHeaderName;
                basicProps.CorrelationId = requestId.ToString();
                basicProps.ReplyTo = _rpcCallbackQueue;
                basicProps.Headers[RpcRespType] = Encoding.UTF8.GetBytes(typeof(TResp).AssemblyQualifiedName);
                basicProps.Headers[RpcArgType] = Encoding.UTF8.GetBytes(typeof(TArgs).AssemblyQualifiedName);

                // Отправить RPC сообщение
                PublishRpc(queue, reqArgs, basicProps, typeof(TArgs));
                Logger.LogTrace($"Отправлено RPC сообщение с ID: {requestId}");

                // Дождаться ответа и вернуть полезную нагрузку
                mre.WaitOne(timeout ?? Configuration.RpcTimeout);
                return mre.Payload;
            });

            lock (_lockDictionary)
            {
                _rpcWaits.Add(requestId.ToString(), ev);
            }

            var rpcWaitTask = Task.Run(() =>
            {
                var response = (RpcResponse)resetWaitFunc(ev);
                if (response == null)
                {
                    throw new TimeoutException("Операция прервана по таймауту");
                }

                if (response.ResponseType == ResponseType.Error)
                {
                    throw new Exception(response.Message);
                }

                return (TResp)response.Response;
            });

            return await rpcWaitTask;
        }

        /// <summary>
        /// Настроить клиент.
        /// </summary>
        public void SetUpClient()
        {
            Initialize();
            if (Configuration.UseRpc)
            {
                var guid = Guid.NewGuid().ToString();
                _rpcModel = Connection.CreateModel();
                _rpcModel.Configure(Configuration);

                // Сгенерировать уникальное имя очереди для ответа
                _rpcCallbackQueue = $"{Configuration.ClientName}_{guid}";
                _rpcModel.QueueDeclare(_rpcCallbackQueue, true, true, true);
                _rpcConsumer = new EventingBasicConsumer(_rpcModel);
                _rpcConsumer.Received += OnRpcCallbackRecived;
                _rpcModel.BasicConsume(_rpcCallbackQueue, true, _rpcConsumer);
            }
        }

        /// <summary>
        /// Добавить конфигурацию для конечной точки
        /// </summary>
        /// <param name="ep">Конечная точка</param>
        public void AppendEndpoint(EndpointConfig ep)
        {
            var eps = Configuration.Endpoints;
            var newEps = new List<EndpointConfig>(eps);
            newEps.Add(ep);
            Configuration.Endpoints = newEps.ToArray();
        }

        /// <summary>
        /// Создать очередь
        /// </summary>
        /// <param name="queue">Очередь</param>
        /// <param name="durable">Durable</param>
        /// <param name="autoDelete">AUto deltet</param>
        /// <param name="vhost">Vhost</param>
        public void QueueDeclare(string queue, bool durable = false, bool autoDelete = true, string vhost = "")
        {
            if (this.Model == null)
            {
                throw new Exception("Client not initialized or initialized with errors");
            }

            Model.QueueDeclare(queue, durable, false, autoDelete);
        }

        /// <summary>
        /// Удаление очереди
        /// </summary>
        /// <param name="queueName">Имя очереди</param>
        public void DeleteQueue(string queueName)
        {
            Model.QueueDelete(queueName);
        }

        /// <summary>
        /// Добавить биндинг между эксчейнджером и очередью
        /// </summary>
        /// <param name="exchanger">Эксчейнджер</param>
        /// <param name="queue">Очередь</param>
        /// <param name="routingKey">Routing key</param>
        public void CreateQueueBinding(string exchanger, string queue, string routingKey = "")
        {
            if (this.Model == null)
            {
                throw new Exception("Client not initialized or initialized with errors");
            }

            Model.QueueBind(queue, exchanger, routingKey);
        }

        /// <summary>
        /// Опубликовать сообщение в брокере в конкретной очереди.
        /// </summary>
        /// <param name="queue">Наименование очереди.</param>
        /// <param name="msg">Сообщение.</param>
        /// <param name="props">Свосйства запроса.</param>
        /// <param name="messageType">Тип отправляемого сообщения.</param>
        public void PublishMessage(string queue, object msg, IBasicProperties props = null, Type messageType = null)
        {
            PublishMessage(string.Empty, queue, msg, props, messageType);
        }

        /// <summary>
        /// Опубликовать сообщение в брокере в конкретной очереди.
        /// </summary>
        /// <param name="exchanger">Эксчейнджер.</param>
        /// <param name="queue">Наименование очереди.</param>
        /// <param name="msg">Сообщение.</param>
        /// <param name="props">Свосйства запроса.</param>
        /// <param name="messageType">Тип отправляемого сообщения.</param>
        public void PublishMessage(string exchanger, string queue, object msg, IBasicProperties props = null, Type messageType = null)
        {
            var bestType = messageType ?? msg.GetType();
            string messageTypeName;

            // Попробовать вытащить данные из кэша аттрибутов
            if (!_typeAliasCache.TryGetValue(bestType, out messageTypeName))
            {
                // Попытаться извлечь данные из арртрибута контракта
                var attr = bestType.GetCustomAttributes<RabbitContractAttribute>().FirstOrDefault();
                if (attr != null)
                {
                    messageTypeName = _typeAliasCache[bestType] = attr.Alias;
                }
                else
                {
                    messageTypeName = _typeAliasCache[bestType] = bestType.AssemblyQualifiedName;
                }
            }

            var message = new OwMessageCommon(messageTypeName);
            message.SetPayload(Serializer.SerializePayload(msg));
            var propsData = props ?? Model.CreateBasicProperties();
            propsData.Persistent = true;
            var bytes = Serializer.SerializeTransferMessage(message, messageType);
            Model.BasicPublish(exchanger, queue, propsData, bytes);
        }

        /// <summary>
        /// Отправить байты сообщеия.
        /// </summary>
        /// <param name="queue">Очередь.</param>
        /// <param name="bytes">Набор байт.</param>
        /// <param name="props">Свойства.</param>
        public void SendBytes(string queue, ReadOnlyMemory<byte> bytes, IBasicProperties props = null)
        {
            var propsData = props ?? Model.CreateBasicProperties();
            propsData.Persistent = true;
            Model.BasicPublish(string.Empty, queue, propsData, bytes);
        }

        /// <summary>
        /// Начать получать сообщения.
        /// </summary>
        public void BeginConsume()
        {
            List<IMessageProcessBehaviuor> behaviours;
            foreach (var epConf in Configuration.Endpoints)
            {
                var consumer = new EventingBasicConsumer(Model);
                consumer.Received += (sender, args) =>
                {
                    try
                    {
                        if (string.IsNullOrEmpty(args.BasicProperties.Type))
                        {
                            var message = Serializer.DeserializeTransferMessage(args.Body, out var type, Dispatcher.TryGetByAlias);

                            // Обработать сообщение, если оно подразумевает только получение
                            Dispatcher.DispatchMessage(
                            this,
                            (ack, requeue, order) => Acknowledgment(epConf, args, ack, requeue, order),
                            message,
                            type);
                        }
                        else if (args.BasicProperties.Type == RpcRequestHeaderName)
                        {
                            try
                            {
                                // Найти соответствующий делегат в списке ожидающих
                                try
                                {
                                    ValidateRpcMessageArgs(args);
                                }
                                catch (Exception ex)
                                {
                                    Logger.LogError(ex, $"Ошибка обработки сообщения DeliveryTag: {args.DeliveryTag}");
                                }

                                var props = Model.CreateBasicProperties();
                                props.CorrelationId = args.BasicProperties.CorrelationId;
                                props.Type = RpcResponseHeaderName;
                                try
                                {
                                    var argTypeName = Encoding.UTF8.GetString((byte[])args.BasicProperties.Headers[RpcArgType]);
                                    var respTypeName = Encoding.UTF8.GetString((byte[])args.BasicProperties.Headers[RpcRespType]);

                                    Logger.LogTrace($"Reviced msg argType: {argTypeName} respType: {respTypeName}");

                                    // Обработать сообщение
                                    var argType = Type.GetType(argTypeName);
                                    var respType = Type.GetType(respTypeName);
                                    if (argType == null)
                                    {
                                        throw new ArgumentException($"Не удалось найти тип аргумента запроса: {argTypeName}");
                                    }

                                    if (respType == null)
                                    {
                                        throw new ArgumentException($"Не удалось найти тип аргумента ответа: {respTypeName}");
                                    }

                                    var message = Serializer.DeserializeRpcMessage(args.Body, argType);
                                    var response = Dispatcher.HandleRpcMessage(
                                        this,
                                        message,
                                        (ack, requeue, order) => Acknowledgment(epConf, args, ack, requeue, order),
                                        argType,
                                        respType);
                                    var responseObject = new RpcResponse(ResponseType.Ok, response, string.Empty);
                                    props.Headers = new Dictionary<string, object>
                                    {
                                        { RpcRespType, args.BasicProperties.Headers[RpcRespType] },
                                    };

                                    // Отправить сообщение в очередь ответа
                                    PublishRpc(args.BasicProperties.ReplyTo, responseObject, props, responseObject.GetType());
                                    Model.BasicAck(args.DeliveryTag, true);
                                }
                                catch (Exception ex)
                                {
                                    // Отправить сообщение об ошибке
                                    var responseObject = new RpcResponse(ResponseType.Error, new object(), ex.ToString());
                                    Logger.Log(LogLevel.Error, ex, "Ошибка во время обработки RPC сообщения");
                                    PublishRpc(args.BasicProperties.ReplyTo, responseObject, props, responseObject.GetType());
                                    Model.BasicNack(args.DeliveryTag, false, false);
                                }
                            }
                            catch (Exception ex)
                            {
                                throw new InvalidDataContractException($"Невозможно ответить на RPC сообщение TAG: [{args.DeliveryTag}] CT: [{args.ConsumerTag}] т.к. заголовки сформированы с ошибкой", ex);
                            }
                        }
                        else
                        {
                            throw new NotSupportedException($"Неподдерживаемый тип сообщения: [{args.BasicProperties.Type}]");
                        }
                    }
                    catch (Exception ex)
                    {
                        if (!epConf.AutoAck)
                        {
                            Acknowledgment(epConf, args, false, false, true);
                        }

                        Logger.LogError(ex, $"Клиенту не удалось обработать сообщение DeliveryTag {args.DeliveryTag} из-за ошибки");
                        if (BehavioursDict.TryGetValue(MessageProcessPhase.OnError, out behaviours))
                        {
                            foreach (var messageProcessBehaviuor in behaviours)
                            {
                                Logger.Log(LogLevel.Debug, $"Отработка вызова поведения фаза [{MessageProcessPhase.OnError}]: {messageProcessBehaviuor}");
                                try
                                {
                                    messageProcessBehaviuor.InvokeBehaviour(args.Body);
                                }
                                catch (Exception bex)
                                {
                                    Logger.Log(LogLevel.Error, bex, $"Ошибка во время отработки поведения: [{messageProcessBehaviuor}]");
                                }
                            }
                        }
                    }
                };

                var tag = Model.BasicConsume(epConf.EndpointName, epConf.AutoAck, consumer);
                if (!RegistratedConsumers.ContainsKey(tag))
                {
                    RegistratedConsumers.Add(tag, consumer);
                }
                else
                {
                    throw new Exception($"Получатель сообщений уже зарегистрирован: {tag} - {epConf.EndpointName}");
                }
            }
        }

        /// <summary>
        /// Очистить ресурсы объекта.
        /// </summary>
        public override void Dispose()
        {
            if (Model != null)
            {
                Model.Dispose();
                Model = null;
            }

            if (_rpcWaits.Any())
            {
                foreach (var rpcWaitsValue in _rpcWaits.Values)
                {
                    rpcWaitsValue.Set();
                }
            }

            if (_rpcModel != null)
            {
                _rpcConsumer.Received -= OnRpcCallbackRecived;
                _rpcModel.Dispose();
            }

            if (Connection != null)
            {
                Connection.Dispose();
                Connection = null;
            }
        }

        private void PublishRpc(string endpoint, object msg, IBasicProperties props, Type requestType)
        {
            var propsData = props ?? Model.CreateBasicProperties();
            propsData.Persistent = true;
            var bytes = Serializer.SerializeTransferMessage(msg, requestType);
            Model.BasicPublish(string.Empty, endpoint, propsData, bytes);
        }

        private void Acknowledgment(EndpointConfig epConf, BasicDeliverEventArgs args, bool ack, bool requeue, bool order)
        {
            if (epConf.AutoAck)
            {
                throw new NotSupportedException("Ручное подтверждение не доступно т.к. выбран режим AutoAck = true");
            }

            if (order)
            {
                if (ack)
                {
                    Model.BasicAck(args.DeliveryTag, ack);
                }
                else
                {
                    Model.BasicNack(args.DeliveryTag, false, requeue);
                }
            }
            else
            {
                if (!ack)
                {
                    Model.BasicAck(args.DeliveryTag, ack);
                }
                else
                {
                    Model.BasicNack(args.DeliveryTag, false, requeue);
                }
            }
        }

        private void OnRpcCallbackRecived(object sender, BasicDeliverEventArgs args)
        {
            if (args.BasicProperties.Type != RpcResponseHeaderName)
            {
                Logger.LogTrace($"В очереди RPC ответов клиента [{_rpcCallbackQueue}] получено ответное сообщение с ошибочным типом [{args.BasicProperties.Type}]");
                return;
            }

            Logger.LogTrace($"Получен RPC ответ c ID: [{args.BasicProperties.CorrelationId}]");
            var messageResponse = (RpcResponse)Serializer.DeserializeRpcMessage(args.Body, typeof(RpcResponse));

            RpcResposeWaitHandle waitHandle;
            lock (_lockDictionary)
            {
                if (!_rpcWaits.ContainsKey(args.BasicProperties.CorrelationId))
                {
                    Logger.Log(LogLevel.Error, $"По RPC получен ответ с CorrelationId: [{args.BasicProperties.CorrelationId}]. ID не найден в списке ожидающих");
                    return;
                }

                waitHandle = _rpcWaits[args.BasicProperties.CorrelationId];
            }

            if (messageResponse.ResponseType == ResponseType.Ok)
            {
                // Если ожидающий запрос найден
                var respType = Encoding.UTF8.GetString((byte[])args.BasicProperties.Headers[RpcRespType]);

                // Десериализовать ответ
                var msgType = Type.GetType(respType);
                messageResponse.Response = !msgType.IsValueType ? ((JToken)messageResponse.Response).ToObject(msgType) : messageResponse.Response;
            }

            waitHandle.Payload = messageResponse;
            waitHandle.Set();

            lock (_lockDictionary)
            {
                _rpcWaits.Remove(args.BasicProperties.CorrelationId);
            }
        }

        private void ValidateRpcMessageArgs(BasicDeliverEventArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            if (!args.BasicProperties.Headers.ContainsKey(RpcArgType))
            {
                throw new ArgumentException($"В заголовке запроса не содержится параметр {RpcArgType}");
            }

            if (!args.BasicProperties.Headers.ContainsKey(RpcRespType))
            {
                throw new ArgumentException($"В заголовке запроса не содержится параметр {RpcRespType}");
            }

            if (string.IsNullOrEmpty(args.BasicProperties.ReplyTo))
            {
                throw new ArgumentException("Не указан параметр ReplyTo для запроса RPC");
            }

            if (string.IsNullOrEmpty(args.BasicProperties.CorrelationId))
            {
                throw new ArgumentException("Не указан параметр CorrelationId для запроса RPC");
            }

            if (string.IsNullOrEmpty(args.BasicProperties.ReplyTo))
            {
                throw new ArgumentException("Не указан параметр CorrelationId для запроса RPC");
            }

            if (!args.BasicProperties.Headers.ContainsKey(RpcArgType))
            {
                throw new ArgumentException("Должен быть указан тип запроса RPC");
            }

            if (!args.BasicProperties.Headers.ContainsKey(RpcRespType))
            {
                throw new ArgumentException("Должен быть указан тип ответа RPC");
            }
        }

        /// <summary>
        /// Подготовить модели поведения.
        /// </summary>
        /// <param name="configuration">Конфигурация очереди.</param>
        private void PrepareBehaviours(QueuesConfig configuration)
        {
            BehavioursDict = new Dictionary<MessageProcessPhase, List<IMessageProcessBehaviuor>>();

            // Разобрать модели поведения если они имеются
            if (configuration.BehaviourConfigurations != null && configuration.BehaviourConfigurations.Any())
            {
                foreach (var bc in configuration.BehaviourConfigurations)
                {
                    try
                    {
                        var behaviour = JsonConvert.DeserializeObject<BehaviourBase>(
                            bc.Configuration,
                            new JsonSerializerSettings
                            {
                                TypeNameHandling = TypeNameHandling.Auto,
                            });

                        List<IMessageProcessBehaviuor> behaviours;
                        if (!BehavioursDict.TryGetValue(behaviour.ProcessPhase, out behaviours))
                        {
                            behaviours = new List<IMessageProcessBehaviuor>();
                            BehavioursDict.Add(behaviour.ProcessPhase, behaviours);
                        }

                        behaviour.SetClient(this);
                        behaviours.Add(behaviour);
                    }
                    catch (Exception ex)
                    {
                        var msg = $"Ошибка во время разбора модели поведения клиента [{bc.Configuration}]";
                        Logger.LogError(ex, msg);
                        throw new Exception(msg, ex);
                    }
                }
            }
        }
    }
}

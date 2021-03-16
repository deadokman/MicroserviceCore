// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OwJsonSerializer.cs" company="Konyshev A.V., Stepanov N.O.">
//   Copyright (c) Konyshev A.V., Stepanov N.O.
// </copyright>
// <summary>
//   Класс-сериализатор по умолчанию
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Msc.Microservice.Layer.RabbitMq.Configuration;
using Msc.Microservice.Layer.RabbitMq.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Msc.Microservice.Layer.RabbitMq
{
    /// <summary>
    /// Сериализатор JSON.
    /// </summary>
    public class OwJsonSerializer : IMessageSerializer
    {
        private ILogger<IMessageSerializer> Logger { get; }

        private readonly IDictionary<string, string> _namespaceMaps;

        /// <summary>
        /// Сериализатор JSON по умолчанию.
        /// </summary>
        /// <param name="config">Конфигурация.</param>
        /// <param name="logger">Логгер.</param>
        public OwJsonSerializer(IOptions<QueuesConfig> config, ILogger<IMessageSerializer> logger)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _namespaceMaps = new Dictionary<string, string>();
            if (config?.Value?.NamespaceMap != null)
            {
                foreach (var item in config.Value.NamespaceMap)
                {
                    if (!string.IsNullOrEmpty(item.SourceNamespace) && !string.IsNullOrEmpty(item.TargetNamespace))
                    {
                        _namespaceMaps[item.SourceNamespace] = item.TargetNamespace;
                    }
                }
            }
        }

        /// <summary>
        /// Сериализовать сообщений в байт-массив.
        /// </summary>
        /// <param name="message">Сообщение.</param>
        /// <param name="messageType">Тип сообщения.</param>
        /// <returns>Байт-массив сообщения.</returns>
        public byte[] SerializeTransferMessage(object message, Type messageType)
        {
            var msg = JsonConvert.SerializeObject(message, Formatting.None);
            return Encoding.UTF8.GetBytes(msg);
        }

        /// <summary>
        /// Диссриализовать сообщения из байт-массива.
        /// </summary>
        /// <param name="body">Тело сообщения.</param>
        /// <param name="contentType">Тип объектной модели сообщения.</param>
        /// <param name="contractTypes">Делегат на получение типа контракта по алиасу.</param>
        /// <returns>Возвращает диссериализованный объект содержимого сообщения.</returns>
        public object DeserializeTransferMessage(ReadOnlyMemory<byte> body, out Type contentType, Func<string, Type> contractTypes = null)
        {
#if NETSTANDARD2_1
            var messageEncoding = Encoding.UTF8.GetString(body.Span);
#else
            var messageEncoding = Encoding.UTF8.GetString(body.ToArray());
#endif

            var messageBody = JToken.Parse(messageEncoding);
            var mesageType = messageBody.Value<string>("MessageType");
            var payload = messageBody.Value<JToken>("Payload")?.ToString();

            if (string.IsNullOrEmpty(mesageType))
            {
                throw new Exception("В сообщении отсутствует информация о его типе");
            }

            if (string.IsNullOrEmpty(payload))
            {
                throw new Exception("В сообщении отсутствуют данные (payload == null)");
            }

            if (contractTypes != null)
            {
                var contractType = contractTypes.Invoke(mesageType);
                if (contractType != null)
                {
                    contentType = contractType;
                    return JsonConvert.DeserializeObject(payload, contractType);
                }
            }

            Type type;
            try
            {
                var headerTypename = TypeNameFromFullQualified.GetTypeName(mesageType);
                var typeName = _namespaceMaps.ContainsKey(headerTypename.TypeNameUnqualified)
                    ? TypeNameFromFullQualified.GetTypeName(_namespaceMaps[headerTypename.TypeNameUnqualified]) : headerTypename;
                type = Type.GetType(typeName.TypeNameUnqualified);
                if (type == null)
                {
                    throw new Exception($"Не удалось создать тип {typeName.TypeNameUnqualified}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Не удалось создать создать тип из заголовка сообщения из-за ошибки: \n {ex}");
            }

            if (string.IsNullOrEmpty(payload))
            {
                throw new Exception("В сообщении отсутствует полезная нагрузка");
            }

            var message = JsonConvert.DeserializeObject(payload, type);
            contentType = type;
            return message;
        }

        /// <summary>
        /// Преобразовать в объект сообщение, которое подразумевает только передачу данных.
        /// </summary>
        /// <param name="body"> Тело сообщения. </param>
        /// <param name="messageType"> Тип сообщения. </param>
        /// <returns>Возвращает диссериализованную инстанцию объекта.</returns>
        public object DeserializeRpcMessage(ReadOnlyMemory<byte> body, Type messageType)
        {
#if NETSTANDARD2_1
            var mesageStr = Encoding.UTF8.GetString(body.Span);
#else
            var mesageStr = Encoding.UTF8.GetString(body.ToArray());
#endif

            Logger.LogTrace($"msgBody: {mesageStr}");
            return JsonConvert.DeserializeObject(mesageStr, messageType);
        }

        /// <summary>
        /// Преобразовать payload в json для передачи.
        /// </summary>
        /// <param name="payload">Данные сообщения.</param>
        /// <returns>JSON - объект сообщения.</returns>
        public string SerializePayload(object payload)
        {
            return JsonConvert.SerializeObject(payload, Formatting.None);
        }
    }
}

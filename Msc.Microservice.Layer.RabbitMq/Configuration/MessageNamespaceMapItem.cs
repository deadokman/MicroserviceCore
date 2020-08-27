// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageNamespaceMapItem.cs" company="Konyshev A.V., Stepanov N.O.">
//   Copyright (c) Konyshev A.V., Stepanov N.O.
// </copyright>
// <summary>
//   Маппинг неймспейса
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Msc.Microservice.Layer.RabbitMq.Configuration
{
    /// <summary>
    /// Экземпляр настроек маппинга неймспейса.
    /// </summary>
    public class MessageNamespaceMapItem
    {
        /// <summary>
        /// Неймспейс источника данных.
        /// </summary>
        public string SourceNamespace { get; set; }

        /// <summary>
        /// Целевой неймспейс.
        /// </summary>
        public string TargetNamespace { get; set; }
    }
}

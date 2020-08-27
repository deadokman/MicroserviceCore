// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BehaviourConfig.cs" company="Konyshev A.V., Stepanov N.O.">
//   Copyright (c) Konyshev A.V., Stepanov N.O.
// </copyright>
// <summary>
//   Класс предназначен для передачи конфигурации бехейверов в клиент
// </summary>
// -------------------------------------------------------------------------------------------------------------------

using Newtonsoft.Json;

namespace Msc.Microservice.Layer.RabbitMq.Configuration
{
    /// <summary>
    /// Класс предназначен для передачи конфигурации бехейверов в клиент.
    /// </summary>
    public class BehaviourConfig
    {
        /// <summary>
        /// Содержит JSON описание бихейвера.
        /// </summary>
        [JsonProperty]
        public string Configuration { get; set; }
    }
}

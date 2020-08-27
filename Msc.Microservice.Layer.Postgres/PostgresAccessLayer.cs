// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostgresAccessLayer.cs" company="Konyshev A.V., Stepanov N.O.">
//   Copyright (c) Konyshev A.V., Stepanov N.O.
// </copyright>
// <summary>
//   Слой доступа к СУБД Postgres
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Msc.Db.Postgres;
using Msc.Interface.Db;
using Msc.Microservice.Core.Standalone.Layering;

namespace Msc.Microservice.Layer.Postgres
{
    /// <summary>
    /// Слой доступа к СУБД Postgres.
    /// </summary>
    public class PostgresAccessLayer : MicroserviceLayerBase
    {
        private readonly string _connString;

        /// <summary>
        /// Инициализировать слой через строку подключения.
        /// </summary>
        /// <param name="connString">Строка подключения.</param>
        public PostgresAccessLayer(string connString)
        {
            _connString = connString;
        }

        /// <summary>
        /// Инициализировать, используя конфигурацию.
        /// </summary>
        public PostgresAccessLayer()
        {
        }

        /// <summary>
        /// Зарегистрировать конфигурационный файл.
        /// </summary>
        /// <param name="configurationBuilder">Построитель конфигурации.</param>
        public override void RegisterConfiguration(IConfigurationBuilder configurationBuilder)
        {
            return;
        }

        /// <summary>
        /// Выполнить регистрацию и валидацию конфигураций.
        /// </summary>
        /// <param name="configurationRoot">Валидация конфигураций.</param>
        /// <param name="serviceCollection">Коллекция служб.</param>
        /// <returns>Список ошибок конфигурации.</returns>
        public override IEnumerable<string> RegisterLayer(IConfigurationRoot configurationRoot, IServiceCollection serviceCollection)
        {
            string connectionString;
            var errors = new List<string>();
            if (!string.IsNullOrEmpty(_connString))
            {
                connectionString = _connString;
            }
            else
            {
                var postgesConfig = configurationRoot.GetSection(nameof(Configuration.Postgres)).Get<Configuration.Postgres>();
                errors.AddRange(postgesConfig.ValidateErrors());
                if (errors.Any())
                {
                    return errors;
                }

                connectionString = postgesConfig.ConnectionString;
            }

            serviceCollection.AddTransient<IDbContext>((provider) => new PgDbContext(connectionString, provider.GetService<ILogger<IDbContext>>()));
            return errors;
        }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PgDbContext.cs" company="Konyshev A.V., Stepanov N.O.">
//   Copyright (c) Konyshev A.V., Stepanov N.O.
// </copyright>
// <summary>
//   Контекст подключения к БД
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Msc.Interface.Db;
using IDbConnection = Msc.Interface.Db.IDbConnection;

namespace Msc.Db.Postgres
{
   /// <summary>
    /// Контекст подключения к БД
    /// </summary>
    public class PgDbContext : AbstractDbContext
    {
        private readonly ILocaleProvider _localeProvider;
        private readonly bool _setLocale;

        static PgDbContext()
        {
        }

        /// <summary>
        /// Конструктор только со строкой подключения к БД
        /// </summary>
        /// <param name="connectionString">Строка подключения</param>
        /// <param name="logger">Интерфейс логгера</param>
        public PgDbContext(string connectionString, ILogger<IDbContext> logger)
        : base(connectionString, logger)
        {
        }

        /// <summary>
        /// Конструктор со строкой подключения, логгером и провайдером локали
        /// </summary>
        /// <param name="connectionString">Строка подключения</param>
        /// <param name="logger">Интерфейс логгера</param>
        /// <param name="localeProvider">Интерфейс провайдера языка</param>
        /// <param name="setLocale">Выставлять локаль</param>
        public PgDbContext(string connectionString, ILogger<IDbContext> logger, ILocaleProvider localeProvider, bool setLocale = true)
            : this(connectionString, logger)
        {
            _localeProvider = localeProvider ?? throw new ArgumentNullException(nameof(localeProvider));
            _setLocale = setLocale;
        }

        /// <inheritdoc />
        public override IDbConnection GetConnection()
        {
            return GetConnection(ConnectionString);
        }

        /// <summary>
        /// Get connection async
        /// </summary>
        /// <returns>Async connection</returns>
        public override async Task<DbConnection> GetConnectionAsync()
        {
            var connection = new PgDbConnection(ConnectionString, Logger);
            await connection.OpenAsync();
            return await Task.FromResult(connection);
        }

        /// <inheritdoc />
        public override IDbConnection GetConnection(string connectionString)
        {
            var connection = new PgDbConnection(connectionString, Logger);
            connection.Open();

            // var tz = $"set time zone interval '{TimeZoneInfo.Local.BaseUtcOffset:hh\\:mm}'";
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "set time zone '00'";
                cmd.ExecuteNonQuery();
            }

            if (_setLocale)
            {
                SetLocale(connection);
            }

            return connection;
        }

        private void SetLocale(PgDbConnection connection)
        {
            if (_localeProvider != null)
            {
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "main.set_language";
                    cmd.CommandType = CommandType.StoredProcedure;
                    var param = cmd.CreateParameter();
                    param.ParameterName = "p_language_id";
                    param.Value = (int)_localeProvider.Locale;
                    param.DbType = DbType.Int32;
                    cmd.Parameters.Add(param);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
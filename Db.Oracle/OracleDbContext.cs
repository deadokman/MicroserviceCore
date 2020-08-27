using System.Data.Common;
using System.Threading.Tasks;
using Dapper;
using Msc.Interface.Db;
using Microsoft.Extensions.Logging;
using IDbConnection = Msc.Interface.Db.IDbConnection;

namespace Msc.Db.Oracle
{
    /// <summary>
    /// Контекст подключения к БД
    /// </summary>
    public class OracleDbContext : AbstractDbContext
    {
        /// <summary>
        /// Конструктор только со строкой подключения к БД
        /// </summary>
        /// <param name="connectionString">Строка подключения</param>
        /// <param name="logger">Интерфейс логгера</param>
        public OracleDbContext(string connectionString, ILogger<IDbContext> logger) : base(connectionString, logger)
        {
            DefaultTypeMap.MatchNamesWithUnderscores = true;
        }

        /// <inheritdoc />
        public override IDbConnection GetConnection()
        {
            var connection = new OracleDbConnection(ConnectionString, Logger);
            connection.Open();

            return connection;
        }

        public override async Task<DbConnection> GetConnectionAsync()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public override IDbConnection GetConnection(string connectionString)
        {
            var connection = new OracleDbConnection(connectionString, Logger);
            connection.Open();

            return connection;
        }
    }
}
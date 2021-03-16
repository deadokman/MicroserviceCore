using Microsoft.Extensions.Logging;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace Msc.Interface.Db
{
    public abstract class AbstractDbContext : IDbContext
    {
        protected readonly string ConnectionString;

        protected readonly ILogger Logger;

        protected AbstractDbContext(string connectionString, ILogger<IDbContext> logger)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            ConnectionString = connectionString;
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public abstract IDbConnection GetConnection();

        /// <summary>
        /// Get connection async
        /// </summary>
        /// <returns>Async connection</returns>
        public abstract Task<DbConnection> GetConnectionAsync();

        public abstract IDbConnection GetConnection(string connectionString);

        /// <inheritdoc />
        public void Dispose()
        {
        }
    }
}

using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace Msc.Interface.Db
{
    /// <summary>
    /// Предоставляет конкретную реализацию соединения к базе данных
    /// </summary>
    public interface IDbContext : IDisposable
    {
        /// <summary>
        /// Получить соединение со строкой подключения по умоланию
        /// </summary>
        /// <returns></returns>
        IDbConnection GetConnection();

        /// <summary>
        /// Get connection async
        /// </summary>
        /// <returns>Async connection</returns>
        Task<DbConnection> GetConnectionAsync();

        /// <summary>
        /// Получить соединение с указанной строкой подключение
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        IDbConnection GetConnection(string connectionString);
    }
}
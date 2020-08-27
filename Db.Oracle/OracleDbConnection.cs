using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using Microsoft.Extensions.Logging;
using Msc.Interface.Db;
using Oracle.ManagedDataAccess.Client;
using IDbConnection = Msc.Interface.Db.IDbConnection;

namespace Msc.Db.Oracle
{
    /// <summary>
    /// Обертка над подключением к источнику данных. Oracle
    /// </summary>
    public class OracleDbConnection : IDbConnection
    {
        private readonly OracleConnection _connection;
        private readonly ILogger _logger;

        static OracleDbConnection()
        {
            DefaultTypeMap.MatchNamesWithUnderscores = true;
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="connectionString">строка подключения к БД</param>
        /// <param name="logger">интерфейс логгера</param>
        internal OracleDbConnection(string connectionString, ILogger logger)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _connection = new OracleConnection(connectionString);
        }

        /// <summary>
        /// Начинает транзакцию базы данных
        /// </summary>
        /// <returns>Транзакция</returns>
        public IDbTransaction BeginTransaction()
        {
            return _connection.BeginTransaction();
        }

        /// <summary>
        /// Начинает транзакцию базы данных с указанным значением <see cref="IsolationLevel"/>
        /// </summary>
        /// <param name="il">уровень изоляции</param>
        /// <returns>Транзакция</returns>
        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            return _connection.BeginTransaction(il);
        }

        /// <summary>
        /// Изменяет текущую базу данных для открытого объекта Connection
        /// </summary>
        /// <param name="databaseName">название БД</param>
        public void ChangeDatabase(string databaseName)
        {
            _connection.ChangeDatabase(databaseName);
        }

        /// <summary>
        /// Создает и возвращает объект Command, связанный с подключением
        /// </summary>
        /// <returns>объект Command</returns>
        public IDbCommand CreateCommand()
        {
            var command = _connection.CreateCommand();
            command.BindByName = true;
            return new OracleDbCommand(command, _logger);
        }

        /// <summary>
        /// Устанавливает подключение к базе данных
        /// </summary>
        public void Open()
        {
            _connection.Open();
        }

        /// <summary>
        /// Закрывает соединение с базой данных
        /// </summary>
        public void Close()
        {
            _connection.Close();
        }

        /// <summary>
        /// Уничтожить объект
        /// </summary>
        public void Dispose()
        {
            _connection.Dispose();
        }

        /// <summary>
        /// Строка подключения
        /// </summary>
        public string ConnectionString
        {
            get => _connection.ConnectionString;
            set => _connection.ConnectionString = value;
        }

        /// <summary>
        /// Получает время ожидания при попытке установки подключения
        /// </summary>
        public int ConnectionTimeout => _connection.ConnectionTimeout;

        /// <summary>
        /// Получает имя текущей базы данных
        /// </summary>
        public string Database => _connection.Database;

        /// <summary>
        /// Возвращает текущее состояние подключения
        /// </summary>
        public ConnectionState State => _connection.State;

        /// <inheritdoc />
        public void BulkCopy<T>(string sql, IEnumerator<T> enumerator, Func<T, object[]> mapFunction)
        {
            throw new NotImplementedException();
        }

        public void BulkCopy<T>(string sql, IEnumerator<T> enumerator, Action<T, IBinaryWriter> writerDelegate)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Вернуть DbType даты для конкретной реализации БД
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public object GetDateByDbType(DateTime dt)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Массовое чтение
        /// </summary>
        /// <param name="sql"></param>
        /// <returns>Читатель</returns>
        public IBulkReader BulkLoad(string sql)
        {
            throw new NotImplementedException();
        }

        public IBinaryWriter BeginBinaryImport(string sql)
        {
            throw new NotImplementedException();
        }
    }
}
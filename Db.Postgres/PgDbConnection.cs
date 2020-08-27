// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PgDbConnection.cs" company="Konyshev A.V., Stepanov N.O.">
//   Copyright (c) Konyshev A.V., Stepanov N.O.
// </copyright>
// <summary>
//   Обертка над подключением к источнику данных. Postgres
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Microsoft.Extensions.Logging;
using Msc.Interface.Db;
using Npgsql;
using NpgsqlTypes;
using IDbConnection = Msc.Interface.Db.IDbConnection;

namespace Msc.Db.Postgres
{
    /// <summary>
    /// Обертка над подключением к источнику данных. Postgres
    /// </summary>
    public class PgDbConnection : DbConnection, IDbConnection
    {
        private readonly NpgsqlConnection _connection;
        private readonly ILogger _logger;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="connectionString">строка подключения к БД</param>
        /// <param name="logger">интерфейс логгера</param>
        internal PgDbConnection(string connectionString, ILogger logger)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _connection = new NpgsqlConnection(connectionString);
        }

        /// <summary>
        /// Изменяет текущую базу данных для открытого объекта Connection
        /// </summary>
        /// <param name="databaseName">название БД</param>
        public override void ChangeDatabase(string databaseName)
        {
            _connection.ChangeDatabase(databaseName);
        }

        /// <summary>
        /// Устанавливает подключение к базе данных
        /// </summary>
        public override void Open()
        {
            _connection.Open();
        }

        /// <summary>
        /// Закрывает соединение с базой данных
        /// </summary>
        public override void Close()
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
        public override string ConnectionString
        {
            get => _connection.ConnectionString;
            set => _connection.ConnectionString = value;
        }

        /// <summary>
        /// Получает время ожидания при попытке установки подключения
        /// </summary>
        public override int ConnectionTimeout => _connection.ConnectionTimeout;

        /// <summary>
        /// Получает имя текущей базы данных
        /// </summary>
        public override string Database => _connection.Database;

        /// <summary>Gets the name of the database server to which to connect.</summary>
        /// <returns>The name of the database server to which to connect. The default value is an empty string.</returns>
        public override string DataSource => _connection.DataSource;

        /// <summary>Gets a string that represents the version of the server to which the object is connected.</summary>
        /// <returns>The version of the database. The format of the string returned depends on the specific type of connection you are using.</returns>
        /// <exception cref="T:System.InvalidOperationException"><see cref="P:System.Data.Common.DbConnection.ServerVersion"></see> was called while the returned Task was not completed and the connection was not opened after a call to <see cref="Overload:System.Data.Common.DbConnection.OpenAsync"></see>.</exception>
        public override string ServerVersion => _connection.ServerVersion;

        /// <summary>
        /// Возвращает текущее состояние подключения
        /// </summary>
        public override ConnectionState State => _connection.State;

        /// <summary>
        /// Сохранение множества записей
        /// </summary>
        /// <param name="sql">Выполняемый запрос</param>
        /// <param name="enumerator">Енумератор-перечислятор ☺</param>
        /// <param name="mapFunction">Преобразование сущности в массив свойств</param>
        /// <typeparam name="T">Тип сохраняемой сущности</typeparam>
        public void BulkCopy<T>(string sql, IEnumerator<T> enumerator, Func<T, object[]> mapFunction)
        {
            using (var writer = _connection.BeginBinaryImport(sql))
            {
                while (enumerator.MoveNext())
                {
                    writer.WriteRow(mapFunction(enumerator.Current));
                }

                writer.Complete();
            }
        }

        /// <summary>
        /// Сохранение множества записей
        /// </summary>
        /// <param name="sql">Выполняемый запрос</param>
        /// <param name="enumerator">Енумератор-перечислятор ☺</param>
        /// <param name="writerDelegate">Делегат для записи в бинарный поток</param>
        /// <typeparam name="T">Тип сохраняемой сущности</typeparam>
        public void BulkCopy<T>(string sql, IEnumerator<T> enumerator, Action<T, IBinaryWriter> writerDelegate)
        {
            using (var writer = _connection.BeginBinaryImport(sql))
            {
                var bWrtier = new PgBinaryWriter(writer);
                while (enumerator.MoveNext())
                {
                   writer.StartRow();
                   writerDelegate.Invoke(enumerator.Current, bWrtier);
                }

                writer.Complete();
            }
        }

        /// <summary>
        /// Выполнить вставку данных из DataRaader
        /// </summary>
        /// <param name="sql">Sql запрос для импортируемых данных</param>
        /// <returns>Интерфейс бинарной записи данных</returns>
        public IBinaryWriter BeginBinaryImport(string sql)
        {
            return new PgBinaryWriter(_connection.BeginBinaryImport(sql));
        }

        /// <summary>
        /// Вернуть DbType даты для конкретной реализации БД
        /// </summary>
        /// <param name="dt">Дата</param>
        /// <returns>DbType даты</returns>
        public object GetDateByDbType(DateTime dt)
        {
            return new NpgsqlDate(dt);
        }

        /// <summary>
        /// Чтение
        /// </summary>
        /// <param name="sql">Запрос для выборки</param>
        /// <returns>Бинарный загрузчик</returns>
        public IBulkReader BulkLoad(string sql)
        {
            var reader = _connection.BeginBinaryExport(sql);
            return new PgDbBulkReader(reader);
        }

        /// <summary>
        /// Начинает транзакцию базы данных
        /// </summary>
        /// <param name="isolationLevel">Уровень изоляции</param>
        /// <returns>Транзакция</returns>
        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            return _connection.BeginTransaction(isolationLevel);
        }

        /// <summary>
        /// Создать команду
        /// </summary>
        /// <returns> Новая команда </returns>
        protected override DbCommand CreateDbCommand()
        {
            var command = _connection.CreateCommand();
            return new PgDbCommand(command, _logger);
        }
    }
}
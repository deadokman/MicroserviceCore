// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PgDbCommand.cs" company="Konyshev A.V., Stepanov N.O.">
//   Copyright (c) Konyshev A.V., Stepanov N.O.
// </copyright>
// <summary>
//   Обёртка над Postgres Command с логированием запросов и ошибок выполнения.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Data;
using System.Data.Common;
using System.Text;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Msc.Db.Postgres
{
    /// <summary>
    /// Обёртка над Postgres Command с логированием запросов и ошибок выполнения.
    /// </summary>
    public class PgDbCommand : DbCommand
    {
        private readonly NpgsqlCommand _command;
        private readonly ILogger _logger;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="command">команда выполнения</param>
        /// <param name="logger">интерфейс логгера</param>
        internal PgDbCommand(NpgsqlCommand command, ILogger logger)
        {
            _command = command ?? throw new ArgumentNullException(nameof(command));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Возвращает или задает текстовую команду для выполнения
        /// </summary>
        public override string CommandText
        {
            get => _command.CommandText;
            set => _command.CommandText = value;
        }

        /// <summary>
        /// Возвращает или задает время ожидания перед завершением попытки выполнить команду
        /// </summary>
        public override int CommandTimeout
        {
            get => _command.CommandTimeout;
            set => _command.CommandTimeout = value;
        }

        /// <summary>
        /// Тип команды
        /// </summary>
        public override CommandType CommandType
        {
            get => _command.CommandType;
            set => _command.CommandType = value;
        }

        /// <summary>Gets or sets a value indicating whether the command object should be visible in a customized interface control.</summary>
        /// <returns>true, if the command object should be visible in a control; otherwise false. The default is true.</returns>
        public override bool DesignTimeVisible
        {
            get => _command.DesignTimeVisible;
            set => _command.DesignTimeVisible = value;
        }

        /// <summary>
        /// Возвращает или задает способ применения результатов команды
        /// </summary>
        public override UpdateRowSource UpdatedRowSource
        {
            get => _command.UpdatedRowSource;
            set => _command.UpdatedRowSource = value;
        }

        /// <summary>
        /// Отмена команды
        /// </summary>
        public override void Cancel()
        {
            _command.Cancel();
        }

        /// <summary>
        /// Выполнить команду и вернуть число задействованных строк
        /// </summary>
        /// <returns>число строк</returns>
        public override int ExecuteNonQuery()
        {
            return Execute(_command.ExecuteNonQuery);
        }

        /// <summary>
        /// Выполняет запрос и возвращает первый столбец первой строки результирующего набора
        /// </summary>
        /// <returns>Первая строка</returns>
        public override object ExecuteScalar()
        {
            return Execute(_command.ExecuteScalar);
        }

        /// <summary>
        /// Компилирование команды выполнения
        /// </summary>
        public override void Prepare()
        {
            _command.Prepare();
        }

        /// <summary>Создает новый экземпляр класса <see cref="T:System.Data.Common.DbParameter"></see> </summary>
        /// <returns>Экземпляр класса <see cref="T:System.Data.Common.DbParameter"></see></returns>
        protected override DbParameter CreateDbParameter()
        {
            return _command.CreateParameter();
        }

        /// <summary>Получает или устанавливает ссылка на <see cref="T:System.Data.Common.DbConnection"></see> используемую в <see cref="T:System.Data.Common.DbCommand"></see>.</summary>
        /// <returns>Подключение к источнику данных.</returns>
        protected override DbConnection DbConnection
        {
            get => _command.Connection;
            set => _command.Connection = (NpgsqlConnection)value;
        }

        /// <summary>Получает коллекцию <see cref="T:System.Data.Common.DbParameter"></see> </summary>
        /// <returns>Параметры для запроса SQL или хранимой прцедуры</returns>
        protected override DbParameterCollection DbParameterCollection => _command.Parameters;

        /// <summary>Получает или устанавливает ссылка на <see cref="P:System.Data.Common.DbCommand.DbTransaction"></see> используемую вместе с классом <see cref="T:System.Data.Common.DbCommand"></see> </summary>
        /// <returns>Транзакция в рамках которой будет выполнена комманда. Значение по умолчанию NULL</returns>
        protected override DbTransaction DbTransaction
        {
            get => _command.Transaction;
            set => _command.Transaction = (NpgsqlTransaction)value;
        }

        /// <summary>Выполняет команду используя соединение</summary>
        /// <param name="behavior">Экземпляр класса <see cref="T:System.Data.CommandBehavior"></see>.</param>
        /// <returns>Объект представляющей собой набор данных.</returns>
        /// <exception cref="T:System.Data.Common.DbException">Ошибка возникающая во время выполнения команды.</exception>
        /// <exception cref="T:System.ArgumentException">An invalid <see cref="T:System.Data.CommandBehavior"></see> value.</exception>
        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            return Execute(() => _command.ExecuteReader(behavior));
        }

        /// <summary>
        /// Обработчик выполнения команды
        /// </summary>
        /// <typeparam name="T">Тип результата выполнения</typeparam>
        /// <param name="func">Функция, которую необходимо выполненить</param>
        /// <returns>Результат выполнения</returns>
        private T Execute<T>(Func<T> func)
        {
            try
            {
                return func();
            }
            catch (NpgsqlException exception)
            {
                Logging(exception.Message);
                throw;
            }
        }

        /// <summary>
        /// Обработка ошибки выполнения запроса
        /// </summary>
        /// <param name="message">Текст ошибки</param>
        private void Logging(string message)
        {
            var logMessage = new StringBuilder();
            logMessage.Append(message);
            logMessage.Append(_command.CommandText);
            for (var i = 0; i < _command.Parameters.Count; i++)
            {
                var parameter = _command.Parameters[i];
                logMessage.Append($"Parameter name:{parameter.ParameterName}, type:{parameter.NpgsqlDbType}, value:{parameter.Value}");
            }

            _logger.Log(LogLevel.Error, $"Exception has occurred: {logMessage}");
        }
    }
}
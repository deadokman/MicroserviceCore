using System;
using System.Data;
using System.Text;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;

namespace Msc.Db.Oracle
{
    /// <summary>
    /// Обёртка над Oracle Command с логированием запросов и ошибок выполнения.
    /// </summary>
    public class OracleDbCommand : IDbCommand
    {
        private readonly OracleCommand _command;
        private readonly ILogger _logger;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="command">команда выполнения</param>
        /// <param name="logger">интерфейс логгера</param>
        internal OracleDbCommand(OracleCommand command, ILogger logger)
        {
            _command = command ?? throw new ArgumentNullException(nameof(command));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Возвращает или задает текстовую команду для выполнения
        /// </summary>
        public string CommandText
        {
            get => _command.CommandText;
            set => _command.CommandText = value;
        }

        /// <summary>
        /// Возвращает или задает время ожидания перед завершением попытки выполнить команду
        /// </summary>
        public int CommandTimeout
        {
            get => _command.CommandTimeout;
            set => _command.CommandTimeout = value;
        }

        /// <summary>
        /// Тип команды
        /// </summary>
        public CommandType CommandType
        {
            get => _command.CommandType;
            set => _command.CommandType = value;
        }

        /// <summary>
        /// Возвращает или задает объект IDbConnection
        /// </summary>
        public IDbConnection Connection
        {
            get => _command.Connection;
            set => _command.Connection = (OracleConnection)value;
        }

        /// <summary>
        /// Возвращает набор параметров запроса
        /// </summary>
        public IDataParameterCollection Parameters => _command.Parameters;

        /// <summary>
        /// Возвращает или задает транзакцию
        /// </summary>
        public IDbTransaction Transaction
        {
            get => _command.Transaction;
            set => _command.Transaction = (OracleTransaction)value;
        }

        /// <summary>
        /// Возвращает или задает способ применения результатов команды
        /// </summary>
        public UpdateRowSource UpdatedRowSource
        {
            get => _command.UpdatedRowSource;
            set => _command.UpdatedRowSource = value;
        }

        /// <summary>
        /// Отмена команды
        /// </summary>
        public void Cancel()
        {
            _command.Cancel();
        }

        /// <summary>
        /// Создать параметр запроса
        /// </summary>
        /// <returns>Параметр запроса</returns>
        public IDbDataParameter CreateParameter()
        {
            return _command.CreateParameter();
        }

        /// <summary>
        /// Уничтожить команду
        /// </summary>
        public void Dispose()
        {
            _command.Dispose();
        }

        /// <summary>
        /// Выполнить команду и вернуть число задействованных строк
        /// </summary>
        /// <returns>число строк</returns>
        public int ExecuteNonQuery()
        {
            return Execute(_command.ExecuteNonQuery);
        }

        /// <summary>
        /// Выполнить запрос и создать IDataReader
        /// </summary>
        /// <returns>Поток результатов выполнения</returns>
        public IDataReader ExecuteReader()
        {
            return Execute(_command.ExecuteReader);
        }

        /// <summary>
        /// Выполнить запрос и создать IDataReader
        /// </summary>
        /// <param name="behavior">CommandBehavior</param>
        /// <returns>Поток результатов выполнения</returns>
        public IDataReader ExecuteReader(CommandBehavior behavior)
        {
            return Execute(() => _command.ExecuteReader(behavior));
        }

        /// <summary>
        /// Выполняет запрос и возвращает первый столбец первой строки результирующего набора
        /// </summary>
        /// <returns>Первая строка</returns>
        public object ExecuteScalar()
        {
            return Execute(_command.ExecuteScalar);
        }

        /// <summary>
        /// Компилирование команды выполнения
        /// </summary>
        public void Prepare()
        {
            _command.Prepare();
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
            catch (OracleException exception)
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
                logMessage.Append($"Parameter name:{parameter.ParameterName}, type:{parameter.OracleDbType}, value:{parameter.Value}");
            }

            _logger.Log(LogLevel.Error, $"Exception has occurred: {logMessage}");
        }
    }
}
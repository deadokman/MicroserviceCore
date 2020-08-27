// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PgDbBulkReader.cs" company="Konyshev A.V., Stepanov N.O.">
//   Copyright (c) Konyshev A.V., Stepanov N.O.
// </copyright>
// <summary>
//   Обёртка над Postgres Command с логированием запросов и ошибок выполнения.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Data;
using Msc.Interface.Db;
using Npgsql;
using NpgsqlTypes;

namespace Msc.Db.Postgres
{
    /// <summary>
    /// Чтение из Postgres
    /// </summary>
    public class PgDbBulkReader : IBulkReader
    {
        private readonly NpgsqlBinaryExporter _reader;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="reader">Читатель</param>
        internal PgDbBulkReader(NpgsqlBinaryExporter reader)
        {
            _reader = reader;
        }

        /// <summary>
        /// Чтение строки
        /// </summary>
        /// <returns>Результат</returns>
        public int StartRow()
        {
            return _reader.StartRow();
        }

        /// <summary>
        /// Чтение текущего столбца
        /// </summary>
        /// <typeparam name="T">Тип</typeparam>
        /// <returns>Результат</returns>
        public T Read<T>()
        {
            return _reader.Read<T>();
        }

        /// <summary>
        /// Чтение текущего столбца с приведением типа
        /// </summary>
        /// <typeparam name="T">Тип результата</typeparam>
        /// <param name="dbType">Тип БД</param>
        /// <returns>Значение</returns>
        public T Read<T>(DbType dbType)
        {
            switch (dbType)
            {
                case DbType.Boolean:
                    return _reader.Read<T>(NpgsqlDbType.Boolean);

                case DbType.Int16:
                    return _reader.Read<T>(NpgsqlDbType.Smallint);

                case DbType.Int32:
                    return _reader.Read<T>(NpgsqlDbType.Integer);

                case DbType.Int64:
                    return _reader.Read<T>(NpgsqlDbType.Bigint);

                case DbType.Single:
                    return _reader.Read<T>(NpgsqlDbType.Real);

                case DbType.Double:
                    return _reader.Read<T>(NpgsqlDbType.Double);

                case DbType.Decimal:
                case DbType.VarNumeric:
                    return _reader.Read<T>(NpgsqlDbType.Numeric);

                case DbType.AnsiString:
                case DbType.AnsiStringFixedLength:
                case DbType.String:
                case DbType.StringFixedLength:
                    return _reader.Read<T>(NpgsqlDbType.Text);

                case DbType.Date:
                    return _reader.Read<T>(NpgsqlDbType.Date);

                case DbType.DateTime:
                case DbType.DateTime2:
                    return _reader.Read<T>(NpgsqlDbType.Timestamp);

                case DbType.DateTimeOffset:
                    return _reader.Read<T>(NpgsqlDbType.TimestampTz);

                case DbType.Time:
                    return _reader.Read<T>(NpgsqlDbType.Time);
                /*
                                case DbType.Binary:
                                    break;
                                case DbType.Byte:
                                    break;
                                case DbType.Currency:
                                    break;
                                case DbType.Guid:
                                    break;
                                case DbType.Object:
                                    break;
                                case DbType.SByte:
                                    break;
                                case DbType.UInt16:
                                    break;
                                case DbType.UInt32:
                                    break;
                                case DbType.UInt64:
                                    break;
                                case DbType.Xml:
                                    break;
                                    */
                default:
                    throw new ArgumentOutOfRangeException(nameof(dbType), dbType, null);
            }
        }

        /// <summary>
        /// Отмена чтения
        /// </summary>
        public void Cancel()
        {
            _reader.Cancel();
        }

        /// <summary>
        /// Освобождение ресурса
        /// </summary>
        public void Dispose()
        {
            _reader?.Dispose();
        }
    }
}

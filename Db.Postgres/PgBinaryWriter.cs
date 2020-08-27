// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PgBinaryWriter.cs" company="Konyshev A.V., Stepanov N.O.">
//   Copyright (c) Konyshev A.V., Stepanov N.O.
// </copyright>
// <summary>
//   Класс выполняющий запись в бинарный файл, представляет обертку над NpgSQl
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
    /// Класс выполняющий запись в бинарный файл, представляет обертку над NpgSQl
    /// </summary>
    public class PgBinaryWriter : IBinaryWriter
    {
        private NpgsqlBinaryImporter _binaryImporter;

        /// <summary>
        /// Конструктор обертки
        /// </summary>
        /// <param name="binaryImporter">Импортер</param>
        public PgBinaryWriter(NpgsqlBinaryImporter binaryImporter)
        {
            _binaryImporter = binaryImporter ?? throw new ArgumentNullException(nameof(binaryImporter));
        }

        #region Write

        /// <summary>
        /// Starts writing a single row, must be invoked before writing any columns.
        /// </summary>
        public void StartRow()
        {
            _binaryImporter.StartRow();
        }

        /// <summary>
        /// Writes a single column in the current row.
        /// </summary>
        /// <param name="value">The value to be written</param>
        /// <typeparam name="T">
        /// The type of the column to be written. This must correspond to the actual type or data
        /// corruption will occur. If in doubt, use <see cref="Write{T}(T, NpgsqlDbType)"/> to manually
        /// specify the type.
        /// </typeparam>
        public void Write<T>(T value)
        {
            _binaryImporter.Write<T>(value);
        }

        /// <summary>
        /// Writes a single column in the current row as type <paramref name="npgsqlDbType"/>.
        /// </summary>
        /// <param name="value">The value to be written</param>
        /// <param name="dbType">
        /// In some cases <typeparamref name="T"/> isn't enough to infer the data type to be written to
        /// the database. This parameter and be used to unambiguously specify the type. An example is
        /// the JSONB type, for which <typeparamref name="T"/> will be a simple string but for which
        /// <paramref name="dbType"/> must be specified as <see cref="DbType"/>.
        /// </param>
        /// <typeparam name="T">The .NET type of the column to be written.</typeparam>
        public void Write<T>(T value, DbType dbType)
        {
            var npgType = ToNpgType(dbType);
            _binaryImporter.Write<T>(value, npgType);
        }

        /// <summary>
        /// Writes a single column in the current row as type <paramref name="dataTypeName"/>.
        /// </summary>
        /// <param name="value">The value to be written</param>
        /// <param name="dataTypeName">
        /// In some cases <typeparamref name="T"/> isn't enough to infer the data type to be written to
        /// the database. This parameter and be used to unambiguously specify the type.
        /// </param>
        /// <typeparam name="T">The .NET type of the column to be written.</typeparam>
        public void Write<T>(T value, string dataTypeName)
        {
            _binaryImporter.Write<T>(value, dataTypeName);
        }

        /// <summary>
        /// Writes a single null column value.
        /// </summary>
        public void WriteNull()
        {
            _binaryImporter.WriteNull();
        }

        /// <summary>
        /// Writes an entire row of columns.
        /// Equivalent to calling <see cref="StartRow"/>, followed by multiple <see cref="Write{T}(T)"/>
        /// on each value.
        /// </summary>
        /// <param name="values">An array of column values to be written as a single row</param>
        public void WriteRow(params object[] values)
        {
            _binaryImporter.WriteRow(values);
        }

        #endregion

        #region Commit / Cancel / Close / Dispose

        /// <summary>
        /// Completes the import operation. The writer is unusable after this operation.
        /// </summary>
        public void Complete()
        {
            _binaryImporter.Complete();
        }

        /// <summary>
        /// Completes that binary import and sets the connection back to idle state
        /// </summary>
        public void Dispose() => Close();

        /// <summary>
        /// Completes the import process and signals to the database to write everything.
        /// </summary>
        public void Close()
        {
            _binaryImporter.Close();
        }

        #endregion

        private NpgsqlDbType ToNpgType(DbType type)
        {
            switch (type)
            {
                case DbType.Date: return NpgsqlDbType.Date;
                case DbType.AnsiString: return NpgsqlDbType.Text;
                case DbType.AnsiStringFixedLength: return NpgsqlDbType.Varchar;
                case DbType.Currency: return NpgsqlDbType.Money;
                case DbType.Int64: return NpgsqlDbType.Bigint;
                case DbType.Boolean: return NpgsqlDbType.Boolean;
                case DbType.String: return NpgsqlDbType.Text;
                case DbType.Binary: return NpgsqlDbType.Bytea;
                case DbType.DateTime: return NpgsqlDbType.Timestamp;
                case DbType.DateTime2: return NpgsqlDbType.Date;
                case DbType.Single: return NpgsqlDbType.Real;
                case DbType.Double: return NpgsqlDbType.Double;
                case DbType.Int16: return NpgsqlDbType.Smallint;
                case DbType.Int32: return NpgsqlDbType.Integer;
                case DbType.Decimal: return NpgsqlDbType.Numeric;
                case DbType.Time: return NpgsqlDbType.Time;
                case DbType.Guid: return NpgsqlDbType.Uuid;
                case DbType.Xml: return NpgsqlDbType.Xml;
                default: throw new NotSupportedException($"Type {type} not supported");
            }
        }
    }
}

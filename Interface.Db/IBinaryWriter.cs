using System;
using System.Data;

namespace Msc.Interface.Db
{
    /// <summary>
    /// Выполняет бинарную запись для СУБД 
    /// </summary>
    public interface IBinaryWriter : IDisposable
    {
        /// <summary>
        /// Starts writing a single row, must be invoked before writing any columns.
        /// </summary>
        void StartRow();

        /// <summary>
        /// Writes a single column in the current row.
        /// </summary>
        /// <param name="value">The value to be written</param>
        /// <typeparam name="T">
        /// The type of the column to be written. This must correspond to the actual type or data
        /// corruption will occur. If in doubt, use <see cref="Write{T}(T, NpgsqlDbType)"/> to manually
        /// specify the type.
        /// </typeparam>
        void Write<T>(T value);

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
        void Write<T>(T value, DbType dbType);

        /// <summary>
        /// Writes a single column in the current row as type <paramref name="dataTypeName"/>.
        /// </summary>
        /// <param name="value">The value to be written</param>
        /// <param name="dataTypeName">
        /// In some cases <typeparamref name="T"/> isn't enough to infer the data type to be written to
        /// the database. This parameter and be used to unambiguously specify the type.
        /// </param>
        /// <typeparam name="T">The .NET type of the column to be written.</typeparam>
        void Write<T>(T value, string dataTypeName);

        /// <summary>
        /// Writes a single null column value.
        /// </summary>
        void WriteNull();

        /// <summary>
        /// Writes an entire row of columns.
        /// Equivalent to calling <see cref="StartRow"/>, followed by multiple <see cref="Write{T}(T)"/>
        /// on each value.
        /// </summary>
        /// <param name="values">An array of column values to be written as a single row</param>
        void WriteRow(params object[] values);



        /// <summary>
        /// Completes the import operation. The writer is unusable after this operation.
        /// </summary>
        void Complete();


        /// <summary>
        /// Completes that binary import and sets the connection back to idle state
        /// </summary>
        void Dispose();

        /// <summary>
        /// Completes the import process and signals to the database to write everything.
        /// </summary>
        void Close();
    }
}

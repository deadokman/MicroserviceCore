using System;
using System.Collections.Generic;

namespace Msc.Interface.Db
{
    public interface IDbConnection : System.Data.IDbConnection
    {
        /// <summary>
        /// Сохранение множества записей
        /// </summary>
        /// <param name="sql">Выполняемый запрос</param>
        /// <param name="enumerator">Енумератор-перечислятор ☺</param>
        /// <param name="mapFunction">Преобразование сущности в массив свойств</param>
        /// <typeparam name="T">Тип сохраняемой сущности</typeparam>
        void BulkCopy<T>(string sql, IEnumerator<T> enumerator, Func<T, object[]> mapFunction);

        /// <summary>
        /// Сохранение множества записей
        /// </summary>
        /// <param name="sql">Выполняемый запрос</param>
        /// <param name="enumerator">Енумератор-перечислятор ☺</param>
        /// <param name="writerDelegate">Делегат для записи в бинарный поток</param>
        /// <typeparam name="T">Тип сохраняемой сущности</typeparam>
        void BulkCopy<T>(string sql, IEnumerator<T> enumerator, Action<T, IBinaryWriter> writerDelegate);

        /// <summary>
        /// Выполнить вставку данных из DataRaader
        /// </summary>
        /// <param name="sql">Sql запрос для импортируемых данных</param>
        /// <returns>Интерфейс бинарной записи данных</returns>
        IBinaryWriter BeginBinaryImport(string sql);

        /// <summary>
        /// Вернуть DbType даты для конкретной реализации БД
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        object GetDateByDbType(DateTime dt);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        IBulkReader BulkLoad(string sql);
    }
}
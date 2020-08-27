using System;
using System.Data;

namespace Msc.Interface.Db
{
    /// <summary>
    /// Интерфейс бинарного загрузчика
    /// </summary>
    public interface IBulkReader : IDisposable
    {
        int StartRow();

        T Read<T>();

        T Read<T>(DbType dbType);

        void Cancel();
    }
}

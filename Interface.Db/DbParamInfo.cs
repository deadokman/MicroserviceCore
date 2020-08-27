using System.Data;

namespace Msc.Interface.Db
{
    /// <summary>
    /// Содержит информацию для создания параметров запроса
    /// </summary>
    public class DbParamInfo
    {
        public string Name { get; set; }
        public DbType Type { get; set; }
        public object Value { get; set; }

        public DbParamInfo(string name, DbType type, object value)
        {
            Name = name;
            Type = type;
            Value = value;
        }

        public DbParamInfo(string name, DbType type)
        {
            Name = name;
            Type = type;
        }
    }
}

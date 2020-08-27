using System.Data;

namespace Msc.Interface.Db
{
    public static class DbReaderExtension
    {
        public static decimal? ReadDecimalN(this IDataReader reader, int idx)
        {
            return reader.IsDBNull(idx)
                ? (decimal?)null
                : reader.GetDecimal(idx);
        }

        public static int? ReadIntN(this IDataReader reader, int idx)
        {
            return reader.IsDBNull(idx)
                ? (int?)null
                : reader.GetInt32(idx);
        }

        public static bool ReadBool(this IDataReader reader, int idx)
        {
            return !reader.IsDBNull(idx) && reader.GetBoolean(idx);
        }

        public static bool? ReadBoolN(this IDataReader reader, int idx)
        {
            return reader.IsDBNull(idx)
                ? (bool?)null
                : reader.GetBoolean(idx);
        }
    }
}

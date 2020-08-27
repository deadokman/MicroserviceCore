using Msc.Interface.Db;
using System.Data;

namespace Msc.Interface.Db
{
    public static class DbConnectionExtension
    {
        public static IDataReader GetReader(this IDbConnection con, string query, params DbParamInfo[] pars)
        {
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = query;

                if (pars != null)
                {
                    foreach (var info in pars)
                    {
                        var par = cmd.CreateParameter();
                        par.ParameterName = info.Name;
                        par.DbType = info.Type;
                        par.Value = info.Value;
                        cmd.Parameters.Add(par);
                    }
                }

                return cmd.ExecuteReader();
            }
        }

        public static IDbCommand GetStoredProcedure(this IDbConnection con, string name, params DbParamInfo[] pars)
        {
            var cmd = con.CreateCommand();
            cmd.CommandText = name;
            cmd.CommandType = CommandType.StoredProcedure;

            if (pars != null)
            {
                foreach (var info in pars)
                {
                    var par = cmd.CreateParameter();
                    par.ParameterName = info.Name;
                    par.DbType = info.Type;
                    par.Value = info.Value;
                    cmd.Parameters.Add(par);
                }
            }

            return cmd;
        }
    }
}
using Msc.Interface.Db;
using System;

namespace Msc.Interface.Db
{
    public static class DbContextExtension
    {
        public static T Execute<T>(this IDbContext db, IDbConnection con, Func<IDbConnection, T> action)
        {
            var b = con == null;
            if (b)
            {
                con = db.GetConnection();
            }

            try
            {
                return action(con);
            }
            finally
            {
                if (b)
                {
                    con.Dispose();
                }
            }
        }

        public static void Execute(this IDbContext db, IDbConnection con, Action<IDbConnection> action)
        {
            var b = con == null;
            if (b)
            {
                con = db.GetConnection();
            }

            try
            {
                action(con);
            }
            finally
            {
                if (b)
                {
                    con.Dispose();
                }
            }
        }
    }
}

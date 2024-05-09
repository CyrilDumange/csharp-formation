using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace fizzbuzz.dal.Migrations
{
    public abstract class FizzHistoryMigration
    {
        public static Task Apply(IDbConnection conn)
        {
            return conn.ExecuteAsync("CREATE TABLE fizz_history(input TEXT PRIMARY KEY, count int);");
        }
    }
}
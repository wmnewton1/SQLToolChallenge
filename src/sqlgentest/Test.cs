using System;
using SqlGen.Contracts;
using SqlGen.Core;

using Microsoft.FSharp.Core;

namespace SqlGenTest
{
    internal static class Program
    {
        private static void Main()
        {
            Table tbl = new Table("Event");

            Field date = new Field("Date", null, tbl);
            Field location = new Field("Location", null, tbl);

            FSharpList<Field> fields = new FSharpList<Field>{
                date,
                location
            };

            Join join = Join.InnerJoin;

            Condition condition = new Condition(
                date, "=", DateTime.Now
            );

            JoinClause joinClause = new JoinClause(
                Join.InnerJoin,
                tbl,
                condition
            );

            FSharpList<JoinClause> joinClauses = new FSharpList<JoinClause>{
                joinClause
            };

            String sql = SqlGen.generateSql(tbl, fields, joinClauses);
            Console.WriteLine(sql);
        }
    }
}

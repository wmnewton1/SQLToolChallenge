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
            Table event = new Table("Event");

            Field date = new Field("Date", null, event);
            Field location = new Field("Location", null, event);

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
                event,
                condition
            );

            FSharpList<JoinClause> joinClauses = new FSharpList<JoinClause>{
                joinClause
            };

            String sql = SqlGen.generateSql(event, fields, joinClauses);
            Console.WriteLine(sql);
        }
    }
}

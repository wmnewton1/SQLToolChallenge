using System;
using System.Collections.Generic;
using SqlGen.Contracts;
using SqlGen.Core;

using Microsoft.FSharp.Collections;

namespace SqlGenTest
{
    internal static class Program
    {
        private static void Main()
        {
            Table tbl = new Table("Event", "Event");

            Field date = new Field("Date", null, tbl);
            Field location = new Field("Location", null, tbl);

            FSharpList<Field> fields = ListModule.OfSeq(new List<Field> { date, location });

            Join join = Join.InnerJoin;

            Condition condition = new Condition(
                date, "=", DateTime.Now
            );

            JoinClause joinClause = new JoinClause(
                Join.InnerJoin,
                tbl,
                condition
            );

            FSharpList<JoinClause> joinClauses = ListModule.OfSeq(new List<JoinClause> { joinClause });

            String sql = SqlGen.generateSql(tbl, fields, joinClauses);
            Console.WriteLine(sql);
        }
    }
}

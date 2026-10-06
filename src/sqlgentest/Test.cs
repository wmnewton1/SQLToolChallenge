using System;
using System.Collections.Generic;
using SqlGen.Contracts;
using SqlGen.Core;
using SqlGen.TreeUtils;

using Microsoft.FSharp.Core;
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

            condition = new Condition(location, "=", "N1 6NU");
            Core.ClauseComponent component = Core.ClauseComponent.NewCon(condition);
            Node<Core.ClauseComponent> rootNode = new Node<Core.ClauseComponent>(
                component,
                FSharpOption<Node<Core.ClauseComponent>>.None,
                FSharpOption<Node<Core.ClauseComponent>>.None
            );

            String sql = Core.generateSql(tbl, fields, joinClauses, rootNode);
            Console.WriteLine(sql);
        }
    }
}

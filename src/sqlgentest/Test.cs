using System;
using SqlGen.Interfaces;
using SqlGen.Core;

using Microsoft.FSharp.Core;

namespace SqlGenTest
{
    internal static class Program
    {
        private static void Main()
        {
            Table table = new Table("Event");

            Field date = new Field("Date", null, Table);
            Field location = new Field("Location", null, Table);

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
                table,
                condition
            );

            FSharpList<JoinClause> joinClauses = new FSharpList<JoinClause>{
                joinClause
            };

            String sql = SqlGen.generateSql(table, fields, joinClauses);
            Console.WriteLine(sql);
        }
    }

    public class Table : Queryable
    {
        public string Name { get; set; }
        public FSharpOption<string> Alias { get; set; }

        public Table(string name) {
            this.Name = name;
        }

        public Table(string name, FSharpOption<string> alias) {
            this.Name = name;
            this.Alias = alias;
        }
    }
}

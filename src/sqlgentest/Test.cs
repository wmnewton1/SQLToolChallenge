using System;
using System.Collections.Generic;
using SqlGen.Types;
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
            Table events = new Table("event", "event");
            Table eventAttendees = new Table("event_attendee", "event_attendee");

            Field eventId = new Field("id", null, events);
            Field date = new Field("date", null, events);
            Field location = new Field("location", null, events);

            FSharpList<Field> fields = ListModule.OfSeq(new List<Field> { eventId, date, location });

            Join join = Join.InnerJoin;

            Field eventAttendee = new Field("id", null, eventAttendees);

            Condition condition = new Condition(
                eventId, "=", eventAttendee
            );

            JoinClause joinClause = new JoinClause(
                Join.InnerJoin,
                events,
                condition
            );

            FSharpList<JoinClause> joinClauses = ListModule.OfSeq(new List<JoinClause> { joinClause });

            condition = new Condition(location, "=", "N1 6NU");
            ClauseComponent component = ClauseComponent.NewCon(condition);
            Node<ClauseComponent> rootNode = new Node<ClauseComponent>(
                component,
                FSharpOption<Node<ClauseComponent>>.None,
                FSharpOption<Node<ClauseComponent>>.None
            );

            String sql = Core.generateSql(events, fields, joinClauses, rootNode);
            Console.WriteLine(sql);
        }
    }
}

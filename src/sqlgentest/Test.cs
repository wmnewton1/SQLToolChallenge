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
        private static ClauseComponent OR_OPERATOR = ClauseComponent.NewLogOp(LogicalOperator.Or);
        private static ClauseComponent AND_OPERATOR = ClauseComponent.NewLogOp(LogicalOperator.And);

        private static void Main()
        {
            Console.WriteLine(generateExampleSql1());
            Console.WriteLine(generateExampleSql2());
        }

        private static string generateExampleSql1() {
            Table events = new Table("event", "Event Alias");

            Field eventId = new Field("id", "Id Alias", events);
            Field date = new Field("date", null, events);
            Field location = new Field("location", null, events);

            FSharpList<Field> fields = ListModule.OfSeq(new List<Field> { eventId, date, location });

            JoinClause innerJoinClause = new JoinClause(
                Join.InnerJoin,
                events,
                getComplexTree(eventId)
            );

            JoinClause leftJoinClause = new JoinClause(
                Join.LeftJoin,
                events,
                getComplexTree(date)
            );

            FSharpList<JoinClause> joinClauses = ListModule.OfSeq(new List<JoinClause> { innerJoinClause, leftJoinClause });

            return Core.generateSql(events, fields, joinClauses, getComplexTree(location));
        }

        private static string generateExampleSql2() {
            Table events = new Table("Events", null);
            Table eventAttendee = new Table("EventAttendees", null);
            Table attendee = new Table("Attendee", null);

            Field eventId = new Field("id", null, events);
            Field eventAttendeeId = new Field("id", null, eventAttendee);
            Field attendeeId = new Field("id", null, attendee);

            FSharpList<Field> fields = ListModule.OfSeq(new List<Field> { eventId });

            ClauseComponent eventsToEventAttendee = ClauseComponent.NewCon(new Condition(eventId, "=", eventAttendeeId));
            ClauseComponent eventAttendeeToAttendee = ClauseComponent.NewCon(new Condition(eventAttendeeId, "=", attendeeId));

            JoinClause innerJoin1 = new JoinClause(
                Join.InnerJoin,
                eventAttendee,
                new Node<ClauseComponent>(
                    eventsToEventAttendee,
                    FSharpOption<Node<ClauseComponent>>.None,
                    FSharpOption<Node<ClauseComponent>>.None
                )
            );
            
            JoinClause innerJoin2 = new JoinClause(
                Join.InnerJoin,
                eventAttendee,
                new Node<ClauseComponent>(
                    eventAttendeeToAttendee,
                    FSharpOption<Node<ClauseComponent>>.None,
                    FSharpOption<Node<ClauseComponent>>.None
                )
            );

            FSharpList<JoinClause> joinClauses = ListModule.OfSeq(new List<JoinClause> { innerJoin1, innerJoin2 });
            
            Field important = new Field("Important", null, events);
            Field attendeeName = new Field("Name", null, attendee);

            return Core.generateSql(events, fields, joinClauses, getBasicTree(important, attendeeName));
        }

        private static Node<ClauseComponent> getBasicTree(Field important, Field attendeeName) {
            Condition leftCondition = new Condition(attendeeName, "=", "bob");
            Condition rightCondition = new Condition(important, "=", 1);

            ClauseComponent leftConditionCmp = ClauseComponent.NewCon(leftCondition);
            ClauseComponent rightConditionCmp = ClauseComponent.NewCon(rightCondition);
            
            Node<ClauseComponent> leftLeaf = new Node<ClauseComponent>(
                leftConditionCmp,
                FSharpOption<Node<ClauseComponent>>.None,
                FSharpOption<Node<ClauseComponent>>.None
            );

            Node<ClauseComponent> rightLeaf = new Node<ClauseComponent>(
                rightConditionCmp,
                FSharpOption<Node<ClauseComponent>>.None,
                FSharpOption<Node<ClauseComponent>>.None
            );

            Node<ClauseComponent> rootNode = new Node<ClauseComponent>(
                OR_OPERATOR,
                leftLeaf,
                rightLeaf
            );

            return rootNode;
        }

        private static Node<ClauseComponent> getComplexTree(Field field) {
            Condition leftCondition = new Condition(field, "=", "LEFT NODE");
            Condition rightCondition = new Condition(field, "=", "RIGHT NODE");

            ClauseComponent leftConditionCmp = ClauseComponent.NewCon(leftCondition);
            ClauseComponent rightConditionCmp = ClauseComponent.NewCon(rightCondition);

            Node<ClauseComponent> leftLeftLeaf = new Node<ClauseComponent>(
                leftConditionCmp,
                FSharpOption<Node<ClauseComponent>>.None,
                FSharpOption<Node<ClauseComponent>>.None
            );
            
            Node<ClauseComponent> leftRightLeaf = new Node<ClauseComponent>(
                rightConditionCmp,
                FSharpOption<Node<ClauseComponent>>.None,
                FSharpOption<Node<ClauseComponent>>.None
            );

            Node<ClauseComponent> leftNode = new Node<ClauseComponent>(
                AND_OPERATOR,
                leftLeftLeaf,
                leftRightLeaf
            );

            Node<ClauseComponent> rightLeaf = new Node<ClauseComponent>(
                rightConditionCmp,
                FSharpOption<Node<ClauseComponent>>.None,
                FSharpOption<Node<ClauseComponent>>.None
            );

            Node<ClauseComponent> rootNode = new Node<ClauseComponent>(
                OR_OPERATOR,
                leftNode,
                rightLeaf
            );

            return rootNode;
        }
    }
}

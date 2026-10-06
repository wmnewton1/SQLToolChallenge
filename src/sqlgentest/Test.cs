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
            Table events = new Table("event", "Event Alias");
            Table eventAttendees = new Table("event_attendee", null);

            Field eventId = new Field("id", "Id Alias", events);
            Field date = new Field("date", null, events);
            Field location = new Field("location", null, events);

            FSharpList<Field> fields = ListModule.OfSeq(new List<Field> { eventId, date, location });

            Join join = Join.InnerJoin;

            Field eventAttendee = new Field("id", null, eventAttendees);

            JoinClause innerJoinClause = new JoinClause(
                Join.InnerJoin,
                events,
                getBasicTree(eventId)
            );

            JoinClause leftJoinClause = new JoinClause(
                Join.LeftJoin,
                events,
                getBasicTree(date)
            );

            FSharpList<JoinClause> joinClauses = ListModule.OfSeq(new List<JoinClause> { innerJoinClause, leftJoinClause });

            String sql = Core.generateSql(events, fields, joinClauses, getBasicTree(location));
            Console.WriteLine(sql);
        }

        private static Node<ClauseComponent> getBasicTree(Field field) {
            Condition leftCondition = new Condition(field, "=", "LEFT NODE");
            Condition rightCondition = new Condition(field, "=", "RIGHT NODE");

            ClauseComponent leftConditionCmp = ClauseComponent.NewCon(leftCondition);
            ClauseComponent rightConditionCmp = ClauseComponent.NewCon(rightCondition);
            
            ClauseComponent andOperatorCmp = ClauseComponent.NewLogOp(LogicalOperator.And);
            ClauseComponent orOperatorCmp = ClauseComponent.NewLogOp(LogicalOperator.Or);

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
                andOperatorCmp,
                leftLeftLeaf,
                leftRightLeaf
            );

            Node<ClauseComponent> rightLeaf = new Node<ClauseComponent>(
                rightConditionCmp,
                FSharpOption<Node<ClauseComponent>>.None,
                FSharpOption<Node<ClauseComponent>>.None
            );

            Node<ClauseComponent> rootNode = new Node<ClauseComponent>(
                orOperatorCmp,
                leftNode,
                rightLeaf
            );

            return rootNode;
        }
    }
}

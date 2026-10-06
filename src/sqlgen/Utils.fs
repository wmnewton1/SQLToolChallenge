namespace SqlGen.Utils

module Utils =
    open SqlGen.Types
    open SqlGen.TreeUtils

    let resolveField(field: Field) : string =
        sprintf "%s.%s" field.Table.Name field.Name

    let resolveValue(value: obj) =
        match value with
        | :? Field as field -> sprintf "%s" (resolveField field)
        | _ -> sprintf "%A" (value)

    let resolveCondition(condition: Condition) : string =
        sprintf "%s %s %s" (resolveField condition.Field) (condition.Operator) (resolveValue condition.Value)

    let resolveOperator = function
        | LogicalOperator.And -> "AND"
        | LogicalOperator.Or -> "OR"

    let resolveNode (node: Node<ClauseComponent>) : string =
        let nodeValue = node.Value;

        match nodeValue with
        | Con condition -> resolveCondition condition
        | LogOp operator -> resolveOperator operator

    let resolveJoin = function
        | Join.InnerJoin -> "INNER JOIN"
        | Join.FullJoin -> "FULL JOIN"
        | Join.LeftJoin -> "LEFT JOIN"
        | Join.RightJoin -> "RIGHT JOIN"

    // TODO implement tree here as well, because join condition can have AND OR etc.
    let resolveJoins(joining: JoinClause list) =
        let joins = ResizeArray<string>()

        for join in joining do
            let joinStr = sprintf "%s %s ON %s" (resolveJoin(join.Join)) (join.Table.Name) (resolveCondition(join.Condition))
            joins.Add(joinStr)

        String.concat " " joins
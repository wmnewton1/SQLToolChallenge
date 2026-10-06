namespace SqlGen.Utils

module Utils =
    open SqlGen.TreeUtils
    open SqlGen.Types

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

    let initQuery(table: Table) (columns: Field list): string =
        let columnsArr = ResizeArray<string>()

        for column in columns do
            columnsArr.Add(resolveField column)

        sprintf "SELECT %s FROM %s" (String.concat ", " columnsArr) (table.Name)

    let rec evaluateChildren(node: Node<ClauseComponent>): string =
        match node.Left, node.Right with
        | Some leftNode, Some rightNode ->
            let leftOperand = evaluateChildren(leftNode)
            let rightOperand = evaluateChildren(rightNode)

            sprintf "(%s %s %s)" (leftOperand) (resolveNode node) (rightOperand)
        | None, None ->
            resolveNode node
        | Some leftNode, None ->
            failwith "Each node must have two children."
        | None, Some rightNode ->
            failwith "Each node must have two children."

    let resolveTree(rootNode: Node<ClauseComponent>): string =
        let query = resolveNode rootNode

        match rootNode.Left, rootNode.Right with
        | None, None ->
            query
        | _ -> evaluateChildren(rootNode)

    let resolveJoins(joining: JoinClause list) =
        let joins = ResizeArray<string>()

        for join in joining do
            let joinStr = sprintf "%s %s ON %s" (resolveJoin(join.Join)) (join.Table.Name) (resolveTree(join.Conditions))
            joins.Add(joinStr)

        String.concat " " joins
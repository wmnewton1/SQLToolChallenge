namespace SqlGen.Utils

module Utils =
    open SqlGen.TreeUtils
    open SqlGen.Types

    let resolveField(field: Field) (withAlias: bool) : string =
        let column = sprintf "%s.%s" field.Table.Name field.Name

        if withAlias = false then
            column
        else
            match field.Alias with
            | Some alias ->
                sprintf "%s AS \"%s\"" (column) (alias)
            | None ->
                column

    let resolveValue(value: obj) =
        match value with
        | :? Field as field -> sprintf "%s" (resolveField field false)
        | :? string as field -> sprintf "\'%s\'" field // don't want double quotes here
        | _ -> sprintf "%A" (value)

    let resolveCondition(condition: Condition) : string =
        sprintf "%s %s %s" (resolveField condition.Field false) (condition.Operator) (resolveValue condition.Value)

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
            columnsArr.Add(resolveField column true)

        let query = sprintf "SELECT %s FROM %s" (String.concat ", " columnsArr) (table.Name)
        
        match table.Alias with
        | Some alias ->
            sprintf "%s AS \"%s\"" (query) (alias)
        | None ->
            query

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

    let appendEnd(query: string) =
        sprintf "%s%s" query ";"
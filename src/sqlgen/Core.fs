namespace SqlGen.Core

module Core =

    open SqlGen.Contracts
    open SqlGen.TreeUtils

    // to compile, run
    // dotnet build src/sqlgen/Repositories.fsproj

    let getJoin = function
        | Join.InnerJoin -> "INNER JOIN"
        | Join.FullJoin -> "FULL JOIN"
        | Join.LeftJoin -> "LEFT JOIN"
        | Join.RightJoin -> "RIGHT JOIN"

    type LogicalOperator =
        | And
        | Or

    let getOperator = function
        | LogicalOperator.And -> "AND"
        | LogicalOperator.Or -> "OR"

    type ClauseComponent =
        | Con of Condition
        | LogOp of LogicalOperator

    let resolveField(field: Field) : string =
        sprintf "%s.%s" field.Table.Name field.Name

    let resolveCondition(condition: Condition) : string =
        sprintf "%s %s %s" (resolveField condition.Field) (condition.Operator) (string condition.Value)

    let handleNodeValue (nodeValue: ClauseComponent) : string =
        match nodeValue with
        | Con condition -> resolveCondition condition
        | LogOp operator -> getOperator operator

    let resolveJoins(joining: JoinClause list) =
        let joins = ResizeArray<string>()

        for join in joining do
            let joinStr = sprintf "%s %s ON %s" (getJoin(join.Join)) (join.Table.Name) (resolveCondition(join.Condition))
            joins.Add(joinStr)

        String.concat " " joins    

    let rec evaluateNode(node: Node<ClauseComponent>, querySoFar: string): string =
        match node.Left, node.Right with
        | None, None ->
            // node is a leaf, therefore a condition
            let condition = handleNodeValue node.Value
            sprintf "%s %s" querySoFar condition
        | Some leftNode, None ->
            sprintf "%s %s" (querySoFar) (string leftNode.Value)
            evaluateNode(leftNode, querySoFar)
        | None, Some rightNode ->
            sprintf "%s %s" (querySoFar) (string rightNode.Value)
            evaluateNode(rightNode, querySoFar)

    let resolveWhere(rootNode: Node<ClauseComponent>): string =
        evaluateNode(rootNode, "")

    let generateSql(table: Table) (columns: Field list) (joining: JoinClause list option) (whereRootNode: Node<ClauseComponent> option): string =
        if (columns.IsEmpty) then
            failwith "At least one column must be specified."

        let comma = ", "

        let columnsArr = ResizeArray<string>()

        for column in columns do
            columnsArr.Add(resolveField column)

        let query = sprintf "SELECT %s FROM %s" (String.concat comma columnsArr) (table.Name)

        match joining, whereRootNode with
        | None, None ->
            query
        | None, Some whereRootNode ->
            let whereClause = resolveWhere(whereRootNode)
            sprintf "%s %s" query whereClause
        | Some joining, None ->
            let joins = resolveJoins(joining)
            sprintf "%s %s" query joins
        | Some joining, Some whereRootNode ->
            let joins = resolveJoins(joining)
            let whereClause = resolveWhere(whereRootNode)
            sprintf "%s %s WHERE %s" query joins whereClause
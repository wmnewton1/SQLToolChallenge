namespace SqlGen.Core

module Core =
    open SqlGen.TreeUtils
    open SqlGen.Types
    open SqlGen.Utils.Utils  

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
            let whereClause = resolveTree(whereRootNode)
            sprintf "%s WHERE %s" query whereClause
        | Some joining, None ->
            let joins = resolveJoins(joining)
            sprintf "%s %s" query joins
        | Some joining, Some whereRootNode ->
            let joins = resolveJoins(joining)
            let whereClause = resolveTree(whereRootNode)
            sprintf "%s %s WHERE %s" query joins whereClause
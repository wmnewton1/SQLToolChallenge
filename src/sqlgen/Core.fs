namespace SqlGen.Core

module Core =

    open SqlGen.Types
    open SqlGen.TreeUtils
    open SqlGen.Utils.Utils  

    let rec evaluateNode(node: Node<ClauseComponent>, querySoFar: string): string =
        match node.Left, node.Right with
        | None, None ->
            // node is a leaf, therefore a condition
            let condition = handleNodeValue node.Value
            sprintf "%s %s" querySoFar condition
        | Some leftNode, None ->
            evaluateNode(leftNode, sprintf "%s %s" (querySoFar) (handleNodeValue leftNode.Value))
        | None, Some rightNode ->
            evaluateNode(rightNode, sprintf "%s %s" (querySoFar) (handleNodeValue rightNode.Value))
        | Some leftNode, Some rightNode ->
            evaluateNode(leftNode, sprintf "%s %s" (querySoFar) (handleNodeValue leftNode.Value))
            evaluateNode(rightNode, sprintf "%s %s" (querySoFar) (handleNodeValue rightNode.Value))

    let resolveWhere(rootNode: Node<ClauseComponent>): string =
        let mutable query = ""

        evaluateNode(rootNode, query)

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
            sprintf "%s WHERE %s" query whereClause
        | Some joining, None ->
            let joins = resolveJoins(joining)
            sprintf "%s %s" query joins
        | Some joining, Some whereRootNode ->
            let joins = resolveJoins(joining)
            let whereClause = resolveWhere(whereRootNode)
            sprintf "%s %s WHERE %s" query joins whereClause
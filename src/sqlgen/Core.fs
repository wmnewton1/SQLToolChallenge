namespace SqlGen.Core

module Core =
    open SqlGen.Types
    open SqlGen.TreeUtils
    open SqlGen.Utils.Utils  

    let rec evaluateChildren(node: Node<ClauseComponent>): string =
        match node.Left, node.Right with
        | None, None ->
            resolveNode node
        | Some leftNode, None ->
            let leftQuery = evaluateChildren(leftNode)
            sprintf "%s %s" (leftQuery) (resolveNode node)
        | None, Some rightNode ->
            let rightQuery = evaluateChildren(rightNode)
            sprintf "%s %s" (resolveNode node) (rightQuery)
        | Some leftNode, Some rightNode ->
            let leftQuery = evaluateChildren(leftNode)
            let rightQuery = evaluateChildren(rightNode)

            sprintf "(%s %s %s)" (leftQuery) (resolveNode node) (rightQuery)

    let resolveTree(rootNode: Node<ClauseComponent>): string =
        let query = resolveNode rootNode

        match rootNode.Left, rootNode.Right with
        | None, None ->
            query
        | _ -> evaluateChildren(rootNode)

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
namespace SqlGen.Core

module Core =
    open SqlGen.TreeUtils
    open SqlGen.Types
    open SqlGen.Utils.Utils

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
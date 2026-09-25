namespace sqlgen

// to compile, run
// dotnet build src/sqlgen/Repositories.fsproj

type Join =
    | InnerJoin = "INNER JOIN"
    | FullJoin = "FULL JOIN"
    | LeftJoin = "LEFT JOIN"
    | RightJoin = "RIGHT JOIN"

type LogicalOperator =
    | And = "AND"
    | Or = "OR"

type Queryable =
    abstract member Name : string
    abstract member Alias : string option

// example implementation of Queryable
// type Event() =
//     interface Queryable with
//         member this.Name = "Event"

type Field(name: string, alias: string option,table: Queryable) =
    member this.Name = name
    member this.Alias = alias
    member this.Table = table

type Condition(field: Field, operator: string, value: obj) =
    member this.Field = field
    member this.Operator = operator
    member this.Value = value

type JoinClause(join: Join, table: Queryable, condition: Condition) =
    member this.Join = join
    member this.Table = table
    member this.Condition = condition

type ClauseComponent =
    | Condition of Condition
    | LogicalOperator of LogicalOperator

let generateSql(table: Queryable) (columns: Field list) (joining: JoinClause list option) (where: Tree<ClauseComponent> option): string =
    if (columns.IsEmpty) then
        failwith "At least one column must be specified."

    let template = "SELECT %s FROM %s"
    let comma = ", "

    let columns = []

    for column in columns do
        columns <- columns :: column.Name

    let query = sprintf template (String.concat comma columns) (table.Name)

    if (joining.IsEmpty && where == null) then
        query
    elif (where.IsEmpty) then
        let joins = resolveJoins(joining)

        sprintf "%s %s" query joins
    elif (joining.IsEmpty) then
        let whereClause = resolveWhere(where)

        sprintf "%s %s" query whereClause
    else
        let joins = resolveJoins(joining)
        let whereClause = resolveWhere(where)

        sprintf "%s %s %s" query joins whereClause

    let resolveJoins(joining: JoinClause list) =
        let template = "%s %s ON %s"

        let joins = []

        for join in joining do
            let joinStr = sprintf template (string join.Join) (join.Table.Name) (resolveCondition(join.Condition))
            joins <- joins :: joinStr

        String.concat " " joins    

    let resolveCondition(condition: Condition) =
        let template = "%s %s %s"

        sprintf template (condition.Field.Name) (condition.Operator) (string condition.Value)

    let resolveWhere(where: Tree<ClauseComponent>) =
        evaluateNode(where.Root)

    let evaluateNode(node: Node<ClauseComponent>, querySoFar: string) =
        if (node.Left == null && node.Right == null) then
            // node is a leaf, therefore a condition
            sprintf "%s %s" (querySoFar) (resolveCondition(node.Value))
        elif (node.Left != null) then
            sprintf "%s %s" (querySoFar) (string node)
            evaluateNode(node.Left)
        else
            sprintf "%s %s" (querySoFar) (string node)
            evaluateNode(node.Right)
    
    ""
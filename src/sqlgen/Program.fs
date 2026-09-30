module sqlgen

open TreeUtils

// to compile, run
// dotnet build src/sqlgen/Repositories.fsproj

type Join =
    | InnerJoin
    | FullJoin
    | LeftJoin
    | RightJoin

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

let resolveCondition(condition: Condition) =
    sprintf "%s %s %s" (condition.Field.Name) (condition.Operator) (string condition.Value)

let resolveJoins(joining: JoinClause list) =
    let joins = ResizeArray<string>()

    for join in joining do
        let joinStr = sprintf "%s %s ON %s" (getJoin(join.Join)) (join.Table.Name) (resolveCondition(join.Condition))
        joins.Add(joinStr)

    String.concat " " joins    

let rec evaluateNode(node: Node<ClauseComponent>, querySoFar: string): string =
    if (node.Left == null && node.Right == null) then
        // node is a leaf, therefore a condition
        sprintf "%s %s" (querySoFar) (resolveCondition(node.Value))
    elif (node.Left != null) then
        sprintf "%s %s" (querySoFar) (string node)
        evaluateNode(node.Left)
    else
        sprintf "%s %s" (querySoFar) (string node)
        evaluateNode(node.Right)

let resolveWhere(where: Tree<ClauseComponent>): string =
    evaluateNode(where.Root)

let generateSql(table: Queryable) (columns: Field list) (joining: JoinClause list option) (where: Tree<ClauseComponent> option): string =
    if (columns.IsEmpty) then
        failwith "At least one column must be specified."

    let comma = ", "

    let columns = ResizeArray<string>()

    for column in columns do
        columns.Add(column.Name)

    let query = sprintf "SELECT %s FROM %s" (String.concat comma columns) (table.Name)

    if (joining == null && where == null) then
        query
    elif (where == null) then
        let joins = resolveJoins(joining)

        sprintf "%s %s" query joins
    elif (joining == null) then
        let whereClause = resolveWhere(where)

        sprintf "%s %s" query whereClause
    else
        let joins = resolveJoins(joining)
        let whereClause = resolveWhere(where)

        sprintf "%s %s %s" query joins whereClause
    
    ""
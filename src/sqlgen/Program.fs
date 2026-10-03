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

type Field(name: string, alias: string, table: Queryable) =
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
    | Con of Condition
    | LogOp of LogicalOperator

let resolveCondition(condition: Condition) =
    sprintf "%s %s %s" (condition.Field.Name) (condition.Operator) (string condition.Value)

let handleNodeValue (nodeValue: ClauseComponent) : string =
    match nodeValue with
    | Con condition -> resolveCondition condition
    | LogOp operator -> getOperator(operator)

let resolveJoins(joining: JoinClause list) =
    let joins = ResizeArray<string>()

    for join in joining do
        let joinStr = sprintf "%s %s ON %s" (getJoin(join.Join)) (join.Table.Name) (resolveCondition(join.Condition))
        joins.Add(joinStr)

    String.concat " " joins    

let rec evaluateNode(node: Node<ClauseComponent>, querySoFar: string): string =
    if (node.Left = null && node.Right = null) then
        // node is a leaf, therefore a condition
        let condition = handleNodeValue(node.Value)

        sprintf "%s %s" (querySoFar) (condition)
    elif (node.Left <> None) then
        sprintf "%s %s" (querySoFar) (string node)
        evaluateNode(node.Left, querySoFar)
    else
        sprintf "%s %s" (querySoFar) (string node)
        evaluateNode(node.Right, querySoFar)

let resolveWhere(rootNode: Node<ClauseComponent>): string =
    evaluateNode(rootNode, "")

let generateSql(table: Queryable) (columns: Field list) (joining: JoinClause list) (whereRootNode: Node<ClauseComponent>): string =
    if (columns.IsEmpty) then
        failwith "At least one column must be specified."

    let comma = ", "

    let columnsArr = ResizeArray<string>()

    for column in columns do
        columnsArr.Add(column.Name)

    let query = sprintf "SELECT %s FROM %s" (String.concat comma columnsArr) (table.Name)

    if (joining == null && whereRootNode == null) then
        query
    elif (whereRootNode == null) then
        let joins = resolveJoins(joining)

        sprintf "%s %s" query joins
    elif (joining == null) then
        let whereClause = resolveWhere(whereRootNode)

        sprintf "%s %s" query whereClause
    else
        let joins = resolveJoins(joining)
        let whereClause = resolveWhere(whereRootNode)

        sprintf "%s %s %s" query joins whereClause
    
    ""
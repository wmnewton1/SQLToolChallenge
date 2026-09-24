namespace sqlgen

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

type JoinClause(join: Join, table: Queryable, condition: Condition) =
    member this.Join = join
    member this.Table = table
    member this.Condition = condition

type Condition(field: Field, operator: string, value: obj) =
    member this.Field = field
    member this.Operator = operator
    member this.Value = value

type Field(name: string, alias: string option,table: Queryable) =
    member this.Name = name
    member this.Alias = alias
    member this.Table = table

type SqlGen(table: Queryable, columns: Field list, joining: JoinClause list option, where: Tree<Condition | LogicalOperator> option) =
    if columns.IsEmpty then
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
        let joins = parseJoins(joining)

        sprintf "%s %s" query joins
    elif (joining.IsEmpty) then
        let whereClause = parseWhere(where)

        sprintf "%s %s" query whereClause
    else
        let joins = parseJoins(joining)
        let whereClause = parseWhere(where)

        sprintf "%s %s %s" query joins whereClause

    let parseJoins(joining: JoinClause list) =
        let template = "%s %s ON %s"

        let joins = []

        for join in joining do
            let joinStr = sprintf template (string join.Join) (join.Table.Name) (parseCondition(join.Condition))
            joins <- joins :: joinStr

        String.concat " " joins

    let parseWhere(where: Tree<Condition | LogicalOperator>) =

    let parseCondition(condition: Condition) =
        let template = "%s %s %s"

        sprintf template (condition.Field.Name) (condition.Operator) (string condition.Value)

    let evaluateNode(node: Node<Condition | LogicalOperator>) =
        if (node.Left == null && node.Right == null)
            // node is a leaf, therefore a condition
            parseCondition(node.Value)
        elif (node.Left != null)
            evaluateNode(node.Left)
        else
            evaluateNode(node.Right)
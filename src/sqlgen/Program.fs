namespace sqlgen

type Join =
    | InnerJoin = "INNER JOIN"
    | FullJoin = "FULL JOIN"

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

    if (joining.IsEmpty && where.IsEmpty) then
        query
    else
        let joins = parseJoins(joining)
        let whereClause = parseWhere(where.Value)

        sprintf "%s %s %s" query joins whereClause

    let parseJoins(joining: JoinClause list) =
        let template = "%s %s ON %s"

        let joins = []

        for join in joining do
            let joinStr = sprintf template (join.Join.ToString()) (join.Table.Name) (sprintf "%s %s %O" join.Condition.Field.Name join.Condition.Operator join.Condition.Value)
            joins <- joins :: joinStr

        String.concat " " joins

    
        



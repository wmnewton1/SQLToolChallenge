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

type SqlGen(table: Queryable, columns: Field list, joining: JoinClause list, where: Tree<Condition | LogicalOperator>) =
    let template = "SELECT %s FROM %s"
    let comma = ", "

    let columns = []

    for column in columns do
        columns <- columns :: column.Name

    let query = sprintf template (String.concat comma columns) (table.Name)
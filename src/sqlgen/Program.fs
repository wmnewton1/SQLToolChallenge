namespace sqlgen

type Queryable =
    abstract member Name : string

// example implementation of Queryable
type Event() =
    interface Queryable with
        member this.Name = "Event"

type JoinClause(join: JoinEnum, table: Queryable, condition: Condition) =
    member this.Join = join
    member this.Table = table
    member this.Condition = condition

type JoinEnum =
    | InnerJoin = "INNER JOIN"
    | FullJoin = "FULL JOIN"

type Condition(field: Field, operator: string, value: obj) =
    member this.Field = field
    member this.Operator = operator
    member this.Value = value

type LogicalOperator =
    | And = "AND"
    | Or = "OR"

type Field(name: string, table: Queryable) =
    member this.Name = name
    member this.Table = table

// TODO change 'where' param type to be binary tree of Condition type
type SqlGen(table: Queryable, columns: Field list, joining: JoinClause list, where: Tree<Condition | LogicalOperator>) =
    // TODO implement
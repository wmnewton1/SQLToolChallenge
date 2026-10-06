namespace SqlGen.Types

type Table(name: string, alias: string) =
    member this.Name = name
    member this.Alias = alias

type Join =
    | InnerJoin
    | FullJoin
    | LeftJoin
    | RightJoin

type Field(name: string, alias: string, table: Table) =
    member this.Name = name
    member this.Alias = alias
    member this.Table = table

type Condition(field: Field, operator: string, value: obj) =
    member this.Field = field
    member this.Operator = operator
    member this.Value = value

type JoinClause(join: Join, table: Table, condition: Condition) =
    member this.Join = join
    member this.Table = table
    member this.Condition = condition

type LogicalOperator =
    | And
    | Or

type ClauseComponent =
    | Con of Condition
    | LogOp of LogicalOperator
namespace SqlGen.Types

open SqlGen.TreeUtils

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

type LogicalOperator =
    | And
    | Or

type ClauseComponent =
    | Con of Condition
    | LogOp of LogicalOperator

type JoinClause(join: Join, table: Table, conditions: Node<ClauseComponent>) =
    member this.Join = join
    member this.Table = table
    member this.Conditions = conditions
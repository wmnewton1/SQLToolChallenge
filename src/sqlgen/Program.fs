namespace sqlgen

type Queryable =
    abstract member Name : string

// example implementation of Queryable
type Event() =
    interface Queryable with
        member this.Name = "Event"

type JoinClause =
    abstract member Join : JoinEnum
    abstract member Table : Queryable
    abstract member Condition : Condition

type JoinEnum =
    | InnerJoin = "INNER JOIN"
    | FullJoin = "FULL JOIN"

type Condition =
    abstract member Field: Field
    abstract member Operator: string
    abstract member Value: obj

type Field =
    abstract member Name: string
    abstract member Table: Queryable

// TODO change 'where' param type to be binary tree of Condition type
type SqlGen(table: Queryable, columns: Field list, joining: JoinClause list, where: Condition) =
    // TODO implement
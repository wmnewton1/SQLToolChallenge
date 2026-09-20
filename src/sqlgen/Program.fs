namespace sqlgen

type Queryable =
    abstract member Name : string

// example implementation of Queryable
type Event() =
    interface Queryable with
        member this.Name = "Event"
type Condition =
    abstract member Field: Field
    abstract member Operator: string
    abstract member Value: obj

type Field =
    abstract member Name: string
    abstract member Table: Queryable

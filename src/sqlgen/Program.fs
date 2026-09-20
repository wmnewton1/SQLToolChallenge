namespace sqlgen

type Queryable =
    abstract member Name : string

// example implementation of Queryable
type Event() =
    interface Queryable with
        member this.Name = "Event"

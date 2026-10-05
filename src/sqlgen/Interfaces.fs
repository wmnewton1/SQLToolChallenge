namespace SqlGen.Interfaces

type Queryable =
    abstract member Name : string
    abstract member Alias : string option
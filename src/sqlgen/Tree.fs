namespace SqlGen.TreeUtils

type Node<'T>(value: 'T, left: Node<'T> option, right: Node<'T> option) =
    member this.Value = value
    member this.Left = left
    member this.Right = right
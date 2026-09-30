namespace TreeUtils

type Node<'T>(value: 'T, left: Node<'T>, right: Node<'T>) =
    member this.Value = value
    member this.Left = left
    member this.Right = right

type Tree(root: Node<'T>) =
    member this.Root = root
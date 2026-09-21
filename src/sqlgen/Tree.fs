namespace sqlgen

type Tree(root: Node) =
    member this.Root = root

type Node(value: obj, left: Node option, right: Node option) =
    member this.Value = value
    member this.Left = left
    member this.Right = right
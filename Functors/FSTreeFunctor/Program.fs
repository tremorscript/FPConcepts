type Tree<'a> = Tree of ('a * Tree<'a> list)

module Tree =
    // 'a -> Tree<'a>
    let leaf x = Tree (x, List.empty)

    // 'a -> Tree<'a> list -> Tree<'a>
    let create x children = Tree (x, children)

    let rec map f (Tree (x, children)) =
        let mappedX = f x
        let mappedChildren = children |> List.map (map f)
        Tree (mappedX, mappedChildren)

// Tree<int>
let source =
    Tree.create 42 [
        Tree.create 1337 [
            Tree.leaf -3]
        Tree.create 7 [
            Tree.leaf -99
            Tree.leaf 100
            Tree.leaf 0]]

// Tree<string>
let dest = source |> Tree.map string

// For more information see https://aka.ms/fsharp-console-apps
printfn "%A" dest
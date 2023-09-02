(*

Let’s go back to thinking of bind as a way of connecting the output of one expression with the input of another.

The bind function “unwraps” the type, and applies the continuation function to the unwrapped value.
But there is nothing in the definition that says that there has to be only one unwrapped value.
There is no reason that we can’t apply the continuation function to each item of the list in turn.

The continuation function passed into bind is required to have a certain signature.
It takes an unwrapped type, but it produces a wrapped type.

In other words, the continuation function must always create a new list as its result.

We can now create a “list workflow”.

 Bind applies the continuation function to each element of the passed in list, and then flattens the resulting list of lists
    into a one-level list. List.collect is a library function that does exactly that.
 Return converts from unwrapped to wrapped. In this case, that just means wrapping a single element in a list.

*)

module Sample1 =
    type ListWorkflowBuilder() =

        member this.Bind(list,f) =
            list |> List.collect f

        member this.Return(x) =
            [x]

    let listWorkflow = new ListWorkflowBuilder()

    let added =
        listWorkflow {
            let! i = [1;2;3]
            let! j = [10;11;12]
            return i + j
        }

    printfn "added=%A" added

    //What does F# do behind the scenes
    let addedM = listWorkflow.Bind([1;2;3], fun x -> 
                                listWorkflow.Bind([10;11;12], fun y -> 
                                    listWorkflow.Return (x + y)))

    printfn "added=%A" addedM

    let multiplied =
        listWorkflow {
            let! i = [1;2;3]
            let! j = [10;11;12]
            return i * j
        }

    printfn "multiplied=%A" multiplied

    //What does F# do behind the scenes.
    let multipliedM = listWorkflow.Bind([1;2;3], fun x -> 
                                    listWorkflow.Bind([10;11;12], fun y ->
                                        listWorkflow.Return (x * y)))

    printfn "multiplied=%A" multipliedM

(*
    Syntactic sugar for "for"

    // let version
    let! i = [1;2;3] in [some expression]

    // for..in..do version
    for i in [1;2;3] do [some expression]

    We need to add a For method to our builder class.
    It generally has exactly the same implementation as the normal Bind method, but is required to accept the sequence type.
*)

module Sample2 =
    type ListWorkflowBuilder() =

        member this.Bind(lst,f) =
            lst |> List.collect f

        member this.Return(x) =
            [x]

        member this.For(lst, f) =
            this.Bind(lst, f)

    let listWorkflow = new ListWorkflowBuilder()

    let multiplied =
        listWorkflow {
            for i in [1;2;3] do
            for j in [10;11;12] do
                return i * j
        }

    printfn "multiplied: %A" multiplied

    //For is the same as bind but it needs to accept the sequence type.
(*
    Every computation expression must have an associated wrapper type.

    But what about the logging example in the previous post? There was no wrapper type there. 
    There was a let! that did things behind the scenes, but the input type was the same as the output type. 
    The type was left unchanged.

    The short answer to this is that you can treat any type as its own “wrapper”.

    If you have a type such as List<T>, it is in fact not a “real” type at all. 
    List<int> is a real type, and List<string> is a real type. 
    But List<T> on its own is incomplete.

    One way to think about List<T> is that it is a function, not a type.
    And like any function it takes a parameter, in this case a “type parameter”. 
    Which is why the concept that .NET developers call “generics” is known as “parametric polymorphism” in computer science terminology.

    Once we grasp the concept of functions that generate one type from another type (called “type constructors”), 
    we can see that what we really mean by a “wrapper type” is just a type constructor.

    But if a “wrapper type” is just a function that maps one type to another type, 
    surely a function that maps a type to the same type fits into this category? And indeed it does. 
    The “identity” function for types fits our definition and can be used as a wrapper type for computation expressions.

    We can define the “identity workflow” as the simplest possible implementation of a workflow builder.
*)

type IdentityBuilder() =
    member this.Bind(x,f) = f x
    member this.Return(x) = x
    member this.ReturnFrom(x) = x

let identity = new IdentityBuilder()

let result = identity {
    let! x = 1
    let! y = 2
    return x + y
}

(*
With this in place, you can see that the logging example discussed earlier is just the identity workflow with some logging added in.
*)

(*
A major use of computation expressions is to unwrap and rewrap values that are stored in some sort of wrapper type.

You can easily compose computation expressions, because the output of a Return can be fed to the input of a Bind.

Every computation expression must have an associated wrapper type.

Any type with a generic parameter can be used as a wrapper type, even lists.

When creating workflows, you should ensure that your implementation conforms to the three sensible rules about wrapping and unwrapping and composition.

*)
// For more information see https://aka.ms/fsharp-console-apps
printfn "Hello from F#"

type CartItem = string // placeholder for a more complicated type

type EmptyState = NoItems // don't use empty list! We want to force clients to handle this as a separate case.
// E.g. "You have no items in your cart"

type ActiveState = { UnpaidItems: CartItem list }

type PaidForState =
    { PaidItems: CartItem list
      Payment: decimal }

type Cart =
    | Empty of EmptyState
    | Active of ActiveState
    | PaidFor of PaidForState


// Operations allowed on the three states - EmptyState, ActiveState and PaidForState

// =============================
// operations on empty state
// =============================

let addToEmptyState item =
    // returns a new active cart
    Cart.Active { UnpaidItems = [ item ] }

// =============================
// operations on active state
// =============================

let addToActiveState state itemToAdd =
    let newList = itemToAdd :: state.UnpaidItems
    Cart.Active { state with UnpaidItems = newList }

let removeFromActiveState state itemToRemove =
    let newList = state.UnpaidItems |> List.filter (fun i -> i <> itemToRemove)

    match newList with
    | [] -> Cart.Empty NoItems
    | _ -> Cart.Active { state with UnpaidItems = newList }

let payForActiveState state amount =
    // returns a new PaidFor Cart
    Cart.PaidFor
        { PaidItems = state.UnpaidItems
          Payment = amount }


type EmptyState with

    member this.Add = addToEmptyState

type ActiveState with

    member this.Add = addToActiveState this
    member this.Remove = removeFromActiveState this
    member this.Pay = payForActiveState this


let addItemToCart cart item =
    match cart with
    | Empty state -> state.Add item
    | Active state -> state.Add item
    | PaidFor state ->
        printfn "Error: The cart is paid for"
        cart

let removeItemFromCart cart item =
    match cart with
    | Empty state ->
        printfn "Error: The cart is empty"
        cart // return the cart
    | Active state -> state.Remove item
    | PaidFor state ->
        printfn "Error: The cart is paid for"
        cart //return the cart

let displayCart cart =
    match cart with
    | Empty state -> printfn "The cart is empty" // can't do state.Items
    | Active state -> printfn "The cart contains %A unpaid items" state.UnpaidItems
    | PaidFor state -> printfn "The cart contains %A paid items. Amount paid: %f" state.PaidItems state.Payment

type Cart with

    static member NewCart = Cart.Empty NoItems
    member this.Add = addItemToCart this
    member this.Remove = removeItemFromCart this
    member this.Display = displayCart this


// Testing the design

let emptyCart = Cart.NewCart
printf "emptyCart="
emptyCart.Display

let cartA = emptyCart.Add "A"
printf "cartA="
cartA.Display

let cartAB = cartA.Add "B"
printf "cartAB="
cartAB.Display

let cartB = cartAB.Remove "A"
printf "cartB="
cartB.Display

let emptyCart2 = cartB.Remove "B"
printf "emptyCart2="
emptyCart2.Display

let emptyCart3 = emptyCart2.Remove "B"
printf "emptyCart3="
emptyCart3.Display

// try to pay for cartA
let cartAPaid =
    match cartA with
    | Empty _
    | PaidFor _ -> cartA
    | Active state -> state.Pay 100m

printf "cartAPaid="
cartAPaid.Display

// try to pay for empty cart
let emptyCartPaid =
    match emptyCart with
    | Empty _
    | PaidFor _ -> emptyCart
    | Active state -> state.Pay 100m

printf "emptyCardPaid="
emptyCartPaid.Display

//  try to pay for cartAB
let cartABPaid =
    match cartAB with
    | Empty _
    | PaidFor _ -> cartAB // return the same cart
    | Active state -> state.Pay 100m

//  try to pay for cartAB again
let cartABPaidAgain =
    match cartABPaid with
    | Empty _
    | PaidFor _ -> cartABPaid // return the same cart
    | Active state -> state.Pay 100m

//So what happens if we have badly written or malicious client code that tries to force payment:
// match cartABPaid with
// | Empty state -> state.Pay 100m
// | PaidFor state -> state.Pay 100m
// | Active state -> state.Pay 100m

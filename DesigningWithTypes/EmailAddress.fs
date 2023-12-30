module EmailAddress

// encapsulated type
type T = EmailAddress of string

// create with continuation
let createWithCont success failure (s: string) =
    if System.Text.RegularExpressions.Regex.IsMatch(s, @"^\S+@\S+\.\S+$") then
        success (EmailAddress s)
    else
        failure "Email address must contain an @ sign"

// create directly
let create s =
    let success e = Some e
    let failure _ = None
    createWithCont success failure s

// unwrap with continuation
let apply f (EmailAddress e) = f e

// unwrap directly
let value e = apply id e

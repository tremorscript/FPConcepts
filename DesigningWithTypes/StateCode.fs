module StateCode

type T = StateCode of string

// create with continuation
let createWithCont success failure (s: string) =
    let s' = s.ToUpper()
    let stateCodes = [ "AZ"; "CA"; "NY" ]

    if stateCodes |> List.exists ((=) s') then
        success (StateCode s')
    else
        failure "State is not in list"

// create directly
let create s =
    let success e = Some e
    let failure _ = None
    createWithCont success failure s

// unwrap with continuation
let apply f (StateCode e) = f e

// unwrap directly
let value e = apply id e

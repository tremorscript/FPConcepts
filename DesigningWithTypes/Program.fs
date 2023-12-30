// For more information see https://aka.ms/fsharp-console-apps
printfn "Hello from F#"

// type Contact =
//   {
//     FirstName: string;
//     MiddleInitial: string;
//     LastName: string;

//     EmailAddress:string;
//     //true if ownership of email address is confirmed
//     IsEmailVerified: bool;

//     Address1: string;
//     Address2: string;
//     City: string;
//     State: string;
//     Zip: string;
//     // true if validated against address service
//     IsAddressValid: string
//   }

open EmailAddress
open StateCode
open ZipCode

type PersonalName =
    { FirstName: string
      // use "option" to signal optionality
      MiddleInitial: string option
      LastName: string }

type EmailContactInfo =
    { EmailAddress: EmailAddress.T
      IsEmailVerified: bool }

type PostalAddress =
    { Address1: string
      Address2: string
      City: string
      State: StateCode.T
      Zip: ZipCode.T }


type PostalContactInfo =
    { Address: PostalAddress
      IsAddressValid: bool }

type ContactInfo =
    | EmailOnly of EmailContactInfo
    | PostOnly of PostalContactInfo
    | EmailAndPost of EmailContactInfo * PostalContactInfo

type Contact =
    { Name: PersonalName
      ContactInfo: ContactInfo }



// using the constructor as a function
"a" |> EmailAddress |> ignore
[ "a"; "b"; "c" ] |> List.map EmailAddress |> ignore

// inline deconstruction
let a' = "a" |> EmailAddress
let (EmailAddress a'') = a'

let addresses = [ "a"; "b"; "c" ] |> List.map EmailAddress

let addresses' = addresses |> List.map (fun (EmailAddress a) -> a)

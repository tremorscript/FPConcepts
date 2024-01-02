namespace WidgetsAndModifiers

open Avalonia.Themes.Fluent
open Fabulous
open Fabulous.Avalonia

open type Fabulous.Avalonia.View

// module App =
//     type Model =
//         { Count: int; Step: int; TimerOn: bool }

//     type Msg =
//         | Increment
//         | Decrement
//         | Reset
//         | SetStep of float
//         | TimerToggled of bool
//         | TimedTick

//     let initModel = { Count = 0; Step = 1; TimerOn = false }

//     let timerCmd () =
//         async {
//             do! Async.Sleep 200
//             return TimedTick
//         }
//         |> Cmd.ofAsyncMsg

//     let init () = initModel, Cmd.none

//     let update msg model =
//         match msg with
//         | Increment ->
//             { model with
//                 Count = model.Count + model.Step },
//             Cmd.none
//         | Decrement ->
//             { model with
//                 Count = model.Count - model.Step },
//             Cmd.none
//         | Reset -> initModel, Cmd.none
//         | SetStep n -> { model with Step = int(n + 0.5) }, Cmd.none
//         | TimerToggled on -> { model with TimerOn = on }, (if on then timerCmd() else Cmd.none)
//         | TimedTick ->
//             if model.TimerOn then
//                 { model with
//                     Count = model.Count + model.Step },
//                 timerCmd()
//             else
//                 model, Cmd.none

//     let view model =
//         (VStack() {
//             TextBlock($"%d{model.Count}").centerText()

//             Button("Increment", Increment).centerHorizontal()

//             Button("Decrement", Decrement).centerHorizontal()

//             (HStack() {
//                 TextBlock("Timer").centerVertical()

//                 ToggleSwitch(model.TimerOn, TimerToggled)
//             })
//                 .margin(20.)
//                 .centerHorizontal()

//             Slider(1., 10, float model.Step, SetStep)

//             TextBlock($"Step size: %d{model.Step}").centerText()

//             Button("Reset", Reset).centerHorizontal()

//         })
//             .center()


// #if MOBILE
//     let app model = SingleViewApplication(view model)
// #else
//     let app model = DesktopApplication(Window(view model))
// #endif


//     let theme = FluentTheme()

//     let program = Program.statefulWithCmd init update app

module App =
    type Model =
        { Balance: decimal
          CurrencySymbol: string
          User: string option }

    type Msg =
        | Spend of decimal
        | Add of decimal
        | Login of string option

    let init () =
        { Balance = 2m
          CurrencySymbol = "$"
          User = Some "User" },
        Cmd.none

    let update msg model =
        match msg with
        | Spend x ->
            { model with
                Balance = model.Balance - x },
            Cmd.none
        | Add x ->
            { model with
                Balance = model.Balance + x },
            Cmd.none
        | Login user -> { model with User = user }, Cmd.none

    let view model =
        (VStack() {
            match model.User with
            | None -> Button("Login", Login(Some "user"))
            | Some user ->
                Label($"Logged in as : {user}")
                Label($"Balance: {model.CurrencySymbol}%.2f{model.Balance}")
                Button("Withdraw", Spend 10.m)
                Button("Deposit", Add 10.m)
                Button("Logout", Login None)
        })


    let theme = FluentTheme()
    let app model = DesktopApplication(Window(view model))

    let program = Program.statefulWithCmd init update app

namespace MessagesAndValidation

open Avalonia.Themes.Fluent
open Fabulous
open Fabulous.Avalonia

open type Fabulous.Avalonia.View

module App =

    // type Animal =
    //     | ValidAnimal of string
    //     | InvalidAnimal of string

    // type Model = { AnimalName: Animal }

    // type Msg = UpdateAnimal of string

    // let validAnimalNames = [ "Emu"; "Kangaroo"; "Platypus"; "Wombat" ]

    // let validateAnimal (animalName: string) =
    //     if List.contains animalName validAnimalNames then
    //         ValidAnimal animalName
    //     else
    //         InvalidAnimal animalName

    // let update msg model =
    //     match msg with
    //     | UpdateAnimal animalName ->
    //         { model with
    //             AnimalName = validateAnimal animalName },
    //         Cmd.none

    // let view (model: model) =
    //     let makeentrycell text =
    //         textbox(text, (fun newvalue -> updateanimal newvalue))

    //     (vstack() {
    //         match model.animalname with
    //         | validanimal validname -> makeentrycell validname
    //         | invalidanimal invalidname ->
    //             makeentrycell invalidname
    //             label(sprintf "%s is not a valid animal name. try %a" invalidname validanimalnames)
    //     })

    // let init () =
    //     { animalname = validateanimal "emu" }, cmd.none

    type Animal = Animal of string

    type ErrorMessage =
        | InvalidName of InputString: string
        | BlankName

    type Model =
        { AnimalName: Result<Animal, ErrorMessage> }

    type Msg = UpdateAnimal of string

    let validAnimalNames = [ "Emu"; "Kangaroo"; "Platypus"; "Wombat" ]

    let validateAnimal (animalName: string) =
        if System.String.IsNullOrWhiteSpace(animalName) then
            Error BlankName
        else if List.contains animalName validAnimalNames then
            Ok(Animal animalName)
        else
            Error(InvalidName animalName)

    let update msg model =
        match msg with
        | UpdateAnimal animalName ->
            { model with
                AnimalName = validateAnimal animalName },
            Cmd.none

    let view (model: Model) =
        let makeEntryCell text =
            TextBox(text, (fun newvalue -> UpdateAnimal newvalue))

        let makeErrorMsg err =
            match err with
            | InvalidName invalidName ->
                (VStack() {
                    makeEntryCell invalidName
                    Label(text = sprintf "%s is not a valid animal name. Try %A" invalidName validAnimalNames)
                })
            | BlankName ->
                (VStack() {
                    makeEntryCell ""
                    Label(text = sprintf "You must input a name")
                })


        (VStack() {
            match model.AnimalName with
            | Ok(Animal validName) -> makeEntryCell validName
            | Error errorMsg -> makeErrorMsg errorMsg
        })

    let init () =
        { AnimalName = validateAnimal "Emu" }, Cmd.none

#if MOBILE
    let app model = SingleViewApplication(view model)
#else
    let app model = DesktopApplication(Window(view model))
#endif


    let theme = FluentTheme()

    let program = Program.statefulWithCmd init update app

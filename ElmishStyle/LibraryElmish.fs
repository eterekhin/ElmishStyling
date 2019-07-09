
module ElmishStyle.LibraryModule
open System
open System
open System
open System.Drawing
open Seed
module oneSomeWorkflow =
    type OneSomeWorkflow() =
        member this.Bind(x, f) =
            match x with
            | Some s -> s
            | None -> f x

        member this.Return(x) = x
    let onesome = OneSomeWorkflow();

type SnapshotInfo =
    {
       text: string
       yPosition: int
       color: ConsoleColor
       formatText: string
    }

let mutable (storage: SnapshotInfo list) = [];

type ChangePosition =
    | Up
    | Down

type TapKey =
    | Button of ConsoleKey

type CountStatesBack = CountStatesBack of int

type RevertModel = CountStatesBack * SnapshotInfo

type ReverseMsg =
    | KeyButton of ConsoleKey

type Model =
    | Read of SnapshotInfo
    | Reverse of RevertModel * SnapshotInfo

type ReadMsg =
    | KeyButton of ConsoleKey
    | EventStore


type Msg =
    | ReadMsg of ReadMsg
    | ReverseMsg of ReverseMsg

let init() =
    { text = ""; yPosition = 0; color = ConsoleColor.Black; formatText = "" }, Cmd.ofMsg ((Author.Block, ConsoleColor.Black))


let getlines (str: string) startY endY =
        let lines = str.Split [| '\n' |]
        let difference = abs (startY - endY);
        if difference > lines.Length
            then failwith "error"
            else
                let (startY', endY') =
                    if startY < 0
                     then (0, difference)
                     else if endY > lines.Length - 1 then (lines.Length - 1 - difference, lines.Length - 1)
                     else (startY, endY)
                lines.[startY'..endY'] |> List.ofArray
                |> String.concat ("\n")



type ButtonAction =
    | ConsoleColor of ConsoleColor
    | Author of Author
    | ChangePosition of ChangePosition

let readUpdate (msg: ReadMsg) (model: SnapshotInfo) =
    match msg with
    | KeyButton button ->

            let (|>>) o1 o2 =
               match o1 with
               | Some s -> Some s
               | None -> o2

            let q =
                    Option.bind (ConsoleColor >> Some)
                        (match button with
                        | ConsoleKey.D1 -> Some ConsoleColor.Black
                        | ConsoleKey.D2 -> Some ConsoleColor.Red
                        | ConsoleKey.D3 -> Some ConsoleColor.Blue
                        | ConsoleKey.D4 -> Some ConsoleColor.Green
                        | ConsoleKey.D5 -> Some ConsoleColor.Cyan
                        | _ -> None)


            let q1 =
                    Option.bind (Author >> Some)
                     (match button with
                     | ConsoleKey.P -> Some Author.Push
                     | ConsoleKey.B -> Some Author.Block
                     | ConsoleKey.E -> Some Author.Esenin
                     | ConsoleKey.L -> Some Author.Ler
                     | _ -> None)

            let q3 =
                    Option.bind (ChangePosition >> Some)
                     (match button with
                     | ConsoleKey.DownArrow -> Some ChangePosition.Down
                     | ConsoleKey.UpArrow -> Some ChangePosition.Up
                     | _ -> None)

            match (q |>> q1 |>> q3) with
            | Some s ->
                match s with
                | ConsoleColor color ->
                    { model with color = color }, Cmd.ofMsg EventStore
                | Author author ->
                     let (Stix text) = seed.[author];
                     let formatText = getlines text 0 2
                     { text = text; yPosition = 0; color = model.color; formatText = formatText }, Cmd.ofMsg EventStore
                | ChangePosition change ->
                     match change with
                     | Up ->
                         if model.yPosition = 0
                          then model, []
                          else
                          let formatText = getlines model.text (model.yPosition - 1) (model.yPosition + 1)
                          { model with yPosition = model.yPosition - 1; formatText = formatText }, Cmd.ofMsg EventStore
                     |Down ->
                          if model.yPosition = (model.text.Split [| '\n' |]).Length - 1
                           then model, []
                           else
                              let formatText = getlines model.text (model.yPosition + 1) (model.yPosition + 3)
                              { model with yPosition = model.yPosition + 1; formatText = formatText }, Cmd.ofMsg EventStore


            | None -> failwith "error"



let reverseUpdate (msg:ReverseMsg) (model:RevertModel) =
    model,[]








let update (msg: Msg) (model: Model) =
    match (msg,model) with
    |(ReadMsg readmsg,Read readmodel) ->
        let (model,cmd) = readUpdate readmsg readmodel
        
    |(ReverseMsg reverseMsg, Reverse (a,b)) ->
        reverseUpdate reverseMsg (Reverse (a,b))
        
        
        

















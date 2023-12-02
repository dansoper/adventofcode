module Day2

open Utils

let partTwo = true

type Round = {
    Red: int
    Green: int
    Blue: int
}

type Game = {
    Id: int
    Rounds: Round array
}

let maxForGame (game: Game) : Round =
    game.Rounds
    |> Array.fold (fun acc curr -> { 
        Red = if curr.Red > acc.Red then curr.Red else acc.Red
        Green = if curr.Green > acc.Green then curr.Green else acc.Green
        Blue = if curr.Blue > acc.Blue then curr.Blue else acc.Blue }) { Red = 0; Green = 0; Blue = 0 }

let product (round: Round): int =
    round.Red * round.Green * round.Blue

let roundWouldBePossible (max: Round) (round: Round)  =
    round.Red <= max.Red && round.Blue <= max.Blue && round.Green <= max.Green

let gameWouldBePossible (game: Game) =
    let b = 
        game.Rounds 
        |> Array.map (roundWouldBePossible { Red = 12; Blue = 14; Green = 13 })
        |> Array.contains false
    not b

let getColour (section: string) (colour: string) =
    let colourSection = 
        section.Split ", "
        |> Array.filter (fun x -> x.Contains colour)
        |> (fun x -> if Array.length x = 0 then "" else Array.head x)
    if colourSection = "" then 0 else colourSection.Split " " |> Array.head |> int

let gameId (game: Game) = game.Id

let getRound section =
    { Red = getColour section "red"; Green = getColour section "green"; Blue = getColour section "blue"; }
    

let getRounds (section: string) =
    section.Split "; "
    |> Array.map getRound

let parseLine (line: string): Game =
    let colon = line.Split ": "
    let gameId = int (colon[0].Substring 5)
    { Id = gameId; Rounds = getRounds colon[1] }

let runDay (input: string) =
    let parsed =
        input
        |> splitByLine
        |> Array.map parseLine
    if partTwo
        then
            parsed
            |> Array.map maxForGame
            |> Array.map product
            |> Array.sum
        else
            parsed
            |> Array.filter gameWouldBePossible
            |> Array.map gameId
            |> Array.sum

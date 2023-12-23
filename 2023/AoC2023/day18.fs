module Day18

open Utils
open System.Text.RegularExpressions
open System

let partTwo = true

type Instruction = {
    Move: char
    Amount: int
    Colour: string
}

let add (a: Coord) (b: Coord): Coord =
    { x = a.x + b.x; y = a.y + b.y }

let parseToObject input =
    let mappings =
        Regex.Matches (input, "([UDLR]) ([0-9]+) \((#[a-f0-9]{6})\)")
        |> Seq.cast<Match>
        |> Seq.map (fun x -> x.Groups)
        |> Seq.map (fun x -> { Move = char x[1].Value; Amount = int x[2].Value; Colour = x[3].Value })
        |> Array.ofSeq
    mappings

let partTwoInstructions (inst: Instruction array) =
    inst
    |> Array.map (fun instruction ->
        let hexString = instruction.Colour.Substring(1, 5)
        let decimal = Convert.ToInt32(hexString, 16)
        let directionString = instruction.Colour.Substring(6, 1)
        let newDirection =
            match directionString with
            | "0" -> 'R'
            | "1" -> 'D'
            | "2" -> 'L'
            | "3" -> 'U'
            | _ -> failwith "Apples"
        { Move = newDirection; Amount = decimal; Colour = "" }
    )

let runInstructions (inst: Instruction array) =
    let mutable curPosition = { x = 0; y = 0 }
    let mutable painted: Coord array = [|curPosition|]
    let mutable extraCount = 0
    inst
    |> Array.iter (fun instruction ->
        let diff =
            match instruction.Move with
            | 'U' -> { x = 0; y = -1 }
            | 'D' -> { x = 0; y = 1 }
            | 'L' -> { x = -1; y = 0 }
            | 'R' -> { x = 1; y = 0 }
            | _ -> failwith ("Bananas")
        
        let actDiff = { x = diff.x * instruction.Amount; y = diff.y * instruction.Amount  }
        let newPos = add curPosition actDiff
        curPosition <- newPos
        painted <- Array.append painted [|curPosition|]
        if instruction.Amount >= 2 then extraCount <- extraCount + instruction.Amount - 1

// Part 1, before optimisation
(*        [1..instruction.Amount]
        |> List.iter (fun x -> 
            let newPos = add curPosition diff
            curPosition <- newPos
            painted <- Array.append painted [|curPosition|]
        )*)
    )
    (painted, extraCount)

// Shoelace formula (actually 'Triangle' formula)
let shoelaceFormula (coords: Coord array) =
    let paired =
        coords
        |> Array.pairwise
    
    let fullPairs = Array.append paired [|(Array.last coords, Array.head coords)|]

    (fullPairs
    |> Array.map (fun (l, r) -> 
        (int64 l.x * int64 r.y) - (int64 r.x * int64 l.y)
    )
    |> Array.sum) / 2L

let picksTheorem coords extra =
    let sum = shoelaceFormula coords
    sum + 1L + ((int64 coords.Length + extra) / 2L)

let runDay (input: string) =
    let instructions = 
        input
        |> parseToObject

    let instructionsToDo = if partTwo then instructions |> partTwoInstructions else instructions

    let (painted, extraCount) = runInstructions instructionsToDo

    picksTheorem painted extraCount
    

    
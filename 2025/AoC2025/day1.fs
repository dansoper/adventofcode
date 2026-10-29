module Day1

open Utils

type MoveEnum =
| L
| R

type Instruction = {
    Move: MoveEnum
    Amount: int
}

let processLine (input: string): Instruction = 
    match input.[0] with
    | 'L' -> { Move = L; Amount = int (input.[1..]) }
    | 'R' -> { Move = R; Amount = int (input.[1..]) }
    | _ -> failwithf "Invalid input: %s" input

let wrapPosition (pos: int): int =
    ((pos % 100) + 100) % 100

let passesOverZeroOne pos moveValue =
    let start = pos
    let finish = pos + moveValue
    let wrapped = wrapPosition finish
    let hm = abs (finish - wrapped) / 100
    let adj = if start = 0 && wrapped <> 0 then 0 else 0
    let adj2 = if moveValue < 0 && wrapped = 0 then 1 else 0

    System.Console.WriteLine $"{start}, {finish}, {wrapped} {start / 100}, {finish / 100}, {hm} {adj} {adj2} {hm + adj + adj2} {max (hm + adj + adj2) 0}"
    max (hm + adj + adj2) 0

let passesOverZero pos moveValue =
    let num = abs(moveValue / 100)
    let bit = moveValue % 100
    let adj = if pos + bit > 99 || pos + bit < 0 then 1 else 0
    num + adj

let runDay (input: string) =
    input
    |> splitByLine
    |> Array.map processLine
    |> Array.fold (fun (pos, stops, passes) instr ->
        let moveValue =
            match instr.Move with
            | L -> -instr.Amount
            | R -> instr.Amount
        
        let newPos = pos + moveValue
        let wrapped = newPos |> wrapPosition

        let numPasses = passesOverZero pos moveValue

        let countOfStoppedZeros = stops + if wrapped = 0 then 1 else 0
        let totalPasses = passes + numPasses

        wrapped, countOfStoppedZeros, totalPasses
    ) (50, 0, 0)
    

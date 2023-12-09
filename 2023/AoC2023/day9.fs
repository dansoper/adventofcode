module Day9

open Utils

let partTwo = true

let allZero (arr: int array) =
    arr 
    |> Array.filter (fun x -> x = 0)
    |> Array.length = (Array.length arr)


let rec findDifference (arr: int array): int array =
    let res =
        arr
        |> Array.pairwise
        |> Array.map (fun (x, y) -> y - x)
    let diffs =
        if allZero res
            then res 
            else findDifference res
    if partTwo
    then
        Array.append [|(Array.head arr) - (Array.head diffs)|] arr
    else
        Array.append arr [|(Array.last arr) + (Array.last diffs)|]

let runDay (input: string) =
    input
    |> splitByLine
    |> Array.map (fun x -> x |> split " " |> Array.map (fun y -> int y))
    |> Array.map (fun x -> findDifference x)
    |> Array.map (fun x -> x |> if partTwo then Array.head else Array.last)
    |> Array.sum
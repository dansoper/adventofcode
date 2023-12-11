module Day10

open Utils
open System.Collections.Generic

type Compass =
    | North = 0
    | East = 1
    | South = 2
    | West = 3

type LRUD =
    | Left = 0
    | Right = 1
    | Up = 2
    | Down = 3

type Movement = {
    x: int
    y: int
    from: Compass
}

let mostRecentMove (prev: Coord) (from: Coord): LRUD =
    if prev.x = from.x then
        if prev.y < from.y then LRUD.Down else LRUD.Up
    else
        if prev.x < from.x then LRUD.Right else LRUD.Left

let rec move (grid: char array array) (target: Coord) (prev: Coord) (from: Coord): Coord =
    let prevChar = grid[prev.y][prev.x]
    let nextChar = grid[from.y][from.x]
    let mostRecent = mostRecentMove prev from
    match nextChar with
    | '|' -> if mostRecent = LRUD.Up then { x = from.x; y = from.y - 1} else { x = from.x; y = from.y + 1} // Move up else down
    | '-' -> if mostRecent = LRUD.Right then { x = from.x + 1; y = from.y} else { x = from.x - 1; y = from.y} // Move right else left
    | 'L' -> if mostRecent = LRUD.Down then { x = from.x + 1; y = from.y} else { x = from.x; y = from.y - 1} // Move right else up
    | 'J' -> if mostRecent = LRUD.Down then { x = from.x - 1; y = from.y} else { x = from.x; y = from.y - 1} // Move left else up
    | '7' -> if mostRecent = LRUD.Up then { x = from.x - 1; y = from.y} else { x = from.x; y = from.y + 1} // Move left else down
    | 'F' -> if mostRecent = LRUD.Up then { x = from.x + 1; y = from.y} else { x = from.x; y = from.y + 1} // Move right else down
    | _ -> from

let runDay (input: string) =
    let grid = 
        input
        |> splitByLine
        |> Array.map splitToGrid
        |> Array.map (fun x -> Array.ofSeq x)

    let startY =
        grid
        |> Array.findIndex (fun y -> y |> Seq.exists (fun x -> x = 'S'))

    let startX = grid[startY] |> Seq.findIndex(fun x -> x = 'S')

    let start = { x = startX; y = startY }

    let mutable reached = false
    let mutable count = 1
    let mutable pos = { x = startX; y = startY + 1 }
    let mutable prevPos = start

    let path = new List<Coord>()
    path.Add start

    while not reached do
        let memory = pos
        path.Add pos
        pos <- move grid start prevPos pos
        reached <- if pos = start then true else reached
        count <- count + 1
        prevPos <- memory

    printf "Part 1: %A\n" (count / 2)

    //printf "%A\n" (Seq.length path)
    //printf "%A" (Array.ofSeq path)

    let mutable count = 0
    for y = 0 to (Array.length grid) - 1 do
        let mutable inside = false
        let mutable inLine = 0
        for x = 0 to (Array.length grid[0]) - 1 do
            let c = if grid[y][x] = 'S' then 'F' else grid[y][x]
            if path.Contains ({ x = x; y = y })
                then
                    printf "Path contains %A %A \n" x y 
                    if (c = '|')
                    then 
                        inside <- not inside
                        printf "Flipping inside to %A\n" inside
                    elif (c = 'L' || c = 'F')
                    then
                        inLine <- if c = 'L' then 1 else -1
                        inside <- not inside
                    elif (c = 'J' || c = '7')
                    then
                        if (inLine = 1 && c = 'J') || (inLine = -1 && c = '7')
                            then
                            inside <- not inside
                            printf "Flipping inside to %A\n" inside
                        inLine <- 0
                else 
                    printf "Path not contains %A %A \n" x y 
                    if inside then 
                        count <- count + 1
                        printf "Count now %A\n" count

    count
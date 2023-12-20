module Day16

open Utils

let partTwo = true

let mutable visited: Coord array = Array.empty
let mutable dejavu: (Coord * Coord) array = Array.empty

let isOutOfBounds (c: Coord) (grid: char seq array): bool =
    c.x < 0 || c.y < 0 || c.x >= Seq.length grid[0] || c.y >= grid.Length

let addToVisited (c: Coord) =
    if not (Array.exists (fun u -> u = c) visited)
    then visited <- Array.append visited [|c|]

let diff (a: Coord) (b: Coord): Coord =
    { x = b.x - a.x; y = b.y - a.y }

let add (a: Coord) (b: Coord): Coord =
    { x = a.x + b.x; y = a.y + b.y }

let flip (a: Coord): Coord =
    { x = a.y; y = a.x }

let flipAndReverse (a: Coord): Coord =
    { x = -a.y; y = -a.x }

let rec go (from: Coord) (until: Coord) (grid: char seq array) =
    if 
        not (isOutOfBounds until grid)
        && not (Array.exists (fun (f, u) -> f = from && u = until) dejavu)
        then
            addToVisited until
            dejavu <- Array.append dejavu [|(from, until)|]
            let difference = diff from until
            let charToActOn = Seq.item until.x grid[until.y]
            match charToActOn with
            | '.' -> go until (add until difference) grid
            | '/' -> go until (add until (flipAndReverse difference)) grid
            | '\\' -> go until (add until (flip difference)) grid
            | '|' ->
                if difference.x = 0
                // y is therefore the change, so we keep going 
                then go until (add until difference) grid
                // x is therefore the change, so there's a split
                else 
                    go until (add until { x = 0; y = -1 }) grid
                    go until (add until { x = 0; y = 1 }) grid
            | '-' ->
                if difference.y = 0
                // y is therefore the change, so we keep going 
                then go until (add until difference) grid
                // x is therefore the change, so there's a split
                else 
                    go until (add until { y = 0; x = -1 }) grid
                    go until (add until { y = 0; x = 1 }) grid
            | _ -> failwith "Wrong character"
            


let runDay (input: string) =
    let grid = 
        input
        |> splitByLine
        |> Array.map (fun x -> splitToGrid x)

    if partTwo
        then
            // Takes a few minutes!
            let colsLength = Seq.length grid[0]
            printfn "Left starting"
            let leftEdge: int list =
                [0..(grid.Length-1)]
                |> List.map (fun x -> 
                    visited <- Array.empty
                    dejavu <- Array.empty
                    go { x = -1; y = x } { x = 0; y = x } grid
                    Array.length visited
                )
            printfn "Right starting"
            let rightEdge: int list =
                [0..(grid.Length-1)]
                |> List.map (fun x -> 
                    visited <- Array.empty
                    dejavu <- Array.empty
                    go { x = colsLength; y = x } { x = colsLength - 1; y = x } grid
                    Array.length visited
                )
            printfn "Top starting"
            let topEdge: int list =
                [0..(colsLength-1)]
                |> List.map (fun x -> 
                    visited <- Array.empty
                    dejavu <- Array.empty
                    go { x = x; y = -1 } { x = x; y = 0 } grid
                    Array.length visited
                )
            printfn "Bottom starting"
            let bottomEdge: int list =
                [0..(colsLength-1)]
                |> List.map (fun x -> 
                    visited <- Array.empty
                    dejavu <- Array.empty
                    go { x = x; y = grid.Length } { x = x; y = grid.Length - 1 } grid
                    Array.length visited
                )
            
            printfn "%A %A %A %A" topEdge bottomEdge leftEdge rightEdge

            let all = List.concat [|topEdge; bottomEdge; leftEdge; rightEdge|]
            let sorted = List.sortDescending all
            sorted[0]
        else
            go { x = -1; y = 0 } { x = 0; y = 0 } grid
            Array.length visited
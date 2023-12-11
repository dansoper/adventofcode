module Day11

open Utils

type GalaxyDistances = {
    Coord: Coord
    Galaxies: GalaxyDistances array
    Distance: int
}

let wholeRowIsDot (row: char array): bool =
    row
    |> Array.filter (fun x -> x = '.')
    |> Array.length = (Array.length row)

let wholeColsAreDots (grid: char array array) =
    let starter = 
        [|0..((Array.length grid[0]-1))|]
        |> Array.map (fun x -> true)

    grid
    |> Array.fold (fun prev cur ->
        prev
        |> Array.mapi (fun index item -> if item && cur[index] = '.' then true else false)    
    ) starter

let runDay (input: string) =
    let repeat = 2

    let partGrid = 
        input
        |> splitByLine
        |> Array.map (fun x -> x |> splitToGrid |> Array.ofSeq)
        |> Array.collect (fun x -> 
            if wholeRowIsDot x 
            then [|0..(repeat - 1)|] |> Array.map (fun y -> x)
            else [|x|])

    let expandCols = wholeColsAreDots partGrid

    let grid =
        partGrid
        |> Array.map (fun x -> 
            x
            |> Array.indexed
            |> Array.collect (fun (index, item) -> 
                if expandCols[index]
                then [|0..(repeat - 1)|] |> Array.map (fun y -> item)
                else [|item|]))

    let galaxies =
        grid
        |> Array.mapi (fun yIndex yItem -> 
            yItem
            |> Array.indexed
            |> Array.filter (fun (xIndex, xItem) -> xItem = '#')
            |> Array.map (fun (xIndex, xItem) -> { x = xIndex; y = yIndex })
        )
        |> Array.collect (fun x -> x)
    
    let num = 
        galaxies
        |> Array.map (fun x -> { 
            Coord = x
            Distance = 0 
            Galaxies = 
                galaxies
                |> Array.filter (fun y -> y <> x)
                |> Array.map (fun y -> { Coord = y; Distance = 0; Galaxies = Array.empty })
        })
        |> Array.map (fun main -> 
            main.Galaxies
            |> Array.map (fun child -> abs (main.Coord.x - child.Coord.x) + abs(main.Coord.y - child.Coord.y))
        )
        |> Array.collect (fun x -> x)
        |> Array.sum

    num / 2
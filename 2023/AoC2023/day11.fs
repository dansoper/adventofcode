module Day11

open Utils

let partTwo = true

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
    |> Array.indexed
    |> Array.filter (fun (index, item) -> item)
    |> Array.map (fun (index, item) -> index)

let repeat = if partTwo then 1000000L else 2

let sum (from: Coord) (until: Coord) (expandRows: int array) (expandCols: int array) =
    let xDiff = int64 (abs (from.x - until.x))
    let yDiff = int64 (abs (from.y - until.y))
    let expandedYs = expandRows |> Array.filter (fun item -> (item >= from.y && item <= until.y) || (item <= from.y && item >= until.y)) |> Array.length
    let expandedXs = expandCols |> Array.filter (fun item -> (item >= from.x && item <= until.x) || (item <= from.x && item >= until.x)) |> Array.length
    xDiff + yDiff + (int64 expandedYs * (repeat - 1L)) + (int64 expandedXs * (repeat - 1L))

let runDay (input: string) =

    let grid = 
        input
        |> splitByLine
        |> Array.map (fun x -> x |> splitToGrid |> Array.ofSeq)

    let expandRows = 
        grid
        |> Array.indexed
        |> Array.filter (fun (index, item) -> wholeRowIsDot item)
        |> Array.map (fun (index, item) -> index) 
    let expandCols = wholeColsAreDots grid

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
            |> Array.map (fun child -> sum main.Coord child.Coord expandRows expandCols)
        )
        |> Array.collect (fun x -> x)
        |> Array.sum

    num / 2L
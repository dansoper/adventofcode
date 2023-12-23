module Day21

open Utils

type ReachedInfo = {
    Info: Map<int, Coord array>
    Avoid: bool
}

let partTwo = true

let dedup (coords: Coord array): Coord array =
    //printfn "DEDUP %A" (Array.length coords)
    coords
    |> Array.indexed
    |> Array.filter (fun (index, coord) -> 
        (coords |> Array.findIndex (fun c -> c = coord)) = index
    )
    |> Array.map (fun (index, coord) -> coord)

let firstAttemptAtPartOne grid startPosition =
    let mutable gridReached: ReachedInfo array array = 
        grid
        |> Array.map (fun row -> 
            row
            |> Array.map (fun char -> 
                { Info = Map.empty; Avoid = if char = '#' then true else false }
            )
        )
    
    // From 16 onwards this starts to get v slow
    [1; 2; 4; 8]
    //[|1..64|]
    |> List.iter (fun i ->
        //printfn "LIST %A" i
        gridReached <-
            gridReached
            |> Array.mapi (fun y row ->
                //printfn "%A y" y
                row
                |> Array.mapi (fun x char ->
                    if char.Avoid then char 
                    else
                        if i = 1
                        then
                            let moveLeft = if x = 0 then [||] else [|{ x = x - 1; y = y }|]
                            let moveRight = if x = (Array.length row) - 1 then [||] else [|{ x = x + 1; y = y }|]
                            let moveUp = if y = 0 then [||] else [|{ x = x; y = y - 1 }|]
                            let moveDown = if y = (Array.length grid) - 1 then [||] else [|{ x = x; y = y + 1 }|]
                            let moves: Coord array = 
                                Array.concat [|moveLeft; moveRight; moveUp; moveDown|]
                                |> Array.filter (fun coord ->  grid[coord.y][coord.x] <> '#')

                            { char with 
                                Info =
                                char.Info |> Map.add 1 moves
                            }
                        else
                            let half = i / 2
                            //let half = i - 1
                            let newItems = 
                                char.Info[half]
                                |> Array.fold (fun acc coord -> 
                                    gridReached[coord.y].[coord.x].Info[half]
                                    |> Array.filter (fun newOne -> not (Array.exists (fun existingOne -> newOne = existingOne) acc))
                                    |> Array.append acc
                                ) Array.empty
                            //printfn "New l %A" (Array.length newItems)
                            { char with
                                Info =
                                Map.empty |> Map.add i newItems
                            }
                 )
            )
    )
    // For the start position, go to the 8s
    // first time will be 16, then 24, 32, 40, 48, 54, 62
    let mutable toCheck: Coord array = [|startPosition|]
    for i = 1 to 8 do
        //printfn "New i %A" i
        toCheck <-
            toCheck
            |> Array.fold (fun acc coord -> 
                gridReached[coord.y].[coord.x].Info[8]
                |> Array.filter (fun newOne -> not (Array.exists (fun existingOne -> newOne = existingOne) acc))
                |> Array.append acc
            ) Array.empty



    toCheck |> Array.length

let bfsForFullSquare (grid: char array array) (startPosition: Coord): Map<Coord, int> =
    let mutable map: Map<Coord, int> = Map.empty
    let mutable queue = [|(startPosition, 0)|]
    
    while (queue |> Array.length) > 0 do
        let (coord, distance) = Array.head queue
        queue <- queue |> Array.skip 1

        if not (map |> Map.containsKey coord) then
            //printfn "%A %A" (queue |> Array.length) (map |> Map.keys |> Seq.length)

            map <- map |> Map.add coord distance
            let moveLeft = if coord.x = 0 then [||] else [|{ x = coord.x - 1; y = coord.y }|]
            let moveRight = if coord.x = (Array.length grid[0]) - 1 then [||] else [|{ x = coord.x + 1; y = coord.y }|]
            let moveUp = if coord.y = 0 then [||] else [|{ x = coord.x; y = coord.y - 1 }|]
            let moveDown = if coord.y = (Array.length grid) - 1 then [||] else [|{ x = coord.x; y = coord.y + 1 }|]
            let moves: Coord array = 
                Array.concat [|moveLeft; moveRight; moveUp; moveDown|]
                |> Array.filter (fun coord -> grid[coord.y][coord.x] <> '#')
            moves |> Array.iter (fun item ->
                queue <- Array.append queue [|(item, distance + 1)|]
            )
    map

let runDay (input: string) =
    let grid = 
        input
        |> splitByLine
        |> Array.map (fun x -> x |> splitToGrid |> Array.ofSeq)

    let startY =
        grid
        |> Array.findIndex (fun y -> y |> Seq.exists (fun x -> x = 'S'))

    let startX = grid[startY] |> Seq.findIndex(fun x -> x = 'S')

    let startPosition = { x = startX; y = startY }
    
    //firstAttemptAtPartOne grid startPosition
    
    if partTwo 
        then
            let results = 
                bfsForFullSquare grid startPosition
                |> Map.toArray
            // https://github.com/villuna/aoc23/wiki/A-Geometric-solution-to-advent-of-code-2023,-day-21
            // V Helpful
            // How many squares we need
            let n = ((26501365L - (int64 (Array.length grid) / 2L)) / int64 (Array.length grid));
            // How many 'even' squares
            let even = n * n;
            // odd
            let odd = (n + 1L) * (n + 1L);
            let evenTilesInSquare = 
                int64 (results
                |> Array.filter (fun (coord, dist) ->
                    dist % 2 = 0
                )
                |> Array.length)
            let oddTilesInSquare = 
                int64 (results
                |> Array.filter (fun (coord, dist) ->
                    dist % 2 = 1
                )
                |> Array.length)
            let evenTilesInCorner = 
                int64 (results
                |> Array.filter (fun (coord, dist) ->
                    dist % 2 = 0 && dist > 65
                )
                |> Array.length)
            let oddTilesInCorner = 
                int64 (results
                |> Array.filter (fun (coord, dist) ->
                    dist % 2 = 1 && dist > 65
                )
                |> Array.length)

            let res = 
                odd * oddTilesInSquare 
                + even * evenTilesInSquare
                - ((n + 1L) * oddTilesInCorner)
                + (n * evenTilesInCorner);
            res
        else
            let results = bfsForFullSquare grid startPosition
            results
            |> Map.toArray
            |> Array.filter (fun (coord, dist) -> dist <= 64 && dist % 2 = 0)
            |> Array.length
            |> int64
module Day23

open Utils

let partTwo = true

type Route = {
    Been: Coord array
    Length: int
    Complete: bool
}

type Route with
    member x.lastCoord = Array.last x.Been


let runDay (input: string) =
    let grid: char array array =
        input
        |> splitByLine
        |> Array.map (fun x -> x |> splitToGrid |> Array.ofSeq)

    let width = Array.length grid[0]
    let height = Array.length grid

    let okPosition currentCoord (newCoord: Coord) = 
        if newCoord.x < 0 || newCoord.y < 0 || newCoord.x >= width || newCoord.y >= height
        then false
        else
            let gridChar = grid[newCoord.y][newCoord.x]
            if (not partTwo) && gridChar = '>' then (diff currentCoord newCoord) = { x = 1; y = 0 }
            elif (not partTwo) && gridChar = '^' then (diff currentCoord newCoord) = { x = 0; y = -1 }
            elif (not partTwo) && gridChar = '<' then (diff currentCoord newCoord) = { x = -1; y = 0 }
            elif (not partTwo) && gridChar = 'v' then (diff currentCoord newCoord) = { x = 0; y = 1 }
            else gridChar <> '#'

    let endGoal = { y = height - 1; x = width - 2 }

    let startingPositions (coord: Coord) =
        [|{ x = coord.x - 1; y = coord.y };{ x = coord.x + 1; y = coord.y };{ x = coord.x; y = coord.y - 1 };{ x = coord.x; y = coord.y + 1}|]
        |> Array.filter (okPosition coord)

    let findNodesFrom (coord: Coord) prev: (Coord * int) option =
        let mutable lastCoord = prev
        let mutable currentCoord = coord
        let mutable pathOK = true
        let mutable optionsRanOut = false
        let mutable pathLength = 1

        while pathOK do
            let options = 
                [|{ x = currentCoord.x - 1; y = currentCoord.y };{ x = currentCoord.x + 1; y = currentCoord.y };{ x = currentCoord.x; y = currentCoord.y - 1 };{ x = currentCoord.x; y = currentCoord.y + 1}|]
                |> Array.filter (fun coord ->
                    coord <> lastCoord && (okPosition currentCoord coord)
                )
            if Array.length options = 1 then
                lastCoord <- currentCoord
                currentCoord <- options[0]
                pathLength <- pathLength + 1
            elif currentCoord <> endGoal && Array.length options = 0 then
                optionsRanOut <- true
                pathOK <- false
            else
                pathOK <- false
        
        if optionsRanOut then None else Some(currentCoord, pathLength)

    let mutable options = [|{ x = 1; y = -1 }|]
    let mutable optionsFrom: Map<Coord, (Coord * int) array> = Map.empty

    while Array.length options > 0 do
        let option = Array.head options
        //printfn "options %A %A" option.y option.x 
        options <- options |> Array.skip 1
        if not (optionsFrom |> Map.containsKey option) then
            let starts = startingPositions option
            let nodes = 
                starts
                |> Array.map (fun start ->
                    //printfn "start %A %A" start.y start.x 
                    let node = findNodesFrom start option
                    //printfn "Node %A" node
                    if node.IsSome then options <- Array.append options [|fst node.Value|]
                    node
                )
                |> Array.filter (fun x -> x.IsSome)
                |> Array.map (fun x -> x.Value)
            //if option <> { x = 1; y = -1 } then
            optionsFrom <- optionsFrom |> Map.add option nodes

    if partTwo then
        let mutable paths = Array.empty
        let mutable highestComplete = 0

        paths <- paths |> Array.append [|{ Been = [|{ x = 1; y = -1 }|]; Complete = false; Length = -1  }|]

        let mutable check = 0
        while paths |> Array.length > 0 do
            check <- check + 1
            // Choose first path that isn't complete
            let (index, path) = paths |> Array.indexed |> Array.filter (fun (ind, x) -> not x.Complete) |> Array.head
            paths <- paths |> Array.removeAt index
            if (check % 5000) = 0 then printfn "len %A highest %A" (paths |> Array.length) highestComplete

            // Get updated path
            let optionsFromLast = optionsFrom[path.lastCoord]
            let newPaths = 
                optionsFromLast
                |> Array.filter (fun (opt, len) -> not (path.Been |> Array.contains opt))
                |> Array.map (fun (opt, len) ->
                    let newLen = path.Length + len
                    if opt = endGoal && newLen > highestComplete then highestComplete <- newLen
                    {
                        Been = Array.append path.Been [|opt|]
                        Length = newLen
                        Complete = (opt = endGoal)
                    }
                )
            
            paths <- Array.append paths newPaths
            // Order by highest score
            paths <- paths |> Array.filter (fun x -> not x.Complete) |> Array.sortByDescending (fun x -> (x.Been |> Array.length, x.Length))
        
        highestComplete
    else 
        let mutable paths = Array.empty
        paths <- paths |> Array.append [|{ Been = [|{ x = 1; y = -1 }|]; Complete = false; Length = -1  }|]
        while paths |> Array.filter (fun x -> not x.Complete) |> Array.length > 0 do
            paths <-
                paths
                |> Array.map (fun path ->
                    if path.Complete then [|path|]
                    else
                    let optionsFromLast = optionsFrom[path.lastCoord]
                    optionsFromLast
                    |> Array.filter (fun (opt, len) -> not (path.Been |> Array.contains opt))
                    |> Array.map (fun (opt, len) ->
                        {
                            Been = Array.append path.Been [|opt|]
                            Length = path.Length + len
                            Complete = (opt = endGoal)
                        }
                    )
                )
                |> Array.collect (fun x -> x)
        (paths |> Array.map (fun x -> x.Length) |> Array.sortDescending |> Array.head)
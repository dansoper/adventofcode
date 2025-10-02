module Day17

open Utils

let partTwo = true

let isOutOfBounds (c: Coord) (grid: int array array): bool =
    c.x < 0 || c.y < 0 || c.x >= Array.length grid[0] || c.y >= grid.Length

let lastCoord grid = { y = (grid |> Array.length) - 1; x = (grid[0] |> Array.length) - 1 } 

type State = {
    Position: Coord
    Diff: Coord
    StraightCount: int
}

type State with
    //member x.toString = x.Position.x.ToString() + "-" + x.Position.y.ToString() + "-" + x.Diff.x.ToString() + "-" + x.Diff.y.ToString() + "-" + x.StraightCount.ToString()
    member x.toString = x.Position.x + (x.Position.y * 1000) + ((x.Diff.x + 5) * 1000000) + ((x.Diff.y + 5) * 10000000) + (x.StraightCount * 100000000)


let heatAtCoord (c: Coord) (grid: int array array): int = grid[c.y][c.x]

let work (grid: int array array) =
    let mutable scoresAndStates: Map<int, State array> = Map.empty
    let mutable previousStates: int array = Array.empty
    let mutable finalScore = 0

    let moveAndAddState (currentCost: int) (currentCoord: Coord) (newStraightCount: int) (diff: Coord) =
        let newCoord = add currentCoord diff
        if not (isOutOfBounds newCoord grid) then
            let newCost = currentCost + (heatAtCoord newCoord grid)
            if newCoord = lastCoord grid && (partTwo = false || newStraightCount > 3) then
                if finalScore = 0 || newCost < finalScore then
                    finalScore <- newCost
                    printfn "possible %A" finalScore
            else
                //printfn "new coord %A" newCoord
                let newState = { Position = newCoord; StraightCount = newStraightCount; Diff = diff }
                if not (previousStates |> Array.contains newState.toString ) then
                    //scoresAndStates <- if Map.containsKey newCost scoresAndStates then scoresAndStates else Map.add newCost Array.empty scoresAndStates
                    
                    let item = scoresAndStates |> Map.tryFind newCost
                    let newItem = 
                        match item with
                        | Some item -> item |> Array.append [|newState|]
                        | None -> Array.empty |> Array.append [|newState|]
                    scoresAndStates <- scoresAndStates |> Map.add newCost newItem

                    //scoresAndStates <- scoresAndStates |> Map.change newCost (fun x -> 
                    //        match x with
                    //        | Some x -> Some(x |> Array.append [|newState|])
                    //        | None -> Some(Array.empty |> Array.append [|newState|])
                    //    )
                    previousStates <- previousStates |> Array.append [|newState.toString|]
        //else
            //printfn "Out of bounds: %A" newCoord

    moveAndAddState 0 { x = 0; y = 0 } 1 { x = 1; y = 0 }
    moveAndAddState 0 { x = 0; y = 0 } 1 { x = 0; y = 1 }

    //printfn "Options found: %A %A" (scoresAndStates |> Map.keys) (lastCoord grid)

    let mutable lowestKeyIsGreaterThanScore = false

    while lowestKeyIsGreaterThanScore = false do
        let lowestKey = scoresAndStates |> Map.keys |> Seq.min
        let bestStates = scoresAndStates |> Map.find lowestKey
        scoresAndStates <- scoresAndStates |> Map.remove lowestKey

        if lowestKey > finalScore && finalScore > 0 then
            lowestKeyIsGreaterThanScore <- true

        printfn "Options remaining: %A, count: %A" (scoresAndStates |> Map.keys) (bestStates |> Array.length)

        bestStates
        |> Array.iter (fun item ->
            if partTwo then
                if item.StraightCount > 3 then
                    moveAndAddState lowestKey item.Position 1 { x = item.Diff.y; y = -item.Diff.x }
                    moveAndAddState lowestKey item.Position 1 { x = -item.Diff.y; y = item.Diff.x }
                if item.StraightCount < 10 then
                    moveAndAddState lowestKey item.Position (item.StraightCount + 1) item.Diff
            else
                moveAndAddState lowestKey item.Position 1 { x = item.Diff.y; y = -item.Diff.x }
                moveAndAddState lowestKey item.Position 1 { x = -item.Diff.y; y = item.Diff.x }

                if item.StraightCount < 3 then
                    moveAndAddState lowestKey item.Position (item.StraightCount + 1) item.Diff
        )

    finalScore

// 1048 too low

type PathInfo = {
    Path: Coord array
    StraightCount: int
    HeatLoss: int
    Complete: bool
    Score: int
}

type Been = {
    Coord: Coord
    From: Coord
    HeatLoss: int
}

let mutable beens: Been array = Array.empty
let mutable successfulScores: int array = Array.empty

type PathInfo with
    member x.lastCoord = Array.last x.Path
    member x.penultimateCoord = if x.Path.Length > 1 then Array.item (x.Path.Length - 2) x.Path else { x = -1; y = 0 }
    member x.lastDiff =  diff x.penultimateCoord x.lastCoord
    member x.nextOptions =
        let leftAndRight = [|add x.lastCoord (flipAndReverse x.lastDiff);add x.lastCoord (flip x.lastDiff)|]
        if x.StraightCount < 2 then Array.append leftAndRight [|add x.lastCoord x.lastDiff|] else leftAndRight


// No point going to a coordinate more than once in same route
// If approaching a coordinate from a direction that we have already done, stop if heatloss is same or higher
// Get list of options; prioritise lowest score so far
// If we have any that have reached, no point doing if heatloss is same or higher

let getOptionsForPosition (info: PathInfo) (grid: int array array) =
    //printfn "Getting options for %A" info
    let opts =
        info.nextOptions
        |> Array.filter (fun x ->
            //printfn "try %A %A %A %A %A " x (not (isOutOfBounds x grid)) (not (info.Path |> Array.exists (fun y -> y = x))) (not (beens |> Array.exists (fun y -> 
            //    y.Coord = x 
            //    && (diff info.lastCoord x) = (diff y.From y.Coord) 
            //    && y.HeatLoss <= info.HeatLoss))) (not ((successfulScores |> Array.length ) > 1 && (info.HeatLoss + (heatAtCoord x grid)) >= (successfulScores |> Array.head)))
            
            not (isOutOfBounds x grid)
            && not (info.Path |> Array.exists (fun y -> y = x))
            && not (beens |> Array.exists (fun y -> 
                y.Coord = x 
                && (diff info.lastCoord x) = (diff y.From y.Coord) 
                && y.HeatLoss <= info.HeatLoss))
            && not ((successfulScores |> Array.length ) > 1 && (info.HeatLoss + (heatAtCoord x grid)) >= (successfulScores |> Array.head))
        )
    //printfn "Options found: %A" (opts |> Array.length)
    opts


let mutable bestScoresAtPositions: Map<(Coord * Coord), int> = Map.empty

let goToPosition (info: PathInfo) (coord: Coord) (grid: int array array): PathInfo =
    let newHeatLoss = info.HeatLoss + (heatAtCoord coord grid)
    if not (bestScoresAtPositions.ContainsKey (info.lastCoord, coord)) || bestScoresAtPositions[(info.lastCoord, coord)] > newHeatLoss then
        bestScoresAtPositions <- bestScoresAtPositions |> Map.add (info.lastCoord, coord) newHeatLoss
        let lastC = lastCoord grid
        
        let complete = if coord = lastCoord grid then true else false
        if complete
            then 
                successfulScores <- successfulScores |> Array.append [|newHeatLoss|] |> Array.sort
                //printfn "Found %A %A %A" info coord grid
        let thisDiff = diff info.lastCoord coord
        let newStraightCount = if thisDiff = info.lastDiff then info.StraightCount + 1 else 0
        beens <- beens |> Array.append [|{ From = info.lastCoord; Coord = coord; HeatLoss = newHeatLoss }|]
        {
            Path = Array.append info.Path [|coord|] 
            HeatLoss = newHeatLoss
            StraightCount = newStraightCount
            Complete = complete
            Score = 10 * ((lastC.x - coord.x) + (lastC.y - coord.y)) * (1 * newHeatLoss)
        }
    else
        //printfn "Rejecting coord %A with new score %A and better score %A" coord newHeatLoss bestScoresAtPositions[coord]
        {
            Path = info.Path
            HeatLoss = 0
            StraightCount = 0
            Complete = true
            Score = 0
        }

let singleRoundOld (paths: PathInfo array) (grid: int array array) =
    paths
    |> Array.map (fun info -> (info, getOptionsForPosition info grid))
    |> Array.map (fun (info, options) ->
        options
        |> Array.map (fun option -> goToPosition info option grid)
    )
    |> Array.collect (fun x -> x)
    |> Array.filter (fun x -> not x.Complete)
    |> Array.sortBy (fun x -> x.HeatLoss)

let addOption (options: PathInfo array) (newOptions: PathInfo array) =
    //printfn "in %A %A" (options |> Array.map (fun x -> x.Score)) (newOptions |> Array.map (fun x -> x.Score))
    let firstSuccess = successfulScores |> Array.tryHead
    let firstSuccessVal =
        match firstSuccess with
        | Some (x) -> x
        | None -> 0

    let mutable nearlyOptions = 
        options
        |> Array.filter (fun x -> (not x.Complete) && (firstSuccessVal = 0 || x.HeatLoss < firstSuccessVal))

    newOptions
    |> Array.filter (fun x -> not x.Complete)
    |> Array.iter (fun item ->
        //printfn "I %A" (nearlyOptions |> Array.map (fun x -> x.Score))

        let index = Array.tryFindIndex (fun (item2: PathInfo) -> item2.Score >= item.Score) nearlyOptions
        match index with
        | Some(x) -> 
            //printfn "Found index %A %A" index item.Score
            let newX = x
            nearlyOptions <- Array.insertAt newX item nearlyOptions
        | None -> 
            //printfn "No found %A" item.Score
            nearlyOptions <- Array.append nearlyOptions [|item|]
    )

    //printfn "S %A" (nearlyOptions |> Array.map (fun x -> x.Score))

    //printfn "SS %A" (nearlyOptions |> Array.map (fun x -> x.Score) |> Array.sort)


    //printfn "HERE WE ARE %A" nearlyOptions
    nearlyOptions

let singleRound (paths: PathInfo array) (grid: int array array) (pathInfos: PathInfo array) =
    let lastC = lastCoord grid
    let firstPathInfo = paths |> Array.head
    let newOptions = getOptionsForPosition firstPathInfo grid
    newOptions
    |> Array.map (fun option -> goToPosition firstPathInfo option grid)
    |> addOption (pathInfos |> Array.skip 1) 
    //|> Array.append (pathInfos |> Array.skip 1)
    //|> Array.sortBy (fun x -> (x.HeatLoss, (lastC.x - x.lastCoord.x) + (lastC.y - x.lastCoord.y)))

let doRounds (grid: int array array) = 
    let mutable options = [|{ Path = [|{ x = 0; y = 0 }|]; HeatLoss = 0; StraightCount = 0; Complete = false; Score = 0 }|]
    let mutable i = 0
    while (options |> Array.length) > 0 do
        i <- i + 1
        options <- singleRound options grid options
        if i % 100 = 0 then printfn "Round %A and options length = %A and complete length = %A" i (Array.length options) (Array.length successfulScores)


let runDay (input: string) =
    let grid = 
        input
        |> splitByLine
        |> Array.map (fun x -> splitToGrid x)
        |> Array.map (fun x -> x |> Seq.map (fun y -> charToInt y) |> Array.ofSeq)

    work grid
    //doRounds grid
    //successfulScores |> Array.head
    //successfulScores


// Not 865, too low
// Not 941, too high
// pt2
// Not 1048
module Day17

open Utils

let partTwo = false

let isOutOfBounds (c: Coord) (grid: int array array): bool =
    c.x < 0 || c.y < 0 || c.x >= Array.length grid[0] || c.y >= grid.Length

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

let heatAtCoord (c: Coord) (grid: int array array): int = grid[c.y][c.x]

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

let lastCoord grid = { y = (grid |> Array.length) - 1; x = (grid[0] |> Array.length) - 1 } 

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

    doRounds grid
    successfulScores |> Array.head
    successfulScores


// Not 865, too low
// Not 941, too high
module Day14

open Utils

let partTwo = true

let getCols (section: char seq array): char array array =
    let mutable cols = Array.empty
    for i = 0 to (Seq.length section[0]) - 1 do
        let mutable col = Array.empty
        for j = 0 to (Array.length section) - 1 do
            let char = (Seq.item i (Array.item j section))
            col <- Array.append col [|char|]
        cols <- Array.append cols [|col|]
    cols

let rec getCubes (start: int) (fullLength: int) (col: char array): (int * int) array =
    let actLength = if fullLength < 0 then col |> Seq.length else fullLength
    let maybe = 
        col
        |> Seq.tryFindIndex (fun x -> x = '#')

    match maybe with
    | Some x -> 
        let newCol = Array.skip  (x + 1) col
        Array.append [|(start, (start + x + 1))|] (getCubes (start + x + 1) actLength newCol )
    | None -> [|start, actLength|]

let getRounded (col: char array): int array =
    col
    |> Array.indexed
    |> Array.filter (fun (index, item) -> item = 'O')
    |> Array.map (fun (index, item) -> index)

type CubesAndRounded = {
    Cubes: (int * int) array
    Rounded: int array
}

type CubeOrRounded = 
    | Cube = 0
    | Rounded = 1

type CoordItem = {
    x: int
    y: int
    item: CubeOrRounded
}

let moveCol (input: CubesAndRounded) =
    { 
        Cubes = input.Cubes
        Rounded = 
            input.Cubes
            |> Array.map (fun (start, finish) -> 
                let roundedCount = input.Rounded |> Seq.filter (fun i -> i > start && i < finish) |> Seq.length
                [|1..roundedCount|]
                |> Array.map (fun x -> start + x)
            )
            |> Array.collect (fun x -> x)
    }

let moveCols (input: CubesAndRounded array) =
    input |> Array.map moveCol

let cubesToPairs (cubes: int array) length: (int * int) array =
    if Array.length cubes = 0 
        then
            [|(-1,length)|]
        else
            (Array.append [|(-1,Array.head cubes)|] (Array.append (Array.pairwise cubes) [|(Array.last cubes, length)|]))

let rotate (input: CubesAndRounded array) =
    // Assumes the grid is square
    let len = Array.length input

    // Get all items into a single sequence
    let all = 
        input
        |> Array.mapi (fun x item ->
            let c = 
                item.Cubes
                |> Array.filter (fun (y, x) -> y >= 0)
                |> Array.map (fun (y, ignore) -> { x = x; y = y; item = CubeOrRounded.Cube })

            let r =
                item.Rounded
                |> Array.map (fun y -> { x = x; y = y; item = CubeOrRounded.Rounded })

            Array.append c r
        )
        |> Array.collect (fun r -> r)

    // Y becomes X of the old one; X becomes len - Y
    let rotated = 
        all
        |> Array.map (fun thing -> { x = (len - thing.y); y = thing.x; item = thing.item })

    [|1..len|]
    |> Array.map (fun i -> 
        let thisCol = rotated |> Array.filter (fun item -> item.x = i)
        let rounded = thisCol |> Array.filter (fun item -> item.item = CubeOrRounded.Rounded) |> Array.map (fun item -> item.y)
        let cubes = thisCol |> Array.filter (fun item -> item.item = CubeOrRounded.Cube) |> Array.map (fun item -> item.y)
        { Rounded = rounded; Cubes = cubesToPairs cubes len }
    )
    
let getScore (fullLength: int) (item: CubesAndRounded) =
    item.Rounded |> Array.map (fun y -> fullLength - y)

let getFullScore fullLength items =
    items
    |> Array.map (getScore fullLength)
    |> Array.map (fun x -> Array.sum x)
    |> Array.sum

let doPartOne (arr: CubesAndRounded array) fullLength =
    arr
    |> moveCols
    |> getFullScore fullLength

let doPartTwo (arr: CubesAndRounded array) fullLength =
    let mutable b = arr
    let mutable sums = Array.empty
    let mutable j = 0
    let mutable repeatFound = 0
    let mutable start = 0
    while repeatFound = 0 do
        j <- j + 1
        for i = 0 to 3 do
            b <- 
                b 
                |> moveCols
                |> rotate
        let item = (b
            |> Seq.map (getScore fullLength)
            |> Seq.map (fun x -> Seq.fold (fun acc cur -> String.concat "," [|acc; string cur|]) "" x)
            |> Seq.fold (fun acc cur -> String.concat "|" [|acc; string cur|]) ""
        )
        
        if Array.exists (fun x -> x = item) sums then 
            start <- (Array.findIndex (fun x -> x = item) sums + 1)
            repeatFound <- j
        else
            sums <- Array.append sums [|item|]

    printfn "Repeat found at %A %A" repeatFound start

    let numberToDo = (1000000000 - start) % (repeatFound - start)

    printfn "So we need to do %A" numberToDo

    b <- arr
    for ii = 1 to numberToDo + start do
        for i = 0 to 3 do
            b <- 
                b 
                |> moveCols
                |> rotate
        //printfn "Score %A %A" ii (b |> getFullScore fullLength)

        
    b
    |> getFullScore fullLength


let runDay (input: string) =
    let cols = 
        input
        |> splitByLine
        |> Array.map (fun x -> splitToGrid x)
        |> getCols
    
    let fullLength = Array.length (Array.item 1 cols)

    let cubes = cols |> Array.map (getCubes -1 -1)
    let rounded = cols |> Array.map getRounded
    
    let a = 
        (cubes, rounded) 
        ||> Array.map2 (fun x y -> { Cubes = x; Rounded = y })
    
    if partTwo
        then doPartTwo a fullLength
        else doPartOne a fullLength
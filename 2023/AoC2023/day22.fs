module Day22

open Utils

let partTwo = true

let toGrid (input: (Coord3D * Coord3D) array): int array array array =
    let maxX = input |> Array.collect (fun (c, d) -> [|c.x;d.x|]) |> Array.max
    let maxY = input |> Array.collect (fun (c, d) -> [|c.y;d.y|]) |> Array.max
    let maxZ = input |> Array.collect (fun (c, d) -> [|c.z;d.z|]) |> Array.max

    //z y x
    let mutable grid = Array.create (maxZ + 1) (Array.create (maxY + 1) (Array.create (maxX + 1) -1))

    input
    |> Array.iteri (fun index (fromItem, toItem) ->
        [fromItem.z..toItem.z]
        |> List.iter (fun z ->
            [fromItem.y..toItem.y]
            |> List.iter (fun y ->
                [fromItem.x..toItem.x]
                |> List.iter (fun x ->
                    //printfn "%A %A %A " x y z
                    grid <- 
                        grid |> Array.updateAt z 
                            (grid[z] |> Array.updateAt y 
                                (grid[z][y] |> Array.updateAt x index))
                )   
            )
        )
    )
    grid

let updateGrid ((fromItem1, toItem1): (Coord3D * Coord3D)) ((fromItem2, toItem2): (Coord3D * Coord3D)) (grid: int array array array): int array array array =
    let mutable mGrid = grid
    let index = mGrid[fromItem1.z].[fromItem1.y][fromItem1.x]
    [fromItem1.z..toItem1.z]
    |> List.iter (fun z ->
        [fromItem1.y..toItem1.y]
        |> List.iter (fun y ->
            [fromItem1.x..toItem1.x]
            |> List.iter (fun x ->
                mGrid <- 
                    mGrid |> Array.updateAt z 
                        (mGrid[z] |> Array.updateAt y 
                            (mGrid[z][y] |> Array.updateAt x -1))
            )   
        )
    )

    [fromItem2.z..toItem2.z]
    |> List.iter (fun z ->
        [fromItem2.y..toItem2.y]
        |> List.iter (fun y ->
            [fromItem2.x..toItem2.x]
            |> List.iter (fun x ->
                if (mGrid[z].[y][x]) <> -1 then printfn "%A" (mGrid[z].[y][x])
                mGrid <- 
                    mGrid |> Array.updateAt z 
                        (mGrid[z] |> Array.updateAt y 
                            (mGrid[z][y] |> Array.updateAt x index))
            )   
        )
    )
    mGrid

// For the test input
let showIt (grid: int array array array) =
    for z = 9 downto 0 do
        for x = 0 to 2 do
            let one = grid[z].[0][x]
            let two = grid[z].[1][x]
            let three = grid[z].[2][x]
            let view = (if one > -1 then string one else (if two > -1 then string two else if three > -1 then string three else "."))
            printf "%A" view
        printf "\n"

    printf "\n"
    for z = 9 downto 0 do
        for y = 0 to 2 do
            let one = grid[z].[y][0]
            let two = grid[z].[y][1]
            let three = grid[z].[y][2]
            let view = (if one > -1 then string one else (if two > -1 then string two else if three > -1 then string three else "."))
            printf "%A" view
        printf "\n"


let itemsAbove (grid: int array array array) ((one, two): (Coord3D * Coord3D)): int array =
    let minX = min one.x two.x
    let maxX = max one.x two.x
    let minY = min one.y two.y
    let maxY = max one.y two.y
    let maxZ = max one.z two.z
    let testItem = grid[one.z].[one.y][one.x]
    let mutable els = Array.empty
    if (maxZ + 1 < (Array.length grid)) then
        for x = minX to maxX do
            for y = minY to maxY do
                let thisItem = grid[maxZ + 1].[y][x]
                if testItem <> thisItem && thisItem <> -1 && not (Array.contains thisItem els) then
                    els <- els |> Array.append [|thisItem|]
    els

let itemsBelow (grid: int array array array) ((one, two): (Coord3D * Coord3D)): int array =
    let minX = min one.x two.x
    let maxX = max one.x two.x
    let minY = min one.y two.y
    let maxY = max one.y two.y
    let minZ = min one.z two.z
    let testItem = grid[one.z].[one.y][one.x]
    let mutable els = Array.empty
    if minZ - 1 > 0 then
        for x = minX to maxX do
            for y = minY to maxY do
                let thisItem = grid[minZ - 1].[y][x]
                if testItem <> thisItem && thisItem <> -1 && not (Array.contains thisItem els) then
                    els <- els |> Array.append [|thisItem|]
    els

let moveAllDown (items: (Coord3D * Coord3D) array) (grid: int array array array): ((Coord3D * Coord3D) array * int array array array * int) =
    let mutable mGrid = grid
    let mutable moved = 0
    let orderedItems = items |> Array.sortBy (fun (one, two) -> min one.z two.z)
    let newItems = 
        orderedItems |> Array.map (fun (one, two) -> 
            let minX = min one.x two.x
            let maxX = max one.x two.x
            let minY = min one.y two.y
            let maxY = max one.y two.y
            let minZ = min one.z two.z
            let mutable zDown = 1
            let mutable ok = true
            while ok = true && (minZ - zDown) >= 1 do
                let mutable thisRowOK = true
                for x = minX to maxX do
                    for y = minY to maxY do
                        thisRowOK <- if mGrid[minZ - zDown].[y][x] = -1 then (thisRowOK && true) else false
                if not thisRowOK
                then 
                    ok <- false
                else
                    zDown <- zDown + 1
            
            // Once we've found how low we can go, we go there
            if zDown > 1 && (minZ - zDown + 1) >= 1
            then
                let newOne = { one with z = one.z - zDown + 1 }
                let newTwo = { two with z = two.z - zDown + 1 }

                mGrid <- updateGrid (one, two) (newOne, newTwo) mGrid
                moved <- moved + 1
                (newOne, newTwo)
            else (one, two)
        )

    (newItems, mGrid, moved)


let runDay (input: string) =
    let items = 
        input
        |> splitByLine
        |> Array.map (fun line -> 
            let lineSplit = line |> split "~"
            let bits = 
                lineSplit
                |> Array.map (fun part ->
                    let partSplit = part |> split "," |> Array.map int
                    { x = partSplit[0]; y = partSplit[1]; z = partSplit[2] }
                )
            (bits[0], bits[1])
        )

    let grid = toGrid items

    // Jenga/tetris fall down
    let mutable mItems = items
    let mutable mGrid = grid
    let mutable moves = 1
    while moves > 0 do
        let (newItems, newGrid, moved) = moveAllDown mItems mGrid
        mItems <- newItems
        mGrid <- newGrid
        moves <- moved
    
    if partTwo then
        let oo = 
            mItems
            |> Array.mapi (fun x item -> 
                let mutable mmItems = mItems |> Array.removeAt x
                let mutable mmGrid = toGrid mmItems
                let mutable moves = 1
                let mutable totalMoves = 0
                while moves > 0 do
                    let (nI, nG, m) = moveAllDown mmItems mmGrid
                    mmItems <- nI
                    mmGrid <- nG
                    moves <- m
                    totalMoves <- totalMoves + m
                totalMoves
            )
        oo |> Array.sum
    else
        let version = 1
        // first option
        if version = 1 then
            // This gets the list of items above each item
            // Then for each of those items, it sees if it is the only one
            let adjusted = 
                mItems
                |> Array.map (itemsAbove mGrid)
            
            adjusted
            |> Array.map (fun item ->
                item |> Array.map (fun sub ->
                    adjusted
                    |> Array.filter (fun ints -> ints |> Array.contains sub)
                    |> Array.length
                )
            )
            |> Array.map (fun item ->
                item |> Array.fold (fun acc cur -> acc && cur <> 1) true
            )
            |> Array.filter (fun a -> a) |> Array.length
        // second option
        elif version = 2 then
            // This has a mutable important array which we fill with anything that is the only item below an item
            let mutable important = Array.empty

            let adjusted = mItems |> Array.map (itemsBelow mGrid)
            adjusted
            |> Array.iter (fun item ->
                if Array.length item = 1 then
                    important <- important |> Array.append item
            )

            important <- important |> Array.distinct

            mItems.Length - (important |> Array.length)
        else 0


    

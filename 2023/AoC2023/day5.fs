module Day5

open Utils

let partTwo = true

type Transformation = {
    Min: int64
    Max: int64
    Transformation: int64
}

type TransformationsForDestination = {
    Destination: string
    Transformations: Transformation array
}

// Get a seamless range
let expandTransformations (min: int64) (max: int64) (transformations: Transformation array): Transformation array =
    // Sort them by the Min property
    let sorted =
        transformations
        |> Array.sortBy (fun x -> x.Min)
    // Get an inner array which includes the original item, preceded and followed by a new element, if required
    sorted
    |> Array.indexed
    |> Array.collect (fun (index, x) ->
        let from = if index = 0 then min else (sorted[index - 1].Max + 1L)
        let until = if index = ((Array.length sorted) - 1) then max else (sorted[index + 1].Min - 1L)
        let one = [|x|]
        let two = if x.Min <= from then one else [|{ Min = from; Max = x.Min - 1L; Transformation = 0 }|] |> Array.append one
        let three = if x.Max >= until then two else two |> Array.append [|{ Min = x.Max + 1L; Max = until; Transformation = 0 }|]
        three
    )


// For Min / Max, get all applicable transformations. We need these Mins, Maxs, but the First Min and Last Max can be adjusted
// This function returns multiple ranges, for each applicable transformation
let performTransformationForRange ((min, max): int64 * int64) (transformations: Transformation array): (int64 * int64) array =
    let applicableTransformations = 
        transformations
        |> expandTransformations min max
        |> Array.filter (fun f -> f.Max >= min && f.Min <= max)
        |> Array.map (fun x -> { 
            Min = if x.Min < min then min else x.Min
            Max = if x.Max > max then max else x.Max
            Transformation = x.Transformation
        })
    applicableTransformations
    |> Array.map (fun x -> (x.Min + x.Transformation, x.Max + x.Transformation))

let performTransformation (incoming: int64) (transformations: Transformation array) =
    let transformation =
        transformations
        |> Array.tryFind (fun x -> x.Min <= incoming && x.Max >= incoming)
    match transformation with
        | Some(x) -> incoming + x.Transformation
        | None -> incoming

let path = ["soil";"fertilizer";"water";"light";"temperature";"humidity";"location"]

let parseSection (input: string) =
    let lines = input |> splitByLine
    let destination = lines[0].Split "to-" |> Array.item 1 |> split " " |> Array.item 0
    let otherLines = Array.removeAt 0 lines
    let transformations =
        otherLines
        |> Array.map (fun x -> 
            let bits =
                x.Split " "
                |> Array.map (fun y -> int64 y)
            { Min = bits[1]; Max = bits[1] + bits[2] - int64 1; Transformation = bits[0] - bits[1]  }
        )
    { Destination = destination; Transformations = transformations }

let forSoil mapSections num = 
    path
        |> List.fold (fun prev p ->
            let mapping =
                mapSections
                |> Array.find (fun x -> x.Destination = p)
            performTransformation prev mapping.Transformations    
        ) num

let forSoilRange mapSections ((min, max): int64 * int64) = 
    let start = [|(min, max)|]
    path
        |> List.fold (fun prev p ->
            let mapping =
                mapSections
                |> Array.find (fun x -> x.Destination = p)
            prev
            // collect flattens the inner arrays
            |> Array.collect (fun x -> performTransformationForRange x mapping.Transformations) 
        ) start

// Joins the lists together into pairs, then removes alternate items so it's a single list
let soilToSoilRanges (num: int64 array): (int64 * int64) array =
    num
    |> Array.pairwise
    |> Array.indexed
    |> Array.filter (fun (index, item) -> (index % 2) = 0)
    |> Array.map (fun x -> snd x)
    |> Array.map (fun (a, b) -> (a, a + b))
    
let runDay (input: string) =
    let sections =
        input
        |> splitByDoubleLine

    let seeds = 
        sections[0].Split " "
        |> Array.skip 1
        |> Array.map (fun x -> int64 x)
    
    let mapSections =
        Array.removeAt 0 sections
        |> Array.map parseSection
    
    if partTwo then
        seeds
        |> soilToSoilRanges
        |> Array.map (fun a -> forSoilRange mapSections a)
        // Flatten all ranges into a single array of numbers
        |> Array.collect (fun x -> x |> Array.collect (fun (y, z) -> [|y;z|]))
        |> Array.sort
        |> Array.take 1
    else
        seeds
        |> Array.map (fun a -> forSoil mapSections a)
        |> Array.sort
        |> Array.take 1

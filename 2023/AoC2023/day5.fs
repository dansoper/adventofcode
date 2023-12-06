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
(*
let performTransformationOnSamples (samples: int64 array) (transformations: Transformation array) =
    samples
    |> Array.map (fun incoming -> 
        let transformation =
            transformations
            |> Array.filter (fun x -> x.Min <= incoming && x.Max >= incoming)
        match transformation with
            | Some(x) -> incoming + x.Transformation
            | None -> incoming
        )
    *)

// For Min / Max, get all applicable transformations. We need these Mins, Maxs, but the First Min and Last Max can be adjusted


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

let megifySeeds (num: int64 array): int64 array =
    let em:int64 array = Array.empty
    
    num
    |> Array.pairwise
    |> Array.indexed
    |> Array.filter (fun (index, item) -> (index % 2) = 0)
    |> Array.map (fun (index, item) ->
        let hmm = [ for i in int64 0 .. (snd item) -> (fst item) + i]
        hmm)
    |> Array.fold (fun prev cur -> Array.append prev (Array.ofList cur)) em

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
    
    let megaSeeds = if partTwo then megifySeeds seeds else seeds

    printf "%A" (Array.length megaSeeds)

    seeds
    |> Array.map (fun a -> forSoil mapSections a)
    |> Array.sort
    |> Array.take 1

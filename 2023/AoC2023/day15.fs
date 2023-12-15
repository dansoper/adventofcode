module Day15

open Utils

let partTwo = true

let hashChar (ch: char) (cur: int) =
    let charVal = int ch
    //let charVal = int (System.Char.GetNumericValue ch)
    let stepOne = cur + charVal
    let stepTwo = stepOne * 17
    //printfn "%A %A" ch charVal
    stepTwo % 256

let hashString (str: string) =
    let mutable value = 0
    str
    |> Seq.iter (fun c -> 
        value <- hashChar c value
    )
    value

type PowerLabel = {
    Power: int
    Label: string
}

let partTwoProcessing (items: string array): int =
    // A mapping of boxes
    let mutable mapping: Map<int, PowerLabel array> = Map.empty
    
    items
    |> Array.iter (fun item -> 
        if item.EndsWith "-"
        then 
            // Find the box, see if there's an item with the label, and remove it (Array.filter)
            let label = item |> split "-" |> Array.head
            let box = hashString label
            // Make sure the box is there
            mapping <- if Map.containsKey box mapping then mapping else Map.add box Array.empty mapping
            // remove
            mapping <-
                mapping
                |> Map.change box (fun x ->
                    match x with
                    | Some s ->
                        Some (s |> Array.filter (fun y -> y.Label <> label))
                    | None -> None
                )
        else
            // Find the box, see if there's an item; replace it; or add to the end
            let label = item |> split "=" |> Array.head
            let power = item |> split "=" |> Array.last |> int
            let box = hashString label
            // Make sure the box is there
            mapping <- if Map.containsKey box mapping then mapping else Map.add box Array.empty mapping
            // Put
            mapping <- 
                mapping
                |> Map.change box (fun x -> 
                    match x with
                    | Some s -> 
                        if s |> Array.exists (fun y -> y.Label = label)
                            then
                            // Replace existing
                            Some(s |> Array.map (fun y -> if y.Label = label then { Power = power; Label = label } else y))
                            else
                            // Add new
                            Some(Array.append s [|{ Power = power; Label = label }|])
                    | None -> None
                )
        
    )
    
    mapping
    |> Map.toArray
    |> Array.map (fun (index, array) ->
        array
        |> Array.mapi (fun innerIndex item -> 
            (index + 1) * (innerIndex + 1) * item.Power
        )
        |> Array.sum
    )
    |> Array.sum

let runDay (input: string) =
    let items = 
        input
        |> split ","
    
    if partTwo
        then
            partTwoProcessing items
        else 
            items
            |> Array.map hashString
            |> Array.sum
module Day19

open Utils
open System.Text.RegularExpressions

let partTwo = true

type ProcessStep = {
    Test: char
    Operation: char
    Value: int
    Action: string
}

type Process = {
    Label: string
    Steps: ProcessStep array
}

type Part = {
    x: int
    m: int
    a: int
    s: int
}

let parseToGroups input =
    // "([a-z]+){((([xmas])[><]([0-9]+):)?(A|R|[a-z]+)[,]?)+}"
    let mappings =
        Regex.Matches (input, "([a-z]+){(([a-z<>0-9:RA]+[,]?)+)}")
        |> Seq.cast<Match>
        |> Seq.map (fun x -> x.Groups)
        |> Seq.map (fun x -> 
            let detail = x[2].Value
            let gps = 
                detail
                |> split ","
                |> Array.map (fun item -> 
                    Regex.Matches (item, "(([xmas])([><])([0-9]+):)?(A|R|[a-z]+)")
                    |> Seq.cast<Match>
                    |> Seq.map (fun x -> x.Groups)
                    |> Seq.map (fun x -> 
                    {
                        Test = if x[2].Value = "" then '-' else char x[2].Value
                        Operation = if x[3].Value = "" then '-' else char x[3].Value
                        Value = if x[4].Value = "" then 0 else int x[4].Value
                        Action = x[5].Value
                    })
                    |> Array.ofSeq
                )
                |> Array.collect (fun x -> x)
            (x[1].Value, { Label = x[1].Value; Steps = gps }))
        |> Map.ofSeq
    mappings

let parseToParts input =
    let mappings =
        Regex.Matches (input, "{x=([0-9]+),m=([0-9]+),a=([0-9]+),s=([0-9]+)}")
        |> Seq.cast<Match>
        |> Seq.map (fun x -> x.Groups)
        |> Seq.map (fun x -> 
            { x = int x[1].Value; m = int x[2].Value; a = int x[3].Value; s = int x[4].Value })
        |> Array.ofSeq
    mappings

let eq a b = a = b
let gt a b = a > b
let lt a b = a < b

let actOnPart (input: Part) (mappings: Map<string, Process>) =
    let mutable goForLabel = "in"
    let mutable complete = false
    let mutable success = 0
    while complete = false do
        let thisProcess = mappings[goForLabel]
        let foundStep = 
            thisProcess.Steps
            |> Array.find (fun step ->
                let test = 
                    match step.Test with
                    | '-' -> 0
                    | 'x' -> input.x
                    | 'm' -> input.m
                    | 'a' -> input.a
                    | 's' -> input.s
                    | _ -> failwith "chocolate"
                let operation =
                    match step.Operation with
                    | '-' -> eq
                    | '>' -> gt
                    | '<' -> lt
                    | _ -> failwith "christmas pudding"
                
                operation test step.Value
            )    
        if foundStep.Action = "A"
            then
                success <- 1
                complete <- true
        elif foundStep.Action = "R"
            then
                success <- -1
                complete <- true
        else
            goForLabel <- foundStep.Action
    (input, success)

let isSuccessful (whichOne: char) (input: int) (mappings: Map<string, Process>) =
    let mutable goForLabel = "in"
    let mutable complete = false
    let mutable success = 0
    while complete = false do
        let thisProcess = mappings[goForLabel]
        let foundStep = 
            thisProcess.Steps
            |> Array.find (fun step ->
                let operation =
                    match step.Operation with
                    | '-' -> eq
                    | '>' -> gt
                    | '<' -> lt
                    | _ -> failwith "christmas pudding"
                if step.Test = whichOne
                    then operation input step.Value
                    else true
            )    
        if foundStep.Action = "A"
            then
                success <- 1
                complete <- true
        elif foundStep.Action = "R"
            then
                success <- -1
                complete <- true
        else
            goForLabel <- foundStep.Action
    success = 1

let findRangesForScore (input: char) (mappings: Map<string, Process>) =
    let mutable goForLabel = "in"
    
    mappings.Values
    |> Array.ofSeq
    |> Array.map (fun thisProcess ->
        thisProcess.Steps
        |> Array.map (fun step ->
            if step.Test = input
                then [|step.Value;if step.Operation = '>' then step.Value + 1 else step.Value - 1|]
                else [||]
        )
        |> Array.collect (fun x -> x)
    )
    |> Array.collect (fun x -> x)
    |> Array.append [|1;4000|]
    |> Array.sort
        

let runDay (input: string) =
    let processes = 
        input
        |> parseToGroups

    let parts =
        input
        |> parseToParts

    if partTwo then
       (* let options = 
            [|'x';'m';'a';'s'|]
            |> Array.map (fun char ->
                let oo = findRangesForScore char processes
                let pairs = (oo |> Array.pairwise |> Array.indexed |> Array.filter (fun (index, item) -> index % 2 = 0) |> Array.map (fun (index, item) -> item))
                pairs
                |> Array.map(fun (start, finish) -> 
                if isSuccessful 'x' start processes
                    then finish - start + 1
                    else 0
                )
                |> Array.sum
            )
        printfn "%A" options

        printfn "%A" (findRangesForScore 'm' processes)
        printfn "%A" (findRangesForScore 'a' processes)
        printfn "%A" (findRangesForScore 's' processes)*)

        let pairsOfPairs = 
            [|'x';'m';'a';'s'|]
            |> Array.map (fun char ->
                let oo = findRangesForScore char processes
                let pairs = (oo |> Array.pairwise |> Array.indexed |> Array.filter (fun (index, item) -> index % 2 = 0) |> Array.map (fun (index, item) -> item))
                pairs
            )

        let all = 
            pairsOfPairs
            |> Array.item 0
            |> Array.map (fun (x, endX) -> 
                pairsOfPairs
                |> Array.item 1
                |> Array.map (fun (m, endM) -> 
                    pairsOfPairs
                    |> Array.item 2
                    |> Array.map (fun (a, endA) -> 
                        pairsOfPairs
                        |> Array.item 3
                        |> Array.map (fun (s, endS) -> 
                            { x = x; m = m; a = a; s = s}
                        )
                    )
                    //|> Array.collect (fun x -> x)
                )
               // |> Array.collect (fun x -> x)
            )
           // |> Array.collect (fun x -> x)

        printfn "%A %A" all (Array.length all)
        23
    else 
        parts
        |> Array.map (fun x -> actOnPart x processes)
        |> Array.filter (fun x -> snd x > 0)
        |> Array.map (fun (part, x) -> part.x + part.m + part.a + part.s)
        |> Array.sum
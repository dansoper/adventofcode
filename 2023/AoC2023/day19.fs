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

type PartRange = {
    xr: (int * int)
    mr: (int * int)
    ar: (int * int)
    sr: (int * int)
    result: int
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

let splitRange (range: (int * int)) at: (int * int) array =
    [|(fst range, at - 1);(at, snd range)|]

let findAndSplitRange (ranges: (int * int) array) (at: int): (int * int) array =
    ranges
    |> Array.map (fun (r1, r2) ->
        if r1 < at && r2 > at then
            splitRange (r1, r2) at
        else [|(r1, r2)|]
    )
    |> Array.collect (fun x -> x)

let getProperty (range: PartRange) (test: char): (int * int) =
    match test with
    | '-' -> failwith "mince"
    | 'x' -> range.xr
    | 'm' -> range.mr
    | 'a' -> range.ar
    | 's' -> range.sr
    | _ -> failwith "chocolate"

let filterFailures (allRanges: PartRange array) (operation: char) (test: char) (value: int): PartRange array =
    allRanges
    |> Array.filter (fun range ->
        // If it's a > and the max of a range is that number or lower
        // Or if its a < and the min of a range is that number or higher
        if operation = '>' then
            snd (getProperty range test) <= value
        elif operation = '<' then
            fst (getProperty range test) >= value
        else failwith "cheese"
    )

let filterPasses (allRanges: PartRange array) (operation: char) (test: char) (value: int): PartRange array =
    allRanges
    |> Array.filter (fun range ->
        // If it's a > and the min of a range is that number or higher
        // Or if its a < and the max of a range is that number or lower
        if operation = '>' then
            fst (getProperty range test) >= value
        elif operation = '<' then
            snd (getProperty range test) <= value
        else failwith "cheese"
    )

let shortenRange (range: int * int) (thisTest: char) (operation: char) (test: char) (value: int) (toPass: bool): int * int =
    if thisTest = test then
        if operation = '>' then
            if toPass then
                (value + 1, snd range)
            else
                (fst range, value)
        elif operation = '<' then
            if toPass then
                (fst range, value - 1)
            else
                (value, snd range)
        else failwith "button"
    else range

let rec rangesForLabel (label: string) (ranges: PartRange array) (mappings: Map<string, Process>): PartRange array =
    //printfn "%A" ranges
    let mutable currentRanges = ranges
    let mutable finalRanges = Array.empty
    let processToUse = mappings[label]
    processToUse.Steps
    |> Array.iter (fun step ->
        if step.Operation = '>' || step.Operation = '<' then
            let rangesThatWouldDefinitelyFail = filterFailures currentRanges step.Operation step.Test step.Value
            let rangesThatWouldDefinitelyPass = filterPasses currentRanges step.Operation step.Test step.Value
            let rangesToSplit = currentRanges |> Array.filter (fun x -> not((Array.contains x rangesThatWouldDefinitelyFail) && (Array.contains x rangesThatWouldDefinitelyPass)))
            let failedSplitRanges = 
                rangesToSplit
                |> Array.map (fun r ->
                    { r with
                        xr = shortenRange r.xr 'x' step.Operation step.Test step.Value false
                        mr = shortenRange r.mr 'm' step.Operation step.Test step.Value false
                        ar = shortenRange r.ar 'a' step.Operation step.Test step.Value false
                        sr = shortenRange r.sr 's' step.Operation step.Test step.Value false
                    }
                )
            let passedSplitRanges = 
                rangesToSplit
                |> Array.map (fun r ->
                    { r with
                        xr = shortenRange r.xr 'x' step.Operation step.Test step.Value true
                        mr = shortenRange r.mr 'm' step.Operation step.Test step.Value true
                        ar = shortenRange r.ar 'a' step.Operation step.Test step.Value true
                        sr = shortenRange r.sr 's' step.Operation step.Test step.Value true
                    }
                )
            
            printfn "Lens %A %A %A %A" (Array.length rangesThatWouldDefinitelyFail) (Array.length rangesThatWouldDefinitelyPass) (Array.length failedSplitRanges) (Array.length passedSplitRanges)

            currentRanges <- Array.append rangesThatWouldDefinitelyFail failedSplitRanges
            let finalRange = Array.append rangesThatWouldDefinitelyPass passedSplitRanges
            if step.Action = "A" then
                finalRanges <- finalRanges |> Array.append (finalRange |> Array.map (fun x -> { x with result = 1 }))
            elif step.Action = "R" then
                finalRanges <- finalRanges |> Array.append (finalRange |> Array.map (fun x -> { x with result = -1 }))
            else 
                //printfn "Going2 in with %A %A" step.Action finalRange
                finalRanges <- finalRanges |> Array.append (rangesForLabel step.Action finalRange mappings)
        elif step.Operation = '-' then
            if step.Action = "A" then
                finalRanges <- finalRanges |> Array.append (currentRanges |> Array.map (fun x -> { x with result = 1 }))
                currentRanges <- Array.empty
            elif step.Action = "R" then
                finalRanges <- finalRanges |> Array.append (currentRanges |> Array.map (fun x -> { x with result = -1 }))
                currentRanges <- Array.empty
            else 
                //printfn "Going in with %A %A" step.Action currentRanges
                finalRanges <- finalRanges |> Array.append (rangesForLabel step.Action currentRanges mappings)
                currentRanges <- Array.empty
        else failwith "chicken"
    )

    finalRanges

(*let getRangesForProcess (input: Process) (ranges: PartRanges): PartRanges =
    input.Steps
    |> Array.fold (fun acc step ->
        if step.Operation = '>' || step.Operation = '<' then
            let v = if step.Operation = '>' then (step.Value + 1) else step.Value
            match step.Test with
            | '-' -> acc
            | 'x' -> { acc with xr = findAndSplitRange acc.xr v }
            | 'm' -> { acc with mr = findAndSplitRange acc.mr v }
            | 'a' -> { acc with ar = findAndSplitRange acc.ar v}
            | 's' -> { acc with sr = findAndSplitRange acc.sr v }
            | _ -> failwith "chocolate"
        else acc
    ) ranges
*)
    

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



let runDay (input: string) =
    let processes = 
        input
        |> parseToGroups

    let parts =
        input
        |> parseToParts

    if partTwo then
        let arr = 
            rangesForLabel "in" [|{
                xr = (1, 4000)
                mr = (1, 4000)
                ar = (1, 4000)
                sr = (1, 4000)
                result = 0
            }|] processes
        
        printfn "%A" arr
        
        let sum =
            arr
            |> Array.filter (fun c -> c.result = 1)
            |> Array.map(fun r ->
                (int64 (snd r.xr - fst r.xr) + 1L)
                * (int64 (snd r.mr - fst r.mr) + 1L)
                * (int64 (snd r.ar - fst r.ar) + 1L)
                * (int64 (snd r.sr - fst r.sr) + 1L)
            )
            |> Array.sum
        printfn "%A" sum
        23
    else 
        parts
        |> Array.map (fun x -> actOnPart x processes)
        |> Array.filter (fun x -> snd x > 0)
        |> Array.map (fun (part, x) -> part.x + part.m + part.a + part.s)
        |> Array.sum
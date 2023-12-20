module Day8

open Utils
open System.Text.RegularExpressions

let partTwo = false

type Mapping = {
    From: string
    L: string
    R: string
}

let allEndWithZ (arr: string seq): bool =
    arr |> Seq.filter (fun x -> x.EndsWith "Z") |> Seq.length = (Seq.length arr)

let runDay (input: string) =
    let sections = 
        input
        |> splitByDoubleLine
    let lrString = sections |> Array.head
    let lrs =
        lrString |> Seq.map (fun x -> x) |> Array.ofSeq

    let mappingsString = sections |> Array.last

    let mappings =
        Regex.Matches (mappingsString, "([A-Z0-9]{3}) = \\(([A-Z0-9]{3}), ([A-Z0-9]{3})\\)")
        |> Seq.cast<Match>
        |> Seq.map (fun x -> x.Groups)
        |> Seq.map (fun x -> (x[1].Value, { From = x[1].Value; L = x[2].Value; R = x[3].Value }))
        |> Map.ofSeq

    let mutable reached = false
    let mutable steps = 0

    if partTwo then
        (* So that's a no! 
        let mutable ats = mappings |> Seq.map (fun x -> x.Key) |> Seq.filter (fun x -> x.EndsWith "A")
        while not reached do
            let index = steps % (Seq.length lrs)
            steps <- steps + 1
            let l = lrs[index]
            ats <- ats |> Seq.map (fun x -> if l = 'L' then mappings[x].L else mappings[x].R)
            reached <- (allEndWithZ ats)
        *)
        let ats = mappings |> Seq.map (fun x -> x.Key) |> Seq.filter (fun x -> x.EndsWith "A")
        ats
        |> Seq.map (fun x -> 
            let mutable at = x
            reached <- false
            steps <- 0
            while not reached do
                let index = steps % (Seq.length lrs)
                steps <- steps + 1
                let l = lrs[index]
                at <- if l = 'L' then mappings[at].L else mappings[at].R
                reached <- if at.EndsWith "Z" then true else false
            steps
        )
        |> Array.ofSeq
        |> Array.fold (fun acc cur -> lcm acc cur) 1
    else
        let mutable at = "AAA"
        while not reached do
            let index = steps % (Seq.length lrs)
            steps <- steps + 1
            let l = lrs[index]
            at <- if l = 'L' then mappings[at].L else mappings[at].R
            reached <- if at = "ZZZ" then true else false
        steps


    
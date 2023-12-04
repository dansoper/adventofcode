module Day4

open Utils

let partTwo = true

let parseToNumberList (str: string) =
    str.Split " "
    |> Array.map (fun x -> x.Trim())
    |> Array.filter (fun x -> x <> "")
    |> Array.map int

let getScoreForLine (line: string) =
    let colon = line.Split ": "
    let halves = colon[1].Split " | "
    let winners = parseToNumberList halves[0]
    let mine = parseToNumberList halves[1]
    if not partTwo
        then
            // Part 1 score is more complex
            mine
            |> Array.fold (fun soFar item ->
                if Array.exists (fun b -> b = item) winners
                    then 
                        if soFar = 0
                            then 1
                            else soFar * 2
                    else soFar
            ) 0
        else
            // Part 2 score - just the count
            mine
            |> Array.fold (fun soFar item ->
                if Array.exists (fun b -> b = item) winners
                    then soFar + 1
                    else soFar
            ) 0

let runDay (input: string) =
    let scores = 
        input
        |> splitByLine
        |> Array.map getScoreForLine

    if partTwo
        then
            let mutable m = Map.empty 
            scores
            |> Array.indexed
            |> Array.iter (fun (meIndex, item) ->
                // This card has at least 1
                m <- m.Add(meIndex, if m.ContainsKey meIndex then m[meIndex] + 1 else 1)
                // For each card
                for j = 0 to (if m.ContainsKey meIndex then m[meIndex] else 0) - 1 do
                    // For each score
                    for i = 0 to item - 1 do  
                        // Add to the following cards
                        let index = meIndex + i + 1
                        m <- m.Add(index, if m.ContainsKey index then m[index] + 1 else 1)
            )

            m
            |> Map.toSeq
            |> Seq.map (fun (key, value) -> value)
            |> Seq.sum
        else
            scores
            |> Array.sum
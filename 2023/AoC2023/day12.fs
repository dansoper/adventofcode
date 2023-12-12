module Day12

open Utils

let partTwo = false

let isPossible (str: string) (template: int array): bool =
    let holes =
        str
        |> split "."
        |> Array.filter (fun x -> x <> "")
    let holesLength = ((holes |> Array.fold (fun acc cur -> acc + cur.Length) 0) + ((holes |> Array.length) - 1))
    let templateLength = ((template |> Array.fold (fun acc cur -> acc + cur) 0) + ((template |> Array.length) - 1))
    holesLength >= templateLength

let groupWouldFit (str: string) firstGroupLength = 
    let subString = str[0..(firstGroupLength-1)]
    (subString |> Seq.filter (fun x -> x <> '.') |> Seq.length) = subString.Length && (str.Length <= firstGroupLength || str[firstGroupLength] <> '#')

// memoization
let mutable memo: Map<string, int64> = Map.empty

let makeString (str: string) (template: int array) =
    let second = (template |> Array.map (fun x -> string x) |> String.concat ",")
    str + " " + second

let rec numberOfOptions (str: string) (template: int array): int64 =
    let madeString = makeString str template
    if memo.ContainsKey madeString
        then
        memo[madeString]
    else
        let result = 
            // If the template won't fit, give up now
            if not (isPossible str template)
                then 0L
            // If the template is empty, we're done!
            elif template |> Array.length = 0
                then
                    if not (str.Contains "#") then 1L else 0L
            else
                match str[0] with
                | '#' ->
                        let firstGroupLength = Array.head template
                        if groupWouldFit str firstGroupLength
                            then  
                                // If it starts with # and the length works, we now recursively check the shorter string
                                let newString = if str.Length > firstGroupLength then str.Substring (firstGroupLength + 1) else ""
                                numberOfOptions newString (Array.removeAt 0 template) // good
                            else
                                // Give up if the template can't fit here
                                0L // bad
                | '.' -> numberOfOptions (str.Substring 1) template
                | '?' -> numberOfOptions ("#" + str.Substring 1) template + numberOfOptions (str.Substring 1) template
                | _ -> failwith "Wrong character!"
        memo <- Map.add madeString result memo
        result

let expandString str: string =
    if partTwo then
        let tooMuch = (str + "?") |> String.replicate 5
        tooMuch.Substring(0, tooMuch.Length - 1)
    else str

let expandTemplate (template: int array): int array =
    if partTwo then
        template |> Array.replicate 5 |> Array.concat
    else template

let runDay (input: string) =
    input
    |> splitByLine
    |> Array.map (fun x -> 
        let bits = x |> split " "

        let str = bits |> Array.head |> expandString

        let template = 
            bits
            |> Array.last
            |> split ","
            |> Array.map (fun x -> int x)
            |> expandTemplate

        numberOfOptions str template
    )
    |> Array.map (fun x -> int64 x)
    |> Array.sum
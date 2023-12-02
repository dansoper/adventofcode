module Day1

open Utils
open System
open System.Text.RegularExpressions

let partTwo = true

let isFirstOrLast len (index, item) = index = 0 || index = len - 1

let filterForFirstOrLast seq = 
    let len = Seq.length seq
    seq
    |> Seq.filter (isFirstOrLast len)

let combineNumbersToStringAndBackToNumber (item: int seq): int =
    let str1 = item |> Seq.head |> string
    let str2 = item |> Seq.last |> string
    let combined = str1 + str2
    combined |> int

let replaceWordNumbers item = 
    item
    |> replaceString "one" "o1e"
    |> replaceString "two" "t2o"
    |> replaceString "three" "t3e"
    |> replaceString "four" "f4r"
    |> replaceString "five" "f5e"
    |> replaceString "six" "s6x"
    |> replaceString "seven" "s7n"
    |> replaceString "eight" "e8t"
    |> replaceString "nine" "n9e"

// turns out this doesn't do the job, as twone should count as tWO AND One!
let replaceWordNumbersWithRegex item = 
    let matchReplacer (regexMatch: Match) =
        match regexMatch.Value with
        | "one" -> "1"
        | "two" -> "2"
        | "three" -> "3"
        | "four" -> "4"
        | "five" -> "5"
        | "six" -> "6"
        | "seven" -> "7"
        | "eight" -> "8"
        | "nine" -> "9"
        | _ -> ""

    Regex.Replace(item, "one|two|three|four|five|six|seven|eight|nine", MatchEvaluator(matchReplacer))


let processLine line =
    let alteredLine: string = if partTwo then replaceWordNumbers line else line
    
    alteredLine
    |> Seq.filter Char.IsDigit
    |> Seq.map charToInt
    |> Seq.indexed
    |> filterForFirstOrLast
    |> Seq.map (fun i -> snd(i))
    |> combineNumbersToStringAndBackToNumber

let runDay (input: string) =
    input
    |> splitByLine
    |> Array.map processLine
    |> Array.sum
    

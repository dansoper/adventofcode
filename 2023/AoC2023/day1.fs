module Day1

open System
open System.Text.RegularExpressions

let isFirstOrLast len (index, item) = index = 0 || index = len - 1
let charToInt c = int c - int '0'

let filterForFirstOrLast seq = 
    let len = Seq.length seq
    seq
    |> Seq.filter (isFirstOrLast len)

let combineNumbersToStringAndBackToNumber (item: int seq): int =
    let str1 = item |> Seq.head |> string
    let str2 = item |> Seq.last |> string
    let combined = str1 + str2
    combined |> int

let replaceString (oldValue: string) newValue (message: string) =
    message.Replace(oldValue, newValue)

let replaceWordNumbers item = 
    item
    |> replaceString "one" "one1one"
    |> replaceString "two" "two2two"
    |> replaceString "three" "three3three"
    |> replaceString "four" "four4four"
    |> replaceString "five" "five5five"
    |> replaceString "six" "six6six"
    |> replaceString "seven" "seven7seven"
    |> replaceString "eight" "eight8eight"
    |> replaceString "nine" "nine9nine"

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
    let partTwo = true
    let alteredLine: string = if partTwo then replaceWordNumbers line else line
    
    alteredLine
    |> Seq.toList
    |> Seq.filter Char.IsDigit
    |> Seq.map charToInt
    |> Seq.indexed
    |> filterForFirstOrLast
    |> Seq.map (fun i -> snd(i))
    |> combineNumbersToStringAndBackToNumber

let runDay (input: string) =
    let lines = input.Split([|"\n"|], StringSplitOptions.None)

    lines
    |> Array.map processLine
    |> Array.sum
    

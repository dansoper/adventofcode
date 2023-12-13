module Day13

open Utils
open System
open System.Text

let partTwo = true

type ColOrRow =
    | Col = 0
    | Row = 1

type ColsAndRows = {
    Cols: int array
    Rows: int array
}

let binaryStringToNumber str = Convert.ToInt32 (string str,  2)
let numberToBinaryString (str: int) = Convert.ToString (str,  2)


let getColsAndRowsForSection (section: char seq array): ColsAndRows =
    let rows =
        section
        |> Array.map binaryStringToNumber

    let mutable colsStrings = Array.empty
    for i = 0 to (Seq.length section[0]) - 1 do
        let sb = new StringBuilder()
        for j = 0 to (Seq.length section) - 1 do
            let char = (Seq.item i (Seq.item j section))
            sb.Append char |> ignore
        let answer = sb.ToString()
        colsStrings <- Array.append colsStrings [|answer|]

    let cols = colsStrings |> Array.map binaryStringToNumber

    { Cols = cols; Rows = rows }

// given an index (e.g. 5) it will return two below that and one above, and then keep going (e.g 3,6 2,7 1,8)
let indexPairs (start: int): (int * int) seq =
    [|for i = start - 2 downto 0 do (i, start + (start - i) - 1)|]

// If the strings are different by only one character, then return true
let stringComparison (one: string) (two: string) =
    let length = Math.Max (one.Length,  two.Length)
    let paddedOne = one.PadLeft(length, '0')
    let paddedTwo = two.PadLeft(length, '0')

    paddedOne
    |> Seq.mapi (fun index item -> if paddedTwo[index] = item then 0 else 1)
    |> Seq.sum = 1

// If lenient is true, the numbers might be equal but with one binary bit flipped
let equal one two lenient =
    if one = two then true
    else 
        if lenient
        then 
            let oneString = numberToBinaryString(one)
            let twoString = numberToBinaryString(two)
            stringComparison oneString twoString
        else false

let findSymmetryForSection (data: int seq) lenient avoid =
    let maybe =
        data 
        |> Seq.indexed
        |> Seq.tryFindIndex (fun (index,  item) ->
            if index > 0 && index <> avoid
            then
                let one = (Seq.item (index - 1) data)
                let two = item
                // Make it mutable so we only use it once
                let mutable tryLenient = lenient
                if equal one two tryLenient
                then 
                    tryLenient <- if one <> two then false else tryLenient
                    let pairs = indexPairs index
                    pairs |> Seq.fold (fun acc (l, r) -> 
                        if (Seq.length data) <= r 
                        then
                            acc
                        else
                            let one = (Seq.item l data)
                            let two = (Seq.item r data)
                            let result = acc && equal one two tryLenient
                            tryLenient <- if one <> two then false else tryLenient
                            result
                        ) true
                else false
            else false
        )
    match maybe with
    | Some(x) -> x
    | None -> -1

let findSymmetryForData (data: ColsAndRows) =
    let forRow = findSymmetryForSection data.Rows false -1
    let forCol = findSymmetryForSection data.Cols false -1

    let actualForRow = 
        if partTwo
        then findSymmetryForSection data.Rows true forRow
        else forRow
    let actualForCol = 
        if partTwo
        then findSymmetryForSection data.Cols true forCol
        else forCol

    if actualForRow >= 0 then (ColOrRow.Row, actualForRow) else (ColOrRow.Col, actualForCol)

let runDay (input: string) =
    input
    |> replaceString "." "0"
    |> replaceString "#" "1"
    |> splitByDoubleLine
    |> Array.map (fun x -> splitByLine x)
    |> Array.map (fun x -> Array.map (fun y -> splitToGrid y) x)
    |> Array.map getColsAndRowsForSection
    |> Array.map findSymmetryForData
    |> Array.map (fun (c, n) -> if c = ColOrRow.Col then n else (n * 100))
    |> Array.sum
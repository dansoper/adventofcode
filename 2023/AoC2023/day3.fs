module Day3

open System
open Utils

let partTwo = true

type ElementType =
    | Nothing = 0
    | Number = 1
    | Symbol = 2
    | Gear = 3

type Element = {
    Type: ElementType
    X: int
    Y: int
    Number: int
}

let typeFromChar char =
    if char = '.'
        then ElementType.Nothing
        else if Char.IsDigit char
            then ElementType.Number
            else if char = '*' then ElementType.Gear else ElementType.Symbol

let numLength (num: int): int = int (string num).Length

let numberFromCharSeq (arr: Char seq): int =
    let len = Seq.length arr
    arr
    |> Seq.indexed
    |> Seq.fold (fun soFar (index, item) -> soFar + ((pown 10 (len - index - 1)) * charToInt item)) 0

let numberFromChar (char: Char) (arr: Char seq) (index: int) = 
    let isFirst = index = 0 || not (Char.IsDigit(Seq.item (index - 1) arr))
    if Char.IsDigit char && isFirst
        then
        let possibleEndIndex = 
            arr
            |> Seq.skip index
            |> Seq.tryFindIndex (fun item -> not (Char.IsDigit item))
        let endIndex = 
            match possibleEndIndex with
            | Some(x) -> x
            | None -> Seq.length arr - index
        let num = arr |> Seq.skip index |> Seq.take endIndex
        numberFromCharSeq num
        else -1

let filterBySymbols (arr: Element seq) : Element seq =
    arr
    |> Seq.filter (fun num -> 
        let minX = num.X - 1
        let maxX = num.X + numLength num.Number
        let minY = num.Y - 1
        let maxY = num.Y + 1
        Seq.exists (fun sym -> 
            (sym.Type = ElementType.Gear || sym.Type = ElementType.Symbol) && sym.X >= minX && sym.X <= maxX && sym.Y >= minY && sym.Y <= maxY
        ) arr)

let filterByGears (arr: Element seq) : Element seq =
    arr
    |> Seq.filter (fun x -> x.Type = ElementType.Gear)
    |> Seq.map (fun gear ->
        let nearby = 
            Seq.filter (fun num ->
            let minX = num.X - 1
            let maxX = num.X + numLength num.Number
            let minY = num.Y - 1
            let maxY = num.Y + 1
            num.Type = ElementType.Number && gear.X >= minX && gear.X <= maxX && gear.Y >= minY && gear.Y <= maxY
            ) arr
        
        if (Seq.length nearby) = 2
        then { Type = gear.Type; X = gear.X; Y = gear.Y; Number = Seq.fold (fun soFar el -> el.Number * soFar) 1 nearby }
        else gear
        )
    |> Seq.filter (fun x -> x.Number > 0)

let toElements ((y, item): int * Char seq) =
    item
    |> Seq.mapi (fun x subItem -> 
        { Type = typeFromChar subItem; X = x; Y = y; Number = numberFromChar subItem item x})
    |> Seq.map (fun x -> { Type = (if x.Type = ElementType.Number && x.Number = -1 then ElementType.Nothing else x.Type); X = x.X; Y = x.Y; Number = x.Number })
    |> Seq.filter (fun x -> x.Type <> ElementType.Nothing)

let runDay (input: string) =
    let elements =
        input
        |> splitByLine
        |> Array.map splitToGrid
        |> Array.indexed
        |> Array.map toElements
        |> Seq.fold Seq.append Seq.empty<Element>

    if partTwo 
        then
            elements
            |> filterByGears 
            |> Seq.filter (fun x -> x.Type = ElementType.Gear)
            |> Seq.map (fun a -> a.Number)
            |> Seq.sum
        else
            elements
            |> filterBySymbols 
            |> Seq.filter (fun x -> x.Type = ElementType.Number)
            |> Seq.map (fun a -> a.Number)
            |> Seq.sum

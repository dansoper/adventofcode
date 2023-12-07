module Day7

open Utils

let partTwo = true

type MatchType =
    | HighCard = 0
    | OnePair = 1
    | TwoPair = 2
    | ThreeOfAKind = 3
    | FullHouse = 4
    | FourOfAKind = 5
    | FiveOfAKind = 6

let isFiveOfAKind (hand: char seq) =
    hand
    |> Seq.filter (fun x -> x = (hand |> Seq.head))
    |> Seq.length = 5

let isFourOfAKind (hand: char seq) =
    hand
    |> Seq.exists (fun x -> (hand |> Seq.filter (fun y -> y = x) |> Seq.length = 4))

let isFullHouse (hand: char seq) =
    hand
    |> Seq.exists (fun x -> (hand |> Seq.filter (fun y -> y = x) |> Seq.length = 3))
    &&
    hand
    |> Seq.exists (fun x -> (hand |> Seq.filter (fun y -> y = x) |> Seq.length = 2))

let isThreeOfAKind (hand: char seq) =
    hand
    |> Seq.exists (fun x -> (hand |> Seq.filter (fun y -> y = x) |> Seq.length = 3))
    && not (hand
    |> Seq.exists (fun x -> (hand |> Seq.filter (fun y -> y = x) |> Seq.length = 2)))

let isTwoPair (hand: char seq) =
    hand
    |> Seq.filter (fun x -> (hand |> Seq.filter (fun y -> y = x) |> Seq.length = 2))
    |> Seq.length = 4 // 4 because it will be found twice!

let isOnePair (hand: char seq) =
    hand
    |> Seq.filter (fun x -> (hand |> Seq.filter (fun y -> y = x) |> Seq.length = 2))
    |> Seq.length = 2 // 2 because it wil be found twice!

let getTypeForHand (hand: char seq): MatchType =
    if isFiveOfAKind hand
    then MatchType.FiveOfAKind
    elif isFourOfAKind hand
    then MatchType.FourOfAKind
    elif isFullHouse hand
    then MatchType.FullHouse
    elif isThreeOfAKind hand
    then MatchType.ThreeOfAKind
    elif isTwoPair hand
    then MatchType.TwoPair
    elif isOnePair hand
    then MatchType.OnePair
    else MatchType.HighCard

let getJokerTypeForHand (hand: char seq): MatchType =
    let noJs =
        hand
        // Remove J
        |> Seq.filter (fun x -> x <> 'J')
    let length = Seq.length noJs
    // If only 1 char: then we have 5 of a kind
    if length = 1
    then MatchType.FiveOfAKind
    // If only 2 chars: if they are the same, we have 5 of a kind, otherwise we have 4 of a kind
    elif length = 2
    then 
        if getTypeForHand noJs = MatchType.OnePair
        then MatchType.FiveOfAKind
        else MatchType.FourOfAKind
    // If only 3 chars: if they are all different, then three of a kind; if all same, 5 of kind; otherwise, 4 of a kind
    elif length = 3
    then
        if getTypeForHand noJs = MatchType.HighCard
        then MatchType.ThreeOfAKind
        elif getTypeForHand noJs = MatchType.ThreeOfAKind
        then MatchType.FiveOfAKind
        else MatchType.FourOfAKind
    // If only 4 chars:
    elif length = 4
    then
        let t = getTypeForHand noJs
        // If FourOfAKind, then 5 of a kind
        if t = MatchType.FourOfAKind then MatchType.FiveOfAKind
        // if TwoPairs, then isFullHouse;
        elif t = MatchType.TwoPair then MatchType.FullHouse
        // if ThreeOfAKind, then 4 of a kind
        elif t = MatchType.ThreeOfAKind then MatchType.FourOfAKind
        // If OnePair, then 3 of a kind
        elif t = MatchType.OnePair then MatchType.ThreeOfAKind
        // If HighCard, then OnePair
        else MatchType.OnePair
    else getTypeForHand hand

type Hand = {
    Value: string
    Bid: int
    MatchType: MatchType
}

let mapCharToScore char =
    match char with
    | '2' -> 1
    | '3' -> 2
    | '4' -> 3
    | '5' -> 4
    | '6' -> 5
    | '7' -> 6
    | '8' -> 7
    | '9' -> 8
    | 'T' -> 9
    | 'J' -> if partTwo then 0 else 10
    | 'Q' -> 11
    | 'K' -> 12
    | 'A' -> 13
    | _ -> 100

type Equality =
    | LGreater = 0
    | RGreater = 1
    | Equal = 2

let compareChars (one: char) two =
    let oneScore = mapCharToScore one
    let twoScore = mapCharToScore two
    if oneScore < twoScore
    then Equality.RGreater
    elif oneScore > twoScore
    then Equality.LGreater
    else Equality.Equal

let compareStrings (one: string) (two: string) =
    let bestIndex =
        one
        |> Seq.indexed
        |> Seq.tryFindIndex (fun (index, item) -> compareChars item two[index] <> Equality.Equal)
    match bestIndex with
        | Some x -> 
            let score = compareChars one[x] two[x]
            if score = Equality.LGreater then 1 else -1
        | None -> 0
    //let score = compareChars one[bestIndex] two[bestIndex]
    //if score = Equality.LGreater then 1 else -1
    

let runDay (input: string) =
    input
    |> splitByLine
    |> Array.map (fun x -> x |> split " ")
    |> Array.map (fun x -> { Value = Array.head x; Bid = int (Array.last x); MatchType = if partTwo then getJokerTypeForHand (Array.head x) else getTypeForHand (Array.head x) })
    |> Array.groupBy (fun x -> x.MatchType)
    |> Array.sort
    |> Array.map (fun x -> 
        //printf "HELLO %A\n" (snd x |> Array.filter (fun x -> x.Value = "T2T22"))
        //printf "Sorting %A\n" (fst x)
        snd x |> Array.sortWith (fun one two -> 
        compareStrings one.Value two.Value
    ))
    |> Array.collect (fun x -> x)
    |> Array.indexed
    |> Array.map (fun (index, item) -> (index + 1) * item.Bid)
    |> Array.sum
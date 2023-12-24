module Day24

open Utils

let partTwo = false

type LineInfo = {
    Coord: Coord3DBig
    Vector: Coord3DBig
}

let getCoord (input: string): Coord3DBig =
    let bits = 
        input.Split ", "
        |> Array.map (fun bit -> int64 bit)
    { xx = bits[0]; yy = bits[1]; zz = bits[2] }


let getLine (input: string): LineInfo =
    let two = input.Split " @ "
    {
        Coord = getCoord two[0]
        Vector = getCoord two[1]
    }

// https://www.hackingwithswift.com/example-code/core-graphics/how-to-calculate-the-point-where-two-lines-intersect
let linesCross(start1: CoordBig, end1: CoordBig) (start2: CoordBig, end2: CoordBig): (decimal * decimal) option =
    // calculate the differences between the start and end X/Y positions for each of our points
    let delta1x: int64 = end1.xx - start1.xx
    let delta1y: int64 = end1.yy - start1.yy
    let delta2x: int64 = end2.xx - start2.xx
    let delta2y: int64 = end2.yy - start2.yy

    // create a 2D matrix from our vectors and calculate the determinant
    let determinant = decimal (delta1x * delta2y - delta2x * delta1y)
    //printfn "Det %A" determinant 

    if abs(determinant) = 0M then
        // if the determinant is effectively zero then the lines are parallel/colinear
        None
    else
        // if the coefficients both lie between 0 and 1 then we have an intersection
        let ab = decimal ((start1.yy - start2.yy) * delta2x - (start1.xx - start2.xx) * delta2y) / determinant
        //printfn "AB %A" ab
        //if ab > 0 && ab < 1  then
        let cd = decimal ((start1.yy - start2.yy) * delta1x - (start1.xx - start2.xx) * delta1y) / determinant

        //if cd > 0 && cd < 1 then
            // lines cross – figure out exactly where and return it
        let intersectX = (decimal start1.xx) + ab * (decimal delta1x)
        let intersectY = decimal (start1.yy) + ab * (decimal delta1y)
        Some ((intersectX, intersectY))
            //else None
        //else None

let add (a: CoordBig) (b: CoordBig): CoordBig =
    { xx = a.xx + b.xx; yy = a.yy + b.yy }

let get2D (input: Coord3DBig): CoordBig =
    { xx = input.xx; yy = input.yy }

let get2DXZ (input: Coord3DBig): CoordBig =
    { xx = input.xx; yy = input.zz }

let getMovedCoord (input: CoordBig) (vector: CoordBig): CoordBig =
    add input vector

let isInFuture (one: CoordBig) (nextOne: CoordBig) (two: CoordBig) (nextTwo:CoordBig) (intersection: (decimal * decimal)): bool =
    let intX = fst intersection
    let intY = snd intersection
    ((intX - decimal one.xx) > 0M) = ((nextOne.xx - one.xx) > 0)
    && ((intY - decimal one.yy) > 0M) = ((nextOne.yy - one.yy) > 0)
    && ((intX - decimal two.xx) > 0M) = ((nextTwo.xx - two.xx) > 0)
    && ((intY - decimal two.yy) > 0M) = ((nextTwo.yy - two.yy) > 0)

let pairsAndTransform transform (lines: LineInfo array) =
    lines
    |> Array.mapi (fun index line1 ->
        let movedLine1 = transform line1
        lines
            |> Array.skip (index + 1)
            |> Array.map (fun line2 ->
                let movedLine2 = transform line2
                (movedLine1, movedLine2)
            )
    )
    |> Array.collect (fun x -> x)

let pairs (lines: LineInfo array) =
    pairsAndTransform (fun x -> x) lines

let closeEqual (a: decimal) (b: decimal) =
    System.Math.Round(a, 0) = System.Math.Round(b, 0)

let commonIntersection (lines: LineInfo array) (diff1: int) (diff2: int) (doZ: bool): (decimal * decimal) option =
    let twoD = if doZ then get2DXZ else get2D
    let mutable possible = true
    let d =
        lines
        |> Array.map (fun item -> 
            let newVec =
                if doZ then
                    { item.Vector with 
                        xx = item.Vector.xx + int64 diff1
                        zz = item.Vector.zz + int64 diff2 }
                else 
                    { item.Vector with 
                        xx = item.Vector.xx + int64 diff1
                        yy = item.Vector.yy + int64 diff2 }
            { item with Vector = newVec })
        |> pairsAndTransform (fun l -> (l, getMovedCoord (twoD l.Coord) (twoD l.Vector)))
        |> Array.fold (fun (count, (acc: (decimal * decimal) option)) ((line1, movedLine1), (line2, movedLine2)) ->
            if possible = true then
                let res = 
                    linesCross 
                        (twoD line1.Coord, movedLine1)
                        (twoD line2.Coord, movedLine2)
                if res.IsSome && (acc.IsNone || (acc.IsSome && (closeEqual (fst acc.Value) (fst res.Value)) && (closeEqual (snd acc.Value) (snd res.Value)))) then
                    //printfn "IN"
                    (count + 1, res)
                else 
                    if count > 1 then printfn "Fail at %A" count
                    //printfn "OUT %A %A" acc res
                    possible <- false
                    (count, None)
            else (count, None)
        ) (0, None)
    snd d

// https://www.reddit.com/r/adventofcode/comments/18pptor/comment/keqwg08/
// https://pastebin.com/gzRpVNU1
let doPartTwo (lines: LineInfo array) =
    let fewerLines = 
        lines
        //|> Array.skip 10
        |> Array.take 3

    let mutable sorted = false
    let mutable found = (0M,0M,0M)
    let mutable x = 0;
    while sorted = false do
        if x % 100 = 0 then printfn "x %A" x
        for y = 0 to 400 do
            //printfn "y %A" y
            if sorted = false then
                for (dx, dy) in [|(-1,1);(1,1);(-1,-1);(1,-1)|] do 
                    if sorted = false then
                        let xy = commonIntersection fewerLines (dx * x) (dy * y) false
                        if xy.IsSome then
                            //printfn "Z work"
                            let mutable z = 0
                            while sorted = false do
                                //printfn "z %A" z
                                for dz in [|-1;1|] do
                                    let xz = commonIntersection fewerLines (dx * x) (dz * z) true
                                    if xz.IsSome then
                                        //printfn "ISSOME"
                                        sorted <- true
                                        found <- (fst xy.Value,snd xy.Value,snd xz.Value)
                                z <- z + 1
                            
                            
        x <- x + 1
    printfn "%A " found
    let (one, two, three) = found
    one + two + three
    

let doPartOne (lines: LineInfo array) =
    let area = (7, 27)
    let area = (200000000000000L, 400000000000000L)

    lines
    |> pairsAndTransform (fun l -> (l, getMovedCoord (get2D l.Coord) (get2D l.Vector)))
    |> Array.map (fun ((line1, movedLine1), (line2, movedLine2)) ->
        let res = 
            linesCross 
                (get2D line1.Coord, movedLine1)
                (get2D line2.Coord, movedLine2)
        if res.IsSome && isInFuture (get2D line1.Coord) movedLine1 (get2D line2.Coord) movedLine2 res.Value then res else None
    )
    |> Array.filter (fun x -> x.IsSome)
    |> Array.map (fun x -> x.Value)
    |> Array.map (fun (x, y) ->
        let (lowerI, higherI) = area
        let lower = decimal lowerI
        let higher = decimal higherI
        //printfn "%A %A" x y
        if x >= lower && x <= higher && y >= lower && y <= higher then true else false
    )
    |> Array.filter (fun x -> x)
    |> Array.length

let runDay (input: string) =
    let lines =
        input
        |> splitByLine
        |> Array.map getLine

    if partTwo then
        doPartTwo lines
    else 
        decimal (doPartOne lines)
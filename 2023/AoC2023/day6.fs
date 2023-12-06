module Day6

let partTwo = false

let distanceForHoldTime (hold: int64) total =
    (total - hold) * hold

let getBestTime (toBeat: int64) time =
    let mutable count = 0
    for i = 0 to time do
        let distance = distanceForHoldTime i time
        count <- if distance > toBeat then count + 1 else count
    count

let runDay (input: string) =
    if not partTwo then
        let a = getBestTime 241 38
        let b = getBestTime 1549 94
        let c = getBestTime 1074 79
        let d = getBestTime 1091 70
        a * b * c * d
    else 
        getBestTime 241154910741091L 38947970

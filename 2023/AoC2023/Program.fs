open FileSystem

let partTwo = true

let printDay day test dayFunc =
    day |> getInput test |> dayFunc |> printfn "%A"

printDay 11 false Day11.runDay
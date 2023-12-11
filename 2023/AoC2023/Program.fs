open FileSystem

let partTwo = true

let printDay day test dayFunc =
    day |> getInput test |> dayFunc |> printfn "%A"

printDay 10 false Day10.runDay
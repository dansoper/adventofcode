open FileSystem

let partTwo = true

let printDay day test dayFunc =
    day |> getInput test |> dayFunc |> printfn "%A"

printDay 16 false Day16.runDay
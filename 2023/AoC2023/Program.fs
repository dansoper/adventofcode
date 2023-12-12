open FileSystem

let partTwo = true

let printDay day test dayFunc =
    day |> getInput test |> dayFunc |> printfn "%A"

printDay 13 true Day13.runDay
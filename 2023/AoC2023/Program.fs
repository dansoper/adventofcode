open FileSystem

let partTwo = true

let printDay day test dayFunc =
    day |> getInput test |> dayFunc |> printfn "%A"

printDay 14 true Day14.runDay
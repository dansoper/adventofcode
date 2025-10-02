open FileSystem

let printDay day test dayFunc =
    day |> getInput test |> dayFunc |> printfn "%A"

printDay 25 false Day25.runDay
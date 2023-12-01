open FileSystem

let printDay day test dayFunc =
    day |> getInput test |> dayFunc |> printfn "%A"

printDay 1 false Day1.runDay
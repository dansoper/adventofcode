open FileSystem

let printDay day test dayFunc =
    day |> getInput test |> dayFunc |> printfn "%A"

printDay 17 true Day17.runDay
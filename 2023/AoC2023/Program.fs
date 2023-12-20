open FileSystem

let printDay day test dayFunc =
    day |> getInput test |> dayFunc |> printfn "%A"

printDay 19 true Day19.runDay
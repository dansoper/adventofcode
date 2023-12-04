open FileSystem

let printDay day test dayFunc =
    day |> getInput test |> dayFunc |> printfn "%A"

printDay 5 true Day5.runDay
module FileSystem 

open System.IO

let getInput test day =
    let testString =
        match test with
        | true -> "test"
        | false -> ""
    let filename day = Path.Combine(__SOURCE_DIRECTORY__, $"inputs/day{day}{testString}.txt")
    File.ReadAllText(filename day)
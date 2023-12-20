module Day20

open Utils

let partTwo = true

type PulseType =
    | Low = -1
    | High = 1
    | None = 0

type ModuleType =
    | FlipFlop = 0
    | Conjunction = 1
    | Broadcast = 2
    | None = 3

type ModuleInfo = {
    Type: ModuleType
    Outputs: string array
    Inputs: Map<string, PulseType>
    PulseToSend: PulseType
    OnOff: bool
}

let flippedType t = 
    if t = PulseType.High 
        then PulseType.Low 
    elif t = PulseType.Low 
        then PulseType.High 
    else PulseType.None

let actOnPulse (pulse: PulseType) (from: string) (moduleInfo: ModuleInfo): ModuleInfo =
    match moduleInfo.Type with
    | ModuleType.Broadcast -> { moduleInfo with PulseToSend = pulse }
    | ModuleType.FlipFlop -> 
        if pulse = PulseType.Low
            then { moduleInfo 
                    with 
                    PulseToSend = if moduleInfo.OnOff then PulseType.Low else PulseType.High
                    OnOff = not moduleInfo.OnOff
            } 
            else { moduleInfo with PulseToSend = PulseType.None }
    | ModuleType.Conjunction ->
        let newMemory = moduleInfo.Inputs |> Map.add from pulse
        // If all are high, emit low; else emit high
        let allItemsHigh = newMemory |> Map.fold (fun acc key cur -> if acc && cur = PulseType.High then true else false) true 
        { moduleInfo
            with 
            PulseToSend = if allItemsHigh then PulseType.Low else PulseType.High
            Inputs = newMemory
        } 
    | ModuleType.None -> { moduleInfo with PulseToSend = PulseType.None }
    | _ -> failwith "wowee"

let runDay (input: string) =
    let mutable modules: Map<string, ModuleInfo> = Map.empty

    let firstModules =
        input
        |> splitByLine
        |> Array.map (fun line ->
            let halves = line.Split " -> "
            let secondHalf = halves |> Array.last
            let outputs = secondHalf.Split ", "
            let firstHalf = halves |> Array.head
            let moduleName = if firstHalf.StartsWith("%") || firstHalf.StartsWith("&") then firstHalf.Substring(1) else firstHalf
            let moduleType = if firstHalf.StartsWith("%") then ModuleType.FlipFlop elif firstHalf.StartsWith("&") then ModuleType.Conjunction else ModuleType.Broadcast
            (moduleName, {
                Type = moduleType
                Inputs = Map.empty
                Outputs = outputs
                PulseToSend = PulseType.Low
                OnOff = false
            })
        )
        |> Array.append [|("rx", {
            // This is for part 1
            Type = ModuleType.None
            Outputs = Array.empty
            Inputs = Map.empty
            PulseToSend = PulseType.None
            OnOff = false
        })|]

    let secondModules =
        firstModules
        |> Array.map (fun (mainName, modInfo) ->
            let inputs = 
                firstModules
                |> Array.filter (fun (name, info) -> info.Outputs |> Array.exists (fun n -> n = mainName))
                |> Array.map (fun (n, i) -> (n, PulseType.Low))
                |> Map.ofArray
            (mainName, { modInfo with Inputs = inputs })
        )
    
    modules <- secondModules |> Map.ofArray

    let mutable actions: ((PulseType * string * string) array) = Array.empty
    
    let mutable highCount = 0
    let mutable lowCount = 0

    // exploration for part two
    let leadsToRx = modules["rx"]
    printfn "%A" leadsToRx // I discovered lg leads to rx
    let leadsToLg = modules["lg"]
    printfn "%A" leadsToLg

    let mutable interestingModules = leadsToLg.Inputs |> Map.keys |> Array.ofSeq |> Array.map (fun x -> (x, 0)) |> Map.ofArray

    for i = 1 to 10000 do
        actions <- Array.append actions [|(PulseType.Low, "broadcaster", "button")|]
        while actions.Length > 0 do
            let (pulse, newModule, fromModule) = actions |> Array.head
            if pulse = PulseType.High then highCount <- highCount + 1
            if pulse = PulseType.Low then lowCount <- lowCount + 1
            
            //printfn "L %A %A" actions.Length newModule
            let foundModule = modules |> Map.tryFind newModule
            if foundModule <> None then
                let updatedModule = actOnPulse pulse fromModule foundModule.Value
                
                //interestingModules |> Array.exists (fun x -> x = newModule) ||
                if newModule = "lg" && pulse = PulseType.High
                    then 
                        printfn "From %A to Module %A Pulse %A i %A" fromModule newModule pulse i
                        if interestingModules[fromModule] = 0 then interestingModules <- interestingModules |> Map.add fromModule i
                
                //printfn "%A %A %A" pulse newModule updatedModule
                modules <- modules |> Map.add newModule updatedModule
                if updatedModule.PulseToSend <> PulseType.None then
                    actions <- 
                    updatedModule.Outputs
                    |> Array.map (fun output -> (updatedModule.PulseToSend, output, newModule))
                    |> Array.append (actions |> Array.skip 1)
                else actions <- actions |> Array.skip 1
            else 
                actions <- actions |> Array.skip 1

    if partTwo then
        interestingModules
        |> Map.toArray
        |> Array.map (fun (x, y) -> y)
        |> Array.fold (fun acc cur -> lcm acc cur) 1
    else
        (int64 lowCount * int64 highCount)
    

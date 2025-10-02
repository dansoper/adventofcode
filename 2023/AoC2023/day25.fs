module Day25

open System
open System.Collections.Generic
open Utils

// Component type with mutable connections
type Component(name: string) =
    let connects = HashSet<Component>()
    member _.Name = name
    member _.Connects = connects
    member c.ConnectsTo(other: Component) =
        connects.Add(other) |> ignore
        other.Connects.Add(c) |> ignore
    member c.Disconnect(other: Component) =
        connects.Remove(other) |> ignore
        other.Connects.Remove(c) |> ignore

// Parse input lines into components
let parseComponents (lines: string[]) =
    let dict = Dictionary<string, Component>()
    let getOrCreate name =
        if not (dict.ContainsKey(name)) then dict.[name] <- Component(name)
        dict.[name]
    for line in lines do
        let parts = line.Split(": ")
        let comp = getOrCreate parts.[0]
        for c in parts.[1].Split(' ') do
            comp.ConnectsTo(getOrCreate c)
    dict

// Find the three edges to remove
let findThreePairs (dict: Dictionary<string, Component>) =
    let pairs = Dictionary<(Component*Component), int>()
    
    // BFS to find "furthest paths" from a component
    let furthestPaths (start: Component) =
        let paths = ResizeArray<ResizeArray<Component>>()
        let seenPaths = HashSet<string>()
        let visited = HashSet<Component>()
        let queue = Queue<Component * ResizeArray<Component>>()
        queue.Enqueue(start, ResizeArray([start]))
        while queue.Count > 0 do
            let (comp, path) = queue.Dequeue()
            visited.Add(comp) |> ignore
            for conn in comp.Connects do
                if visited.Add(conn) then
                    let newPath = ResizeArray(path)
                    newPath.Add(conn)
                    queue.Enqueue(conn, newPath)
                else
                    let pathKey = path |> Seq.map (fun x -> x.Name) |> Seq.rev |> String.concat "-"
                    if seenPaths.Add(pathKey) then
                        paths.Add(path)
        // take the longest paths
        paths
        |> Seq.groupBy (fun p -> p.Count)
        |> Seq.sortByDescending fst
        |> Seq.head
        |> snd

    for comp in dict.Values do
        for path in furthestPaths comp do
            for i in 0 .. path.Count - 2 do
                for j in i+1 .. path.Count - 1 do
                    let a, b = path.[i], path.[j]
                    let key = if String.Compare(a.Name, b.Name) > 0 then (a, b) else (b, a)
                    if pairs.ContainsKey(key) then pairs.[key] <- pairs.[key] + 1
                    else pairs.[key] <- 1

    pairs
    |> Seq.sortByDescending (fun kv -> kv.Value)
    |> Seq.take 3
    |> Seq.map (fun kv -> kv.Key)
    |> Seq.toList

// Assign components to two groups after removing edges
let countGroups (dict: Dictionary<string, Component>) =
    let group1 = ResizeArray<Component>()
    let group2 = ResizeArray<Component>()
    let pairs = findThreePairs dict

    for (c1, c2) in pairs do
        if group1.Count = 0 then
            group1.Add(c1)
            group2.Add(c2)
        c1.Disconnect(c2)

    let remaining = HashSet(dict.Values)
    remaining.ExceptWith(group1)
    remaining.ExceptWith(group2)

    let rec assignGroups () =
        let mutable changed = false
        for comp in remaining |> Seq.toList do
            if comp.Connects |> Seq.exists group1.Contains then
                group1.Add(comp)
                remaining.Remove(comp) |> ignore
                changed <- true
            elif comp.Connects |> Seq.exists group2.Contains then
                group2.Add(comp)
                remaining.Remove(comp) |> ignore
                changed <- true
        if changed then assignGroups()
    assignGroups()

    group1.Count * group2.Count, group1, group2



let runDay (input: string) =
    let lines = input |> split "\n"
    let comps = parseComponents lines
    let value, g1, g2 = countGroups comps
    printfn "Remove edges: %A" (findThreePairs comps)
    printfn "Group sizes: %d and %d (product %d)" g1.Count g2.Count value



//let runDay (input: string) =
  //  input
   // |> graph
   // |> undirected
    
  //  |> stuff
  //  |> Map.iter (fun node links ->
   //     printfn "%s -> %A" node (Set.toList links))


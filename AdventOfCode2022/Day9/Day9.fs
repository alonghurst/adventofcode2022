namespace Day9

open System
open System.IO
open Xunit

module Solver = 

    let PuzzleFilename = "Day9/input.txt"
    let TestFilename = "Day9/test.txt"
    let Test2Filename = "Day9/test2.txt"

    let NonBlank s = not (String.IsNullOrWhiteSpace s)

    let ReadData f = 
        File.ReadLines f
        |> Seq.filter NonBlank
  
    let inline charToInt c = int c - int '0'

    type Coord = int * int

    let Up = 0, 1
    let Down = 0, -1
    let Left = -1, 0
    let Right = 1, 0
    
    let ParseMove (line: string) = 
        let split = line.Split([| ' ' |], StringSplitOptions.RemoveEmptyEntries)
        let num = Convert.ToInt32 split[1]
        let dir = 
            match split[0].ToLower() with
            | "u" -> Up
            | "d" -> Down
            | "l" -> Left
            | "r" -> Right
            | _ -> failwith $"Unable to parse {split[0]} to direction"
        (dir, num)

    let DrawCell c head (tails: ((int * int) * (int * int) list)[]) =
        if c = head then "H"
        else
            let t = 
                tails
                |> Array.mapi (fun i (p, _) -> if p = c then Some i else None)
                |> Array.tryFind (fun s -> s.IsSome)
            if t.IsSome && t.Value.IsSome then t.Value.Value.ToString()
            else "."

    let DrawRow size y head tails = 
        seq {
            for x in 0 .. size do
                yield DrawCell (x, y) head tails
        }
        |> Seq.toArray

    let DrawRows size head tails =
        seq {
            for y in 0 .. size do
                yield DrawRow size y head tails
        }
        |> Seq.toArray

    let Draw head (tails: ((int * int) * (int * int) list)[]) =
        printfn "----------------------------------------"
        printfn "Head is %s" (head.ToString())
        for x in 0 .. tails.Length - 1 do
            let (t, v) = tails[x]
            printfn "Tail %x is %s (%i)" x (t.ToString()) (v |> List.distinct |> List.length)
        let grid = DrawRows 20 head tails
        for y in 0 .. grid.Length - 1 do
            for x in 0 .. grid[y].Length - 1 do
                let c = grid[y][x]
                printf "%s" c
            printfn ""

    let Move (x, y) (dX, dY) = (x + dX, y + dY)

    let IsCloseEnough (hX: int, hY: int) (tX, tY) =
        let aX = Math.Abs (hX - tX)
        let aY = Math.Abs (hY - tY)
        not (aX > 1 || aY > 1)
    let DirTo a b =
        if a > b then 1
        else if a < b then -1 
        else 0

    let Catchup (hX, hY) (tX, tY) =
        if IsCloseEnough (hX, hY) (tX, tY) then (tX, tY)
        else 
            let dX = DirTo hX tX
            let dY = DirTo hY tY
            Move (tX, tY) (dX, dY)

    let rec DoTail head (tails: ((int * int) * (int * int) list) []) i = 
        if i = tails.Length then tails
        else
            let (tail, visited) = tails[i]
            let nTail = Catchup head tail
            let nVisited = nTail :: visited
            let nTails = tails |> Array.mapi (fun elI el -> if elI = i  then (nTail, nVisited) else el)
            DoTail nTail nTails (i + 1)

    let rec DoMove head tails (dir, num) = 
        //Draw head tails
        if num = 0 then (head, tails)
        else
            let nHead = Move head dir
            let nTails = DoTail head tails 0
            DoMove nHead nTails (dir, num - 1) 

    let MakeTails c len: ((int * int) * (int * int) list) [] =
        seq {
            for i in 1 .. len do 
                yield (c, [ c ])
        }
        |> Seq.toArray

    let CountTails lines len = 
        let moves = lines |> Seq.map ParseMove
        let start = (0, 0)
        let tails = MakeTails start len
        let (head, tails) = 
            moves 
            |> Seq.fold (fun (head, tails) move -> DoMove head tails move) (start, tails)

        let (_, visited) = tails[tails.Length - 1]
        let visitedD =
            visited
            |> List.distinctBy (fun (aX, aY) -> $"{aX},{aY}")
        let visitedCount = 
            visitedD
            |> List.length

        // Off by 1...
        printfn "Visited %i" (visitedCount + 1)

    let Solve1 lines = 
        CountTails lines 1

    let Solve2 lines = 
        CountTails lines 9

    let Solve () =
        let data = ReadData PuzzleFilename
        Solve1 data |> ignore
        Solve2 data |> ignore

    
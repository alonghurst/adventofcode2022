namespace Day14

open System
open System.IO
open Xunit

module Solver = 

    let PuzzleFilename = "Day14/input.txt"
    let TestFilename = "Day14/test.txt"

    let NonBlank s = not (String.IsNullOrWhiteSpace s)

    let ReadData f = 
        File.ReadLines f
        |> Seq.filter NonBlank
        |> Seq.toArray
  
    let RemoveNonNumeric (s: string) =
        let c =
            s
            |> Seq.filter Char.IsDigit
            |> Seq.toArray
        String(c)

    let ParsePart (part: string) =
        let s = part.Split([| ","|], StringSplitOptions.RemoveEmptyEntries)
        let x = Convert.ToInt32 s[0]
        let y = Convert.ToInt32 s[1]
        (x, y)

    let ParseLine (line: string) = 
        let parts = line.Split([| "->"|], StringSplitOptions.RemoveEmptyEntries)
        parts
        |> Array.map ParsePart

    let ParseLines (lines: string array) =
        lines
        |> Array.map ParseLine

    let FindExtents rocks =
        let flat =
            rocks
            |> Array.reduce Array.append
        let (width, _) = 
            flat
            |> Array.maxBy (fun (x, _) -> x)
        let (_, height) = 
            flat
            |> Array.maxBy (fun (_, y) -> y)
        (width + 1, height + 1)

    type Cell = Air | Rock | Sand

    let rec PaintGrid (grid: Cell array2d) (x, y) (tX, tY) (dX, dY) =
        if x = tX && y = tY then 
            grid[x, y] <- Rock
            ()
        else
            grid[x, y] <- Rock
            PaintGrid grid (x + dX, y + dY) (tX, tY) (dX, dY)

    let FindDirection a b =
        if a > b then -1
        else if b > a then 1
        else 0

    let FindDirections (xA, yA) (xB, yB) =
        let xD = FindDirection xA xB
        let yD = FindDirection yA yB
        (xD, yD)

    let AddRockToGrid (grid: Cell array2d) a b =
        let d = FindDirections a b
        PaintGrid grid a b d

    let MakeGrid (rocks: (int * int) array  array) addX addY =
        let (eX, eY) = FindExtents rocks
        let (width, height) = (eX + addX, eY + addY)
        let grid = Array2D.init width height (fun _ _ -> Air)
        for rock in rocks do
            for i in 0 .. rock.Length - 2 do
                AddRockToGrid grid rock[i] rock[i+1]
        if addY <> 0 then AddRockToGrid grid (0, height - 1) (width - 1, height - 1)
        grid

    let DrawCell c =
        let ch = 
            match c with
            | Rock -> "#"
            | Air -> "."
            | Sand -> "O"
        printf "%s" ch

    let DrawGrid (grid: Cell array2d) startX b =
        if not b then ()
        else
            let width = grid.GetLength(0)
            let height = grid.GetLength(1)
            for y in 0 .. height - 1 do
                for x in startX .. width - 1 do
                    DrawCell grid[x, y]
                printfn ""

    let IsOutOfBounds (grid: Cell array2d) (x, y) =
        let width = grid.GetLength(0)
        let height = grid.GetLength(1)
        if x < 0 || y < 0 then true
        else if x = width || y = height then true
        else false

    let IsBlockedBelow (grid: Cell array2d) (x, y) =
        let (cX, cY) = (x, y + 1)
        if IsOutOfBounds grid (cX, cY) then false
        else
            match grid[cX, cY] with
            | Air -> false
            | Sand -> true
            | Rock -> true

    let IsAtRest (grid: Cell array2d) (x, y) =
        let a = IsBlockedBelow grid (x, y)
        let b = IsBlockedBelow grid (x - 1, y)
        let c = IsBlockedBelow grid (x + 1, y)
        (a, b, c)

    let rec DropSand (grid: Cell array2d) (x, y) =
        if IsOutOfBounds grid (x, y) then None
        else 
            let (a, b, c) = IsAtRest grid (x, y)
            if a && b && c then Some (x, y)
            else if not a then DropSand grid (x, y + 1)
            else if not b then DropSand grid (x - 1, y + 1)
            else if not c then DropSand grid (x + 1, y + 1)
            else failwith "Shouldn't get here"
    
    let rec TryAddSand grid p i drawGrid stopAtStart =
        let s = DropSand grid p
        if s.IsSome then
            let (x, y) = s.Value
            grid[x, y] <- Sand
            //printfn "============= Sands %i ==============" (i + 1)
            DrawGrid grid 450 drawGrid
            if stopAtStart && (x, y) = p then 
                printfn "Start is blocked"
                i + 1
            else TryAddSand grid p (i + 1) drawGrid stopAtStart
        else i


    let Solve1 lines b = 
        let rocks = ParseLines lines
        let grid = MakeGrid rocks 0 0
        let count = TryAddSand grid (500, 0) 0 b false
        printfn "Added %i sands" count

    let Solve2 lines b e = 
        let rocks = ParseLines lines
        let grid = MakeGrid rocks e 2
        let count = TryAddSand grid (500, 0) 0 b true
        printfn "Added %i sands" count

    let Solve () =
        let d1 = ReadData TestFilename
        let d2 = ReadData PuzzleFilename
        Solve1 d1 false|> ignore
        Solve1 d2 false |> ignore
        Solve2 d1 false 25 |> ignore
        Solve2 d2 false 1000 |> ignore
        //Solve2 d1 |> ignore
        //Solve2 d2|> ignore

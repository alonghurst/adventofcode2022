namespace Day5

open System
open System.IO
open Xunit

module Solver = 

    let Filename = "Day5/input.txt"

    let NonBlank s = not (String.IsNullOrWhiteSpace s)

    let ReadData f = 
        File.ReadLines f
        |> Seq.filter NonBlank
    
    let StringToSeqCharHack s =
        s 
        |> Seq.map (fun x -> x |> Seq.map(fun y -> y))
    
    let Top col = 
        if Seq.length col = 0 then None
        else Some (Seq.item 0 col)

    let Tops crates = 
        crates
        |> Seq.map Top

    let PrintTops (crates: char[][]) =
        let tops = Tops crates
        for t in tops do
            if t.IsSome then printf "%c" t.Value
            else printf "#"
        printf "%s" Environment.NewLine

    let Crate s = 
        if Seq.length s = 0 then None
        else
            match Seq.item 0 s with
            | '[' -> Some (Seq.item 1 s)
            | _ -> None

    let rec CrateRow (s: string) i = 
        seq {
            if i >= (s.Length - 3) then ()
            else
                yield Crate (s |> Seq.skip i |> Seq.take 4)
                yield! CrateRow s (i + 4)
        }
    
    let rec CrateColumnSeq (lines: string[]) pos row =
        seq {
            if row < 0 then ()
            else
                let cur = lines[row]
                if cur.Trim().Length = 0 then ()
                else
                    yield Crate cur[pos..]
                    yield! CrateColumnSeq lines pos (row - 1)
        } 

    let CrateColumn lines pos row = 
        CrateColumnSeq lines pos row
        |> Seq.filter (fun x -> x.IsSome)
        |> Seq.map (fun x -> x.Value)
        |> Seq.rev
        |> Seq.toArray

    let rec FindDefinitionLine (lines: string[]) i b = 
        match lines[i][0] with
        | ' ' when b -> (lines[i], i)
        | '[' -> FindDefinitionLine lines (i + 1) true
        | _ -> FindDefinitionLine lines (i + 1) false

    let FindColumns (line: string) = 
        let s = line.Split([| ' ' |],  StringSplitOptions.RemoveEmptyEntries)
        s
        |> Seq.map Convert.ToInt32
        |> Seq.max

    let BuildCrates lines = 
        let (d, s) = FindDefinitionLine lines 0 false
        let c = FindColumns d
        seq {
            for i = 0 to (c - 1) do
                yield CrateColumn lines (i * 4) (s - 1)
        }
        |> Seq.toArray

    let Parse (s: string) = 
        let (b, i) = Int32.TryParse s
        if b then Some i else None 

    let ParseMoves (line: string) =
        let s = line.Split([| ' ' |], StringSplitOptions.RemoveEmptyEntries)
        let p =
            s 
            |> Seq.map Parse
            |> Seq.filter (fun x -> x.IsSome)
            |> Seq.map (fun x -> x.Value)
            |> Seq.toArray
        (p[0], p[1], p[2])

    let ExecuteMove (cols: char[][]) fIn tIn toTake =
        // fIn and tIn are column numbers, not indexes
        let f = fIn - 1
        let t = tIn - 1
        let fromCol = Array.item f cols
        let items = Array.take toTake fromCol
        seq {
            for (i, x) in Seq.indexed cols do
                let r = if i = f then x |> Array.skip toTake
                        else if i = t then x |> Array.append items
                        else x
                yield r
        }
        |> Seq.toArray

    let rec ExecuteMoves cols f t i mode =
        if i = 0 then cols
        else 
            if mode then ExecuteMove cols f t i
            else
                let moved = ExecuteMove cols f t 1
                ExecuteMoves moved f t (i - 1) mode
    
    let rec ExecuteAllMoves (cols: char[][]) moves i mode =
        PrintTops cols
        if i = Seq.length moves then cols
        else
            let (x, f, t) = Seq.item i moves
            printfn "move %i: %i %i %i" i x f t
            let moved = ExecuteMoves cols f t x mode
            ExecuteAllMoves moved moves (i + 1) mode

    let ReadMoves lines =
        let (_, line) = FindDefinitionLine lines 0 false
        lines 
        |> Seq.skip (line + 1)
        |> Seq.map ParseMoves

    let Solve1 = 
        let data = ReadData Filename |> Seq.toArray
        let crates = BuildCrates data
        PrintTops crates
        let moves = ReadMoves data
        let moved = ExecuteAllMoves crates moves 0 false
        printfn "----------------------------"
        PrintTops moved 

    let Solve2 = 
        let data = ReadData Filename |> Seq.toArray
        let crates = BuildCrates data
        PrintTops crates
        let moves = ReadMoves data
        let moved = ExecuteAllMoves crates moves 0 true
        printfn "----------------------------"
        PrintTops moved 


    let Solve =
        Solve1 |> ignore
        Solve2 |> ignore

    let TestData = 
        [|
             "    [D]";    
            "[N] [C]    ";
            "[Z] [M] [P]";
             " 1   2   3 ";
            "move 1 from 2 to 1";
            "move 3 from 1 to 3";
            "move 2 from 2 to 1";
            "move 1 from 1 to 2";
        |]
    
    let ValidateCrates c (e: string) =
        Assert.Equal(Seq.length c, e.Length)
        for i = 0 to e.Length - 1 do
            let ci = Seq.item i c
            Assert.Equal(e[i], ci)

    [<Fact>]
    let ExecuteMoves_works () =
        let crates = BuildCrates TestData
        let moved = ExecuteMoves crates 1 3 2 false |> Seq.toArray
        ValidateCrates moved[0] ""
        ValidateCrates moved[2] "ZNP"

    [<Fact>]
    let ExecuteMove_works () =
        let crates = BuildCrates TestData
        let moved = ExecuteMove crates 1 3 1 |> Seq.toArray
        ValidateCrates moved[0] "Z"
        ValidateCrates moved[2] "NP"
    
    [<Fact>]
    let ReadMoves_works () =
        let moves = ReadMoves TestData
        Assert.Equal(Seq.length moves, 4)

    [<Fact>]
    let ParseMoves_works () =
        let Validate ((m0, m1, m2): int * int * int) x y z =
            Assert.Equal(m0, x)
            Assert.Equal(m1, y)
            Assert.Equal(m2, z)
        let r = TestData[4..] |> Seq.map ParseMoves |> Seq.toArray
        Validate r[0] 1 2 1
        Validate r[1] 3 1 3
        Validate r[2] 2 2 1
        Validate r[3] 1 1 2


    [<Fact>]
    let FindDefinitionLine_works () =
        let (line, i) = FindDefinitionLine TestData 0 false
        Assert.Equal(i, 3)
        Assert.Equal(line, TestData[3])

    [<Fact>]
    let FindColumns_works () =
        let r = FindColumns TestData[3]
        Assert.Equal(r, 3)

    [<Fact>]
    let CrateColumn_works_on_single () =
        let r = CrateColumn TestData (2 * 4) 2 |> Seq.toArray
        Assert.Equal(r.Length, 1)
        Assert.Equal(r[0], 'P')

    [<Fact>]
    let BuildCrates_Works () = 
        let crates = BuildCrates TestData
        Assert.Equal(Seq.length crates, 3)
        ValidateCrates (Seq.item 0 crates) "NZ"
        ValidateCrates (Seq.item 1 crates) "DCM"
        ValidateCrates (Seq.item 2 crates) "P"

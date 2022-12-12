namespace Day10

open System
open System.IO
open Xunit

module Solver = 

    let PuzzleFilename = "Day10/input.txt"
    let TestFilename = "Day10/test.txt"

    let NonBlank s = not (String.IsNullOrWhiteSpace s)

    let ReadData f = 
        File.ReadLines f
        |> Seq.filter NonBlank
        |> Seq.toArray
  
    let inline charToInt c = int c - int '0'

    let CylclesOfInterest = 
        [|
            20;
            60;
            100;
            140;
            180;
            220;
        |]

    let SignalStrength c x = c * x
    
    let PrintCycle (c, x, s) = 
        printfn "c: %i x: %i s: %i" c x s

    let PrintCycles cycles =
        for c in cycles do
            PrintCycle c

    let ParseCommand (line: string) =
        let s = line.Split( [| " "|], StringSplitOptions.RemoveEmptyEntries)
        match s[0] with
        | "noop" -> (1, 0)
        | "addx" -> (2, Convert.ToInt32 s[1])
        | _ -> failwith $"Unable to parse {s[0]}"

    let ProcessLines (lines: string[]) =
        let mutable c = 0
        let mutable x = 1
        seq {
            yield (c, x, 1)
            for line in lines do
                let (pC, pX) = ParseCommand line
                for i in 0 .. (pC - 1) do
                    c <- c + 1
                    let s = SignalStrength c x

                    printfn "%s %i" line i
                    PrintCycle (c, x, s)

                    yield (c, x, s)
                x <- x + pX
        }
        |> Seq.toArray

    let ExamineCycles cycles = 
        //PrintCycles cycles
        let interesting =
            cycles
            |> Array.filter (fun (c, _, _) -> Array.contains c CylclesOfInterest)
        PrintCycles interesting
        let sum = 
            interesting 
            |> Array.sumBy (fun (_, _, s) -> s)
        printfn "Sum is %i" sum

    let ScreenWidth = 40

    let rec IsSprite p x =
        if p >= ScreenWidth then IsSprite (p - ScreenWidth) x
        else
            p = x || p = x - 1 || p = x + 1

    let Pixel p x = 
        if IsSprite p x then "#"
        else "."

    let RenderCyclesOld (cycles: (int * int * int)[])  =
        let mutable p = 0
        for (_, x, _) in cycles do
            if p = ScreenWidth then 
                printfn ""
                p <- 0
            let pix = Pixel p x
            printf "%s" pix
            p <- p + 1

    let ProcsesLinesForRendering lines = 
        let mutable p = 0
        let mutable x = 1
        let mutable b = ""
        for line in lines do
            let (pC, pX) = ParseCommand line
            for i in 0 .. (pC - 1) do
                let pix = Pixel p x
                b <- b + pix
                p <- p + 1
                if p = ScreenWidth then
                    printfn "%s" b
                    b <- ""
                    p <- 0
            x <- x + pX

    let Solve1 lines = 
        let cycles = ProcessLines lines
        ExamineCycles cycles

    let Solve2 lines = 
        ProcsesLinesForRendering lines

    let Solve () =
        let data = ReadData PuzzleFilename
        Solve1 data |> ignore
        Solve2 data |> ignore

    let ExpectedTestSignals =
        [|
            420;
            1140;
            1800;
            2940;
            2880;
            3960;
        |]
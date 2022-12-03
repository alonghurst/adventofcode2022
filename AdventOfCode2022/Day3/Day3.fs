namespace Day3

open System
open System.IO
open System.Globalization
open Xunit

module Solver = 

    let Filename = "Day3/input.txt"

    let NonBlank s = not (String.IsNullOrWhiteSpace s)

    let ReadData f = 
        File.ReadLines f
        |> Seq.filter NonBlank

    let Priority c = 
        match c with
        | _ when Char.IsLower c -> (int c) - 96
        | _ when Char.IsUpper c -> (int c) - 38
        | _ -> failwith $"{c} is not an ASCII letter"

    let Solve1 = 
        ()

    let Solve2 = 
        ()

    let Solve =
        Solve1 |> ignore
        Solve2 |> ignore

    [<Fact>]
    let Priority_returns_correct_value () =
        Assert.Equal(Priority 'b', 2)
        Assert.Equal(Priority 'A', 27)
        Assert.Equal(Priority 'p', 16)
        Assert.Equal(Priority 'L', 38)
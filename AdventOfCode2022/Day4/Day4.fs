namespace Day4

open System
open System.IO
open Xunit

module Solver = 

    let Filename = "Day4/input.txt"

    let NonBlank s = not (String.IsNullOrWhiteSpace s)

    let ReadData f = 
        File.ReadLines f
        |> Seq.filter NonBlank
    
    let StringToTuple (str: string) f s =
        let split = str.Split [| s |]
        (f split[0], f split[1])

    let Range s = StringToTuple s Convert.ToInt32 '-'

    let Pairs s = StringToTuple s Range ','
        
    let Contains (a, b) (c, d) =
        a <= c && b >= d

    let EitherContains (x, y) =
        Contains x y || Contains y x

    let Overlaps x (c, d) = 
        Contains x (c, c) || Contains x (d, d)

    let EitherOverlaps (x, y) =
        Overlaps x y || Overlaps y x

    let CountContaining data = 
        data
        |> Seq.map Pairs
        |> Seq.filter EitherContains
        |> Seq.length

    let CountOverlapping data = 
        data
        |> Seq.map Pairs
        |> Seq.filter EitherOverlaps
        |> Seq.length

    let Solve1 = 
        let data = ReadData Filename
        let count = CountContaining data
        printfn "There are %i containing pairs" count

    let Solve2 = 
        let data = ReadData Filename
        let count = CountOverlapping data
        printfn "There are %i overlapping pairs" count


    let Solve =
        Solve1 |> ignore
        Solve2 |> ignore

    let TestData = 
        [|
            "2-4,6-8";
            "2-3,4-5";
            "5-7,7-9";
            "2-8,3-7";
            "6-6,4-6";
            "2-6,4-8";
        |]
    
    [<Fact>]
    let Works_with_test_data () =
        let count = CountContaining TestData
        Assert.Equal(count, 2)

    [<Fact>]
    let Contains_works () =
        let r = Contains (1, 4) (2, 3)
        Assert.Equal(r, true)
namespace Day6

open System
open System.IO
open Xunit

module Solver = 

    let Filename = "Day6/input.txt"

    let ReadData f = 
        File.ReadLines f
        |> Seq.head
        |> Seq.toArray
   
    let IsSequence i x a =
        let l =
            a 
            |> Array.skip i
            |> Array.take x
            |> Array.distinct
            |> Array.length
        l = x

    let rec FindSequence i x a=
        if IsSequence i x a then (i + x)
        else if i > (a.Length - x) then failwith "No characters left to check"
        else FindSequence (i + 1) x a
            

    let Solve1 = 
        let data = ReadData Filename
        let s = FindSequence 0 4 data
        printfn "Sequence after %i" s

    let Solve2 = 
        let data = ReadData Filename
        let s = FindSequence 0 14 data
        printfn "Sequence after %i" s


    let Solve =
        Solve1 |> ignore
        Solve2 |> ignore

    let TestData = 
        [|
            (5, "bvwbjplbgvbhsrlpgdmjqwftvncz");
            (6, "nppdvjthqldpwncqszvftbrmjlhg");
            (10, "nznrnfrfntjfmvfwmzdfjlvtqnbhcprsg");
            (11, "zcfzfwzzqfrljwzlrfnpqdbhtmscgvjw");
        |]

    let TestData14 = 
        [|
            (19, "mjqjpqmgbljsphdztnvjfqwrcgsmlb");
            (23, "bvwbjplbgvbhsrlpgdmjqwftvncz");
            (23, "nppdvjthqldpwncqszvftbrmjlhg");
            (29, "nznrnfrfntjfmvfwmzdfjlvtqnbhcprsg");
            (26, "zcfzfwzzqfrljwzlrfnpqdbhtmscgvjw");
        |] 

    [<Fact>]
    let FindFirstMarker_works () =
        let Validate x s =
            let r = FindSequence 0 4 s
            Assert.Equal(r, x)
        for (x, s) in TestData do
            Validate x (s |> Seq.toArray)

    [<Fact>]
    let FindFirstMarker_works_for_14 () =
        let Validate x s =
            let r = FindSequence 0 14 s
            Assert.Equal(r, x)
        for (x, s) in TestData14 do
            Validate x (s |> Seq.toArray)
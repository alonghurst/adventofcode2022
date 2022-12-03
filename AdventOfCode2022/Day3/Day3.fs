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

    // Had to do this to treat a string[] as a seq<seq<char>>
    // It seems seq<string> is not a convertable type to be used as <seq<seq<char>>
    let StringToSeqCharHack s =
        s 
        |> Seq.map (fun x -> x |> Seq.map(fun y -> y))

    let Priority c = 
        match c with
        | _ when Char.IsLower c -> (int c) - 96
        | _ when Char.IsUpper c -> (int c) - 38
        | _ -> failwith $"{c} is not an ASCII letter"

    let Rucksack (s: string) =
        let h = s.Length / 2
        (s[.. h - 1], s[h ..])

    let Rucksacks lines =
        lines
        |> Seq.map Rucksack

    let AllContain f items =
        let c = items |> Seq.tryFindIndex (fun y -> not (Seq.contains f y))
        c.IsNone

    let ItemsInCommon items =
        let allItems =
            items
            |> Seq.reduce Seq.append
            |> Seq.distinct

        allItems
        |> Seq.filter (fun x -> AllContain x items)

    let SackItemsInCommon (a, b) = ItemsInCommon [| a; b |]

    let SackItemInCommon sack =
        SackItemsInCommon sack |> Seq.head

    let ElfTeam (elves: seq<string>) =
        let charElves = StringToSeqCharHack elves
        ItemsInCommon charElves
        |> Seq.head

    let rec ElfTriplets i lines =
        seq {
            yield 
                lines
                |> Seq.skip i
                |> Seq.take 3
            if i + 3 < Seq.length lines
            then yield! ElfTriplets (i + 3) lines
        }

    let TotalPriority s =
        s
        |> Rucksacks
        |> Seq.map SackItemInCommon
        |> Seq.map Priority
        |> Seq.sum

    let TotalTeamPriority s =
        s
        |> ElfTriplets 0
        |> Seq.map ElfTeam
        |> Seq.map Priority
        |> Seq.sum

    let Solve1 = 
        let data = ReadData Filename
        let totalPriority = TotalPriority data
        printfn "Total priority: %i" totalPriority

    let Solve2 = 
        let data = ReadData Filename
        let totalPriority = TotalTeamPriority data
        printfn "Total triplet priority: %i" totalPriority


    let Solve =
        Solve1 |> ignore
        Solve2 |> ignore

    let SampleLines =
        [|
            "vJrwpWtwJgWrhcsFMMfFFhFp";
            "jqHRNqRjqzjGDLGLrsFMfFZSrLrFZsSL";
            "PmmdzqPrVvPwwTWBwg";
            "wMqvLMZHhHMvwLHjbvcjnnSBnvTQFn";
            "ttgJtRGJQctTZtZT";
            "CrZsJsPPZsGzwwsLwLmpwMDw";
        |]

    [<Fact>]
    let TotalTeamPriority_test () =
        let result = TotalTeamPriority SampleLines
        Assert.Equal(result, 70)

    [<Fact>]
    let ElfTeam_test () =
        let tr =
            SampleLines
            |> ElfTriplets 0
        let result =
            SampleLines
            |> ElfTriplets 0
            |> Seq.map ElfTeam
            |> Seq.toArray

        Assert.Equal(result.Length, 2)
        Assert.Equal(result[0], 'r')
        Assert.Equal(result[1], 'Z')

    [<Fact>]
    let SackItemsInCommon_test () =
        let common = SackItemsInCommon ("vJrwpWtwJgWr", "hcsFMMfFFhFp") |> Seq.toArray
        Assert.Equal(common.Length, 1)
        Assert.Equal(common[0], 'p')

    [<Fact>]
    let ItemsInCommon_test () =
        let Test (s: string[]) (e: char) =
            // Had to do sseq to treat a string[] as a seq<seq<char>>
            let sseq = StringToSeqCharHack s
            let common = ItemsInCommon sseq |> Seq.toArray
            Assert.Equal(common.Length, 1)
            Assert.Equal(common[0], e)
        
        Test SampleLines[..2] 'r'
        Test SampleLines[3..] 'Z'


    [<Fact>]
    let TotalPriority_test () =
        let result = TotalPriority SampleLines
        Assert.Equal(result, 157)

    [<Fact>]
    let Priority_test () =
        Assert.Equal(Priority 'b', 2)
        Assert.Equal(Priority 'A', 27)
        Assert.Equal(Priority 'p', 16)
        Assert.Equal(Priority 'L', 38)

    [<Fact>]
    let Rucksack_test () =
        let Test s eA eB =
            let (a, b) = Rucksack s
            Assert.Equal(a.Length, b.Length)
            Assert.Equal(a, eA)
            Assert.Equal(b, eB)
            
        Test "vJrwpWtwJgWrhcsFMMfFFhFp" "vJrwpWtwJgWr" "hcsFMMfFFhFp"
        Test "jqHRNqRjqzjGDLGLrsFMfFZSrLrFZsSL" "jqHRNqRjqzjGDLGL" "rsFMfFZSrLrFZsSL"

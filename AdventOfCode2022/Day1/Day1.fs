namespace Day1

open System
open System.IO

module Solver = 

    let Filename = "Day1/input.txt"

    let ReadData = File.ReadLines Filename |> Seq.toArray

    let LineToCalories (s: string) = Convert.ToInt32 s

    let rec GetElf (str: string[]) i = 
        seq {
            if i >= str.Length then ()
            else
                let s = str[i]
                match s with
                | _ when String.IsNullOrWhiteSpace(s) -> ()
                | s ->
                    yield s
                    yield! GetElf str (i + 1)
        }

    let rec GetElves (str: string[]) i =
        seq {
            if i >= str.Length then ()
            else
                let elf = GetElf str i
                let elfLength = elf |> Seq.length
                match elfLength with
                    | 0 -> yield! GetElves str (i + 1)
                    | elfLength -> 
                        yield elf
                        yield! GetElves str (i + elfLength)
        }

    let SizeElf e = 
        e |> Seq.sumBy LineToCalories

    let SizeElves e =
        e |> Seq.map SizeElf

    let CaloriesOfTopN n (c: seq<int>) =
        c 
            |> Seq.sortDescending
            |> Seq.take n
            |> Seq.sum

    let Solve =
        let data = ReadData
        let elves = GetElves data 0
        let numElves = elves |> Seq.length
        printf "There are %i elves" numElves
        let sizedElves = SizeElves elves
        let biggestElf = sizedElves |> Seq.max
        printf "The biggest elf has %i calories" biggestElf
        let top3Elves = sizedElves |> CaloriesOfTopN 3
        printf "The top 3 biggest elves have %i calories" top3Elves
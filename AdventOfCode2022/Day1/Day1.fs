namespace Day1

open System
open System.IO

module Solver = 

    let Filename = "Day1/input.txt"

    let ReadData = File.ReadLines Filename |> Seq.toArray

    let GetElvesOld (s: string) = s.Split (Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)

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

    let Solve =
        let data = ReadData
        let elves = GetElves data 0
        let numElves = elves |> Seq.length
        printf "there are %i elves" numElves

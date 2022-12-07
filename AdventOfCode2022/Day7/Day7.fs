namespace Day7

open System
open System.IO
open Xunit

module Solver = 

    let Filename = "Day7/input.txt"

    let NonBlank s = not (String.IsNullOrWhiteSpace s)

    let ReadData f = 
        File.ReadLines f
        |> Seq.filter NonBlank
   
    type Entry =
        | File of size : int * name : string
        | Directory of entries: Entry[] * name : string
    
    let ParseFile (line: string) = 
        let s = line.Split ([| " " |], StringSplitOptions.RemoveEmptyEntries)
        let size = Convert.ToInt32 s[0]
        File(size, s[1])

    let ParseDirectory (line: string) e =
        let s = line.Split ([| " " |], StringSplitOptions.RemoveEmptyEntries)
        Directory(e, s[1])
    
    let ParseCD (line: string) e =
        let s = line.Split ([| " " |], StringSplitOptions.RemoveEmptyEntries)
        if s[0] = "cd" then Some s[2]
        else None

    let rec PrintEntries i depth (e: Entry[]) =
        if i = e.Length then ()
        else
            for d = 0 to depth do printf " "
            match e[i] with
            | File (size, name) -> printfn "-%s (file, size=%i)" name size
            | Directory (en, name) -> printfn "-%s (dir)" name
                                      for i2 = 0 to en.Length do
                                        PrintEntries i2 (depth + 1) en      

    let Solve1 = 
        ()

    let Solve2 = 
        ()


    let Solve =
        Solve1 |> ignore
        Solve2 |> ignore

    let TestData = 
        [|
            "$ cd /";
            "$ ls";
            "dir a";
            "14848514 b.txt";
            "8504156 c.dat";
            "dir d";
            "$ cd a";
            "$ ls";
            "dir e";
            "29116 f";
            "2557 g";
            "62596 h.lst";
            "$ cd e";
            "$ ls";
            "584 i";
            "$ cd ..";
            "$ cd ..";
            "$ cd d";
            "$ ls";
            "4060174 j";
            "8033020 d.log";
            "5626152 d.ext";
            "7214296 k";
        |]
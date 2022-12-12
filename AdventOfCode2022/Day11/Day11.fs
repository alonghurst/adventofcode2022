namespace Day11

open System
open System.IO
open Xunit

module Solver = 

    let PuzzleFilename = "Day11/input.txt"
    let TestFilename = "Day11/test.txt"

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

    let StringNumber (s: string) i =
        let splits = s.Split([| " "|], StringSplitOptions.RemoveEmptyEntries)
        let n = RemoveNonNumeric splits[i]
        Convert.ToInt32 n

    let StringNumbers (s: string) i =
        let splits = s.Split([| " "|], StringSplitOptions.RemoveEmptyEntries)
        splits
        |> Array.skip i
        |> Array.map (fun c-> 
            let c2 = RemoveNonNumeric c
            Convert.ToInt64 c2)

    type Monkey = {
        number: int;
        items: int64 array;
        operator: int64 -> int64;
        test: int64;
        ifTrue: int;
        ifFalse: int;
    }

    let ParseOperator (s: string): int64 -> int64 =
        let splits = s.Split([| " "|], StringSplitOptions.RemoveEmptyEntries)
        if splits[5] = "old" then
            match splits[4] with
            | "*" -> (fun x -> x * x)
            | "/" -> (fun x -> x / x)
            | "-" -> (fun x -> x - x)
            | "+" -> (fun x -> x + x)
            | _ -> failwith $"unable to parse {splits[4]}"
        else
            let by = Convert.ToInt64 splits[5]
            match splits[4] with
            | "*" -> (fun x -> x * by)
            | "/" -> (fun x -> x / by)
            | "-" -> (fun x -> x - by)
            | "+" -> (fun x -> x + by)
            | _ -> failwith $"unable to parse {splits[4]}"

    let ParseMonkey (lines: string[]) i = 
        let idx = i * 6
        let num = StringNumber lines[idx] 1
        if num <> i  then failwith $"Parsed a non matching number {i} {num}"
        else
            let items = StringNumbers lines[idx + 1] 2
            let operator = ParseOperator lines[idx + 2] 
            let test = StringNumber lines[idx + 3] 3
            let ifTrue = StringNumber lines[idx + 4] 5
            let ifFalse = StringNumber lines[idx + 5] 5
            { 
                number = num;
                items = items;
                operator = operator;
                test = test;
                ifTrue = ifTrue;
                ifFalse = ifFalse;
            }

    let ParseMonkeys (lines: string[]) =
        seq {
            let num = lines.Length / 6
            for i in 0 .. (num - 1) do
                yield ParseMonkey lines i
        }
        |> Seq.toArray

    let PrintMonkey (m: Monkey) =
        printfn "Number: %i" m.number
        printfn "Test: %i t: %i f: %i" m.test m.ifTrue m.ifFalse
        printfn "Starting: %A" m.items

    let DeWorry x (div: int64) modulo = 
        if div = 1 then (x % modulo) else
            let x2 : float = (float x) / (float div)
            let f = Math.Floor x2
            Convert.ToInt64 f

    let RunMonkey (m: Monkey) (items: int64 list array) i div modulo =
        let inspected =
            items[m.number]
            |> List.map (fun x -> m.operator x)
            |> List.map (fun x -> DeWorry x div modulo)
        items[m.number] <- []
        for i in inspected do
            let b = i % m.test = 0
            let toNum = if b then m.ifTrue else m.ifFalse
            items[toNum] <- items[toNum] @ [i]
        let ins = 
            inspected
            |> List.length
        i + ins

    let RunMonkeys (monkeys: Monkey array) (items: int64 list array) (i: int array) div modulo =
        for m in monkeys do
            let ni = RunMonkey m items i[m.number] div modulo
            i[m.number] <- ni

    let RunRounds (monkeys: Monkey array) rounds div modulo =
        let items =
            monkeys
            |> Array.map (fun m -> m.items |> Array.toList)
        let inspected =
            monkeys
            |> Array.map (fun _ -> 0)
        for _ in 1 .. rounds do
            RunMonkeys monkeys items inspected div modulo
        (items, inspected)

    let MonkeyBusiness monkeys rounds div =
        printfn "Monkey business for rounds %i" rounds
        let modulo = 
            monkeys
            |> Array.fold (fun s m -> s * m.test) (int64 1)
        let (_, inspected) = RunRounds monkeys rounds div modulo
        printfn "Inspected: %A" inspected
        let businessMonkeys =
            inspected
            |> Array.sortDescending
            |> Array.take 2
            |> Array.map Convert.ToInt64
        let business =
            businessMonkeys
            |> Array.fold (fun (s: int64) (v: int64) -> if s = 0 then v else v * s) (int64 0)
        printfn "Monkey business was %i %A" business businessMonkeys
        printfn ""

    let Solve1 lines = 
        let monkeys = ParseMonkeys lines
        for m in monkeys do 
            PrintMonkey m
        MonkeyBusiness monkeys 20 3

    let Solve2 lines = 
        let monkeys = ParseMonkeys lines
        MonkeyBusiness monkeys 1 1
        MonkeyBusiness monkeys 20 1
        MonkeyBusiness monkeys 1000 1
        MonkeyBusiness monkeys 5000 1
        MonkeyBusiness monkeys 10000 1

    let Solve () =
        let data = ReadData PuzzleFilename
        Solve1 data |> ignore
        Solve2 data |> ignore

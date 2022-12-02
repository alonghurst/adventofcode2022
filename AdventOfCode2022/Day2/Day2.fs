namespace Day2

open System
open System.IO

module Solver = 

    let Filename = "Day2/input.txt"

    let ReadData f = File.ReadLines f

    type Move = Rock | Paper | Scissors
    type Result = Lose | Draw | Win
    
    let ParseMove c =
        match System.Char.ToLower c with
        | 'a' -> Rock
        | 'b' -> Paper
        | 'c' -> Scissors
        | 'y' -> Paper
        | 'x' -> Rock
        | 'z' -> Scissors
        | _ -> raise (System.ArgumentException($"Unable to parse \"{c}\""))
                
    let ParseResult c =
        match System.Char.ToLower c with
        | 'y' -> Draw
        | 'x' -> Lose
        | 'z' -> Win
        | _ -> raise (System.ArgumentException($"Unable to parse \"{c}\""))

    let ParseRound parseX parseY (s: string) = 
        match s with
        | s when s.Length = 3 -> Some(parseX s[0], parseY s[2])
        | _ -> None

    let ParseRounds (parse: 'a -> Option<'b>) lines =
        lines 
        |> Seq.map parse 
        |> Seq.filter (fun l -> l.IsSome) 
        |> Seq.map (fun l -> l.Value)

    let LosesVs x = 
        match x with
        | Scissors -> Paper
        | Rock -> Scissors
        | Paper -> Rock

    let DrawsVs x = x

    let WinsVs x =
        match x with
        | Scissors -> Rock
        | Rock -> Paper
        | Paper -> Scissors

    let ResolveRound (x, y) =
        match x with
        | x when x = y -> Draw
        | x when y = (WinsVs x) -> Win
        | x when y = (LosesVs x) -> Lose
        | _ -> raise (System.ArgumentException($"Unable to resolve \"{x}\" vs \"{y}\""))

    let StrategiseRound (x, y) = 
        match (x, y) with
        | (_, Lose) -> (x, LosesVs x)
        | (_, Draw) -> (x, DrawsVs x)
        | (_, Win) -> (x, WinsVs x)

    let PointsForResult r =
        match r with
        | Win -> 6
        | Draw -> 3
        | Lose -> 0
    
    let PointsForMove m =
        match m with
        | Rock -> 1
        | Paper -> 2
        | Scissors -> 3

    let PointsForRound (x, y) =
        let r = ResolveRound (x, y)
        (PointsForResult r) + (PointsForMove y)

    let PointsForRounds rounds =
        rounds
        |> Seq.map PointsForRound
        |> Seq.sum

    let StrategiesIntoRounds strategies =
        strategies
        |> Seq.map StrategiseRound

    let Solve1 = 
        let data = ReadData Filename
        let parse = ParseRound ParseMove ParseMove
        let rounds = data |> ParseRounds parse
        let points = rounds |> PointsForRounds 
        printfn "Puzzle 1: Got %i points" points

    let Solve2 = 
        let data = ReadData Filename
        let parse = ParseRound ParseMove ParseResult
        let rounds = data |> ParseRounds parse |> StrategiesIntoRounds
        let points = rounds |> PointsForRounds
        printfn "Puzzle 2: Got %i points" points

    let Solve =
        Solve1 |> ignore
        Solve2 |> ignore
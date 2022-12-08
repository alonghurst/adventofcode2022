namespace Day8

open System
open System.IO
open Xunit

module Solver = 

    let Filename = "Day8/input.txt"

    let NonBlank s = not (String.IsNullOrWhiteSpace s)

    let ReadData f = 
        File.ReadLines f
        |> Seq.filter NonBlank
  
    let inline charToInt c = int c - int '0'

    type Direction = int * int

    let Top = 0, -1
    let Bottom = 0, 1
    let Left = -1, 0
    let Right = 1, 0
    
    let AllDirections: Direction[] = 
        [| 
            Top; 
            Bottom; 
            Left; 
            Right 
        |]

    let ToGrid lines =
        lines 
        |> Seq.map (fun r -> r |> Seq.map (fun (c: char) -> charToInt c) |> Seq.toArray)
        |> Seq.toArray

    let Move (x, y) (dX, dY) = (x + dX, y + dY)

    let rec IsVisibleFromMover h d c (grid: 'a[][]) = 
        let (x, y) = Move c d
        if x < 0 || y < 0 || y = grid.Length || x = grid[y].Length then true
        else
            let hh = grid[y][x]
            if hh >= h then false
            else 
                IsVisibleFromMover h d (x, y) grid

    let IsVisible (x, y) (grid: 'a[][]) =
        if x = 0 || y = 0 || y = grid.Length - 1 || x = grid[y].Length - 1 then true
        else
            let h = grid[y][x]
            let d =
                AllDirections
                |> Seq.map (fun d -> IsVisibleFromMover h d (x, y) grid)
                |> Seq.tryFind (fun x -> x) 
            d.IsSome

    let CountVisible lines =
        let grid = ToGrid lines
        grid
        |> Seq.mapi (fun y row -> row |> Seq.mapi (fun x col -> IsVisible (x,y) grid))
        |> Seq.reduce Seq.append
        |> Seq.filter (fun b -> b)
        |> Seq.length

    let rec ScenicScoreFromMover score h d c (grid: 'a[][]) = 
        let (x, y) = Move c d
        if x < 0 || y < 0 || y = grid.Length || x = grid[y].Length then score
        else
            let hh = grid[y][x]
            if hh >= h then score + 1
            else 1 + (ScenicScoreFromMover score h d (x, y) grid)
     
    let ScenicScore (x, y) (grid: 'a[][]) =
        let h = grid[y][x]
        AllDirections
        |> Seq.map (fun d -> ScenicScoreFromMover 0 h d (x, y) grid)
        |> Seq.fold (fun s x -> if s = 0 then x else s * x) 0

    let Solve1 lines = 
        let visible = CountVisible lines
        printfn "There are %i visible trees" visible

    let Solve2 lines = 
        let grid = ToGrid lines
        let bestScore = 
            grid
            |> Seq.mapi (fun y row -> row |> Seq.mapi (fun x col -> ScenicScore (x,y) grid))
            |> Seq.reduce Seq.append
            |> Seq.max
        printfn "Best scenic score is %i" bestScore

    let Solve () =
        let data = ReadData Filename
        Solve1 data |> ignore
        Solve2 data |> ignore

    let TestData =
        [|
            "30373";
            "25512";
            "65332";
            "33549";
            "35390"    
        |]

    [<Fact>]
    let TestData_works_as_expected_for_ScenicScore_2_1 () =
        let grid = ToGrid TestData
        let c = (2, 1)
        let score = ScenicScore c grid
        Assert.Equal(score, 4)

    [<Fact>]
    let TestData_works_as_expected_for_ScenicScore_2_3 () =
        let grid = ToGrid TestData
        let c = (2, 3)
        let score = ScenicScore c grid
        Assert.Equal(score, 8)

    [<Fact>]
    let TestData_works_as_expected_for_ScenicScore_mover_2_1 () =
        let grid = ToGrid TestData
        let c = (2, 1)
        let h = grid[1][2]
        let s = ScenicScoreFromMover 0 h Top c grid
        Assert.Equal(s, 1)
        let s = ScenicScoreFromMover 0 h Bottom c grid
        Assert.Equal(s, 2)
        let s = ScenicScoreFromMover 0 h Left c grid
        Assert.Equal(s, 1)
        let s = ScenicScoreFromMover 0 h Right c grid
        Assert.Equal(s, 2)


    [<Fact>]
    let TestData_works_as_expected_1_2 () =
        let grid = ToGrid TestData
        let c = (1, 2)
        let h = grid[2][1]
        let r = IsVisibleFromMover h Right c grid
        Assert.Equal(r, true)

    [<Fact>]
    let TestData_works_as_expected () =
        let grid = ToGrid TestData
        let Test c b =
            let r = IsVisible c grid
            Assert.Equal(r, b)
        Test (0, 0) true
        Test (4, 0) true
        Test (0, 4) true
        Test (4, 4) true
        Test (1, 1) true
        Test (2, 1) true
        Test (3, 1) false
        Test (1, 2) true
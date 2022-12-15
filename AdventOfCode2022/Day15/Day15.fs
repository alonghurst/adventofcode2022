namespace Day15

open System
open System.IO
open Xunit

module Solver = 

    let PuzzleFilename = "Day15/input.txt"
    let TestFilename = "Day15/test.txt"

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

    let ParsePart (part: string) =
        let s = part.Replace("x=", "").Replace("y=", "").Split([| ", "|], StringSplitOptions.RemoveEmptyEntries)
        let x = Convert.ToInt32 s[0]
        let y = Convert.ToInt32 s[1]
        (x, y)

    let ParseLine (line: string) = 
        let parts = line.Replace("Sensor at ", "").Replace("closest beacon is at ", "").Split([| ":"|], StringSplitOptions.RemoveEmptyEntries)
        let a = ParsePart parts[0]
        let b = ParsePart parts[1]
        (a, b)

    let ManhattanDistance (xA: int, yA: int) (xB: int, yB: int) =
        let xD = Math.Abs(xA - xB)
        let yD = Math.Abs(yA - yB)
        xD + yD

    let ParseLines (lines: string array) =
        lines
        |> Array.map ParseLine
        |> Array.map (fun (a, b) -> (a, b, ManhattanDistance a b))

    let FindExtents (sbd: ((int * int) * (int * int) * int) array) h =
        if h then ((int -5000000), (int 5000000))
        else
            let xA = 
                sbd 
                |> Array.map (fun ((xA, _), (xB, _), _) -> if xA < xB then xA else xB)
                |> Array.min
            let xB = 
                sbd 
                |> Array.map (fun ((xA, _), (xB, _), _) -> if xA > xB then xA else xB)
                |> Array.max
            (xA, xB)
        
    let CantBeBeacon sbd pos h =
        let any =
            sbd
            |> Array.map (fun (s, b, d) -> (s, b, d, ManhattanDistance s pos))
            |> Array.tryFind (fun (s, b, d, dp) -> 
                //printfn "%O %O %i %i" s pos d dp
                if b = pos then h 
                else dp <= d
            )
        any.IsSome

    let NotPossibleBeacons sbd xA xB y =
        seq {
            for x in xA .. xB do
                let cantBeBeacon = CantBeBeacon sbd (x, y) false
                //printfn "%i %i %b" x y cantBeBeacon
                if cantBeBeacon then Some (x, y)
                else None
        }
        |> Seq.toArray
        |> Array.filter (fun x -> x.IsSome)
        |> Array.map (fun x -> x.Value)

    let TuningFrequency (x, y) = (x * 4000000) + y

    let BruteFind sbd min max =
        seq {
            for y in min .. max do
                if y % 10 = 0 then printfn "Y: %i" y
                for x in min .. max do
                    let cant = CantBeBeacon sbd (x, y) true
                    //printfn "%i %i %b" x y cant
                    if cant then None
                    else Some (x, y)
        }
        |> Seq.tryFind (fun x -> x.IsSome)
    
    let SensorPosition ((xS, yS), b, d) = 
        printfn "Making a sensor %i %i %i" xS yS d
        seq {
            for x in (xS - (d + 1)) .. (xS + (d + 1)) do
                for y in (yS - (d + 1)) .. (yS + (d + 1)) do
                    let dp = ManhattanDistance (xS, yS) (x, y) 
                    if dp <= d then Some (x, y)
                    else None
        }
        |> Seq.filter (fun x -> x.IsSome)
        |> Seq.map (fun x -> x.Value)
        |> Seq.toArray

    let SensorPositions sbd =
        sbd
        |> Array.map SensorPosition
        |> Array.reduce Array.append
        |> Array.distinct

    let BrutePositionalFind sbd min max =
        let positions = SensorPositions sbd 
        let rec BrutePositionalFinder x y =
            if y % 10 = 0 then printfn "Y: %i" y
            let e = positions |> Array.tryFind (fun c -> c = (x, y))
            if e.IsNone then Some (x, y)
            else
                let mutable xN = x + 1
                let mutable yN = y
                if xN > max then
                    xN <- min
                    yN <- y + 1
                if yN > max then None
                else BrutePositionalFinder xN yN
        BrutePositionalFinder min min

    let rec DoUntil (xS, yS) t (dX, dY) =
        let n = (xS + dX, yS + dY)
        seq {
            yield n
            if n = t then ()
            else yield! DoUntil n t (dX, dY)
        }

    let GetBoundary (x, y) d =
        let a = DoUntil (x, y + d) (x + d, y) (1, -1) |> Seq.toArray
        let b = DoUntil (x + d, y) (x, y - d) (-1, -1) |> Seq.toArray
        let c = DoUntil (x, y - d) (x - d, y) (-1, 1) |> Seq.toArray
        let d = DoUntil (x - d, y) (x, y + d) (1, 1) |> Seq.toArray
        a |> Array.append b |> Array.append c |> Array.append d

    let DrawBoundary (s, b, d) boundaries =
        for y in 10 .. 30 do
            printfn ""
            for x in -10 .. 10 do
                if (x, y) = s then printf "S"
                else if (x, y) = b then printf "B"
                else
                    let bound = 
                        boundaries
                        |> Array.tryFind (fun b -> b = (x,y))
                    if bound.IsSome then printf "#"
                    else printf "."
        printfn ""

    let Check a b d =
        let d2 = ManhattanDistance a b
        d2 <= d

    let BoundaryCheck (x, y) min max sbd =
        if x >= min && x <= max && y >= min && y <= max then
            let check = 
                sbd 
                |> Array.tryFind (fun (s, _, d) -> Check (x, y) s d)
            if check.IsNone then Some ((x,y))
            else None
        else None

    let CheckBoundaries boundaries min max sbd = 
        let b =
            boundaries
            |> Array.map (fun b -> BoundaryCheck b min max sbd)
            |> Array.tryFind (fun b -> b.IsSome)
        if b.IsSome then
            printfn "%O" b.Value
            if b.Value.IsSome then b.Value
            else None
        else None
        
    let FindFromBoundaries sbd min max =
        let boundaries = sbd |> Array.map (fun (s, _, d) -> GetBoundary s d)
        boundaries
            |> Array.map (fun b -> CheckBoundaries b min max sbd)
            |> Array.filter (fun b -> b.IsSome)               

    let LastTry sbd max =
        for y in 0 .. max + 1 do
            let mutable ranges: (int * int) list = []
            for ((xS, yS), b, d) in sbd do
                let dist = Math.Abs(yS - y)
                if d > dist then
                    let r = d - dist
                    let s = Math.Max(0, xS - r)
                    let e = Math.Min(max, xS + r)
                    ranges <- (s, e) :: ranges
            let mutable lastEnd = 0
            for (s, e) in ranges |> List.sort do
                if lastEnd >= s then
                    lastEnd <- Math.Max(lastEnd, e)
                else
                    let c = (lastEnd + 1, y)
                    let tuning = TuningFrequency c
                    printfn "%O %i" c tuning

    let Solve1 lines y h = 
        printfn "Solve 1"
        let sbd = ParseLines lines
        let (xA, xB) = FindExtents sbd h
        printfn "Extents are %i - %i" xA xB
        let notPossibleBeacons = NotPossibleBeacons  sbd xA xB y
        let notPossibleBeaconsCount =
            notPossibleBeacons
            |> Array.length
        printfn "There are %i places where a beacon is not possible " notPossibleBeaconsCount

    let Solve2 lines min max = 
        printfn "Solve 2"
        let sbd = ParseLines lines
        LastTry sbd max
        
    let Solve () =
        let d1 = ReadData TestFilename
        let d2 = ReadData PuzzleFilename
        Solve1 d1 10 false 
        //Solve1 d2 2000000 true 
        Solve2 d1 0 20 
        Solve2 d2 0 4000000 

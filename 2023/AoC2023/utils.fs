module Utils

open System

type Coord = {
    x: int
    y: int
}

/// Just a normal string replace function
let replaceString (oldValue: string) newValue (message: string) =
    message.Replace(oldValue, newValue)

/// Split a string by line break
let splitByLine (input: string) = input.Split([|"\n"|], StringSplitOptions.None)

/// Split a string by double line break
let splitByDoubleLine (input: string) = input.Split([|"\n\n"|], StringSplitOptions.None)

// Just a split that can be used in pipeline
let split (by: string) (input: string) = input.Split by

/// Split into characters
let splitToGrid (line: string): Char seq = line


/// If the char is '0' to '9', return the int 0 to 9
let charToInt (c: Char) = int c - int '0'

// Greatest Common Divisor
let rec gcd (a: int64) (b: int64): int64 =
    if b = 0 then a else gcd b (a % b)

// Lowest Common Multiple
let lcm (a: int64) (b: int64): int64 =
    a / (gcd a b) * b
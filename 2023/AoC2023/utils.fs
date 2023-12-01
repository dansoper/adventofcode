module Utils

open System

/// Just a normal string replace function
let replaceString (oldValue: string) newValue (message: string) =
    message.Replace(oldValue, newValue)

/// Split a string by line break
let splitByLine (input: string) = input.Split([|"\n"|], StringSplitOptions.None)

/// If the char is '0' to '9', return the int 0 to 9
let charToInt (c: Char) = int c - int '0'

module Util
open System
open System.Security.Cryptography
open Akka.Actor
open Akka.Configuration
open Akka.FSharp

    let ByteToString bytes = 
        bytes 
        |> Array.map (fun (x : byte) -> System.String.Format("{0:X2}", x))
        |> String.concat System.String.Empty

    let sha256 (s:string) =
        let byetString= System.Text.Encoding.ASCII.GetBytes(s)
        let hexString = byetString|> (new SHA256Managed()).ComputeHash
        ByteToString hexString

    let findValidBitcoin (noOfZeroes:int) (hashProduced:string) =
        let mutable isValid = false
        let mutable compareStr = ""
        for i in 0 .. noOfZeroes-1 do
            compareStr <- compareStr + "0"
        if hashProduced.StartsWith(compareStr) then
            isValid <- true
        else
            isValid <- false
        isValid

    let rec findSHAinlength (num:int) (s:string) (count:int) (actor:IActorRef)=
        if count > 0 then
            let newcount = count-1
            for (j:char) in char(33) .. char(126) do
                let tempString = s + j.ToString()
                findSHAinlength num tempString newcount actor
        else
            let sha = sha256 s
            if findValidBitcoin num sha then
                let outputString = s + "\t" + sha
                actor <! outputString

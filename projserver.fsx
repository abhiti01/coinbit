
#time "on"
#r "nuget: Akka.FSharp" 
#r "nuget: Akka.Remote"
#load "Util.fs"
open System.Security.Cryptography
open System
open Akka.Actor
open Akka.Configuration
open Akka.FSharp
open Util
[<Struct>]
    type RecordStructure =
        {
            passingString: string
            numZeroes: int
            AllocatedLength: int
        }

let config =
    Configuration.parse // Change Server IP here
        @"akka {
            actor.provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
            remote.helios.tcp {
                hostname = ""127.0.0.1""
                port = 1234
            }
        }"

let system = System.create "RemoteFSharp" config

let echoServer = 
    spawn system "EchoServer"
        (fun mailbox ->
            let mutable length = 0
            let mutable numZero = 0
            let mutable currentlength = 0
            let mutable prefString = ""
            let mutable count = 0;
            let rec loopingFunction() = actor {
                let! message = mailbox.Receive()
                let sender = mailbox.Sender()
                match box message with
                | :? string as message -> 
                    printfn "%s" message
                    count <- count + 1
                | :? bool as flag ->
                    printfn "An actor has joined"
                    if flag && currentlength < length then
                        currentlength <- currentlength + 1
                        let record = prefString + "\n" + currentlength.ToString() + "\n" + numZero.ToString()
                        sender <! record
                        
                | :? RecordStructure as message ->
                    printfn "---BITCOIN MINING STARTED---"
                    length <- message.AllocatedLength
                    numZero <- message.numZeroes
                    prefString <- message.passingString
                    count <- 0
                | _ -> ()
    
                return! loopingFunction()
            }
            loopingFunction()
        )


let BitcoinActor (mailbox: Actor<_>) =
    let rec loop() = actor {
        let! message = mailbox.Receive()
        let sender = mailbox.Sender()
        match box message with
        | :? int as x -> 
            printfn "%A" x
        | :? string as x -> 
            let inp = x.Split '\n'
            let num0 = inp.[2] |> int
            let currstr = inp.[0]
            let alloclen = inp.[1] |> int
            let s = mailbox.Self.Path.Name + " : working on task with length " + inp.[1]
            sender <! s
            findSHAinlength num0 currstr alloclen sender
            sender <! true
        | :? ActorSelection as a ->
            printfn "Active : %s" mailbox.Self.Path.Name
            a <! true
        | _ -> ()
            
        return! loop()
    }
    loop()



let input :int = if fsi.CommandLineArgs.Length > 1 then 
                    fsi.CommandLineArgs.[1] |> int 
                    else 4
let lengthRange = 50 // additional string length
let numOfActors = 4 // server actors
let s = "abhitisachdeva" // Initial string
let tempRecord = {passingString=s; AllocatedLength=lengthRange; numZeroes=input}
echoServer <! tempRecord
Console.ReadLine() |> ignore
// Making server actors
let actorList = [
    for i in 1 .. numOfActors do
        let name = "ServerActor" + i.ToString()
        let temp = 
            spawn system name BitcoinActor
        yield temp
]
let server = select "akka://RemoteFSharp/user/EchoServer" system

for i in 0 .. (actorList.Length - 1) do
    actorList.Item(i) <! server
    System.Threading.Thread.Sleep(100)

Console.ReadLine() |> ignore

0


#time "on"
#r "nuget: Akka.FSharp" 
#r "nuget: Akka.Remote"
#load "Util.fs"
open System
open System.Security.Cryptography
open Akka.FSharp
open Akka.IO
open System.Net
open System.Text
open Akka.Actor
open Akka.Remote
open Akka.Configuration
open Util

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
            let currstr = inp.[0] //string we are working on
            let alloclen = inp.[1] |> int //length which we have to fill with random characters
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

let configuration = 
    ConfigurationFactory.ParseString(
        @"akka {
            actor {
                provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
                deployment {
                    /remoteecho {
                        remote = ""akka.tcp://RemoteFSharp@127.0.0.1:1234""
                    }
                }
            }
            remote {
                helios.tcp {
                    port = 0
                    hostname = ""127.0.0.1""
                }
            }
        }")
let system = ActorSystem.Create("RemoteFSharp", configuration)
let ip = if fsi.CommandLineArgs.Length > 1 then 
            fsi.CommandLineArgs.[1]
            else "127.0.0.1"
let numOfActors = 8 // working actors in client
let actorList = [
    for i in 1 .. numOfActors do
        let name = "ClientActor" + i.ToString()
        let temp = 
            spawn system name BitcoinActor
        yield temp
]

let addr = "akka.tcp://RemoteFSharp@" + ip + ":1234/user/EchoServer"
let server = system.ActorSelection(addr)

for i in 0 .. (actorList.Length - 1) do
    actorList.Item(i) <! server
    System.Threading.Thread.Sleep(100)



System.Console.ReadLine() |> ignore

0

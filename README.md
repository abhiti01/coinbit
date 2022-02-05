
# COP5615 Distributed Operating System Principles

# How to run this program 

## Locally:
```bash
$ cd project-directory
$ dotnet fsi projserver.fsx <no-of-zeroes>
```
When the terminal says “BITCOIN MINING STARTED”, do the following in a separate terminal
```bash
$ dotnet fsi projclient.fsx
```
Press enter to deploy server’s actors on the server


## Remotely:

Change IP address of server in projserver.fsx from ""127.0.0.1"" to “”your-local-ip-address””
```bash
$ cd project-directory
$ dotnet fsi projserver.fsx <no-of-zeroes> (on server machine)
$ dotnet fsi projclient.fsx <ip-of-server> (on client machine)
```
# Requirements Completed

- [X] Print BitCoin string, with the hash value that has number of leading zeroes
    specified by the user
    
- [X] Implemented concurrency using actor model with the help of Akka.net library in F#
    
- [X] Implemented distributed implementation of actor model, which allows enlisting other machines to our coin mining abilities
    
All the testing of this program was done on an ARM based chip, which might affect results

# Size of work unit

The number of actors created on each client that connects to the server is 8.

# Results when running with input 4

Real Time for input 4 – 262200 milliseconds

CPU time for input 4 – 7800 milliseconds

Ratio of CPU time to real time – 0. 0297482

# Coin with most zeroes 

We were able to retrieve coins with upto 6 zeroes in a reasonable amount of time (using
two machines). The coins are 


abhitisachdeva#E0n
abhitisachdeva&3IV
abhitisachdeva!'xBL


# Largest number of working machines run using code 

We were able to test this code on up to 4 machines.



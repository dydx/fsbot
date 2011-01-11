#light

(*
  fsbot.fs - simple irc bot written in F#
  joshua sandlin <joshua.sandlin@gmail.com>
*)

open System
open System.IO
open System.Net
open System.Net.Sockets

let server  = "irc.enigmagroup.org"
let port    = 6697
let channel = "#enigmagroup"
let nick    = "ishbot"

(*
  Handle the basics of connecting to IRC
*)

type IRCClient( host : string, port = int ) =
  do printf "Connecting to %s:%d" host port
  
  let conn = new TcpClient()
  let reader = new StreamReader( conn.GetStream() )
  let writer = new StreamWriter( conn.GetStream() )
  writer.AutoFlush <- true
  
  // connect to the server
  member this.Connect =
    conn.Connect( host, port )

  // identify with server
  member this.Identify( nick : string )
    writer.WriteLine( sprintf "USER %s %s %s %s\n" nick nick nick nick )
    writer.WriteLine( sprintf "NICK %s\n" nick )

  // join a given room
  member this.Join( chan : string )
    writer.WriteLine( sprintf "JOIN %s\n" chan )
  

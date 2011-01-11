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
let channel = "#bots"
let nick    = "ishbot"

type IRCClient( h : string, p = int ) =

  let host = h
  let port = p
  let conn = new TcpClient()

  // read from the output stream
  member this.Read =
    let sr = new StreamReader( conn.GetStream() )
    sr.ReadLine()

  // write to the output stream
  member this.Write( msg : string ) =
    let sw = new StreamWriter( conn.GetStream() )
    sw.WriteLine( sprintf "%s\n" msg )
    sw.Flush()
  
  // connect to the server
  member this.Connect =
    conn.Connect( host, port )

  member this.Pong =
    this.Write( sprintf "PONG %s" host)

  // identify with server
  member this.Identify( nick : string ) =
    this.Write( sprintf "USER %s %s %s %s" nick nick nick nick )
    this.Write( sprintf "NICK %s" nick )

  // join a given room
  member this.Join( chan : string ) =
    this.Write( sprintf "JOIN %s" chan )

  // talk to the room
  member this.Privmsg( chan : string, msg : string ) =
    this.Write( sprintf "PRIVMSG %s %s" chan msg )

  // quit the IRC
  member this.Quit =
    this.Write( sprintf "QUIT" )

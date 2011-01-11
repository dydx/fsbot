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

type IRCClient( h : string, p : int, c : string, n : string ) =

  let host = h
  let port = p
  let chan = c
  let nick = n
  let conn = new TcpClient()

  // gather up the stream processing stuffs
  let sr = new StreamReader( conn.GetStream() )
  let sw = new StreamWriter( conn.GetStream() )
  sw.AutoFlush <- true
  
  // connect to the server
  member this.Connect =
    conn.Connect( host, port )

  member this.Pong =
    sw.WriteLine( sprintf "PONG %s\n" host)

  // identify with server
  member this.Identify =
    sw.WriteLine( sprintf "USER %s %s %s %s\n" nick nick nick nick )
    sw.WriteLine( sprintf "NICK %s\n" nick )

  // join a given room
  member this.Join =
    sw.WriteLine( sprintf "JOIN %s\n" chan )

  // talk to the room
  member this.Privmsg( msg : string ) =
    sw.WriteLine( sprintf "PRIVMSG %s %s\n" chan msg )

  // quit the IRC
  member this.Quit =
    sw.WriteLine( sprintf "QUIT\n" )

  member this.Homeostasis =
    do this.Connect   // connect to server
    do this.Identify // identify with server
    do this.Join      // join a room
    Console.WriteLine( sprintf "Successfully joined %s on %s:%d as %s\n" chan host port nick)
    while( sr.EndOfStream = false ) do
      let line = sr.ReadLine()
      if (line.Contains("PING")) then
        this.Pong
      let msg = line.Substring( line.Substring(1).IndexOf(":") + 2)
      if (msg.Contains("*help")) then
        this.Privmsg( "What do you want now?" )


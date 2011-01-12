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
let port    = 6667
let channel = "#bots"
let nick    = "ishbot"

type IRCClient( h : string, p : int, c : string, n : string ) =

  let host = h
  let port = p
  let chan = c
  let nick = n
  let conn = new TcpClient()
  do conn.Connect( host, port )

  // gather up the stream processing stuffs
  let sr = new StreamReader( conn.GetStream() )
  let sw = new StreamWriter( conn.GetStream() )

  member this.Pong =
    sw.WriteLine( sprintf "PONG %s\n" host)
    sw.Flush()

  // identify with server
  member this.Identify =
    sw.WriteLine( sprintf "USER %s %s %s %s\n" nick nick nick nick )
    sw.WriteLine( sprintf "NICK %s\n" nick )
    sw.Flush()

  // join a given room
  member this.Join =
    sw.WriteLine( sprintf "JOIN %s\n" chan )
    sw.Flush()

  // talk to the room
  member this.Privmsg( msg : string ) =
    sw.WriteLine( sprintf "PRIVMSG %s %s\n" chan msg )
    sw.Flush()

  // quit the IRC
  member this.Quit =
    sw.WriteLine( sprintf "QUIT\n" )
    sw.Flush()

  member this.Run =
    do this.Identify // identify with server
    do this.Join      // join a room
    Console.WriteLine( sprintf "Successfully joined %s on %s:%d as %s\n" chan host port nick)
    while( sr.EndOfStream = false ) do
      let line = sr.ReadLine()
      if (line.Contains("PING")) then
        this.Pong
        
      let msg = line.Substring( line.Substring(1).IndexOf(":") + 2)
      let (|Prefix|_|)(p:string)(s:string) =
        if s.StartsWith( p ) then
          Some( s.Substring(p.Length))
        else
          None
      match msg with
      | Prefix "!version" rest -> this.Privmsg ( sprintf "%s on %A" nick Environment.Version )
      | Prefix "!date" rest -> this.Privmsg ( sprintf "%A" System.DateTime.Now )
      | Prefix "!help" rest -> this.Privmsg "Google it for now..."
       


// Lets actually use this badboy!
let fsbot = new IRCClient( server, port, channel, nick )
fsbot.Run


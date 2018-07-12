module Intranet

open System.Net
open System.IO

type Data =
    {FullName : string
     UserName : string
     Password : string
     Cookie   : CookieContainer
     }

type UserData = 
    Student of Data
  | Professor of Data
  | Staff of Data


val getFullName  : UserData -> string

val getUserName  : UserData -> string

val getPassword  : UserData -> string

val getUserType  : UserData -> string

val getCookie    : UserData -> CookieContainer

val newUser      : UserData -> UserData

val changeCookie : UserData -> CookieContainer -> UserData

val login : string -> string -> UserData option

val response_string : string -> (string * string) list -> UserData -> (string -> 'a) -> ('a * UserData)
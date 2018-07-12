﻿module Intranet

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

let getFullName user = 
    match user with
        | Student data -> data.FullName
        | Professor data -> data.FullName
        | Staff data -> data.FullName

let getUserName user = 
    match user with
        | Student data -> data.UserName
        | Professor data -> data.UserName
        | Staff data -> data.UserName

let getPassword user = 
    match user with
        | Student data -> data.Password
        | Professor data -> data.Password
        | Staff data -> data.Password

let getUserType user = 
    match user with
        | Student data -> "Student"
        | Professor data -> "Professor"
        | Staff data -> "Staff"

let getCookie user = 
    match user with
        | Student data -> data.Cookie
        | Professor data -> data.Cookie
        | Staff data -> data.Cookie

let changeCookie user cookie = 
    match user with
        | Student data -> Student {data with Cookie = cookie}
        | Professor data -> Professor {data with Cookie = cookie}
        | Staff data -> Staff {data with Cookie = cookie}

let student_data = ("http://estudiantes.upslp.edu.mx:9080/Intralumno/go_alumno",
                    "http://estudiantes.upslp.edu.mx:9080/Intralumno/alumno.do",
                    "E",
                    Student)

let professor_data = ("http://intranet.upslp.edu.mx:9080/Users/go_academico",
                      "http://intranet.upslp.edu.mx:9080/Users/user.do",
                      "A",
                      Professor)

let staff_data = ("http://intranet.upslp.edu.mx:9080/Users/go_personal",
                  "http://intranet.upslp.edu.mx:9080/Users/user.do",
                  "P",
                  Staff)

let read_response (response : HttpWebResponse) =
    let receiveStream = response.GetResponseStream()
//    let encode = System.Text.Encoding.GetEncoding("utf-8")
    let encode = System.Text.Encoding.GetEncoding("ISO-8859-1")
    let readStream = new StreamReader( receiveStream, encode )
    let response = readStream.ReadToEnd()
    readStream.Close()
    response

let request_string cookie base_url parameters =
    let query = parameters
                    |> List.map (fun (k, v) -> k + "=" + v)
                    |> String.concat "&"
    let url = base_url + "?" + query
    let request = WebRequest.CreateHttp(url)
    request.CookieContainer <- cookie
    let response = request.GetResponse() :?> HttpWebResponse
    read_response response

let login user password =
    let encrypt str =
        String.collect (fun c -> let c' = (string << int) c
                                 (string << String.length) c' + c') str
    let checkLogin (url1, url2, t, kind) =
        let cookie = new CookieContainer()
        ignore (request_string cookie url1 [])
        let welcome = request_string cookie url2
                                     [("mensaje",password); ("method", "home"); ("name", user); ("pwd", encrypt password); ("tipo",t)]

        if welcome.Contains "<h1>Bienvenid@"
        then let welcome' = welcome.Substring (welcome.IndexOf "<h1>Bienvenid@")
             if welcome'.Contains "@ "
             then let welcome'' = welcome'.Substring (welcome'.IndexOf "@ " + 2)
                  if welcome''.Contains "!"
                  then //Some (kind (welcome''.Remove (welcome'.IndexOf "@ "), cookie))
                       Some (kind {FullName = welcome''.Remove (welcome''.IndexOf "!")
                                   UserName = user
                                   Password = password
                                   Cookie = cookie})
                  else None
             else None
        else None
    List.tryPick checkLogin [student_data; professor_data; staff_data]

let newCookie user =
    match login (getUserName user) (getPassword user) with
        Some user -> Some (getCookie user)
      | None      -> None

let rec newUser user =
    match login (getUserName user) (getPassword user) with
        Some user -> user
      | None      -> failwith "Login failed!"

let rec recursive_timeout time f v =
    try
        let tokenSource = new System.Threading.CancellationTokenSource()
        let token = tokenSource.Token
        let task = System.Threading.Tasks.Task.Factory.StartNew(fun () -> f v, token)
        if not (task.Wait(time, token))
        then printfn "Tiempo de espera agotado! Intentando nuevamente..."
             recursive_timeout time f v
        else (fun (x, y) -> x) task.Result
    with e -> printfn "Se generó alguna excepción! Intentando nuevamente..."
              recursive_timeout time f v

let db_timeout = 120000

let rec response_string url parameters user parse =
    try
        let f () = request_string 
                        (getCookie user)
                        url
                        parameters
        let str = recursive_timeout db_timeout f ()
        (parse str, user)
    with | :? System.Xml.XmlException   -> let cookie = Option.get (newCookie user)
                                           let user = changeCookie user cookie
                                           response_string url parameters user parse
         | :? System.Net.WebException   -> let cookie = Option.get (newCookie user)
                                           let user = changeCookie user cookie
                                           response_string url parameters user parse
         | :? System.AggregateException -> let cookie = Option.get (newCookie user)
                                           let user = changeCookie user cookie
                                           response_string url parameters user parse

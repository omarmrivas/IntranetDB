// Learn more about F# at http://fsharp.org

open System

let readPassword () =
    let rec readMask pw =
        let k = System.Console.ReadKey()
        match k.Key with
        | System.ConsoleKey.Enter -> pw
        | System.ConsoleKey.Escape -> pw
        | System.ConsoleKey.Backspace ->
            match pw with
            | [] -> readMask []
            | _::t ->
                System.Console.Write " \b"
                readMask t
        | _ ->
            System.Console.Write "\b*"
            readMask (k.KeyChar::pw)
    let password = readMask [] |> Seq.rev |> System.String.Concat
    System.Console.WriteLine ()
    password

let periodo_actual () =
    string DateTime.Now.Year + 
    (match DateTime.Now.Month with
          1 | 2 | 3 | 4 | 5 | 6 -> "1S"
          | _ -> "3S")

let periodos () = 
        "20013S" ::
            [for ano in 2002 .. System.DateTime.Now.Year do
                for s in 1..3 do
                    yield string ano + string s + "S"]

let carreras = ["ITI"; "ITEM"; "ISTI"; "ITMA"; "LAG"; "LMKT"]

let rec verifica_periodo (periodo : string) =
    match periodo.Split [|'-'|] |> Array.toList with
        [starts; ends] -> List.exists (fun s -> s = starts) (periodos ()) &&
                          List.exists (fun s -> s = ends) (periodos ())
      | [periodo] -> 
            if not (List.exists (fun s -> s = periodo) ("*" :: periodos ()))
            then printfn "El valor de periodo debe ser uno de los siguientes valores: %A" periodos
                 false
            else true
      | _ -> printfn "Periodo %s no esperado" periodo
             false

let obtiene_periodos (periodo : string) =
    match periodo.Split [|'-'|] |> Array.toList with
        [starts; ends] -> periodos () |> List.skipWhile (fun p -> p <> starts)
                                      |> List.takeWhile(fun p -> p <> ends)
                                      |> (fun l -> l @ [ends])
      | [periodo] -> 
            match periodo with
                "*" -> periodos ()
              | periodo -> [periodo]
      | _ -> printfn "Periodo %s no esperado" periodo
             []

let verifica_carrera carrera =
    if not (List.exists (fun s -> s = carrera) ("*" :: carreras))
    then printfn "El valor de carrera debe ser uno de los siguientes valores: %A" ("*" :: carreras)
         false
    else true

let obtiene_carreras carrera =
    match carrera with
        "*" -> carreras
      | carrera -> [carrera]


[<EntryPoint>]
let main argv =
    let accion = argv.[0]

    match accion with
        "update" -> printf "Usuario: "
                    let usuario = System.Console.ReadLine ()
                    printf "Contraseña: "
                    let password = readPassword ()
                    printf "Carrera: "
                    let carrera = System.Console.ReadLine ()
                    printf "Periodo: "
                    let periodo = System.Console.ReadLine ()
                    if verifica_carrera carrera && verifica_periodo periodo
                    then let programas = obtiene_carreras carrera
                         let periodos = obtiene_periodos periodo
                         printfn "Actualizando programas %A en el periodo %A." programas periodos
                         match Intranet.login usuario password with
                            Some user -> printfn "Bienvenido %s" (Intranet.getFullName user)
                                         for periodo in periodos do
                                             for carrera in carreras do
                                                 let (alumnos, user) = Alumnos.obtener_alumnos user (carrera, periodo)
                                                 Array.iter DB.actualiza_alumno alumnos
                                                 let (profesores, user) = Profesores.obtener_profesores user (carrera, periodo)
                                                 Array.iter DB.actualiza_profesor profesores
                                                 let (materias, user) = Kardex.obtener_kardex user (carrera, periodo)
                                                 Array.iter DB.actualiza_kardex materias
                          | None -> printfn "Error en registro (usuario o contraseña invalidos)"
                    else printfn "Saliendo"
(*      | "train"  -> printf "Carrera: "
                    let carrera = System.Console.ReadLine ()
                    printf "Periodo: "
                    let periodo = System.Console.ReadLine ()
                    if verifica_carrera carrera && verifica_periodo periodo
                    then let programas = obtiene_carreras carrera
                         let periodos = obtiene_periodos periodo
                         printfn "Entrenando modelos predictivos para programa %A en el periodo %A." programas periodos
                         let resultado = DB.datos_entrenamiento (Types.periodo_to_int "20171S") 
                                                                (Types.periodo_to_int "20171S") "Análisis y Diseño de Algoritmos" "ITI"
                         List.iter (printfn "%A") resultado
                         printfn "%i" (List.length resultado)
                    else printfn "Saliendo"*)
      | _ -> printfn "Comando no reconocido"

    0 // return an integer exit code


// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

(*open System.IO


[<EntryPoint>]
let main argv =
    if Array.length argv > 0 &&
       argv.[0] = "-csv"
    then printf "Archivo de asignación parcial: "
         let archivo = System.Console.ReadLine ()
         Asignacion.to_csv archivo
    elif Array.length argv > 0 &&
         argv.[0] = "-mail"
    then printf "Archivo de asignación parcial: "
         let archivo = System.Console.ReadLine ()
         let matriculas = Asignacion.matriculas archivo
         List.iter (printfn "%s@upslp.edu.mx") matriculas
    else 
        printf "Usuario: "
        let usuario = System.Console.ReadLine ()
        printf "Contraseña: "
        let password = Library.readPassword ()
        printf "Carrera: "
        let carrera = System.Console.ReadLine ()
        let periodo = periodo ()
        printfn "Periodo: %s" periodo
        printf "Archivo de asignación parcial: "
        let parcial = System.Console.ReadLine ()


        let user = IntranetAccess.login usuario password

        match user with
            Some user -> let asignaciones = Asignacion.hacer_asignacion parcial user (carrera, periodo)
                         let outFile = new StreamWriter(carrera + ".txt")
                         Array.iter (outFile.WriteLine : string -> unit) asignaciones
                         outFile.Flush()
                         outFile.Close()
                         let asignaciones = Asignacion.nivelar_asesorias user (carrera, periodo)
                         let outFile = new StreamWriter(carrera + ".txt")
                         Array.iter (outFile.WriteLine : string -> unit) asignaciones
                         outFile.Flush()
                         outFile.Close()
                         printfn "Alumnos asesorados: %d" (Asignacion.contar_asesorados (carrera + ".txt"))
                         printfn "Alumnos revisados: %d" (Asignacion.contar_revisiones (carrera + ".txt"))
                         Asignacion.profesores_con_menos_alumnos (carrera + ".txt")
          | None -> ()

    0 // return an integer exit code*)


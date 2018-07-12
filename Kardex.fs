module Kardex

open FSharp.Data
open Types


[<Literal>]
let Parciales = """
    <table>
        <row>
            <column>100251</column>
            <column>Narváez Carrizalez Imelda</column>
            <column>ITI</column>
            <column>20151S</column>
            <column>Compiladores</column>
            <column>E15-837 - Reyes  / Alejandro </column>
            <column>7.5</column>
            <column>0</column>
            <column>5.0</column>
            <column>2</column>
            <column>8.0</column>
            <column>0</column>
            <column>8.0</column>
            <column>7.3</column>
            <column>2</column>
            <column> </column>
            <column> </column>
        </row>
        <row>
            <column>100251</column>
            <column>Narváez Carrizalez Imelda</column>
            <column>ITI</column>
            <column>20151S</column>
            <column>Inteligencia Artificial II</column>
            <column>E15-643 - Montaño Rivas / Omar</column>
            <column>6.5</column>
            <column>4</column>
            <column>4.0</column>
            <column>0</column>
            <column>10.0</column>
            <column>0</column>
            <column>10.0</column>
            <column>8.1</column>
            <column>4</column>
            <column> </column>
            <column> </column>
        </row>
    </table>"""

type TParciales = XmlProvider<Parciales>

let extraerGrupo (info : string) =
    match info.Split [|'-'|] |> Array.toList with
        | code1 :: code2 :: _ -> let grupo = code1.Trim() + "-" + code2.Trim()
                                 if String.length grupo > 7
                                 then code1.Trim()
                                 else grupo
        | _ -> printfn "No se pudo extraer la información del grupo! %s" info
               ""

let rec calificacion cal =
    match cal with
        "" -> None
      | "AC" -> Some AC
      | "NP" -> Some NP
      | "SD" -> Some SD
      | str -> if str.Length > 5
               then (try
                        let v = double str
                        if v > 10.0
                        then let str' = string (v / 10.0)
                             printfn "Cambiando calificación '%s' por '%s'" str str'
                             calificacion str'
                        else printfn "Algun error en el valor numérico de '%s' (truncando)" str
                             Some (Error (str.Substring(0,4)))
                     with | :? System.FormatException -> 
                                    printfn "Algun error en el formato del valor numérico de '%s' (truncando)" str
                                    Some (Error (str.Substring(0,4))))
               else try double str |> ignore
                        Some (Val str)
                    with | :? System.FormatException -> 
                                    printfn "Algun error en el formato del valor numérico de '%s' (omitiendo)" str
                                    None

let inasistencia i =
    match i with
        "" -> None
      | _ -> try 
                Some (int i)
             with | :? System.FormatException -> 
                            printfn "Algun error en el formate del valor numérico de '%s'" i
                            None

let obtener_estatus final extra regu =
    let aux cal =
        match cal with
            Some AC -> Some "Aprobado"
          | Some NP -> Some "Reprobado"
          | Some SD -> Some "Reprobado"
          | Some (Error _) -> None
          | Some (Val str) -> if double str < 7.0
                              then Some "Reprobado"
                              else Some "Aprobado"
          | None -> None
    match aux regu with
        Some x -> Some x
      | None -> match aux extra with
                    Some x -> Some x
                  | None -> match aux final with
                              Some x -> Some x
                            | None -> None

let obtener_kardex user (carrera, periodo) =
    printfn "Actualizando el kardex de la carrera %s en el periodo %s..." carrera periodo
//    let user = Intranet.newUser user
    let url = @"http://intranet.upslp.edu.mx:9080/Users/kardex.do"
    let parameters =
        [("6578706f7274","1")
         ("cveMateria","0")
         ("d-1782-e","3")
         ("gpo","*")
         ("matricula","*")
         ("method","parciales")
         ("nomalu","*")
         ("nommat","*")
         ("pdo",periodo)
         ("plan",carrera)
         ("rep","si")]
    let (rows, user) = Intranet.response_string url parameters user (fun xml -> TParciales.Parse(xml).Rows)
//    let rows = TParciales.Parse(xml).Rows
    let materias = Array.map (fun (materia : TParciales.Row) ->
                let valores = [| for campo in materia.Columns do
                                                match campo.String with
                                                    Some s -> yield s
                                                  | _ -> yield "" |]
                let matricula = valores.[0].Trim()
                let materia = valores.[4].Trim()
                let grupo = extraerGrupo (valores.[5].Trim())
                let c1 = calificacion (valores.[6].Trim())
                let i1 = inasistencia (valores.[7].Trim())
                let c2 = calificacion (valores.[8].Trim())
                let i2 = inasistencia (valores.[9].Trim())
                let c3 = calificacion (valores.[10].Trim())
                let i3 = inasistencia (valores.[11].Trim())
                let efinal = calificacion (valores.[12].Trim())
                let final = calificacion (valores.[13].Trim())
                let inasistencias = inasistencia (valores.[14].Trim())
                let extraordinario = calificacion (valores.[15].Trim())
                let regularizacion = calificacion (valores.[16].Trim())
                let estatus = obtener_estatus final extraordinario regularizacion
                let kardex = {matricula      = matricula
                              grupo          = grupo
                              materia        = materia
                              periodo        = periodo
                              carrera        = carrera
                              c1             = c1
                              i1             = i1
                              c2             = c2
                              i2             = i2
                              c3             = c3
                              i3             = i3
                              efinal         = efinal
                              final          = final
                              inasistencias  = inasistencias
                              extraordinario = extraordinario
                              regularizacion = regularizacion
                              estatus        = estatus} : kardex
                kardex) rows
    (materias, user)

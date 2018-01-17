module Alumnos

open System
open FSharp.Data
open System.Net
open System.Collections.Generic

[<Literal>]
let TablaAlumnos = """
    <table>
        <row>
            <column>20013S</column>
            <column>Inscripci&#243;n</column>
            <column>1</column>
            <column>ITI</column>
            <column>010393</column>
            <column>Alba Torres Victor Hugo</column>
            <column>M</column>
            <column>  21</column>
            <column>20013S</column>
            <column></column>
            <column>8-11-73-31</column>
            <column>BASALTO #500</column>
            <column>San Alberto </column>
            <column>San Luis Potos&#237;</column>
            <column>S.E.E.R. - Profra. Celia Fernandez Capetillo</column>
            <column></column>
            <column></column>
            <column>08-01-1980</column>
            <column></column>
            <column></column>
        </row>
        <row>
            <column>20013S</column>
            <column>Inscripci&#243;n</column>
            <column>1</column>
            <column>ITI</column>
            <column>010284</column>
            <column>&#193;lvarez Castillo Gabriel Iznardo</column>
            <column>M</column>
            <column>  17</column>
            <column>20013S</column>
            <column></column>
            <column>8-31-10-95</column>
            <column>Lanzagorta 606 &quot;A&quot;</column>
            <column>Soledad de Graciano Sanch&#233;z</column>
            <column>San Luis Potos&#237;</column>
            <column>S.E.E.R. - Profr.Librado Rivera</column>
            <column></column>
            <column></column>
            <column>07-05-1984</column>
            <column></column>
            <column></column>
        </row>
    </table>"""

type Alumnos = XmlProvider<TablaAlumnos>

let obtener_alumnos user (carrera, periodo) = 
    printfn "Actualizando Alumnos de la carrera %s en el periodo %s..." carrera periodo
    let url = @"http://intranet.upslp.edu.mx:9080/Users/periodo.do"
    let parameters =
        [("6578706f7274","1")
         ("d-1782-e", "3")
         ("matricula", "*")
         ("method", "inscritos")
         ("nomalu","*")
         ("pdo",periodo)
         ("planest", carrera)
         ("reg1","0")
         ("reg2", "99")
         ("rep", "si")
         ("sem1","0")
         ("sem2","0")
         ("sexo","*")
         ("ultimo","20013S")]
    let (rows, user) = Intranet.response_string url parameters user (fun xml -> Alumnos.Parse(xml).Rows)
    let alumnos = 
        Array.map (fun (alumno : Alumnos.Row) ->
        let valores = [| for campo in alumno.Columns do
                            match campo.String with
                                Some s -> yield s
                                | _ -> yield "" |]
        let alumno =
            {matricula = valores.[4].Trim()
             nombre    = valores.[5].Replace("'", "")
             genero    = valores.[6].Trim()
             fecha_nacimiento = [| '-' |] |> valores.[18].Split
                                          |> (fun arr -> match arr with
                                                          [| dia; mes; ano |] -> DateTime(int ano, int mes, int dia)
                                                         | _ -> DateTime.Now)
             periodo   = valores.[0].Trim()
             ingreso   = valores.[8].Trim()
             telefono  = valores.[10].Trim()
             direccion = valores.[11].Trim()
             colonia   = valores.[12].Trim()
             cp        = valores.[13].Trim()
             municipio = valores.[14].Trim()
             procedencia = valores.[15].Trim()
             semestre  = valores.[2].Trim() |> int
             plan      = carrera} : Types.Alumno
        alumno) rows
    (alumnos, user)

module DB

open FSharp.Data.Sql
open System.Data.Linq.SqlClient
open Types

// Add references to System.Data and System.Data.Linq.

[<Literal>]
let connectionString = @"Server=localhost,1401; User ID=SA; Password=HolaMund0!; Database=Intranet"

[<Literal>]
let dbVendor = Common.DatabaseProviderTypes.MSSQLSERVER

let [<Literal>] indivAmount = 1000

type sql = 
    SqlDataProvider<
        ConnectionString = connectionString,
        DatabaseVendor = dbVendor,
        IndividualsAmount = indivAmount,
        UseOptionTypes = true >

//type private EntityConnection = SqlEntityConnection<ConnectionString=connectionString,Pluralize = true> ?

let actualiza_alumno (alumno : Alumno) =
    let ctx = sql.GetDataContext()
    let result = query { for registro in ctx.Dbo.Alumnos do
                         where (registro.Matricula = alumno.matricula && registro.Periodo = Types.periodo_to_int alumno.periodo)
                         select (registro) }
    let registro = match Seq.tryHead result with
                        Some registro -> registro
                      | None -> let registro = ctx.Dbo.Alumnos.Create()
                                registro.Matricula <- alumno.matricula
                                registro.Periodo <- Types.periodo_to_int alumno.periodo
                                registro
    registro.Nombre <- alumno.nombre
    registro.Genero <- alumno.genero
    registro.FechaNacimiento <- alumno.fecha_nacimiento
    registro.Ingreso <- Types.periodo_to_int alumno.ingreso
    registro.Telefono <- alumno.telefono
    registro.Direccion <- alumno.direccion
    registro.Colonia <- alumno.colonia
    registro.Cp <- alumno.cp
    registro.Municipio <- alumno.municipio
    registro.Procedencia <- alumno.procedencia
    registro.Semestre <- alumno.semestre
    registro.Plan <- alumno.plan
    ctx.SubmitUpdates()


let actualiza_profesor (profesor : Profesor) =
    let ctx = sql.GetDataContext()
    let result = query { for registro in ctx.Dbo.Profesores do
                         where (registro.Profesor = profesor.profesor && registro.Periodo = Types.periodo_to_int profesor.periodo)
                         select (registro) }
    let registro = match Seq.tryHead result with
                        Some registro -> registro
                      | None -> let registro = ctx.Dbo.Profesores.Create()
                                registro.Profesor <- profesor.profesor
                                registro.Periodo <- Types.periodo_to_int profesor.periodo
                                registro
    registro.Nombre <- profesor.nombre
    registro.Apellidos <- profesor.apellidos
    registro.Tipo <- profesor.tipo
    ctx.SubmitUpdates()


let actualiza_kardex (kardex : kardex) =
    let ctx = sql.GetDataContext()
    let result = query { for registro in ctx.Dbo.Kardex do
                         where (registro.Matricula = kardex.matricula && registro.Grupo = kardex.grupo)
                         select (registro) }
    let registro = match Seq.tryHead result with
                        Some registro -> registro
                      | None -> let registro = ctx.Dbo.Kardex.Create()
                                registro.Matricula <- kardex.matricula
                                registro.Grupo <- kardex.grupo
                                registro
    registro.Materia <- kardex.materia
    registro.Periodo <- Types.periodo_to_int kardex.periodo
    registro.Carrera <- kardex.carrera
    registro.C1 <- cal_to_float kardex.c1
    registro.I1 <- kardex.i1
    registro.C2 <- cal_to_float kardex.c2
    registro.I2 <- kardex.i2
    registro.C3 <- cal_to_float kardex.c3
    registro.I3 <- kardex.i3
    registro.Efinal <- cal_to_float kardex.efinal
    registro.Final <- cal_to_float kardex.final
    registro.Inasistencias <- kardex.inasistencias
    registro.Extraordinario <- cal_to_float kardex.extraordinario
    registro.Regularizacion <- cal_to_float kardex.regularizacion
    registro.Estatus <- kardex.estatus
    ctx.SubmitUpdates()

let datos_entrenamiento periodoInicial periodoFinal materia carrera =
    let ctx = sql.GetDataContext()
    ctx.Procedures.DatosEntrenamiento.Invoke(periodoInicial, periodoFinal, materia, carrera).ResultSet
        |> Seq.map (fun r -> r.MapTo<Types.kardex_in>())
        |> Seq.distinctBy (fun k -> (k.matricula, k.materia))
        |> Seq.toList


(* SQL Not supported! *)
(*let datos_entrenamiento2 periodoInicial periodoFinal materia carrera =
    let ctx = sql.GetDataContext()
    query {for A in ctx.Dbo.Kardex do
           where (A.Periodo <= periodoFinal &&
                  A.Carrera = carrera &&
//                  A.Estatus <> None &&
                  query {for B in ctx.Dbo.Kardex do
                         exists (B.Materia = materia &&
                                 B.Carrera = carrera &&
//                                 B.Estatus <> None &&
                                 A.Matricula = B.Matricula &&
                                 B.Periodo <= periodoFinal &&
                                 B.Periodo >= periodoInicial)})
           sortBy A.Matricula
           thenBy A.Materia
           thenBy A.Periodo
           select A}
        |> Seq.map (fun r -> r.MapTo<Types.kardex_in>())
        |> Seq.distinctBy (fun k -> (k.matricula, k.materia))
        |> Seq.toList*)

module Types

open System

type Alumno =
    {matricula : string
     periodo   : string
     nombre    : string
     genero    : string
     fecha_nacimiento : DateTime
     ingreso   : string
     telefono  : string
     direccion : string
     colonia   : string
     cp        : string
     municipio : string
     procedencia : string
     semestre  : int
     plan      : string}

type Profesor =
    {profesor  : int
     periodo   : string
     nombre    : string
     apellidos : string
     tipo      : string}


(*
    AC = Acreditada
    NP = No presentó
    SD = Sin derecho
    CP = ?
    NA = ?
    REG = ?
*)

type cal = AC | NP | SD | Val of string | Error of string

type kardex = 
    {matricula      : string
     grupo          : string
     materia        : string
     periodo        : string
     carrera        : string
     c1             : cal option
     i1             : int option
     c2             : cal option
     i2             : int option
     c3             : cal option
     i3             : int option
     efinal         : cal option
     final          : cal option
     inasistencias  : int option
     extraordinario : cal option
     regularizacion : cal option
     estatus        : string option}

let cal_to_float cal =
    match cal with
        Some AC -> Some 10.0
      | Some NP -> Some -10.0
      | Some SD -> Some -5.0
      | Some (Val v) -> (Some << float) v
      | Some (Error _) -> None
      | None -> None


let periodo_to_int (periodo : string) =
    let sem = periodo.Substring(4).Remove(1)
    let year = periodo.Remove(4)
    (int year - 2001) * 3 + int sem - 1


type kardex_in = 
    {matricula      : string
     grupo          : string
     materia        : string
     periodo        : int
     carrera        : string
     c1             : float
     i1             : int
     c2             : float
     i2             : int
     c3             : float
     i3             : int
     efinal         : float
     final          : float
     inasistencias  : int
     extraordinario : float
     regularizacion : float
     estatus        : string}

CREATE PROCEDURE datos_entrenamiento 
      @periodoInicial int,
      @periodoFinal int,
      @materia NVARCHAR(300),
      @carrera NVARCHAR(5)
AS
SELECT matricula, grupo, materia, periodo, carrera, c1, i1, c2, i2, c3, i3, efinal, [final], inasistencias, extraordinario, regularizacion, estatus
FROM Intranet.dbo.Kardex A
    WHERE (A.periodo <= @periodoFinal AND
           (A.periodo < @periodoFinal OR A.materia = @materia) AND
           A.carrera = @carrera AND
           A.estatus IS NOT NULL AND
           EXISTS (SELECT * FROM Intranet.dbo.Kardex B
                       WHERE (B.materia = @materia AND
                              B.carrera = @carrera AND
                              B.estatus IS NOT NULL AND
                              A.matricula = B.matricula AND
                              B.periodo <= @periodoFinal AND
                              B.periodo >= @periodoInicial)))
ORDER BY A.matricula, A.materia, A.periodo;
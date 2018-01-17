CREATE TABLE Intranet.dbo.Alumnos (
    matricula nchar(6) NOT NULL,
    periodo int NOT NULL,
    nombre nvarchar(200) NOT NULL,
    genero nchar(1) NOT NULL,
    fecha_nacimiento date NOT NULL,
    ingreso int NOT NULL,
    telefono nvarchar(100) NOT NULL,
    direccion nvarchar(300) NOT NULL,
    colonia nvarchar(300) NOT NULL,
    cp nvarchar(10) NOT NULL,
    municipio nvarchar(200) NOT NULL,
    procedencia nvarchar(500) NOT NULL,
    semestre int NOT NULL,
    [plan] nvarchar(5) NOT NULL,
    CONSTRAINT Alumnos_PK PRIMARY KEY (matricula,periodo)
) go

CREATE TABLE Intranet.dbo.Profesores (
    profesor int NOT NULL,
    periodo int NOT NULL,
    nombre nvarchar(200) NOT NULL,
    apellidos nvarchar(200) NOT NULL,
    tipo nvarchar(100) NOT NULL,
    CONSTRAINT Profesor_PK PRIMARY KEY (profesor,periodo)
) go
IF OBJECT_ID(N'dbo.clientes', N'U') IS NULL
BEGIN
 CREATE TABLE dbo.clientes (
  id_cliente INT IDENTITY(1,1) NOT NULL,
  cui VARCHAR(13) NULL,
  nit VARCHAR(15) NOT NULL,
  nombres VARCHAR(60) NOT NULL,
  apellidos VARCHAR(60) NOT NULL,
  direccion VARCHAR(150) NULL,
  telefono VARCHAR(12) NULL,
  fecha_nacimiento DATE NULL,
  CONSTRAINT PK_clientes PRIMARY KEY CLUSTERED (id_cliente)
 );
END;

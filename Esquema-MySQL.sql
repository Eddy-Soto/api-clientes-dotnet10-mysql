CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) NOT NULL,
    `ProductVersion` varchar(32) NOT NULL,
    PRIMARY KEY (`MigrationId`)
);

START TRANSACTION;
CREATE TABLE `Clientes` (
    `Id_cliente` int NOT NULL AUTO_INCREMENT,
    `CUI` varchar(13) NOT NULL,
    `NIT` varchar(20) NOT NULL,
    `Nombres` varchar(100) NOT NULL,
    `Apellidos` varchar(100) NOT NULL,
    `Direccion` varchar(250) NOT NULL,
    `Telefono` varchar(25) NOT NULL,
    `Fecha_Nacimiento` date NOT NULL,
    PRIMARY KEY (`Id_cliente`)
);

CREATE UNIQUE INDEX `IX_Clientes_CUI` ON `Clientes` (`CUI`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260926054746_InitialMySql', '10.0.11');

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260926055736_ConvertirFechaMySql', '10.0.11');

COMMIT;


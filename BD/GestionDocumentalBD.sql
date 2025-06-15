CREATE DATABASE  IF NOT EXISTS `gestiondocumentalbd` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `gestiondocumentalbd`;
-- MySQL dump 10.13  Distrib 8.0.40, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: gestiondocumentalbd
-- ------------------------------------------------------
-- Server version	8.0.40

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `centros_educativos`
--

DROP TABLE IF EXISTS `centros_educativos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `centros_educativos` (
  `id_centro_educativo` int NOT NULL AUTO_INCREMENT,
  `nombre` varchar(255) DEFAULT NULL,
  `direccion` varchar(255) DEFAULT NULL,
  `cif` varchar(20) DEFAULT NULL,
  `telefono` varchar(20) DEFAULT NULL,
  `fax` varchar(20) DEFAULT NULL,
  `codigo_postal` varchar(10) DEFAULT NULL,
  `director` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`id_centro_educativo`),
  UNIQUE KEY `cif` (`cif`)
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `centros_educativos`
--

LOCK TABLES `centros_educativos` WRITE;
/*!40000 ALTER TABLE `centros_educativos` DISABLE KEYS */;
INSERT INTO `centros_educativos` VALUES (1,'Centro de Innovación Educativa','Calle Aprendizaje, 789','CIF11223344','123789456','456123789','46002','Sara López'),(2,'IES Tecnológico Valencia','Avenida del Puerto, 54','CIF22334455','963421876','963421877','46024','María Sánchez'),(3,'Colegio Innovación Digital','Calle San Vicente, 120','CIF33445566','962135498','962135499','46007','Pedro Martínez'),(4,'IES Ciencias','Plaza Mayor, 8','CIF44556677','961248569','961248570','46001','Teresa Ruiz'),(5,'Centro de Formación Profesional Técnica','Calle Colón, 80','CIF55667788','963587412','963587413','46004','Antonio López'),(6,'Escuela de Artes y Oficios','Avenida Blasco Ibáñez, 15','CIF66778899','964258741','964258742','46010','Carmen Pérez'),(7,'IES María Moliner','Calle Poeta Querol, 45','CIF77889900','965214783','965214784','46002','Francisco Gómez'),(8,'Centro de Estudios Superiores','Gran Vía Marqués del Turia, 24','CIF88990011','966325874','966325875','46005','Isabel Torres'),(9,'Escuela Técnica Superior','Paseo de la Alameda, 35','CIF99001122','967412563','967412564','46023','Miguel Ángel García'),(10,'Academia de Formación Continua','Calle Ruzafa, 28','CIF10101010','968523698','968523699','46006','Laura Fernández'),(11,'IES HENRI MATISSE','Carrer Enric Valor, s/n, 46980 Paterna, Valencia','Q4668001A','961206280','Sin Fax','46980','Samuel');
/*!40000 ALTER TABLE `centros_educativos` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `centros_trabajo`
--

DROP TABLE IF EXISTS `centros_trabajo`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `centros_trabajo` (
  `id_centro_trabajo` int NOT NULL AUTO_INCREMENT,
  `direccion` varchar(255) DEFAULT NULL,
  `telefono` varchar(20) DEFAULT NULL,
  `id_empresa` int DEFAULT NULL,
  PRIMARY KEY (`id_centro_trabajo`),
  KEY `id_empresa` (`id_empresa`),
  CONSTRAINT `centros_trabajo_ibfk_1` FOREIGN KEY (`id_empresa`) REFERENCES `empresas` (`id_empresa`)
) ENGINE=InnoDB AUTO_INCREMENT=46 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `centros_trabajo`
--

LOCK TABLES `centros_trabajo` WRITE;
/*!40000 ALTER TABLE `centros_trabajo` DISABLE KEYS */;
INSERT INTO `centros_trabajo` VALUES (1,'Calle Innovación, 123','123456789',1),(2,'Avenida Sabiduría, 456','987654321',2),(3,'Calle del Mar, 15','963258741',3),(4,'Avenida Reino de Valencia, 25','964369852',4),(5,'Plaza del Ayuntamiento, 10','965478963',5),(6,'Calle Sorní, 18','966589074',6),(7,'Calle Cirilo Amorós, 42','966589076',6),(8,'Avenida de Aragón, 30','967690185',7),(9,'Calle Pintor Sorolla, 22','968701296',8),(10,'Gran Vía Fernando el Católico, 45','969812307',9),(11,'Avenida Blasco Ibáñez, 125','969812309',9),(12,'Avenida del Cid, 60','970923418',10),(13,'Calle Jorge Juan, 35','971034529',11),(14,'Avenida Pérez Galdós, 85','972145630',12),(15,'Calle Doctor Manuel Candela, 41','973256741',13),(16,'Avenida Maestro Rodrigo, 95','974367852',14),(17,'Calle Burriana, 28','975478963',15),(18,'Avenida de Francia, 55','976589074',16),(19,'Calle Císcar, 65','977690185',17),(20,'Avenida del Puerto, 189','978701296',18),(21,'CentroTrabajoPrueba','',20),(22,'cawdafafa','',24),(23,'pruebaCentroPro','',22),(24,'PreubaCentroMEga','6245742457',21),(25,'Centro Arquitecto 43','896323567',19),(26,'Calle superProfesional3','435346363',23),(27,'Calle super mega pro','323423427',25),(28,'CentroMeganUevo','234243324',26),(29,'efdafdawfafaw','324234253',27),(30,'CentroNuevo','234235234',27),(31,'CentroTrabajoExtremo','234235235',28),(43,'CentroProfesional','675456786',40),(44,'Wdadwasd','234234235',NULL),(45,'wadsfsafasdfas','323423425',NULL);
/*!40000 ALTER TABLE `centros_trabajo` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `documentos`
--

DROP TABLE IF EXISTS `documentos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `documentos` (
  `id_documento` int NOT NULL AUTO_INCREMENT,
  `numero_concierto` varchar(50) DEFAULT NULL,
  `fecha_firma` date DEFAULT NULL,
  `id_centro_educativo` int DEFAULT NULL,
  `id_empresa` int DEFAULT NULL,
  `id_usuario` int DEFAULT NULL,
  `ruta` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`id_documento`),
  UNIQUE KEY `numero_concierto` (`numero_concierto`),
  KEY `id_centro_educativo` (`id_centro_educativo`),
  KEY `id_empresa` (`id_empresa`),
  KEY `id_usuario` (`id_usuario`),
  CONSTRAINT `documentos_ibfk_1` FOREIGN KEY (`id_centro_educativo`) REFERENCES `centros_educativos` (`id_centro_educativo`),
  CONSTRAINT `documentos_ibfk_2` FOREIGN KEY (`id_empresa`) REFERENCES `empresas` (`id_empresa`),
  CONSTRAINT `documentos_ibfk_4` FOREIGN KEY (`id_usuario`) REFERENCES `usuarios` (`id_usuario`)
) ENGINE=InnoDB AUTO_INCREMENT=71 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `documentos`
--

LOCK TABLES `documentos` WRITE;
/*!40000 ALTER TABLE `documentos` DISABLE KEYS */;
INSERT INTO `documentos` VALUES (1,'CON12345','2025-03-01',1,1,40,NULL),(2,'CON67890','2025-03-05',1,2,40,''),(3,'CON11111','2025-03-10',1,3,40,''),(4,'CON22222','2025-03-12',1,4,4,''),(5,'CON33333','2025-03-15',2,5,5,''),(6,'CON44444','2023-03-18',4,6,6,NULL),(7,'CON55555','2025-03-20',2,7,7,NULL),(8,'CON66666','2025-03-22',1,8,8,NULL),(9,'CON77777','2023-03-25',5,9,9,''),(10,'CON88888','2025-03-28',2,10,10,NULL),(11,'CON99999','2023-04-01',1,11,11,NULL),(12,'CON10101','2025-04-03',10,12,12,NULL),(13,'CON11112','2025-04-05',2,13,13,NULL),(14,'CON12121','2025-04-08',1,14,14,NULL),(15,'CON13131','2025-04-10',3,15,15,NULL),(16,'CON14141','2025-04-12',2,16,16,NULL),(17,'CON15151','2025-04-15',1,17,17,NULL),(18,'CON16161','2025-04-18',6,18,18,NULL),(19,'CON17171','2025-04-20',2,3,19,NULL),(20,'CON18181','2025-04-22',1,4,20,NULL),(21,'CON19191','2025-04-25',9,5,21,NULL),(22,'CON20202','2025-04-28',2,6,22,NULL),(23,'CON21212','2025-05-01',1,7,23,NULL),(24,'CON22222B','2025-05-03',2,8,24,NULL),(25,'CON23232','2025-05-05',2,9,25,NULL),(26,'CON24242','2025-05-08',1,10,26,NULL),(27,'CON25252','2025-05-10',5,11,27,NULL),(28,'CON26262','2025-05-12',2,12,28,NULL),(29,'CON27272','2025-05-15',7,13,40,NULL),(30,'CON28282','2025-05-18',8,14,30,NULL),(31,'CON29292','2025-05-20',9,15,40,NULL),(32,'CON30303','2024-05-22',1,16,2,NULL),(33,'CON31313','2025-05-25',1,17,3,NULL),(34,'CON32323','2024-05-28',2,18,4,NULL),(35,'CON33333B','2025-06-01',1,3,5,NULL),(36,'CON34343','2023-06-03',4,4,6,NULL),(37,'CON35353','2025-06-05',2,5,7,NULL),(38,'CON36363','2023-06-08',1,6,8,NULL),(39,'CON37373','2023-06-10',7,7,9,NULL),(40,'CON38383','2025-06-12',2,8,10,NULL),(41,'CON39393','2025-06-15',1,9,11,NULL),(42,'CON40404','2025-06-18',10,10,12,NULL),(43,'CON41414','2025-06-20',2,11,13,NULL),(44,'CON42424','2024-06-22',1,12,14,NULL),(45,'CON43434','2023-06-25',3,13,15,NULL),(46,'CON44444B','2025-06-28',4,14,40,NULL),(47,'CON45454','2024-07-01',1,15,2,NULL),(48,'CON46464','2025-07-03',6,16,3,NULL),(51,'C12351','2025-05-16',6,6,40,''),(52,'C1231CACAC','2024-05-22',5,4,40,''),(53,'Prueba12','2025-05-21',4,6,40,''),(54,'PruebaActualizar','2023-05-22',7,6,40,''),(55,'Prueba4','2023-06-06',4,6,40,''),(56,'prueba1','2023-06-03',4,4,40,''),(57,'PruebaNueva','2023-05-25',3,7,40,''),(59,'PruebaFInalFinal','2025-05-29',3,4,40,''),(66,'EmpresaFinal','2024-07-12',3,4,40,''),(67,'DocumentoNuevoLucia','2024-06-13',4,4,40,''),(68,'DocumentoTutoriial','2025-06-13',11,40,40,'');
/*!40000 ALTER TABLE `documentos` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `empresas`
--

DROP TABLE IF EXISTS `empresas`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `empresas` (
  `id_empresa` int NOT NULL AUTO_INCREMENT,
  `razon_social` varchar(255) DEFAULT NULL,
  `cif` varchar(20) DEFAULT NULL,
  `direccion` varchar(255) DEFAULT NULL,
  `telefono` varchar(20) DEFAULT NULL,
  `localidad` varchar(100) DEFAULT NULL,
  `provincia` varchar(100) DEFAULT NULL,
  `fax` varchar(20) DEFAULT NULL,
  `codigo_postal` varchar(10) DEFAULT NULL,
  `sector` varchar(100) DEFAULT NULL,
  `id_responsable` int DEFAULT NULL,
  PRIMARY KEY (`id_empresa`),
  UNIQUE KEY `cif` (`cif`),
  UNIQUE KEY `id_responsable` (`id_responsable`),
  CONSTRAINT `id_responsable` FOREIGN KEY (`id_responsable`) REFERENCES `responsables` (`id_responsable`)
) ENGINE=InnoDB AUTO_INCREMENT=42 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `empresas`
--

LOCK TABLES `empresas` WRITE;
/*!40000 ALTER TABLE `empresas` DISABLE KEYS */;
INSERT INTO `empresas` VALUES (1,'Tech Solutions S.L.','CIF12345678','Calle Innovación, 123','123456789','Valencia','Valencia','987654321','46001','Tecnología',1),(2,'Educativa Plus S.A.','CIF87654321','Avenida Sabiduría, 456','987654321','Madrid','Madrid',NULL,'28001','Educación',2),(3,'Innovación Digital SL','CIF23456789','Calle del Mar, 15','963258741','Valencia','Valencia','963258742','46003','Informática',3),(4,'Educación Superior SA','CIF34567890','Avenida Reino de Valencia, 25','964369852','Valencia','Valencia','964369853','46005','Educación',4),(5,'Formación Profesional SL','CIF45678901','Plaza del Ayuntamiento, 10','965478963','Valencia','Valencia','965478964','46002','Formación',5),(6,'Desarrollo Web SL','CIF56789012','Calle Sorní, 18','966589074','Valencia','Valencia','966589075','46004','Desarrollo Software',6),(7,'Consultores Educativos SA','CIF67890123','Avenida de Aragón, 30','967690185','Valencia','Valencia','967690186','46021','Consultoría',7),(8,'Tecnologías Aplicadas SL','CIF78901234','Calle Pintor Sorolla, 22','968701296','Valencia','Valencia','968701297','46002','Tecnología',8),(9,'Innovación Educativa SA','CIF89012345','Gran Vía Fernando el Católico, 45','969812307','Valencia','Valencia','969812308','46008','Innovación',9),(10,'Soluciones Formativas SL','CIF90123456','Avenida del Cid, 60','970923418','Valencia','Valencia','970923419','46018','Formación',10),(11,'Recursos Educativos SA','CIF01234567','Calle Jorge Juan, 35','971034529','Valencia','Valencia','971034530','46004','Educación',11),(12,'TecnoEducación SL','CIF12345670','Avenida Pérez Galdós, 85','972145630','Valencia','Valencia','972145631','46008','Tecnología Educativa',12),(13,'Formación Continua SA','CIF23456701','Calle Doctor Manuel Candela, 41','973256741','Valencia','Valencia','973256742','46021','Formación',13),(14,'EdTech Solutions SL','CIF34567012','Avenida Maestro Rodrigo, 95','974367852','Valencia','Valencia','974367853','46015','Tecnología Educativa',14),(15,'Campus Digital SA','CIF45670123','Calle Burriana, 28','975478963','Valencia','Valencia','975478964','46005','Educación Digital',15),(16,'Formación Avanzada SL','CIF56701234','Avenida de Francia, 55','976589074','Valencia','Valencia','976589075','46023','Formación',16),(17,'Innova Educación SA','CIF67012345','Calle Císcar, 65','977690185','Valencia','Valencia','977690186','46005','Innovación Educativa',17),(18,'TechLearn SL','CIF78123456','Avenida del Puerto, 189','978701296','Valencia','Valencia','978701297','46022','Aprendizaje Tecnológico',18),(19,'SamuelEmpresaTech','12532342','Calle matisse n6','678423567','Paterna','Valencia',NULL,'56435','TecnologiasPro',22),(20,'EmpresaNuevaTech','12345','Calle empresa nueva 46','32523424','fesfsg','sefsf',NULL,'43534','Si',21),(21,'EmpresaNuevaProfesional2','1231241','Calle profesional','352342','Paterna','Valencia',NULL,'34532','TecnologiasPro',19),(22,'Empresa si','134234532','Calle si n 6','3242342','fafw','adwaf',NULL,'3223','TecnologiasPro',23),(23,'Empresa super Pro mega','342342','Calle super pro mega','3242342','Valencia','Valencia',NULL,'435354','Formación',20),(24,'EmpresaNuevawadadad','CIF23435446','Calle la maldad, 435','23423','Paterna','Valencia',NULL,'4353','fsfef',24),(25,'EmpresasMartinez','3241231','afawdad','234234','fafsef','fefeafaw',NULL,'453453','Si',25),(26,'Google 2','CIF2342123','calle google 2','53234253','Valencia','Valencia',NULL,'2342','TecnologiasPro',26),(27,'TecnologiasHibernate','1234234','Calle hibernate n6','323564345','Valencia','Valencia',NULL,'46764','Hibernate',30),(28,'DocumentoPrueb','1231231','afwaf','232342','sfsg','rdgdg',NULL,'23424','Hibernate',27),(29,'Empresa rivera','CIF23423546','Calle rivera 98','23423','Valencia','Valencia',NULL,'32342','Hibernate',31),(31,'Gibraltar empresas','CIF34235231','calle numero 56','342532','fawfaf','wfafa',NULL,'34234','Informática',29),(40,'NuevoDocumentoTutorial','3242342','Calle super tecnologico','5423423467','Valencia','Valencia',NULL,'45678','Informática',41);
/*!40000 ALTER TABLE `empresas` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `permisos`
--

DROP TABLE IF EXISTS `permisos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `permisos` (
  `id_permiso` int NOT NULL AUTO_INCREMENT,
  `codigo` varchar(45) DEFAULT NULL,
  `descripcion` text,
  PRIMARY KEY (`id_permiso`),
  UNIQUE KEY `codigo_UNIQUE` (`codigo`)
) ENGINE=InnoDB AUTO_INCREMENT=24 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `permisos`
--

LOCK TABLES `permisos` WRITE;
/*!40000 ALTER TABLE `permisos` DISABLE KEYS */;
INSERT INTO `permisos` VALUES (1,'DOC_CREATE','Aniadir documentos'),(2,'DOC_DELETE_OWN','Borrar documento propio'),(3,'CENTER_DATA','Datos del centro'),(4,'PASSWORD_CHANGE_OWN','Cambiar contrasenia'),(5,'USER_MANAGE','Alta y modificacion de usuarios'),(6,'REPORTS_CREATE','Realizar informes'),(7,'CHARTS_VIEW','Visualizar graficos'),(8,'PASSWORD_CHANGE_ALL','Modificar contrasenia de otro'),(9,'ROLES_MANAGE','Gestion de roles y permisos'),(10,'IMPORT_OPS','Operaciones de importacion'),(11,'RESPONSABLE_MANAGE','Aniadir o modificar responsables'),(12,'DOC_DELETE_ALL','Borrar cualquier documento'),(13,'DOC_ASSIGN_CENTER','Asignar cualquier centro en el documento'),(14,'DOC_EDIT_ALL','Editar cualquier documento'),(15,'DOC_EDIT_OWN','Editar documento propio'),(16,'CREATE_JOB_CENTER','Crear centro de trabajo');
/*!40000 ALTER TABLE `permisos` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `responsables`
--

DROP TABLE IF EXISTS `responsables`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `responsables` (
  `id_responsable` int NOT NULL AUTO_INCREMENT,
  `nombre` varchar(100) DEFAULT NULL,
  `apellidos` varchar(100) DEFAULT NULL,
  `dni` varchar(20) DEFAULT NULL,
  `correo` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`id_responsable`),
  UNIQUE KEY `dni` (`dni`)
) ENGINE=InnoDB AUTO_INCREMENT=44 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `responsables`
--

LOCK TABLES `responsables` WRITE;
/*!40000 ALTER TABLE `responsables` DISABLE KEYS */;
INSERT INTO `responsables` VALUES (1,'Juan','Pérez','DNI13579246C','juan.perez@example.com'),(2,'Ana','Gómez','DNI24681357D','ana.gomez@example.com'),(3,'María','Rogue','DNI35791246E','maria.rodriguez@example.com'),(4,'Pedro','Fernández','DNI46802357F','pedro.fernandez@example.com'),(5,'Carmen','Martínez','DNI57913468G','carmen.martinez@example.com'),(6,'José','López','DNI68024579H','jose.lopez@example.com'),(7,'Laura','García','DNI79135680I','laura.garcia@example.com'),(8,'Francisco','Torres','DNI80246791J','francisco.torres@example.com'),(9,'Isabel','Sánchez','DNI91357802K','isabel.sanchez@example.com'),(10,'Miguel','Navarro','DNI02468913L','miguel.navarro@example.com'),(11,'Sara','Ruiz','DNI13579024M','sara.ruiz@example.com'),(12,'David','Moreno','DNI24680135N','david.moreno@example.com'),(13,'Elena','Serrano','DNI35791246O','elena.serrano@example.com'),(14,'Antonio','Molina','DNI46802357P','antonio.molina@example.com'),(15,'Marta','Vega','DNI57913468Q','marta.vega@example.com'),(16,'Javier','Castro','DNI68024579R','javier.castro@example.com'),(17,'Lucía','Ortega','DNI79135680S','lucia.ortega@example.com'),(18,'Carlos','Flores','DNI80246791T','carlos.flores@example.com'),(19,'PruebaRes','pruebaResponsable','pruebaResponsable','pruebaResponsable'),(20,'Jaimito','Perez Perez','325753478V','jaimito.perez@example.com'),(21,'Pablito','Gomez','8797689347D','pablito.gomez@example.com'),(22,'Marquitos','Jimenez','2342532425K','jaimitoprofesional@gmail.com'),(23,'Peptio','qwfafaw','234235235y','wdafafawda'),(24,'Kiko','Kikoto','234234242','ajfajfoadadw'),(25,'Laurentino','adaf','23423424','fafawda'),(26,'fadawd','sfafwa','afwfda','fawfa'),(27,'Kike','Kikoto','DNI12312412','fafafa@fafaf'),(29,'Samuel','Radu','DNI1234565','samuel.radu@example.com'),(30,'Esther','Si','DNI123531','esther.si@example.com'),(31,'Manuelito','Gomez','DNI1231241','manuelitogomez@gmail.com'),(41,'Carlitos','Perez','X7823456775','carlos.perez@example.com');
/*!40000 ALTER TABLE `responsables` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `roles`
--

DROP TABLE IF EXISTS `roles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `roles` (
  `id_rol` int NOT NULL AUTO_INCREMENT,
  `nombre_rol` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`id_rol`),
  UNIQUE KEY `nombre_rol` (`nombre_rol`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `roles`
--

LOCK TABLES `roles` WRITE;
/*!40000 ALTER TABLE `roles` DISABLE KEYS */;
INSERT INTO `roles` VALUES (1,'Administrador'),(2,'Profesor');
/*!40000 ALTER TABLE `roles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `roles_permisos`
--

DROP TABLE IF EXISTS `roles_permisos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `roles_permisos` (
  `id_rol` int NOT NULL,
  `id_permiso` int NOT NULL,
  PRIMARY KEY (`id_rol`,`id_permiso`),
  KEY `id_permiso` (`id_permiso`),
  CONSTRAINT `roles_permisos_ibfk_1` FOREIGN KEY (`id_rol`) REFERENCES `roles` (`id_rol`) ON DELETE CASCADE,
  CONSTRAINT `roles_permisos_ibfk_2` FOREIGN KEY (`id_permiso`) REFERENCES `permisos` (`id_permiso`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `roles_permisos`
--

LOCK TABLES `roles_permisos` WRITE;
/*!40000 ALTER TABLE `roles_permisos` DISABLE KEYS */;
INSERT INTO `roles_permisos` VALUES (1,1),(2,1),(1,2),(2,2),(1,3),(1,4),(2,4),(1,5),(1,6),(1,7),(1,8),(1,9),(1,10),(1,11),(1,12),(1,13),(1,14),(1,15),(2,15),(1,16);
/*!40000 ALTER TABLE `roles_permisos` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `usuarios`
--

DROP TABLE IF EXISTS `usuarios`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `usuarios` (
  `id_usuario` int NOT NULL AUTO_INCREMENT,
  `nombre` varchar(100) DEFAULT NULL,
  `apellidos` varchar(100) DEFAULT NULL,
  `dni` varchar(20) DEFAULT NULL,
  `correo` varchar(100) DEFAULT NULL,
  `contrasenia` varchar(255) DEFAULT NULL,
  `fecha_alta` date DEFAULT NULL,
  `fecha_baja` date DEFAULT NULL,
  `observaciones` text,
  `id_rol` int DEFAULT NULL,
  `id_centro_educativo` int DEFAULT NULL,
  PRIMARY KEY (`id_usuario`),
  UNIQUE KEY `dni` (`dni`),
  UNIQUE KEY `correo` (`correo`),
  KEY `id_rol` (`id_rol`),
  KEY `fk_usuario_centro` (`id_centro_educativo`),
  CONSTRAINT `fk_usuario_centro` FOREIGN KEY (`id_centro_educativo`) REFERENCES `centros_educativos` (`id_centro_educativo`),
  CONSTRAINT `usuarios_ibfk_1` FOREIGN KEY (`id_rol`) REFERENCES `roles` (`id_rol`)
) ENGINE=InnoDB AUTO_INCREMENT=42 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `usuarios`
--

LOCK TABLES `usuarios` WRITE;
/*!40000 ALTER TABLE `usuarios` DISABLE KEYS */;
INSERT INTO `usuarios` VALUES (2,'Lucía','Martínez','DNI87654321B','lucia.martinez@example.com','123456','2025-05-30',NULL,'Ninguna observación',2,1),(3,'Antonio','Sánchez','DNI23456789C','antonio.sanchez@example.com','123456','2025-05-30',NULL,'Usuario administrador',1,9),(4,'Elena','Rodríguez','DNI34567890K','elena.rodriguez@example.com','123456','2025-05-30',NULL,'Profesora de informática',2,2),(5,'Pablo','Fernández','DNI45678901E','pablo.fernandez@example.com','pablopwd','2025-01-25',NULL,'Profesor de tecnología',2,1),(6,'María','López','DNI56789012F','maria.lopez@example.com','123456','2025-05-30',NULL,'Administradora del sistema',1,5),(7,'José','Martínez','DNI67890123G','jose.martinez@example.com','jose2025','2025-02-05',NULL,'Profesor de matemáticas',2,2),(8,'Laura','Pérez','DNI78901234H','laura.perez@example.com','laura456','2025-02-10',NULL,'Profesora de ciencias',2,1),(9,'Francisco','Gómez','DNI89012345I','francisco.gomez@example.com','fran789','2025-02-15',NULL,'Administrador de la plataforma',1,10),(10,'Ana','Torres','DNI90123456J','ana.torres@example.com','ana2025','2025-02-20',NULL,'Profesora de lengua',2,2),(11,'Miguel','Ramírez','DNI01234567K','miguel.ramirez@example.com','miguel123','2025-02-25',NULL,'Profesor de historia',2,1),(12,'Sara','Hernández','DNI12345670L','sara.hernandez@example.com','sara456','2025-03-01',NULL,'Administradora de contenidos',1,9),(13,'David','Díaz','DNI23456701M','david.diaz@example.com','david789','2025-03-05',NULL,'Profesor de física',2,2),(14,'Carmen','Moreno','DNI34567012N','carmen.moreno@example.com','carmen123','2025-03-10',NULL,'Profesora de química',2,1),(15,'Javier','Álvarez','DNI45670123O','javier.alvarez@example.com','javier456','2025-03-15',NULL,'Administrador técnico',1,8),(16,'Lucía','Muñoz','DNI56701234P','lucia.munoz@example.com','lucia789','2025-03-20',NULL,'Profesora de biología',2,2),(17,'Carlos','Gutiérrez','DNI67012345Q','carlos.gutierrez@example.com','carlos123','2025-03-25',NULL,'Profesor de economía',2,1),(18,'Isabel','Navarro','DNI78123456R','isabel.navarro@example.com','isabel456','2025-03-30',NULL,'Administradora de sistemas',1,6),(19,'Manuel','Cruz','DNI89234567S','manuel.cruz@example.com','manuel789','2025-04-01',NULL,'Profesor de filosofía',2,2),(20,'Teresa','Romero','DNI90345678T','teresa.romero@example.com','teresa123','2025-04-05',NULL,'Profesora de arte',2,1),(21,'Roberto','Gil','DNI01456789U','roberto.gil@example.com','roberto456','2025-04-10',NULL,'Administrador de red',1,7),(22,'Patricia','Serrano','DNI12567890V','patricia.serrano@example.com','patricia789','2025-04-15',NULL,'Profesora de música',2,2),(23,'Fernando','Jiménez','DNI23678901W','fernando.jimenez@example.com','fernando123','2025-04-20',NULL,'Profesor de educación física',2,1),(24,'Cristina','Ruiz','DNI34789012X','cristina.ruiz@example.com','cristina456','2025-04-25',NULL,'Administradora de base de datos',1,4),(25,'Alberto','Castro','DNI45890123Y','alberto.castro@example.com','alberto789','2025-04-30',NULL,'Profesor de programación',2,2),(26,'Raquel','Ortega','DNI56901234Z','raquel.ortega@example.com','raquel123','2025-05-01',NULL,'Profesora de diseño',2,1),(27,'Sergio','Molina','DNI67012345AA','sergio.molina@example.com','sergio456','2025-05-05',NULL,'Administrador de seguridad',1,5),(28,'Natalia','Delgado','DNI78123456BB','natalia.delgado@example.com','natalia789','2025-05-10','2025-06-08','Profesora de inglés',2,2),(30,'Alicia','Flores','DNI90345678DD','alicia.flores@example.com','alicia456','2025-05-20',NULL,'Administradora de proyectos',1,4),(34,'Raimito','Perez Perez','DNI12345678N','raimito.perez@example.com','123456','2025-05-30','2025-06-03',NULL,2,3),(40,'Carlos','Garcia','DNI12345436A','carlos.garcia@example.com','123456','2025-05-20',NULL,'Pro',1,11);
/*!40000 ALTER TABLE `usuarios` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-06-15 11:29:26

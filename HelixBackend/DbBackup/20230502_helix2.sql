-- MySQL dump 10.13  Distrib 8.0.30, for Win64 (x86_64)
--
-- Host: localhost    Database: helix
-- ------------------------------------------------------
-- Server version	8.0.30

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
-- Table structure for table `account`
--

DROP TABLE IF EXISTS `account`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `account` (
  `uuid` varchar(36) NOT NULL,
  `user` varchar(128) NOT NULL,
  `communication_medium_uuid` varchar(36) NOT NULL,
  `creation_datetime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `active` tinyint NOT NULL DEFAULT '1',
  `activation_datetime` datetime DEFAULT NULL,
  `deleted` tinyint DEFAULT '0',
  `deletion_datetime` datetime DEFAULT NULL,
  `changed_datetime` datetime DEFAULT NULL,
  PRIMARY KEY (`user`,`communication_medium_uuid`),
  KEY `uuid_Idx` (`uuid`),
  KEY `fk_accountToCommMedium_idx` (`communication_medium_uuid`),
  CONSTRAINT `fk_accountToCommMedium` FOREIGN KEY (`communication_medium_uuid`) REFERENCES `communication_medium` (`uuid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `account`
--

LOCK TABLES `account` WRITE;
/*!40000 ALTER TABLE `account` DISABLE KEYS */;
INSERT INTO `account` VALUES ('1292baca-cef4-11ec-83a3-7085c294413b','0x00405a00','199b28e3-d921-11eb-81f0-842afd097283','2022-05-08 17:27:02',1,NULL,0,NULL,NULL),('d0eb95d2-3425-11ec-b696-d8bbc10f2ae0','mika-roos@web.de','199b28e3-d921-11eb-81f0-842afd097283','2021-10-23 17:22:36',1,NULL,0,NULL,NULL),('edf04962-1ca5-11ec-a4a4-d8bbc10f2ae0','root','65980434-1ca4-11ec-a4a4-d8bbc10f2ae0','2021-09-23 19:39:12',1,NULL,0,NULL,NULL),('8aaa84d0-e245-11eb-9dd4-309c2364fdb6','service@helixdb.org','6ad9523e-d91a-11eb-81f0-842afd097283','2021-07-11 12:43:07',1,NULL,0,NULL,NULL),('98caf4cf-3418-11ec-b696-d8bbc10f2ae0','Strato Mail','6ad9523e-d91a-11eb-81f0-842afd097283','2021-10-23 15:47:58',1,NULL,0,NULL,NULL),('dcd8ec4e-bfa2-11eb-a11f-309c2364fdb6','test2@helixdb.org','199b28e3-d921-11eb-81f0-842afd097283','2021-05-28 10:52:57',1,NULL,0,NULL,'2021-10-16 12:36:53'),('322c1723-dbf0-11eb-8cd9-842afd097283','test3@helixdb.org','199b28e3-d921-11eb-81f0-842afd097283','2021-07-03 11:17:04',1,NULL,0,NULL,'2021-10-16 12:36:53'),('dcd8ac39-bfa2-11eb-a11f-309c2364fdb6','test@helixdb.org','199b28e3-d921-11eb-81f0-842afd097283','2021-05-28 10:52:57',1,NULL,0,NULL,'2021-10-16 12:36:53');
/*!40000 ALTER TABLE `account` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `account_BEFORE_INSERT` BEFORE INSERT ON `account` FOR EACH ROW BEGIN

SET new.creation_datetime = NOW();   
END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `account_BEFORE_UPDATE` BEFORE UPDATE ON `account` FOR EACH ROW BEGIN

SET new.changed_datetime = NOW();
END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `communication_medium`
--

DROP TABLE IF EXISTS `communication_medium`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `communication_medium` (
  `uuid` varchar(36) NOT NULL,
  `name` varchar(128) NOT NULL,
  `creation_datetime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `active` tinyint NOT NULL DEFAULT '1',
  `activation_datetime` datetime DEFAULT NULL,
  `deleted` tinyint DEFAULT '0',
  `deletion_datetime` datetime DEFAULT NULL,
  `changed_datetime` datetime DEFAULT NULL,
  PRIMARY KEY (`uuid`),
  KEY `name_Idx` (`name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `communication_medium`
--

LOCK TABLES `communication_medium` WRITE;
/*!40000 ALTER TABLE `communication_medium` DISABLE KEYS */;
INSERT INTO `communication_medium` VALUES ('199b28e3-d921-11eb-81f0-842afd097283','Front End User','2021-06-29 21:29:35',1,NULL,0,NULL,'2021-10-16 12:36:49'),('258b6f62-d91d-11eb-81f0-842afd097283','External SMTP Service','2021-06-29 21:01:17',1,NULL,0,NULL,'2021-06-29 21:29:35'),('258b88a9-d91d-11eb-81f0-842afd097283','External POP3 Service','2021-06-29 21:01:17',1,NULL,0,NULL,'2021-06-29 21:29:35'),('65980434-1ca4-11ec-a4a4-d8bbc10f2ae0','Localhost','2021-09-23 19:28:45',1,NULL,0,NULL,NULL),('6ad81c21-d91a-11eb-81f0-842afd097283','Discord','2021-06-29 20:41:44',1,NULL,0,NULL,NULL),('6ad9523e-d91a-11eb-81f0-842afd097283','External IMAP Service','2021-06-29 20:41:44',1,NULL,0,NULL,'2021-06-29 21:29:35'),('6ad96e3c-d91a-11eb-81f0-842afd097283','Signal','2021-06-29 20:41:44',1,NULL,0,NULL,NULL),('6ad98075-d91a-11eb-81f0-842afd097283','Telegramm','2021-06-29 20:41:44',1,NULL,0,NULL,NULL),('6ad993d8-d91a-11eb-81f0-842afd097283','WhatsApp Business','2021-06-29 20:41:44',1,NULL,0,NULL,NULL);
/*!40000 ALTER TABLE `communication_medium` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `system_message_user_type_BEFORE_INSERT` BEFORE INSERT ON `communication_medium` FOR EACH ROW BEGIN
SET new.creation_datetime = NOW();   
END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `system_message_user_type_BEFORE_UPDATE` BEFORE UPDATE ON `communication_medium` FOR EACH ROW BEGIN
SET new.changed_datetime = NOW();
END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `connection_type`
--

DROP TABLE IF EXISTS `connection_type`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `connection_type` (
  `uuid` varchar(36) NOT NULL,
  `name` varchar(45) NOT NULL,
  `description` varchar(256) DEFAULT NULL,
  `creation_datetime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `active` tinyint NOT NULL DEFAULT '1',
  `activation_datetime` datetime DEFAULT NULL,
  `deleted` tinyint DEFAULT '0',
  `deletion_datetime` datetime DEFAULT NULL,
  `changed_datetime` datetime DEFAULT NULL,
  PRIMARY KEY (`uuid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `connection_type`
--

LOCK TABLES `connection_type` WRITE;
/*!40000 ALTER TABLE `connection_type` DISABLE KEYS */;
INSERT INTO `connection_type` VALUES ('989ae37c-a5b4-11eb-bac0-309c2364fdb6','Standard TCP/IP','Connects directly to Serverport, Encryption managed by Server eg. SSL Cert.','2021-04-25 10:54:23',1,NULL,0,NULL,'2021-05-27 11:48:28'),('989b33d8-a5b4-11eb-bac0-309c2364fdb6','TCP/IP over SSH','Connects to Server via Secure Shell (for encryption) and then to local SQL Server on the connected Hostsystem.','2021-04-25 10:54:23',1,NULL,0,NULL,'2021-05-27 11:48:28'),('989b3c0a-a5b4-11eb-bac0-309c2364fdb6','SSL/TLS','Connects like Standard TCP/IP but encrypted the connection via SSL/TLS (Certificate).','2021-04-25 10:54:23',1,NULL,0,NULL,'2021-05-27 11:48:28'),('989b5015-a5b4-11eb-bac0-309c2364fdb6','File Database','Retrieve Data from Filedatabases e.g. MS Access, MS Excel etc.','2021-04-25 10:54:23',1,NULL,0,NULL,'2021-05-27 11:48:28'),('989b5936-a5b4-11eb-bac0-309c2364fdb6','Proxy','Connects to Databaseserver via Proxy','2021-04-25 10:54:23',1,NULL,0,NULL,'2021-05-27 11:48:28');
/*!40000 ALTER TABLE `connection_type` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`rest`@`%`*/ /*!50003 TRIGGER `connection_type_BEFORE_INSERT` BEFORE INSERT ON `connection_type` FOR EACH ROW BEGIN 	SET new.creation_datetime = NOW();   END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`rest`@`%`*/ /*!50003 TRIGGER `connection_type_BEFORE_UPDATE` BEFORE UPDATE ON `connection_type` FOR EACH ROW BEGIN 	SET new.changed_datetime = NOW();END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `connectionstring_values`
--

DROP TABLE IF EXISTS `connectionstring_values`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `connectionstring_values` (
  `uuid` varchar(36) NOT NULL,
  `value_name` varchar(96) DEFAULT NULL,
  `type` varchar(45) DEFAULT NULL,
  `description` varchar(256) DEFAULT NULL,
  `mandatory` tinyint NOT NULL COMMENT 'Pflichtfeld: Ob Value pflciht im Connectionstring ist',
  `creation_datetime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `active` tinyint NOT NULL DEFAULT '1',
  `activation_datetime` datetime DEFAULT NULL,
  `deleted` tinyint DEFAULT '0',
  `deletion_datetime` datetime DEFAULT NULL,
  `changed_datetime` datetime DEFAULT NULL,
  PRIMARY KEY (`uuid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `connectionstring_values`
--

LOCK TABLES `connectionstring_values` WRITE;
/*!40000 ALTER TABLE `connectionstring_values` DISABLE KEYS */;
INSERT INTO `connectionstring_values` VALUES ('eda913fd-a5b4-11eb-bac0-309c2364fdb6','DRIVER','string','Drivername',1,'2021-04-25 10:56:46',1,NULL,0,NULL,'2021-05-27 11:48:28'),('eda97394-a5b4-11eb-bac0-309c2364fdb6','SERVER','ip','Server Ip ',1,'2021-04-25 10:56:46',1,NULL,0,NULL,'2021-05-27 11:48:28'),('eda9762d-a5b4-11eb-bac0-309c2364fdb6','PORT','uint','Server Port',1,'2021-04-25 10:56:46',1,NULL,0,NULL,'2021-05-27 11:48:28'),('eda97726-a5b4-11eb-bac0-309c2364fdb6','DATABASE','string','Databasename',1,'2021-04-25 10:56:46',1,NULL,0,NULL,'2021-05-27 11:48:28'),('eda977ff-a5b4-11eb-bac0-309c2364fdb6','UID','string','User Id/Username',1,'2021-04-25 10:56:46',1,NULL,0,NULL,'2021-05-27 11:48:28'),('eda978c6-a5b4-11eb-bac0-309c2364fdb6','PASSWORD','string','User Password',1,'2021-04-25 10:56:46',1,NULL,0,NULL,'2021-05-27 11:48:28'),('eda9798e-a5b4-11eb-bac0-309c2364fdb6','OPTION','int','Specific Options',0,'2021-04-25 10:56:46',1,NULL,0,NULL,'2021-05-27 11:48:28'),('eda97a54-a5b4-11eb-bac0-309c2364fdb6','DBQ','string','Database File e.g MS Excel or Access File',1,'2021-04-25 10:56:46',1,NULL,0,NULL,'2021-05-27 11:48:28'),('eda98332-a5b4-11eb-bac0-309c2364fdb6','EXTENSION','regex','File Extension depence on DBQ Value',0,'2021-04-25 10:56:46',1,NULL,0,NULL,'2021-05-27 11:48:28'),('eda99d69-a5b4-11eb-bac0-309c2364fdb6','Use SSH','bool','Boolean Value for Secure Shell Use (true = use, false = dont use)',0,'2021-04-25 10:56:46',1,NULL,0,NULL,'2021-05-27 11:48:28'),('eda99f11-a5b4-11eb-bac0-309c2364fdb6','SSH Hostname','ip','Secure Shell Hostname',0,'2021-04-25 10:56:46',1,NULL,0,NULL,'2021-05-27 11:48:28'),('eda99ff9-a5b4-11eb-bac0-309c2364fdb6','SSH Port','uint','Secure Shell Port',0,'2021-04-25 10:56:46',1,NULL,0,NULL,'2021-05-27 11:48:28'),('eda9a0cc-a5b4-11eb-bac0-309c2364fdb6','SSH Username','string','Secure Shell Username',0,'2021-04-25 10:56:46',1,NULL,0,NULL,'2021-05-27 11:48:28'),('eda9a1a8-a5b4-11eb-bac0-309c2364fdb6','SSH Password','string','Secure Shell Password',0,'2021-04-25 10:56:46',1,NULL,0,NULL,'2021-05-27 11:48:28'),('eda9a272-a5b4-11eb-bac0-309c2364fdb6','SSH Client Key','string','Secure Shell Key for Client Auth',0,'2021-04-25 10:56:46',1,NULL,0,NULL,'2021-05-27 11:48:28'),('eda9a338-a5b4-11eb-bac0-309c2364fdb6','SSH Client Key Password','string','Secure Shell Client Key Auth Passphrase',0,'2021-04-25 10:56:46',1,NULL,0,NULL,'2021-05-27 11:48:28'),('eda9a406-a5b4-11eb-bac0-309c2364fdb6','SSH Server Key','string','Secure Shell Public Key from Server',0,'2021-04-25 10:56:46',1,NULL,0,NULL,'2021-05-27 11:48:28'),('eda9a4ca-a5b4-11eb-bac0-309c2364fdb6','SSH Storage Path','string','Secure Shell Storage PATH',0,'2021-04-25 10:56:46',1,NULL,0,NULL,'2021-05-27 11:48:28');
/*!40000 ALTER TABLE `connectionstring_values` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`rest`@`%`*/ /*!50003 TRIGGER `connectionstring_values_BEFORE_INSERT` BEFORE INSERT ON `connectionstring_values` FOR EACH ROW BEGIN 	SET new.creation_datetime = NOW();   END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`rest`@`%`*/ /*!50003 TRIGGER `connectionstring_values_BEFORE_UPDATE` BEFORE UPDATE ON `connectionstring_values` FOR EACH ROW BEGIN 	SET new.changed_datetime = NOW();END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `crud`
--

DROP TABLE IF EXISTS `crud`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `crud` (
  `uuid` varchar(36) NOT NULL,
  `name` varchar(45) NOT NULL,
  `flag` int NOT NULL,
  `creation_datetime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `active` tinyint NOT NULL DEFAULT '1',
  `activation_datetime` datetime DEFAULT NULL,
  `deleted` tinyint DEFAULT '0',
  `deletion_datetime` datetime DEFAULT NULL,
  `changed_datetime` datetime DEFAULT NULL,
  PRIMARY KEY (`uuid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `crud`
--

LOCK TABLES `crud` WRITE;
/*!40000 ALTER TABLE `crud` DISABLE KEYS */;
INSERT INTO `crud` VALUES ('d22a1e89-1937-11ec-a4ca-d8bbc10f2ae0','Create',1,'2021-09-19 10:53:56',1,NULL,0,NULL,NULL),('d722ee4b-1937-11ec-a4ca-d8bbc10f2ae0','Read',2,'2021-09-19 10:53:56',1,NULL,0,NULL,NULL),('db440aef-1937-11ec-a4ca-d8bbc10f2ae0','Update',4,'2021-09-19 10:53:56',1,NULL,0,NULL,NULL),('dee44a8b-1937-11ec-a4ca-d8bbc10f2ae0','Delete',8,'2021-09-19 10:53:56',1,NULL,0,NULL,NULL);
/*!40000 ALTER TABLE `crud` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `message`
--

DROP TABLE IF EXISTS `message`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `message` (
  `uuid` varchar(36) NOT NULL,
  `message_conversation_uuid` varchar(36) NOT NULL COMMENT 'Every Message must be a member of a conversation, because every sent msg is received by  a recipient, so two participant (sender + recipient) surrender to a conversation',
  `subject` varchar(45) DEFAULT NULL,
  `content` blob COMMENT 'Message Content e.g. HTML+CSS (MIME), JSON, Plain-Text etc.',
  `checked_for_virus` tinyint NOT NULL COMMENT 'Bool für AntiVirus check, ob geprüft wurde durch nClam oder nicht',
  `checked_for_violence` tinyint NOT NULL COMMENT 'Bool für gewaltverherlichende Sprache oderracism speech',
  `checked_for_sexual_content` tinyint NOT NULL COMMENT 'Bool für Prüfung ob Inhalt/Attachment sexual ist (durch KI von Facebook Abgleichen oder trainierte TensorFlow Modelle), wenn Sexual Content == true, dann Inhalt verbieten',
  `checked_for_sexual_content_meta` json NOT NULL COMMENT 'Speichert den KI Score für das Attachment',
  `hash` varchar(48) DEFAULT NULL,
  `creation_datetime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `active` tinyint NOT NULL DEFAULT '1',
  `activation_datetime` datetime DEFAULT NULL,
  `deleted` tinyint DEFAULT '0',
  `deletion_datetime` datetime DEFAULT NULL,
  `changed_datetime` datetime DEFAULT NULL,
  PRIMARY KEY (`uuid`),
  KEY `fk_messageToConversation_idx` (`message_conversation_uuid`),
  CONSTRAINT `fk_messageToConf` FOREIGN KEY (`message_conversation_uuid`) REFERENCES `message_conversation` (`uuid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `message`
--

LOCK TABLES `message` WRITE;
/*!40000 ALTER TABLE `message` DISABLE KEYS */;
INSERT INTO `message` VALUES ('56bff630-e246-11eb-9dd4-309c2364fdb6','56bc3e8d-e246-11eb-9dd4-309c2364fdb6','ticket eröffnet',_binary 'ticket created\r\nAm Sonntag, 27. Juni 2021 18:40, Service@helixDB schrieb:\r\n> test mit attachment und hier ist der body\r\n> \r\n',0,0,0,'{\"test\": \"1\"}','56e740e1eb43b540a081f47326e4d2a4','2021-07-11 12:48:49',1,NULL,0,NULL,NULL),('67565129-e246-11eb-9dd4-309c2364fdb6','6754f03c-e246-11eb-9dd4-309c2364fdb6','ticket eröffnet',_binary 'ticket created\r\nAm Sonntag, 27. Juni 2021 18:40, Service@helixDB schrieb:\r\n> test mit attachment und hier ist der body\r\n> \r\n',0,0,0,'{\"test\": \"1\"}','56e740e1eb43b540a081f47326e4d2a4','2021-07-11 12:49:17',1,NULL,0,NULL,NULL),('73eec4b8-e245-11eb-9dd4-309c2364fdb6','73eac06a-e245-11eb-9dd4-309c2364fdb6','ticket eröffnet',_binary 'ticket created\r\nAm Sonntag, 27. Juni 2021 18:40, Service@helixDB schrieb:\r\n> test mit attachment und hier ist der body\r\n> \r\n',0,0,0,'{\"test\": \"1\"}','56e740e1eb43b540a081f47326e4d2a4','2021-07-11 12:42:29',1,NULL,0,NULL,NULL),('793e7a40-e246-11eb-9dd4-309c2364fdb6','793ad72b-e246-11eb-9dd4-309c2364fdb6','ticket eröffnet',_binary 'ticket created\r\nAm Sonntag, 27. Juni 2021 18:40, Service@helixDB schrieb:\r\n> test mit attachment und hier ist der body\r\n> \r\n',0,0,0,'{\"test\": \"1\"}','56e740e1eb43b540a081f47326e4d2a4','2021-07-11 12:49:47',1,NULL,0,NULL,NULL),('98bda32b-3418-11ec-b696-d8bbc10f2ae0','98baa894-3418-11ec-b696-d8bbc10f2ae0','Returned Mail: Activation Link',_binary '*** MAILVERSAND FEHLERBERICHT ***\r\n\r\nDie E-Mail wurde eingeliefert am Sonnabend, 23. Oktober 2021 17:46:29 +0200 (CEST)\r\nvon Host [10.0.0.199]   .\r\n\r\nBetreff: Activation Link\r\n\r\nDer Mailversand zum folgenden Empfänger ist endgültig gescheitert:\r\n\r\nmika-roos@web.de\r\n   Letzter Fehler: 554 5.0.0 \r\n   Erklärung: host mx-ha02.web.de [212.227.17.8] said: Transaction failed Reject due \r\n              to policy restrictions. For explanation visit \r\n              https://web.de/email/senderguidelines?ip=81.169.146.161&c=\r\n              hi\r\n\r\n   Auszug aus dem Session-Protokoll:\r\n   ... während der Kommunikation mit dem Mailserver mx-ha02.web.de [212.227.17.8]:\r\n   >>> DATA (EOM)\r\n   <<< 554 Transaction failed Reject due to policy restrictions. For \r\n       explanation visit \r\n       https://web.de/email/senderguidelines?ip=81.169.146.161&c=hi\r\n',0,0,0,'{\"test\": \"1\"}','f5acc378431b9db0205db1dc6e3cc9c9','2021-10-23 15:47:58',1,NULL,0,NULL,NULL),('b01ee2f6-e246-11eb-9dd4-309c2364fdb6','b01b31aa-e246-11eb-9dd4-309c2364fdb6','ticket eröffnet',_binary 'ticket created\r\nAm Sonntag, 27. Juni 2021 18:40, Service@helixDB schrieb:\r\n> test mit attachment und hier ist der body\r\n> \r\n',0,0,0,'{\"test\": \"1\"}','56e740e1eb43b540a081f47326e4d2a4','2021-07-11 12:51:19',1,NULL,0,NULL,NULL),('c783ca5d-e245-11eb-9dd4-309c2364fdb6','c7800829-e245-11eb-9dd4-309c2364fdb6','ticket eröffnet',_binary 'ticket created\r\nAm Sonntag, 27. Juni 2021 18:40, Service@helixDB schrieb:\r\n> test mit attachment und hier ist der body\r\n> \r\n',0,0,0,'{\"test\": \"1\"}','56e740e1eb43b540a081f47326e4d2a4','2021-07-11 12:44:49',1,NULL,0,NULL,NULL),('d47869b1-e246-11eb-9dd4-309c2364fdb6','d4764908-e246-11eb-9dd4-309c2364fdb6','test',_binary 'test\r\n\r\n',0,0,0,'{\"test\": \"1\"}','c3587fd3fd995e51a017b7f4d346b292','2021-07-11 12:52:20',1,NULL,0,NULL,NULL),('e17e5d0f-3418-11ec-b696-d8bbc10f2ae0','e17d1cc1-3418-11ec-b696-d8bbc10f2ae0','Returned Mail: Activation Link',_binary '*** MAILVERSAND FEHLERBERICHT ***\r\n\r\nDie E-Mail wurde eingeliefert am Sonnabend, 23. Oktober 2021 17:49:33 +0200 (CEST)\r\nvon Host [10.0.0.199]   .\r\n\r\nBetreff: Activation Link\r\n\r\nDer Mailversand zum folgenden Empfänger ist endgültig gescheitert:\r\n\r\nmika-roos@web.de\r\n   Letzter Fehler: 554 5.0.0 \r\n   Erklärung: host mx-ha02.web.de [212.227.17.8] said: Transaction failed Reject due \r\n              to policy restrictions. For explanation visit \r\n              https://web.de/email/senderguidelines?ip=85.215.255.24&c=h\r\n              i\r\n\r\n   Auszug aus dem Session-Protokoll:\r\n   ... während der Kommunikation mit dem Mailserver mx-ha02.web.de [212.227.17.8]:\r\n   >>> DATA (EOM)\r\n   <<< 554 Transaction failed Reject due to policy restrictions. For \r\n       explanation visit \r\n       https://web.de/email/senderguidelines?ip=85.215.255.24&c=hi\r\n',0,0,0,'{\"test\": \"1\"}','f5acc378431b9db0205db1dc6e3cc9c9','2021-10-23 15:50:00',1,NULL,0,NULL,NULL),('ed57815b-e246-11eb-9dd4-309c2364fdb6','d4764908-e246-11eb-9dd4-309c2364fdb6','Re: [#CONF-20210711-8#] - Ticket opened',_binary 'das ist meine zweite nachricht auf test\r\n\r\nAm 11.07.2021 um 14:52 schrieb service@helixdb.org:\r\n> Am Donnerstag, 8. Juli 2021 20:18, Mika Roos schrieb:\r\n>> test\r\n>>\r\n',0,0,0,'{\"test\": \"1\"}','3964b548268425872c98e82917accb7c','2021-07-11 12:53:02',1,NULL,0,NULL,NULL),('fb8bc372-e244-11eb-9dd4-309c2364fdb6','fb871255-e244-11eb-9dd4-309c2364fdb6','ticket eröffnet',_binary 'ticket created\r\nAm Sonntag, 27. Juni 2021 18:40, Service@helixDB schrieb:\r\n> test mit attachment und hier ist der body\r\n> \r\n',0,0,0,'{\"test\": \"1\"}','56e740e1eb43b540a081f47326e4d2a4','2021-07-11 12:39:07',1,NULL,0,NULL,NULL);
/*!40000 ALTER TABLE `message` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `message_BEFORE_INSERT` BEFORE INSERT ON `message` FOR EACH ROW BEGIN

SET new.creation_datetime = NOW();   
END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `message_BEFORE_UPDATE` BEFORE UPDATE ON `message` FOR EACH ROW BEGIN
SET new.changed_datetime = NOW();
END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `message_attachment`
--

DROP TABLE IF EXISTS `message_attachment`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `message_attachment` (
  `uuid` varchar(36) NOT NULL,
  `first_source_message_uuid` varchar(36) DEFAULT NULL,
  `account_uuid` varchar(36) DEFAULT NULL,
  `path` varchar(512) NOT NULL,
  `file_extension` varchar(5) DEFAULT NULL,
  `hash` varchar(48) NOT NULL,
  `checked_for_virus` tinyint NOT NULL COMMENT 'Bool für AntiVirus check, ob geprüft wurde durch nClam oder nicht',
  `checked_for_violence` tinyint NOT NULL COMMENT 'Bool für gewaltverherlichende Sprache oderracism speech',
  `checked_for_sexual_content` tinyint NOT NULL COMMENT 'Bool für Prüfung ob Inhalt/Attachment sexual ist (durch KI von Facebook Abgleichen oder trainierte TensorFlow Modelle), wenn Sexual Content == true, dann Inhalt verbieten',
  `checked_for_sexual_content_meta` json NOT NULL COMMENT 'Speichert den KI Score für das Attachment',
  `creation_datetime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `activation_datetime` datetime DEFAULT NULL,
  `active` tinyint NOT NULL DEFAULT '1',
  `deleted` tinyint DEFAULT '0',
  `deletion_datetime` datetime DEFAULT NULL,
  `changed_datetime` datetime DEFAULT NULL,
  PRIMARY KEY (`hash`),
  KEY `uuid_Idx` (`uuid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `message_attachment`
--

LOCK TABLES `message_attachment` WRITE;
/*!40000 ALTER TABLE `message_attachment` DISABLE KEYS */;
INSERT INTO `message_attachment` VALUES ('98d1cceb-3418-11ec-b696-d8bbc10f2ae0','98bda32b-3418-11ec-b696-d8bbc10f2ae0','98caf4cf-3418-11ec-b696-d8bbc10f2ae0','','.','09B8E3C2918CD2DD5B90E85DF69D4AD8',0,0,0,'{\"test\": \"1\"}','2021-10-23 15:47:58',NULL,1,0,NULL,NULL),('e1819f20-3418-11ec-b696-d8bbc10f2ae0','e17e5d0f-3418-11ec-b696-d8bbc10f2ae0','98caf4cf-3418-11ec-b696-d8bbc10f2ae0','','.','1C4D8D83D22E9A43725A0D5BF605D0A5',0,0,0,'{\"test\": \"1\"}','2021-10-23 15:50:00',NULL,1,0,NULL,NULL),('e17f87f5-3418-11ec-b696-d8bbc10f2ae0','e17e5d0f-3418-11ec-b696-d8bbc10f2ae0','98caf4cf-3418-11ec-b696-d8bbc10f2ae0','','.','29D4154023945C9C74E9764B5DC3AF62',0,0,0,'{\"test\": \"1\"}','2021-10-23 15:50:00',NULL,1,0,NULL,NULL),('522c4561-e015-11eb-8d92-309c2364fdb6','0690e796-e015-11eb-8d92-309c2364fdb6','34b543fb-deb2-11eb-8405-842afd097283','','.png','2C2B0E79F27C38A278437AD8FFBC469F',0,0,0,'{\"test\": \"1\"}','2021-07-08 17:52:54',NULL,1,0,NULL,NULL),('98cc57b7-3418-11ec-b696-d8bbc10f2ae0','98bda32b-3418-11ec-b696-d8bbc10f2ae0','98caf4cf-3418-11ec-b696-d8bbc10f2ae0','','.','808EF79F2ED4A9D237A3987767822590',0,0,0,'{\"test\": \"1\"}','2021-10-23 15:47:58',NULL,1,0,NULL,NULL),('3ff43c4f-e015-11eb-8d92-309c2364fdb6','0690e796-e015-11eb-8d92-309c2364fdb6','34b543fb-deb2-11eb-8405-842afd097283','','.txt','C45D7E8A442A09A3889F1D4BDAA4D202',0,0,0,'{\"test\": \"1\"}','2021-07-08 17:52:23',NULL,1,0,NULL,NULL),('c0604af6-e012-11eb-8d92-309c2364fdb6','00000000-0000-0000-0000-000000000000','34b543fb-deb2-11eb-8405-842afd097283','','.txt','CA630EC44BEA1A969E4454F3BFD9FCBD',0,0,0,'{\"test\": \"1\"}','2021-07-08 17:34:30',NULL,1,0,NULL,NULL);
/*!40000 ALTER TABLE `message_attachment` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `message_attachment_BEFORE_INSERT` BEFORE INSERT ON `message_attachment` FOR EACH ROW BEGIN

SET new.creation_datetime = NOW();   
END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `message_attachment_BEFORE_UPDATE` BEFORE UPDATE ON `message_attachment` FOR EACH ROW BEGIN
SET new.changed_datetime = NOW();
END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `message_conversation`
--

DROP TABLE IF EXISTS `message_conversation`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `message_conversation` (
  `uuid` varchar(36) NOT NULL,
  `creation_datetime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `display_conf_identifier` varchar(64) NOT NULL,
  `scoped_conf_count` int NOT NULL DEFAULT '0',
  `message_conversation_state_type_uuid` varchar(36) NOT NULL,
  `communication_medium_uuid` varchar(36) NOT NULL,
  `message_queue_uuid` varchar(36) NOT NULL,
  `active` tinyint NOT NULL DEFAULT '1',
  `activation_datetime` datetime DEFAULT NULL,
  `deleted` tinyint DEFAULT '0',
  `deletion_datetime` datetime DEFAULT NULL,
  `changed_datetime` datetime DEFAULT NULL,
  PRIMARY KEY (`display_conf_identifier`),
  KEY `fk_messageToConvType_idx` (`message_conversation_state_type_uuid`),
  KEY `uuid_Idx` (`uuid`),
  KEY `fk_messageConvToCommMedium_idx` (`communication_medium_uuid`),
  KEY `fk_messageToQueue_idx` (`message_queue_uuid`),
  CONSTRAINT `fk_messageConvToCommMedium` FOREIGN KEY (`communication_medium_uuid`) REFERENCES `communication_medium` (`uuid`),
  CONSTRAINT `fk_messageToConvType` FOREIGN KEY (`message_conversation_state_type_uuid`) REFERENCES `message_conversation_state_type` (`uuid`),
  CONSTRAINT `fk_messageToQueue` FOREIGN KEY (`message_queue_uuid`) REFERENCES `message_queue` (`uuid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `message_conversation`
--

LOCK TABLES `message_conversation` WRITE;
/*!40000 ALTER TABLE `message_conversation` DISABLE KEYS */;
INSERT INTO `message_conversation` VALUES ('fb871255-e244-11eb-9dd4-309c2364fdb6','2021-07-11 12:39:07','[#CONF-20210711-1#]',1,'8c7cb72d-bfa7-11eb-a11f-309c2364fdb6','6ad9523e-d91a-11eb-81f0-842afd097283','f0d473ae-e231-11eb-9dd4-309c2364fdb6',1,NULL,0,NULL,NULL),('73eac06a-e245-11eb-9dd4-309c2364fdb6','2021-07-11 12:42:29','[#CONF-20210711-2#]',2,'8c7cb72d-bfa7-11eb-a11f-309c2364fdb6','6ad9523e-d91a-11eb-81f0-842afd097283','f0d473ae-e231-11eb-9dd4-309c2364fdb6',1,NULL,0,NULL,NULL),('c7800829-e245-11eb-9dd4-309c2364fdb6','2021-07-11 12:44:49','[#CONF-20210711-3#]',3,'8c7cb72d-bfa7-11eb-a11f-309c2364fdb6','6ad9523e-d91a-11eb-81f0-842afd097283','f0d473ae-e231-11eb-9dd4-309c2364fdb6',1,NULL,0,NULL,NULL),('56bc3e8d-e246-11eb-9dd4-309c2364fdb6','2021-07-11 12:48:49','[#CONF-20210711-4#]',4,'8c7cb72d-bfa7-11eb-a11f-309c2364fdb6','6ad9523e-d91a-11eb-81f0-842afd097283','f0d473ae-e231-11eb-9dd4-309c2364fdb6',1,NULL,0,NULL,NULL),('6754f03c-e246-11eb-9dd4-309c2364fdb6','2021-07-11 12:49:17','[#CONF-20210711-5#]',5,'8c7cb72d-bfa7-11eb-a11f-309c2364fdb6','6ad9523e-d91a-11eb-81f0-842afd097283','f0d473ae-e231-11eb-9dd4-309c2364fdb6',1,NULL,0,NULL,NULL),('793ad72b-e246-11eb-9dd4-309c2364fdb6','2021-07-11 12:49:47','[#CONF-20210711-6#]',6,'8c7cb72d-bfa7-11eb-a11f-309c2364fdb6','6ad9523e-d91a-11eb-81f0-842afd097283','f0d473ae-e231-11eb-9dd4-309c2364fdb6',1,NULL,0,NULL,NULL),('b01b31aa-e246-11eb-9dd4-309c2364fdb6','2021-07-11 12:51:19','[#CONF-20210711-7#]',7,'8c7cb72d-bfa7-11eb-a11f-309c2364fdb6','6ad9523e-d91a-11eb-81f0-842afd097283','f0d473ae-e231-11eb-9dd4-309c2364fdb6',1,NULL,0,NULL,NULL),('d4764908-e246-11eb-9dd4-309c2364fdb6','2021-07-11 12:52:20','[#CONF-20210711-8#]',8,'8c7cb72d-bfa7-11eb-a11f-309c2364fdb6','6ad9523e-d91a-11eb-81f0-842afd097283','f0d473ae-e231-11eb-9dd4-309c2364fdb6',1,NULL,0,NULL,'2021-07-11 17:48:40'),('e17d1cc1-3418-11ec-b696-d8bbc10f2ae0','2021-10-23 15:50:00','[#CONF-20211023-10#]',10,'8c7cb72d-bfa7-11eb-a11f-309c2364fdb6','6ad9523e-d91a-11eb-81f0-842afd097283','f0d473ae-e231-11eb-9dd4-309c2364fdb6',1,NULL,0,NULL,NULL),('98baa894-3418-11ec-b696-d8bbc10f2ae0','2021-10-23 15:47:58','[#CONF-20211023-9#]',9,'8c7cb72d-bfa7-11eb-a11f-309c2364fdb6','6ad9523e-d91a-11eb-81f0-842afd097283','f0d473ae-e231-11eb-9dd4-309c2364fdb6',1,NULL,0,NULL,NULL);
/*!40000 ALTER TABLE `message_conversation` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `message_conversation_BEFORE_INSERT` BEFORE INSERT ON `message_conversation` FOR EACH ROW BEGIN

	DECLARE v1 INT;
	SET new.creation_datetime = NOW();   

	
	select MAX(scoped_conf_count) INTO v1 from message_conversation where YEAR(CURDATE()) = YEAR(creation_datetime);
	
	SET new.scoped_conf_count = coalesce(v1+1,1);
	SET new.display_conf_identifier = CONCAT('[#CONF-',DATE_FORMAT(CURDATE(),'%Y%m%d'),'-',new.scoped_conf_count,'#]');
	
END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `message_conversation_BEFORE_UPDATE` BEFORE UPDATE ON `message_conversation` FOR EACH ROW BEGIN
SET new.changed_datetime = NOW();
END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `message_conversation_state_type`
--

DROP TABLE IF EXISTS `message_conversation_state_type`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `message_conversation_state_type` (
  `uuid` varchar(36) NOT NULL,
  `creation_datetime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `name` varchar(45) DEFAULT NULL,
  `active` tinyint NOT NULL DEFAULT '1',
  `activation_datetime` datetime DEFAULT NULL,
  `deleted` tinyint DEFAULT '0',
  `deletion_datetime` datetime DEFAULT NULL,
  `changed_datetime` datetime DEFAULT NULL,
  PRIMARY KEY (`uuid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `message_conversation_state_type`
--

LOCK TABLES `message_conversation_state_type` WRITE;
/*!40000 ALTER TABLE `message_conversation_state_type` DISABLE KEYS */;
INSERT INTO `message_conversation_state_type` VALUES ('8c7cb72d-bfa7-11eb-a11f-309c2364fdb6','2021-05-28 11:26:30','open',1,NULL,0,NULL,NULL),('8c7cffc2-bfa7-11eb-a11f-309c2364fdb6','2021-05-28 11:26:30','closed',1,NULL,0,NULL,NULL);
/*!40000 ALTER TABLE `message_conversation_state_type` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `message_conversation_type_BEFORE_INSERT` BEFORE INSERT ON `message_conversation_state_type` FOR EACH ROW BEGIN

SET new.creation_datetime = NOW();   


END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `message_conversation_type_BEFORE_UPDATE` BEFORE UPDATE ON `message_conversation_state_type` FOR EACH ROW BEGIN
SET new.changed_datetime = NOW();
END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `message_queue`
--

DROP TABLE IF EXISTS `message_queue`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `message_queue` (
  `uuid` varchar(36) NOT NULL,
  `system_message_user_uuid` varchar(36) NOT NULL,
  `message_queue_uuid` varchar(36) DEFAULT NULL COMMENT 'parent queue hirarchy',
  `name` varchar(45) NOT NULL,
  `inbox_folder` varchar(45) DEFAULT NULL,
  `procedded_folder` varchar(45) DEFAULT NULL,
  `delete_mails_after_processing` tinyint NOT NULL,
  `is_junk` tinyint NOT NULL,
  `initial_msg` text,
  `creation_datetime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `active` tinyint NOT NULL DEFAULT '1',
  `activation_datetime` datetime DEFAULT NULL,
  `deleted` tinyint DEFAULT '1',
  `deletion_datetime` datetime DEFAULT NULL,
  `changed_datetime` datetime DEFAULT NULL,
  `max_attachment_file_size` double DEFAULT NULL,
  `allowed_attachment_file_extensions` json DEFAULT NULL,
  `attachment_av_check` tinyint DEFAULT NULL,
  PRIMARY KEY (`system_message_user_uuid`,`name`),
  KEY `uuid_idx` (`uuid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `message_queue`
--

LOCK TABLES `message_queue` WRITE;
/*!40000 ALTER TABLE `message_queue` DISABLE KEYS */;
INSERT INTO `message_queue` VALUES ('405c169e-e250-11eb-9dd4-309c2364fdb6','1b72a750-dbe3-11eb-8cd9-842afd097283',NULL,'Junk',NULL,NULL,0,1,NULL,'2021-07-11 14:00:29',1,NULL,1,NULL,'2021-07-24 11:54:06',NULL,NULL,1),('f0d473ae-e231-11eb-9dd4-309c2364fdb6','1b72a750-dbe3-11eb-8cd9-842afd097283',NULL,'Service','INBOX','processed',0,0,'Ticket opened Body TXT','2021-07-11 10:24:18',1,NULL,0,NULL,'2021-07-24 12:26:21',2,'[{\"extension\": \".png\"}, {\"extension\": \".pdf\"}, {\"extension\": \".jpg\"}]',1);
/*!40000 ALTER TABLE `message_queue` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `message_queue_BEFORE_INSERT` BEFORE INSERT ON `message_queue` FOR EACH ROW BEGIN
	SET new.creation_datetime = NOW();
    
END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `message_queue_BEFORE_UPDATE` BEFORE UPDATE ON `message_queue` FOR EACH ROW BEGIN
	SET new.changed_datetime = NOW();
END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `message_relation_to_account`
--

DROP TABLE IF EXISTS `message_relation_to_account`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `message_relation_to_account` (
  `uuid` varchar(36) NOT NULL,
  `message_uuid` varchar(36) NOT NULL,
  `account_uuid` varchar(36) NOT NULL,
  `message_sending_type_uuid` varchar(36) NOT NULL,
  `creation_datetime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `active` tinyint NOT NULL DEFAULT '1',
  `activation_datetime` datetime DEFAULT NULL,
  `deleted` tinyint DEFAULT '0',
  `deletion_datetime` datetime DEFAULT NULL,
  `changed_datetime` datetime DEFAULT NULL,
  PRIMARY KEY (`message_uuid`,`account_uuid`,`message_sending_type_uuid`),
  KEY `uuid_Idx` (`uuid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `message_relation_to_account`
--

LOCK TABLES `message_relation_to_account` WRITE;
/*!40000 ALTER TABLE `message_relation_to_account` DISABLE KEYS */;
INSERT INTO `message_relation_to_account` VALUES ('56c366ae-e246-11eb-9dd4-309c2364fdb6','56bff630-e246-11eb-9dd4-309c2364fdb6','34b543fb-deb2-11eb-8405-842afd097283','6e9e9021-bfa0-11eb-a11f-309c2364fdb6','2021-07-11 12:48:49',1,NULL,0,NULL,NULL),('56c235ba-e246-11eb-9dd4-309c2364fdb6','56bff630-e246-11eb-9dd4-309c2364fdb6','8aaa84d0-e245-11eb-9dd4-309c2364fdb6','6e9ec4ed-bfa0-11eb-a11f-309c2364fdb6','2021-07-11 12:48:49',1,NULL,0,NULL,NULL),('67588ef0-e246-11eb-9dd4-309c2364fdb6','67565129-e246-11eb-9dd4-309c2364fdb6','34b543fb-deb2-11eb-8405-842afd097283','6e9e9021-bfa0-11eb-a11f-309c2364fdb6','2021-07-11 12:49:17',1,NULL,0,NULL,NULL),('675766f3-e246-11eb-9dd4-309c2364fdb6','67565129-e246-11eb-9dd4-309c2364fdb6','8aaa84d0-e245-11eb-9dd4-309c2364fdb6','6e9ec4ed-bfa0-11eb-a11f-309c2364fdb6','2021-07-11 12:49:17',1,NULL,0,NULL,NULL),('930fafc3-e245-11eb-9dd4-309c2364fdb6','73eec4b8-e245-11eb-9dd4-309c2364fdb6','8aaa84d0-e245-11eb-9dd4-309c2364fdb6','6e9ec4ed-bfa0-11eb-a11f-309c2364fdb6','2021-07-11 12:43:21',1,NULL,0,NULL,NULL),('7941ca57-e246-11eb-9dd4-309c2364fdb6','793e7a40-e246-11eb-9dd4-309c2364fdb6','34b543fb-deb2-11eb-8405-842afd097283','6e9e9021-bfa0-11eb-a11f-309c2364fdb6','2021-07-11 12:49:47',1,NULL,0,NULL,NULL),('79407229-e246-11eb-9dd4-309c2364fdb6','793e7a40-e246-11eb-9dd4-309c2364fdb6','8aaa84d0-e245-11eb-9dd4-309c2364fdb6','6e9ec4ed-bfa0-11eb-a11f-309c2364fdb6','2021-07-11 12:49:47',1,NULL,0,NULL,NULL),('98d580df-3418-11ec-b696-d8bbc10f2ae0','98bda32b-3418-11ec-b696-d8bbc10f2ae0','8aaa84d0-e245-11eb-9dd4-309c2364fdb6','6e9e9021-bfa0-11eb-a11f-309c2364fdb6','2021-10-23 15:47:58',1,NULL,0,NULL,NULL),('98d3d245-3418-11ec-b696-d8bbc10f2ae0','98bda32b-3418-11ec-b696-d8bbc10f2ae0','98caf4cf-3418-11ec-b696-d8bbc10f2ae0','6e9ec4ed-bfa0-11eb-a11f-309c2364fdb6','2021-10-23 15:47:58',1,NULL,0,NULL,NULL),('b0223afd-e246-11eb-9dd4-309c2364fdb6','b01ee2f6-e246-11eb-9dd4-309c2364fdb6','34b543fb-deb2-11eb-8405-842afd097283','6e9e9021-bfa0-11eb-a11f-309c2364fdb6','2021-07-11 12:51:19',1,NULL,0,NULL,NULL),('b0210f0c-e246-11eb-9dd4-309c2364fdb6','b01ee2f6-e246-11eb-9dd4-309c2364fdb6','8aaa84d0-e245-11eb-9dd4-309c2364fdb6','6e9ec4ed-bfa0-11eb-a11f-309c2364fdb6','2021-07-11 12:51:19',1,NULL,0,NULL,NULL),('c7871b60-e245-11eb-9dd4-309c2364fdb6','c783ca5d-e245-11eb-9dd4-309c2364fdb6','34b543fb-deb2-11eb-8405-842afd097283','6e9e9021-bfa0-11eb-a11f-309c2364fdb6','2021-07-11 12:44:49',1,NULL,0,NULL,NULL),('c785fdbd-e245-11eb-9dd4-309c2364fdb6','c783ca5d-e245-11eb-9dd4-309c2364fdb6','8aaa84d0-e245-11eb-9dd4-309c2364fdb6','6e9ec4ed-bfa0-11eb-a11f-309c2364fdb6','2021-07-11 12:44:49',1,NULL,0,NULL,NULL),('d47b00dd-e246-11eb-9dd4-309c2364fdb6','d47869b1-e246-11eb-9dd4-309c2364fdb6','8aaa84d0-e245-11eb-9dd4-309c2364fdb6','6e9e9021-bfa0-11eb-a11f-309c2364fdb6','2021-07-11 12:52:20',1,NULL,0,NULL,NULL),('d479da88-e246-11eb-9dd4-309c2364fdb6','d47869b1-e246-11eb-9dd4-309c2364fdb6','a546f7aa-dea9-11eb-8405-842afd097283','6e9ec4ed-bfa0-11eb-a11f-309c2364fdb6','2021-07-11 12:52:20',1,NULL,0,NULL,NULL),('e1841bbf-3418-11ec-b696-d8bbc10f2ae0','e17e5d0f-3418-11ec-b696-d8bbc10f2ae0','8aaa84d0-e245-11eb-9dd4-309c2364fdb6','6e9e9021-bfa0-11eb-a11f-309c2364fdb6','2021-10-23 15:50:00',1,NULL,0,NULL,NULL),('e1835b16-3418-11ec-b696-d8bbc10f2ae0','e17e5d0f-3418-11ec-b696-d8bbc10f2ae0','98caf4cf-3418-11ec-b696-d8bbc10f2ae0','6e9ec4ed-bfa0-11eb-a11f-309c2364fdb6','2021-10-23 15:50:00',1,NULL,0,NULL,NULL),('ed58c9e7-e246-11eb-9dd4-309c2364fdb6','ed57815b-e246-11eb-9dd4-309c2364fdb6','a546f7aa-dea9-11eb-8405-842afd097283','6e9ec4ed-bfa0-11eb-a11f-309c2364fdb6','2021-07-11 12:53:02',1,NULL,0,NULL,NULL);
/*!40000 ALTER TABLE `message_relation_to_account` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `message_relation_to_account_BEFORE_INSERT` BEFORE INSERT ON `message_relation_to_account` FOR EACH ROW BEGIN

SET new.creation_datetime = NOW();   
END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `message_relation_to_account_BEFORE_UPDATE` BEFORE UPDATE ON `message_relation_to_account` FOR EACH ROW BEGIN

SET new.changed_datetime = NOW();
END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `message_relation_to_attachment`
--

DROP TABLE IF EXISTS `message_relation_to_attachment`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `message_relation_to_attachment` (
  `uuid` varchar(36) NOT NULL,
  `message_uuid` varchar(36) NOT NULL,
  `message_attachment_uuid` varchar(36) NOT NULL,
  `file_extension` varchar(5) DEFAULT NULL,
  `creation_datetime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `active` tinyint NOT NULL DEFAULT '1',
  `activation_datetime` datetime DEFAULT NULL,
  `deleted` tinyint DEFAULT '0',
  `deletion_datetime` datetime DEFAULT NULL,
  `changed_datetime` datetime DEFAULT NULL,
  PRIMARY KEY (`message_uuid`,`message_attachment_uuid`),
  KEY `uuid_Idx` (`uuid`),
  KEY `fk_messageRelToAttach2_idx` (`message_attachment_uuid`),
  CONSTRAINT `fk_messageRelToAttach1` FOREIGN KEY (`message_uuid`) REFERENCES `message` (`uuid`),
  CONSTRAINT `fk_messageRelToAttach2` FOREIGN KEY (`message_attachment_uuid`) REFERENCES `message_attachment` (`uuid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `message_relation_to_attachment`
--

LOCK TABLES `message_relation_to_attachment` WRITE;
/*!40000 ALTER TABLE `message_relation_to_attachment` DISABLE KEYS */;
INSERT INTO `message_relation_to_attachment` VALUES ('98cfde81-3418-11ec-b696-d8bbc10f2ae0','98bda32b-3418-11ec-b696-d8bbc10f2ae0','98cc57b7-3418-11ec-b696-d8bbc10f2ae0','.','2021-10-23 15:47:58',1,NULL,0,NULL,NULL),('98d30218-3418-11ec-b696-d8bbc10f2ae0','98bda32b-3418-11ec-b696-d8bbc10f2ae0','98d1cceb-3418-11ec-b696-d8bbc10f2ae0','.','2021-10-23 15:47:58',1,NULL,0,NULL,NULL),('e180b51d-3418-11ec-b696-d8bbc10f2ae0','e17e5d0f-3418-11ec-b696-d8bbc10f2ae0','e17f87f5-3418-11ec-b696-d8bbc10f2ae0','.','2021-10-23 15:50:00',1,NULL,0,NULL,NULL),('e182cc39-3418-11ec-b696-d8bbc10f2ae0','e17e5d0f-3418-11ec-b696-d8bbc10f2ae0','e1819f20-3418-11ec-b696-d8bbc10f2ae0','.','2021-10-23 15:50:00',1,NULL,0,NULL,NULL);
/*!40000 ALTER TABLE `message_relation_to_attachment` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `message_relation_to_attachment_BEFORE_INSERT` BEFORE INSERT ON `message_relation_to_attachment` FOR EACH ROW BEGIN

SET new.creation_datetime = NOW();   
END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `message_relation_to_attachment_BEFORE_UPDATE` BEFORE UPDATE ON `message_relation_to_attachment` FOR EACH ROW BEGIN
SET new.changed_datetime = NOW();
END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `message_sending_type`
--

DROP TABLE IF EXISTS `message_sending_type`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `message_sending_type` (
  `uuid` varchar(36) NOT NULL,
  `name` varchar(45) NOT NULL,
  `description` varchar(256) DEFAULT NULL,
  `creation_datetime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `activation_datetime` datetime DEFAULT NULL,
  `deleted` tinyint DEFAULT '0',
  `deletion_datetime` datetime DEFAULT NULL,
  `changed_datetime` datetime DEFAULT NULL,
  PRIMARY KEY (`uuid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `message_sending_type`
--

LOCK TABLES `message_sending_type` WRITE;
/*!40000 ALTER TABLE `message_sending_type` DISABLE KEYS */;
INSERT INTO `message_sending_type` VALUES ('6e9e9021-bfa0-11eb-a11f-309c2364fdb6','to','receiver','2021-05-28 10:35:33',NULL,0,NULL,NULL),('6e9eaf47-bfa0-11eb-a11f-309c2364fdb6','cc','carbon copy','2021-05-28 10:35:33',NULL,0,NULL,NULL),('6e9ebb70-bfa0-11eb-a11f-309c2364fdb6','bcc','blind carbon copy','2021-05-28 10:35:33',NULL,0,NULL,NULL),('6e9ec4ed-bfa0-11eb-a11f-309c2364fdb6','from','sender','2021-05-28 10:35:33',NULL,0,NULL,NULL);
/*!40000 ALTER TABLE `message_sending_type` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `account_type_BEFORE_INSERT` BEFORE INSERT ON `message_sending_type` FOR EACH ROW BEGIN
SET new.creation_datetime = NOW();   
END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `account_type_BEFORE_UPDATE` BEFORE UPDATE ON `message_sending_type` FOR EACH ROW BEGIN
SET new.changed_datetime = NOW();
END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `odbc_driver`
--

DROP TABLE IF EXISTS `odbc_driver`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `odbc_driver` (
  `uuid` varchar(36) NOT NULL,
  `name` varchar(128) NOT NULL,
  `description` varchar(256) NOT NULL,
  `icon_exist_flag` tinyint NOT NULL DEFAULT '0' COMMENT 'Only Icon with Size 32x32',
  `banner_exist_flag` tinyint NOT NULL DEFAULT '0' COMMENT 'Like Icon but with max Size B:256,H:128',
  `creation_datetime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `active` tinyint NOT NULL DEFAULT '1',
  `activation_datetime` datetime DEFAULT NULL,
  `deleted` tinyint DEFAULT '0',
  `deletion_datetime` datetime DEFAULT NULL,
  `changed_datetime` datetime DEFAULT NULL,
  PRIMARY KEY (`uuid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `odbc_driver`
--

LOCK TABLES `odbc_driver` WRITE;
/*!40000 ALTER TABLE `odbc_driver` DISABLE KEYS */;
INSERT INTO `odbc_driver` VALUES ('266b1137-a5b5-11eb-bac0-309c2364fdb6','MySQL','MySQL ODBC Driver',1,1,'2021-04-25 10:58:21',1,NULL,0,NULL,'2021-05-27 15:57:51'),('266b9d70-a5b5-11eb-bac0-309c2364fdb6','PostgreSQL','Postgresql ODBC Driver',1,1,'2021-04-25 10:58:21',1,NULL,0,NULL,'2021-05-27 15:57:51'),('266ba188-a5b5-11eb-bac0-309c2364fdb6','Microsoft Excel','Microsoft Default ODBC Driver for Excel',1,1,'2021-04-25 10:58:21',1,NULL,0,NULL,'2021-05-27 15:57:51'),('266bca8b-a5b5-11eb-bac0-309c2364fdb6','Microsoft Access','Microsoft Default ODBC Driver for Access',0,0,'2021-04-25 10:58:21',1,NULL,0,NULL,'2021-05-27 11:47:52');
/*!40000 ALTER TABLE `odbc_driver` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`rest`@`%`*/ /*!50003 TRIGGER `odbc_driver_BEFORE_INSERT` BEFORE INSERT ON `odbc_driver` FOR EACH ROW BEGIN 	SET new.creation_datetime = NOW();   END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`rest`@`%`*/ /*!50003 TRIGGER `odbc_driver_BEFORE_UPDATE` BEFORE UPDATE ON `odbc_driver` FOR EACH ROW BEGIN 	SET new.changed_datetime = NOW();END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `odbc_driver_version`
--

DROP TABLE IF EXISTS `odbc_driver_version`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `odbc_driver_version` (
  `uuid` varchar(36) NOT NULL,
  `odbc_driver_uuid` varchar(36) DEFAULT NULL,
  `version_vendor` varchar(90) NOT NULL,
  `version_internal` varchar(45) NOT NULL,
  `version_subversion_internal` varchar(45) NOT NULL,
  `encoding` varchar(45) NOT NULL,
  `creation_datetime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `active` tinyint NOT NULL DEFAULT '1',
  `activation_datetime` datetime DEFAULT NULL,
  `deleted` tinyint DEFAULT '0',
  `deletion_datetime` datetime DEFAULT NULL,
  `changed_datetime` datetime DEFAULT NULL,
  PRIMARY KEY (`uuid`),
  KEY `fk_odbcDriverVersionToOdbcDriver_idx` (`odbc_driver_uuid`),
  CONSTRAINT `fk_odbcDriverVersionToOdbcDriver` FOREIGN KEY (`odbc_driver_uuid`) REFERENCES `odbc_driver` (`uuid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `odbc_driver_version`
--

LOCK TABLES `odbc_driver_version` WRITE;
/*!40000 ALTER TABLE `odbc_driver_version` DISABLE KEYS */;
INSERT INTO `odbc_driver_version` VALUES ('1d5f08ed-a5b7-11eb-bac0-309c2364fdb6','266b1137-a5b5-11eb-bac0-309c2364fdb6','8.0.19','8.0','19','Unicode','2021-04-25 11:12:25',1,NULL,0,NULL,'2021-05-27 11:47:52'),('1d5f7eb1-a5b7-11eb-bac0-309c2364fdb6','266b9d70-a5b5-11eb-bac0-309c2364fdb6','12.2','12','2','Unicode','2021-04-25 11:12:25',1,NULL,0,NULL,'2021-05-27 11:47:52'),('1d5f80cc-a5b7-11eb-bac0-309c2364fdb6','266b1137-a5b5-11eb-bac0-309c2364fdb6','8.0.11','8.0','11','Unicode','2021-04-25 11:12:25',1,NULL,0,NULL,'2021-05-27 11:47:52'),('1d5f820d-a5b7-11eb-bac0-309c2364fdb6','266ba188-a5b5-11eb-bac0-309c2364fdb6','1.0','1','0','Unicode','2021-04-25 11:12:25',1,NULL,0,NULL,'2021-05-27 11:47:52'),('1d5f8336-a5b7-11eb-bac0-309c2364fdb6','266bca8b-a5b5-11eb-bac0-309c2364fdb6','1.0','1','0','Unicode','2021-04-25 11:12:25',1,NULL,0,NULL,'2021-05-27 11:47:52'),('1d5f8445-a5b7-11eb-bac0-309c2364fdb6','266b1137-a5b5-11eb-bac0-309c2364fdb6','8.0.19','8.0','19','ANSI','2021-04-25 11:12:25',1,NULL,0,NULL,'2021-05-27 17:31:30'),('1d5f854b-a5b7-11eb-bac0-309c2364fdb6','266b1137-a5b5-11eb-bac0-309c2364fdb6','8.0.11','8.0','11','ANSI','2021-04-25 11:12:25',1,NULL,0,NULL,'2021-05-27 11:47:52'),('1d5f864e-a5b7-11eb-bac0-309c2364fdb6','266b9d70-a5b5-11eb-bac0-309c2364fdb6','12.2','12','2','ANSI','2021-04-25 11:12:25',1,NULL,0,NULL,'2021-05-27 11:47:52'),('254d6dee-f1f7-11eb-ad62-309c2364fdb6','266b1137-a5b5-11eb-bac0-309c2364fdb6','99.1','99','1','Test','2021-07-31 12:02:52',1,NULL,1,NULL,'2022-04-19 20:33:05');
/*!40000 ALTER TABLE `odbc_driver_version` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`rest`@`%`*/ /*!50003 TRIGGER `odbc_driver_version_BEFORE_INSERT` BEFORE INSERT ON `odbc_driver_version` FOR EACH ROW BEGIN 	SET new.creation_datetime = NOW();   END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`rest`@`%`*/ /*!50003 TRIGGER `odbc_driver_version_BEFORE_UPDATE` BEFORE UPDATE ON `odbc_driver_version` FOR EACH ROW BEGIN 	SET new.changed_datetime = NOW();END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `odbc_driver_version_driver_file`
--

DROP TABLE IF EXISTS `odbc_driver_version_driver_file`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `odbc_driver_version_driver_file` (
  `uuid` varchar(36) NOT NULL,
  `odbc_driver_version_uuid` varchar(36) NOT NULL,
  `operating_system_uuid` varchar(36) NOT NULL,
  `library_file` varchar(128) NOT NULL,
  `setup_file` varchar(128) NOT NULL,
  `library_file_hash` varchar(128) DEFAULT NULL,
  `setup_file_hash` varchar(128) DEFAULT NULL,
  `creation_datetime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `active` tinyint NOT NULL DEFAULT '1',
  `activation_datetime` datetime DEFAULT NULL,
  `deleted` tinyint DEFAULT '0',
  `deletion_datetime` datetime DEFAULT NULL,
  `changed_datetime` datetime DEFAULT NULL,
  PRIMARY KEY (`uuid`),
  KEY `fk_odbcDriverVersionToVersionOsArch_idx` (`odbc_driver_version_uuid`),
  KEY `fk_driverVersionFileToOs_idx` (`operating_system_uuid`),
  CONSTRAINT `fk_driverVersionFileToDriverVersion` FOREIGN KEY (`odbc_driver_version_uuid`) REFERENCES `odbc_driver_version` (`uuid`),
  CONSTRAINT `fk_driverVersionFileToOs` FOREIGN KEY (`operating_system_uuid`) REFERENCES `operating_system` (`uuid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `odbc_driver_version_driver_file`
--

LOCK TABLES `odbc_driver_version_driver_file` WRITE;
/*!40000 ALTER TABLE `odbc_driver_version_driver_file` DISABLE KEYS */;
INSERT INTO `odbc_driver_version_driver_file` VALUES ('ba59769b-bed6-11eb-8e36-309c2364fdb6','1d5f08ed-a5b7-11eb-bac0-309c2364fdb6','3feb2832-a5b5-11eb-bac0-309c2364fdb6','test.png','test2.png','D3D7403A0BF484518CD6E7B75E24530E','D3D7403A0BF484518CD6E7B75E24530E','2021-05-27 10:31:42',1,NULL,0,NULL,'2021-08-12 19:00:14'),('ba5a42e6-bed6-11eb-8e36-309c2364fdb6','1d5f8445-a5b7-11eb-bac0-309c2364fdb6','3feb2832-a5b5-11eb-bac0-309c2364fdb6','myodbc8w.dll','myodbc8S.dll',' ',NULL,'2021-05-27 10:31:42',1,NULL,0,NULL,'2021-06-08 20:31:38'),('ba5ac9e9-bed6-11eb-8e36-309c2364fdb6','1d5f7eb1-a5b7-11eb-bac0-309c2364fdb6','3feb2832-a5b5-11eb-bac0-309c2364fdb6','psqlodbc35w.dll','psqlodbc35w.dll',' ',NULL,'2021-05-27 10:31:42',1,NULL,0,NULL,'2021-06-08 20:31:38'),('ba5b45a3-bed6-11eb-8e36-309c2364fdb6','1d5f864e-a5b7-11eb-bac0-309c2364fdb6','3feb2832-a5b5-11eb-bac0-309c2364fdb6','psqlodbc35w.dll','psqlodbc35w.dll',' ',NULL,'2021-05-27 10:31:42',1,NULL,0,NULL,'2021-06-08 20:31:38'),('ba5bc4db-bed6-11eb-8e36-309c2364fdb6','1d5f80cc-a5b7-11eb-bac0-309c2364fdb6','3feb2832-a5b5-11eb-bac0-309c2364fdb6','myodbc8w.dll','myodbc8S.dll',' ',NULL,'2021-05-27 10:31:42',1,NULL,0,NULL,'2021-06-08 20:31:38'),('ba5c370f-bed6-11eb-8e36-309c2364fdb6','1d5f854b-a5b7-11eb-bac0-309c2364fdb6','3feb2832-a5b5-11eb-bac0-309c2364fdb6','myodbc8w.dll','myodbc8S.dll',' ',NULL,'2021-05-27 10:31:42',1,NULL,0,NULL,'2021-06-08 20:31:38'),('ba5cade4-bed6-11eb-8e36-309c2364fdb6','1d5f820d-a5b7-11eb-bac0-309c2364fdb6','6f192049-c881-11eb-a494-309c2364fdb6','odbc32.dll','odbc32.dll',' ',NULL,'2021-05-27 10:31:42',1,NULL,0,NULL,'2021-06-08 20:31:38'),('ba5d3884-bed6-11eb-8e36-309c2364fdb6','1d5f8336-a5b7-11eb-bac0-309c2364fdb6','6f192049-c881-11eb-a494-309c2364fdb6','odbc32.dll','odbc32.dll',' ',NULL,'2021-05-27 10:31:42',1,NULL,0,NULL,'2021-06-08 20:31:38'),('f0eb4898-f1f8-11eb-ad62-309c2364fdb6','254d6dee-f1f7-11eb-ad62-309c2364fdb6','6f192049-c881-11eb-a494-309c2364fdb6','test.dll','test.dll',' ',NULL,'2021-07-31 12:15:12',1,NULL,0,NULL,NULL);
/*!40000 ALTER TABLE `odbc_driver_version_driver_file` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `odbc_driver_version_file_osa_BEFORE_INSERT` BEFORE INSERT ON `odbc_driver_version_driver_file` FOR EACH ROW BEGIN
	SET new.creation_datetime = NOW();
    
END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `odbc_driver_version_file_osa_BEFORE_UPDATE` BEFORE UPDATE ON `odbc_driver_version_driver_file` FOR EACH ROW BEGIN
	
	SET new.changed_datetime = NOW();
END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `odbc_driver_version_relation_to_command`
--

DROP TABLE IF EXISTS `odbc_driver_version_relation_to_command`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `odbc_driver_version_relation_to_command` (
  `uuid` varchar(36) NOT NULL,
  `odbc_driver_version_uuid` varchar(36) NOT NULL,
  `schema_data_command_uuid` varchar(36) NOT NULL,
  `sql_statement_template` varchar(2048) NOT NULL,
  `creation_datetime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `active` tinyint NOT NULL DEFAULT '1',
  `activation_datetime` datetime DEFAULT NULL,
  `deleted` tinyint DEFAULT '0',
  `deletion_datetime` datetime DEFAULT NULL,
  `changed_datetime` datetime DEFAULT NULL,
  PRIMARY KEY (`odbc_driver_version_uuid`,`schema_data_command_uuid`),
  KEY `fk_odbcDriverVersionToVersion4_idx` (`odbc_driver_version_uuid`),
  KEY `fk_odbcDriverVersionToSchemaDataCommand_idx` (`schema_data_command_uuid`),
  CONSTRAINT `fk_odbcDriverVersionToSchemaDataCommand` FOREIGN KEY (`schema_data_command_uuid`) REFERENCES `schema_data_command` (`uuid`),
  CONSTRAINT `fk_odbcDriverVersionToVersion4` FOREIGN KEY (`odbc_driver_version_uuid`) REFERENCES `odbc_driver_version` (`uuid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `odbc_driver_version_relation_to_command`
--

LOCK TABLES `odbc_driver_version_relation_to_command` WRITE;
/*!40000 ALTER TABLE `odbc_driver_version_relation_to_command` DISABLE KEYS */;
/*!40000 ALTER TABLE `odbc_driver_version_relation_to_command` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`rest`@`%`*/ /*!50003 TRIGGER `odbc_driver_version_relation_to_command_BEFORE_INSERT` BEFORE INSERT ON `odbc_driver_version_relation_to_command` FOR EACH ROW BEGIN 	SET new.creation_datetime = NOW();   END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`rest`@`%`*/ /*!50003 TRIGGER `odbc_driver_version_relation_to_command_BEFORE_UPDATE` BEFORE UPDATE ON `odbc_driver_version_relation_to_command` FOR EACH ROW BEGIN 	SET new.changed_datetime = NOW();END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `odbc_driver_version_relation_to_connection_type`
--

DROP TABLE IF EXISTS `odbc_driver_version_relation_to_connection_type`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `odbc_driver_version_relation_to_connection_type` (
  `uuid` varchar(36) NOT NULL,
  `odbc_driver_version_uuid` varchar(36) NOT NULL,
  `connection_type_uuid` varchar(36) NOT NULL,
  `creation_datetime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `active` tinyint NOT NULL DEFAULT '1',
  `activation_datetime` datetime DEFAULT NULL,
  `deleted` tinyint DEFAULT '0',
  `deletion_datetime` datetime DEFAULT NULL,
  `changed_datetime` datetime DEFAULT NULL,
  PRIMARY KEY (`odbc_driver_version_uuid`,`connection_type_uuid`),
  KEY `fk_odbcDriverVersionToConnectionType_idx` (`connection_type_uuid`),
  KEY `fk_odbcDriverVersionToVersion_idx` (`odbc_driver_version_uuid`),
  CONSTRAINT `fk_odbcDriverVersionToConnectionType` FOREIGN KEY (`connection_type_uuid`) REFERENCES `connection_type` (`uuid`),
  CONSTRAINT `fk_odbcDriverVersionToVersion` FOREIGN KEY (`odbc_driver_version_uuid`) REFERENCES `odbc_driver_version` (`uuid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `odbc_driver_version_relation_to_connection_type`
--

LOCK TABLES `odbc_driver_version_relation_to_connection_type` WRITE;
/*!40000 ALTER TABLE `odbc_driver_version_relation_to_connection_type` DISABLE KEYS */;
INSERT INTO `odbc_driver_version_relation_to_connection_type` VALUES ('063a1b55-a5b8-11eb-bac0-309c2364fdb6','1d5f08ed-a5b7-11eb-bac0-309c2364fdb6','989ae37c-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:18:56',1,NULL,0,NULL,'2021-05-27 11:47:52'),('063a7491-a5b8-11eb-bac0-309c2364fdb6','1d5f08ed-a5b7-11eb-bac0-309c2364fdb6','989b33d8-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:18:56',1,NULL,0,NULL,'2021-05-27 11:47:52'),('063a76b1-a5b8-11eb-bac0-309c2364fdb6','1d5f08ed-a5b7-11eb-bac0-309c2364fdb6','989b3c0a-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:18:56',1,NULL,0,NULL,'2021-05-27 11:47:52'),('063a7805-a5b8-11eb-bac0-309c2364fdb6','1d5f08ed-a5b7-11eb-bac0-309c2364fdb6','989b5936-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:18:56',1,NULL,0,NULL,'2021-05-27 11:47:52'),('063a7942-a5b8-11eb-bac0-309c2364fdb6','1d5f7eb1-a5b7-11eb-bac0-309c2364fdb6','989ae37c-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:18:56',1,NULL,0,NULL,'2021-05-27 11:47:52'),('063a7a76-a5b8-11eb-bac0-309c2364fdb6','1d5f7eb1-a5b7-11eb-bac0-309c2364fdb6','989b33d8-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:18:56',1,NULL,0,NULL,'2021-05-27 11:47:52'),('063a7ba6-a5b8-11eb-bac0-309c2364fdb6','1d5f7eb1-a5b7-11eb-bac0-309c2364fdb6','989b3c0a-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:18:56',1,NULL,0,NULL,'2021-05-27 11:47:52'),('063a7cd3-a5b8-11eb-bac0-309c2364fdb6','1d5f7eb1-a5b7-11eb-bac0-309c2364fdb6','989b5936-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:18:56',1,NULL,0,NULL,'2021-05-27 11:47:52'),('063a7e1b-a5b8-11eb-bac0-309c2364fdb6','1d5f80cc-a5b7-11eb-bac0-309c2364fdb6','989ae37c-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:18:56',1,NULL,0,NULL,'2021-05-27 11:47:52'),('063a7f4e-a5b8-11eb-bac0-309c2364fdb6','1d5f80cc-a5b7-11eb-bac0-309c2364fdb6','989b33d8-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:18:56',1,NULL,0,NULL,'2021-05-27 11:47:52'),('063a806d-a5b8-11eb-bac0-309c2364fdb6','1d5f80cc-a5b7-11eb-bac0-309c2364fdb6','989b3c0a-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:18:56',1,NULL,0,NULL,'2021-05-27 11:47:52'),('063a8193-a5b8-11eb-bac0-309c2364fdb6','1d5f80cc-a5b7-11eb-bac0-309c2364fdb6','989b5936-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:18:56',1,NULL,0,NULL,'2021-05-27 11:47:52'),('063a82b7-a5b8-11eb-bac0-309c2364fdb6','1d5f820d-a5b7-11eb-bac0-309c2364fdb6','989b5015-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:18:56',1,NULL,0,NULL,'2021-05-27 11:47:52'),('063a83d1-a5b8-11eb-bac0-309c2364fdb6','1d5f8336-a5b7-11eb-bac0-309c2364fdb6','989b5015-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:18:56',1,NULL,0,NULL,'2021-05-27 11:47:52'),('063a84f8-a5b8-11eb-bac0-309c2364fdb6','1d5f8445-a5b7-11eb-bac0-309c2364fdb6','989ae37c-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:18:56',1,NULL,0,NULL,'2021-05-27 11:47:52'),('063a8612-a5b8-11eb-bac0-309c2364fdb6','1d5f8445-a5b7-11eb-bac0-309c2364fdb6','989b33d8-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:18:56',1,NULL,0,NULL,'2021-05-27 11:47:52'),('063a8732-a5b8-11eb-bac0-309c2364fdb6','1d5f8445-a5b7-11eb-bac0-309c2364fdb6','989b3c0a-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:18:56',1,NULL,0,NULL,'2021-05-27 11:47:52'),('063a8855-a5b8-11eb-bac0-309c2364fdb6','1d5f8445-a5b7-11eb-bac0-309c2364fdb6','989b5936-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:18:56',1,NULL,0,NULL,'2021-05-27 11:47:52'),('063a8997-a5b8-11eb-bac0-309c2364fdb6','1d5f854b-a5b7-11eb-bac0-309c2364fdb6','989ae37c-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:18:56',1,NULL,0,NULL,'2021-05-27 11:47:52'),('063a8ab9-a5b8-11eb-bac0-309c2364fdb6','1d5f854b-a5b7-11eb-bac0-309c2364fdb6','989b33d8-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:18:56',1,NULL,0,NULL,'2021-05-27 11:47:52'),('063a8bde-a5b8-11eb-bac0-309c2364fdb6','1d5f854b-a5b7-11eb-bac0-309c2364fdb6','989b3c0a-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:18:56',1,NULL,0,NULL,'2021-05-27 11:47:52'),('063a8d0a-a5b8-11eb-bac0-309c2364fdb6','1d5f854b-a5b7-11eb-bac0-309c2364fdb6','989b5936-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:18:56',1,NULL,0,NULL,'2021-05-27 11:47:52'),('e3dc8c90-f444-11eb-b474-309c2364fdb6','1d5f864e-a5b7-11eb-bac0-309c2364fdb6','989ae37c-a5b4-11eb-bac0-309c2364fdb6','2021-08-03 10:23:48',1,NULL,0,NULL,NULL),('e3de03df-f444-11eb-b474-309c2364fdb6','1d5f864e-a5b7-11eb-bac0-309c2364fdb6','989b33d8-a5b4-11eb-bac0-309c2364fdb6','2021-08-03 10:23:48',1,NULL,0,NULL,NULL),('e3dee0bf-f444-11eb-b474-309c2364fdb6','1d5f864e-a5b7-11eb-bac0-309c2364fdb6','989b3c0a-a5b4-11eb-bac0-309c2364fdb6','2021-08-03 10:23:48',1,NULL,0,NULL,NULL),('e3dfb352-f444-11eb-b474-309c2364fdb6','1d5f864e-a5b7-11eb-bac0-309c2364fdb6','989b5015-a5b4-11eb-bac0-309c2364fdb6','2021-08-03 10:23:48',1,NULL,0,NULL,NULL),('e3e08e47-f444-11eb-b474-309c2364fdb6','1d5f864e-a5b7-11eb-bac0-309c2364fdb6','989b5936-a5b4-11eb-bac0-309c2364fdb6','2021-08-03 10:23:48',1,NULL,0,NULL,NULL);
/*!40000 ALTER TABLE `odbc_driver_version_relation_to_connection_type` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`rest`@`%`*/ /*!50003 TRIGGER `odbc_driver_version_relation_to_connection_type_BEFORE_INSERT` BEFORE INSERT ON `odbc_driver_version_relation_to_connection_type` FOR EACH ROW BEGIN 	SET new.creation_datetime = NOW();   END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`rest`@`%`*/ /*!50003 TRIGGER `odbc_driver_version_relation_to_connection_type_BEFORE_UPDATE` BEFORE UPDATE ON `odbc_driver_version_relation_to_connection_type` FOR EACH ROW BEGIN 	SET new.changed_datetime = NOW();END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `odbc_driver_version_relation_to_connectionstring_values`
--

DROP TABLE IF EXISTS `odbc_driver_version_relation_to_connectionstring_values`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `odbc_driver_version_relation_to_connectionstring_values` (
  `uuid` varchar(36) NOT NULL,
  `odbc_driver_version_uuid` varchar(36) NOT NULL,
  `connectionstring_values_uuid` varchar(36) NOT NULL,
  `creation_datetime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `active` tinyint NOT NULL DEFAULT '1',
  `activation_datetime` datetime DEFAULT NULL,
  `deleted` tinyint DEFAULT '0',
  `deletion_datetime` datetime DEFAULT NULL,
  `changed_datetime` datetime DEFAULT NULL,
  PRIMARY KEY (`odbc_driver_version_uuid`,`connectionstring_values_uuid`),
  KEY `fk_odbcDriverVersionToConnectionStringValues_idx` (`connectionstring_values_uuid`),
  KEY `fk_odbcDriverVersionToVersion2_idx` (`odbc_driver_version_uuid`),
  CONSTRAINT `fk_odbcDriverVersionToConnectionStringValues` FOREIGN KEY (`connectionstring_values_uuid`) REFERENCES `connectionstring_values` (`uuid`),
  CONSTRAINT `fk_odbcDriverVersionToVersion2` FOREIGN KEY (`odbc_driver_version_uuid`) REFERENCES `odbc_driver_version` (`uuid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `odbc_driver_version_relation_to_connectionstring_values`
--

LOCK TABLES `odbc_driver_version_relation_to_connectionstring_values` WRITE;
/*!40000 ALTER TABLE `odbc_driver_version_relation_to_connectionstring_values` DISABLE KEYS */;
INSERT INTO `odbc_driver_version_relation_to_connectionstring_values` VALUES ('f768f01e-a5ba-11eb-bac0-309c2364fdb6','1d5f08ed-a5b7-11eb-bac0-309c2364fdb6','eda913fd-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f769564c-a5ba-11eb-bac0-309c2364fdb6','1d5f08ed-a5b7-11eb-bac0-309c2364fdb6','eda97394-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f7695807-a5ba-11eb-bac0-309c2364fdb6','1d5f08ed-a5b7-11eb-bac0-309c2364fdb6','eda9762d-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f769593f-a5ba-11eb-bac0-309c2364fdb6','1d5f08ed-a5b7-11eb-bac0-309c2364fdb6','eda97726-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f7695a74-a5ba-11eb-bac0-309c2364fdb6','1d5f08ed-a5b7-11eb-bac0-309c2364fdb6','eda977ff-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f7695b9a-a5ba-11eb-bac0-309c2364fdb6','1d5f08ed-a5b7-11eb-bac0-309c2364fdb6','eda978c6-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f7695cc2-a5ba-11eb-bac0-309c2364fdb6','1d5f08ed-a5b7-11eb-bac0-309c2364fdb6','eda99d69-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f7695dca-a5ba-11eb-bac0-309c2364fdb6','1d5f08ed-a5b7-11eb-bac0-309c2364fdb6','eda99f11-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f7695edc-a5ba-11eb-bac0-309c2364fdb6','1d5f08ed-a5b7-11eb-bac0-309c2364fdb6','eda99ff9-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f7696003-a5ba-11eb-bac0-309c2364fdb6','1d5f08ed-a5b7-11eb-bac0-309c2364fdb6','eda9a0cc-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f769610c-a5ba-11eb-bac0-309c2364fdb6','1d5f08ed-a5b7-11eb-bac0-309c2364fdb6','eda9a1a8-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f7696211-a5ba-11eb-bac0-309c2364fdb6','1d5f08ed-a5b7-11eb-bac0-309c2364fdb6','eda9a272-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f7696315-a5ba-11eb-bac0-309c2364fdb6','1d5f08ed-a5b7-11eb-bac0-309c2364fdb6','eda9a338-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f7696419-a5ba-11eb-bac0-309c2364fdb6','1d5f08ed-a5b7-11eb-bac0-309c2364fdb6','eda9a406-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f769651e-a5ba-11eb-bac0-309c2364fdb6','1d5f08ed-a5b7-11eb-bac0-309c2364fdb6','eda9a4ca-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f769661d-a5ba-11eb-bac0-309c2364fdb6','1d5f7eb1-a5b7-11eb-bac0-309c2364fdb6','eda913fd-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f7696726-a5ba-11eb-bac0-309c2364fdb6','1d5f7eb1-a5b7-11eb-bac0-309c2364fdb6','eda97394-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f769682f-a5ba-11eb-bac0-309c2364fdb6','1d5f7eb1-a5b7-11eb-bac0-309c2364fdb6','eda9762d-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f769694b-a5ba-11eb-bac0-309c2364fdb6','1d5f7eb1-a5b7-11eb-bac0-309c2364fdb6','eda97726-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f7696a55-a5ba-11eb-bac0-309c2364fdb6','1d5f7eb1-a5b7-11eb-bac0-309c2364fdb6','eda977ff-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f7696b66-a5ba-11eb-bac0-309c2364fdb6','1d5f7eb1-a5b7-11eb-bac0-309c2364fdb6','eda978c6-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f7696c69-a5ba-11eb-bac0-309c2364fdb6','1d5f7eb1-a5b7-11eb-bac0-309c2364fdb6','eda99d69-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f7696d74-a5ba-11eb-bac0-309c2364fdb6','1d5f7eb1-a5b7-11eb-bac0-309c2364fdb6','eda99f11-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f7696e79-a5ba-11eb-bac0-309c2364fdb6','1d5f7eb1-a5b7-11eb-bac0-309c2364fdb6','eda99ff9-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f7696f88-a5ba-11eb-bac0-309c2364fdb6','1d5f7eb1-a5b7-11eb-bac0-309c2364fdb6','eda9a0cc-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f76970b8-a5ba-11eb-bac0-309c2364fdb6','1d5f7eb1-a5b7-11eb-bac0-309c2364fdb6','eda9a1a8-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f76971bf-a5ba-11eb-bac0-309c2364fdb6','1d5f7eb1-a5b7-11eb-bac0-309c2364fdb6','eda9a272-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f76972ef-a5ba-11eb-bac0-309c2364fdb6','1d5f7eb1-a5b7-11eb-bac0-309c2364fdb6','eda9a338-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f7697408-a5ba-11eb-bac0-309c2364fdb6','1d5f7eb1-a5b7-11eb-bac0-309c2364fdb6','eda9a406-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f7697513-a5ba-11eb-bac0-309c2364fdb6','1d5f7eb1-a5b7-11eb-bac0-309c2364fdb6','eda9a4ca-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f769762b-a5ba-11eb-bac0-309c2364fdb6','1d5f80cc-a5b7-11eb-bac0-309c2364fdb6','eda913fd-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f7697744-a5ba-11eb-bac0-309c2364fdb6','1d5f80cc-a5b7-11eb-bac0-309c2364fdb6','eda97394-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f7697852-a5ba-11eb-bac0-309c2364fdb6','1d5f80cc-a5b7-11eb-bac0-309c2364fdb6','eda9762d-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f76979b0-a5ba-11eb-bac0-309c2364fdb6','1d5f80cc-a5b7-11eb-bac0-309c2364fdb6','eda97726-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f7697ac2-a5ba-11eb-bac0-309c2364fdb6','1d5f80cc-a5b7-11eb-bac0-309c2364fdb6','eda977ff-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f7697bea-a5ba-11eb-bac0-309c2364fdb6','1d5f80cc-a5b7-11eb-bac0-309c2364fdb6','eda978c6-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f7697cff-a5ba-11eb-bac0-309c2364fdb6','1d5f80cc-a5b7-11eb-bac0-309c2364fdb6','eda99d69-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f7697e08-a5ba-11eb-bac0-309c2364fdb6','1d5f80cc-a5b7-11eb-bac0-309c2364fdb6','eda99f11-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f7697f15-a5ba-11eb-bac0-309c2364fdb6','1d5f80cc-a5b7-11eb-bac0-309c2364fdb6','eda99ff9-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f7698026-a5ba-11eb-bac0-309c2364fdb6','1d5f80cc-a5b7-11eb-bac0-309c2364fdb6','eda9a0cc-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f7698134-a5ba-11eb-bac0-309c2364fdb6','1d5f80cc-a5b7-11eb-bac0-309c2364fdb6','eda9a1a8-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f769823f-a5ba-11eb-bac0-309c2364fdb6','1d5f80cc-a5b7-11eb-bac0-309c2364fdb6','eda9a272-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f769839d-a5ba-11eb-bac0-309c2364fdb6','1d5f80cc-a5b7-11eb-bac0-309c2364fdb6','eda9a338-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f76985aa-a5ba-11eb-bac0-309c2364fdb6','1d5f80cc-a5b7-11eb-bac0-309c2364fdb6','eda9a406-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f76986fb-a5ba-11eb-bac0-309c2364fdb6','1d5f80cc-a5b7-11eb-bac0-309c2364fdb6','eda9a4ca-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f7698837-a5ba-11eb-bac0-309c2364fdb6','1d5f820d-a5b7-11eb-bac0-309c2364fdb6','eda97a54-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f7698981-a5ba-11eb-bac0-309c2364fdb6','1d5f820d-a5b7-11eb-bac0-309c2364fdb6','eda98332-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f7698a9b-a5ba-11eb-bac0-309c2364fdb6','1d5f8336-a5b7-11eb-bac0-309c2364fdb6','eda97a54-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f7698ba6-a5ba-11eb-bac0-309c2364fdb6','1d5f8336-a5b7-11eb-bac0-309c2364fdb6','eda98332-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f7698cad-a5ba-11eb-bac0-309c2364fdb6','1d5f8445-a5b7-11eb-bac0-309c2364fdb6','eda913fd-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f7698db4-a5ba-11eb-bac0-309c2364fdb6','1d5f8445-a5b7-11eb-bac0-309c2364fdb6','eda97394-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f7698ec0-a5ba-11eb-bac0-309c2364fdb6','1d5f8445-a5b7-11eb-bac0-309c2364fdb6','eda9762d-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f7698fcf-a5ba-11eb-bac0-309c2364fdb6','1d5f8445-a5b7-11eb-bac0-309c2364fdb6','eda97726-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f769912c-a5ba-11eb-bac0-309c2364fdb6','1d5f8445-a5b7-11eb-bac0-309c2364fdb6','eda977ff-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f7699230-a5ba-11eb-bac0-309c2364fdb6','1d5f8445-a5b7-11eb-bac0-309c2364fdb6','eda978c6-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f7699337-a5ba-11eb-bac0-309c2364fdb6','1d5f8445-a5b7-11eb-bac0-309c2364fdb6','eda99d69-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f7699441-a5ba-11eb-bac0-309c2364fdb6','1d5f8445-a5b7-11eb-bac0-309c2364fdb6','eda99f11-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f7699546-a5ba-11eb-bac0-309c2364fdb6','1d5f8445-a5b7-11eb-bac0-309c2364fdb6','eda99ff9-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f769964d-a5ba-11eb-bac0-309c2364fdb6','1d5f8445-a5b7-11eb-bac0-309c2364fdb6','eda9a0cc-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f7699768-a5ba-11eb-bac0-309c2364fdb6','1d5f8445-a5b7-11eb-bac0-309c2364fdb6','eda9a1a8-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f769987f-a5ba-11eb-bac0-309c2364fdb6','1d5f8445-a5b7-11eb-bac0-309c2364fdb6','eda9a272-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f7699989-a5ba-11eb-bac0-309c2364fdb6','1d5f8445-a5b7-11eb-bac0-309c2364fdb6','eda9a338-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f7699a8f-a5ba-11eb-bac0-309c2364fdb6','1d5f8445-a5b7-11eb-bac0-309c2364fdb6','eda9a406-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f7699b99-a5ba-11eb-bac0-309c2364fdb6','1d5f8445-a5b7-11eb-bac0-309c2364fdb6','eda9a4ca-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f7699ca9-a5ba-11eb-bac0-309c2364fdb6','1d5f854b-a5b7-11eb-bac0-309c2364fdb6','eda913fd-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f7699dba-a5ba-11eb-bac0-309c2364fdb6','1d5f854b-a5b7-11eb-bac0-309c2364fdb6','eda97394-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f7699ec4-a5ba-11eb-bac0-309c2364fdb6','1d5f854b-a5b7-11eb-bac0-309c2364fdb6','eda9762d-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f7699fc8-a5ba-11eb-bac0-309c2364fdb6','1d5f854b-a5b7-11eb-bac0-309c2364fdb6','eda97726-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f769a0f9-a5ba-11eb-bac0-309c2364fdb6','1d5f854b-a5b7-11eb-bac0-309c2364fdb6','eda977ff-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f769a204-a5ba-11eb-bac0-309c2364fdb6','1d5f854b-a5b7-11eb-bac0-309c2364fdb6','eda978c6-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f769a347-a5ba-11eb-bac0-309c2364fdb6','1d5f854b-a5b7-11eb-bac0-309c2364fdb6','eda99d69-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f769a45a-a5ba-11eb-bac0-309c2364fdb6','1d5f854b-a5b7-11eb-bac0-309c2364fdb6','eda99f11-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f769a56a-a5ba-11eb-bac0-309c2364fdb6','1d5f854b-a5b7-11eb-bac0-309c2364fdb6','eda99ff9-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f769a673-a5ba-11eb-bac0-309c2364fdb6','1d5f854b-a5b7-11eb-bac0-309c2364fdb6','eda9a0cc-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f769a780-a5ba-11eb-bac0-309c2364fdb6','1d5f854b-a5b7-11eb-bac0-309c2364fdb6','eda9a1a8-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f769a888-a5ba-11eb-bac0-309c2364fdb6','1d5f854b-a5b7-11eb-bac0-309c2364fdb6','eda9a272-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f769a999-a5ba-11eb-bac0-309c2364fdb6','1d5f854b-a5b7-11eb-bac0-309c2364fdb6','eda9a338-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f769aaa3-a5ba-11eb-bac0-309c2364fdb6','1d5f854b-a5b7-11eb-bac0-309c2364fdb6','eda9a406-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f769abb2-a5ba-11eb-bac0-309c2364fdb6','1d5f854b-a5b7-11eb-bac0-309c2364fdb6','eda9a4ca-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f769acc9-a5ba-11eb-bac0-309c2364fdb6','1d5f864e-a5b7-11eb-bac0-309c2364fdb6','eda913fd-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f769add3-a5ba-11eb-bac0-309c2364fdb6','1d5f864e-a5b7-11eb-bac0-309c2364fdb6','eda97394-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f769aedb-a5ba-11eb-bac0-309c2364fdb6','1d5f864e-a5b7-11eb-bac0-309c2364fdb6','eda9762d-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f769afed-a5ba-11eb-bac0-309c2364fdb6','1d5f864e-a5b7-11eb-bac0-309c2364fdb6','eda97726-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f769b0fb-a5ba-11eb-bac0-309c2364fdb6','1d5f864e-a5b7-11eb-bac0-309c2364fdb6','eda977ff-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f769b20b-a5ba-11eb-bac0-309c2364fdb6','1d5f864e-a5b7-11eb-bac0-309c2364fdb6','eda978c6-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f769b314-a5ba-11eb-bac0-309c2364fdb6','1d5f864e-a5b7-11eb-bac0-309c2364fdb6','eda99d69-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f769b425-a5ba-11eb-bac0-309c2364fdb6','1d5f864e-a5b7-11eb-bac0-309c2364fdb6','eda99f11-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f769b53b-a5ba-11eb-bac0-309c2364fdb6','1d5f864e-a5b7-11eb-bac0-309c2364fdb6','eda99ff9-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f769b651-a5ba-11eb-bac0-309c2364fdb6','1d5f864e-a5b7-11eb-bac0-309c2364fdb6','eda9a0cc-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f769b77a-a5ba-11eb-bac0-309c2364fdb6','1d5f864e-a5b7-11eb-bac0-309c2364fdb6','eda9a1a8-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f769b886-a5ba-11eb-bac0-309c2364fdb6','1d5f864e-a5b7-11eb-bac0-309c2364fdb6','eda9a272-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f769b9d6-a5ba-11eb-bac0-309c2364fdb6','1d5f864e-a5b7-11eb-bac0-309c2364fdb6','eda9a338-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f769baed-a5ba-11eb-bac0-309c2364fdb6','1d5f864e-a5b7-11eb-bac0-309c2364fdb6','eda9a406-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23'),('f769bbf9-a5ba-11eb-bac0-309c2364fdb6','1d5f864e-a5b7-11eb-bac0-309c2364fdb6','eda9a4ca-a5b4-11eb-bac0-309c2364fdb6','2021-04-25 11:39:59',1,NULL,0,NULL,'2021-05-27 11:47:23');
/*!40000 ALTER TABLE `odbc_driver_version_relation_to_connectionstring_values` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `odbc_driver_version_csv_BEFORE_INSERT` BEFORE INSERT ON `odbc_driver_version_relation_to_connectionstring_values` FOR EACH ROW BEGIN

	SET new.creation_datetime = NOW();
    
END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `odbc_driver_version_csv_BEFORE_UPDATE` BEFORE UPDATE ON `odbc_driver_version_relation_to_connectionstring_values` FOR EACH ROW BEGIN

	SET new.changed_datetime = NOW();
END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `operating_system`
--

DROP TABLE IF EXISTS `operating_system`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `operating_system` (
  `uuid` varchar(36) NOT NULL,
  `name` varchar(90) DEFAULT NULL,
  `creation_datetime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `active` tinyint NOT NULL DEFAULT '1',
  `activation_datetime` datetime DEFAULT NULL,
  `deleted` tinyint DEFAULT '0',
  `deletion_datetime` datetime DEFAULT NULL,
  `changed_datetime` datetime DEFAULT NULL,
  PRIMARY KEY (`uuid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `operating_system`
--

LOCK TABLES `operating_system` WRITE;
/*!40000 ALTER TABLE `operating_system` DISABLE KEYS */;
INSERT INTO `operating_system` VALUES ('3feb0b44-a5b5-11eb-bac0-309c2364fdb6','Linux x64','2021-04-25 10:59:04',1,NULL,0,NULL,'2021-06-08 19:46:20'),('3feb26c5-a5b5-11eb-bac0-309c2364fdb6','Mac OS x64','2021-04-25 10:59:04',1,NULL,0,NULL,'2021-06-08 19:46:20'),('3feb2832-a5b5-11eb-bac0-309c2364fdb6','Windows x64','2021-04-25 10:59:04',1,NULL,0,NULL,'2021-06-08 19:46:20'),('6f190273-c881-11eb-a494-309c2364fdb6','Linux x86','2021-06-08 17:46:20',1,NULL,0,NULL,'2021-05-27 11:46:17'),('6f1917b7-c881-11eb-a494-309c2364fdb6','Mac OS x86','2021-06-08 17:46:20',1,NULL,0,NULL,'2021-05-27 11:46:17'),('6f192049-c881-11eb-a494-309c2364fdb6','Windows x86','2021-06-08 17:46:20',1,NULL,0,NULL,'2021-05-27 11:46:17');
/*!40000 ALTER TABLE `operating_system` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`rest`@`%`*/ /*!50003 TRIGGER `operating_system_BEFORE_INSERT` BEFORE INSERT ON `operating_system` FOR EACH ROW BEGIN 	SET new.creation_datetime = NOW();   END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`rest`@`%`*/ /*!50003 TRIGGER `operating_system_BEFORE_UPDATE` BEFORE UPDATE ON `operating_system` FOR EACH ROW BEGIN 	SET new.changed_datetime = NOW();END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `role`
--

DROP TABLE IF EXISTS `role`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `role` (
  `uuid` varchar(36) NOT NULL,
  `name` varchar(45) NOT NULL,
  `description` varchar(128) NOT NULL,
  `creation_datetime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `active` tinyint NOT NULL DEFAULT '1',
  `activation_datetime` datetime DEFAULT NULL,
  `deleted` tinyint DEFAULT '0',
  `deletion_datetime` datetime DEFAULT NULL,
  `changed_datetime` datetime DEFAULT NULL,
  `max_request_per_hour` int NOT NULL DEFAULT '200',
  `max_time_after_request_in_ms` int NOT NULL DEFAULT '1000',
  PRIMARY KEY (`uuid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `role`
--

LOCK TABLES `role` WRITE;
/*!40000 ALTER TABLE `role` DISABLE KEYS */;
INSERT INTO `role` VALUES ('2a9e2014-1ca4-11ec-a4a4-d8bbc10f2ae0','root','Root Role','2021-09-23 19:26:37',1,NULL,0,NULL,NULL,200,1000),('4c155424-a5b5-11eb-bac0-309c2364fdb6','Admin','Adminrole','2021-04-25 10:59:24',1,NULL,0,NULL,'2021-05-27 11:46:02',-1,-1),('4c15b437-a5b5-11eb-bac0-309c2364fdb6','User','Userrole','2021-04-25 10:59:24',1,NULL,0,NULL,'2021-05-27 11:46:02',-1,-1),('79e083c6-2f31-11ec-a420-d8bbc10f2ae0','anonymous','Root Role','2021-10-17 10:03:28',1,NULL,0,NULL,NULL,200,1000);
/*!40000 ALTER TABLE `role` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`rest`@`%`*/ /*!50003 TRIGGER `role_BEFORE_INSERT` BEFORE INSERT ON `role` FOR EACH ROW BEGIN 	SET new.creation_datetime = NOW();   END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`rest`@`%`*/ /*!50003 TRIGGER `role_BEFORE_UPDATE` BEFORE UPDATE ON `role` FOR EACH ROW BEGIN 	SET new.changed_datetime = NOW();END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `schema_data_command`
--

DROP TABLE IF EXISTS `schema_data_command`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `schema_data_command` (
  `uuid` varchar(36) NOT NULL,
  `name` varchar(1024) NOT NULL,
  `creation_datetime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `active` tinyint NOT NULL DEFAULT '1',
  `activation_datetime` datetime DEFAULT NULL,
  `deleted` tinyint DEFAULT '0',
  `deletion_datetime` datetime DEFAULT NULL,
  `changed_datetime` datetime DEFAULT NULL,
  PRIMARY KEY (`uuid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `schema_data_command`
--

LOCK TABLES `schema_data_command` WRITE;
/*!40000 ALTER TABLE `schema_data_command` DISABLE KEYS */;
INSERT INTO `schema_data_command` VALUES ('58159a03-a5b5-11eb-bac0-309c2364fdb6','GET_SCHEMAS','2021-04-25 10:59:45',1,NULL,0,NULL,'2021-05-27 11:45:15'),('58160a11-a5b5-11eb-bac0-309c2364fdb6','GET_TABLES','2021-04-25 10:59:45',1,NULL,0,NULL,'2021-05-27 11:45:15'),('58160b7d-a5b5-11eb-bac0-309c2364fdb6','GET_VIEWS','2021-04-25 10:59:45',1,NULL,0,NULL,'2021-05-27 11:45:15'),('58160c51-a5b5-11eb-bac0-309c2364fdb6','GET_FUNCTIONS','2021-04-25 10:59:45',1,NULL,0,NULL,'2021-05-27 11:45:15'),('58160d17-a5b5-11eb-bac0-309c2364fdb6','GET_PROCEDURES','2021-04-25 10:59:45',1,NULL,0,NULL,'2021-05-27 11:45:15'),('58160dde-a5b5-11eb-bac0-309c2364fdb6','GET_TRIGGERS','2021-04-25 10:59:45',1,NULL,0,NULL,'2021-05-27 11:45:15'),('58160e9c-a5b5-11eb-bac0-309c2364fdb6','GET_TABLE_COLUMNS','2021-04-25 10:59:45',1,NULL,0,NULL,'2021-05-27 11:45:15'),('58160f59-a5b5-11eb-bac0-309c2364fdb6','GET_TABLE_KEYS','2021-04-25 10:59:45',1,NULL,0,NULL,'2021-05-27 11:45:15'),('5816101f-a5b5-11eb-bac0-309c2364fdb6','GET_SERVER_STATUS_BYVAR','2021-04-25 10:59:45',1,NULL,0,NULL,'2021-05-27 11:45:15'),('5816120f-a5b5-11eb-bac0-309c2364fdb6','GET_SERVER_STATUS_ALL','2021-04-25 10:59:45',1,NULL,0,NULL,'2021-05-27 11:45:15'),('58161340-a5b5-11eb-bac0-309c2364fdb6','GET_TABLE_CONSTRAINTS','2021-04-25 10:59:45',1,NULL,0,NULL,'2021-05-27 11:45:15');
/*!40000 ALTER TABLE `schema_data_command` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`rest`@`%`*/ /*!50003 TRIGGER `schema_data_command_BEFORE_INSERT` BEFORE INSERT ON `schema_data_command` FOR EACH ROW BEGIN 	SET new.creation_datetime = NOW();   END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`rest`@`%`*/ /*!50003 TRIGGER `schema_data_command_BEFORE_UPDATE` BEFORE UPDATE ON `schema_data_command` FOR EACH ROW BEGIN 	SET new.changed_datetime = NOW();END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `software`
--

DROP TABLE IF EXISTS `software`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `software` (
  `uuid` varchar(36) NOT NULL,
  `name` varchar(90) NOT NULL,
  `description` varchar(256) NOT NULL,
  `creation_datetime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `active` tinyint NOT NULL DEFAULT '1',
  `activation_datetime` datetime DEFAULT NULL,
  `deleted` tinyint DEFAULT '0',
  `deletion_datetime` datetime DEFAULT NULL,
  `changed_datetime` datetime DEFAULT NULL,
  PRIMARY KEY (`uuid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `software`
--

LOCK TABLES `software` WRITE;
/*!40000 ALTER TABLE `software` DISABLE KEYS */;
INSERT INTO `software` VALUES ('5f0f1048-a5b5-11eb-bac0-309c2364fdb6','HelixDBM','Helix Database Modelling and Management Software','2021-04-25 10:59:56',1,NULL,0,NULL,'2021-05-13 23:57:21'),('5f0f25c1-a5b5-11eb-bac0-309c2364fdb6','test222222','test33333','2021-04-25 10:59:56',1,NULL,0,NULL,'2021-07-31 14:25:20'),('7afbb295-f6a1-11eb-8281-309c2364fdb6','neuanlageooooty','testbeschreibung','2021-08-06 10:31:37',1,NULL,0,NULL,NULL),('855b6997-f69f-11eb-8281-309c2364fdb6','neuanlageoooob','testbeschreibung','2021-08-06 10:17:36',1,NULL,0,NULL,NULL),('b8c6a80d-f44a-11eb-b474-309c2364fdb6','neuanlageoooobbb','neuebeschreibung','2021-08-03 11:05:32',1,NULL,0,NULL,'2022-04-19 23:19:31'),('f5840781-bff8-11ec-8e16-7085c294413b','meineneuanlage','testbeschreibung','2022-04-19 15:54:13',1,NULL,0,NULL,NULL),('f59a565b-f6a0-11eb-8281-309c2364fdb6','neuanlageooootz','testbeschreibung','2021-08-06 10:27:54',1,NULL,0,NULL,NULL);
/*!40000 ALTER TABLE `software` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`rest`@`%`*/ /*!50003 TRIGGER `software_BEFORE_INSERT` BEFORE INSERT ON `software` FOR EACH ROW BEGIN 	SET new.creation_datetime = NOW();   END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`rest`@`%`*/ /*!50003 TRIGGER `software_BEFORE_UPDATE` BEFORE UPDATE ON `software` FOR EACH ROW BEGIN 	SET new.changed_datetime = NOW();END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `software_version`
--

DROP TABLE IF EXISTS `software_version`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `software_version` (
  `uuid` varchar(36) NOT NULL,
  `software_uuid` varchar(36) DEFAULT NULL,
  `version_type_uuid` varchar(36) DEFAULT NULL,
  `version_internal` varchar(45) NOT NULL,
  `version_subversion_internal` varchar(45) NOT NULL,
  `release_note` text NOT NULL,
  `file_extension` varchar(45) DEFAULT NULL,
  `hash` varchar(128) DEFAULT NULL,
  `creation_datetime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `active` tinyint NOT NULL DEFAULT '1',
  `activation_datetime` datetime DEFAULT NULL,
  `deleted` tinyint DEFAULT '0',
  `deletion_datetime` datetime DEFAULT NULL,
  `changed_datetime` datetime DEFAULT NULL,
  PRIMARY KEY (`uuid`),
  KEY `fk_softwareVersionToSoftware_idx` (`software_uuid`),
  KEY `fk_softwareVersionToVersionType_idx` (`version_type_uuid`),
  CONSTRAINT `fk_softwareVersionToSoftware` FOREIGN KEY (`software_uuid`) REFERENCES `software` (`uuid`),
  CONSTRAINT `fk_softwareVersionToVersionType` FOREIGN KEY (`version_type_uuid`) REFERENCES `version_type` (`uuid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `software_version`
--

LOCK TABLES `software_version` WRITE;
/*!40000 ALTER TABLE `software_version` DISABLE KEYS */;
INSERT INTO `software_version` VALUES ('0664c47a-f793-11eb-9131-309c2364fdb6','f59a565b-f6a0-11eb-8281-309c2364fdb6','78ee5778-a5b5-11eb-bac0-309c2364fdb6','0','2','aBCdef',NULL,'v','2021-08-07 15:20:40',1,NULL,0,NULL,'2021-05-27 11:44:45'),('0a514fc7-f792-11eb-9131-309c2364fdb6','f59a565b-f6a0-11eb-8281-309c2364fdb6','78ee5778-a5b5-11eb-bac0-309c2364fdb6','0','2','aBCdef',NULL,'v','2021-08-07 15:13:37',1,NULL,0,NULL,'2021-05-27 11:44:45'),('0c69426e-f7a1-11eb-9fdf-309c2364fdb6','f59a565b-f6a0-11eb-8281-309c2364fdb6','78ee5778-a5b5-11eb-bac0-309c2364fdb6','0','2','aBCdefghijkhkhhgjfg',NULL,'v','2021-08-07 17:01:03',1,NULL,0,NULL,'2021-05-27 11:44:45'),('0e3d0527-f798-11eb-9131-309c2364fdb6','f59a565b-f6a0-11eb-8281-309c2364fdb6','78ee5778-a5b5-11eb-bac0-309c2364fdb6','0','2','aBCdefg',NULL,'v','2021-08-07 15:56:41',1,NULL,0,NULL,'2021-05-27 11:44:45'),('106afedc-f79a-11eb-9fdf-309c2364fdb6','f59a565b-f6a0-11eb-8281-309c2364fdb6','78ee5778-a5b5-11eb-bac0-309c2364fdb6','0','2','aBCdefghijkh',NULL,'v','2021-08-07 16:11:03',1,NULL,0,NULL,'2021-05-27 11:44:45'),('178d0511-f798-11eb-9131-309c2364fdb6','f59a565b-f6a0-11eb-8281-309c2364fdb6','78ee5778-a5b5-11eb-bac0-309c2364fdb6','0','2','aBCdefgh',NULL,'v','2021-08-07 15:56:56',1,NULL,0,NULL,'2021-05-27 11:44:45'),('1a25aaff-f799-11eb-9131-309c2364fdb6','f59a565b-f6a0-11eb-8281-309c2364fdb6','78ee5778-a5b5-11eb-bac0-309c2364fdb6','0','2','aBCdefghijk',NULL,'v','2021-08-07 16:04:10',1,NULL,0,NULL,'2021-05-27 11:44:45'),('2d944d16-f792-11eb-9131-309c2364fdb6','f59a565b-f6a0-11eb-8281-309c2364fdb6','78ee5778-a5b5-11eb-bac0-309c2364fdb6','0','2','aBCdef',NULL,'v','2021-08-07 15:14:36',1,NULL,0,NULL,'2021-05-27 11:44:45'),('3494cfad-f7a0-11eb-9fdf-309c2364fdb6','f59a565b-f6a0-11eb-8281-309c2364fdb6','78ee5778-a5b5-11eb-bac0-309c2364fdb6','0','2','aBCdefghijkhkhhgjg',NULL,'v','2021-08-07 16:55:01',1,NULL,0,NULL,'2021-05-27 11:44:45'),('4061a07e-f7a4-11eb-9fdf-309c2364fdb6','f59a565b-f6a0-11eb-8281-309c2364fdb6','78ee5778-a5b5-11eb-bac0-309c2364fdb6','0','2','aBCdefghijkfhfffkhhgjfg',NULL,'v','2021-08-07 17:23:59',1,NULL,0,NULL,'2021-05-27 11:44:45'),('41a28deb-f79e-11eb-9fdf-309c2364fdb6','f59a565b-f6a0-11eb-8281-309c2364fdb6','78ee5778-a5b5-11eb-bac0-309c2364fdb6','0','2','aBCdefghijkhkhh',NULL,'v','2021-08-07 16:41:04',1,NULL,0,NULL,'2021-05-27 11:44:45'),('41b0e18c-f79d-11eb-9fdf-309c2364fdb6','f59a565b-f6a0-11eb-8281-309c2364fdb6','78ee5778-a5b5-11eb-bac0-309c2364fdb6','0','2','aBCdefghijkhk',NULL,'v','2021-08-07 16:33:54',1,NULL,0,NULL,'2021-05-27 11:44:45'),('490e9594-f794-11eb-9131-309c2364fdb6','f59a565b-f6a0-11eb-8281-309c2364fdb6','78ee5778-a5b5-11eb-bac0-309c2364fdb6','0','2','aBCdef',NULL,'v','2021-08-07 15:29:41',1,NULL,0,NULL,'2021-05-27 11:44:45'),('4a12d695-f790-11eb-9131-309c2364fdb6','f59a565b-f6a0-11eb-8281-309c2364fdb6','78ee5778-a5b5-11eb-bac0-309c2364fdb6','0','2','aBCd',NULL,'v','2021-08-07 15:01:05',1,NULL,0,NULL,'2021-05-27 11:44:45'),('518067f9-f790-11eb-9131-309c2364fdb6','f59a565b-f6a0-11eb-8281-309c2364fdb6','78ee5778-a5b5-11eb-bac0-309c2364fdb6','0','2','aBCde',NULL,'v','2021-08-07 15:01:17',1,NULL,0,NULL,'2021-05-27 11:44:45'),('57115adc-f7a5-11eb-9fdf-309c2364fdb6','f59a565b-f6a0-11eb-8281-309c2364fdb6','78ee5778-a5b5-11eb-bac0-309c2364fdb6','0','2','aBCdefghijfkfhfffkhhgjffg',NULL,'v','2021-08-07 17:31:46',1,NULL,0,NULL,'2021-05-27 11:44:45'),('59a9a526-f79b-11eb-9fdf-309c2364fdb6','f59a565b-f6a0-11eb-8281-309c2364fdb6','78ee5778-a5b5-11eb-bac0-309c2364fdb6','0','2','aBCdefghijkh',NULL,'v','2021-08-07 16:20:16',1,NULL,0,NULL,'2021-05-27 11:44:45'),('5c5371fa-f790-11eb-9131-309c2364fdb6','f59a565b-f6a0-11eb-8281-309c2364fdb6','78ee5778-a5b5-11eb-bac0-309c2364fdb6','0','2','aBCde',NULL,'v','2021-08-07 15:01:36',1,NULL,0,NULL,'2021-05-27 11:44:45'),('60901a65-f798-11eb-9131-309c2364fdb6','f59a565b-f6a0-11eb-8281-309c2364fdb6','78ee5778-a5b5-11eb-bac0-309c2364fdb6','0','2','aBCdefghi',NULL,'v','2021-08-07 15:58:59',1,NULL,0,NULL,'2021-05-27 11:44:45'),('6237adcc-f790-11eb-9131-309c2364fdb6','f59a565b-f6a0-11eb-8281-309c2364fdb6','78ee5778-a5b5-11eb-bac0-309c2364fdb6','0','2','aBCde',NULL,'v','2021-08-07 15:01:46',1,NULL,0,NULL,'2021-05-27 11:44:45'),('69164248-f7a3-11eb-9fdf-309c2364fdb6','f59a565b-f6a0-11eb-8281-309c2364fdb6','78ee5778-a5b5-11eb-bac0-309c2364fdb6','0','2','aBCdefghijkfhfkhhgjfg',NULL,'v','2021-08-07 17:17:57',1,NULL,0,NULL,'2021-05-27 11:44:45'),('6f182708-f79f-11eb-9fdf-309c2364fdb6','f59a565b-f6a0-11eb-8281-309c2364fdb6','78ee5778-a5b5-11eb-bac0-309c2364fdb6','0','2','aBCdefghijkhkhhgj',NULL,'v','2021-08-07 16:49:30',1,NULL,0,NULL,'2021-05-27 11:44:45'),('6ffd92b0-f794-11eb-9131-309c2364fdb6','f59a565b-f6a0-11eb-8281-309c2364fdb6','78ee5778-a5b5-11eb-bac0-309c2364fdb6','0','2','aBCdef',NULL,'v','2021-08-07 15:30:47',1,NULL,0,NULL,'2021-05-27 11:44:45'),('71710f07-f79e-11eb-9fdf-309c2364fdb6','f59a565b-f6a0-11eb-8281-309c2364fdb6','78ee5778-a5b5-11eb-bac0-309c2364fdb6','0','2','aBCdefghijkhkhhg',NULL,'v','2021-08-07 16:42:24',1,NULL,0,NULL,'2021-05-27 11:44:45'),('7ccc7012-f7a3-11eb-9fdf-309c2364fdb6','f59a565b-f6a0-11eb-8281-309c2364fdb6','78ee5778-a5b5-11eb-bac0-309c2364fdb6','0','2','aBCdefghijkfhffkhhgjfg',NULL,'v','2021-08-07 17:18:31',1,NULL,0,NULL,'2021-05-27 11:44:45'),('97541bd2-f79b-11eb-9fdf-309c2364fdb6','f59a565b-f6a0-11eb-8281-309c2364fdb6','78ee5778-a5b5-11eb-bac0-309c2364fdb6','0','2','aBCdefghijkhk',NULL,'v','2021-08-07 16:21:59',1,NULL,0,NULL,'2021-05-27 11:44:45'),('979bfe4a-f798-11eb-9131-309c2364fdb6','f59a565b-f6a0-11eb-8281-309c2364fdb6','78ee5778-a5b5-11eb-bac0-309c2364fdb6','0','2','aBCdefghij',NULL,'v','2021-08-07 16:00:31',1,NULL,0,NULL,'2021-05-27 11:44:45'),('993852cb-f790-11eb-9131-309c2364fdb6','f59a565b-f6a0-11eb-8281-309c2364fdb6','78ee5778-a5b5-11eb-bac0-309c2364fdb6','0','2','aBCde',NULL,'v','2021-08-07 15:03:18',1,NULL,0,NULL,'2021-05-27 11:44:45'),('9f3b73f7-f7a5-11eb-9fdf-309c2364fdb6','f59a565b-f6a0-11eb-8281-309c2364fdb6','78ee5778-a5b5-11eb-bac0-309c2364fdb6','0','2','aBCdefghijfkfhffffkhhgjffg',NULL,'v','2021-08-07 17:33:47',1,NULL,0,NULL,'2021-05-27 11:44:45'),('b21953c0-f6a6-11eb-8281-309c2364fdb6','f59a565b-f6a0-11eb-8281-309c2364fdb6','78ee5778-a5b5-11eb-bac0-309c2364fdb6','0','2','aBC',NULL,'v','2021-08-06 11:08:57',1,NULL,0,NULL,'2021-05-27 11:44:45'),('c363bf3d-f798-11eb-9131-309c2364fdb6','f59a565b-f6a0-11eb-8281-309c2364fdb6','78ee5778-a5b5-11eb-bac0-309c2364fdb6','0','2','aBCdefghijk',NULL,'v','2021-08-07 16:01:45',1,NULL,0,NULL,'2021-05-27 11:44:45'),('c373d265-f79e-11eb-9fdf-309c2364fdb6','f59a565b-f6a0-11eb-8281-309c2364fdb6','78ee5778-a5b5-11eb-bac0-309c2364fdb6','0','2','aBCdefghijkhkhhgj',NULL,'v','2021-08-07 16:44:42',1,NULL,0,NULL,'2021-05-27 11:44:45'),('c54d7b1c-a5bb-11eb-bac0-309c2364fdb6','5f0f1048-a5b5-11eb-bac0-309c2364fdb6','78ee5d00-a5b5-11eb-bac0-309c2364fdb6','0','1','Alphaphase',NULL,'','2021-04-25 11:45:45',1,NULL,0,NULL,'2021-05-13 23:58:38'),('c54dba6f-a5bb-11eb-bac0-309c2364fdb6','5f0f25c1-a5b5-11eb-bac0-309c2364fdb6','78ee5778-a5b5-11eb-bac0-309c2364fdb6','0','2','a',NULL,' ','2021-04-25 11:45:45',1,NULL,0,NULL,'2021-05-27 11:44:45'),('cdaf1368-f79f-11eb-9fdf-309c2364fdb6','f59a565b-f6a0-11eb-8281-309c2364fdb6','78ee5778-a5b5-11eb-bac0-309c2364fdb6','0','2','aBCdefghijkhkhhgj',NULL,'v','2021-08-07 16:52:08',1,NULL,0,NULL,'2021-05-27 11:44:45'),('d8cf4d0a-f6a6-11eb-8281-309c2364fdb6','f59a565b-f6a0-11eb-8281-309c2364fdb6','78ee5778-a5b5-11eb-bac0-309c2364fdb6','0','2','aBCd',NULL,'v','2021-08-06 11:10:02',1,NULL,0,NULL,'2021-05-27 11:44:45'),('e5cd6d6c-f79d-11eb-9fdf-309c2364fdb6','f59a565b-f6a0-11eb-8281-309c2364fdb6','78ee5778-a5b5-11eb-bac0-309c2364fdb6','0','2','aBCdefghijkhk',NULL,'v','2021-08-07 16:38:30',1,NULL,0,NULL,'2021-05-27 11:44:45'),('e7677832-f7a2-11eb-9fdf-309c2364fdb6','f59a565b-f6a0-11eb-8281-309c2364fdb6','78ee5778-a5b5-11eb-bac0-309c2364fdb6','0','2','aBCdefghijkhfkhhgjfg',NULL,'v','2021-08-07 17:14:20',1,NULL,0,NULL,'2021-05-27 11:44:45'),('ead26508-f788-11eb-9131-309c2364fdb6','f59a565b-f6a0-11eb-8281-309c2364fdb6','78ee5778-a5b5-11eb-bac0-309c2364fdb6','0','2','aBCd',NULL,'v','2021-08-07 14:08:19',1,NULL,0,NULL,'2021-05-27 11:44:45'),('f3da8b29-f78e-11eb-9131-309c2364fdb6','f59a565b-f6a0-11eb-8281-309c2364fdb6','78ee5778-a5b5-11eb-bac0-309c2364fdb6','0','2','aBCd',NULL,'v','2021-08-07 14:51:31',1,NULL,0,NULL,'2021-05-27 11:44:45');
/*!40000 ALTER TABLE `software_version` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`rest`@`%`*/ /*!50003 TRIGGER `software_version_BEFORE_INSERT` BEFORE INSERT ON `software_version` FOR EACH ROW BEGIN 	SET new.creation_datetime = NOW();   END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`rest`@`%`*/ /*!50003 TRIGGER `software_version_BEFORE_UPDATE` BEFORE UPDATE ON `software_version` FOR EACH ROW BEGIN 	SET new.changed_datetime = NOW();END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `software_version_relation_to_operating_system`
--

DROP TABLE IF EXISTS `software_version_relation_to_operating_system`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `software_version_relation_to_operating_system` (
  `uuid` varchar(36) NOT NULL DEFAULT 'UUID',
  `software_version_uuid` varchar(36) NOT NULL,
  `operating_system_uuid` varchar(36) NOT NULL,
  `creation_datetime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `active` tinyint NOT NULL DEFAULT '1',
  `activation_datetime` datetime DEFAULT NULL,
  `deleted` tinyint DEFAULT '0',
  `deletion_datetime` datetime DEFAULT NULL,
  `changed_datetime` datetime DEFAULT NULL,
  PRIMARY KEY (`software_version_uuid`,`operating_system_uuid`),
  KEY `fk_softwareVersionOpArchToSoftwareVersion_idx` (`software_version_uuid`),
  KEY `fk_softwareVersionOpArchToOperatingSystem_idx` (`operating_system_uuid`),
  CONSTRAINT `fk_softwareVersionOpArchToOperatingSystem` FOREIGN KEY (`operating_system_uuid`) REFERENCES `operating_system` (`uuid`),
  CONSTRAINT `fk_softwareVersionOpArchToSoftwareVersion` FOREIGN KEY (`software_version_uuid`) REFERENCES `software_version` (`uuid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `software_version_relation_to_operating_system`
--

LOCK TABLES `software_version_relation_to_operating_system` WRITE;
/*!40000 ALTER TABLE `software_version_relation_to_operating_system` DISABLE KEYS */;
INSERT INTO `software_version_relation_to_operating_system` VALUES ('e2411bd4-a5bb-11eb-bac0-309c2364fdb6','c54d7b1c-a5bb-11eb-bac0-309c2364fdb6','3feb2832-a5b5-11eb-bac0-309c2364fdb6','2021-04-25 11:46:33',1,NULL,0,NULL,'2021-06-08 20:01:23'),('e2417776-a5bb-11eb-bac0-309c2364fdb6','c54d7b1c-a5bb-11eb-bac0-309c2364fdb6','6f192049-c881-11eb-a494-309c2364fdb6','2021-04-25 11:46:33',1,NULL,0,NULL,'2021-06-08 20:01:23'),('e24185ca-a5bb-11eb-bac0-309c2364fdb6','c54dba6f-a5bb-11eb-bac0-309c2364fdb6','3feb2832-a5b5-11eb-bac0-309c2364fdb6','2021-04-25 11:46:33',1,NULL,0,NULL,'2021-06-08 20:01:23');
/*!40000 ALTER TABLE `software_version_relation_to_operating_system` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`rest`@`%`*/ /*!50003 TRIGGER `software_version_relation_to_operating_system_arch_BEFORE_INSERT` BEFORE INSERT ON `software_version_relation_to_operating_system` FOR EACH ROW BEGIN 	SET new.creation_datetime = NOW();   END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`rest`@`%`*/ /*!50003 TRIGGER `software_version_relation_to_operating_system_arch_BEFORE_UPDATE` BEFORE UPDATE ON `software_version_relation_to_operating_system` FOR EACH ROW BEGIN 	SET new.changed_datetime = NOW();END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `system_message_user`
--

DROP TABLE IF EXISTS `system_message_user`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `system_message_user` (
  `uuid` varchar(36) NOT NULL,
  `user` varchar(128) NOT NULL,
  `password` varchar(45) NOT NULL,
  `communication_medium_uuid` varchar(36) NOT NULL,
  `creation_datetime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `active` tinyint NOT NULL DEFAULT '1',
  `activation_datetime` datetime DEFAULT NULL,
  `deleted` tinyint DEFAULT '0',
  `deletion_datetime` datetime DEFAULT NULL,
  `changed_datetime` datetime DEFAULT NULL,
  PRIMARY KEY (`user`,`communication_medium_uuid`),
  KEY `user_Idx` (`user`),
  KEY `fk_sysMsgUserToCommMedium_idx` (`communication_medium_uuid`),
  CONSTRAINT `fk_sysMsgUserToCommMedium` FOREIGN KEY (`communication_medium_uuid`) REFERENCES `communication_medium` (`uuid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `system_message_user`
--

LOCK TABLES `system_message_user` WRITE;
/*!40000 ALTER TABLE `system_message_user` DISABLE KEYS */;
INSERT INTO `system_message_user` VALUES ('1b745f22-dbe3-11eb-8cd9-842afd097283','datenschutz@helixdb.org','g9iMFhYnxGMbzs9','258b6f62-d91d-11eb-81f0-842afd097283','2021-07-03 09:43:22',1,NULL,1,NULL,'2021-07-04 17:38:36'),('1b73e51b-dbe3-11eb-8cd9-842afd097283','datenschutz@helixdb.org','g9iMFhYnxGMbzs9','6ad9523e-d91a-11eb-81f0-842afd097283','2021-07-03 09:43:22',1,NULL,1,NULL,'2021-07-04 17:38:36'),('1b743ff7-dbe3-11eb-8cd9-842afd097283','service@helixdb.org','g9iMFhYnxGMbzs9','258b6f62-d91d-11eb-81f0-842afd097283','2021-07-03 09:43:22',1,NULL,0,NULL,NULL),('1b72a750-dbe3-11eb-8cd9-842afd097283','service@helixdb.org','g9iMFhYnxGMbzs9','6ad9523e-d91a-11eb-81f0-842afd097283','2021-07-03 09:43:22',1,NULL,0,NULL,NULL),('1b747710-dbe3-11eb-8cd9-842afd097283','support@helixdb.org','g9iMFhYnxGMbzs9','258b6f62-d91d-11eb-81f0-842afd097283','2021-07-03 09:43:22',1,NULL,1,NULL,'2021-07-04 17:38:36'),('1b740830-dbe3-11eb-8cd9-842afd097283','support@helixdb.org','g9iMFhYnxGMbzs9','6ad9523e-d91a-11eb-81f0-842afd097283','2021-07-03 09:43:22',1,NULL,1,NULL,'2021-07-04 17:38:36');
/*!40000 ALTER TABLE `system_message_user` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `system_message_user_BEFORE_INSERT` BEFORE INSERT ON `system_message_user` FOR EACH ROW BEGIN
SET new.creation_datetime = NOW();   
END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `system_message_user_BEFORE_UPDATE` BEFORE UPDATE ON `system_message_user` FOR EACH ROW BEGIN
SET new.changed_datetime = NOW();
END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `user`
--

DROP TABLE IF EXISTS `user`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user` (
  `uuid` varchar(36) NOT NULL,
  `user_type_uuid` varchar(36) NOT NULL,
  `account_uuid` varchar(36) NOT NULL,
  `user` varchar(20) NOT NULL,
  `password` varchar(45) NOT NULL,
  `first_name` varchar(45) DEFAULT NULL,
  `last_name` varchar(45) DEFAULT NULL,
  `phone` varchar(45) DEFAULT NULL,
  `date_of_birth` date DEFAULT NULL,
  `activation_code` varchar(4) DEFAULT NULL,
  `activation_token` varchar(1024) DEFAULT NULL,
  `max_auth_token` int NOT NULL DEFAULT '1',
  `creation_datetime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `active` tinyint NOT NULL DEFAULT '1',
  `activation_datetime` datetime DEFAULT NULL,
  `deleted` tinyint DEFAULT '0',
  `deletion_datetime` datetime DEFAULT NULL,
  `changed_datetime` datetime DEFAULT NULL,
  `api_access_granted` tinyint NOT NULL DEFAULT '0' COMMENT 'Wenn Software bezahlt erhält er API Access',
  `user_profile_pic_file_ext` varchar(5) DEFAULT NULL,
  PRIMARY KEY (`uuid`),
  KEY `fk_userToUserType_idx` (`user_type_uuid`),
  KEY `fk_userToAccount_idx` (`account_uuid`),
  CONSTRAINT `fk_userToAccount` FOREIGN KEY (`account_uuid`) REFERENCES `account` (`uuid`),
  CONSTRAINT `fk_userToUserType` FOREIGN KEY (`user_type_uuid`) REFERENCES `user_type` (`uuid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `user`
--

LOCK TABLES `user` WRITE;
/*!40000 ALTER TABLE `user` DISABLE KEYS */;
INSERT INTO `user` VALUES ('19ae4715-cef4-11ec-83a3-7085c294413b','0e1244f8-1ca3-11ec-a4a4-d8bbc10f2ae0','1292baca-cef4-11ec-83a3-7085c294413b','0x00405a00','meinpw','Joel Mika','Roos','+4917643394274','1996-05-06','ffff','ffff',1,'2022-05-08 17:27:13',1,NULL,0,NULL,NULL,0,NULL),('3e166107-be6a-11eb-9f54-842afd097283','7340425e-a5b5-11eb-bac0-309c2364fdb6','dcd8ec4e-bfa2-11eb-a11f-309c2364fdb6','test2','098f6bcd4621d373cade4e832627b4f6',NULL,NULL,NULL,NULL,NULL,NULL,1,'2021-05-26 21:35:08',1,NULL,0,NULL,'2021-11-13 16:41:59',1,NULL),('6c7d07e0-a5b5-11eb-bac0-309c2364fdb6','7340425e-a5b5-11eb-bac0-309c2364fdb6','dcd8ac39-bfa2-11eb-a11f-309c2364fdb6','test','098f6bcd4621d373cade4e832627b4f6',NULL,NULL,NULL,NULL,NULL,NULL,1,'2021-04-25 11:00:19',1,NULL,0,NULL,'2021-11-13 16:41:59',1,'.jpg'),('d0f751a8-3425-11ec-b696-d8bbc10f2ae0','0e1244f8-1ca3-11ec-a4a4-d8bbc10f2ae0','d0eb95d2-3425-11ec-b696-d8bbc10f2ae0','mika-roos@web.de','ecdb9464e6fadfbbff7e7fc139405e61','test','test','02373177057',NULL,'7540','eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJ1dWlkIjoiMDAwMDAwMDAtMDAwMC0wMDAwLTAwMDAtMDAwMDAwMDAwMDAwIiwidXNlciI6Im1pa2Etcm9vc0B3ZWIuZGUiLCJ1c2VyX3R5cGVfaWQiOiIwMDAwMDAwMC0wMDAwLTAwMDAtMDAwMC0wMDAwMDAwMDAwMDAiLCJlbWFpbCI6IjAwMDAwMDAwLTAwMDAtMDAwMC0wMDAwLTAwMDAwMDAwMDAwMCIsImV4cGlyZXNfdGltZSI6IjA3LjExLjIwMjEgMTQ6NDY6MDYiLCJuYmYiOjE2MzU2ODc5NjYsImV4cCI6MTYzNjI5Mjc2NiwiaWF0IjoxNjM1Njg3OTY2fQ.jPk4rsd3ERkX4wiABLioNmv9zmS-zGyB-o78t4RceBnfquRJlRsOIO4JuVQ8RkQJsZTbxCH91UFxZFTezoZ2aQ',1,'2021-10-23 17:22:36',1,NULL,0,NULL,'2021-11-27 17:34:50',1,NULL),('f8708bc4-1ca5-11ec-a4a4-d8bbc10f2ae0','0e1244f8-1ca3-11ec-a4a4-d8bbc10f2ae0','edf04962-1ca5-11ec-a4a4-d8bbc10f2ae0','root','63a9f0ea7bb98050796b649e85481845',NULL,NULL,NULL,NULL,NULL,NULL,1,'2021-09-23 19:39:30',1,NULL,0,NULL,'2021-11-13 16:41:59',1,NULL);
/*!40000 ALTER TABLE `user` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`rest`@`%`*/ /*!50003 TRIGGER `user_BEFORE_INSERT` BEFORE INSERT ON `user` FOR EACH ROW BEGIN 	SET new.creation_datetime = NOW();   END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`rest`@`%`*/ /*!50003 TRIGGER `user_BEFORE_UPDATE` BEFORE UPDATE ON `user` FOR EACH ROW BEGIN 	SET new.changed_datetime = NOW();END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `user_relation_to_role`
--

DROP TABLE IF EXISTS `user_relation_to_role`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user_relation_to_role` (
  `uuid` varchar(36) NOT NULL,
  `user_uuid` varchar(36) NOT NULL,
  `role_uuid` varchar(36) NOT NULL,
  `creation_datetime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `active` tinyint NOT NULL DEFAULT '1',
  `activation_datetime` datetime DEFAULT NULL,
  `deleted` tinyint DEFAULT '0',
  `deletion_datetime` datetime DEFAULT NULL,
  `changed_datetime` datetime DEFAULT NULL,
  PRIMARY KEY (`user_uuid`,`role_uuid`),
  KEY `fk_userRoleToRole_idx` (`role_uuid`),
  KEY `fk_userRoleToUser_idx` (`user_uuid`),
  CONSTRAINT `fk_userRoleToRole` FOREIGN KEY (`role_uuid`) REFERENCES `role` (`uuid`),
  CONSTRAINT `fk_userRoleToUser` FOREIGN KEY (`user_uuid`) REFERENCES `user` (`uuid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `user_relation_to_role`
--

LOCK TABLES `user_relation_to_role` WRITE;
/*!40000 ALTER TABLE `user_relation_to_role` DISABLE KEYS */;
INSERT INTO `user_relation_to_role` VALUES ('24e382b0-cef4-11ec-83a3-7085c294413b','19ae4715-cef4-11ec-83a3-7085c294413b','2a9e2014-1ca4-11ec-a4a4-d8bbc10f2ae0','2022-05-08 17:27:32',1,NULL,0,NULL,NULL),('5160a65e-bec5-11eb-8e36-309c2364fdb6','3e166107-be6a-11eb-9f54-842afd097283','4c155424-a5b5-11eb-bac0-309c2364fdb6','2021-05-27 08:27:04',1,NULL,0,NULL,'2021-05-27 12:45:46'),('4a1b3cd4-bec2-11eb-8e36-309c2364fdb6','6c7d07e0-a5b5-11eb-bac0-309c2364fdb6','4c155424-a5b5-11eb-bac0-309c2364fdb6','2021-05-27 08:05:24',1,NULL,0,NULL,NULL),('54380de7-1cad-11ec-a4a4-d8bbc10f2ae0','f8708bc4-1ca5-11ec-a4a4-d8bbc10f2ae0','2a9e2014-1ca4-11ec-a4a4-d8bbc10f2ae0','2021-09-23 20:32:10',1,NULL,0,NULL,NULL);
/*!40000 ALTER TABLE `user_relation_to_role` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`rest`@`%`*/ /*!50003 TRIGGER `user_relation_to_role_BEFORE_INSERT` BEFORE INSERT ON `user_relation_to_role` FOR EACH ROW BEGIN 	SET new.creation_datetime = NOW();   END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`rest`@`%`*/ /*!50003 TRIGGER `user_relation_to_role_BEFORE_UPDATE` BEFORE UPDATE ON `user_relation_to_role` FOR EACH ROW BEGIN 	SET new.changed_datetime = NOW();END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `user_type`
--

DROP TABLE IF EXISTS `user_type`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user_type` (
  `uuid` varchar(36) NOT NULL DEFAULT 'UUID',
  `name` varchar(45) NOT NULL,
  `creation_datetime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `active` tinyint NOT NULL DEFAULT '1',
  `activation_datetime` datetime DEFAULT NULL,
  `deleted` tinyint DEFAULT '0',
  `deletion_datetime` datetime DEFAULT NULL,
  `changed_datetime` datetime DEFAULT NULL,
  PRIMARY KEY (`uuid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `user_type`
--

LOCK TABLES `user_type` WRITE;
/*!40000 ALTER TABLE `user_type` DISABLE KEYS */;
INSERT INTO `user_type` VALUES ('0e1244f8-1ca3-11ec-a4a4-d8bbc10f2ae0','root','2021-09-23 19:18:38',1,NULL,0,NULL,NULL),('733fd09c-a5b5-11eb-bac0-309c2364fdb6','Administrator','2021-04-25 11:00:30',1,NULL,0,NULL,NULL),('7340425e-a5b5-11eb-bac0-309c2364fdb6','User','2021-04-25 11:00:30',1,NULL,0,NULL,NULL);
/*!40000 ALTER TABLE `user_type` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`rest`@`%`*/ /*!50003 TRIGGER `user_type_BEFORE_INSERT` BEFORE INSERT ON `user_type` FOR EACH ROW BEGIN 	SET new.creation_datetime = NOW();   END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`rest`@`%`*/ /*!50003 TRIGGER `user_type_BEFORE_UPDATE` BEFORE UPDATE ON `user_type` FOR EACH ROW BEGIN 	SET new.changed_datetime = NOW();END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `version_type`
--

DROP TABLE IF EXISTS `version_type`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `version_type` (
  `uuid` varchar(36) NOT NULL DEFAULT 'UUID',
  `name` varchar(90) DEFAULT NULL,
  `description` varchar(256) DEFAULT NULL,
  `creation_datetime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `active` tinyint NOT NULL DEFAULT '1',
  `activation_datetime` datetime DEFAULT NULL,
  `deleted` tinyint DEFAULT '0',
  `deletion_datetime` datetime DEFAULT NULL,
  `changed_datetime` datetime DEFAULT NULL,
  PRIMARY KEY (`uuid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `version_type`
--

LOCK TABLES `version_type` WRITE;
/*!40000 ALTER TABLE `version_type` DISABLE KEYS */;
INSERT INTO `version_type` VALUES ('78edf51c-a5b5-11eb-bac0-309c2364fdb6','Long-term support','LTS','2021-04-25 11:00:40',1,NULL,0,NULL,'2021-05-27 11:43:17'),('78ee562a-a5b5-11eb-bac0-309c2364fdb6','Short term support','STS','2021-04-25 11:00:40',1,NULL,0,NULL,'2021-05-27 11:43:17'),('78ee5778-a5b5-11eb-bac0-309c2364fdb6','Beta','Pre LTS & STS','2021-04-25 11:00:40',1,NULL,0,NULL,'2021-05-27 11:43:17'),('78ee5d00-a5b5-11eb-bac0-309c2364fdb6','Alpha','Pre Beta','2021-04-25 11:00:40',1,NULL,0,NULL,'2021-05-27 11:43:17'),('78ee5dce-a5b5-11eb-bac0-309c2364fdb6','RC','Release Candidate','2021-04-25 11:00:40',1,NULL,0,NULL,'2021-05-27 11:43:17');
/*!40000 ALTER TABLE `version_type` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`rest`@`%`*/ /*!50003 TRIGGER `version_type_BEFORE_INSERT` BEFORE INSERT ON `version_type` FOR EACH ROW BEGIN 	SET new.creation_datetime = NOW();   END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`rest`@`%`*/ /*!50003 TRIGGER `version_type_BEFORE_UPDATE` BEFORE UPDATE ON `version_type` FOR EACH ROW BEGIN 	SET new.changed_datetime = NOW();END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Dumping events for database 'helix'
--

--
-- Dumping routines for database 'helix'
--
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2023-05-02 22:18:01

-- MySQL dump 10.13  Distrib 8.0.30, for Win64 (x86_64)
--
-- Host: localhost    Database: jellyfish
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
-- Table structure for table `auth`
--

DROP TABLE IF EXISTS `auth`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `auth` (
  `uuid` varchar(36) NOT NULL,
  `user_uuid` varchar(36) NOT NULL,
  `ip_addrv4_remote` varchar(45) DEFAULT NULL,
  `ip_addrv6_remote` varchar(150) DEFAULT NULL,
  `remote_port` int DEFAULT NULL,
  `ip_addrv4_local` varchar(45) DEFAULT NULL,
  `ip_addrv6_local` varchar(150) DEFAULT NULL,
  `local_port` int DEFAULT NULL,
  `token` varchar(2000) NOT NULL,
  `token_expires_in` datetime NOT NULL,
  `user_agent` text NOT NULL,
  `logout_datetime` datetime DEFAULT NULL,
  `refresh_token` varchar(2000) NOT NULL,
  `refresh_token_expires_in` datetime NOT NULL,
  `creation_datetime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `active` tinyint NOT NULL DEFAULT '1',
  `activation_datetime` datetime DEFAULT NULL,
  `deleted` tinyint DEFAULT '0',
  `deletion_datetime` datetime DEFAULT NULL,
  `changed_datetime` datetime DEFAULT NULL,
  PRIMARY KEY (`uuid`),
  KEY `fk_authToUser_idx` (`user_uuid`),
  CONSTRAINT `fk_authToUser` FOREIGN KEY (`user_uuid`) REFERENCES `user` (`uuid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `auth`
--

LOCK TABLES `auth` WRITE;
/*!40000 ALTER TABLE `auth` DISABLE KEYS */;
INSERT INTO `auth` VALUES ('6fe6b8bd-eabd-11ed-a0f1-7085c294413b','b35d8651-eabb-11ed-a0f1-7085c294413b','10.100.0.200',NULL,35308,'10.100.0.200',NULL,5030,'eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJ1dWlkIjoiYjM1ZDg2NTEtZWFiYi0xMWVkLWEwZjEtNzA4NWMyOTQ0MTNiIiwidXNlciI6InJvb3QiLCJ1c2VyX3R5cGVfaWQiOiJhNWYzMTQyZC1lYWJiLTExZWQtYTBmMS03MDg1YzI5NDQxM2IiLCJlbWFpbCI6IjAwMDAwMDAwLTAwMDAtMDAwMC0wMDAwLTAwMDAwMDAwMDAwMCIsImV4cGlyZXNfdGltZSI6IjA0LjA1LjIwMjMgMjM6MjE6MjYiLCJhcGlfYWNjZXNzX2dyYW50IjoiRmFsc2UiLCJuYmYiOjE2ODMyMzM0ODYsImV4cCI6MTY4MzIzNTI4NiwiaWF0IjoxNjgzMjMzNDg2LCJpc3MiOiJodHRwOi8vbG9jYWxob3N0OjUwMTAvIiwiYXVkIjoiaHR0cDovL2xvY2FsaG9zdDo1MDEwLyJ9.pj-bu444KuX7-0qRwNstyDKbnmS8XFwINFewSKr1FgTzrcrc3I9TAK4MwDrrzPAN7hEsi0nT61wCaX-cNi-mww','2023-05-04 23:21:26','PostmanRuntime/7.29.2',NULL,'6fe5ed03-eabd-11ed-a0f1-7085c294413b','2023-05-04 23:51:26','2023-05-04 20:51:26',1,NULL,0,NULL,NULL),('9ddf35d1-eabf-11ed-a0f1-7085c294413b','b35d8651-eabb-11ed-a0f1-7085c294413b','10.100.0.200',NULL,35583,'10.100.0.200',NULL,5030,'eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJ1dWlkIjoiYjM1ZDg2NTEtZWFiYi0xMWVkLWEwZjEtNzA4NWMyOTQ0MTNiIiwidXNlciI6InJvb3QiLCJ1c2VyX3R5cGVfaWQiOiJhNWYzMTQyZC1lYWJiLTExZWQtYTBmMS03MDg1YzI5NDQxM2IiLCJlbWFpbCI6IjAwMDAwMDAwLTAwMDAtMDAwMC0wMDAwLTAwMDAwMDAwMDAwMCIsImV4cGlyZXNfdGltZSI6IjA0LjA1LjIwMjMgMjM6Mzc6MDIiLCJhcGlfYWNjZXNzX2dyYW50IjoiRmFsc2UiLCJuYmYiOjE2ODMyMzQ0MjIsImV4cCI6MTY4MzIzNjIyMiwiaWF0IjoxNjgzMjM0NDIyLCJpc3MiOiJodHRwOi8vbG9jYWxob3N0OjUwMTAvIiwiYXVkIjoiaHR0cDovL2xvY2FsaG9zdDo1MDEwLyJ9.2i1uy47ppd4fmkjFZzVJAGojSR9At846sHnoCM8JfZgHEAXMdQyafm3XDbegxGiIVeFOHrW4ZeTi5R7_LdWtWA','2023-05-04 23:37:03','PostmanRuntime/7.29.2',NULL,'9dde78c8-eabf-11ed-a0f1-7085c294413b','2023-05-05 00:07:03','2023-05-04 21:07:02',1,NULL,0,NULL,NULL);
/*!40000 ALTER TABLE `auth` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `chat`
--

DROP TABLE IF EXISTS `chat`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `chat` (
  `uuid` varchar(36) NOT NULL,
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
-- Dumping data for table `chat`
--

LOCK TABLES `chat` WRITE;
/*!40000 ALTER TABLE `chat` DISABLE KEYS */;
/*!40000 ALTER TABLE `chat` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `chat_relation_to_user`
--

DROP TABLE IF EXISTS `chat_relation_to_user`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `chat_relation_to_user` (
  `uuid` varchar(36) NOT NULL,
  `creation_datetime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `active` tinyint NOT NULL DEFAULT '1',
  `activation_datetime` datetime DEFAULT NULL,
  `deleted` tinyint DEFAULT '0',
  `deletion_datetime` datetime DEFAULT NULL,
  `changed_datetime` datetime DEFAULT NULL,
  `chat_uuid` varchar(36) NOT NULL,
  `user_uuid` varchar(36) NOT NULL,
  `name` varchar(64) DEFAULT NULL,
  `description` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`uuid`),
  KEY `fkChatToUser_idx` (`user_uuid`),
  KEY `fkChatToChat_idx` (`chat_uuid`),
  CONSTRAINT `fkChatToChat` FOREIGN KEY (`chat_uuid`) REFERENCES `chat` (`uuid`),
  CONSTRAINT `fkChatToUser` FOREIGN KEY (`user_uuid`) REFERENCES `user` (`uuid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `chat_relation_to_user`
--

LOCK TABLES `chat_relation_to_user` WRITE;
/*!40000 ALTER TABLE `chat_relation_to_user` DISABLE KEYS */;
/*!40000 ALTER TABLE `chat_relation_to_user` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `message`
--

DROP TABLE IF EXISTS `message`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `message` (
  `uuid` varchar(36) NOT NULL,
  `creation_datetime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `active` tinyint NOT NULL DEFAULT '1',
  `activation_datetime` datetime DEFAULT NULL,
  `deleted` tinyint DEFAULT '0',
  `deletion_datetime` datetime DEFAULT NULL,
  `changed_datetime` datetime DEFAULT NULL,
  `chat_uuid` varchar(36) NOT NULL,
  `message_owner` varchar(36) DEFAULT NULL,
  `text` blob NOT NULL,
  PRIMARY KEY (`uuid`),
  KEY `fkMessageToChat_idx` (`chat_uuid`),
  KEY `fkMessageOwnerToUser_idx` (`message_owner`),
  CONSTRAINT `fkMessageOwnerToUser` FOREIGN KEY (`message_owner`) REFERENCES `user` (`uuid`),
  CONSTRAINT `fkMessageToChat` FOREIGN KEY (`chat_uuid`) REFERENCES `chat` (`uuid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `message`
--

LOCK TABLES `message` WRITE;
/*!40000 ALTER TABLE `message` DISABLE KEYS */;
/*!40000 ALTER TABLE `message` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `message_acknowledge`
--

DROP TABLE IF EXISTS `message_acknowledge`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `message_acknowledge` (
  `message_uuid` varchar(36) NOT NULL,
  `user_uuid` varchar(36) DEFAULT NULL,
  `uuid` varchar(36) NOT NULL,
  `creation_datetime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `active` tinyint NOT NULL DEFAULT '1',
  `activation_datetime` datetime DEFAULT NULL,
  `deleted` tinyint DEFAULT '0',
  `deletion_datetime` datetime DEFAULT NULL,
  `changed_datetime` datetime DEFAULT NULL,
  PRIMARY KEY (`message_uuid`),
  KEY `fkMessageAckToUser_idx` (`user_uuid`),
  CONSTRAINT `fkMessageAckToMessage` FOREIGN KEY (`message_uuid`) REFERENCES `message` (`uuid`),
  CONSTRAINT `fkMessageAckToUser` FOREIGN KEY (`user_uuid`) REFERENCES `user` (`uuid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `message_acknowledge`
--

LOCK TABLES `message_acknowledge` WRITE;
/*!40000 ALTER TABLE `message_acknowledge` DISABLE KEYS */;
/*!40000 ALTER TABLE `message_acknowledge` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `message_attachment`
--

DROP TABLE IF EXISTS `message_attachment`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `message_attachment` (
  `uuid` varchar(36) NOT NULL,
  `creation_datetime` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `active` tinyint NOT NULL DEFAULT '1',
  `activation_datetime` datetime DEFAULT NULL,
  `deleted` tinyint DEFAULT '0',
  `deletion_datetime` datetime DEFAULT NULL,
  `changed_datetime` datetime DEFAULT NULL,
  `message_uuid` varchar(36) NOT NULL,
  `content` blob NOT NULL,
  PRIMARY KEY (`uuid`),
  KEY `fkMessageAttachmentToMessage_idx` (`message_uuid`),
  CONSTRAINT `fkMessageAttachmentToMessage` FOREIGN KEY (`message_uuid`) REFERENCES `message` (`uuid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `message_attachment`
--

LOCK TABLES `message_attachment` WRITE;
/*!40000 ALTER TABLE `message_attachment` DISABLE KEYS */;
/*!40000 ALTER TABLE `message_attachment` ENABLE KEYS */;
UNLOCK TABLES;

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
/*!40000 ALTER TABLE `role` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `user`
--

DROP TABLE IF EXISTS `user`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user` (
  `uuid` varchar(36) NOT NULL,
  `user_type_uuid` varchar(36) NOT NULL,
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
  `user_profile_pic_file_ext` varchar(5) DEFAULT NULL,
  PRIMARY KEY (`uuid`),
  KEY `fk_userToUserType_idx` (`user_type_uuid`),
  CONSTRAINT `fk_userToUserType` FOREIGN KEY (`user_type_uuid`) REFERENCES `user_type` (`uuid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `user`
--

LOCK TABLES `user` WRITE;
/*!40000 ALTER TABLE `user` DISABLE KEYS */;
INSERT INTO `user` VALUES ('b35d8651-eabb-11ed-a0f1-7085c294413b','a5f3142d-eabb-11ed-a0f1-7085c294413b','root','63A9F0EA7BB98050796B649E85481845','root','root',NULL,NULL,NULL,NULL,1,'2023-05-04 20:39:24',1,NULL,0,NULL,NULL,NULL);
/*!40000 ALTER TABLE `user` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Temporary view structure for view `user_active_view`
--

DROP TABLE IF EXISTS `user_active_view`;
/*!50001 DROP VIEW IF EXISTS `user_active_view`*/;
SET @saved_cs_client     = @@character_set_client;
/*!50503 SET character_set_client = utf8mb4 */;
/*!50001 CREATE VIEW `user_active_view` AS SELECT 
 1 AS `uuid`,
 1 AS `user_type_uuid`,
 1 AS `user`,
 1 AS `password`,
 1 AS `max_auth_token`,
 1 AS `creation_datetime`,
 1 AS `active`,
 1 AS `activation_datetime`,
 1 AS `deleted`,
 1 AS `deletion_datetime`,
 1 AS `changed_datetime`,
 1 AS `user_profile_pic_file_ext`*/;
SET character_set_client = @saved_cs_client;

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
/*!40000 ALTER TABLE `user_relation_to_role` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Temporary view structure for view `user_role_view`
--

DROP TABLE IF EXISTS `user_role_view`;
/*!50001 DROP VIEW IF EXISTS `user_role_view`*/;
SET @saved_cs_client     = @@character_set_client;
/*!50503 SET character_set_client = utf8mb4 */;
/*!50001 CREATE VIEW `user_role_view` AS SELECT 
 1 AS `uuid`,
 1 AS `name`,
 1 AS `description`,
 1 AS `creation_datetime`,
 1 AS `active`,
 1 AS `activation_datetime`,
 1 AS `deleted`,
 1 AS `deletion_datetime`,
 1 AS `changed_datetime`,
 1 AS `user_uuid`*/;
SET character_set_client = @saved_cs_client;

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
INSERT INTO `user_type` VALUES ('a5f3142d-eabb-11ed-a0f1-7085c294413b','root','2023-05-04 20:38:54',1,NULL,0,NULL,NULL);
/*!40000 ALTER TABLE `user_type` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping events for database 'jellyfish'
--

--
-- Dumping routines for database 'jellyfish'
--

--
-- Final view structure for view `user_active_view`
--

/*!50001 DROP VIEW IF EXISTS `user_active_view`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = utf8mb4 */;
/*!50001 SET character_set_results     = utf8mb4 */;
/*!50001 SET collation_connection      = utf8mb4_0900_ai_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`root`@`localhost` SQL SECURITY DEFINER */
/*!50001 VIEW `user_active_view` AS select `user`.`uuid` AS `uuid`,`user`.`user_type_uuid` AS `user_type_uuid`,`user`.`user` AS `user`,`user`.`password` AS `password`,`user`.`max_auth_token` AS `max_auth_token`,`user`.`creation_datetime` AS `creation_datetime`,`user`.`active` AS `active`,`user`.`activation_datetime` AS `activation_datetime`,`user`.`deleted` AS `deleted`,`user`.`deletion_datetime` AS `deletion_datetime`,`user`.`changed_datetime` AS `changed_datetime`,`user`.`user_profile_pic_file_ext` AS `user_profile_pic_file_ext` from `user` where (`user`.`active` = 1) */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;

--
-- Final view structure for view `user_role_view`
--

/*!50001 DROP VIEW IF EXISTS `user_role_view`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = utf8mb4 */;
/*!50001 SET character_set_results     = utf8mb4 */;
/*!50001 SET collation_connection      = utf8mb4_0900_ai_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`root`@`localhost` SQL SECURITY DEFINER */
/*!50001 VIEW `user_role_view` AS select `r`.`uuid` AS `uuid`,`r`.`name` AS `name`,`r`.`description` AS `description`,`r`.`creation_datetime` AS `creation_datetime`,`r`.`active` AS `active`,`r`.`activation_datetime` AS `activation_datetime`,`r`.`deleted` AS `deleted`,`r`.`deletion_datetime` AS `deletion_datetime`,`r`.`changed_datetime` AS `changed_datetime`,`ur`.`user_uuid` AS `user_uuid` from ((`user_relation_to_role` `ur` join `role` `r` on((`r`.`uuid` = `ur`.`role_uuid`))) join `user` `u` on((`u`.`uuid` = `ur`.`user_uuid`))) group by `ur`.`uuid` */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2023-05-05  0:02:16

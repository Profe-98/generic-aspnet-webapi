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
/*!40000 ALTER TABLE `user` ENABLE KEYS */;
UNLOCK TABLES;

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
/*!40000 ALTER TABLE `user_type` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping events for database 'jellyfish'
--

--
-- Dumping routines for database 'jellyfish'
--
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2023-05-02 22:18:12

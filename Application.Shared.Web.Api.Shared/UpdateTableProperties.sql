use api_gateway;
SET @tab_name = 'site_protect';
SET @sche_name = 'api_gateway';

CALL AlterTableAddColumnIfNotExists(@sche_name,@tab_name,'creation_datetime','ADD COLUMN `creation_datetime` TIMESTAMP NULL DEFAULT CURRENT_TIMESTAMP;');
CALL AlterTableAddColumnIfNotExists(@sche_name,@tab_name,'active','ADD COLUMN `active` TINYINT NOT NULL DEFAULT \'1\' AFTER `creation_datetime`;');
CALL AlterTableAddColumnIfNotExists(@sche_name,@tab_name,'activation_datetime','ADD COLUMN `activation_datetime` DATETIME NULL DEFAULT NULL AFTER `active`;');
CALL AlterTableAddColumnIfNotExists(@sche_name,@tab_name,'deleted','ADD COLUMN `deleted` TINYINT NULL DEFAULT \'1\' AFTER `activation_datetime`;');
CALL AlterTableAddColumnIfNotExists(@sche_name,@tab_name,'deletion_datetime','ADD COLUMN `deletion_datetime` DATETIME NULL DEFAULT NULL AFTER `deleted`;');
CALL AlterTableAddColumnIfNotExists(@sche_name,@tab_name,'changed_datetime','ADD COLUMN `changed_datetime` DATETIME NULL DEFAULT NULL AFTER `deletion_datetime`;');

DELIMITER $$
USE `api_gateway`$$
DROP TRIGGER IF EXISTS `api_gateway`.`site_protect_BEFORE_INSERT` $$
CREATE DEFINER = CURRENT_USER TRIGGER `api_gateway`.`site_protect_BEFORE_INSERT` BEFORE INSERT ON `site_protect` FOR EACH ROW
BEGIN
	SET new.creation_datetime = NOW();
    SET new.UUID = UUID();
END$$
DELIMITER ;

DELIMITER $$
USE `api_gateway`$$
DROP TRIGGER IF EXISTS `api_gateway`.`site_protect_BEFORE_UPDATE` $$
CREATE DEFINER = CURRENT_USER TRIGGER `api_gateway`.`site_protect_BEFORE_UPDATE` BEFORE UPDATE ON `site_protect` FOR EACH ROW
BEGIN
	SET new.changed_datetime = NOW();
END$$
DELIMITER ;
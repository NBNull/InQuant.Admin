CREATE DATABASE IF NOT EXISTS inquant_admin DEFAULT CHARSET utf8 COLLATE utf8_general_ci;

use inquant_admin;

# 用户角色表
CREATE TABLE IF NOT EXISTS `t_user_role`(
   `id` INT UNSIGNED AUTO_INCREMENT,
   userId int not null comment '用户Id',
   roleId int not null comment '角色ID',
   PRIMARY KEY (`id`) USING BTREE,
   INDEX `idx_user_role_user_id`(`userId`) USING BTREE
)ENGINE=InnoDB DEFAULT CHARSET=utf8;


# 角色表
CREATE TABLE IF NOT EXISTS `t_role`(
   `id` INT UNSIGNED AUTO_INCREMENT,
   `name` varchar(100) not null comment '角色名称',
   PRIMARY KEY ( `id` ) USING BTREE,
   INDEX `idx_role_name`(`name`) USING BTREE
)ENGINE=InnoDB DEFAULT CHARSET=utf8;

# 角色权限表
CREATE TABLE IF NOT EXISTS `t_role_permission`(
   `id` INT UNSIGNED AUTO_INCREMENT,
   roleId int not null comment '角色ID',
   permissionName varchar(200) not null comment '权限项名称',
   PRIMARY KEY ( `id` ) USING BTREE,
   INDEX `idx_role_permission_roleId`(`roleId`) USING BTREE
)ENGINE=InnoDB DEFAULT CHARSET=utf8;


#用户表
create table if not exists `t_admin_user`(
  `id` int(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `userName` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '用户名',
  `nickName` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '昵称',
  `salt` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT 'password hash salt',
  `passwordHash` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '密码',
  `isValid` tinyint(1) NOT NULL DEFAULT 1 COMMENT '是否有效',
  `isAdmin` tinyint(1) NOT NULL DEFAULT 0 COMMENT '是否管理员',
  `isDeleted` tinyint(1) NOT NULL DEFAULT 0 COMMENT '是否删除',
  `lastModifiedTime` datetime(0) NOT NULL COMMENT '最后修改时间',
  `lastModifier` int(11) NOT NULL DEFAULT 0 COMMENT '最后修改人',
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `idx_admin_user_username_isvalid`(`userName`, `isValid`) USING BTREE
  INDEX `idx_admin_user_username`(`userName`) USING BTREE
)engine=innoDB default charset=utf8;

#多语言
DROP TABLE IF EXISTS `t_localization`;
CREATE TABLE `t_localization`  (
  `id` bigint(20) UNSIGNED NOT NULL AUTO_INCREMENT,
  `culture` varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '语言',
  `resourceKey` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '分类',
  `key` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT 'key',
  `text` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '翻译后的key',
  `lastModifiedTime` datetime(0) NOT NULL COMMENT '最后修改时间',
  `hasTrans` tinyint(1) NOT NULL DEFAULT 0 COMMENT '已经翻译',
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `idx_t_localization_query`(`resourceKey`, `culture`) USING BTREE
)engine=innoDB default charset=utf8;


-- Firma tablosuna yeni boolean alanları ekle
-- İş Takip Sistemi için: isPdks, isAlarm, isKamera alanları

USE `YourDatabaseName`; -- Database adını buraya yazın

-- Safe update mode'u geçici olarak kapat
SET SQL_SAFE_UPDATES = 0;

-- t_firma tablosuna yeni kolonlar ekle (eğer daha önce eklenmemişse)
ALTER TABLE `t_firma`
ADD COLUMN IF NOT EXISTS `IS_PDKS` TINYINT(1) DEFAULT 0,
ADD COLUMN IF NOT EXISTS `IS_ALARM` TINYINT(1) DEFAULT 0,
ADD COLUMN IF NOT EXISTS `IS_KAMERA` TINYINT(1) DEFAULT 0;

-- Varolan kayıtlar için varsayılan değerler ata (tüm kayıtlar için güvenli)
UPDATE `t_firma` 
SET `IS_PDKS` = COALESCE(`IS_PDKS`, 0), 
    `IS_ALARM` = COALESCE(`IS_ALARM`, 0), 
    `IS_KAMERA` = COALESCE(`IS_KAMERA`, 0);

-- Kolonları NOT NULL olarak ayarla
ALTER TABLE `t_firma`
MODIFY COLUMN `IS_PDKS` TINYINT(1) NOT NULL DEFAULT 0,
MODIFY COLUMN `IS_ALARM` TINYINT(1) NOT NULL DEFAULT 0,
MODIFY COLUMN `IS_KAMERA` TINYINT(1) NOT NULL DEFAULT 0;

-- Safe update mode'u tekrar aç
SET SQL_SAFE_UPDATES = 1;

-- Kontrol et
SELECT 
    COUNT(*) as toplam_kayit,
    SUM(`IS_PDKS`) as pdks_aktif,
    SUM(`IS_ALARM`) as alarm_aktif,
    SUM(`IS_KAMERA`) as kamera_aktif
FROM `t_firma`;

SELECT 'Firma tablosuna IS_PDKS, IS_ALARM, IS_KAMERA kolonları başarıyla eklendi ve güncellendi.' AS message;

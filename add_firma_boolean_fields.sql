-- Firma tablosuna yeni boolean alanları ekle
-- İş Takip Sistemi için: isPdks, isAlarm, isKamera alanları

USE [YourDatabaseName] -- Database adını buraya yazın
GO

-- t_firma tablosuna yeni kolonlar ekle
ALTER TABLE t_firma
ADD IS_PDKS BIT DEFAULT 0,
    IS_ALARM BIT DEFAULT 0,
    IS_KAMERA BIT DEFAULT 0
GO

-- Varolan kayıtlar için varsayılan değerler ata
UPDATE t_firma 
SET IS_PDKS = 0, 
    IS_ALARM = 0, 
    IS_KAMERA = 0 
WHERE IS_PDKS IS NULL OR IS_ALARM IS NULL OR IS_KAMERA IS NULL
GO

-- Kolonları NOT NULL olarak ayarla
ALTER TABLE t_firma
ALTER COLUMN IS_PDKS BIT NOT NULL
GO

ALTER TABLE t_firma
ALTER COLUMN IS_ALARM BIT NOT NULL
GO

ALTER TABLE t_firma
ALTER COLUMN IS_KAMERA BIT NOT NULL
GO

PRINT 'Firma tablosuna IS_PDKS, IS_ALARM, IS_KAMERA kolonları başarıyla eklendi.'

-- Firma Cihaz tablosuna log parsing konfigürasyon alanları ekleme
-- SQL dosyası: add_cihaz_log_config_fields.sql

-- Güvenli mod ayarlama
SET SQL_SAFE_UPDATES = 0;

-- t_firma_cihaz tablosuna yeni alanlar ekleme
ALTER TABLE t_firma_cihaz 
ADD COLUMN LOG_PARSER_CONFIG JSON COMMENT 'Log parsing konfigürasyonu (JSON format)',
ADD COLUMN LOG_DELIMITER VARCHAR(10) DEFAULT ',' COMMENT 'Log ayıraç karakteri (virgül, noktalı virgül vs)',
ADD COLUMN LOG_DATE_FORMAT VARCHAR(50) DEFAULT 'ddMMyy' COMMENT 'Tarih formatı pattern',
ADD COLUMN LOG_TIME_FORMAT VARCHAR(50) DEFAULT 'HH:mm' COMMENT 'Saat formatı pattern',
ADD COLUMN LOG_FIELD_MAPPING JSON COMMENT 'Alan pozisyon mapping (JSON)',
ADD COLUMN LOG_SAMPLE TEXT COMMENT 'Örnek log satırı';

-- Güvenli mod tekrar açma
SET SQL_SAFE_UPDATES = 1;

-- Test verisi ekleme örneği
UPDATE t_firma_cihaz 
SET 
    LOG_DELIMITER = ',',
    LOG_DATE_FORMAT = 'ddMMyy',
    LOG_TIME_FORMAT = 'HH:mm',
    LOG_FIELD_MAPPING = JSON_OBJECT(
        'userId', 0,
        'time', 1, 
        'date', 2,
        'direction', 3,
        'deviceId', 4
    ),
    LOG_SAMPLE = '00007,14:00,060125,1,001',
    LOG_PARSER_CONFIG = JSON_OBJECT(
        'type', 'PDKS',
        'skipLines', 0,
        'encoding', 'UTF-8',
        'hasHeader', false,
        'validationRules', JSON_ARRAY(
            JSON_OBJECT('field', 'userId', 'required', true, 'type', 'numeric'),
            JSON_OBJECT('field', 'time', 'required', true, 'pattern', '^([0-1]?[0-9]|2[0-3]):[0-5][0-9]$'),
            JSON_OBJECT('field', 'date', 'required', true, 'pattern', '^[0-9]{6}$')
        )
    )
WHERE FIRMA_CIHAZ_TIP_KID = 1010001; -- QR Model için örnek

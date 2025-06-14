using System;
using System.Collections.Generic;
using LorePdks.COMMON.DTO.LogParser;

namespace LorePdks.BAL.Services.LogParsing.Interfaces
{
    /// <summary>
    /// Log parsing servisi - Farklı cihaz tiplerinden gelen logları merkezi olarak parse eder
    /// </summary>
    public interface ILogParserService
    {
        /// <summary>
        /// Ham log verisini parse ederek yapılandırılmış veri döndürür
        /// </summary>
        /// <param name="rawLogData">Ham log verisi</param>
        /// <param name="parserConfig">JSON formatında parser konfigürasyonu</param>
        /// <returns>Parse edilmiş log verisi</returns>
        Dictionary<string, object> ParseLog(string rawLogData, string parserConfig);

        /// <summary>
        /// PDKS cihazından gelen log verisini parse eder
        /// </summary>
        /// <param name="rawLogData">Ham PDKS log verisi</param>
        /// <param name="parserConfig">Parser konfigürasyonu</param>
        /// <returns>Parse edilmiş PDKS verisi</returns>
        Dictionary<string, object> ParsePdksLog(string rawLogData, string parserConfig);

        /// <summary>
        /// Alarm cihazından gelen log verisini parse eder
        /// </summary>
        /// <param name="rawLogData">Ham alarm log verisi</param>
        /// <param name="parserConfig">Parser konfigürasyonu</param>
        /// <returns>Parse edilmiş alarm verisi</returns>
        Dictionary<string, object> ParseAlarmLog(string rawLogData, string parserConfig);

        /// <summary>
        /// Kamera cihazından gelen log verisini parse eder
        /// </summary>
        /// <param name="rawLogData">Ham kamera log verisi</param>
        /// <param name="parserConfig">Parser konfigürasyonu</param>
        /// <returns>Parse edilmiş kamera verisi</returns>
        Dictionary<string, object> ParseKameraLog(string rawLogData, string parserConfig);

        /// <summary>
        /// Parser konfigürasyonunu test eder
        /// </summary>
        /// <param name="sampleLogData">Test için örnek log verisi</param>
        /// <param name="parserConfig">Test edilecek parser konfigürasyonu</param>
        /// <returns>Test sonucu ve parse edilmiş veri</returns>
        (bool Success, string Message, Dictionary<string, object> ParsedData) TestParserConfig(string sampleLogData, string parserConfig);

        /// <summary>
        /// Parser konfigürasyonunu doğrular
        /// </summary>
        /// <param name="parserConfig">Doğrulanacak parser konfigürasyonu</param>
        /// <returns>Doğrulama sonucu</returns>
        (bool IsValid, string ErrorMessage) ValidateParserConfig(string parserConfig);

        // CRUD Operations for LogParser
        /// <summary>
        /// Log parser konfigürasyonu kaydet
        /// </summary>
        /// <param name="logParserDto">Kaydedilecek log parser DTO</param>
        /// <returns>Kaydedilen log parser DTO</returns>
        LogParserDTO saveLogParser(LogParserDTO logParserDto);

        /// <summary>
        /// ID'ye göre log parser konfigürasyonu getir
        /// </summary>
        /// <param name="id">Log parser ID</param>
        /// <returns>Log parser DTO</returns>
        LogParserDTO getLogParserById(int id);

        /// <summary>
        /// Firmaya göre log parser konfigürasyonları getir
        /// </summary>
        /// <param name="firmaId">Firma ID</param>
        /// <returns>Log parser DTO listesi</returns>
        List<LogParserDTO> getLogParserListByFirmaId(int firmaId);

        /// <summary>
        /// Tüm log parser konfigürasyonları getir
        /// </summary>
        /// <returns>Log parser DTO listesi</returns>
        List<LogParserDTO> getAllLogParsers();

        /// <summary>
        /// Konfigürasyon ID'si ile log verisi parse et
        /// </summary>
        /// <param name="rawLogData">Ham log verisi</param>
        /// <param name="configId">Konfigürasyon ID</param>
        /// <returns>Parse edilmiş log verisi</returns>
        Dictionary<string, object> parseLogData(string rawLogData, int configId);

        /// <summary>
        /// Log parser konfigürasyonunu test et
        /// </summary>
        /// <param name="sampleLogData">Test için örnek log verisi</param>
        /// <param name="config">Test edilecek konfigürasyon JSON</param>
        /// <returns>Test sonucu</returns>
        Dictionary<string, object> testLogParserConfig(string sampleLogData, string config);        /// <summary>
        /// Log parser konfigürasyonu sil
        /// </summary>
        /// <param name="id">Silinecek log parser ID</param>
        void deleteLogParser(int id);

        /// <summary>
        /// Sistem şablonlarını getirir
        /// </summary>
        /// <returns>Sistem şablonları listesi</returns>
        List<LogParserTemplateDTO> GetSystemTemplates();

        /// <summary>
        /// Cihaz tipine göre şablonları filtreler
        /// </summary>
        /// <param name="deviceType">Cihaz tipi (PDKS, ALARM, KAMERA)</param>
        /// <returns>Filtrelenmiş şablon listesi</returns>
        List<LogParserTemplateDTO> GetTemplatesByDeviceType(string deviceType);

        /// <summary>
        /// Şablondan konfigürasyon oluşturur
        /// </summary>
        /// <param name="template">Şablon verisi</param>
        /// <returns>JSON formatında parser konfigürasyonu</returns>
        string CreateConfigFromTemplate(LogParserTemplateDTO template);

        /// <summary>
        /// Akıllı örnek veri analizi yaparak alan eşlemesi önerir
        /// </summary>
        /// <param name="sampleData">Örnek log verisi</param>
        /// <param name="delimiter">Ayırıcı karakter</param>
        /// <returns>Önerilen alan eşlemeleri</returns>
        List<FieldMappingDetail> AnalyzeSampleDataAndSuggestMappings(string sampleData, string delimiter);
    }
}

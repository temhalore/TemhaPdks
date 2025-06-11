using System;
using System.Collections.Generic;

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
    }
}

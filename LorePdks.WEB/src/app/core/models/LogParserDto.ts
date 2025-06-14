/**
 * Log parser konfigürasyon bilgilerini temsil eden modeller
 */

export interface LogParserConfigDto {
  delimiter: string;
  dateFormat: string;
  timeFormat: string;
  regexPattern?: string;
  fieldMapping: FieldMappingDto[];
  sampleLogData?: string;  // Örnek log verisi
}

export interface FieldMappingDto {
  name: string;      // Alan adı (userId, date, time, vb.)
  index: number;     // Pozisyon (0, 1, 2...)
  type: string;      // Veri tipi (string, number, date, time, datetime)
  format?: string;   // Format (hh:mm:ss, dd.MM.yyyy vb.)
}

export interface LogParserTestResultDto {
  success: boolean;
  message: string;
  parsedData?: { [key: string]: any };
}

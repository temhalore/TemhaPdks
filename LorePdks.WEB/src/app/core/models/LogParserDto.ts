/**
 * Log parser konfigürasyon bilgilerini temsil eden modeller
 */

export interface LogParserConfigDto {
  delimiter: string;
  dateFormat: string;
  timeFormat: string;
  regexPattern?: string;
  fieldMapping: FieldMappingDto[];
}

export interface FieldMappingDto {
  name: string;
  index: number;
  type: string;
  format?: string;
}

export interface LogParserTestResultDto {
  success: boolean;
  message: string;
  parsedData?: { [key: string]: any };
}

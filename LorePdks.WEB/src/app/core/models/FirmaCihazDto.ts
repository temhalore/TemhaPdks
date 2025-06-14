import { baseDto } from "./baseDto";
import { FirmaDto } from "./FirmaDto";
import { KodDto } from "./KodDto";

/**
 * Firma cihaz bilgisini temsil eden model
 */
export class FirmaCihazDto extends baseDto {
  firmaDto: FirmaDto;
  cihazMakineGercekId: number;
  firmaCihazTipKodDto: KodDto;
  ad: string;
  aciklama: string;
  
  // Log Parser özellikleri
  logParserConfig?: string;
  logDelimiter?: string;
  logDateFormat?: string;
  logTimeFormat?: string;
  logFieldMapping?: string;
  logSample?: string;
  logSampleData?: string;
}
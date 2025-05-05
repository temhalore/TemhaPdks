import { baseDto } from "./baseDto";
import { FirmaDto } from "./FirmaDto";
import { KodDto } from "./KodDto";

/**
 * Firma cihaz bilgisini temsil eden model
 */
export interface FirmaCihazDto extends baseDto {
  firmaDto: FirmaDto;
  cihazMakineGercekId: number;
  firmaCihazTipKodDto: KodDto;
  ad: string;
  aciklama: string;
}
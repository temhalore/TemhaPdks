import { baseDto } from "./baseDto";
import { FirmaDto } from "./FirmaDto";
import { KisiDto } from "./KisiDto";
import { KodDto } from "./KodDto";

/**
 * Firma kişi bilgisini temsil eden model
 */
export interface FirmaKisiDto extends baseDto {
  firmaDto: FirmaDto;
  kisiDto: KisiDto;
  firmaKisiTipKodDto: KodDto;
}
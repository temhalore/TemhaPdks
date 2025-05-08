import { baseDto } from "./baseDto";
import { FirmaDto } from "./FirmaDto";
import { KodDto } from "./KodDto";

/**
 * Hareket bilgisini temsil eden model
 */
export class HareketDto extends baseDto {
  firmaDto: FirmaDto;
  hareketTipKodDto: KodDto;
  hareketDurumKodDto: KodDto;
  hareketKayitTarih?: Date;
  hareketIslemeTarih?: Date;
  hareketdata: string;
}
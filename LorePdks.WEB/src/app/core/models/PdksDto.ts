import { baseDto } from "./baseDto";
import { FirmaDto } from "./FirmaDto";
import { FirmaCihazDto } from "./FirmaCihazDto";
import { KisiDto } from "./KisiDto";

/**
 * Pdks bilgisini temsil eden model
 */
export interface PdksDto extends baseDto {
  firmaDto: FirmaDto;
  kisiDto: KisiDto;
  firmaCihazDto: FirmaCihazDto;
  girisTarih?: Date;
  cikisTarih?: Date;
}
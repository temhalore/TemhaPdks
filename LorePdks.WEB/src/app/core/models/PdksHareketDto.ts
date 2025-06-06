import { baseDto } from "./baseDto";
import { HareketDto } from "./HareketDto";
import { PdksDto } from "./PdksDto";

/**
 * Pdks hareket bilgisini temsil eden model
 */
export class PdksHareketDto extends baseDto {
  pdksDto: PdksDto;
  hareketDto: HareketDto;
}
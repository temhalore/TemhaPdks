import { baseDto } from "./baseDto";

/**
 * Firma bilgisini temsil eden model
 */
export class FirmaDto extends baseDto {
  ad: string;
  kod: string;
  aciklama: string;
  adres: string;
  mesaiSaat?: number;
  molaSaat?: number;  cumartesiMesaiSaat?: number;
  cumartesiMolaSaat?: number;
  isPdks: boolean;
  isAlarm: boolean;
  isKamera: boolean;
}
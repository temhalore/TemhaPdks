import { baseDto } from "./baseDto";

/**
 * Kod bilgisini temsil eden model
 */
export interface KodDto extends baseDto {
  tipId: number;
  kod: string;
  kisaAd: string;
  sira: number;
  digerUygEnumAd: string;
  digerUygEnumDeger: number;
}
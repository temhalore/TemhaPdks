import { baseDto } from "./baseDto";

/**
 * Kod bilgisini temsil eden model
 */
export class KodDto  {
  id: number;
  tipId: number;
  kod: string;
  kisaAd: string;
  sira: number;
  digerUygEnumAd: string;
  digerUygEnumDeger: number;
}
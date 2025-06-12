import { baseDto } from "./baseDto";

/**
 * Kamera hareket bilgisini temsil eden model
 */
export class KameraHareketDto extends baseDto {
  firmaEid: string;
  cihazNumarasi: string;
  eventTipi: number;
  eventTarihi: Date;
  aciklama: string;
  lokasyon: string;
  kullaniciEid: string;
  dosyaYolu: string;
  ip: string;
  rawData: string;
  processedAt: Date;
  isProcessed: boolean;
  
  // Navigation properties
  firmaAdi: string;
  eventTipiAdi: string;
  kullaniciAdi: string;
}

import { baseDto } from "./baseDto";

/**
 * Alarm hareket bilgisini temsil eden model
 */
export class AlarmHareketDto extends baseDto {
  firmaEid: string;
  cihazNumarasi: string;
  alarmTarihi: Date;
  alarmTipi: number;
  aciklama: string;
  lokasyon: string;
  kullaniciEid: string;
  isGercekAlarm: boolean;
  rawData: string;
  processedAt: Date;
  isProcessed: boolean;
  
  // Navigation properties
  firmaAdi: string;
  alarmTipiAdi: string;
  kullaniciAdi: string;
}

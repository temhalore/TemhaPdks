import e from "express";
import { baseDto } from "./baseDto";

/**
 * Kullanıcı bilgisini temsil eden model
 */
export interface EkranDTO extends baseDto {
 
  ekranAdi: string;
  ekranYolu: string;
  ekranKodu: string;
  aciklama: string;
  ustEkranId?: number;
  siraNo?: number;
  ikon: string;
  aktif: boolean;
  altEkranlar: EkranDTO[];
}


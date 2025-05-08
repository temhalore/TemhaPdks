import { baseDto } from './baseDto';

/**
 * Ekran bilgilerini temsil eden model
 */
export class EkranDto extends baseDto {
  ekranAdi: string;
  ekranYolu: string;
  ekranKodu: string;
  aciklama: string;
  ustEkranEidDto: baseDto;
  siraNo: number;
  ikon: string;
  aktif: boolean;
  altEkranlar: EkranDto[];
}
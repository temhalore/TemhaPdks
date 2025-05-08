import { baseDto } from './baseDto';
import { EkranDto } from './EkranDto';

/**
 * Rol bilgilerini temsil eden model
 */
export class RolDto extends baseDto {
  rolAdi: string;
  aciklama: string;
  ekranlar: EkranDto[];
}
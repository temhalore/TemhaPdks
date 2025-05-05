import { baseDto } from './baseDto';
import { EkranDto } from './EkranDto';

/**
 * Rol bilgilerini temsil eden model
 */
export interface RolDto extends baseDto {
  rolAdi: string;
  aciklama: string;
  controllerName: string;
  controllerMethodName: string;
  ekranlar: EkranDto[];
}
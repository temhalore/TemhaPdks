import { baseDto } from './baseDto';

/**
 * Kişi rol bilgilerini temsil eden model
 */
export interface KisiRolDto extends baseDto {
  kisiEidDto: baseDto;
  rolEidDto: baseDto;
  rolAdi: string;
}
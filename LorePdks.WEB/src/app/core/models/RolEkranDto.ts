import { baseDto } from './baseDto';

/**
 * Rol ekran ilişkisini temsil eden model
 */
export interface RolEkranDto extends baseDto {
  rolEidDto: baseDto;
  ekranEidDto: baseDto;
}
import { baseDto } from './baseDto';

/**
 * Rol controller method bilgilerini temsil eden model
 */
export interface RolControllerMethodDto extends baseDto {
  rolEidDto: baseDto;
  controllerName: string;
  methodName: string;
}
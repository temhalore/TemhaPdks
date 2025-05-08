import { baseDto } from './baseDto';

/**
 * Rol controller method bilgilerini temsil eden model
 */
export class RolControllerMethodDto extends baseDto {
  rolEidDto: baseDto;
  controllerName: string;
  methodName: string;
}
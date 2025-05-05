import { baseDto } from './baseDto';

/**
 * Rol controller methods request bilgilerini temsil eden model
 */
export interface RolControllerMethodsRequestDto {
  rolEidDto: baseDto;
  controllerMethods: any[]; // ControllerAndMethodsDto siz tarafından oluşturulacağı için any[] olarak bırakıyorum
}
import { baseDto } from "./baseDto";
import { EkranDTO } from "./EkranDTO";

/**
 * Kullanıcı bilgisini temsil eden model
 */
export interface RolDto extends baseDto{

  rolAdi: string;
  aciklama: string;
  controllerName?: string;
  controllerMethodName?: string;
  ekranlar?: EkranDTO[];

}


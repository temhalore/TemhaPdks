import { baseDto } from "./baseDto";
import { EkranDto } from "./EkranDto";
import { KisiDto } from "./KisiDto";
import { RolDto } from "./RolDto";

/**
 * Kullanıcı token bilgisini temsil eden model
 */
export class KisiTokenDto extends baseDto {
  loginName: string;
  token: string;
  ipAdresi: string;
  userAgent: string;
  expDate?: Date;
  isLogin: boolean;
  kisiDto?: KisiDto;
  ekranDtoList?: EkranDto[];
}


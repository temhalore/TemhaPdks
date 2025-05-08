import { baseDto } from "./baseDto";

/**
 * Kullanıcı bilgisini temsil eden model
 */
export class KisiDto extends baseDto {
  ad: string;
  soyad: string;
  tc?: string;
  cepTel?: string;
  email?: string;
  loginName: string;
  sifre?: string;
}
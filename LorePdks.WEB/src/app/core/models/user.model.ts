/**
 * Kullanıcı token bilgisini temsil eden model
 */
export interface KisiToken {
  id: number;
  kisiId: number;
  loginName: string;
  token: string;
  ipAdresi: string;
  userAgent: string;
  expDate?: Date;
  isLogin: boolean;
  kisiDto?: Kisi;
}

/**
 * Kullanıcı bilgisini temsil eden model
 */
export interface Kisi {
  id: number;
  ad: string;
  soyad: string;
  tc?: string;
  cepTel?: string;
  email?: string;
  loginName: string;
}
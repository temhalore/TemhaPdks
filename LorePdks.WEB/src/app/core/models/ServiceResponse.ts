/**
 * API'den gelen yanıtları temsil eden model sınıfı
 */
export interface ServiceResponse<T> {
  IsSuccess: boolean;
  message: string;
  data: T;
  errorCode?: number;
}
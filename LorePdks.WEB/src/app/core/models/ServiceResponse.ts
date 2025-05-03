/**
 * API'den gelen yanıtları temsil eden model sınıfı
 */
export interface ServiceResponse<T> {
  isSuccess: boolean;
  message: string;
  messageType?: string;
  data: T;
  errorCode?: number;
}
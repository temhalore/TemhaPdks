/**
 * DataTable yanıt bilgilerini temsil eden model
 */
export interface DataTableResponseDto<T> {
  data: T[];
  draw: number;
  recordsFiltered: number;
  recordsTotal: number;
}

/**
 * DataTable genel yanıt bilgilerini temsil eden model
 */
export interface DataTableResponse {
  data: any[];
  recordsTotal: number;
}
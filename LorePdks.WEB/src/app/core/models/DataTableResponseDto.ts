/**
 * DataTable yanıt bilgilerini temsil eden model
 */
export class DataTableResponseDto<T> {
  data: T[];
  draw: number;
  recordsFiltered: number;
  recordsTotal: number;
}

/**
 * DataTable genel yanıt bilgilerini temsil eden model
 */
export class DataTableResponse {
  data: any[];
  recordsTotal: number;
}
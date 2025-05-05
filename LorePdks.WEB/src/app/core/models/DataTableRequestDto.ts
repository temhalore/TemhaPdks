/**
 * DataTable kolon bilgilerini temsil eden model
 */
export interface DataTableColumns {
  data: string;
  name: string;
  orderable: boolean;
  searchable: boolean;
}

/**
 * DataTable sıralama bilgilerini temsil eden model
 */
export interface DataTableOrder {
  column: number;
  dir: string;
}

/**
 * DataTable arama bilgilerini temsil eden model
 */
export interface DataTableSearch {
  regex: boolean;
  value: string;
}

/**
 * DataTable istek bilgilerini temsil eden model
 */
export interface DataTableRequestDto<T> {
  columns: DataTableColumns[];
  start: number;
  length: number;
  draw: number;
  order: DataTableOrder[];
  search: DataTableSearch;
  data: T;
}
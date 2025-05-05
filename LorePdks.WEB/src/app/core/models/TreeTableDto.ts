/**
 * Ağaç tablosu yapısını temsil eden model
 */
export interface TreeTableDto<T> {
  data: T;
  expanded: boolean;
  children: TreeTableDto<T>[];
}
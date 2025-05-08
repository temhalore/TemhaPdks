/**
 * Ağaç tablosu yapısını temsil eden model
 */
export class TreeTableDto<T> {
  data: T;
  expanded: boolean;
  children: TreeTableDto<T>[];
}
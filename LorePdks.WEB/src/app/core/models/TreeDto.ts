/**
 * Ağaç yapısını temsil eden model
 */
export interface TreeDto {
  label: string;
  data: any;
  expandedIcon: string;
  collapsedIcon: string;
  children: TreeDto[];
}
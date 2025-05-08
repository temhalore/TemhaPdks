/**
 * Ağaç yapısını temsil eden model
 */
export class TreeDto {
  label: string;
  data: any;
  expandedIcon: string;
  collapsedIcon: string;
  children: TreeDto[];
}
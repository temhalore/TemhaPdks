import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { TooltipModule } from 'primeng/tooltip';

/**
 * Aksiyon buton konfigürasyonu
 */
export interface ActionButtonConfig {
  icon: string;            // Button ikonu
  tooltip: string;         // Tooltip metni
  action: string;          // Aksiyon tanımlayıcı
  class?: string;          // Ek CSS sınıfları
}

/**
 * TreeNode interfacei - ağaç veri yapısı için temel arayüz
 */
export interface TreeNode<T = any> {
  data: T;                        // Node verileri
  expanded?: boolean;             // Node genişletilmiş mi?
  parent?: TreeNode<T> | null;    // Üst node
  children?: TreeNode<T>[];       // Alt nodlar
  leaf?: boolean;                 // Yaprak node mu? (alt öğesi yok)
  id?: string | number;           // Benzersiz tanımlayıcı
  icon?: string;                  // Node ikonu
  label?: string;                 // Gösterilecek etiket
  level?: number;                 // Ağaç seviyesi (kök = 0)
  selectable?: boolean;           // Seçilebilir mi?
  selected?: boolean;             // Seçili mi?
}

/**
 * TreeNodeTemplateContext - Özel şablonlar için context
 */
export interface TreeNodeTemplateContext<T = any> {
  $implicit: TreeNode<T>;
  node: TreeNode<T>;
}

/**
 * TreeView bileşeni
 * Hiyerarşik veri yapılarını göstermek için kullanılır
 */
@Component({
  selector: 'app-tree-view',
  templateUrl: './tree-view.component.html',
  styleUrls: ['./tree-view.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ButtonModule,
    TooltipModule
  ]
})
export class TreeViewComponent implements OnInit {
  // Giriş özellikleri
  @Input() nodes: TreeNode[] = [];
  @Input() selectionMode: 'single' | 'multiple' | 'none' = 'single';
  @Input() expandedIcon: string = 'pi pi-chevron-down';
  @Input() collapsedIcon: string = 'pi pi-chevron-right';
  @Input() leafIcon: string = 'pi pi-file';
  @Input() nodeIcon: string = 'pi pi-folder';
  @Input() loading: boolean = false;
  @Input() emptyMessage: string = 'Veri bulunamadı';
  @Input() actionButtons: ActionButtonConfig[] = [];
  
  // Çıkış olayları
  @Output() nodeSelect = new EventEmitter<TreeNode>();
  @Output() nodeUnselect = new EventEmitter<TreeNode>();
  @Output() nodeExpand = new EventEmitter<TreeNode>();
  @Output() nodeCollapse = new EventEmitter<TreeNode>();
  @Output() nodeContextMenu = new EventEmitter<{originalEvent: MouseEvent, node: TreeNode}>();
  @Output() nodeDoubleClick = new EventEmitter<{originalEvent: MouseEvent, node: TreeNode}>();
  @Output() nodeAction = new EventEmitter<{action: string, node: TreeNode}>();
  
  // İç değişkenler
  selectedNodes: TreeNode[] = [];

  constructor() { }
  
  ngOnInit(): void {
    // Nodeların seviyelerini başlangıçta belirleyelim
    this.updateNodeLevels(this.nodes);
  }
  
  /**
   * Node üzerinde tıklama olayını işler
   * @param event Mouse event
   * @param node Tıklanan node
   */
  onNodeClick(event: MouseEvent, node: TreeNode): void {
    if (this.selectionMode === 'none') return;
    
    if (node.selectable === false) return;
    
    if (this.selectionMode === 'single') {
      // Tüm nodeları seçilmemiş yap
      this.clearSelection();
      
      // Seçili nodu işaretle
      node.selected = true;
      this.selectedNodes = [node];
      this.nodeSelect.emit(node);
    } else if (this.selectionMode === 'multiple') {
      // CTRL tuşu basılı değilse ve node seçili değilse, önce tüm seçimleri temizle
      if (!event.ctrlKey && !node.selected) {
        this.clearSelection();
      }
      
      // Node'un durumunu değiştir
      if (node.selected) {
        node.selected = false;
        this.selectedNodes = this.selectedNodes.filter(n => n !== node);
        this.nodeUnselect.emit(node);
      } else {
        node.selected = true;
        this.selectedNodes.push(node);
        this.nodeSelect.emit(node);
      }
    }
  }
  
  /**
   * Node'un durumunu değiştirilebilir mi kontrolü
   * @param node TreeNode
   * @returns Boolean
   */
  isNodeSelectable(node: TreeNode): boolean {
    return node.selectable !== false;
  }
  
  /**
   * Genişlet/Daralt ikonuna tıklama olayını işler
   * @param event Mouse event
   * @param node Tıklanan node
   */
  onToggleClick(event: MouseEvent, node: TreeNode): void {
    event.stopPropagation();
    
    if (node.leaf) return;
    
    if (node.expanded) {
      node.expanded = false;
      this.nodeCollapse.emit(node);
    } else {
      node.expanded = true;
      this.nodeExpand.emit(node);
    }
  }
  
  /**
   * Node çift tıklama olayını işler
   * @param event Mouse event
   * @param node Tıklanan node
   */
  onNodeDoubleClick(event: MouseEvent, node: TreeNode): void {
    this.nodeDoubleClick.emit({ originalEvent: event, node });
    
    // Eğer yaprak değilse, genişlet/daralt
    if (!node.leaf) {
      this.onToggleClick(event, node);
    }
  }
  
  /**
   * Tüm node seçimlerini temizler
   */
  clearSelection(): void {
    this.selectedNodes.forEach(node => {
      node.selected = false;
    });
    this.selectedNodes = [];
  }
  
  /**
   * Bir node'un seviyesini belirler ve alt nodelarına yayar
   * @param nodes Node listesi
   * @param level Seviye (varsayılan: 0)
   * @param parent Üst node (kök için null)
   */
  updateNodeLevels(nodes: TreeNode[], level: number = 0, parent: TreeNode | null = null): void {
    if (!nodes) return;
    
    nodes.forEach(node => {
      // Node seviyesini ayarla
      node.level = level;
      
      // Üst nodu belirt
      node.parent = parent;
      
      // Yaprak olup olmadığını kontrol et
      node.leaf = !node.children || node.children.length === 0;
      
      // Alt nodelar için recursive çağrı
      if (node.children && node.children.length > 0) {
        this.updateNodeLevels(node.children, level + 1, node);
      }
    });
  }
  
  /**
   * Node için uygun ikonu döndürür
   * @param node TreeNode
   * @returns İkon sınıfı
   */
  getNodeIcon(node: TreeNode): string {
    if (node.icon) return node.icon;
    
    if (node.leaf) return this.leafIcon;
    
    return this.nodeIcon;
  }
    /**
   * Genişlet/Daralt ikonu döndürür
   * @param node TreeNode
   * @returns İkon sınıfı
   */
  getToggleIcon(node: TreeNode): string {
    if (node.leaf) return '';
    
    return node.expanded ? this.expandedIcon : this.collapsedIcon;
  }
  
  /**
   * İşlem butonuna tıklama olayını işler
   * @param event Mouse event
   * @param node İşlem yapılacak node
   * @param action Buton aksiyonu
   */
  onActionButtonClick(event: MouseEvent, node: TreeNode, action: string): void {
    event.stopPropagation();
    
    // Node aksiyonunu emit et
    this.nodeAction.emit({ action, node });
  }
}

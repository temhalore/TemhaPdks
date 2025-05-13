import { Component, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { InputNumberModule } from 'primeng/inputnumber';
import { CheckboxModule } from 'primeng/checkbox';

import { ConfirmDialogComponent } from '../../../core/components/confirm-dialog/confirm-dialog.component';
import { DataGridComponent, ActionButtonConfig } from '../../../core/components/data-grid/data-grid.component';
import { ModalComponent } from '../../../core/components/modal/modal.component';
import { SelectComponent } from '../../../core/components/select/select.component';
import { TextInputComponent } from '../../../core/components/text-input/text-input.component';
import { TreeViewComponent, TreeNode, ActionButtonConfig as TreeActionButtonConfig } from '../../../core/components/tree-view/tree-view.component';
import { YetkiService } from '../../../core/services/modules/yetki.service';
import { EkranDto } from '../../../core/models/EkranDto';
import { BehaviorSubject, Observable, finalize, of } from 'rxjs';
import { SelectInputModel } from '../../../core/components/select/select-input.model';

@Component({
  selector: 'app-ekran',
  templateUrl: './ekran.component.html',
  styleUrls: ['./ekran.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ButtonModule,
    InputTextModule,
    InputTextareaModule,
    InputNumberModule,
    CheckboxModule,
    DataGridComponent,
    ModalComponent,
    ConfirmDialogComponent,
    SelectComponent,
    TextInputComponent,
    TreeViewComponent
  ]
})
export class EkranComponent implements OnInit {
  // Ekran listesi
  ekranList: EkranDto[] = [];
  loading: boolean = false;
  
  // Tree görünümü için hiyerarşik veri
  treeNodes: TreeNode[] = [];
  
  // Görünüm modu
  viewMode: 'grid' | 'tree' = 'grid';
    // İşlem butonları tanımı
  actionButtons: ActionButtonConfig[] = [
    { icon: 'pi pi-pencil', tooltip: 'Düzenle', action: 'edit' },
    { icon: 'pi pi-trash', tooltip: 'Sil', action: 'delete', class: 'p-button-danger' }
  ];
  
  // Tree görünümü için işlem butonları
  treeActionButtons: TreeActionButtonConfig[] = [
    { icon: 'pi pi-pencil', tooltip: 'Düzenle', action: 'edit' },
    { icon: 'pi pi-trash', tooltip: 'Sil', action: 'delete', class: 'p-button-danger' }
  ];
  
  // Data grid sütun tanımları
  columns = [
    { field: 'ekranAdi', header: 'Ekran Adı' },
    { field: 'ekranYolu', header: 'Ekran Yolu' },
    { field: 'ekranKodu', header: 'Ekran Kodu' },
    { field: 'siraNo', header: 'Sıra No' },
    { field: 'aciklama', header: 'Açıklama' },
    { field: 'aktif', header: 'Aktif', customTemplate: 'aktifTemplate' }
  ];
  
  // Modal durum bayrakları
  ekranModalVisible: boolean = false;
  
  // Seçili ekran ve form modeli
  selectedEkran: EkranDto | null = null;
  ekranModel: EkranDto = new EkranDto();
  // Üst ekran seçimi için
  ustEkranListDto$: BehaviorSubject<SelectInputModel[]> = new BehaviorSubject<SelectInputModel[]>([]);
  selectedUstEkranId: string | null = null;
  loadingUstEkranlar$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  
  // Onay dialogu
  @ViewChild(ConfirmDialogComponent) confirmDialog!: ConfirmDialogComponent;
  
  constructor(private yetkiService: YetkiService) { 
    // Modeli başlat
    this.ekranModel = new EkranDto();
    this.ekranModel.aktif = true;
  }
  
  ngOnInit(): void {
    this.loadEkranList();
  }  /**
   * Ekran listesini yükler
   */
  loadEkranList(): void {
    this.loading = true;
    this.yetkiService.getAllEkranList()
      .pipe(finalize(() => this.loading = false))
      .subscribe({
        next: (data) => {
          this.ekranList = data;
          // Ağaç görünümü için veriyi dönüştür
          this.treeNodes = this.prepareTreeNodes(data);
        },
        error: (err) => {
          console.error('Ekran listesi yüklenirken hata oluştu:', err);
        }
      });
  }
  
  /**
   * Ağaç görünümü için veriyi hazırlar
   * @param ekranList Ekran listesi
   * @returns TreeNode listesi
   */
  prepareTreeNodes(ekranList: EkranDto[]): TreeNode[] {
    // Tüm ekranları ID'ye göre hızlı erişim için map oluşturalım
    const ekranMap = new Map<string, EkranDto>();
    ekranList.forEach(ekran => {
      ekranMap.set(ekran.eid, ekran);
    });
    
    // Sonuç nodes dizisi
    const nodes: TreeNode[] = [];
    
    // Kök ekranları bulalım (üst ekranı olmayanlar)
    const rootEkranlar = ekranList.filter(e => !e.ustEkranEidDto || !e.ustEkranEidDto.eid);
    
    // Kök ekranlardan nodeları oluşturalım
    rootEkranlar.forEach(rootEkran => {
      const node = this.createTreeNode(rootEkran, ekranList, ekranMap);
      nodes.push(node);
    });
    
    return nodes;
  }
  
  /**
   * Bir ekran için TreeNode oluşturur
   * @param ekran Ekran verisi
   * @param ekranList Tüm ekranların listesi
   * @param ekranMap Ekran ID'ye göre map
   * @returns TreeNode
   */
  createTreeNode(ekran: EkranDto, ekranList: EkranDto[], ekranMap: Map<string, EkranDto>): TreeNode {
    // Alt ekranları bulalım
    const childEkranlar = ekranList.filter(e => 
      e.ustEkranEidDto && 
      e.ustEkranEidDto.eid === ekran.eid
    );
    
    // TreeNode nesnesi oluştur
    const node: TreeNode = {
      data: ekran,
      id: ekran.eid,
      label: ekran.ekranAdi,
      icon: ekran.ikon || 'pi pi-folder',
      selectable: true,
      expanded: false,
      children: []
    };
    
    // Alt ekranları recursive olarak ekleyelim
    if (childEkranlar.length > 0) {
      childEkranlar.forEach(childEkran => {
        const childNode = this.createTreeNode(childEkran, ekranList, ekranMap);
        node.children!.push(childNode);
      });
      
      // Alt öğesi varsa yaprak değildir
      node.leaf = false;
    } else {
      // Alt öğesi yoksa yapraktır
      node.leaf = true;
      node.icon = 'pi pi-file';
    }
    
    return node;
  }
    /**
   * Yeni ekran ekleme modalını açar
   */
  openAddEkranModal(): void {
    this.ekranModel = new EkranDto();
    this.ekranModel.aktif = true;
    this.ekranModel.ekranAdi = '';
    this.ekranModel.ekranYolu = '';
    this.ekranModel.ekranKodu = '';
    this.ekranModel.aciklama = '';
    this.ekranModel.ikon = '';
    this.ekranModel.siraNo = 0;
    this.selectedUstEkranId = null;
    this.loadUstEkranlar();
    this.ekranModalVisible = true;
  }
  /**
   * Ekran düzenleme modalını açar
   * @param ekran Düzenlenecek ekran
   */
  openEditEkranModal(ekran: EkranDto): void {
    this.selectedEkran = ekran;
    this.ekranModel = { ...ekran }; // Ekran bilgilerini doğrudan kopyala
    
    // Eksik bilgileri kontrol et ve varsayılan değerlerle doldur
    this.ekranModel.ekranAdi = this.ekranModel.ekranAdi || '';
    this.ekranModel.ekranYolu = this.ekranModel.ekranYolu || '';
    this.ekranModel.ekranKodu = this.ekranModel.ekranKodu || '';
    this.ekranModel.aciklama = this.ekranModel.aciklama || '';
    this.ekranModel.ikon = this.ekranModel.ikon || '';
    this.ekranModel.siraNo = this.ekranModel.siraNo || 0;
    this.ekranModel.aktif = this.ekranModel.aktif !== undefined ? this.ekranModel.aktif : true;
      // Üst ekran ID'si varsa al, yoksa null ata
    const ustEkranId = ekran.ustEkranEidDto?.eid ? String(ekran.ustEkranEidDto.eid) : null;
    console.log('Düzenlenen ekranın üst ekran ID\'si:', ustEkranId);
    this.selectedUstEkranId = ustEkranId;
    
    // Önce üst ekranlar yüklensin, sonra modal açılsın
    this.loadUstEkranlar();
    this.ekranModalVisible = true;
  }
  
  /**
   * Ekran silme onay dialogunu gösterir
   * @param ekran Silinecek ekran
   */
  confirmDeleteEkran(ekran: EkranDto): void {
    this.selectedEkran = ekran;
    this.confirmDialog.header = 'Ekran Silme Onayı';
    this.confirmDialog.message = `"${ekran.ekranAdi}" ekranını silmek istediğinize emin misiniz?`;
    this.confirmDialog.show();
  }
  
  /**
   * Onay sonrası ekran silme işlemini gerçekleştirir
   */
  deleteEkran(): void {
    // YetkiService'te ekran silme endpoint'i olsaydı burada kullanılırdı
    // Şu an için, bu işlevsellik eksik görünüyor
    console.error('Ekran silme fonksiyonu henüz implemente edilmemiş');
  }
  /**
   * Ekran kaydetme/güncelleme işlemini gerçekleştirir
   */
  saveEkran(): void {
    console.log('Kaydedilecek ekran modeli:', JSON.stringify(this.ekranModel));
    console.log('Seçilen üst ekran ID:', this.selectedUstEkranId);
    
    // Üst ekran seçimini ayarla
    if (this.selectedUstEkranId) {
      this.ekranModel.ustEkranEidDto = { eid: this.selectedUstEkranId };
      console.log('Üst ekran ataması yapıldı:', this.ekranModel.ustEkranEidDto);
    } else {
      this.ekranModel.ustEkranEidDto = null as any;
      console.log('Üst ekran ataması temizlendi');
    }
    
    // Güncelleme kontrolü için eid bilgisini doğru şekilde ayarla
    if (this.selectedEkran && this.selectedEkran.eid) {
      this.ekranModel.eid = this.selectedEkran.eid;
      console.log('Mevcut ekran güncelleniyor, EID:', this.ekranModel.eid);
    } else {
      console.log('Yeni ekran oluşturuluyor');
    }
    
    // Ekran kaydet servis çağrısı
    this.yetkiService.saveEkran(this.ekranModel)
      .subscribe({
        next: (response) => {
          console.log('Ekran başarıyla kaydedildi:', response);
          
          // Başarılı kaydetme sonrası temizlik ve yeniden yükleme
          this.ekranModalVisible = false;
          this.selectedEkran = null;
          this.loadEkranList();
        },
        error: (err) => {
          console.error('Ekran kaydedilirken hata oluştu:', err);
        }
      });
  }
  
  /**
   * Modal kapatıldığında seçili ekranı temizler
   */
  onEkranModalClosed(): void {
    this.selectedEkran = null;
  }
  /**
   * Üst ekran seçimi için ekran listesini yükler
   */
  loadUstEkranlar(): void {
    this.loadingUstEkranlar$.next(true);
    this.yetkiService.getAllEkranList()
      .pipe(finalize(() => this.loadingUstEkranlar$.next(false)))
      .subscribe({
        next: (data) => {
          console.log('Tüm ekranlar:', data);  // Debug amaçlı log

          // Kendisini üst ekran olarak seçememeli
          const filteredData = this.selectedEkran 
            ? data.filter(e => e.eid !== this.selectedEkran!.eid) 
            : data;
          
          console.log('Filtrelenmiş ekranlar:', filteredData);  // Debug amaçlı log
            // Select component için uygun formata dönüştür
          const selectItems = filteredData.map(e => ({
            key: String(e.eid), // Ekran ID'sini string'e dönüştür
            value: e.ekranAdi || 'İsimsiz Ekran'  // Null veya boş değer kontrolü ekledik
          }));
          
          console.log('Select için hazırlanan veri:', selectItems);  // Debug amaçlı log
          
          // BehaviorSubject'e ekran listesini aktar
          this.ustEkranListDto$.next(selectItems);
        },
        error: (err) => {
          console.error('Üst ekranlar yüklenirken hata oluştu:', err);
          // Hata durumunda bile en azından boş bir liste göster
          this.ustEkranListDto$.next([]);
        }
      });
  }
    /**
   * Üst ekran seçim değişikliğini işler
   * @param event Seçim değişikliği olayı
   */
  onUstEkranChange(value: any): void {
    console.log('Üst ekran seçimi değişti:', value);
    this.selectedUstEkranId = value;
    
    // Seçilen üst ekranın adını bul ve göster (debug amaçlı)
    if (value) {
      const secilenEkran = this.ekranList.find(e => e.eid === value);
      if (secilenEkran) {
        console.log('Seçilen üst ekran:', secilenEkran.ekranAdi);
      } else {
        console.log('Seçilen ID\'ye sahip ekran bulunamadı');
      }
    } else {
      console.log('Üst ekran seçimi temizlendi');
    }
  }
    /**
   * DataGrid'den gelen satır aksiyonlarını işler
   * @param event Aksiyon olayı
   */  onRowAction(event: { action: string, data: any }): void {
    switch (event.action) {
      case 'edit':
        this.openEditEkranModal(event.data);
        break;
      case 'delete':
        this.confirmDeleteEkran(event.data);
        break;
    }
  }
  
  /**
   * Görünüm modunu değiştirir (ağaç/tablo)
   * @param mode Görünüm modu
   */
  switchViewMode(mode: 'grid' | 'tree'): void {
    this.viewMode = mode;
  }
    /**
   * Ağaç görünümünde node seçimi olayını işler
   * @param node Seçilen node
   */
  onTreeNodeSelect(node: TreeNode): void {
    this.selectedEkran = node.data;
  }
  
  /**
   * Ağaç görünümünde node üzerinde çift tıklama olayını işler
   * @param event Node çift tıklama olayı
   */
  onTreeNodeDoubleClick(event: {originalEvent: MouseEvent, node: TreeNode}): void {
    this.openEditEkranModal(event.node.data);
  }
  
  /**
   * Ağaç görünümünde node işlem butonlarına tıklama olayını işler
   * @param event Node aksiyon olayı
   */
  onTreeNodeAction(event: {action: string, node: TreeNode}): void {
    const { action, node } = event;
    
    switch (action) {
      case 'edit':
        this.openEditEkranModal(node.data);
        break;
      case 'delete':
        this.confirmDeleteEkran(node.data);
        break;
    }
  }
}

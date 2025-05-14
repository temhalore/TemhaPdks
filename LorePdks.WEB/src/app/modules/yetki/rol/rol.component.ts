import { Component, OnInit, ViewChild } from '@angular/core';
import { TreeNode } from 'primeng/api';
import { finalize } from 'rxjs';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { TreeModule } from 'primeng/tree';
import { CheckboxModule } from 'primeng/checkbox';
import { TooltipModule } from 'primeng/tooltip';
import { ConfirmDialogComponent } from '../../../core/components/confirm-dialog/confirm-dialog.component';
import { DataGridComponent, ActionButtonConfig } from '../../../core/components/data-grid/data-grid.component';
import { ModalComponent } from '../../../core/components/modal/modal.component';
import { ControllerAndMethodsDTO } from '../../../core/models/ControllerAndMethodsDTO';
import { YetkiService } from '../../../core/services/modules/yetki.service';
import { RolDto } from '../../../core/models/RolDto';
import { EkranDto } from '../../../core/models/EkranDto';
import { RolControllerMethodsRequestDto } from '../../../core/models/RolControllerMethodsRequestDto';
import { KisiService } from '../../../core/services/modules/kisi.service';
import { KisiDto } from '../../../core/models/KisiDto';
import { AutoCompleteComponent } from '../../../core/components/auto-complete';

@Component({
  selector: 'app-rol',
  templateUrl: './rol.component.html',
  styleUrls: ['./rol.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ButtonModule,
    InputTextModule,
    InputTextareaModule,
    TreeModule,
    CheckboxModule,
    TooltipModule,
    DataGridComponent,
    ModalComponent,
    ConfirmDialogComponent,
    AutoCompleteComponent
  ]
})
export class RolComponent implements OnInit {
  // Rol listesi
  rolList: RolDto[] = [];
  loading: boolean = false;
  
  // İşlem butonları tanımı
  actionButtons: ActionButtonConfig[] = [
    { icon: 'pi pi-pencil', tooltip: 'Düzenle', action: 'edit' },
    { icon: 'pi pi-trash', tooltip: 'Sil', action: 'delete', class: 'p-button-danger' },
    { icon: 'pik pi-list', tooltip: 'Ekran Ekle/Çıkar', action: 'ekran', class: 'p-button-info' },
    { icon: 'pi pi-cog', tooltip: 'Controller-Method Ekle/Çıkar', action: 'controller', class: 'p-button-info' },
    { icon: 'pi pi-users', tooltip: 'Kişi Ekle/Çıkar', action: 'kisi', class: 'p-button-success' }
  ];
  
  // Data grid sütun tanımları
  columns = [
    { field: 'rolAdi', header: 'Rol Adı' },
    { field: 'aciklama', header: 'Açıklama' }
  ];
  
  // Modal durum bayrakları
  rolModalVisible: boolean = false;
  ekranModalVisible: boolean = false;
  controllerModalVisible: boolean = false;
  kisiModalVisible: boolean = false;
  
  // Seçili rol ve form modeli
  selectedRol: RolDto | null = null;
  rolModel: RolDto = new RolDto();
  
  // Ekran ağacı
  ekranTree: TreeNode[] = [];
  selectedEkranNodes: TreeNode[] = [];
  allEkranList: EkranDto[] = [];
  loadingEkranlar: boolean = false;
  
  // Controller ve Metotlar
  controllerMethods: ControllerAndMethodsDTO[] = [];
  selectedControllerMethods: ControllerAndMethodsDTO[] = [];
  loadingControllers: boolean = false;
    // Kişi-Rol özellikleri
  kisiList: KisiDto[] = [];
  filteredKisiList: KisiDto[] = [];
  selectedKisi: KisiDto | null = null;
  loadingKisiler: boolean = false;
  roleKisiList: KisiDto[] = [];
  rolIdKisiMap: Map<string, KisiDto[]> = new Map();
  
  // Onay dialogu
  @ViewChild(ConfirmDialogComponent) confirmDialog!: ConfirmDialogComponent;
  
  constructor(
    private yetkiService: YetkiService,
    private kisiService: KisiService
  ) { }
  
  ngOnInit(): void {
    this.loadRolList();
  }
  
  /**
   * Rol listesini yükler
   */
  loadRolList(): void {
    this.loading = true;
    this.yetkiService.getAllRolList()
      .pipe(finalize(() => this.loading = false))
      .subscribe({
        next: (data) => {
          this.rolList = data;
        },
        error: (err) => {
          console.error('Rol listesi yüklenirken hata oluştu:', err);
        }
      });
  }
  
  /**
   * Yeni rol ekleme modalını açar
   */
  openAddRolModal(): void {
    this.rolModel = new RolDto();
    this.rolModalVisible = true;
  }
  
  /**
   * Rol düzenleme modalını açar
   * @param rol Düzenlenecek rol
   */
  openEditRolModal(rol: RolDto): void {
    // Rol verisini doğrudan kullan, tekrar API çağrısı yapma
    this.selectedRol = rol;
    this.rolModel = { ...rol }; // Rol bilgilerini doğrudan kopyala
    this.rolModalVisible = true;
  }
  
  /**
   * Rol silme onay dialogunu gösterir
   * @param rol Silinecek rol
   */
  confirmDeleteRol(rol: RolDto): void {
    this.selectedRol = rol;
    this.confirmDialog.header = 'Rol Silme Onayı';
    this.confirmDialog.message = `"${rol.rolAdi}" rolünü silmek istediğinize emin misiniz?`;
    this.confirmDialog.show();
  }
  
  /**
   * Onay sonrası rol silme işlemini gerçekleştirir
   */
  deleteRol(): void {
    if (!this.selectedRol) return;
    
    this.yetkiService.deleteRol(this.selectedRol.eid)
      .subscribe({
        next: () => {
          this.loadRolList();
        },
        error: (err) => {
          console.error('Rol silinirken hata oluştu:', err);
        }
      });
  }
  
  /**
   * Rol kaydetme/güncelleme işlemini gerçekleştirir
   */
  saveRol(): void {
    // Güncelleme kontrolü için eid bilgisini doğru şekilde ayarla
    if (this.selectedRol && this.selectedRol.eid) {
      this.rolModel.eid = this.selectedRol.eid;
    }
    
    this.yetkiService.saveRol(this.rolModel)
      .subscribe({
        next: () => {
          this.rolModalVisible = false;
          this.selectedRol = null; // Modal kapandığında seçili rol temizleniyor
          this.loadRolList();
        },
        error: (err) => {
          console.error('Rol kaydedilirken hata oluştu:', err);
        }
      });
  }
  
  /**
   * Modal kapatıldığında seçili rolü temizler
   */
  onRolModalClosed(): void {
    this.selectedRol = null;
  }
  
  /**
   * Ekran ekleme/çıkarma modalını açar
   * @param rol Role ekran eklenecek/çıkarılacak
   */
  openEkranModal(rol: RolDto): void {
    this.selectedRol = rol;
    this.loadingEkranlar = true;
    this.ekranTree = [];
    this.selectedEkranNodes = [];
    
    // Tüm ekranları ve role ait ekranları paralel olarak yükle
    Promise.all([
      this.loadAllEkranlar(),
      this.loadRolEkranlar(rol.eid)
    ]).then(() => {
      this.ekranModalVisible = true;
      this.loadingEkranlar = false;
    }).catch(err => {
      console.error('Ekranlar yüklenirken hata oluştu:', err);
      this.loadingEkranlar = false;
    });
  }
  
  /**
   * Tüm ekranları yükler ve ağaç yapısı oluşturur
   */
  loadAllEkranlar(): Promise<void> {
    return new Promise((resolve, reject) => {
      this.yetkiService.getAllEkranList()
        .subscribe({
          next: (data) => {
            this.allEkranList = data;
            // Ağaç yapısı oluştur
            this.ekranTree = this.buildEkranTree(this.allEkranList);
            resolve();
          },
          error: (err) => {
            console.error('Ekranlar yüklenirken hata oluştu:', err);
            reject(err);
          }
        });
    });
  }
  
  /**
   * Role ait ekranları yükler ve ağaçta seçili hale getirir
   * @param rolId Rol ID
   */
  loadRolEkranlar(rolId: string): Promise<void> {
    return new Promise((resolve, reject) => {
      this.yetkiService.getEkransByRolId(rolId)
        .subscribe({
          next: (data) => {
            // Rol ekranlarını belirle
            const rolEkranEids = data.map(e => e.eid);
            // Rol ekranlarını ağaçta seçili hale getir
            this.markSelectedEkranlar(rolEkranEids);
            resolve();
          },
          error: (err) => {
            console.error('Rol ekranları yüklenirken hata oluştu:', err);
            reject(err);
          }
        });
    });
  }
  
  /**
   * Ekran listesinden ağaç yapısı oluşturur
   * @param ekranlar Ekran listesi
   * @param parentId Üst ekran ID
   * @returns Ağaç yapısı
   */
  buildEkranTree(ekranlar: EkranDto[], parentId: string = ''): TreeNode[] {
    const result: TreeNode[] = [];
    
    const children = ekranlar.filter(e => 
      (!parentId && !e.ustEkranEidDto?.eid) || 
      (e.ustEkranEidDto?.eid === parentId)
    );
    
    children.forEach(child => {
      const node: TreeNode = {
        key: child.eid,
        label: child.ekranAdi,
        data: child,
        children: this.buildEkranTree(ekranlar, child.eid),
        selectable: true
      };
      
      result.push(node);
    });
    
    return result;
  }
  
  /**
   * Rol ekranlarını ağaçta seçili hale getirir
   * @param ekranEids Role ait ekran ID'leri
   */
  markSelectedEkranlar(ekranEids: string[]): void {
    this.selectedEkranNodes = [];
    
    const findAndMarkNodes = (nodes: TreeNode[]) => {
      nodes.forEach(node => {
        if (ekranEids.includes(node.key as string)) {
          this.selectedEkranNodes.push(node);
        }
        
        if (node.children) {
          findAndMarkNodes(node.children);
        }
      });
    };
    
    findAndMarkNodes(this.ekranTree);
  }
  
  /**
   * Role ekran ekler/çıkarır
   */
  saveEkranlar(): void {
    if (!this.selectedRol) return;
    
    const rolId = this.selectedRol.eid;
    const allTreeNodes: TreeNode[] = [];
    
    // Tüm ağaç düğümlerini düz liste haline getir
    const flattenNodes = (nodes: TreeNode[]) => {
      nodes.forEach(node => {
        allTreeNodes.push(node);
        if (node.children) {
          flattenNodes(node.children);
        }
      });
    };
    
    flattenNodes(this.ekranTree);
    
    // Mevcut seçili ekran ID'leri
    const selectedEkranEids = this.selectedEkranNodes.map(n => n.key as string);
    
    // Role ait ekranları yükle ve karşılaştır
    this.yetkiService.getEkransByRolId(rolId)
      .subscribe({
        next: (rolEkranlar) => {
          const currentEkranEids = rolEkranlar.map(e => e.eid);
          
          // Eklenecek ekranlar
          const ekranlarToAdd = selectedEkranEids.filter(eid => !currentEkranEids.includes(eid));
          
          // Çıkarılacak ekranlar
          const ekranlarToRemove = currentEkranEids.filter(eid => !selectedEkranEids.includes(eid));
          
          // Ekran ekleme ve çıkarma işlemlerini gerçekleştir
          const promises = [
            ...ekranlarToAdd.map(ekranEid => 
              this.yetkiService.addEkranToRol(rolId, ekranEid).toPromise()
            ),
            ...ekranlarToRemove.map(ekranEid => 
              this.yetkiService.removeEkranFromRol(rolId, ekranEid).toPromise()
            )
          ];
          
          Promise.all(promises)
            .then(() => {
              this.ekranModalVisible = false;
              this.selectedRol = null; // Modal kapandığında seçili rol temizleniyor
            })
            .catch(err => {
              console.error('Ekranlar kaydedilirken hata oluştu:', err);
            });
        },
        error: (err) => {
          console.error('Rol ekranları yüklenirken hata oluştu:', err);
        }
      });
  }
  
  /**
   * Ekran modalı kapatıldığında seçili rolü temizler
   */
  onEkranModalClosed(): void {
    this.selectedRol = null;
  }
  
  /**
   * Controller-Method ekleme/çıkarma modalını açar
   * @param rol Role controller-method eklenecek/çıkarılacak
   */
  openControllerModal(rol: RolDto): void {
    this.selectedRol = rol;
    this.loadingControllers = true;
    this.controllerMethods = [];
    this.selectedControllerMethods = [];
    
    // Tüm controller-methodları ve role ait controller-methodları paralel olarak yükle
    Promise.all([
      this.loadAllControllerMethods(),
      this.loadRolControllerMethods(rol.eid)
    ]).then(() => {
      this.controllerModalVisible = true;
      this.loadingControllers = false;
    }).catch(err => {
      console.error('Controller-Methodlar yüklenirken hata oluştu:', err);
      this.loadingControllers = false;
    });
  }
  
  /**
   * Tüm controller ve metotları yükler
   */
  loadAllControllerMethods(): Promise<void> {
    return new Promise((resolve, reject) => {
      this.yetkiService.getControllerAndMethodsList()
        .subscribe({
          next: (data) => {
            this.controllerMethods = data;
            resolve();
          },
          error: (err) => {
            console.error('Controller ve metotlar yüklenirken hata oluştu:', err);
            reject(err);
          }
        });
    });
  }
  
  /**
   * Role ait controller ve metotları yükler
   * @param rolId Rol ID
   */
  loadRolControllerMethods(rolId: string): Promise<void> {
    return new Promise((resolve, reject) => {
      this.yetkiService.getRolControllerMethods(rolId)
        .subscribe({
          next: (data) => {
            // Role ait controller-metot bilgilerini düzenle
            const rolControllerMethods = data.reduce((acc, item) => {
              let controller = acc.find(c => c.controllerName === item.controllerName);
              
              if (!controller) {
                controller = {
                  controllerName: item.controllerName,
                  methods: []
                };
                acc.push(controller);
              }
              
              controller.methods.push(item.methodName);
              return acc;
            }, [] as ControllerAndMethodsDTO[]);
            
            this.selectedControllerMethods = rolControllerMethods;
            resolve();
          },
          error: (err) => {
            console.error('Rol controller-metotları yüklenirken hata oluştu:', err);
            reject(err);
          }
        });
    });
  }
  
  /**
   * Controllerın seçili olup olmadığını kontrol eder
   * @param controller Controller adı
   * @returns Seçili ise true, değilse false
   */
  isControllerSelected(controller: string): boolean {
    return this.selectedControllerMethods.some(c => c.controllerName === controller);
  }
  
  /**
   * Metodun seçili olup olmadığını kontrol eder
   * @param controller Controller adı
   * @param method Metot adı
   * @returns Seçili ise true, değilse false
   */
  isMethodSelected(controller: string, method: string): boolean {
    const selectedController = this.selectedControllerMethods.find(c => c.controllerName === controller);
    return selectedController ? selectedController.methods.includes(method) : false;
  }
  
  /**
   * Controller seçim durumunu değiştirir
   * @param controller Controller
   * @param event Checkbox olayı
   */
  onControllerSelectionChange(controller: ControllerAndMethodsDTO, event: any): void {
    const checked = event.checked !== undefined ? event.checked : event;
    
    if (checked) {
      // Controller ve tüm metotlarını ekle
      this.selectedControllerMethods = this.selectedControllerMethods.filter(c => c.controllerName !== controller.controllerName);
      this.selectedControllerMethods.push({ ...controller });
    } else {
      // Controller ve tüm metotlarını çıkar
      this.selectedControllerMethods = this.selectedControllerMethods.filter(c => c.controllerName !== controller.controllerName);
    }
  }
  
  /**
   * Metot seçim durumunu değiştirir
   * @param controller Controller adı
   * @param method Metot adı
   * @param event Checkbox olayı
   */
  onMethodSelectionChange(controller: string, method: string, event: any): void {
    const checked = event.checked !== undefined ? event.checked : event;
    
    const controllerObj = this.controllerMethods.find(c => c.controllerName === controller);
    
    if (!controllerObj) return;
    
    let selectedController = this.selectedControllerMethods.find(c => c.controllerName === controller);
    
    if (!selectedController) {
      selectedController = {
        controllerName: controller,
        methods: []
      };
      this.selectedControllerMethods.push(selectedController);
    }
    
    if (checked) {
      // Metodu ekle
      if (!selectedController.methods.includes(method)) {
        selectedController.methods.push(method);
      }
    } else {
      // Metodu çıkar
      selectedController.methods = selectedController.methods.filter(m => m !== method);
      
      // Eğer hiç metot kalmadıysa controller'ı da çıkar
      if (selectedController.methods.length === 0) {
        this.selectedControllerMethods = this.selectedControllerMethods.filter(c => c.controllerName !== controller);
      }
    }
  }
  
  /**
   * Role controller ve metot yetkilerini kaydeder
   */
  saveControllerMethods(): void {
    if (!this.selectedRol) return;
    
    const request: RolControllerMethodsRequestDto = {
      rolEidDto: {eid: this.selectedRol.eid},
      controllerMethods: this.selectedControllerMethods
    };
    
    this.yetkiService.saveRolControllerMethods(request)
      .subscribe({
        next: () => {
          this.controllerModalVisible = false;
          this.selectedRol = null; // Modal kapandığında seçili rol temizleniyor
        },
        error: (err) => {
          console.error('Controller-Method yetkileri kaydedilirken hata oluştu:', err);
        }
      });
  }
  
  /**
   * Controller modal kapatıldığında seçili rolü temizler
   */
  onControllerModalClosed(): void {
    this.selectedRol = null;
  }
  
  /**
   * Kişi ekleme/çıkarma modalını açar
   * @param rol Rol verisi
   */
  openKisiModal(rol: RolDto): void {
    this.selectedRol = rol;
    this.selectedKisi = null;
    this.filteredKisiList = [];
    this.loadingKisiler = true;
    
    // Role ait kişileri yükle
    this.loadRoleKisiler(rol.eid);
    
    this.kisiModalVisible = true;
  }  /**
   * Role ait kişileri yükler
   * @param rolId Rol ID
   */
  loadRoleKisiler(rolId: string): void {
    this.loadingKisiler = true;
    this.roleKisiList = [];
    
    // Önce cache kontrolü yap
    if (this.rolIdKisiMap.has(rolId)) {
      this.roleKisiList = this.rolIdKisiMap.get(rolId) || [];
      this.loadingKisiler = false;
      return;
    }
    
    // Backend'den rol id'ye göre doğrudan kişileri getir
    this.yetkiService.getKisisByRolId(rolId)
      .pipe(finalize(() => this.loadingKisiler = false))
      .subscribe({
        next: (kisiList) => {
          // Kişileri sakla
          this.roleKisiList = kisiList;
          
          // Önbelleğe al
          this.rolIdKisiMap.set(rolId, kisiList);
          
          // Not: Tüm kişileri önceden yüklemek yerine, 
          // sadece arama yapıldığında ilgili kişileri getiriyoruz (searchKisi metodunda)
        },
        error: (err) => {
          console.error('Role ait kişiler yüklenirken hata oluştu:', err);
          this.roleKisiList = [];
        }
      });
  }
  /**
   * Kişi arama işlemini gerçekleştirir
   * @param query Arama metni
   */
  searchKisi(query: string): void {
    if (!query || query.length < 2) {
      this.filteredKisiList = [];
      return;
    }
    
    // API'den doğrudan arama yap
    this.kisiService.getKisiListByAramaText(query)
      .subscribe({
        next: (data) => {
          // Role ait olmayan kişileri filtrele
          this.filteredKisiList = data.filter(k => 
            !this.roleKisiList.some(rk => rk.eid === k.eid)
          );
        },
        error: (err) => {
          console.error('Kişi araması yapılırken hata oluştu:', err);
          this.filteredKisiList = [];
        }
      });
  }
  /**
   * Role kişi ekler
   */
  addKisiToRol(): void {
    if (!this.selectedRol || !this.selectedKisi) return;
    
    // Yükleniyor durumunu göster
    this.loadingKisiler = true;
    
    this.yetkiService.addRolToKisi(this.selectedKisi.eid, this.selectedRol.eid)
      .pipe(finalize(() => this.loadingKisiler = false))
      .subscribe({
        next: () => {
          // Önbelleği temizle
          if (this.selectedRol) {
            this.rolIdKisiMap.delete(this.selectedRol.eid);
          }
          
          // Listeyi güncelle
          this.loadRoleKisiler(this.selectedRol!.eid);
          this.selectedKisi = null;
          
          // Filtrelenmiş listeyi temizle
          this.filteredKisiList = [];
        },
        error: (err) => {
          console.error('Kişi role eklenirken hata oluştu:', err);
          // Hatayı kullanıcıya göster (burada bir toast mesajı gösterilebilir)
          alert('Kişi role eklenirken bir hata oluştu. Lütfen tekrar deneyin.');
        }
      });
  }
  /**
   * Rolden kişi çıkarır
   * @param kisi Çıkarılacak kişi
   */
  removeKisiFromRol(kisi: KisiDto): void {
    if (!this.selectedRol) return;
    
    // Yükleniyor durumunu göster
    this.loadingKisiler = true;
    
    this.yetkiService.removeRolFromKisi(kisi.eid, this.selectedRol.eid)
      .pipe(finalize(() => this.loadingKisiler = false))
      .subscribe({
        next: () => {
          // Önbelleği temizle
          if (this.selectedRol) {
            this.rolIdKisiMap.delete(this.selectedRol.eid);
          }
          
          // Listeyi güncelle
          this.loadRoleKisiler(this.selectedRol!.eid);
        },
        error: (err) => {
          console.error('Kişi rolden çıkarılırken hata oluştu:', err);
          // Hatayı kullanıcıya göster
          alert('Kişi rolden çıkarılırken bir hata oluştu. Lütfen tekrar deneyin.');
        }
      });
  }
  
  /**
   * Data grid üzerindeki aksiyon butonları için işlemleri yönetir
   * @param event Aksiyon verisi
   */
  onRowAction(event: {action: string, data: RolDto}): void {
    switch (event.action) {
      case 'edit':
        this.openEditRolModal(event.data);
        break;
      case 'delete':
        this.confirmDeleteRol(event.data);
        break;
      case 'ekran':
        this.openEkranModal(event.data);
        break;
      case 'controller':
        this.openControllerModal(event.data);
        break;
      case 'kisi':
        this.openKisiModal(event.data);
        break;
    }
  }

  /**
   * Özel action butonları için işlemleri yönetir
   * @param action Aksiyon adı
   * @param rol Rol verisi
   */
  onAction(action: string, rol: RolDto): void {
    this.onRowAction({action, data: rol});
  }

  /**
   * Kişi modal kapatıldığında seçili rol ve kişi verilerini temizler
   */
  onKisiModalClosed(): void {
    this.selectedRol = null;
    this.selectedKisi = null;
    this.filteredKisiList = [];
  }
}
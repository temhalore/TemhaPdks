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
import { ConfirmDialogComponent } from '../../../core/components/confirm-dialog/confirm-dialog.component';
import { DataGridComponent } from '../../../core/components/data-grid/data-grid.component';
import { ModalComponent } from '../../../core/components/modal/modal.component';
import { ControllerAndMethodsDTO } from '../../../core/models/ControllerAndMethodsDTO';
import { YetkiService } from '../../../core/services/modules/yetki.service';
import { RolDto } from '../../../core/models/RolDto';
import { EkranDto } from '../../../core/models/EkranDto';
import { RolControllerMethodsRequestDto } from '../../../core/models/RolControllerMethodsRequestDto';

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
    DataGridComponent,
    ModalComponent,
    ConfirmDialogComponent
  ]
})
export class RolComponent implements OnInit {
  // Rol listesi
  rolList: RolDto[] = [];
  loading: boolean = false;
  
  // Data grid sütun tanımları
  columns = [
    { field: 'rolAdi', header: 'Rol Adı' },
    { field: 'aciklama', header: 'Açıklama' }
  ];
  
  // Modal durum bayrakları
  rolModalVisible: boolean = false;
  ekranModalVisible: boolean = false;
  controllerModalVisible: boolean = false;
  
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
  
  // Onay dialogu
  @ViewChild(ConfirmDialogComponent) confirmDialog!: ConfirmDialogComponent;
  
  constructor(private yetkiService: YetkiService) { }
  
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
    this.selectedRol = rol;
    this.yetkiService.getRolById(rol.eid)
      .subscribe({
        next: (data) => {
          this.rolModel = { ...data };
          this.rolModalVisible = true;
        },
        error: (err) => {
          console.error('Rol bilgisi yüklenirken hata oluştu:', err);
        }
      });
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
    this.yetkiService.saveRol(this.rolModel)
      .subscribe({
        next: () => {
          this.rolModalVisible = false;
          this.loadRolList();
        },
        error: (err) => {
          console.error('Rol kaydedilirken hata oluştu:', err);
        }
      });
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
        },
        error: (err) => {
          console.error('Controller-Method yetkileri kaydedilirken hata oluştu:', err);
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
    }
  }
}
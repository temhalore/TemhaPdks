import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { finalize, Subscription } from 'rxjs';

// PrimeNG Modülleri
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { TableModule } from 'primeng/table';
import { TooltipModule } from 'primeng/tooltip';

// Uygulama Bileşenleri
import { DataGridComponent, ActionButtonConfig } from '../../../core/components/data-grid/data-grid.component';
import { ModalComponent } from '../../../core/components/modal/modal.component';
import { ConfirmDialogComponent } from '../../../core/components/confirm-dialog/confirm-dialog.component';
import { TextInputComponent } from '../../../core/components/text-input/text-input.component';

// Servisler ve Modeller
import { KisiService } from '../../../core/services/modules/kisi.service';
import { KisiDto } from '../../../core/models/KisiDto';

@Component({
  selector: 'app-kisi-list',
  templateUrl: './kisi-list.component.html',
  styleUrls: ['./kisi-list.component.scss'],  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ButtonModule,
    InputTextModule,
    InputTextareaModule,
    TableModule,
    TooltipModule,
    DataGridComponent,
    ModalComponent,
    ConfirmDialogComponent,
    TextInputComponent
  ]
})
export class KisiListComponent implements OnInit, OnDestroy {
  // Kişi listesi
  kisiList: KisiDto[] = [];
  loading: boolean = false;
  
  // Subscription'ları takip etmek için
  private subscriptions: Subscription[] = [];
  
  // İşlem butonları tanımı
  actionButtons: ActionButtonConfig[] = [
    { icon: 'pi pi-pencil', tooltip: 'Düzenle', action: 'edit' },
    { icon: 'pi pi-trash', tooltip: 'Sil', action: 'delete', class: 'p-button-danger' }
  ];
  
  // Data grid sütun tanımları
  columns = [
    { field: 'ad', header: 'Ad' },
    { field: 'soyad', header: 'Soyad' },
    { field: 'tc', header: 'TC Kimlik No' },
    { field: 'cepTel', header: 'Telefon' },
    { field: 'email', header: 'E-posta' },
    { field: 'loginName', header: 'Kullanıcı Adı' }
  ];
  
  // Modal durum bayrakları
  kisiModalVisible: boolean = false;
  
  // Seçili kişi ve form modeli
  selectedKisi: KisiDto | null = null;
  kisiModel: KisiDto = new KisiDto();
  
  // Onay dialogu
  @ViewChild(ConfirmDialogComponent) confirmDialog!: ConfirmDialogComponent;
  
  constructor(private kisiService: KisiService) { }
  
  ngOnInit(): void {
    this.loadKisiList();
  }
  
  /**
   * Kişi listesini yükler
   */
  loadKisiList(): void {
    this.loading = true;
    this.kisiService.getAllKisiList()
      .pipe(finalize(() => this.loading = false))
      .subscribe({
        next: (data) => {
          this.kisiList = data;
        },
        error: (err) => {
          console.error('Kişi listesi yüklenirken hata oluştu:', err);
        }
      });
  }
    /**
   * Kişi ekleme modalını açar
   */
  openAddKisiModal(): void {
    this.kisiModel = new KisiDto();
    // Başlangıç değerlerini ayarla
    this.kisiModel.ad = '';
    this.kisiModel.soyad = '';
    this.kisiModel.tc = '';
    this.kisiModel.cepTel = '';
    this.kisiModel.email = '';
    this.kisiModel.loginName = '';
    this.kisiModel.sifre = '';
    
    this.selectedKisi = null;
    this.kisiModalVisible = true;
  }
  
  /**
   * Data grid'teki aksiyon butonlarına tıklandığında tetiklenir
   * @param event Aksiyon bilgisi
   */
  onActionClick(event: { action: string, data: any }): void {
    this.selectedKisi = event.data;
    
    switch (event.action) {
      case 'edit':
        this.openEditKisiModal();
        break;
      case 'delete':
        this.confirmDelete();
        break;
    }
  }
    /**
   * Kişi düzenleme modalını açar
   */
  openEditKisiModal(): void {
    if (!this.selectedKisi) return;
    
    // Kişi bilgilerini formda göstermek için kopyalama
    this.kisiModel = { ...this.selectedKisi };
    
    // Null veya undefined değerleri boş string ile değiştir
    this.kisiModel.ad = this.kisiModel.ad || '';
    this.kisiModel.soyad = this.kisiModel.soyad || '';
    this.kisiModel.tc = this.kisiModel.tc || '';
    this.kisiModel.cepTel = this.kisiModel.cepTel || '';
    this.kisiModel.email = this.kisiModel.email || '';
    this.kisiModel.loginName = this.kisiModel.loginName || '';
    
    this.kisiModalVisible = true;
  }/**
   * Kişi silme işlemi için onay dialogu açar
   */
  confirmDelete(): void {
    if (!this.selectedKisi) return;
    
    this.confirmDialog.message = `${this.selectedKisi.ad} ${this.selectedKisi.soyad} adlı kişiyi silmek istediğinize emin misiniz?`;
    this.confirmDialog.acceptLabel = 'Evet, Sil';
    this.confirmDialog.rejectLabel = 'İptal';
    
    // Önceki subscription'ları temizleyip yenisini oluştur
    const confirmSub = this.confirmDialog.confirmed.subscribe(() => {
      this.deleteKisi();
      confirmSub.unsubscribe(); // Kullanıldıktan sonra temizle
    });
    
    this.confirmDialog.show();
  }
  
  /**
   * Component yok edildiğinde çalışır
   */
  ngOnDestroy(): void {
    // Tüm subscription'ları temizle
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }  /**
   * Kişi kaydeder veya günceller
   */
  saveKisi(): void {
    // Form doğrulama kontrolü - artık butonu disable ettiğimiz için burada tekrar kontrol etmeye gerek yok
    console.log('saveKisi metodu çağrıldı! Gönderilecek veri:', this.kisiModel);
    this.loading = true;
    this.kisiService.saveKisi(this.kisiModel)
      .pipe(finalize(() => this.loading = false))
      .subscribe({
        next: (response) => {
          console.log('Kişi kaydedildi:', response);
          this.kisiModalVisible = false;
          this.loadKisiList(); // Listeyi yenile
        },
        error: (err) => {
          console.error('Kişi kaydedilirken hata oluştu:', err);
          // Hata yönetimi WebUI'daki genel mekanizma ile yapılacak
          // Hata bilgileri ApiService içerisinde işleniyor
        }
      });
  }
  
  /**
   * Form alanlarının geçerli olup olmadığını kontrol eder
   * @returns boolean
   */
  isFormValid(): boolean {
    // Zorunlu alanların dolu olup olmadığını kontrol et
    return !!(this.kisiModel.ad && this.kisiModel.soyad && this.kisiModel.loginName && 
      // Yeni kişi ekliyorsak şifre zorunlu, düzenliyorsak değil
      (this.selectedKisi || (this.kisiModel.sifre && this.kisiModel.sifre.trim() !== '')));
  }
  
  /**
   * Kişi siler
   */
  deleteKisi(): void {
    if (!this.selectedKisi?.eid) return;
    
    this.loading = true;
    this.kisiService.deleteKisi(this.selectedKisi.eid)
      .pipe(finalize(() => this.loading = false))
      .subscribe({
        next: (success) => {
          if (success) {
            this.loadKisiList(); // Listeyi yenile
          }
        },
        error: (err) => {
          console.error('Kişi silinirken hata oluştu:', err);
        }
      });
  }
  
  /**
   * Kişi modalını kapatır ve formu temizler
   */
  closeKisiModal(): void {
    this.kisiModalVisible = false;
    this.kisiModel = new KisiDto();
    this.selectedKisi = null;
  }
  
  /**
   * Kişi ara
   * @param event Input olayı
   */
  searchKisi(event: any): void {
    const searchText = event.target.value;
    
    if (!searchText || searchText.length < 3) {
      this.loadKisiList();
      return;
    }
    
    this.loading = true;
    this.kisiService.getKisiListByAramaText(searchText)
      .pipe(finalize(() => this.loading = false))
      .subscribe({
        next: (data) => {
          this.kisiList = data;
        },
        error: (err) => {
          console.error('Kişi aranırken hata oluştu:', err);
        }
      });
  }
}

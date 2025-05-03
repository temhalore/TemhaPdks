import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { EkranDTO } from 'src/app/core/models/EkranDTO';
import { YetkiService } from '../../../services/yetki.service';

@Component({
  selector: 'app-ekran-modal',
  templateUrl: './ekran-modal.component.html',
  styleUrls: ['./ekran-modal.component.scss']
})
export class EkranModalComponent implements OnInit {
  @Input() isNew: boolean = true;
  @Input() ekran: EkranDTO | null = null;
  @Input() ekranlar: EkranDTO[] = [];
  
  @Output() modalClose = new EventEmitter<void>();
  @Output() ekranSaved = new EventEmitter<EkranDTO>();

  ekranForm: FormGroup;
  displayModal: boolean = true;
  ustEkranlar: EkranDTO[] = [];
  ikonlar: {label: string, value: string}[] = [
    { label: 'Klasör', value: 'pi pi-fw pi-folder' },
    { label: 'Belge', value: 'pi pi-fw pi-file' },
    { label: 'Kullanıcı', value: 'pi pi-fw pi-user' },
    { label: 'Ayarlar', value: 'pi pi-fw pi-cog' },
    { label: 'Ev', value: 'pi pi-fw pi-home' },
    { label: 'Anahtar', value: 'pi pi-fw pi-key' },
    { label: 'Kilitle', value: 'pi pi-fw pi-lock' }
  ];

  constructor(
    private fb: FormBuilder,
    private yetkiService: YetkiService
  ) {
    this.ekranForm = this.fb.group({
      id: [0],
      eid: [''],
      ekranAdi: ['', Validators.required],
      ekranYolu: ['', Validators.required],
      ekranKodu: ['', Validators.required],
      aciklama: [''],
      ustEkranId: [null],
      siraNo: [0, [Validators.required, Validators.min(0)]],
      ikon: ['pi pi-fw pi-folder'],
      aktif: [true]
    });
  }

  ngOnInit(): void {
    this.loadUstEkranlar();
    if (this.ekran) {
      this.ekranForm.patchValue(this.ekran);
    }
  }

  /**
   * Üst ekran olarak seçilebilecek ekranları filtreler ve yükler
   */
  loadUstEkranlar(): void {
    // Eğer düzenleme modundaysak, kendi ID'si dışındaki ekranları listeleyeceğiz
    this.ustEkranlar = this.ekranlar.filter(e => {
      // Kendisini üst ekran olarak seçemez
      if (this.ekran && e.id === this.ekran.id) {
        return false;
      }
      
      // Alt ekranlarını üst ekran olarak seçemez (sonsuz döngü olmaması için)
      // Burada basit bir kontrolle sadece 1 seviye kontrol ediliyor
      // Daha karmaşık bir yapı için recursive kontrol gerekebilir
      if (this.ekran && e.ustEkranId === this.ekran.id) {
        return false;
      }
      
      return true;
    });
  }

  /**
   * Modalı kapatır
   */
  closeModal(): void {
    this.displayModal = false;
    this.modalClose.emit();
  }

  /**
   * Formu gönderme işlemi
   */
  onSubmit(): void {
    if (this.ekranForm.invalid) {
      // Form geçersizse, tüm hataları göster
      Object.keys(this.ekranForm.controls).forEach(key => {
        const control = this.ekranForm.get(key);
        if (control) {
          control.markAsTouched();
        }
      });
      return;
    }

    const ekranData: EkranDTO = this.ekranForm.value;
    
    this.yetkiService.saveEkran(ekranData).subscribe({
      next: (savedEkran) => {
        this.ekranSaved.emit(savedEkran);
      },
      error: (err) => {
        console.error('Ekran kaydedilirken hata oluştu', err);
      }
    });
  }
}
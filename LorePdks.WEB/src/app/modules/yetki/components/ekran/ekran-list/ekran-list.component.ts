import { Component, OnInit } from '@angular/core';
import { EkranDTO } from 'src/app/core/models/EkranDTO';
import { YetkiService } from '../../../services/yetki.service';
import { TreeNode } from 'primeng/api';

@Component({
  selector: 'app-ekran-list',
  templateUrl: './ekran-list.component.html',
  styleUrls: ['./ekran-list.component.scss']
})
export class EkranListComponent implements OnInit {
  // Ekranlar listesi
  ekranlar: EkranDTO[] = [];
  
  // TreeTable için veri
  ekranlarTree: TreeNode[] = [];
  
  // Düzenleme modalı için
  displayModal: boolean = false;
  selectedEkran: EkranDTO | null = null;
  isNewEkran: boolean = false;

  constructor(private yetkiService: YetkiService) { }

  ngOnInit(): void {
    this.loadEkranlar();
  }

  /**
   * Ekranları yükler ve ağaç yapısını oluşturur
   */
  loadEkranlar(): void {
    this.yetkiService.getAllEkranList().subscribe({
      next: (data) => {
        this.ekranlar = data;
        this.createTreeData();
      },
      error: (err) => {
        console.error('Ekranlar yüklenirken hata oluştu', err);
      }
    });
  }

  /**
   * Ağaç yapısını oluşturur
   */
  createTreeData(): void {
    // Ana ekranları bul (üst ekranı olmayanlar)
    const anaEkranlar = this.ekranlar.filter(e => !e.ustEkranId || e.ustEkranId <= 0);
    
    // TreeNode[] yapısına dönüştür
    this.ekranlarTree = anaEkranlar.map(ekran => this.createTreeNode(ekran));
  }

  /**
   * Tekil ekranı TreeNode yapısına dönüştürür
   */
  createTreeNode(ekran: EkranDTO): TreeNode {
    // Alt ekranları bul
    const altEkranlar = this.ekranlar.filter(e => e.ustEkranId === ekran.id);
    
    return {
      data: ekran,
      children: altEkranlar.map(alt => this.createTreeNode(alt)),
      expanded: false
    };
  }

  /**
   * Yeni ekran ekleme modalını açar
   */
  openNewEkranModal(): void {
    this.isNewEkran = true;
    this.selectedEkran = {
      id: 0,
      eid: '',
      ekranAdi: '',
      ekranYolu: '',
      ekranKodu: '',
      aciklama: '',
      siraNo: 0,
      ikon: 'pi pi-fw pi-folder',
      aktif: true,
      altEkranlar: []
    };
    this.displayModal = true;
  }

  /**
   * Düzenleme modalını açar
   */
  openEditEkranModal(ekran: EkranDTO): void {
    this.isNewEkran = false;
    this.selectedEkran = {...ekran};
    this.displayModal = true;
  }

  /**
   * Modal kapatıldığında çağrılır
   */
  onModalClose(): void {
    this.displayModal = false;
    this.selectedEkran = null;
  }

  /**
   * Ekran kaydedildiğinde yeniden yükleme yapar
   */
  onEkranSaved(): void {
    this.displayModal = false;
    this.loadEkranlar();
  }

  /**
   * Ekran siler
   */
  deleteEkran(ekran: EkranDTO): void {
    if (confirm(`${ekran.ekranAdi} ekranını silmek istediğinize emin misiniz?`)) {
      this.yetkiService.deleteEkran(ekran.id).subscribe({
        next: () => {
          this.loadEkranlar();
        },
        error: (err) => {
          console.error('Ekran silinirken hata oluştu', err);
        }
      });
    }
  }
}
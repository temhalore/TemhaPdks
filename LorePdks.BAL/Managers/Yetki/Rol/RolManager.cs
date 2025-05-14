using LorePdks.DAL.Model;
using LorePdks.DAL.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using LorePdks.COMMON.Enums;
using LorePdks.COMMON.Models;
using System.Threading.Tasks;
using LorePdks.BAL.Managers.Kisi.Interfaces;
using LorePdks.COMMON.DTO.Kisi;
using LorePdks.COMMON.DTO.Yetki.Ekran;
using LorePdks.COMMON.DTO.Yetki.Rol;
using LorePdks.BAL.Managers.Yetki.Rol.Interfaces;
using LorePdks.COMMON.DTO.Yetki;

namespace LorePdks.BAL.Managers.Yetki.Rol
{
    public class RolManager(
        IMapper _mapper,
        IKisiManager _kisiManager
        ) : IRolManager
    {

        private readonly GenericRepository<t_rol> _repoRol = new GenericRepository<t_rol>();
        private readonly GenericRepository<t_rol_ekran> _repoRolEkran = new GenericRepository<t_rol_ekran>();
        private readonly GenericRepository<t_kisi_rol> _repoKisiRol = new GenericRepository<t_kisi_rol>();
        private readonly GenericRepository<t_ekran> _repoEkran = new GenericRepository<t_ekran>();
        private readonly GenericRepository<t_rol_controller_method> _repoRolControllerMethod = new GenericRepository<t_rol_controller_method>();



        public RolDTO saveRol(RolDTO rolDTO)
        {
            bool isGuncelleniyor = false;

            if (rolDTO.id > 0) isGuncelleniyor = true;

            var dbRol = getRolByRolId(rolDTO.id, isYoksaHataDondur: isGuncelleniyor);

            // Rol için gerekli kontrolleri yap
            checkRolDtoKayitEdilebilirMi(rolDTO);

            t_rol rol = _mapper.Map<t_rol>(rolDTO);

            if (!isGuncelleniyor)
            {

            }
            else
            {

            }

            _repoRol.Save(rol);
            return _mapper.Map<RolDTO>(rol);
        }

        public void deleteRolByRolId(int rolId)
        {
            var dbRol = getRolByRolId(rolId, isYoksaHataDondur: true);

            // Role bağlı kişi ve ekran ilişkilerini kontrol et
            bool isKullanilmis = false;
            var kisiRoller = _repoKisiRol.GetList("ROL_ID=@rolId", new { rolId });


            _repoRol.Delete(dbRol);
        }

        public t_rol getRolByRolId(int rolId, bool isYoksaHataDondur = false)
        {
            var rol = _repoRol.Get(rolId);

            if (isYoksaHataDondur && rol == null)
            {
                throw new AppException(MessageCode.ERROR_503_GECERSIZ_VERI_GONDERIMI, $"{rolId} id'li rol sistemde bulunamadı");
            }
            return rol;
        }

        public RolDTO getRolDtoById(int rolId, bool isYoksaHataDondur = true)
        {
            var rol = getRolByRolId(rolId, isYoksaHataDondur);

            var rolDTO = _mapper.Map<RolDTO>(rol);

            // Rol'e ait ekranları da getir
            var rolEkranlar = _repoRolEkran.GetList("ROL_ID=@rolId", new { rolId });
            if (rolEkranlar != null && rolEkranlar.Any())
            {
                var ekranIdler = rolEkranlar.Select(x => x.EKRAN_ID).Distinct().ToList();

                // IN sorgusu için string oluşturma
                string ekranIdParams = string.Join(",", ekranIdler);
                var ekranlar = _repoEkran.GetList($"ID IN ({ekranIdParams})");

                rolDTO.ekranlar = _mapper.Map<List<EkranDTO>>(ekranlar);
            }

            return rolDTO;
        }

        public List<RolDTO> getRolDtoList(bool isYoksaHataDondur = false)
        {
            var roller = _repoRol.GetList();

            if (isYoksaHataDondur && (roller == null || !roller.Any()))
            {
                throw new AppException(MessageCode.ERROR_503_GECERSIZ_VERI_GONDERIMI, $"Rol bulunamadı");
            }

            return _mapper.Map<List<RolDTO>>(roller);
        }

        /// <summary>
        /// kişi rolleri ve ekranları döner
        /// </summary>
        /// <param name="kisiId"></param>
        /// <param name="isYoksaHataDondur"></param>
        /// <returns></returns>
        /// <exception cref="AppException"></exception>
        public List<RolDTO> getRolDtoListByKisiId(int kisiId, bool isYoksaHataDondur = false)
        {
            var kisiRoller = _repoKisiRol.GetList("KISI_ID=@kisiId", new { kisiId });

            if (isYoksaHataDondur && (kisiRoller == null || !kisiRoller.Any()))
            {
                throw new AppException(MessageCode.ERROR_503_GECERSIZ_VERI_GONDERIMI, $"{kisiId} id'li kişiye ait rol bulunamadı");
            }
            var aa = _mapper.Map<List<RolDTO>>(kisiRoller);

            return aa;
        }



        public bool addEkranToRol(int rolId, int ekranId)
        {
            // Rol ve ekranın var olduğunu kontrol et
            var rol = getRolByRolId(rolId, isYoksaHataDondur: true);
            var ekran = _repoEkran.Get(ekranId);

            if (ekran == null)
            {
                throw new AppException(MessageCode.ERROR_503_GECERSIZ_VERI_GONDERIMI, $"{ekranId} id'li ekran sistemde bulunamadı");
            }

            var rolEkran = _repoRolEkran.GetList("ROL_ID=@rolId AND EKRAN_ID=@ekranId", new { rolId, ekranId }).FirstOrDefault();

            if (rolEkran != null)
            {
                if (rolEkran.ISDELETED == 1)
                {
                    rolEkran.ISDELETED = 0;
                    rolEkran.MODIFIEDDATE = DateTime.Now;
                    _repoRolEkran.Save(rolEkran);
                }
                return true;
            }

            rolEkran = new t_rol_ekran
            {
                ROL_ID = rolId,
                EKRAN_ID = ekranId,
                ISDELETED = 0,
                CREATEDDATE = DateTime.Now
            };

            _repoRolEkran.Save(rolEkran);
            return true;
        }

        public bool addRolToKisi(int kisiId, int rolId)
        {
            // Rol ve kişinin var olduğunu kontrol et
            var rol = getRolByRolId(rolId, isYoksaHataDondur: true);
            var kisi = _kisiManager.getKisiByKisiId(kisiId, true);

            if (kisi == null)
            {
                throw new AppException(MessageCode.ERROR_503_GECERSIZ_VERI_GONDERIMI, $"{kisiId} id'li kişi sistemde bulunamadı");
            }

            var kisiRol = _repoKisiRol.GetList("KISI_ID=@kisiId AND ROL_ID=@rolId", new { kisiId, rolId }).FirstOrDefault();

            if (kisiRol != null)
            {
                if (kisiRol.ISDELETED == 1)
                {
                    kisiRol.ISDELETED = 0;
                    kisiRol.MODIFIEDDATE = DateTime.Now;
                    _repoKisiRol.Save(kisiRol);
                }
                return true;
            }

            kisiRol = new t_kisi_rol
            {
                KISI_ID = kisiId,
                ROL_ID = rolId,
                ISDELETED = 0,
                CREATEDDATE = DateTime.Now
            };

            _repoKisiRol.Save(kisiRol);
            return true;
        }

        public bool removeEkranFromRol(int rolId, int ekranId)
        {
            var rolEkran = _repoRolEkran.GetList("ROL_ID=@rolId AND EKRAN_ID=@ekranId", new { rolId, ekranId }).FirstOrDefault();

            if (rolEkran == null)
                return false;

            _repoRolEkran.Delete(rolEkran);
            return true;
        }

        public bool removeRolFromKisi(int kisiId, int rolId)
        {
            var kisiRol = _repoKisiRol.GetList("KISI_ID=@kisiId AND ROL_ID=@rolId", new { kisiId, rolId }).FirstOrDefault();

            if (kisiRol == null)
                return false;

            _repoKisiRol.Delete(kisiRol);
            return true;
        }

        /// <summary>
        /// Rol kaydedilebilir mi kontrol eder
        /// </summary>
        /// <param name="rolDTO"></param>
        /// <exception cref="AppException"></exception>
        private void checkRolDtoKayitEdilebilirMi(RolDTO rolDTO)
        {
            if (string.IsNullOrEmpty(rolDTO.rolAdi))
                throw new AppException(MessageCode.ERROR_502_EKSIK_VERI_GONDERIMI, $"Rol adı boş olamaz");

            var allRolList = _repoRol.GetList("ID <> @id", new { rolDTO.id });

            // Rol adı benzersiz olmalı
            if (allRolList.Any(x => x.ROL_ADI.Equals(rolDTO.rolAdi, StringComparison.OrdinalIgnoreCase)))
            {
                throw new AppException(MessageCode.ERROR_503_GECERSIZ_VERI_GONDERIMI, $"'{rolDTO.rolAdi}' adlı başka bir rol zaten mevcut");
            }
        }

        public List<RolControllerMethodDTO> getRolControllerMethodDtoListByRolId(int rolId, bool isYoksaHataDondur = false)
        {
            var rolControllerMethods = _repoRolControllerMethod.GetList("ROL_ID=@rolId", new { rolId });

            if (isYoksaHataDondur && (rolControllerMethods == null || !rolControllerMethods.Any()))
            {
                throw new AppException(MessageCode.ERROR_503_GECERSIZ_VERI_GONDERIMI, $"{rolId} id'li role ait controller ve method bulunamadı");
            }

            return _mapper.Map<List<RolControllerMethodDTO>>(rolControllerMethods);
        }

        public bool addControllerMethodToRol(int rolId, string controllerName, string methodName)
        {
            // Rol'ün var olduğunu kontrol et
            var rol = getRolByRolId(rolId, isYoksaHataDondur: true);

            var rolControllerMethod = _repoRolControllerMethod.GetList(
                "ROL_ID=@rolId AND CONTROLLER_NAME=@controllerName AND METHOD_NAME=@methodName", 
                new { rolId, controllerName, methodName }
            ).FirstOrDefault();

            if (rolControllerMethod != null)
            {
                return true;
            }

            rolControllerMethod = new t_rol_controller_method
            {
                ROL_ID = rolId,
                CONTROLLER_NAME = controllerName,
                METHOD_NAME = methodName
            };

            _repoRolControllerMethod.Save(rolControllerMethod);
            return true;
        }

        public bool removeControllerMethodFromRol(int rolId, string controllerName, string methodName)
        {
            var rolControllerMethod = _repoRolControllerMethod.GetList(
                "ROL_ID=@rolId AND CONTROLLER_NAME=@controllerName AND METHOD_NAME=@methodName", 
                new { rolId, controllerName, methodName }
            ).FirstOrDefault();

            if (rolControllerMethod == null)
                return false;

            _repoRolControllerMethod.Delete(rolControllerMethod);
            return true;
        }

        public bool saveRolControllerMethods(int rolId, List<ControllerAndMethodsDTO> controllerMethods)
        {
            // Rol'ün var olduğunu kontrol et
            var rol = getRolByRolId(rolId, isYoksaHataDondur: true);

            // İşlem transaction içinde yapılacak
            using (var transaction = new System.Transactions.TransactionScope(
                System.Transactions.TransactionScopeOption.Required,
                new System.Transactions.TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                System.Transactions.TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    // Önce bu role ait tüm controller-method ilişkilerini sil
                    var existingMethods = _repoRolControllerMethod.GetList("ROL_ID=@rolId", new { rolId });
                    foreach (var method in existingMethods)
                    {
                        _repoRolControllerMethod.Delete(method);
                    }

                    // Seçilen controller ve methodları ekle
                    foreach (var controller in controllerMethods)
                    {
                        foreach (var method in controller.methods)
                        {
                            var rolControllerMethod = new t_rol_controller_method
                            {
                                ROL_ID = rolId,
                                CONTROLLER_NAME = controller.controllerName,
                                METHOD_NAME = method
                            };

                            _repoRolControllerMethod.Save(rolControllerMethod);
                        }
                    }

                    transaction.Complete();
                    return true;
                }
                catch
                {
                    // Transaction başarısız olursa otomatik olarak rollback yapılacak
                    return false;
                }
            }
        }

        /// <summary>
        /// Rol ID'ye göre kişileri getirir
        /// </summary>
        /// <param name="rolId">Rol ID</param>
        /// <param name="isYoksaHataDondur">Sonuç bulunamazsa hata döndürülsün mü</param>
        /// <returns>KisiDTO listesi</returns>
        /// <exception cref="AppException"></exception>
        public List<KisiDTO> getKisiDtoListByRolId(int rolId, bool isYoksaHataDondur = false)
        {
            // Rol ID'ye göre kişi-rol ilişkilerini çek
            var kisiRoller = _repoKisiRol.GetList("ROL_ID=@rolId AND ISDELETED=0", new { rolId });

            if (isYoksaHataDondur && (kisiRoller == null || !kisiRoller.Any()))
            {
                throw new AppException(MessageCode.ERROR_503_GECERSIZ_VERI_GONDERIMI, $"{rolId} id'li role ait kişi bulunamadı");
            }

            List<KisiDTO> kisiListesi = new List<KisiDTO>();
            
            // Her bir kişi-rol ilişkisi için KisiDTO doldur
            foreach (var kisiRol in kisiRoller)
            {
                try
                {
                    // Kişi ID'sine göre Kişi detaylarını getir
                    var kisiDto = _kisiManager.getKisiDtoById(kisiRol.KISI_ID, false);
                    
                    if (kisiDto != null)
                    {
                        kisiListesi.Add(kisiDto);
                    }
                }
                catch (Exception)
                {
                    // Kişi bulunamazsa hata yönetimi (sessizce geç)
                    continue;
                }
            }

            return kisiListesi;
        }
    }
}
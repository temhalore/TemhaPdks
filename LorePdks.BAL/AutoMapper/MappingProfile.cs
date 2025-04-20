using AutoMapper;
using LorePdks.BAL.Managers.Common.Kod.Interfaces;
using LorePdks.COMMON.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace LorePdks.BAL.AutoMapper
{
    public class MappingProfile : Profile
    {
        protected readonly Lazy<IKodManager>_kodManager;
        
        public MappingProfile()
        {
            _kodManager = ServiceProviderHelper.ServiceProvider.GetService<Lazy<IKodManager>>();
            
            // Entity dönüşümleri ilgili mapper sınıflarında tanımlanmıştır.
            // Örnek: KisiMapper, KisiTokenMapper vs.
            // Bu sınıf sadece genel yapılandırma ve ortak özellikleri içerir.
        }
    }
}
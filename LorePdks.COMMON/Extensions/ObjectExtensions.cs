using AutoMapper;
using LorePdks.COMMON.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace LorePdks.COMMON.Extensions
{
    public static class ObjectExtensions
    {
        public static T Map<T>(this object value)
        {
            var _mapper = ServiceProviderHelper.ServiceProvider.GetService<IMapper>();
            return _mapper.Map<T>(value);
        }



    }
}

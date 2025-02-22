
using System.Reflection;
using Castle.DynamicProxy;
using LorePdks.COMMON.Aspects.Logging.Serilog.Logger;

namespace LorePdks.COMMON.Aspects.Interceptors
{
    public class AspectInterceptorSelector : IInterceptorSelector
    {
        // Methodlarda kullanılan aspectleri bulur ve çalıştırır.
        // Tüm methodlarda otomatik olarak kullanılmasını istediğimiz aspectler buraya eklenirse otomatik olarak tüm methodlara eklemiş olur.
        public IInterceptor[] SelectInterceptors(Type type, MethodInfo method, IInterceptor[] interceptors)
        {
            var classAttributes = type.GetCustomAttributes<MethodInterceptionBaseAttribute>(true).ToList();


            var methodAttributes = type.GetMethods().Where(m => m.Name == method.Name).FirstOrDefault()?.GetCustomAttributes<MethodInterceptionBaseAttribute>(true);
            
            if (methodAttributes != null)
                classAttributes.AddRange(methodAttributes);

            classAttributes.Add(new ExceptionLogAspect(typeof(ExceptionLogger))); // Tüm methodlar hata aldığında loglanması sağlar
            classAttributes.Add(new PermissionAspect()); // Tüm methodlar hata aldığında loglanması sağlar

            if (!method.Name.StartsWith("Get") && method.Name != "login" && method.Name != "logout")
            {
                // classAttributes.Add(new LogAspect(typeof(PostLogger))); // Post methodlarına yapılan her isteğin loglanması sağlar
            }

            if (method.Name.StartsWith("Get"))
            {
                //classAttributes.Add(new TransactionReadUncommitted()); // Get methodlarına yapılan her isteği transaction içerisine alır.
            }

            classAttributes.Add(new LogAspect(typeof(UserActionLogger))); // Tüm methodlarda yapılan her isteğin loglanması sağlar

            classAttributes.Add(new PerformanceAspect(5)); // Tüm methodların çalışma süreleri kontrol edilir. Çalışma süresi 5sn geçerse loglanır.

            return classAttributes.OrderBy(x => x.Priority).ToArray();
        }
    }
}

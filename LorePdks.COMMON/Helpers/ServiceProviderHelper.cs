using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace LorePdks.COMMON.Helpers
{
    public static class ServiceProviderHelper
    {
        public static IServiceProvider ServiceProvider { get; set; }
    }

    ////// ServiceTool.cs
    ////public static class ServiceTool
    ////{
    ////    public static IServiceProvider ServiceProvider { get; private set; }

    ////    public static void Create(IServiceProvider serviceProvider)
    ////    {
    ////        ServiceProvider = serviceProvider;
    ////    }
    ////}

    //// ServiceProviderHelper.cs
    //public static class ServiceProviderHelper
    //{
    //    public static IServiceProvider ServiceProvider { get; private set; }

    //    public static void Create(IServiceProvider serviceProvider)
    //    {
    //        ServiceProvider = serviceProvider;
    //    }
    //}
}

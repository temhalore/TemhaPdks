
using AspNetCoreRateLimit;
using LorePdks.API.Filters;
using LorePdks.API;
using LorePdks.BAL.AutoMapper;
using LorePdks.BAL.BaseManager.MinIo.Interfaces;
using LorePdks.BAL.Managers.Common.Kod.Interfaces;
using LorePdks.BAL.Managers.Common.Kod;
using LorePdks.BAL.Managers.Deneme.Interfaces;
using LorePdks.BAL.Managers.Deneme;
using LorePdks.BAL.Managers.Helper.Interfaces;
using LorePdks.BAL.Managers.Helper;
using LorePdks.COMMON.Aspects.Caching;
using LorePdks.COMMON.Configuration;
using LorePdks.COMMON.Models.ServiceResponse.Interfaces;
using LorePdks.COMMON.Models.ServiceResponse;
using Microsoft.OpenApi.Models;
using Quartz;
using System.Diagnostics;
//using LorePdks.BAL.Managers.Job;
using LorePdks.COMMON.Extensions;
using LorePdks.BAL.BaseManager;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using Autofac;
using Serilog;
using static System.Runtime.InteropServices.JavaScript.JSType;
using LorePdks.COMMON.Aspects.Interceptors;
using System.Reflection;
using LorePdks.COMMON.Helpers;
using LorePdks.COMMON.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions();
builder.Services.AddMemoryCache();
#region  appsetting ve confla kısmı

//enviroment değerindeki settingsleride yüklemek için alttaki kod ( normalde otomatik Development ve Production u ekliyor kullanıma 
builder.Configuration.AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true, true);

// direk build öncesi atama yapmak için ve aşağıda rahat kullanmak için

var sectionCoreConfig = builder.Configuration.GetSection("CoreConfig");
sectionCoreConfig.Get<CoreConfig>(); // modele direk atama yapar //Prj.COMMON.Configuration.CoreConfig.TokenKeyName = cm.GetSection("CoreConfig:TokenKeyName").Value; tek tek eklemeye gerek kalmaz

CoreConfig.ConfigName = builder.Configuration.GetSection("ConfigName").Value;   // config name valuemizi uygun yere yazalım bilgi amaçlı




#endregion
//builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
//builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>  // Container yerine ContainerBuilder kullanıyoruz
//{
//    containerBuilder.RegisterModule(new BusinessModule()); // BusinessModule'ü kaydet
//});




builder.Services.AddInMemoryRateLimiting();

builder.Services.AddTransient(
               typeof(Lazy<>),
               typeof(LazilyResolved<>));

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddSingleton<IClientPolicyStore, MemoryCacheClientPolicyStore>();
builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();



//builder.Services.AddTransient<IKisiTokenManager, KisiTokenManager>();
//builder.Services.AddTransient<IAuthManager, AuthManager>();
//builder.Services.AddTransient<IMenuManager, MenuManager>();
//builder.Services.AddTransient<IWidgetManager, WidgetManager>();
//builder.Services.AddTransient<IPermissionManager, PermissionManager>();
//builder.Services.AddTransient<IPageManager, PageManager>();
//builder.Services.AddTransient<IRoleManager, RoleManager>();
//builder.Services.AddTransient<IRoleUserManager, RoleUserManager>();
//builder.Services.AddTransient<IUserManager, UserManager>();
//builder.Services.AddTransient<IRoleWidgetManager, RoleWidgetManager>();
//builder.Services.AddTransient<IDataTableService, DataTableService>();


//builder.Services.AddSingleton<RedisServer>();
//builder.Services.AddSingleton<ICacheManager, RedisCacheManager>();
builder.Services.AddSingleton<ICacheManager, MemoryCacheManager>(); // redis değil bunu kullanacağız
builder.Services.AddSingleton<Stopwatch>();


builder.Logging.AddSerilog(Log.Logger);
//buid anında kullanılmayacaklar .app ten sonra setlenecek
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.Configure<ClientRateLimitOptions>(builder.Configuration.GetSection("ClientRateLimiting"));
builder.Services.Configure<ClientRateLimitPolicies>(builder.Configuration.GetSection("ClientRateLimitPolicies"));

//builder.Services.AddSingleton<IElasticsearchService, ElasticsearchService>();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader()
        );
});

builder.Services.AddCors();
builder.Services.AddHttpContextAccessor();

// tüm json nesennelerinde uygulanýr gelen ve giden
builder.Services.AddControllers().
    AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.IgnoreNullValues = true; // null datalarý dönmez ve almaz
        options.JsonSerializerOptions.IgnoreReadOnlyFields = true;
        options.JsonSerializerOptions.IgnoreReadOnlyProperties = true;
    });
builder.Services.AddControllers(options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);
builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddAutoMapper(cfg =>
{
    cfg.ConstructServicesUsing(type => builder.Services.BuildServiceProvider().GetService(type));
    cfg.AddProfile<MappingProfile>(); // MappingProfile'ı ekleyin
});
builder.Services.AddScoped<SecurityFilter>();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "LorePDKS.Api", Version = "v1" });
    c.AddSecurityDefinition("appToken", new OpenApiSecurityScheme
    {
        Description = "Token problemi oluştu: \"{token}\"",
        In = ParameterLocation.Header,
        Name = CoreConfig.TokenKeyName,
        Type = SecuritySchemeType.ApiKey
    });

});





#region crone- job kısmı
//cron ayar kısmı tüm cron ları buradan örnekteki gibi ayarla developer anında çalışmaması için özel if mevcut
//if (!builder.Environment.IsDevelopment())
if (CoreConfig.IsJobRun && !builder.Environment.IsDevelopment())


{


    builder.Services.AddQuartz(q =>
    {
        //  UseMicrosoftDependencyInjectionScopedJobFactory
        q.UseMicrosoftDependencyInjectionJobFactory();

        //// job 1
        //var key_Job_CallBakUrl_Manager = new JobKey("Job_CallBakUrl_Manager");
        //q.AddJob<Job_CallBakUrl_Manager>(opts => opts.WithIdentity(key_Job_CallBakUrl_Manager));
        //q.AddTrigger(opts => opts
        //.ForJob(key_Job_CallBakUrl_Manager)
        //    .WithIdentity("Job_CallBakUrl_Manager-trigger")
        //    .WithCronSchedule("0/5 * * * * ?")
        //    );

        //// job 2
        //var key_Job_SuresiDolanlariGuncelle_Manager = new JobKey("Job_SuresiDolanlariGuncelle_Manager");
        //q.AddJob<Job_SuresiDolanlariGuncelle_Manager>(opts => opts.WithIdentity(key_Job_SuresiDolanlariGuncelle_Manager));
        //q.AddTrigger(opts => opts
        //    .ForJob(key_Job_SuresiDolanlariGuncelle_Manager)
        //    .WithIdentity("Job_SuresiDolanlariGuncelle_Manager-trigger")
        //    .WithCronSchedule("0/5 * * * * ?")
        //    );

    });

    builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

}
#endregion


#region uygulamamızda yazılan bussines servislerin managerlerin vs eklendiği bölüm
builder.Services.AddTransient(typeof(IServiceResponse<>), typeof(ServiceResponse<>));
//builder.Services.AddTransient<IMinioManager, MinioManager>();
builder.Services.AddTransient<IHelperManager, HelperManager>();
builder.Services.AddTransient<IDenemeManager, DenemeManager>();
builder.Services.AddTransient<IKodManager, KodManager>();
builder.Services.AddTransient<IFirmaManager, FirmaManager>();
builder.Services.AddTransient<IFirmaCihazManager, FirmaCihazManager>();

#endregion uygulamamızda yazılan bussines servislerin managerlerin vs eklendiği bölüm

builder.Services.AddControllers(options => options.Filters.Add(new SecurityFilter()));

// Autofac container'ı yapılandır
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
// Autofac modüllerini yapılandır
//builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
//{
//    // Debug için
//    Debug.WriteLine("Configuring Autofac container...");

//    // Castle Dynamic Proxy için gerekli assembly'leri kaydet
//    var assembly = Assembly.GetExecutingAssembly();
//    containerBuilder.RegisterAssemblyTypes(assembly)
//        .AsImplementedInterfaces();

//    // BusinessModule'ü kaydet
//    containerBuilder.RegisterModule(new BusinessModule());

//    Debug.WriteLine("Autofac container configuration completed");
//});

builder.Host.ConfigureContainer<ContainerBuilder>(builder =>
 {
     builder.RegisterModule(new LorePdks.BAL.BaseManager.BusinessModule());
 });


var app = builder.Build();
ServiceProviderHelper.ServiceProvider = app.Services;

var forwardingOptions = new ForwardedHeadersOptions()
{
    ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.All
};
forwardingOptions.KnownNetworks.Clear();
forwardingOptions.KnownProxies.Clear();

app.UseForwardedHeaders(forwardingOptions);


// http requestleri
if (app.Environment.IsDevelopment())
{
    //app.UseDeveloperExceptionPage();
    app.UseSwagger();
       app.UseSwaggerUI();
    //app.UseSwaggerUI(c =>
    //{
    //    c.SwaggerEndpoint("/swagger/v1/swagger.json", "LorePDKS.Api v1");
    //    c.RoutePrefix = "";
    //
    //});
    // app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    //app.UseHsts();
}
else
{
    app.UseHsts();
}

app.Services.GetService<IHttpContextAccessor>();

app.ConfigureCustomMiddleware(); // bunmlar bizim middlewarelerimiz


app.UseDefaultFiles(new DefaultFilesOptions
{
    DefaultFileNames = new
              List<string> { "index.html" }
});
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowAll");
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});



app.Run();


//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.

//builder.Services.AddControllers();
//// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

//app.Run();

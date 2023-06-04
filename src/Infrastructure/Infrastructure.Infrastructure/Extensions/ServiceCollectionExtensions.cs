using Application.EInvoices.Interfaces.VNPT;
using Application.Interfaces;
using Application.Interfaces.CacheRepositories;
using Application.Interfaces.Contexts;
using Application.Interfaces.Repositories;
using Application.Providers;
using Ardalis.Specification;
using Domain.Entities;
using Domain.Identity;
using Infrastructure.Infrastructure.CacheRepositories;
using Infrastructure.Infrastructure.DbContexts;
using Infrastructure.Infrastructure.HubS;
using Infrastructure.Infrastructure.Identity.Services;
using Infrastructure.Infrastructure.Repositories;
using Infrastructure.Webservice.Repository.VNPT;
using Infrastructure.Webservice.Repository.VNPT.HKD;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Reflection;
using VNPay.Services;
using Yanga.Module.EntityFrameworkCore.AuditTrail;

namespace Infrastructure.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddPersistenceContexts(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
        }

        public static void AddRepositories(this IServiceCollection services)
        {
            #region Repositories
            services.AddTransient(typeof(IRepositoryAsync<>), typeof(RepositoryAsync<>));
           // services.AddTransient<AuditableContext, ApplicationDbContext>();
            services.AddTransient<AuditableLogContext, ApplicationDbContext>();
            services.AddTransient<IIdentityService, IdentityService>();

            services.AddTransient<IMemoryCacheRepository, MemoryCacheRepository>();

            services.AddTransient<IPermissionRepository<Permission>, PermissionRepository>();
            services.AddTransient<IPermissionCacheRepository, PermissionCacheRepository>();
            services.AddTransient<ICategoryProductRepository<CategoryProduct>, CategoryProductRepository>();
            services.AddTransient<ICategoryPostRepository<CategoryPost>, CategoryPostRepository>();
            services.AddTransient<ISpecificationEvaluator, SpecificationEvaluator>();
            services.AddTransient<IJobRepository, JobRepository>();
            services.AddTransient<IDapperRepository, DapperBaseRepository>();
            services.AddTransient<IUserManager, UserManager>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IFormFileHelperRepository, FormFileHelperRepository>();
            services.AddTransient<ISignInManagerRepository<ApplicationUser>, SignInManagerRepository>();
            services.AddTransient<IUserManagerCacheRepository<ApplicationUser>, UserManagerCacheRepository>();
            services.AddTransient<IUserManagerRepository<ApplicationUser>, UserManagerRepository>();

            services.AddScoped<IVnPayService, VnPayService>();//ngân hàng


            services.AddTransient(typeof(IRepositoryCacheAsync<>), typeof(RepositoryCacheAsync<>));
            //services.AddLogging();
            services.AddTransient<IStatusOrderRepository, StatusOrderRepository>();
            services.AddTransient<ICartRepository<Cart>, CartRepository>();
            services.AddTransient<ICartDetailtRepository, CartDetailtRepository>();
            services.AddTransient<IOrderRepository<Order>, OrderRepository>();
            services.AddTransient<IInvoicePepository<Invoice>, InvoicePepository>();
            services.AddTransient<IRoomAndTableRepository<RoomAndTable>, RoomAndTableRepository>();
            services.AddTransient<INotifyUserRepository<NotifiUser>, NotifiUserRepository>();
            services.AddTransient<IEInvoiceRepository<EInvoice>, EInvoiceRepository>();
            services.AddTransient<IManagerPatternEInvoiceRepository<ManagerPatternEInvoice>, ManagerPatternEInvoiceRepository>();
            services.AddTransient<ISupplierEInvoiceRepository<SupplierEInvoice>, SupplierEInvoiceRepository>();
            services.AddTransient<IPurchaseOrderRepository<PurchaseOrder>, PurchaseOrderRepository>();
            services.AddTransient<ISuppliersRepository, SuppliersRepository>();
            services.AddTransient<IAreaRepository, AreaRepository>();
            services.AddTransient<IRevenueExpenditureRepository<RevenueExpenditure>, RevenueExpenditureRepository>();
            services.AddTransient<IAutoSendTimerRepository<AutoSendTimer>, AutoSendTimerRepository>();

            // web service VNPT
            services.AddTransient<IVNPTPortalServiceRepository, VNPTPortalServiceRepository>();
            services.AddTransient<IVNPTPublishServiceRepository, VNPTPublishServiceRepository>();
            services.AddTransient<IVNPTBusinessServiceRepository, VNPTBusinessServiceRepository>();

            ///end
            //----------APi HDK
            //services.AddTransient<HttpClient>();//helper
            services.AddHttpClient<IRestClientHelper, RestClientHelper>();
           services.AddTransient<IRestClientHelper, RestClientHelper>();//helper

            services.AddTransient<IVNPTHKDApiRepository, VNPTHKDApiRepository>();
            //-------------
            services.AddTransient<IReportPosRepository, ReportPosRepository>();

            services.AddTransient<IProductPepository<Product>, ProductPepository>();
            services.AddTransient<IContentPromotionProductRepository<ContentPromotionProduct>, ContentPromotionProductRepository>();
            services.AddTransient<IHandlingRepository, HandlingRepository>();
            services.AddTransient<IPaymentMethodRepository, PaymentMethodRepository>();
            services.AddTransient<INotifyChitkenRepository, NotifyChitkenRepository>();
            services.AddTransient<IDetailtKitchenRepository<DetailtKitchen>, DetailtKitchenRepository>();

            services.AddTransient<ITableLinkRepository, TableLinkRepository>();
            services.AddTransient<ICategoryCacheRepository, CategoryCacheRepository>();
            services.AddTransient<ITypeCategoryRepository<TypeCategory>, TypeCategoryRepository>();
            services.AddTransient<ITypeSpecificationRepository, TypeSpecificationRepository>();
            services.AddTransient<ICompanyAdminInfoRepository, CompanyAdminInfoRepository>();

            services.AddTransient<ISpecificationRepository, SpecificationRepository>();
            services.AddTransient<IDistrictRepository, DistrictRepository>();
            services.AddTransient<IWardRepository, WardRepository>();
            services.AddTransient<ICustomerCacheRepository, CustomerCacheRepository>();
            services.AddTransient<IManagerIdCustomerRepository, ManagerIdCustomerRepository>();
            services.AddTransient<IOrderTableRepository, OrderTableRepository>();
            services.AddTransient<IManagerInvNoRepository, ManagerInvNoRepository>();
            services.AddTransient<ITemplateInvoiceRepository<TemplateInvoice>, TemplateInvoiceRepository>();
            services.AddTransient<IHistoryOrderRepository<HistoryOrder>, HistoryOrderRepository>();

            services.AddTransient<IPostRepository<Post>, PostRepository>();
            services.AddTransient<IPagePostRepository<PagePost>, PagePostRepository>();
            services.AddTransient<ICustomerRepository, CustomerRepository>();
            services.AddTransient<IPromotionRunRepository, PromotionRunRepository>();
            services.AddTransient<ICommentProductRepository<Comment>, CommentProductRepository>();

            services.AddSingleton<SignalRHub>();
            services.AddSingleton<SubscribeProductTableDependency>();
            // services.AddSingleton<ISubscribeTableDependency>();


            services.AddTransient<IReSearchRepository, ReSearchRepository>();// tạo lần đầu khi chạy ứng dụng và dùng mãi đến khi tắt ứng dụng
            services.AddTransient<IParametersEmailRepository, ParametersEmailRepository>();
            services.AddTransient<IEmailHistoryRepository<Domain.Entities.Mailhistory>, EmailHistoryRepository>();
            services.AddTransient<IEmailRepository<MailSettings>, EmailRepository>();
            services.AddTransient<IDbFactory, DbFactory>();
            services.AddScoped<ILogRepository, LogRepository>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();

            #endregion Repositories
        }

    }
}

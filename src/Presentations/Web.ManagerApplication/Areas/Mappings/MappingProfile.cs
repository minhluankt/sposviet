

using Application.DTOs.Logs;
using Application.Features.Areas.Commands;
using Application.Features.Banners.Commands;
using Application.Features.Brands.Commands;
using Application.Features.CategoryCevenues.Commands;
using Application.Features.CategorysPost.Commands;
using Application.Features.CategorysProduct.Commands;
using Application.Features.Citys.Commands;
using Application.Features.Comments.Commands;
using Application.Features.CompanyInfo.Commands;
using Application.Features.ConfigSystems.Commands;
using Application.Features.Customers.Commands;
using Application.Features.Districts.Commands;
using Application.Features.Invoices.Commands;
using Application.Features.Kitchens.Commands;
using Application.Features.Mail.Commands;
using Application.Features.ManagerPatternEInvoices.Commands;
using Application.Features.OrderTablePos.Commands;
using Application.Features.OrderTables.Commands;
using Application.Features.ParametersEmails.Commands;
using Application.Features.PaymentMethods.Commands;
using Application.Features.Permissions.Commands;
using Application.Features.PostOnePages.Commands;
using Application.Features.Posts.Commands;
using Application.Features.Products.Commands;
using Application.Features.PromotionRuns.Commands;
using Application.Features.ReportPoss.Query;
using Application.Features.RevenueExpenditures.Commands;
using Application.Features.RoomAndTables.Commands;
using Application.Features.Specification.Commands;
using Application.Features.Specificationss.Commands;
using Application.Features.SupplierEInvoices.Commands;
using Application.Features.Supplierss.Commands;
using Application.Features.TemplateInvoices.Commands;
using Application.Features.TypeCategorys.Commands;
using Application.Features.TypeSpecification.Commands;
using Application.Features.Units.Commands;
using AutoMapper;
using Domain.Entities;
using Domain.ViewModel;
using Model;
using Web.ManagerApplication.Areas.Admin.Models;
using Web.ManagerApplication.Areas.Admin.Models.Categorys;
using Yanga.Module.EntityFrameworkCore.AuditTrail.Models;

namespace Web.ManagerApplication.Areas.Mappings
{
    internal class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<decimal, decimal>().ConvertUsing(x => Math.Round(x, 3));
            CreateMap<UpdatePermissionCommand, Domain.Entities.Permission>().ReverseMap();
            CreateMap<CreatePermissionCommand, Domain.Entities.Permission>().ReverseMap();

            CreateMap<CreatePostOnePageCommand, Domain.Entities.PagePost>().ReverseMap();
            CreateMap<UpdatePostOnePageCommand, Domain.Entities.PagePost>().ReverseMap();

            CreateMap<CreateCommentProductCommand, CommentModel>().ReverseMap();
            CreateMap<CreateCommentProductCommand, Comment>().ReverseMap();
            //CreateMap<CreatePermissionCommand, Domain.Entities.Permission>().ReverseMap();


            CreateMap<CreateParametersEmailCommand, ParametersEmail>().ReverseMap();
            CreateMap<UpdateParametersEmailCommand, ParametersEmail>().ReverseMap();

            CreateMap<CreateRoomAndTableCommand, RoomAndTable>().ReverseMap();
            CreateMap<UpdateRoomAndTableCommand, RoomAndTable>().ReverseMap();
            CreateMap<Invoice, OrderTable>().ReverseMap();
            CreateMap<OrderTableItem, InvoiceItem>().ReverseMap();

            CreateMap<SplitOrderCommand, SplitOrderModel>().ReverseMap();
            CreateMap<CreatePaymentMethodCommand, PaymentMethod>().ReverseMap();
            CreateMap<UpdatePaymentMethodCommand, PaymentMethod>().ReverseMap();

            CreateMap<CreateCustomerCommand, CustomerModel>().ReverseMap();
            CreateMap<UpdateCustomerCommand, CustomerModel>().ReverseMap();

            CreateMap<CreateCustomerCommand, Customer>().ReverseMap();
            CreateMap<UpdateCustomerCommand, Customer>().ReverseMap();

            CreateMap<CreateAreaCommand, Area>().ReverseMap();
            CreateMap<UpdateAreaCommand, Area>().ReverseMap();

            CreateMap<CreateTemplateInvoiceCommand, TemplateInvoice>().ReverseMap();
            CreateMap<UpdateTemplateInvoiceCommand, TemplateInvoice>().ReverseMap();

            CreateMap<CreateCategoryCevenueCommand, CategoryCevenue>().ReverseMap();
            CreateMap<UpdateCategoryCevenueCommand, CategoryCevenue>().ReverseMap();

            CreateMap<CreateRevenueExpenditureCommand, Domain.Entities.RevenueExpenditure>().ReverseMap();
            CreateMap<UpdateRevenueExpenditureCommand, Domain.Entities.RevenueExpenditure>().ReverseMap();
            CreateMap<RevenueExpenditureModel, Domain.Entities.RevenueExpenditure> ().ReverseMap();


            CreateMap<UpdateConfigSystemCommand, ConfigSystem>().ReverseMap();
            CreateMap<UpdateConfigSystemCommand, ConfigSystemModel>().ReverseMap();
            CreateMap<UpdateConfigSystemCommand, SellModelSetting>().ReverseMap();

            CreateMap<UpdateConfigSystemCommand, ConfigSaleParametersModel>().ReverseMap();

            CreateMap<CreatePromotionRunCommand, PromotionRunViewModel>().ReverseMap();
            CreateMap<CreatePromotionRunCommand, PromotionRun>().ReverseMap();
            CreateMap<UpdatePromotionRunCommand, PromotionRun>().ReverseMap();
            CreateMap<PromotionRunViewModel, PromotionRun>().ReverseMap();


            CreateMap<CreateCompanyInfoCommand, CompanyAdminInfoViewModel>().ReverseMap();
            CreateMap<UpdateCompanyInfoCommand, CompanyAdminInfoViewModel>().ReverseMap();
            CreateMap<CompanyAdminInfoViewModel, CompanyAdminInfo>().ReverseMap();


            CreateMap<CreateBannerCommand, Banner>().ReverseMap();
            CreateMap<UpdateBannerCommand, Banner>().ReverseMap();

            CreateMap<CreateBrandCommand, Brand>().ReverseMap();
            CreateMap<UpdateBrandCommand, Brand>().ReverseMap();

             CreateMap<CreateSupplierEInvoiceCommand, SupplierEInvoice>().ReverseMap();
            CreateMap<UpdateSupplierEInvoiceCommand, SupplierEInvoice>().ReverseMap();

             CreateMap<CreateManagerPatternEInvoiceCommand, ManagerPatternEInvoice>().ReverseMap();
            CreateMap<UpdateManagerPatternEInvoiceCommand, ManagerPatternEInvoice>().ReverseMap();


            CreateMap<UpdateOrderTableCommand, OrderTableModel>().ReverseMap();
            CreateMap<CreateNotifyChitkenCommand, NotifyKitChenModel>().ReverseMap();
            CreateMap<UpdateNotifyChitkenCommand, NotifyKitChenModel>().ReverseMap();


            CreateMap<GetReportDashboardQuery, SearchReportPosModel>().ReverseMap();

            CreateMap<PublishEInvoiceMergeCommand, PublishInvoiceMergeModel>().ReverseMap();

            CreateMap<UpdatePostCommand, PostModel>().ReverseMap();
            CreateMap<CreatePostCommand, PostModel>().ReverseMap();
            CreateMap<PostModel, Post>().ReverseMap();

            CreateMap<CreateCategorysProductCommand, CategoryProduct>().ReverseMap();
            CreateMap<UpdateCategorysProductCommand, CategoryProduct>().ReverseMap();
            CreateMap<CreateUnitCommand, Unit>().ReverseMap();
            CreateMap<UpdateUnitCommand, Unit>().ReverseMap();

            CreateMap<CreateCategorysPostCommand, CategoryPost>().ReverseMap();
            CreateMap<UpdateCategorysPostCommand, CategoryPost>().ReverseMap();

            CreateMap<CreateSupplierCommand, Suppliers>().ReverseMap();
            CreateMap<UpdateSuppliersCommand, Suppliers>().ReverseMap();

            CreateMap<CustomerModelView, Customer>().ReverseMap();
            CreateMap<CustomerModel, Customer>().ReverseMap();
            CreateMap<UpdateInfoDeliveryCustomerCommand, CustomerModelView>().ReverseMap();

           
            CreateMap<UpdateCustomerCommand, CustomerModelView>().ReverseMap();

            CreateMap<GetReportPosQuery, SearchReportPosModel>().ReverseMap();
            CreateMap<GetReportOnhandQuery, SearchReportPosModel>().ReverseMap();
            CreateMap<ExportImportOnhandQuery, SearchReportPosModel>().ReverseMap();
            CreateMap<GetReportsProductsQuery, SearchReportPosModel>().ReverseMap();


            CreateMap<CartModelView, Cart>().ReverseMap();
            CreateMap<StyleProductModel, StyleProduct>().ReverseMap();
            CreateMap<OptionsNameProductModel, OptionsName>().ReverseMap();
            CreateMap<OptionsDetailtProductModel, OptionsDetailtProduct>().ReverseMap();



            CreateMap<CreateMailSettingCommand, MailSettingViewModel>().ReverseMap();
            CreateMap<UpdateMailSettingCommand, MailSettingViewModel>().ReverseMap();
            CreateMap<MailSettingViewModel, MailSettings>().ReverseMap();


            CreateMap<CreateTypeCategoryCommand, TypeCategoryViewModel>().ReverseMap();
            CreateMap<UpdateTypeCategoryCommand, TypeCategoryViewModel>().ReverseMap();
            //  CreateMap<MailSettingViewModel, MailSettings>().ReverseMap();

            CreateMap<CreateTypeSpecificationsCommand, TypeSpecifications>().ReverseMap();
            CreateMap<UpdateTypeSpecificationsCommand, TypeSpecifications>().ReverseMap();
            //  CreateMap<MailSettingViewModel, MailSettings>().ReverseMap();

            CreateMap<CreateSpecificationsCommand, SpecificationsViewModel>().ReverseMap();
            CreateMap<CreateSpecificationsCommand, Specifications>().ReverseMap();
            CreateMap<UpdateSpecificationsCommand, Specifications>().ReverseMap();
            CreateMap<UpdateSpecificationsCommand, SpecificationsViewModel>().ReverseMap();
            CreateMap<SpecificationsViewModel, Specifications>().ReverseMap();
            //  CreateMap<MailSettingViewModel, MailSettings>().ReverseMap();

            CreateMap<CreateCityCommand, City>().ReverseMap();
            CreateMap<UpdateCityCommand, City>().ReverseMap();
            //  CreateMap<MailSettingViewModel, MailSettings>().ReverseMap();
            CreateMap<CreateDistrictCommand, District>().ReverseMap();
            CreateMap<UpdateDistrictCommand, District>().ReverseMap();
            CreateMap<DistrictViewModel, CreateDistrictCommand>().ReverseMap();
            CreateMap<DistrictViewModel, UpdateDistrictCommand>().ReverseMap();


            CreateMap<CreateProductCommand, ProductModelView>().ReverseMap();
            CreateMap<UpdateProductCommand, ProductModelView>().ReverseMap();
            CreateMap<ProductModelView, Product>().ReverseMap();

            CreateMap<AuditLogResponse, Audit>().ReverseMap();

        }
    }
}

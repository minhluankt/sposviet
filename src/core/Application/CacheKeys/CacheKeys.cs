namespace Application.CacheKeys
{
    public static class PermissionCacheKeys
    {
        public static string ListKey => "PermissionList";

        public static string SelectListKey => "PermissionSelectList";

        public static string GetKey(int PermissionId) => $"Permission-{PermissionId}";

        public static string GetDetailsKey(int PermissionId) => $"PermissionDetails-{PermissionId}";
    }

    public static class CustomerRequestCacheKeys
    {
        public static string ListKey => "CustomerRequest";

        public static string SelectListKey => "CustomerRequest";

        public static string GetKey(int PermissionId) => $"CustomerRequest-{PermissionId}";

        public static string GetDetailsKey(int PermissionId) => $"CustomerRequest-{PermissionId}";
    }
    public static class TypeCategoryCacheKeys
    {
        public static string ListKey => "TypeCategoryRequest";

        public static string SelectListKey => "TypeCategoryRequest";


    }
    public static class CustomerCacheKeys
    {
        public static string ListKey => "CustomerRequest";

        //  public static string SelectListKey => "TypeCategoryRequest";


    }
    public static class PromotionRunCacheKeys
    {
        public static string ListKey => "PromotionRunRequest";
    }
    public static class ParametersEmailCacheKeys
    {
        public static string ListKey => "ParametersEmailRequest";
    }
    public static class SpecificationsCacheKeys
    {
        public static string ListKey => "SpecificationsList";

    }
    public static class UsersCacheKeys
    {
        public static string ListKey => "UsersRequest";
    }
    public static class PostCacheKeys
    {
        public static string ListKey => "PostRequest";
    }
    public static class PagePostCacheKeys
    {
        public static string ListKey => "PagePostRequest";
    }
    public static class ConsultationCacheKeys
    {
        public static string ListKey => "ConsultationRequest";

        //  public static string SelectListKey => "TypeCategoryRequest";


    }
    public static class DepositPaymentCacheKeys
    {
        public static string ListKey => "DepositPaymentCacheKeysRequest";

    }
    public static class PaymentCacheKeys
    {
        public static string ListKey => "PaymentsRequest";

    }
    public static class ReSearchCacheKeys
    {
        public static string ListKey => "ReSearchRequest";

    }
    public static class HistoryReSearchCacheKeys
    {
        public static string ListKey => "HistoryReSearchRequest";

    }
    public static class ConfigSystemCacheKeys
    {
        public static string key => "ConfigSystemRequest";

        //  public static string SelectListKey => "TypeCategoryRequest";


    }
    public static class DistrictCacheKeys
    {
        public static string ListKey => "DistrictRequest";

        //  public static string SelectListKey => "TypeCategoryRequest";


    }
    public static class BrandCacheKeys
    {
        public static string ListKey => "BrandRequest";

        //  public static string SelectListKey => "AccessaryRequest";


    }
    public static class BannerCacheKeys
    {
        public static string ListKey => "BannerRequest";

        //  public static string SelectListKey => "AccessaryRequest";


    }
    public static class CityCacheKeys
    {
        public static string ListKey => "CountryRequest";

        public static string SelectListKey => "CountryRequest";
    }
    public static class ProductCacheKeys
    {
        public static string ListKey => "ProductRequest";

        public static string SelectListKey => "ProductRequest";
    }
    public static class RoomAndTableCacheKeys
    {
        public static string RoomAndTableList(int idtype) => $"RoomAndTableList-{idtype}";
    }

    public static class TypeSpecificationsCacheKeys
    {
        public static string ListKey => "TypeSpecificationsRequest";

        public static string SelectListKey => "TypeSpecificationsRequest";


    }
    public static class CompanyPartnerCacheKeys
    {
        public static string ListKey => "CompanyPartnerList";

        public static string ListkeyByid(int idtype) => $"CompanyPartnerList-{idtype}";
        public static string SelectListKey => "CompanyPartnerSelectList";

        public static string GetKey(int CompanyPartnerId) => $"CompanyPartner-{CompanyPartnerId}";

        public static string GetDetailsKey(int CompanyPartnerId) => $"CompanyPartnerDetails-{CompanyPartnerId}";
    }
    public static class CartByUserCacheKeys
    {
        public static string KeyByid(int idUser) => $"CartOrderUserId-{idUser}";

    }
    public static class CompanyAdminInfoCacheKeys
    {
        public static string ListKey => " CompanyAdminInfo";

    }
    public static class CategoryCacheKeys
    {
        public static string ListProductKey => "CategoryListProduct";
        public static string ListPostKey => "CategoryListPost";
        public static string ListKeyProduct => "ListKeyProduct";

        public static string SelectListKey => "CategorySelectList";

        public static string GetKey(int CategoryId) => $"Category-{CategoryId}";
        public static string GetKey(string name) => $"Category-{name}";

        public static string GetDetailsKey(int CategoryId) => $"CategoryDetails-{CategoryId}";
    }

    public static class MailSetting
    {
        public static string Getkey => "MailSetting";
    }

}

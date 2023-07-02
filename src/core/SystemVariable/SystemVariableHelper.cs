using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace SystemVariable
{
    public static class Config
    {
        public static IConfiguration Configuration { get; set; }
        public static void ConfigsAppsettings(this IServiceCollection services, IConfiguration _configuration)
        {
            services.AddDataProtection();
            Configuration = _configuration;
        }
    }
    public static class SystemVariableHelper
    {

        //private static  IConfiguration Configuration;
        public const int LengthCodeProduct = 6;
        public const int LengthCodeAccessary = 9;
        // public static IConfiguration Configuration { get; }
        //private static readonly ILog Log = LogManager.GetLogger(typeof(SystemVariableHelper));
        public const string passDefaultCompanyUser = "123456aA@";
        public const string lienhehotline = "0918.796.393";
        public const string publicKey = "6A6CE9C74B3B5B39FCD9EA115DB0986F";
        public const string vietqrClientID = "2c3f984f-98f2-4454-89ea-8333491af6a3";
        public const string vietqrAPIKey = "5160dece-7857-4e79-b7de-ed23419e420a";
        public const string folderLog = "Logs/";
        public const string TemplateWord = "Template/";
        public const string FolderUpload = "Upload/";
        public const string FolderSendMail = "SendMail/";
        public const string UrlNoImg = "/images/no-img.png";
        public const string UrlImgPos = "images/ristorante.png";
        public const string UrlImgDefault_user = "images/default-user.png";
        public const string template_sendmail_confirm_account = "template_sendmail_confirm_account.txt";
        //private static Entities db = new Entities();
        //public  static string ConnectionString = "Data Source=S-AA-HB1Y7-HR;Initial Catalog=eIMSHotel;Persist Security Info=True;User ID=hotel;Password=Admin1234;MultipleActiveResultSets=True";
        // public static string ConnectionString = "Data Source=DESKTOP-D6DQIDC\\SQLEXPRESS;Initial Catalog=eIMSHotel;Persist Security Info=True;User ID=hotel;Password=Admin1234;MultipleActiveResultSets=True";
        public static string ConnectionString = Config.Configuration.GetConnectionString("ApplicationConnection");
        public static string IdentityConnection = Config.Configuration.GetConnectionString("IdentityConnection");
        public static string LicenseFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), @"AppDataInvoice\License");
        public static string LicenseName = "lc";
        public static string SignatureValidInvoice = "Signature Valid";
        public static string URISignInvoice = "data:image/png;base64,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";

        public static string DoaminApi = Config.Configuration.GetValue<string>("DoaminApi");
        //public static string FolderBackUp = Configuration.GetSection("FolderBackUp").Value;
        public static int PortProxy = int.Parse(!string.IsNullOrEmpty(Config.Configuration.GetSection("PortProxy").Value) ? Config.Configuration.GetSection("PortProxy").Value : "0");
        public static string DomainProxy = Config.Configuration.GetSection("DomainProxy").Value;
        public static string UserNameProxy = Config.Configuration.GetSection("UserNameProxy").Value;
        public static string PassWordProxy = Config.Configuration.GetSection("PassWordProxy").Value;
        public static bool EnableProxyData = Config.Configuration.GetValue<bool>("EnableProxyData");
        public static string UrlTelegram = Config.Configuration.GetValue<string>("UrlTelegram");
        public static string UrlWeb = Config.Configuration.GetValue<string>("UrlWeb");


    }

}


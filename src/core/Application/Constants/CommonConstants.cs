using System.Collections.Generic;

namespace Application.Constants
{
    public class CommonConstants
    {
        public static readonly string[] listTypeSpecification = { "MUCGIA" };
        public const string PassDefault = "123456aA@";
        public const string URIApiHKD = "/apigateway/api";
    }
    public class FolderRootImg
    {
        public const string files_efinder = "files-efinder";
    }
    public class CommonOKEinvoice
    {
        public const string OK = "OK:";
    }  
    public class CommonKhachle
    {
        public const string Name = "Khách lẻ";
    }  
    public class MidpointRoundingCommon
    {
        public const int Three = 3;
    } 
    public class CommonException
    {
        public const string ExceptionXML = "ExceptionXML:";
        public const string Exception = "Exception:";
    } 
    public class CommonERREinvoice
    {
        public const string ERR = "ERR:";
        public const string ERR13 = "ERR:13";
    } 
   
    public class CommonTypeLoginExt
    {
        public const string Facebook = "Facebook";
    }
    public class CommonParametersSecretUrl
    {
        public const string secret = "secret";
    }
    public class ParametersProduct
    {
        public const string MUCGIA = "MUCGIA";
        public const string STYLESANPHAM = "STYLESANPHAM";
    }
    public class ParametersClassStyleProduct
    {
        public const string NEW = "newrowedit";
        public const string REMOVE = "remove";
    }
    public class ConfigCustomerLogin
    {
        public const string Image = "Image";
        public const string ImageDefault = "~/images/user.png";
    }
    public class ParametersConfigSystem
    {
        public const string pageSizeTable = "pageSizeTable";
        public const string layoutHeader = "layoutHeader";
        public const string pageSizeProductInCategory = "pageSizeProductInCategory";
        public const string lstIdAndNameCategoryShowInHome = "lstIdAndNameCategoryShowInHome";
        public const string SellSettingInHome = "SellSettingInHome";
    }
    public static class KeyTitleMail
    {
        public static string thong_bao_huy_don_hang => "thong_bao_huy_don_hang";
        public static string thong_bao_cap_nhat_trang_thai_don_hang => "thong_bao_cap_nhat_trang_thai_don_hang";
        public static string thong_bao_dat_hang_thanh_cong => "thong_bao_dat_hang_thanh_cong";
        public static string thong_bao_giao_hang_thanh_cong => "thong_bao_giao_hang_thanh_cong";
        public static string xac_nhan_tai_khoan => "xac_nhan_tai_khoan";
    }
    public static class TitleMailConstant
    {
        public const string ban_da_huy_bo_don_hang = "Đơn hàng của bạn đã hủy bỏ";
        public const string ban_co_don_hang_cho_xac_nhan = "Bạn có đơn hàng chờ xác nhận";
        public const string ban_co_don_hang_dang_xu_ly = "Bạn có đơn hàng đang xử lý";
        public const string ban_co_don_hang_dang_van_chuyen = "Bạn có đơn hàng đang vận chuyển";
        public const string ban_co_don_hang_giao_thanh_cong = "Bạn có đơn hàng giao thành công";
    }
    public static class VATRateConstant
    {
        public static decimal[] GetAll() => (new List<decimal>() {-3, -2, -1, 0, 5, 8, 10}).ToArray();
    }
    public static class InfoSeachInvCons
    {
        public static string thong_tin_tra_cuu(string url,string fkey,string macqt) => $"Tra cứu hóa đơn điện tử tại: {url}, mã tra cứu:{fkey}<br/>Mã cơ quan thuế: {macqt}";
    } 
    public static class GetNumberNhapTraHang
    {
        public static string ma_nhap_hang(int no) => $"PNH{no.ToString("0000000")}";
        public static string ma_tra_hang(int no) => $"PTH{no.ToString("0000000")}";
    }
    public static class GetNoProduct
    {
        public static string get_ma_sp(int no) => $"SP{no.ToString("0000000")}";
    } 
    public static class GetEInvoiceNoFormat
    {
        public static string get_no(int no) => $"{no.ToString("00000000")}";
        public static string get_kyhieu(string pattern,string serial) => $"{pattern.Split('/')[0]}{serial}";
    }
    public static class ContentMailConstant
    {
        public static string ban_da_huy_bo_don_hang(string order, string note, string user = "") => $"Đơn hàng <b>{order}</b> của bạn hủy bỏ, lý do: {note}, <br/> Người hủy: {user}";
        public static string ban_co_don_hang_cho_xac_nhan(string order) => $"Đơn hàng <b>{order}</b> của bạn đang chờ Shop xác nhận";
        public static string ban_co_don_hang_dang_xu_ly(string order, string note = "") => $"Đơn hàng <b>{order}</b> của bạn đã được xác nhận và đang được xử lý, {(!string.IsNullOrEmpty(note) ? ", ghi chú " + note : "")}";
        public static string ban_co_don_hang_dang_van_chuyen(string order, string address = "") => $"Đơn hàng <b>{order}</b> của bạn đang vận chuyển{(!string.IsNullOrEmpty(address) ? ", ghi chú " + address : "")}";
        public static string ban_co_don_hang_giao_thanh_cong(string order) => $"Đơn hàng <b>{order}</b> đã giao đến bạn thành công";
    }

}

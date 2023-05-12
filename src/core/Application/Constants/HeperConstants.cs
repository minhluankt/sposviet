namespace Application.Constants
{
    public class HeperConstantss
    {
        public const string ERR000 = "DataException";//dữ liệu k hợp lệ
        public const string ERR001 = "ErrUnknown";
        public const string ERR002 = "EmailFormat"; // không đúng email
        public const string ERR003 = "UserOrEmailNotExit";
        public const string ERR004 = "Emailmatch";
        public const string ERR005 = "EmailExit";
        public const string ERR006 = "PhoneExit";
        public const string ERR007 = "DeleteERR";//
        public const string ERR008 = "CategoryMaxlevel";
        public const string ERR009 = "SessionFail";
        public const string ERR010 = "AddERR";
        public const string ERR011 = "EditERR";
        public const string ERR012 = "NotData"; // không tìm thấy dữ liệu
        public const string ERR013 = "LoadDataErr";
        public const string ERR014 = "ExitData";
        public const string ERR015 = "DataERR";
        public const string ERR016 = "Datause";
        public const string ERR017 = "DataRunNotDone";
        public const string ERR018 = "PostNameExit";
        // company
        public const string ERR019 = "CompanyNotnull";
        public const string ERR020 = "CustaxcodeNotnull";
        public const string ERR021 = "NameCompanyNotnull";
        // role
        public const string ERR022 = "PermissionExit";
        //contract
        public const string ERR023 = "NoContract";

        public const string ERR024 = "AccountNoConfirm"; // chưa xác nhận tài khoản
        public const string ERR025 = "AccountLock"; // tài khoản bị khóa
        public const string ERR026 = "loginFail"; // k đúng tài khoản
        public const string ERR027 = "NoAccount"; // tài khoản không tồn tại
        public const string ERR028 = "UserNotFoud"; // k tìm thấy người dùng

        public const string ERR029 = "ExitCusTaxCode";
        public const string ERR030 = "NodataEvent";
        public const string ERR031 = "EndDateEventisValid";// thời gian kết thúc k hợp lệ
        public const string ERR032 = "EventIsSart"; // Sự kiện dg diễn ra, không thể thay đổi
        public const string ERR041 = "ExitEventIsSart"; // Sự kiện dg diễn ra, không thể thay đổi

        public const string ERR033 = "SessionLoginFail"; // hện thời gian login
        public const string ERR034 = "NoDataProduct"; // k tìm thấy người dùng

        public const string ERR035 = "AddCartERR"; // thêm cart lỗi
        public const string ERR036 = "NoCartItem"; // khong có sản phẩm trong giỏ

        public const string ERR037 = "CancelOrderFail"; // khong có sản phẩm trong giỏ
        public const string ERR038 = "OrderNotAwaitingConfirmation"; // Đơn hàng không thể xóa
        public const string ERR039 = "StatuOrderFail"; // Trạng thái đơn hàng k hợp lệ
        public const string ERR040 = "OrderisCancel"; // Trạng thái đơn hàng k hợp lệ
        /// <summary>
        /// //
        /// </summary>
        public const string ERR042 = "NotOrderPos"; // Trạng thái đơn hàng k hợp lệ
        public const string ERR043 = "NotFoundOrder"; // Không tìm thấy đơn hàng // dành cho pos
        public const string ERR044 = "NotFoundOrderSplit"; // Không tìm thấy dữ liệu order cần tách đên
        public const string ERR045 = "NotFoundTable"; // Không tìm thấy bàn/phòng 
        public const string ERR046 = "SplitQuanityValid"; // không có món cần thông báo
        public const string ERR047 = "ValidDeletechitken"; // không thể xóa món khi số lượng còn > 0
        public const string ERR048 = "CompanyNoTemplateInvoice"; // Công ty chưa cấu hình mẫu
        public const string ERR049 = "NOSETINGPATTERN"; // Công ty chưa cấu hình mẫu số ký hiệu
        public const string ERR050 = "NotSupportMutiVATRateOrder"; // Không hỗ trợ nhiều thues trên 1 đơn
        public const string ERR051 = "VATrateProNotSupportInvoice"; // Hóa đơn có sản phẩm thuế suất không khớp với
        // 
        /// <summary>
        /// / login
        /// </summary>


        //ok

        public const string SUS001 = "ProfileUpdateOk";
        public const string SUS010 = "DeleteUserLogin";
        public const string SUS002 = "Signoutsuccessful";
        public const string SUS003 = "LoginSuccess"; // đăng nhập thàn hcoong

        public const string SUS004 = "SendEmailOK";

        public const string SUS005 = "SentEequestOK";



        public const string SUS006 = "EditOk";
        public const string SUS007 = "DeleteOk";
        public const string SUS008 = "AddOk";
        public const string SUS009 = "RegisterOk";
        public const string SUS011 = "AddCartOK";
        public const string SUS012 = "AddOrderOK";

        public const string SUS013 = "CancelOrderOK";
        public const string SUS014 = "CheckOutOrderOK";
        public const string SUS015 = "SplitOrderOK";
        public const string SUS016 = "UpdateStatuOK";
    }

}

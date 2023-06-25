using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Application.Enums
{

    public enum CommonEnum
    {
        User = 0,
        Company = 1
    } 
 
    public enum EnumTypeSericeOrder
    {
       DON_DAT_BAN = 0,
       DON_AN_TAI_BAN = 1,
       DON_MANG_DI =2,
       DON_GIAO_HANG = 3,
    } 
 

    public enum EnumTypeSyncEInvoice
    {
        TRANG_THAI_HOA_DON = 0,
        TRANG_THAI_CQT = 1
    }
    public enum EnumTypeUpdateDefaultFoodOrder
    {
        UPDATE_FOOD = 0,//CẬP NHẬT MẶT HÀNG ABC
        UPDATE_QUANTITY_FOOD = 1,//CẬP NHẬT SỐ LƯỢNG CỦA MẶT HÀNG ĐÓ
        DELETE_FOOD = 2,//XÓA FOOD ITEM ĐƠN
        DELETE_MUTI_FOOD = 3//XÓA NHIỀU
    }
    public enum AutocompleteTypeCustomer
    {
        NONE = 0,
        CUSCODE = 1,
        CUSNAME = 2,
        BUYER = 3,
        TAXCODE = 4,
    }

    public enum EnumTypeValue
    {
        BOOL = 1,
        STRING = 2,
        INT = 3,
    } 
    public enum EnumTypeUpdatePriceProduct
    {
        PRICE = 1,
        VATPRICE = 2,
        PRICENOVAT = 3,
    }
    public enum EnumTypeEventInvoice
    {
        Cancel = 0,
        Restore = 1,//khôi phục hóa đơn hủy
        PublishEInvoice = 2,//phát  hànhhddt
        CreateEInvoice = 3,//tạo mới hddt
        SycnEInvoice = 4,//dodognf bộ
        SendCQT = 5,//gửi thuế
        Delete = 6,//xóa hóa đơn khỏi hệ thống
        PublishEInvoiceMerge = 7,//phát hành gộp
        DeleteIsMerge = 8,//Xóa các hóa đơn lieine quan đã gộp
        UpdateCustomer = 9,//update khách hàng
        IsGetHashPublishEInvoice = 10,//lấy hash khi phát ahfnh = token vnpt
        PublishEInvoiceTokenByHash = 11,//phát hành sau khi lấy hash = token vnpt
        DeleteEInvoiceErrorInPublish = 12,//xóa các hóa đơn điện tử sau khi người dùng ký token nhưng bấm hủy bỏ k ký
        PublishEInvoieDraft = 13,//Phát hành nháp hóa đơn tạo mới
        DeleteEInvoieDraft = 14,//Xóa hóa đơn  nháp hóa đơn tạo mới
    }
    public enum ENumTypeCustomer
    {
        Personal = 0,
        Company = 1

    }
    public enum ENumTypeEventEinvoicePosrtal
    {
        Print = 0,
        ExportXML = 1,
        ExportPDF = 2

    }
    public enum ENumTypeUpdateProduct
    {
        STOPBUSINESS = 1,
        UNSTOPBUSINESS = 2,

    }
    public enum ENumTypeStausSendCQT
    {
        CHUAGUI_CQT = 0,
        DAGUI_CQT = 1,
        CQT_CHAPNHAN = 2,
        CQT_TUCHOI = 3,
        //0: Chưa gửi cơ quan thuế
        //                           	1: Đã gửi cơ quan thuế
        //                           	2: Đã được CQT chấp nhận
        //                           	3: Đã bị CQT từ chối

    }
    public enum ENumTypeEInvoice
    {
        GTGT = 1,
        BANHANG = 2,
        KHAC = 5,

    } 
    public enum ENumTypePublishServiceEInvoice//phát hành hóa đơn kiểu
    {
        PHATHANH = 0,
        THAYTHE = 1,
        DIEUCHINHTANG = 2,
        DIEUCHINHGIAM = 3,
        DIEUCHINHTHONGTIN = 4
    }
    public enum ENumTypePublishEinvoice
    {

        ERROR = 0,//lỗi=
        DONDAHUY = 1,//DƯƠN ĐÃ HỦY
        PHATHANHOK = 2,//THỰC HIỆN PHÁT HÀNH OK
        TAOMOIOK = 3,//TẠO MỚI OK
        DAPHATHANHROI = 4,// PHÁT HÀNH RỒI
        TAOMOIPHATHANHLOI = 5,//TẠO MỚI MÀ PHÁT HÀNH LỖI
        KHONGCOSANPHAM = 6,//TẠO MỚI MÀ PHÁT HÀNH LỖI
        KHONGTONTAINHACUNGCAP = 7,//K TỒN TẠI NHÀ CUNG CẤP
        PHATHANHLOI = 8,//K TỒN TẠI NHÀ CUNG CẤP
        TRANGTHAIPHATHANHKHONGHOPLE = 9,//TRẠNG THÁI K HỢP LỆ
        DONGBOTHANHCONG = 10,//ĐỒNG BỌ OPK
        DONGBOTHATBAI = 11,//ĐÔNG FBOOJ FAIL
        HOADONCHUAPHATHANH = 12,//HOA ĐƠN CHƯa PHÁT HÀNH
        HUYHOADONOK = 13,//HỦy HÓa ĐƠN OK
        HUYHOADONTHATBAI = 14,//HỦy HÓa ĐƠN FAIL
        GUICQTLOI = 15,//GỬI THUỄ LÔI
        GUICQTTHANHCONG = 16,//GỬI THUỄ ok
        XOADONTHANHCONG = 17,//XÓA HÓA ĐƠN BÁN HÀNG CHƯA PHÁT ĐIỆN TỬ
        XOADONTHATBAI = 18,//XÓA HÓA ĐƠN BÁN HÀNG CHƯA PHÁT ĐIỆN TỬ
        SendCQTOK = 19,//hóa đơn gửi CQT ok
        SendCQTFail = 20,//hóa đơn gửi CQT fail
        SentInvOK = 21,//hóa đơn đã gửi thuế
        UnSentInv = 22,//hóa đơn chưa gửi thuế
        SendCQT = 23,//hóa đơn đã gửi thuế
        GET_HASH_PUBLISH_FAIL = 24,//LẤY CHUỖI HASH ĐỂ KÝ TOKEN THẤT BẠI
    }
    public enum TCHHDVuLoai
    {
        HHoa = 1, // Hàng hóa, dịch vụ
        KMai = 2, // Khuyến mại
        CKhau = 3, // Chiết khấu thương mại 
        GChu = 4, // Ghi chú/diễn giải
    }
    public enum VATRateInv
    {
        KHONGVAT = -1, // Hàng hóa, dịch vụ không chịu thuế là  hóa đơn bán hàng
    }

    public enum NOVAT
    {
        NOVAT = -3, // hóa đơn không có thuế, tức là k xuất hóa đơn k tính thuế, 
    } 
    public enum LISTVAT
    {
        NOVAT = -3, // hóa đơn không có thuế, tức là k xuất hóa đơn k tính thuế, 
        VATRate0 = 0, // hóa đơn không có thuế, tức là k xuất hóa đơn k tính thuế, 
        VATRate5 = 5, // hóa đơn không có thuế, tức là k xuất hóa đơn k tính thuế, 
        VATRate8 = 8, // hóa đơn không có thuế, tức là k xuất hóa đơn k tính thuế, 
        VATRate10 = 10, // hóa đơn không có thuế, tức là k xuất hóa đơn k tính thuế, 
    }
    public enum StatusStaffEventEInvoice
    {
        TaoMoiHoaDon = 1,
        InHoaDon = 1,
        HuyHoaDon = 2,
        PhatHanhHoaDon = 3,
        DongBoHoaDon = 4,
        XoaHoaDon = 5,
        SendCQT = 6,
    }
    public enum StatusEinvoice
    {
        [Display(Name = "Chọn")]
        Null = -1,
        [Display(Name = "Hóa đơn tạo mới")]
        NewInv = 0,
        [Display(Name = "Hóa đơn có đủ chữ ký")]
        SignedInv = 1,
        //InUseInv = 2,
        [Display(Name = "Hóa đơn đã thay thế")]
        ReplacedInv = 3,
        [Display(Name = "Hóa đơn điều chỉnh")]
        AdjustedInv = 4,
        [Display(Name = "Hóa đơn đã bị hủy")]
        CanceledInv = 5,
        [Display(Name = "Hóa đơn chưa gửi thuế")]
        UnSendInv = 6,
        [Display(Name = "Hóa đơn đã gửi cơ quản thuế")]
        SentInv = 7,
        [Display(Name = "Hóa đơn đã được Cơ quan thuế chấp nhận")]
        AcceptedInv = 8,
        [Display(Name = "Hóa đơn bị Cơ quan thuế từ chối")]
        RejectedInv = 9
    }
    public enum ENumSupplierEInvoice
    {
        [Display(Name = "Không xác định nhà cung cấp")]
        NONE = 0,
        [Display(Name = "VNPT - Tập đoàn Bưu chính Viễn thông Việt Nam")]
        VNPT = 1
    }
    public enum ENumTypeSeri //chữ ký số
    {
        NONE = 0,
        [Display(Name = "VNPT HSM")]
        HSM = 1,
        [Display(Name = "VNPT Token")]
        TOKEN = 2,
        [Display(Name = "VNPT SmartCA")]
        VNPTSmartCA = 3,
    }
    public enum EnumConfigParameters //chữ ký số
    {
        NONE = 0,
        [Display(Name = "Cho phép xóa hóa đơn đã thanh toán (hóa đơn chưa xuất VAT)")]
        DELETEINVOICENOPAYMENT = 1,
        [Display(Name = "Cho phép lựa chọn ghi nhận doanh thu khi đã xóa hóa đơn")]
        ACCEPTPAYMENTDELETEINVOICE = 2,
        [Display(Name = "Cho phép bán hàng khi hết hàng tồn kho")]
        ACCEPTSALEOUTOFSTOCK = 3,
        [Display(Name = "Mặc định xuất hóa đơn máy tính tiền khi thanh toán")]
        AUTOVATINPAYMENT = 4,
        [Display(Name = "Xuất gộp thành một hóa đơn điện tử")]
        PUBLISHMERGEINVOICE = 5,
        [Display(Name = "Xóa hóa đơn bán hàng sau khi đã gộp thành một đơn")]
        DELETEIPUBLISHMERGEINVOICEAFTER = 6,
        [Display(Name = "In báo bếp")]
        PRINT_BAO_BEP = 7,
        [Display(Name = "Phương pháp in kết nối Plugin SposViet")]
        PRINT_KET_NOI = 8,
        [Display(Name = "Tính năng nhân viên hủy món có chờ bếp xác nhận")]
        CANCEL_FOOD_PENDING_CONFIRM = 9, 
        [Display(Name = "Tính năng mặc định hàng hóa khi tạo đơn")]
        DEFAULT_FOOD_CREATE_ORDER = 10, 
        [Display(Name = "Tính chiết khấu theo giá sau thuế")]
        DISCOUNT_PRICE_AFTER_TAX = 11,
    }
    public enum EnumConfigParametersType // loại nào
    {
        NONE = 0,
        [Display(Name = "Bán hàng")]
        SELLINVOICE = 1
    }
    public enum ENumTypeManagerInv
    {
        Invoice = 0,
        OrderTable = 1,
        Receipts = 2,//phiếu thu
        Payment = 3,//phiếu chi
        EInvoice = 4,//hóa dodwnd deinetj ử mã duy nhất để tra cứu nhanh
        ImportGoods = 5,//Nhập hàng
        PurchaseReturns = 6,//trả hàng nhập
        Product = 7,//sản phẩm 
        Suppliers = 8//sản phẩm 
    }
    public enum ENumTypePrint
    {

        Printer = 1,
        PrintConvert = 2
    }
    public enum ENumTypeCustomerSEX
    {

        Nam = 1,
        Nu = 2,
        Khac = 3
    }
    public enum CommonEnumSpecifications
    {
        DienTich = 0,
        MucGia = 1,
        Huong = 2
    }
    public enum EnumImgMenus // kiểu ảnh địa diện thực đơn
    {
        BIEUTUONG = 0,
        THUTU = 1
    }
    public enum EnumStatusRevenueExpenditure
    {
        HOANTHANH = 0,
        HUYBO = 1
    }
    public enum EnumTypeTemplate // loại sản phẩm
    {
        INVOICEPOS = 1,
        PRINT_BEP = 2,//in bếp
        PRINT_TAM_TINH = 3//in bếp
    }
    public enum EnumTypeInvoice // loại sản phẩm
    {
        INVOICE = 1,
        INVOICE_ORDER = 2
    }
    public enum EnumStatusCompany // trạng thái cty
    {
        New = 0,//vừa khởi tạo
        Active = 1,//Đang hoạt động
        Expired = 2,//Hết hạn
        Lock = 3,//bị khóa
    }
    public enum EnumTypeCompany // chính thức hay thử nghiệm
    {
        [Display(Name = "Hệ thống thử nghiệm")]
        DEMO = 0,//demo
        [Display(Name = "Hệ thống chính thức")]
        Active = 1,//thử nghiệm

    }
    public enum EnumtypeComponentProduct
    {
        COMPONENT = 1,//LÀ THÀNH PHẦN
        EXTRA_TOPPING = 2,//LÀ MÓN THÊM
    }
    public enum EnumTypeProductCategory
    {
        NONE = 0,//
        PRODUCT = 1,//SẢN PHẨM
        COOKING = 2,//MÓN CHẾ BIẾN
        SERVICE = 3,//LÀ SP DỊCH VỤ như tinhstieefn giờ cho thuế
        COMBO = 4,//LÀ Combo
        EXTRA_TOPPING = 5//LÀ MÓN THÊM,(dành cho món chế biến và hàng hóa thông thường, và dịch vụ)
    }
    public enum EnumTypeProduct // loại dịch vụ
    {
        [Display(Name = "None")]
        NONE = -1,//
        [Display(Name = "Thời trang,shop")]
        THOITRANG = 0,// THỜi TRANG, laptop
        [Display(Name = "Cafe, bar, nhà hàng, bida")]
        AMTHUC = 1, // ĂN UỐnG, cafe,nhà hàng, bida
        [Display(Name = "Nghành hàng bán lẻ")]
        BAN_LE = 2, // BÁN lẻ, như đồ hột maca, thực phẩm, mực..được khách đặt và tạo đơn quản lý
        [Display(Name = "Tạp hóa , siêu thị")]
        TAPHOA_SIEUTHI = 3, // tạp hóa siêu thị
        [Display(Name = "Vật liệu xây dựng")]
        VATLIEU_XAYDUNG = 4, // tạp hóa siêu thị
        [Display(Name = "Nhà nghỉ, Bida, Karaoke")]
        HOTEL_BIDA = 5, // bida, karaoke, hotel,motel
    }
    public enum EnumTypeReportDashboard
    {
        DOANHTHU = 0,// 
    } 
    public enum EnumTypeReportEInvoice
    {
        DOANHtHU_HOADON = 0,// 
        DOANHtHU_SANPHAM_HOADON = 1,// 
    }
    public enum EnumTypeReportPos // loại báo cáo doanh thu
    {
        [Display(Name = "None")]
        NONE = -1,//
        [Display(Name = "Báo cáo doanh thu")]
        DOANHTHU = 0,// 
        [Display(Name = "Hủy đơn")]
        HUYDON = 1, //
        //[Display(Name = "Hình thức thanh toán")]
        //HINHTHUCTHANHTOAN = 2, //
        [Display(Name = "Hình thức phục vụ")]
        HINHTHUCPHUVU = 3, //
        //[Display(Name = "Hóa đơn chưa thanh toán")]
        //HOADONCHUATHANHTOAN = 4, // 
    }
    public enum EnumTypeReportProduct // loại bsao cáo theo mặt hàng
    {
        [Display(Name = "None")]
        NONE = -1,//
        [Display(Name = "Danh mục mặt hàng")]
        DANHMUCMATHANG = 0,// 
        [Display(Name = "Mặt hàng bán chạy")]
        MATHANGBANCHAY = 1, //
    }
    public enum EnumStatusPurchaseOrder // trạng thái
    {
        PHIEU_TAM = 0,
        DA_NHAP_HANG = 1,
        DA_TRA_HANG = 2,
        DA_HUY = 3,
    }
    public enum EnumStatusKitchenOrder // trạng thái món bếp
    {
        MOI = 0,
        READY = 1,
        DONE = 2,
        CANCEL = 3,
        Processing = 4,//đang thực hiện
    }
    public enum EnumTypeKitchenOrder // trạng thái chi tiết lịch sử bếp thông báo món
    {
        HUY = 0,
        THEM = 1,
        CHUYEN = 2,
        GHEP = 3,
    }
    public enum EnumTypeRevenueExpenditure// là thu hay chi
    {

        THU = 1,
        CHI = 2
    }
    public enum EnumTypeNotifyKitChen// loại nhà bếp
    {

        NHA_BEP_1 = 1,
        NHA_BEP_2 = 2
    }
    public enum EnumTypeCategoryThuChi// là danh mục thu chi mặc định của hệ thống khi thu bán hàng và chi nhập hàng
    {
        None = 0,
        [Display(Name = "Thu chi khác")]
        THUCHIKKHAC = 1,
        [Display(Name = "Tiền hàng")]
        TIENHANG = 2,
    }
    public enum EnumTypeObjectRevenueExpenditure// là thu hay chi
    {
        [Display(Name = "Khách hàng")]
        KHACHHANG = 1,
        [Display(Name = "Đối tác/Nhà cung cấp")]
        DOITAC = 2,
        [Display(Name = "Đối tượng khác")]
        DOITUONGKHAC = 3
    }
    public enum EnumTypeSignalRHub // trạng thái thôgn báo
    {
        CHITKEN = 0,
        POS = 1,
        UPDATECHITKEN = 2,
        DELETECHITKEN = 3,
        KITCHENTOPOS = 4,
    }
    public enum EnumTypePrint // loại in
    {
        TEST = 0,//TEST
        PrintBaoBep = 1,//báo hủy và chế biến
        RealtimeOrder = 2,//load đơn
    }  
    public enum EnumTypeNotifyKitchenBar // nhân viên báo cho bếp
    {
        TEST = 0,//TEST
        CANCEL = 1,
        DONE = 2,
    }
    public enum EnumTypeNotifyKitchenOrder // cập nhật thông báo beepslaf đã xong chờ cung ứng
    {
        NOTIFYCHITKEN = 0,// theo món ưu tiên
        Orocessed = 1,// theo món ưu tiên
        UPDATEBYFOOD = 2,//theo món all
        UPDATEBYTABLE = 3,// theo bàn
        DELETEKITCHEN = 4,// theo bàn
        Processing = 5,// update món dg nhận chế biến
    }

    public enum EnumTypePurchaseOrder // kiểu
    {
        NHAP_HANG = 0,
        TRA_HANG_NHAP = 1, //phiếu trả hàng nhập của nhà cung cấp
        TRA_HANG_DON = 2,//LÀ TRẢ hàng lại cho khách khi khách đặt đơn
    }
    public enum EnumStatusOrderTable // trạng thái đơn đặt bàn
    {
        [Display(Name = "Đang phục vụ")]
        DANG_DAT = 0,
        [Display(Name = "Đã thanh toán")]
        DA_THANH_TOAN = 1,
        [Display(Name = "Đã hủy")]
        HUY_BO = 3,

    }
    public enum EnumStatusPublishInvoiceOrder
    {
        NONE = 0,//TỌA
        CREATE = 1,//TỌA
        PUBLISH = 2, //PHÁT HÀNH
        CANCEL = 3, // HỦy BỎ
    }
    public enum EnumStatusInvoice // trạng thái hóa đơn
    {
        [Display(Name = "Đã thanh toán")]
        DA_THANH_TOAN = 1,
        [Display(Name = "Chờ xác nhận thanh toán")]
        CHƠ_XAC_NHAN_THANH_TOAN = 2,
        [Display(Name = "Đã hủy")]
        HUY_BO = 3,
        [Display(Name = "Hoàn tiền")]
        HOAN_TIEN = 4,
        [Display(Name = "Hoàn tiền một phần")]
        HOAN_TIEN_MOT_PHAN = 5,
        [Display(Name = "Đã xóa bỏ")]
        XOA_BO = 6,

    }
    public enum EnumSalesChannel // kiểu ảnh địa diện thực đơn
    {
        BAN_TAI_NHA_HANG = 0,
        BAN_ONLINE = 1
    }

    public enum CustomerAccountStatus
    {
        NoConfirm = 0,
        Confirm = 1,
        Lock = 2,
    }
    public enum FolderUpload
    {

        [Display(Name = "Company")]
        Company,
        [Display(Name = "Product")]
        Product
    }
    public enum StyleProductEnum
    {

        [Display(Name = "MUCGIA")]
        MUCGIA = 0,
        [Display(Name = "STYLESANPHAM")]
        STYLESANPHAM = 1
    }
    public enum CategorySerach
    {
        SanPham = 0,
        TinTuc = 1
    }
    public enum TypeSerach
    {
        Category = 0,
        Keyword = 1
    }
    public enum DeviceType
    {
        IsMobile = 1,
        IsTablet = 2,
        IsDesktop = 3
    }

    public enum StatusPromotionRun
    {
        New = 0, //chưa diễn ra
        Upcoming = 1,//Sắp diễn ra
        Processing = 2,//đang diễn ra
        Done = 3,//đã kt
        Cancel = 4// đã hủy bỏ
    }

    public enum EnumStatusOrder
    {
        AwaitingConfirmation = 0, // chờ xác nhận
        Processing = 1,//Đang xử lý
        Shipping = 2,//đang giao hàng
        Delivered = 3,// đã giao hàng
        Cancel = 4// đã hủy bỏ
    }

    public enum EnumTypeUpdatePos
    {
        Unknown = 0,
        UpdateQuantity = 1,
        RemoveRowItem = 2,
        AddProduct = 3,
        ChangedCustomer = 4,
        RemoveOrder = 5,
        Payment = 6,
        CheckOutOrder = 7,
        AddNoteOrder = 8,
        SplitOrder = 9,
        ReplaceQuantity = 10,// update số lượng theo form nhập luôn nhế
        ConvertInvoice = 11,// chuyển từ hóa đơn sang đơn đặt hàng dành cho bán lẻ
        UpdateRoomOrTableInOrder = 12,// update lại bàn hoặc phòng cho đơn đó khi khách chuyển bàn/phòng
        UpdateNoteAndTopping = 13,// cập nhật note và món thêm của 1 item trong order
        CloneItemOrder = 14,// clone item order
        UpdateStaffOrder = 15,// cập nhật nhân viên cho đơn
        UpdateStatusFoodService = 16,// cập nhật là dừng hay tiếp tục tính giờ cho hàng hóa là dịch vụ tính tiền theo giờ
        UpdateDateTimeFoodService = 17,// cập nhật giờ bắt đầu hay kết thúc của sản phẩm là dịch vụ tính tiền theo giờ
        UpdatePriceAndDiscountItemOrder = 18,// cập nhật giá hoặc giảm giá cho item đơn hàng
    }
    public enum EnumTypeSpitOrder
    {
        Unknown = 0,
        Separate = 1, // tách
        Graft = 2, // ghép
    }
    public enum EnumTypeDiscount  {
    NONE= 0,//
    DISCOUNT= 1,//PHẦN %
    CASH=2//TIỀ MẶT
}
public enum EnumStatusArea
    {
        NONE = -1,
        NGUNG_HOAT_DONG = 0,
        DANG_HOAT_DONG = 1, // tách
    } 
    public enum EnumStatusProduct
    {
        NONE = 0,
        [Display(Name = "Ngưng bán")]
        NGUNG_HOAT_DONG = 1,
        [Display(Name = "Đang bán")]
        DANG_HOAT_DONG = 2, // tách
    }
    public enum EnumTypeTemplatePrint
    {
        NONE = 0,
        [Display(Name = "Mẫu in thanh toán")]
        IN_BILL = 1, 
        [Display(Name = "Mẫu in tạm tính")]
        IN_TAM_TINH = 2,
        [Display(Name = "Mãu in báo bếp món chế biến")]
        IN_BA0_CHE_BIEN =3,
        [Display(Name = "Mãu in báo bếp hủy món")]
        IN_BA0_HUY_CHE_BIEN =4,
    }
}

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
    public enum EnumTypeVATEinvoice
    {
        VAT = 1,
        VAT_MTT = 2
    }  
    public enum EnumTypeSyncEInvoice
    {
        TRANG_THAI_HOA_DON = 0,
        TRANG_THAI_CQT = 1
    } 
    public enum AutocompleteTypeCustomer 
    {
        NONE= 0,
        CUSCODE= 1,
        CUSNAME= 2,
        BUYER= 3,
        TAXCODE= 4,
    }
public enum EnumTypePrintBarCode
    {
        XPRINTER_2TEM = 1
    } 
    public enum EnumTypeValue
    {
        BOOL = 1,
        STRING = 2,
        INT = 3,
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
    }
    public enum ENumTypeCustomer
    {
        Personal = 0,
        Company = 1

    }  public enum ENumTypeEventEinvoicePosrtal
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
    public enum StatusStaffEventEInvoice
    {
      TaoMoiHoaDon =1,
      InHoaDon =1,
      HuyHoaDon =2,
      PhatHanhHoaDon =3,
      DongBoHoaDon =4,
      XoaHoaDon =5,
      SendCQT =6,
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
        NONE =0,
        [Display(Name ="VNPT - Tập đoàn Bưu chính Viễn thông Việt Nam")]
        VNPT = 1
    }
    public enum ENumTypeSeri //chữ ký số
    {
        NONE =0,
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
    } public enum ENumTypeCustomerSEX
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
        INVOICEPOS = 1
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
        DEMO = 0,//demo
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
       SERVICE  = 3,//LÀ SP DỊCH VỤ như tinhstieefn giờ cho thuế
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
        [Display(Name  = "Khách hàng")]
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
    public enum EnumTypeNotifyKitchenOrder // cập nhật thông báo beepslaf đã xong chờ cung ứng
    {
        NOTIFYCHITKEN = 0,// theo món ưu tiên
        Orocessed = 1,// theo món ưu tiên
        UPDATEBYFOOD = 2,//theo món all
        UPDATEBYTABLE = 3,// theo bàn
        DELETEKITCHEN = 4,// theo bàn
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
    }
    public enum EnumTypeSpitOrder
    {
        Unknown = 0,
        Separate = 1, // tách
        Graft = 2, // ghép
    }
    public enum EnumStatusArea
    {
        NONE = -1,
        NGUNG_HOAT_DONG = 0,
        DANG_HOAT_DONG = 1, // tách
    }
}

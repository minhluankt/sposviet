using Application.Constants;
using Application.Enums;

namespace Application.Hepers
{
    public class GeneralMess
    {
        public static string GeneralMessStartPublishEInvoice(string mess)
        {
            string html = string.Empty;
            switch (mess)
            {
                case "ERR:1":
                    html = "Tài khoản đăng nhập sai hoặc không có quyền";
                    break;
                case "ERR:3":
                    html = "Dữ liệu đầu vào không đúng quy định";
                    break;  
                case "ERR:5":
                    html = "Có lỗi xảy ra";
                    break; 
                case "ERR:6":
                    html = "Chuỗi Invtoken không đúng định dạng";
                    break; 
                case "ERR:7":
                    html = "Chuỗi Invtoken phải của cùng 1 loại pattern";
                    break;  
                case "ERR:8":
                    html = "Chuỗi Invtoken phải của cùng 1 loại serial";
                    break;  
                case "ERR:10":
                    html = "Lô có số hóa đơn vượt quá số lượng tối đa cho phép (500)";
                    break; 
                case "ERR:11":
                    html = "Toàn bộ lô hóa đơn hoặc không tìm thấy hoặc đã gửi thuế";
                    break;
                case "ERR:20":
                    html = "Thông tin pattern hoặc serial không hợp lệ";
                    break;
                case "ERR:21":
                    html = "Không tìm thấy công ty hoặc tài khoản không tồn tại";
                    break;
                case "ERR:22":
                    html = "Công ty chưa đăng ký chứng thư số";
                    break;
                case "ERR:28":
                    html = "Chưa có thông tin chứng thư trong hệ thống";
                    break;
                case "ERR:24":
                    html = "Chứng thư truyền lên không đúng với chứng thư đăng ký trong hệ thống";
                    break;
                case "ERR:27":
                    html = "Chứng thư chưa đến thời điểm sử dụng";
                    break;
                case "ERR:26":
                    html = "Chứng thư số hết hạn";
                    break;
                case "ERR:35":
                    html = "Thiếu thông tin pattern và serial";
                    break;
                case "ERR:51":
                    html = "Chứng thư số đã bị thu hồi";
                    break;
                default:
                    break;
            }
            return html +" "+ mess;
        }
        public static string GeneralMessStatusPublishEInvoice(ENumTypePublishEinvoice status)
        {
            string html = string.Empty;
            switch (status)
            {
                case ENumTypePublishEinvoice.DONDAHUY:
                    html = "Đơn đã bị hủy";
                    break;
                case ENumTypePublishEinvoice.PHATHANHOK:
                    html = "Đơn đã phát hành ký số thành công hóa đơn điện tử";
                    break; 
                case ENumTypePublishEinvoice.TAOMOIOK:
                    html = "Đơn đã được tạo mới thành công hóa đơn điện tử";
                    break;
                case ENumTypePublishEinvoice.DAPHATHANHROI:
                    html = "Đơn đã được phát hành hóa đơn điện tử trước đó";
                    break;
                case ENumTypePublishEinvoice.TAOMOIPHATHANHLOI:
                    html = "Đơn đã tạo mới thành công hóa đơn điện tử nhưng phát hành lỗi";
                    break; 
                case ENumTypePublishEinvoice.KHONGCOSANPHAM:
                    html = "Đơn không có sản phẩm không thể phát hành hóa đơn điện tử";
                    break;
                default:
                    break;
            }
            return html;
        }
        public static string GeneralMessEnumStatusInvoice(EnumStatusInvoice status)
        {
            string html = string.Empty;
            switch (status)
            {
                case EnumStatusInvoice.DA_THANH_TOAN:
                    html = "Đã thanh toán";
                    break;
                case EnumStatusInvoice.CHƠ_XAC_NHAN_THANH_TOAN:
                    html = "Chờ xác nhận thanh toán";
                    break;
                case EnumStatusInvoice.HUY_BO:
                    html = "Đã hủy bỏ";
                    break;
                case EnumStatusInvoice.XOA_BO:
                    html = "Đã xóa bỏ";
                    break;
                case EnumStatusInvoice.HOAN_TIEN:
                    html = "Đã hoàn tiền";
                    break;
                case EnumStatusInvoice.HOAN_TIEN_MOT_PHAN:
                    html = "Hoàn tiền một phần";
                    break;
                default:
                    break;
            }

            return html;
        }
        public static string GeneralMessStatusEInvoice(StatusEinvoice status)
        {

            string html = string.Empty;
            switch (status)
            {
                case StatusEinvoice.Null:
                    html = status.ToString();
                    break;
                case StatusEinvoice.NewInv:
                    html = "Hóa đơn vừa khởi tạo";
                    break;
                case StatusEinvoice.SignedInv:
                    html = "Hóa đơn có đủ chữ ký";
                    break;
                //case StatusEinvoice.InUseInv:
                //    html = "Hóa đơn sai sót bị thay thế";
                //    break;
                case StatusEinvoice.ReplacedInv:
                    html = "Hóa đơn sai sót bị thay thế";
                    break;
                case StatusEinvoice.AdjustedInv:
                    html = "Hóa đơn sai sót bị điều chỉnh";
                    break;
                case StatusEinvoice.CanceledInv:
                    html = "Hóa đơn xóa bỏ";
                    break;
                case StatusEinvoice.UnSendInv:
                    html = "Hóa đơn chưa gửi Cơ quan thuế";
                    break;
                case StatusEinvoice.SentInv:
                    html = "Hóa đơn đã gửi Cơ quan thuế";
                    break;
                case StatusEinvoice.AcceptedInv:
                    html = "Hóa đơn đã được Cơ quan thuế chấp nhận";
                    break;
                case StatusEinvoice.RejectedInv:
                    html = "Hóa đơn đã bị Cơ quan thuế từ chối";
                    break;
                default:
                    html = status.ToString();
                    break;
            }
            return html;
        }
        public static string ConvertStatusToString(EnumStatusString messUserString)
        {
            string txt = string.Empty;
            switch (messUserString)
            {
                case EnumStatusString.PassSameConfirmPassword:
                    txt = "Mật khẩu nhập lại không khớp";
                    break;
                case EnumStatusString.Err1:
                    txt = "Không tìm thấy người dùng";
                    break;
                case EnumStatusString.ExceptionErr:
                    txt = "Lỗi Exception không xác định!";
                    break;
                default:
                    break;
            }
            return txt;
        }
        public static string ConvertStatusOrderHtml(EnumStatusOrder EnumStatusOrder)
        {
            string txt = string.Empty;
            switch (EnumStatusOrder)
            {
                case EnumStatusOrder.AwaitingConfirmation:
                    txt = "<span class='badge badge-warning'><i class='fas fa-ban'></i> Chờ xác nhận</span>";
                    break;
                case EnumStatusOrder.Processing:
                    txt = "<span class='badge badge-secondary'><i class='fas fa-sync'></i> Đang xử lý</span>";
                    break;
                case EnumStatusOrder.Shipping:
                    txt = "<span class='badge badge-primary'><i class='fas fa-shipping-fast'></i> Đang vận chuyển</span>";
                    break;
                case EnumStatusOrder.Delivered:
                    txt = "<span class='badge badge-success'><i class='fas fa-box-check'></i> Đã giao hàng</span>";
                    break;
                case EnumStatusOrder.Cancel:
                    txt = "<span class='badge badge-danger'><i class='fas fa-times-circle'></i> Đã hủy</span>";
                    break;
                default:
                    txt = EnumStatusOrder.ToString();
                    break;
            }
            return txt;
        }
        public static string ConvertStatusOrder(EnumStatusOrder EnumStatusOrder)
        {
            string txt = string.Empty;
            switch (EnumStatusOrder)
            {
                case EnumStatusOrder.AwaitingConfirmation:
                    txt = "Chờ xác nhận";
                    break;
                case EnumStatusOrder.Processing:
                    txt = "Đang xử lý";
                    break;
                case EnumStatusOrder.Shipping:
                    txt = "Đang vận chuyển";
                    break;
                case EnumStatusOrder.Delivered:
                    txt = "Giao hàng thành công";
                    break;
                case EnumStatusOrder.Cancel:
                    txt = "Đã hủy";
                    break;
                default:
                    txt = EnumStatusOrder.ToString();
                    break;
            }
            return txt;
        }

        public static string ConvertStatusToString(string code)
        {
            string txt = string.Empty;
            switch (code)
            {

                case HeperConstantss.ERR048:
                    txt = " Công ty chưa cấu hình mẫu in!";
                    break;
                case HeperConstantss.ERR047:
                    txt = " Không thể xóa món khi số lượng còn > 0!";
                    break;
                case HeperConstantss.ERR046:
                    txt = " Không tìm thấy món mới cần thông báo!";
                    break;
                case HeperConstantss.ERR045:
                    txt = " Không tìm thấy dữ liệu bàn/phòng!";
                    break;
                case HeperConstantss.ERR044:
                    txt = " Không tìm thấy dữ liệu order cần tách đến!";
                    break;
                case HeperConstantss.ERR043:
                    txt = " Không tìm thấy đơn tại bàn!";
                    break;
                case HeperConstantss.ERR042:
                    txt = "Trạng thái đơn hàng không hợp lệ!";
                    break;
                case HeperConstantss.ERR041:
                    txt = "Sự kiện dg diễn ra, không thể thay đổi!";
                    break;
                case HeperConstantss.ERR040:
                    txt = "Đơn hàng đã bị hủy bỏ không thể cập nhập!";
                    break;
                case HeperConstantss.ERR039:
                    txt = "Trạng thái đơn hàng không hợp lệ!";
                    break;
                case HeperConstantss.ERR038:
                    txt = "Không thể hủy bỏ đơn hàng!";
                    break;

                case HeperConstantss.SUS016:
                    txt = "Thay đổi trạng thái thành công!";
                    break;
                case HeperConstantss.SUS015:
                    txt = "Tách ghép đơn thành công!";
                    break;
                case HeperConstantss.SUS013:
                    txt = "Hủy đơn hàng thành công!";
                    break;
                case HeperConstantss.SUS014:
                    txt = "Thanh toán thành công!";
                    break;
                case HeperConstantss.ERR037:
                    txt = "Hủy đơn hàng thất bại!";
                    break;
                case HeperConstantss.ERR036:
                    txt = "Giỏ hàng của bạn không có sản phẩm!";
                    break;
                case HeperConstantss.SUS012:
                    txt = "Đặt hàng thành công!";
                    break;
                case HeperConstantss.ERR035:
                    txt = "Có lỗi khi thêm vào giỏ hàng, vui lòng thử lại!";
                    break;
                case HeperConstantss.ERR034:
                    txt = "Không tìm thấy sản phẩm, vui lòng thử lại!";
                    break;
                case HeperConstantss.SUS011:
                    txt = "Thêm vào giỏ hàng thành công";
                    break;
                case HeperConstantss.ERR033:
                    txt = "Hết thời gian đăng nhập, vui lòng đăng nhập lại!";
                    break;
                case HeperConstantss.ERR032:
                    txt = "Sự kiện đang diễn ra vui lòng không thay đổi thời gian bắt đầu hoặc thời gian kết thúc!";
                    break;
                case HeperConstantss.ERR031:
                    txt = "Thời gian kết thúc sự kiện phải lớn hơn thời gian hiện tại trên 10 phút!";
                    break;
                case HeperConstantss.ERR030:
                    txt = "Sự kiện nhân bản không tồn tại!";
                    break;
                case HeperConstantss.ERR027:
                    txt = "Tài khoản không tồn tại";
                    break;
                case HeperConstantss.SUS003:
                    txt = "Đăng nhập thành công";
                    break;
                case HeperConstantss.ERR028:
                    txt = "Không tìm thấy thông tin  người dùng";
                    break;

                case HeperConstantss.ERR008:
                    txt = "Vui lòng chọn danh mục lớn hơn";
                    break;

                case HeperConstantss.ERR026:
                    txt = "Tên đăng nhập hoặc mật khẩu không đúng";
                    break;

                case HeperConstantss.ERR018:
                    txt = "Tên bài viết đã tồn tại";
                    break;

                case HeperConstantss.ERR025:
                    txt = "Tài khoản đã bị khóa";
                    break;
                case HeperConstantss.ERR024:
                    txt = "Tài khoản chưa được kích hoạt";
                    break;
                case HeperConstantss.SUS005:
                    txt = "Gửi yêu cầu thành công";
                    break;
                case HeperConstantss.SUS007:
                    txt = "Xóa thành công";
                    break;
                case HeperConstantss.ERR007:
                    txt = "Xóa thất bại";
                    break;
                case HeperConstantss.ERR000:
                    txt = "Lỗi dữ liệu không hợp lệ!";
                    break;
                case HeperConstantss.ERR001:
                    txt = "Lỗi Exception không xác định!";
                    break;
                case HeperConstantss.ERR002:
                    txt = "Email không đúng định dạng!";
                    break;
                case HeperConstantss.ERR003:
                    txt = "Tài khoản hoặc email không tồn tại!";
                    break;
                case HeperConstantss.ERR005:
                    txt = "Email đã tồn tại!";
                    break;
                case HeperConstantss.ERR004:
                    txt = "Email trùng với email hiện tại!";
                    break;
                case HeperConstantss.ERR006:
                    txt = "Số điện thoại đã tồn tại!";
                    break;
                case HeperConstantss.SUS008:
                    txt = "Thêm mới thành công!";
                    break;
                case HeperConstantss.SUS009:
                    txt = "Đăng ký thành công!";
                    break;
                case HeperConstantss.ERR010:
                    txt = "Thêm mới thất bại!";
                    break;
                case HeperConstantss.SUS006:
                    txt = "Cập nhật thành công!";
                    break;
                case HeperConstantss.ERR011:
                    txt = "Cập nhật thất bại!";
                    break;
                case HeperConstantss.ERR012:
                    txt = "Không tìm thấy dữ liệu yêu cầu!";
                    break;
                case HeperConstantss.ERR014:
                    txt = "Dữ liệu đã tồn tại";
                    break;
                case HeperConstantss.ERR016:
                    txt = "Dữ liệu đã được sử dụng!";
                    break;
                case HeperConstantss.ERR029:
                    txt = "Lỗi Exception không xác định!";
                    break;
                case HeperConstantss.SUS004:
                    txt = "Gủi email thành công!";
                    break;
                case HeperConstantss.SUS002:
                    txt = "Đăng xuất thành công!";
                    break;
                default:
                    txt = code;
                    break;
            }
            return txt;
        }


    }
}

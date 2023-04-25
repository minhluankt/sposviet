//https://datatables.net/examples/api/show_hide.html
var dataTableOut;
var dataTable;
var ProductsArrays = "ProductsArrays";
var ProductsArrays = [];
var ListArrNameStyleProduct = [];
var ListArrNameStyleProductinDb = [];// danh sách từ db
var _classremoteTr = "remove";
var _classloadTrDb = "readData";
var _varIdGuidSelectproductAutocomplete = "";
var _classPosEvent = {
    input_searchProduct: $(".search-product"),
    input_searchCustomer: $(".search-customer"),

}
var ua = navigator.userAgent.toLowerCase();
var isAndroid = ua.indexOf("android") > -1; //&& ua.indexOf("mobile");
var CusByOrderIvnoice = null;
var NoteByOrderIvnoice = null;
var setTimeoutremoveactiverow;
var ListCusByOrderPos = [];
var ListNoteOrder = [];//
var listport = [8056];
var NOVATRate = -3;
var TypeEventWebSocket = {
    SendTestConnect: -1,//ký hóa đơn
    SignEInvoice: 0,//ký hóa đơn
    PrintEInvoice: 1,
    PrintInvoice: 2,
    PrintBep: 3//thông báo bếp
}
var EnumTypeValue = {
    BOOL: "BOOL",
    STRING: "STRING",
    INT: "INT",
}
var TypeSyncEInvoice =
{
    TRANG_THAI_HOA_DON: 0,
    TRANG_THAI_CQT: 1
}
var AutocompleteTypeCustomer =
{
    NONE: 0,
    CUSCODE: 1,
    CUSNAME: 2,
    BUYER: 3,
    TAXCODE: 4,
}
var ENumTypeSeri = //chữ ký số
{
    NONE: 0,
    HSM: 1,
    TOKEN: 2,
    VNPTSmartCA: 3,
}
var TypeSelectDiscount = {
    Percent: 1,
    Cash: 2
}
var TypeThuChi = {
    THU: 1,
    CHI: 2
}
var TypeEInvoice = {
    GTGT: 1,
    BANHANG: 2,
    KHAC: 5
}
var TypePurchaseOrder = {
    NHAP_HANG: 0,
    TRA_HANG_NHAP: 1, //phiếu trả hàng nhập của nhà cung cấp
    TRA_HANG_DON: 2,//LÀ TRẢ hàng lại cho khách khi khách đặt đơn
}
var TypeVATRateInv = {
    KHONGVAT: -1
}
var EnumTypeProductCategory =
{
    PRODUCT: 1,//SẢN PHẨM
    COOKING: 2,//MÓN CHẾ BIẾN
    SERVICE: 3,//LÀ SP DỊCH VỤ như tinhstieefn giờ cho thuế
    COMBO: 4//LÀ Combo
}
var EnumTypeObjectRevenueExpenditure = {
    KHACHHANG: 1,
    DOITAC: 2,
    DOITUONGKHAC: 3
}
var ENumSupplierEInvoice = {
    VNPT: 1
}
var EnumStatusRevenueExpenditure =
{
    HOANTHANH: 0,
    HUYBO: 1
}
var EnumStatusPurchaseOrder = // trạng thái
{
    PHIEU_TAM: 0,
    DA_NHAP_HANG: 1,
    DA_TRA_HANG: 2,
    DA_HUY: 3,
}
var EnumConfigParameters = //
{
    NONE: 0,
    //[Display(Name = "Cho phép xóa hóa đơn đã thanh toán (hóa đơn chưa xuất VAT)")]
    DELETEINVOICENOPAYMENT: 1,
    //  [Display(Name = "Cho phép lựa chọn ghi nhận doanh thu khi đã xóa hóa đơn")]
    ACCEPTPAYMENTDELETEINVOICE: 2,
    //  [Display(Name = "Cho phép bán hàng khi hết hàng tồn kho")]
    ACCEPTSALEOUTOFSTOCK: 3,
    AUTOVATINPAYMENT: 4,
    PUBLISHMERGEINVOICE: 5,
    DELETEIPUBLISHMERGEINVOICEAFTER: 6,
}
var _TypeUpdatePos = {

    UpdateQuantity: 1,// khi bấm vào nút cộng item
    RemoveRowItem: 2,// khi bấm vào nút trừ item
    AddProduct: 3,// khi bấm vào sản phẩm
    ChangedCustomer: 4,// thay đổi khách hàng
    RemoveOrder: 5,// xóa order
    Payment: 6,// thanh tonas
    CheckOutOrder: 7,// thanh tonas
    AddNoteOrder: 8,// them note cho don
    SplitOrder: 9,// tach don
    ReplaceQuantity: 10,// lấy số này luôn
    ConvertInvoice: 11,// chuyển từ hóa đơn sang đơn đặt hàng dành cho bán lẻ
    UpdateRoomOrTableInOrder: 12// update lại bàn hoặc phòng cho đơn đó khi khách chuyển bàn/phòng
}
var _TypeSpitOrderPos = {
    Unknown: 0,
    Separate: 1, // tách
    Graft: 2, // ghép
}
var _TypeSelectTableRadio = {
    All: 0,
    IsActive: 1, // tách
    No: 2, // ghép
}
var _TypeNotifyKitchenOrder = {
    Orocessed: 1,// theo món ưu tiên
    UPDATEBYFOOD: 2,//theo món all
    UPDATEBYTABLE: 3,// theo bàn
    DELETE: 4,// XÓA KITCHEN LÀ xóa món đó khi sl = 0
}
var StatusKitchenOrder = {
    MOI: 0,
    READY: 1,
    DONE: 2,
    CANCEL: 3,
    DELETE: 4,
}
var EnumTypeSignalRHub = { // trạng thái thôgn báo
    CHITKEN: 0,
    POS: 1,
    UPDATECHITKEN: 2,
    DELETECHITKEN: 3,
    KITCHENTOPOS: 4,
}
var EnumTypeEventInvoice = { // trạng thái hóa đơn
    Cancel: 0,
    Restore: 1,
    PublishEInvoice: 2,
    CreateEInvoice: 3,
    PublishEInvoiceMerge: 4
}
var EnumTypePrinteInvoice = { // trạng thái hóa đơn
    Printer: 1,
    PrintConvert: 2,

}
var EnumTypeConvertInvoice = { // tloại chuyển đổi hóa đơn
    INVOICE: 1,
    INVOICE_ORDER: 2,
}
const zeroPadInvoice = (num, places) => String(num).padStart(8, '0')
jQuery.extend(jQuery.validator.messages, {
    required: "Trường dữ liệu bắt buộc nhập",
    remote: "Please fix this field.",
    email: "Email không đúng định dạng.",
    url: "Please enter a valid URL.",
    date: "Please enter a valid date.",
    dateISO: "Please enter a valid date (ISO).",
    number: "Please enter a valid number.",
    digits: "Please enter only digits.",
    creditcard: "Please enter a valid credit card number.",
    equalTo: "Mật khẩu nhập lại không khớp.",
    accept: "Please enter a value with a valid extension.",
    maxlength: jQuery.validator.format("Trường dữ liệu tối đa {0} ký tự."),
    minlength: jQuery.validator.format("Trường dữ liệu ít nhất {0} ký tự."),
    rangelength: jQuery.validator.format("Please enter a value between {0} and {1} characters long."),
    range: jQuery.validator.format("Please enter a value between {0} and {1}."),
    max: jQuery.validator.format("Please enter a value less than or equal to {0}."),
    min: jQuery.validator.format("Please enter a value greater than or equal to {0}.")
});
typeCurrency = "VND";
this.fnInitialFormatNumber = function (element) {
    var _val = $(element).val();
    if (_val) {
        _val = _val.replaceAll(',', '');
        if ($(element)[0].className.indexOf('quantity') > -1) {
            if (_val.indexOf(',') > -1) {
                $(element).val(parseFloat(_val).format0VND(2, 3, typeCurrency, true));
            } else {
                $(element).val(parseFloat(_val).format0VND(0, 3));
            }
        } else {
            if (typeCurrency == "VND") {
                $(element).val(parseFloat(_val).format0VND(0, 3));
            } else {
                if (typeCurrency == "USD") {
                    $(element).val(parseFloat(_val).format0VND(2, 3, typeCurrency));
                }
            }
        }
    };
};
this.fnFocusOut = function (element) {
    var _val = $(element).val().replaceAll(',', '');
    if ($(element)[0].className.indexOf('quantity') > -1) {
        if (_val) {
            var _fr = _val.indexOf('.') > -1 ? parseFloat(_val).format0VND(3, 3, typeCurrency, true) : parseFloat(_val).format0VND(3, 3, typeCurrency, true);
            $(element).val(_fr || '');
        }
    } else {
        if (_val) {
            var _fr = _val.indexOf('.') > -1 ? parseFloat(_val).format0VND(3, 3, typeCurrency, true) : parseFloat(_val).format0VND(3, 3, typeCurrency, true);
            $(element).val(_fr || '');
        }
    };
};

//6. Chỉ cho nhập số vào textbox (nếu input có class comma thì có thể nhập dấu ','?)
this.checkDotComma = function (numberStr, keyPress) {
    if (numberStr !== null & numberStr !== undefined) {
        var arr = numberStr.toString().split(',');
        // check prevent more than 2 dot and comma after dot
        if (arr.length == 2) {
            if (arr[1].indexOf('.') > 0) {
                return false;
            }
            if (keyPress === 46 || keyPress === 44) {
                return false;
            }
            return true;
        };
        if (arr.length > 2) {
            return false;
        };
        return true;
    }
}
jQuery.fn.ForceNumericOnly = function () {
    return this.each(function () {
        $(this).blur(function () {
            if ($(this).val() == "NaN") {
                $(this).val(0);
            }
        });
        $(this).keypress(function (e) {
            var className = $(this).attr('class');
            //---function base only number allow '-','.',',' 
            var keypressed = null;
            if (window.event) { //IE
                keypressed = window.event.keyCode;
                if (className.indexOf('number') > -1 || className.indexOf('quantity') > -1) {
                    // 46 là dấu '.'; 45 là dấu âm; 44 là dấu ','
                    if ((keypressed < 48 && keypressed != 46 && keypressed != 44 && keypressed != 45) || (keypressed > 57 && keypressed != 189)) {
                        return false;
                    } else {
                        return checkDotComma(e.target.value, keypressed);
                    }
                }
            } else {
                keypressed = e.which; //NON-IE, Standard
                if (keypressed === 8) { return; } // if key is backspace allow remove character
                if (className.indexOf('number') > -1 || className.indexOf('quantity') > -1) {
                    // 46 là dấu '.'; 45 là dấu âm; 44 là dấu ','
                    if ((keypressed < 48 && keypressed != 46 && keypressed != 44 && keypressed != 45) || (keypressed > 57 && keypressed != 189)) {
                        return false;
                    } else {
                        return checkDotComma(e.target.value, keypressed);
                    }
                }
            };
            //--- CND 20170317

            //--- /CND 20170317
        });
    });
};
//$.toast({
//    text: "Don't forget to star the repository if you like it.", // Text that is to be shown in the toast
//    heading: 'Note', // Optional heading to be shown on the toast
//    icon: 'warning', // Type of toast icon
//    showHideTransition: 'fade', // fade, slide or plain
//    allowToastClose: true, // Boolean value true or false
//    hideAfter: 3000, // false to make it sticky or number representing the miliseconds as time after which toast needs to be hidden
//    stack: 5, // false if there should be only one toast at a time or a number representing the maximum number of toasts to be shown at a time
//    position: 'bottom-left', // bottom-left or bottom-right or bottom-center or top-left or top-right or top-center or mid-center or an object representing the left, right, top, bottom values



//    textAlign: 'left',  // Text alignment i.e. left, right or center
//    loader: true,  // Whether to show loader or not. True by default
//    loaderBg: '#9EC600',  // Background color of the toast loader
//    beforeShow: function () { }, // will be triggered before the toast is shown
//    afterShown: function () { }, // will be triggered after the toat has been shown
//    beforeHide: function () { }, // will be triggered before the toast gets hidden
//    afterHidden: function () { }  // will be triggered after the toast has been hidden
//});
var toastrcus = {
    error: function (content) {
        $.toast({
            text: content,
            heading: 'Error', loader: false,
            // hideAfter: false,
            icon: 'error', stack: false,
            position: 'top-right'       // bottom-left or bottom-right or bottom-center or top-left or top-right or top-center or mid-center or an object representing the left, right, top, bottom values to position the toast on page
        })
    },
    success: function (content) {
        $.toast({
            text: content,
            heading: 'Success', loader: false,
            // hideAfter: false,
            icon: 'success', stack: false,
            position: 'top-right'
        })
    },
    warning: function (content) {
        $.toast({
            text: content,
            heading: 'Warning', loader: false,
            // hideAfter: false,
            icon: 'warning', stack: false,
            position: 'top-right'
        })
    }
}
toastr.options = {
    "closeButton": true,
    "debug": false,
    "newestOnTop": false,
    "progressBar": false,
    "positionClass": "toast-top-right",
    "preventDuplicates": true,
    "showDuration": "300",
    "hideDuration": "1000",
    "timeOut": "5000",
    "extendedTimeOut": "1000",
    "showEasing": "swing",
    "hideEasing": "linear",
    "showMethod": "fadeIn",
    "hideMethod": "fadeOut"
}


Number.prototype.format0 = function (n, x, typeCurrency, isQuantity) {

    var numberBehindComma = 0

    var re = "";
    if (isQuantity) {
        var n = numberBehindComma, x = 3;
    } else {
        if (typeCurrency == "USD") {
            n = numberBehindComma;
        } else {
            // nếu là VND 
            n = numberBehindComma;
        };
    };
    if (isNaN(this.valueOf())) {
        return undefined;
    }
    var value = parseFloat(this.toFixed(numberBehindComma));
    re = '\\d(?=(\\d{' + (x || 3) + '})+' + (n > 0 ? '\\.' : '$') + ')';
    value = value.toFixed(Math.max(0, ~ ~n)).replace(new RegExp(re, 'g'), '$&,');
    var result = value.toString().replaceAll(",", "_").replaceAll(".", ",").replaceAll("_", ".");
    return result;
};
Number.prototype.format0VND = function (n, x, typeCurrency, isQuantity) {
    //var vl = this.valueOf();
    //
    var numberBehindComma = 3

    var re = "";
    if (isQuantity) {
        var n = numberBehindComma, x = 3;
    } else {
        if (typeCurrency == "USD") {
            n = numberBehindComma;
        } else {
            // nếu là VND 
            n = numberBehindComma;
        };
    };
    if (isNaN(this.valueOf())) {
        return undefined;
    }

    var value = parseFloat(this.toFixed(numberBehindComma));
    if (value.toString().includes(".")) {
        let _value = value.toString().split('.');
        if (_value[1].length < x) {
            n = _value[1].length;
        }
    } else {
        n = 0;
    }
    re = '\\d(?=(\\d{' + (x || 3) + '})+' + (n > 0 ? '\\.' : '$') + ')';
    value = value.toFixed(Math.max(0, ~ ~n)).replace(new RegExp(re, 'g'), '$&,');

    // var result = value.toString().replaceAll(",", "_").replaceAll(".", ",").replaceAll("_", ".");
    return value;
};
//chuyển giây về đếm ngược dd/hh/mm/ss
function secondsToDhms(seconds) {
    seconds = Number(seconds);
    var d = Math.floor(seconds / (3600 * 24));
    var h = Math.floor(seconds % (3600 * 24) / 3600);
    var m = Math.floor(seconds % 3600 / 60);
    var s = Math.floor(seconds % 60);

    var dDisplay = d > 0 ? d.toString().padStart(2, '0') + (d == 1 ? " day, " : " days, ") : "00 ";
    var hDisplay = h > 0 ? h.toString().padStart(2, '0') + (h == 1 ? " hour, " : " hours, ") : "";
    var mDisplay = m > 0 ? m.toString().padStart(2, '0') + (m == 1 ? " minute, " : " minutes, ") : "";
    var sDisplay = s > 0 ? s.toString().padStart(2, '0') + (s == 1 ? " second" : " seconds") : "";
    return dDisplay + hDisplay + mDisplay + sDisplay;
}

function secondsTohms(seconds) {
    seconds = Number(seconds);
    // var d = Math.floor(seconds / (3600 * 24));
    var h = Math.floor(seconds / 3600);
    var m = Math.floor(seconds % 3600 / 60);
    var s = Math.floor(seconds % 60);

    var hDisplay = h > 0 ? h.toString().padStart(2, '0') + (h == 1 ? " hour, " : "|") : "00|";
    var mDisplay = m > 0 ? m.toString().padStart(2, '0') + (m == 1 ? " minute, " : "|") : "00";
    var sDisplay = s > 0 ? s.toString().padStart(2, '0') + (s == 1 ? " second" : "") : "00";
    return hDisplay + mDisplay + sDisplay;
}
//var showModelsweetalert2 = {
//    Show: function (title, mess, size = "modal-lg") {

//    },
//}
var jQueryModal = {
    Show: function (title, mess, size = "modal-lg") {
        var confirmModal = $('<div id="myModalShowMess" class="modal fade">' +
            '<div class="modal-dialog modal-custom ' + size + '">' +
            '<div class="modal-content">' +
            '<div class="modal-header">' +
            '<h5 class="modal-title w-100">' + title + '</h5>' +
            '<button type="button" class="close" data-dismiss="modal" aria-label="Close">' +
            '<span aria-hidden="true">&times;</span>' +
            '</button>' +
            '</div>' +
            '<div class="modal-body">' +
            mess +
            '</div>' +
            '<div class="modal-footer">' +
            '<button class="btn btn-primary" data-dismiss="modal">Đóng</button>' +
            '</div>' +
            '</div>' +
            '</div>' +
            '</div>');
        //confirmModal.find('#okButton').click(function (event) {
        //    confirmModal.modal('hide');
        //    $(this).unbind();
        //    $(this).remove();
        //});

        confirmModal.modal('show');
        confirmModal.on('hide.bs.modal', function () {
            $(this).unbind();
            $(this).remove();
        });

    },
    ShowNotFooter: function (title, mess, size = "modal-lg") {
        var confirmModal = $('<div id="myModalShowMess" class="modal fade">' +
            '<div class="modal-dialog modal-custom ' + size + '">' +
            '<div class="modal-content">' +
            '<div class="modal-header">' +
            '<h5 class="modal-title w-100">' + title + '</h5>' +
            '<button type="button" class="close" data-dismiss="modal" aria-label="Close">' +
            '<span aria-hidden="true">&times;</span>' +
            '</button>' +
            '</div>' +
            '<div class="modal-body">' +
            mess +
            '</div>' +
            '</div>' +
            '</div>' +
            '</div>');
        //confirmModal.find('#okButton').click(function (event) {
        //    confirmModal.modal('hide');
        //    $(this).unbind();
        //    $(this).remove();
        //});
        confirmModal.modal('show');
        confirmModal.on('hide.bs.modal', function () {
            $(this).unbind();
            $(this).remove();
        });

    }
}

function confirms(heading, question, cancelButtonTxt, okButtonTxt, callback, callbackfalse = null) {
    var confirmModal =
        $('<div class="modal fade" id="myModalShowMess">' +
            '<div class="modal-dialog modal-dialog-centered">' +
            '<div class="modal-content">' +
            '<div class="modal-header">' +
            '<h6 class="modal-title">' + heading + '</h6>' +
            '<button type="button" class="close" data-dismiss="modal" aria-label="Close">' +
            '<span aria-hidden="true">×</span ></button >' +
            '</div>' +

            '<div class="modal-body">' +
            '<p>' + question + '</p>' +
            '</div>' +

            '<div class="modal-footer">' +
            '<button type="button" id="cancelButtonTxt" class="btn btn-secondary" data-dismiss="modal">' +
            cancelButtonTxt +
            '</button>' +
            '<button type="button" id="okButton" class="btn btn-danger">' +
            okButtonTxt +
            '</button>' +
            '</div>' +
            '</div>' +
            '</div>' +
            '</div>');

    confirmModal.find('#okButton').click(function (event) {
        callback();
        //  $(".modal-backdrop").remove();
        //  $(".modal").remove();
        confirmModal.modal('hide');
        $(this).unbind();
        $(this).remove();
    });
    confirmModal.find('#cancelButtonTxt').click(function (event) {
        if (callbackfalse != null) {
            callbackfalse();
            //  $(".modal-backdrop").remove();
            //  $(".modal").remove();

        }
        confirmModal.modal('hide');
        $(this).unbind();
        $(this).remove();
    });
    confirmModal.on('hide.bs.modal', function () {
        $("div#myModal").empty();
        $(this).unbind();
        $(this).remove();

    });
    confirmModal.modal('show');
};
$(document).ajaxStart(function () {
    $.blockUI({
        message: $('<div class="loader mx-auto">\n <div class="line-scale-pulse-out">\n   <div class="bg-success"></div>\n <div class="bg-success"></div>\n  <div class="bg-success"></div>\n <div class="bg-success"></div>\n                                <div class="bg-success"></div>\n                            </div>\n                        </div>')
    })
}).ajaxStop(function () {
    $('[data-toggle="tooltip"]').tooltip();
    Ladda.stopAll();
    $.unblockUI();
});

function toggleFullScreen() {
    var doc = window.document;
    var docEl = doc.documentElement;

    var requestFullScreen = docEl.requestFullscreen || docEl.mozRequestFullScreen || docEl.webkitRequestFullScreen || docEl.msRequestFullscreen;
    var cancelFullScreen = doc.exitFullscreen || doc.mozCancelFullScreen || doc.webkitExitFullscreen || doc.msExitFullscreen;

    if (!doc.fullscreenElement && !doc.mozFullScreenElement && !doc.webkitFullscreenElement && !doc.msFullscreenElement) {
        requestFullScreen.call(docEl);
    }
    else {
        cancelFullScreen.call(doc);
    }
}

function loadeventAddSell() {

    $('input#isRunPromotion').on('ifChanged', function (event) {

        if ($(this).prop("checked") == true) {

            $("#IdPromotionRun").removeAttr("disabled");
            // $("#IdPromotionRun").valid();
            $(this).parents("fieldset").find("input.form-control").removeAttr("disabled");
        } else if ($(this).prop("checked") == false) {
            $("#IdPromotionRun").attr("disabled", true);
            $("#IdPromotionRun").valid();
            $(this).parents("fieldset").find("input.form-control").attr("disabled", true);
        }
    });

    $('input#isPromotion').on('ifChanged', function (event) {

        if ($(this).prop("checked") == true) {
            $(this).parents("fieldset").find("input.form-control").removeAttr("disabled");
        } else if ($(this).prop("checked") == false) {
            $(this).parents("fieldset").find("input.form-control").attr("disabled", true);
        }
    });
    $('input#IsAddingOptions').on('ifChanged', async function (event) {

        let html = `<div class="card-style-option" id="items-input-addcontent">
                    <div class="input-addcontent pr-3 pt-3 pb-3">
                                                       <div class="header-select mb-3 item-grid-sort">
                                                            <label for="">Tên tùy chọn<span class="required">(*)</span></label>

                                                            <button type="button" class="Sortable" role="button"><svg viewBox="0 0 20 20" class="Polaris-Icon__Svg_375hu" focusable="false" aria-hidden="true"><path d="M7 2a2 2 0 1 0 .001 4.001 2 2 0 0 0-.001-4.001zm0 6a2 2 0 1 0 .001 4.001 2 2 0 0 0-.001-4.001zm0 6a2 2 0 1 0 .001 4.001 2 2 0 0 0-.001-4.001zm6-8a2 2 0 1 0-.001-4.001 2 2 0 0 0 .001 4.001zm0 2a2 2 0 1 0 .001 4.001 2 2 0 0 0-.001-4.001zm0 6a2 2 0 1 0 .001 4.001 2 2 0 0 0-.001-4.001z"></path></svg></button>
                                                              
                                                                <select class="form-control">
                                                                    <opiton></opiton>
                                                                </select>
                                                            <i class="fa fa-trash"></i>
                                                        </div>
                                                        <div class="body-addoption">
                                                            <div class="tile-option-choose hidden-Sortable">
                                                                 <label for="">Giá trị tùy chọn<span class="required">(*)</span></label>
                                                            </div>
                                                           <div class="item-grid-sort">
                                                                <button type="button" class="Sortable-body-addoption" role="button"><svg viewBox="0 0 20 20" class="Polaris-Icon__Svg_375hu" focusable="false" aria-hidden="true"><path d="M7 2a2 2 0 1 0 .001 4.001 2 2 0 0 0-.001-4.001zm0 6a2 2 0 1 0 .001 4.001 2 2 0 0 0-.001-4.001zm0 6a2 2 0 1 0 .001 4.001 2 2 0 0 0-.001-4.001zm6-8a2 2 0 1 0-.001-4.001 2 2 0 0 0 .001 4.001zm0 2a2 2 0 1 0 .001 4.001 2 2 0 0 0-.001-4.001zm0 6a2 2 0 1 0 .001 4.001 2 2 0 0 0-.001-4.001z"></path></svg></button>
                                                                <input type="text" class="form-control" data-id="" name="" placeholder="Nhập giá trị" />
                                                                <i class="fa fa-trash"></i>
                                                            </div>
                                                        </div>
                                                        <button class="btn btn-success" type="button" data-style="expand-left">
                                                            <span class="ladda-label"><i class="fas fa-check"></i> Xong</span>
                                                        </button>
                                                    </div>
                                                   
                                                </div>`;
        let htmlfooter = `<div class="card-footer">
                                                    <div class="btn-addoption">
                                                        <i class="fas fa-solid fa-plus mr-2"></i> Thêm tùy chọn khác
                                                    </div>
                                                </div>`;
        if ($(this).prop("checked") == true) {
            $("#add-option-body").append(html);
            $(".option-add .card").append(htmlfooter);
            $("#items-input-addcontent .header-select").addClass("new-input-data");

            //let el = document.getElementById('items-input-addcontent');
            // Sortable.create(el);
            loadEventadmin.eventsortableproduct();
            loadEventadmin.event_removerowsortable($(".header-select"));
            await loadEventadmin.event_loadajaxdataselect2($(".header-select select"));

        } else if ($(this).prop("checked") == false) {
            $("#add-option-body").find(".card-style-option").remove();
            $(".option-add .card-footer").remove();
            loadEventadmin.eventloadremovetablestyleproduct();
        }
        loadEventadmin.eventaddstypeoptionproduct($("#items-input-addcontent"));
        loadEventadmin.eventbtnaddstypeoptionproduct();
    });

}
//$("#fomrvalid").validate({
//    onfocusout: false,
//    onkeyup: false,
//    onclick: false,
//    rules: {
//        "Name": {
//            required: true,
//            minlength: 5
//        },
//        "password": {
//            required: true,
//            minlength: 8
//        },
//        "re-password": {
//            equalTo: "#password",
//            minlength: 8

//        }
//    },
//    messages: {
//        "Name": {
//            required: "Bắt buộc nhập username",
//            maxlength: "Hãy nhập ít nhất 8 ký tự"
//        },
//        "password": {
//            required: "Bắt buộc nhập password",
//            minlength: "Hãy nhập ít nhất 8 ký tự"
//        },
//        "re-password": {
//            equalTo: "Hai password phải giống nhau",
//            minlength: "Hãy nhập ít nhất 8 ký tự"
//        }
//    }
//});
$("#TableData").DataTable({
    "lengthMenu": [
        [10, 25, 50, 100, 200, -1],
        [10, 25, 50, 100, 200, "All"]
    ],
    "pageLength": 10,
    "pagingType": "full_numbers",
    "order": [
        [0, 'asc']
    ],
    "language": {
        "oPaginate": {
            "sFirst": "Đầu",
            "sPrevious": "Trước",
            "sNext": "Tiếp",
            "sLast": "Cuối"
        },

        "sSearch": "Tìm kiếm:",
        "sInfoFiltered": "(được lọc từ _MAX_ mục)",
        "lengthMenu": "Hiển thị _MENU_ dòng trên một trang",
        "zeroRecords": "Không tìm thấy kết quả nào",
        "sInfo": "Hiển thị dòng _START_  đến dòng _END_  trên tổng số _TOTAL_ dòng (Trang _PAGE_/_PAGES_)",
        //"info": "Hiển thị trang _PAGE_ trên tổng _PAGES_ trang",

        "infoEmpty": "Không có dữ liệu",
        "infoFiltered": "(Lọc được từ _MAX_ tổng số mục )"
    }
});

function LoadDataTable() {
    dataTable = $("#TableData").DataTable({
        "lengthMenu": [
            [10, 25, 50, 100, 200, -1],
            [10, 25, 50, 100, 200, "All"]
        ],
        "pageLength": 10,
        "order": [
            [0, 'desc']
        ],
        "pagingType": "full_numbers",
        "language": {
            "oPaginate": {
                "sFirst": "Đầu",
                "sPrevious": "Trước",
                "sNext": "Tiếp",
                "sLast": "Cuối"
            },

            "sSearch": "Tìm kiếm:",
            "sInfoFiltered": "(được lọc từ _MAX_ mục)",
            "lengthMenu": "Hiển thị _MENU_ dòng trên một trang",
            "zeroRecords": "Không tìm thấy kết quả nào",
            "sInfo": "Hiển thị dòng _START_  đến dòng _END_  trên tổng số _TOTAL_ dòng (Trang _PAGE_/_PAGES_)",
            //"info": "Hiển thị trang _PAGE_ trên tổng _PAGES_ trang",

            "infoEmpty": "Không có dữ liệu",
            "infoFiltered": "(Lọc được từ _MAX_ tổng số mục )"
        }
    });
}

function LoadDataTableById(id) {
    dataTable = $("#" + id).DataTable({
        "lengthMenu": [
            [10, 25, 50, 100, 200, -1],
            [10, 25, 50, 100, 200, "All"]
        ],
        "pageLength": 10,
        "pagingType": "full_numbers",
        "language": {
            "oPaginate": {
                "sFirst": "Đầu",
                "sPrevious": "Trước",
                "sNext": "Tiếp",
                "sLast": "Cuối"
            },

            "sSearch": "Tìm kiếm:",
            "sInfoFiltered": "(được lọc từ _MAX_ mục)",
            "lengthMenu": "Hiển thị _MENU_ dòng trên một trang",
            "zeroRecords": "Không tìm thấy kết quả nào",
            "sInfo": "Hiển thị dòng _START_  đến dòng _END_  trên tổng số _TOTAL_ dòng (Trang _PAGE_/_PAGES_)",
            //"info": "Hiển thị trang _PAGE_ trên tổng _PAGES_ trang",

            "infoEmpty": "Không có dữ liệu",
            "infoFiltered": "(Lọc được từ _MAX_ tổng số mục )"
        }
    });
}

function validateFormUser() {
    $("#FormValidate").validate({
        //onfocusout: false,
        // onkeyup: false,
        // onclick: false,

        ignore: 'input[type=hidden]',
        rules: {
            "FullName": {
                required: true,
                minlength: 4
            },
            "Email": {
                required: true,
                email: true
            },
            "Password": {
                required: true,
                minlength: 4
            },
            "ConfirmPassword": {
                equalTo: "#Password",

            }
        },
        messages: {
            "FullName": {
                required: errrequired,
                minlength: errminlength5
            },
            "Email": {
                required: errrequired,
                email: "Email không đúng định dạng"
            },
            "Password": {
                required: errrequired,
                minlength: errminlength5
            },
            "ConfirmPassword": {
                equalTo: "Mật khẩu nhập lại không khớp"
            }
        },
        errorElement: 'span',
        errorPlacement: function errorPlacement(error, element) {
            Ladda.stopAll();
            // input.removeAttr('readonly').removeAttr('disabled');
            error.addClass('invalid-feedback');

            if (element.prop('type') === 'checkbox') {
                error.insertAfter(element.parent('label'));
                Ladda.stopAll();
                //input.removeAttr('readonly').removeAttr('disabled');
            } else {
                var a = element.parent();
                a = a.children().last();

                error.insertAfter(a.last());
                Ladda.stopAll();
                //input.removeAttr('readonly').removeAttr('disabled');
            }
            var elem = $(element);
            if (elem.hasClass("select2-hidden-accessible")) {
                // element = $("#select2-" + elem.attr("id") + "-container").parent();
                element = $("#select2-" + elem.attr("id") + "-container").parents(".form-group");
                //error.insertAfter(element);
                element.append(error);
            }
        },
        // eslint-disable-next-line object-shorthand
        highlight: function highlight(element) {
            let errorClass = "is-invalid";
            $(element).addClass('is-invalid').removeClass('is-valid');
            Ladda.stopAll();
            var elem = $(element);
            if (elem.hasClass("select2-hidden-accessible")) {
                // $("#select2-" + elem.attr("id") + "-container").parent().addClass(errorClass).removeClass('is-valid');
                $("#select2-" + elem.attr("id") + "-container").parents(".form-group").find("span.select2-selection--single").addClass(errorClass).removeClass('is-valid');
            }
            //input.removeAttr('readonly').removeAttr('disabled');
        },
        // eslint-disable-next-line object-shorthand
        unhighlight: function unhighlight(element) {
            let errorClass = "is-invalid";
            $(element).addClass('is-valid').removeClass('is-invalid');
            var elem = $(element);
            if (elem.hasClass("select2-hidden-accessible")) {
                // $("#select2-" + elem.attr("id") + "-container").parent().removeClass(errorClass).addClass('is-valid');
                $("#select2-" + elem.attr("id") + "-container").parents(".form-group").find("span.select2-selection--single").removeClass(errorClass).addClass('is-valid');
            }
            //Ladda.stopAll();
        },
        submitHandler: function (form) {
        }
    });
}

var validateForm = {
    AddPublishInvoiceMerge: function () {
        $("#create-formProduct").validate({
            ignore: 'input[type=hidden]',
            rules: {
                "VATAmount": {
                    required: true,
                    number: true
                },
                "Amount": {
                    required: true,
                    number: true
                },
                "Vatrate": {
                    required: true,
                    number: true
                },
                "IdPaymentMethod": {
                    required: true,
                    number: true,
                    min: 1,
                },
            },
            messages: {
                "VATAmount": {
                    required: errrequired,
                    number: errnumber
                },
                "Amount": {
                    required: errrequired,
                    number: errnumber
                },
                "Vatrate": {
                    required: errrequired,
                    number: errnumber
                },
                "IdPaymentMethod": {
                    required: errrequired,
                    number: errnumber
                },
            },
            errorElement: 'span',
            errorPlacement: function errorPlacement(error, element) {
                Ladda.stopAll();
                // input.removeAttr('readonly').removeAttr('disabled');
                error.addClass('invalid-feedback');

                if (element.prop('type') === 'checkbox') {
                    error.insertAfter(element.parent('label'));
                    Ladda.stopAll();
                    //input.removeAttr('readonly').removeAttr('disabled');
                } else {
                    var a = element.parent();
                    a = a.children().last();

                    error.insertAfter(a.last());
                    Ladda.stopAll();
                    //input.removeAttr('readonly').removeAttr('disabled');
                }
                var elem = $(element);
                if (elem.hasClass("select2-hidden-accessible")) {
                    // element = $("#select2-" + elem.attr("id") + "-container").parent();
                    element = $("#select2-" + elem.attr("id") + "-container").parents(".form-group");
                    //error.insertAfter(element);
                    element.append(error);
                }
            },
            // eslint-disable-next-line object-shorthand
            highlight: function highlight(element) {
                let errorClass = "is-invalid";
                $(element).addClass('is-invalid').removeClass('is-valid');
                Ladda.stopAll();
                var elem = $(element);
                if (elem.hasClass("select2-hidden-accessible")) {
                    // $("#select2-" + elem.attr("id") + "-container").parent().addClass(errorClass).removeClass('is-valid');
                    $("#select2-" + elem.attr("id") + "-container").parents(".form-group").find("span.select2-selection--single").addClass(errorClass).removeClass('is-valid');
                }
                //input.removeAttr('readonly').removeAttr('disabled');
            },
            // eslint-disable-next-line object-shorthand
            unhighlight: function unhighlight(element) {
                let errorClass = "is-invalid";
                $(element).removeClass('is-invalid');
                var elem = $(element);
                if (elem.hasClass("select2-hidden-accessible")) {
                    // $("#select2-" + elem.attr("id") + "-container").parent().removeClass(errorClass).addClass('is-valid');
                    $("#select2-" + elem.attr("id") + "-container").parents(".form-group").find("span.select2-selection--single").removeClass(errorClass).addClass('is-valid');
                }
                //Ladda.stopAll();
            },
            submitHandler: function (form) {
            }
        });
    },
    EditAndAddCustomer: function () {
        $("#create-form").validate({
            ignore: 'input[type=hidden]',
            rules: {
                "Name": {
                    required: true,
                    minlength: 3
                },
            },
            messages: {
                "Name": {
                    required: errrequired,
                    minlength: errminlength3
                },
            },
            errorElement: 'span',
            errorPlacement: function errorPlacement(error, element) {
                Ladda.stopAll();
                // input.removeAttr('readonly').removeAttr('disabled');
                error.addClass('invalid-feedback');

                if (element.prop('type') === 'checkbox') {
                    error.insertAfter(element.parent('label'));
                    Ladda.stopAll();
                    //input.removeAttr('readonly').removeAttr('disabled');
                } else {
                    var a = element.parent();
                    a = a.children().last();

                    error.insertAfter(a.last());
                    Ladda.stopAll();
                    //input.removeAttr('readonly').removeAttr('disabled');
                }
                var elem = $(element);
                if (elem.hasClass("select2-hidden-accessible")) {
                    // element = $("#select2-" + elem.attr("id") + "-container").parent();
                    element = $("#select2-" + elem.attr("id") + "-container").parents(".form-group");
                    //error.insertAfter(element);
                    element.append(error);
                }
            },
            // eslint-disable-next-line object-shorthand
            highlight: function highlight(element) {
                let errorClass = "is-invalid";
                $(element).addClass('is-invalid').removeClass('is-valid');
                Ladda.stopAll();
                var elem = $(element);
                if (elem.hasClass("select2-hidden-accessible")) {
                    // $("#select2-" + elem.attr("id") + "-container").parent().addClass(errorClass).removeClass('is-valid');
                    $("#select2-" + elem.attr("id") + "-container").parents(".form-group").find("span.select2-selection--single").addClass(errorClass).removeClass('is-valid');
                }
                //input.removeAttr('readonly').removeAttr('disabled');
            },
            // eslint-disable-next-line object-shorthand
            unhighlight: function unhighlight(element) {
                let errorClass = "is-invalid";
                $(element).removeClass('is-invalid');
                var elem = $(element);
                if (elem.hasClass("select2-hidden-accessible")) {
                    // $("#select2-" + elem.attr("id") + "-container").parent().removeClass(errorClass).addClass('is-valid');
                    $("#select2-" + elem.attr("id") + "-container").parents(".form-group").find("span.select2-selection--single").removeClass(errorClass).addClass('is-valid');
                }
                //Ladda.stopAll();
            },
            submitHandler: function (form) {
            }
        });
    },
    EditAndAddRevenueExpenditure: function () {
        $("#create-form").validate({
            ignore: 'input[type=hidden]',
            rules: {
                "Name": {
                    required: true,

                },
                "Amount": {
                    required: true,

                },
                "IdCategoryCevenue": {
                    required: true,

                },
                "ObjectRevenueExpenditure": {
                    required: true,
                },
                "IdPayment": {
                    required: true,

                },
                "CustomerName": {
                    required: true,

                },
                "Date": {
                    required: true,

                },
            },
            messages: {
                "Name": {
                    required: errrequired
                },
                "Amount": {
                    required: errrequired,
                }, "IdCategoryCevenue": {
                    required: errrequired
                }, "ObjectRevenueExpenditure": {
                    required: errrequired
                }, "IdPayment": {
                    required: errrequired
                }, "CustomerName": {
                    required: errrequired
                }, "Date": {
                    required: errrequired
                },
            },
            errorElement: 'span',
            errorPlacement: function errorPlacement(error, element) {
                Ladda.stopAll();
                // input.removeAttr('readonly').removeAttr('disabled');
                error.addClass('invalid-feedback');

                if (element.prop('type') === 'checkbox') {
                    error.insertAfter(element.parent('label'));
                    Ladda.stopAll();
                    //input.removeAttr('readonly').removeAttr('disabled');
                } else {
                    var a = element.parent();
                    a = a.children().last();

                    error.insertAfter(a.last());
                    Ladda.stopAll();
                    //input.removeAttr('readonly').removeAttr('disabled');
                }
                var elem = $(element);
                if (elem.hasClass("select2-hidden-accessible")) {
                    // element = $("#select2-" + elem.attr("id") + "-container").parent();
                    element = $("#select2-" + elem.attr("id") + "-container").parents(".form-group");
                    //error.insertAfter(element);
                    element.append(error);
                }
            },
            // eslint-disable-next-line object-shorthand
            highlight: function highlight(element) {
                let errorClass = "is-invalid";
                $(element).addClass('is-invalid').removeClass('is-valid');
                Ladda.stopAll();
                var elem = $(element);
                if (elem.hasClass("select2-hidden-accessible")) {
                    // $("#select2-" + elem.attr("id") + "-container").parent().addClass(errorClass).removeClass('is-valid');
                    $("#select2-" + elem.attr("id") + "-container").parents(".form-group").find("span.select2-selection--single").addClass(errorClass).removeClass('is-valid');
                }
                //input.removeAttr('readonly').removeAttr('disabled');
            },
            // eslint-disable-next-line object-shorthand
            unhighlight: function unhighlight(element) {
                let errorClass = "is-invalid";
                $(element).removeClass('is-invalid');
                var elem = $(element);
                if (elem.hasClass("select2-hidden-accessible")) {
                    // $("#select2-" + elem.attr("id") + "-container").parent().removeClass(errorClass).addClass('is-valid');
                    $("#select2-" + elem.attr("id") + "-container").parents(".form-group").find("span.select2-selection--single").removeClass(errorClass).addClass('is-valid');
                }
                //Ladda.stopAll();
            },
            submitHandler: function (form) {
            }
        });
    },
    formEivnoiceVNPT: function () {
        $("#create-form").validate({
            ignore: 'input[type=hidden]',
            rules: {
                "DomainName": {
                    required: true
                },
                "UserNameService": {
                    required: true
                },
                "PassWordService": {
                    required: true,
                },
                "DomainName": {
                    required: true,
                },
                "UserNameAdmin": {
                    required: true
                }, "PassWordAdmin": {
                    required: true
                },
            },
            messages: {
                "DomainName": {
                    required: errrequired,
                }, "UserNameService": {
                    required: errrequired,
                }, "PassWordService": {
                    required: errrequired,
                }, "DomainName": {
                    required: errrequired,
                }, "UserNameAdmin": {
                    required: errrequired,
                }, "PassWordAdmin": {
                    required: errrequired,
                },
            },
            errorElement: 'span',
            errorPlacement: function errorPlacement(error, element) {
                Ladda.stopAll();
                // input.removeAttr('readonly').removeAttr('disabled');
                error.addClass('invalid-feedback');

                if (element.prop('type') === 'checkbox') {
                    error.insertAfter(element.parent('label'));
                    Ladda.stopAll();
                    //input.removeAttr('readonly').removeAttr('disabled');
                } else {
                    var a = element.parent();
                    a = a.children().last();

                    error.insertAfter(a.last());
                    Ladda.stopAll();
                    //input.removeAttr('readonly').removeAttr('disabled');
                }
                var elem = $(element);
                if (elem.hasClass("select2-hidden-accessible")) {
                    // element = $("#select2-" + elem.attr("id") + "-container").parent();
                    element = $("#select2-" + elem.attr("id") + "-container").parents(".form-group");
                    //error.insertAfter(element);
                    element.append(error);
                }
            },
            // eslint-disable-next-line object-shorthand
            highlight: function highlight(element) {
                let errorClass = "is-invalid";
                $(element).addClass('is-invalid').removeClass('is-valid');
                Ladda.stopAll();
                var elem = $(element);
                if (elem.hasClass("select2-hidden-accessible")) {
                    // $("#select2-" + elem.attr("id") + "-container").parent().addClass(errorClass).removeClass('is-valid');
                    $("#select2-" + elem.attr("id") + "-container").parents(".form-group").find("span.select2-selection--single").addClass(errorClass).removeClass('is-valid');
                }
                //input.removeAttr('readonly').removeAttr('disabled');
            },
            // eslint-disable-next-line object-shorthand
            unhighlight: function unhighlight(element) {
                let errorClass = "is-invalid";
                $(element).removeClass('is-invalid');
                var elem = $(element);
                if (elem.hasClass("select2-hidden-accessible")) {
                    // $("#select2-" + elem.attr("id") + "-container").parent().removeClass(errorClass).addClass('is-valid');
                    $("#select2-" + elem.attr("id") + "-container").parents(".form-group").find("span.select2-selection--single").removeClass(errorClass).addClass('is-valid');
                }
                //Ladda.stopAll();
            },
            submitHandler: function (form) {
            }
        });
    },
    EditAndAddTableRoom: function () {
        $("#create-form").validate({
            ignore: 'input[type=hidden]',
            rules: {
                "Name": {
                    required: true,
                    minlength: 3
                }, "NameSelect": {
                    required: true,
                }, "Fromno": {
                    required: true,
                    min: 1,
                    number: true
                }, "Tono": {
                    required: true,
                    min: 1,
                    number: true
                }
            },
            messages: {
                "Name": {
                    required: errrequired,
                    minlength: errminlength3
                },
                "Fromno": {
                    required: errrequired,
                    number: "Vui lòng nhập số",
                    min: "Số phải bắt đầu từ 1",

                },
                "Tono": {
                    required: errrequired,
                    number: "Vui lòng nhập số",
                    min: "Số phải bắt đầu từ 1",

                }
            },
            errorElement: 'span',
            errorPlacement: function errorPlacement(error, element) {
                Ladda.stopAll();
                // input.removeAttr('readonly').removeAttr('disabled');
                error.addClass('invalid-feedback');

                if (element.prop('type') === 'checkbox') {
                    error.insertAfter(element.parent('label'));
                    Ladda.stopAll();
                    //input.removeAttr('readonly').removeAttr('disabled');
                } else {
                    var a = element.parent();
                    a = a.children().last();

                    error.insertAfter(a.last());
                    Ladda.stopAll();
                    //input.removeAttr('readonly').removeAttr('disabled');
                }
                var elem = $(element);
                if (elem.hasClass("select2-hidden-accessible")) {
                    // element = $("#select2-" + elem.attr("id") + "-container").parent();
                    element = $("#select2-" + elem.attr("id") + "-container").parents(".form-group");
                    //error.insertAfter(element);
                    element.append(error);
                }
            },
            // eslint-disable-next-line object-shorthand
            highlight: function highlight(element) {
                let errorClass = "is-invalid";
                $(element).addClass('is-invalid').removeClass('is-valid');
                Ladda.stopAll();
                var elem = $(element);
                if (elem.hasClass("select2-hidden-accessible")) {
                    // $("#select2-" + elem.attr("id") + "-container").parent().addClass(errorClass).removeClass('is-valid');
                    $("#select2-" + elem.attr("id") + "-container").parents(".form-group").find("span.select2-selection--single").addClass(errorClass).removeClass('is-valid');
                }
                //input.removeAttr('readonly').removeAttr('disabled');
            },
            // eslint-disable-next-line object-shorthand
            unhighlight: function unhighlight(element) {
                let errorClass = "is-invalid";
                $(element).removeClass('is-invalid');
                var elem = $(element);
                if (elem.hasClass("select2-hidden-accessible")) {
                    // $("#select2-" + elem.attr("id") + "-container").parent().removeClass(errorClass).addClass('is-valid');
                    $("#select2-" + elem.attr("id") + "-container").parents(".form-group").find("span.select2-selection--single").removeClass(errorClass).addClass('is-valid');
                }
                //Ladda.stopAll();
            },
            submitHandler: function (form) {
            }
        });
    },

    EditAndAddSupplier: function () {
        $("#create-form").validate({
            ignore: 'input[type=hidden]',
            rules: {
                "Name": {
                    required: true
                },
                "Address": {
                    required: true
                }
            },
            messages: {
                "Name": {
                    required: errrequired
                }, "Address": {
                    required: errrequired
                },
            },
            errorElement: 'span',
            errorPlacement: function errorPlacement(error, element) {
                Ladda.stopAll();
                // input.removeAttr('readonly').removeAttr('disabled');
                error.addClass('invalid-feedback');

                if (element.prop('type') === 'checkbox') {
                    error.insertAfter(element.parent('label'));
                    Ladda.stopAll();
                    //input.removeAttr('readonly').removeAttr('disabled');
                } else {
                    var a = element.parent();
                    a = a.children().last();

                    error.insertAfter(a.last());
                    Ladda.stopAll();
                    //input.removeAttr('readonly').removeAttr('disabled');
                }
                var elem = $(element);
                if (elem.hasClass("select2-hidden-accessible")) {
                    // element = $("#select2-" + elem.attr("id") + "-container").parent();
                    element = $("#select2-" + elem.attr("id") + "-container").parents(".form-group");
                    //error.insertAfter(element);
                    element.append(error);
                }
            },
            // eslint-disable-next-line object-shorthand
            highlight: function highlight(element) {
                let errorClass = "is-invalid";
                $(element).addClass('is-invalid').removeClass('is-valid');
                Ladda.stopAll();
                var elem = $(element);
                if (elem.hasClass("select2-hidden-accessible")) {
                    // $("#select2-" + elem.attr("id") + "-container").parent().addClass(errorClass).removeClass('is-valid');
                    $("#select2-" + elem.attr("id") + "-container").parents(".form-group").find("span.select2-selection--single").addClass(errorClass).removeClass('is-valid');
                }
                //input.removeAttr('readonly').removeAttr('disabled');
            },
            // eslint-disable-next-line object-shorthand
            unhighlight: function unhighlight(element) {
                let errorClass = "is-invalid";
                $(element).removeClass('is-invalid');
                var elem = $(element);
                if (elem.hasClass("select2-hidden-accessible")) {
                    // $("#select2-" + elem.attr("id") + "-container").parent().removeClass(errorClass).addClass('is-valid');
                    $("#select2-" + elem.attr("id") + "-container").parents(".form-group").find("span.select2-selection--single").removeClass(errorClass).addClass('is-valid');
                }
                //Ladda.stopAll();
            },
            submitHandler: function (form) {
            }
        });
    },
    EditAndAddUnit: function () {
        $("#create-form").validate({
            ignore: 'input[type=hidden]',
            rules: {
                "Name": {
                    required: true
                }
            },
            messages: {
                "Name": {
                    required: errrequired
                }
            },
            errorElement: 'span',
            errorPlacement: function errorPlacement(error, element) {
                Ladda.stopAll();
                // input.removeAttr('readonly').removeAttr('disabled');
                error.addClass('invalid-feedback');

                if (element.prop('type') === 'checkbox') {
                    error.insertAfter(element.parent('label'));
                    Ladda.stopAll();
                    //input.removeAttr('readonly').removeAttr('disabled');
                } else {
                    var a = element.parent();
                    a = a.children().last();

                    error.insertAfter(a.last());
                    Ladda.stopAll();
                    //input.removeAttr('readonly').removeAttr('disabled');
                }
                var elem = $(element);
                if (elem.hasClass("select2-hidden-accessible")) {
                    // element = $("#select2-" + elem.attr("id") + "-container").parent();
                    element = $("#select2-" + elem.attr("id") + "-container").parents(".form-group");
                    //error.insertAfter(element);
                    element.append(error);
                }
            },
            // eslint-disable-next-line object-shorthand
            highlight: function highlight(element) {
                let errorClass = "is-invalid";
                $(element).addClass('is-invalid').removeClass('is-valid');
                Ladda.stopAll();
                var elem = $(element);
                if (elem.hasClass("select2-hidden-accessible")) {
                    // $("#select2-" + elem.attr("id") + "-container").parent().addClass(errorClass).removeClass('is-valid');
                    $("#select2-" + elem.attr("id") + "-container").parents(".form-group").find("span.select2-selection--single").addClass(errorClass).removeClass('is-valid');
                }
                //input.removeAttr('readonly').removeAttr('disabled');
            },
            // eslint-disable-next-line object-shorthand
            unhighlight: function unhighlight(element) {
                let errorClass = "is-invalid";
                $(element).removeClass('is-invalid');
                var elem = $(element);
                if (elem.hasClass("select2-hidden-accessible")) {
                    // $("#select2-" + elem.attr("id") + "-container").parent().removeClass(errorClass).addClass('is-valid');
                    $("#select2-" + elem.attr("id") + "-container").parents(".form-group").find("span.select2-selection--single").removeClass(errorClass).addClass('is-valid');
                }
                //Ladda.stopAll();
            },
            submitHandler: function (form) {
            }
        });
    }
    , EditAndAddPaymentMethod: function () {
        $("#create-form").validate({
            ignore: 'input[type=hidden]',
            rules: {
                "Name": {
                    required: true,
                    minlength: 3
                }, "Code": {
                    required: true,
                }
            },
            messages: {
                "Name": {
                    required: errrequired,
                    minlength: errminlength3
                },
                "Code": {
                    required: errrequired,
                }
            },
            errorElement: 'span',
            errorPlacement: function errorPlacement(error, element) {
                Ladda.stopAll();
                // input.removeAttr('readonly').removeAttr('disabled');
                error.addClass('invalid-feedback');

                if (element.prop('type') === 'checkbox') {
                    error.insertAfter(element.parent('label'));
                    Ladda.stopAll();
                    //input.removeAttr('readonly').removeAttr('disabled');
                } else {
                    var a = element.parent();
                    a = a.children().last();

                    error.insertAfter(a.last());
                    Ladda.stopAll();
                    //input.removeAttr('readonly').removeAttr('disabled');
                }
                var elem = $(element);
                if (elem.hasClass("select2-hidden-accessible")) {
                    // element = $("#select2-" + elem.attr("id") + "-container").parent();
                    element = $("#select2-" + elem.attr("id") + "-container").parents(".form-group");
                    //error.insertAfter(element);
                    element.append(error);
                }
            },
            // eslint-disable-next-line object-shorthand
            highlight: function highlight(element) {
                let errorClass = "is-invalid";
                $(element).addClass('is-invalid').removeClass('is-valid');
                Ladda.stopAll();
                var elem = $(element);
                if (elem.hasClass("select2-hidden-accessible")) {
                    // $("#select2-" + elem.attr("id") + "-container").parent().addClass(errorClass).removeClass('is-valid');
                    $("#select2-" + elem.attr("id") + "-container").parents(".form-group").find("span.select2-selection--single").addClass(errorClass).removeClass('is-valid');
                }
                //input.removeAttr('readonly').removeAttr('disabled');
            },
            // eslint-disable-next-line object-shorthand
            unhighlight: function unhighlight(element) {
                let errorClass = "is-invalid";
                $(element).removeClass('is-invalid');
                var elem = $(element);
                if (elem.hasClass("select2-hidden-accessible")) {
                    // $("#select2-" + elem.attr("id") + "-container").parent().removeClass(errorClass).addClass('is-valid');
                    $("#select2-" + elem.attr("id") + "-container").parents(".form-group").find("span.select2-selection--single").removeClass(errorClass).addClass('is-valid');
                }
                //Ladda.stopAll();
            },
            submitHandler: function (form) {
            }
        });
    },
    EditAndAddTemplateInvoice: function () {
        $("#create-form").validate({
            ignore: 'input[type=hidden]',
            rules: {
                "Name": {
                    required: true,
                    minlength: 3
                },
                "Template": {
                    required: true,
                }
            },
            messages: {
                "Name": {
                    required: errrequired,
                    minlength: errminlength3
                }, "Template": {
                    required: errrequired,
                    minlength: errminlength3
                }
            },
            submitHandler: function (form) {
            }
        });
    },
    addOrEditMangerPatternEInvoice: function () {
        $("#create-form").validate({
            ignore: 'input[type=hidden]',
            rules: {
                "Pattern": {
                    required: true,
                    minlength: 3
                },
                "Serial": {
                    required: true,
                }
            },
            messages: {
                "Serial": {
                    required: errrequired,
                    minlength: errminlength3
                },
                "Serial": {
                    required: errrequired,
                    minlength: errminlength3
                }
            },
            submitHandler: function (form) {
            }
        });
    },
    EditOrUpdateProduct: function () {
        $("#create-formProduct").validate({
            ignore: 'input[disabled=disabled]',
            ignore: 'select[disabled=disabled]',
            rules: {
                Name: {
                    required: true,
                    minlength: 1
                },

                idCategory: {
                    required: true
                },
            },
            messages: {
                Name: {
                    required: errrequired,
                    minlength: errminlength5
                },

                idCategory: {
                    required: errrequired,
                },

                idPrice: {
                    required: errrequired,
                },
                _Price: {
                    minStrict: "Vui lòng nhập đơn giá",
                    required: errrequired,
                },
                Description: {
                    required: errrequired,
                }
            },
            errorElement: 'span',
            errorPlacement: function errorPlacement(error, element) {
                Ladda.stopAll();
                // input.removeAttr('readonly').removeAttr('disabled');
                error.addClass('invalid-feedback');

                if (element.prop('type') === 'checkbox') {
                    error.insertAfter(element.parent('label'));
                    Ladda.stopAll();
                    //input.removeAttr('readonly').removeAttr('disabled');
                } else {
                    var a = element.parent();
                    a = a.children().last();

                    error.insertAfter(a.last());
                    Ladda.stopAll();
                    //input.removeAttr('readonly').removeAttr('disabled');
                }
                var elem = $(element);
                if (elem.hasClass("select2-hidden-accessible")) {
                    // element = $("#select2-" + elem.attr("id") + "-container").parent();
                    element = $("#select2-" + elem.attr("id") + "-container").parents(".form-group");
                    //error.insertAfter(element);
                    element.append(error);
                }
            },
            // eslint-disable-next-line object-shorthand
            highlight: function highlight(element) {
                let errorClass = "is-invalid";
                $(element).addClass('is-invalid');
                Ladda.stopAll();
                var elem = $(element);
                if (elem.hasClass("select2-hidden-accessible")) {
                    // $("#select2-" + elem.attr("id") + "-container").parent().addClass(errorClass).removeClass('is-valid');
                    $("#select2-" + elem.attr("id") + "-container").parents(".form-group").find("span.select2-selection--single").addClass(errorClass);
                }
                //input.removeAttr('readonly').removeAttr('disabled');
            },
            // eslint-disable-next-line object-shorthand
            unhighlight: function unhighlight(element) {
                let errorClass = "is-invalid";
                $(element).removeClass('is-invalid');
                var elem = $(element);

                if (elem.hasClass("select2-hidden-accessible")) {
                    // $("#select2-" + elem.attr("id") + "-container").parent().removeClass(errorClass).addClass('is-valid');
                    $("#select2-" + elem.attr("id") + "-container").parents(".form-group").find("span.select2-selection--single").removeClass(errorClass);
                }
                //Ladda.stopAll();
            },
            submitHandler: function (form) {
                // loadingStart();
                //  form.submit();
            }
        });
        $('#create-formProduct .select2_cre').on("change", function (e) {
            $(this).valid()
        });
    },
    UpdateConfigSystem_settingsellBannerheader: function () {
        $("#setting-sellBannerheader").validate({
            ignore: 'input[disabled=disabled]',
            ignore: "input[type='color']",
            ignore: 'select[disabled=disabled]',
            rules: {
                UrlSell: {
                    required: true,
                    minlength: 5
                },
                TitleSell: {
                    required: true,
                    minlength: 5
                },
                ColorTitle: {
                    required: true
                },
                Color: {
                    required: true
                }
            },
            messages: {
                UrlSell: {
                    required: errrequired,
                    minlength: errminlength5
                },
                TitleSell: {
                    required: errrequired,
                    minlength: errminlength5
                },

                ColorTitle: {
                    required: errrequired,
                },
                Color: {
                    required: errrequired,
                }
            },
            errorElement: 'span',
            errorPlacement: function errorPlacement(error, element) {
                Ladda.stopAll();
                // input.removeAttr('readonly').removeAttr('disabled');
                error.addClass('invalid-feedback');

                if (element.prop('type') === 'checkbox') {
                    error.insertAfter(element.parent('label'));
                    Ladda.stopAll();
                    //input.removeAttr('readonly').removeAttr('disabled');
                } else {
                    var a = element.parent();
                    a = a.children().last();

                    error.insertAfter(a.last());
                    Ladda.stopAll();
                    //input.removeAttr('readonly').removeAttr('disabled');
                }
                var elem = $(element);
                if (elem.hasClass("select2-hidden-accessible")) {
                    // element = $("#select2-" + elem.attr("id") + "-container").parent();
                    element = $("#select2-" + elem.attr("id") + "-container").parents(".form-group");
                    //error.insertAfter(element);
                    element.append(error);
                }
            },
            // eslint-disable-next-line object-shorthand
            highlight: function highlight(element) {
                let errorClass = "is-invalid";
                $(element).addClass('is-invalid').removeClass('is-valid');
                Ladda.stopAll();
                var elem = $(element);
                if (elem.hasClass("select2-hidden-accessible")) {
                    // $("#select2-" + elem.attr("id") + "-container").parent().addClass(errorClass).removeClass('is-valid');
                    $("#select2-" + elem.attr("id") + "-container").parents(".form-group").find("span.select2-selection--single").addClass(errorClass).removeClass('is-valid');
                }
                //input.removeAttr('readonly').removeAttr('disabled');
            },
            // eslint-disable-next-line object-shorthand
            unhighlight: function unhighlight(element) {
                let errorClass = "is-invalid";
                $(element).addClass('is-valid').removeClass('is-invalid');
                var elem = $(element);

                if (elem.hasClass("select2-hidden-accessible")) {
                    // $("#select2-" + elem.attr("id") + "-container").parent().removeClass(errorClass).addClass('is-valid');
                    $("#select2-" + elem.attr("id") + "-container").parents(".form-group").find("span.select2-selection--single").removeClass(errorClass).addClass('is-valid');
                }
                //Ladda.stopAll();
            },
            submitHandler: function (form) {
                // loadingStart();
                //  form.submit();
            }
        });
    }
}
$(function () {
    var url = window.location;
    /*  $('#sidebar-menu a[href="' + url + '"]').parent('li').addClass('current-page');*/
    $('#menuAdmin a').filter(function () {
        return this.href == url;
    }).addClass("active").parents('li.nav-item').addClass('menu-open').children('a').addClass('active');
});
Ladda.bind('.ladda-button', {
    callback: function callback(instance) {
        var input = $('input#email_send,input#password,input#confirm_password,input#email_to,input#email_cc,input#email_bcc,input#email_name');
        input.attr('readonly', 'readonly');
        // var progress = 0;
    },
    timeout: 200000,
});

function loadLadda() {
    Ladda.bind('.ladda-button', {
        callback: function callback(instance) {
            var input = $('input#email_send,input#password,input#confirm_password,input#email_to,input#email_cc,input#email_bcc,input#email_name');
            input.attr('readonly', 'readonly');
            // var progress = 0;
        },
        timeout: 200000,
    });
}
function loadEventIcheck() {
    $('input.icheck').iCheck({
        checkboxClass: 'icheckbox_square-green',
        radioClass: 'iradio_square-green',
        increaseArea: '20%' // optional
    });
}
function loadevent() {
    loadEventIcheck();
    $(".select2").select2({
        placeholder: "Chọn giá trị",
        allowClear: true,
        language: {
            noResults: function () {
                return "Không tìm thấy dữ liệu";
            }
        },
    });
}

function getImgURL(url, callback) {
    var xhr = new XMLHttpRequest();
    xhr.onload = function () {
        callback(xhr.response);
    };
    xhr.open('GET', url);
    xhr.responseType = 'blob';
    xhr.send();
}

function loadURLToInputField(url, filename, mime) {
    getImgURL(url, (imgBlob) => {
        // Load img blob to input
        let fileName = filename != "" ? filename : 'hasFilename.jpg' // should .replace(/[/\\?%*:|"<>]/g, '-') for remove special char like / \
        let file = new File([imgBlob], fileName, {
            type: (mime != "" ? mime : "image/jpeg"),
            lastModified: new Date().getTime()
        }, 'utf-8');
        let container = new DataTransfer();
        container.items.add(file);
        document.querySelector('#elfinderfile').files = container.files;
    })
}
$(document).ready(function () {
    $('[data-toggle="tooltip"]').tooltip();
    /*  $.fn.modal.Constructor.prototype.enforceFocus = function () { };*/
    $('input.icheck').iCheck({
        checkboxClass: 'icheckbox_square-green',
        radioClass: 'iradio_square-green',
        increaseArea: '20%' // optional
    });
    $(document).on("click", '#ipxChangepass', function (event) {
        if ($(this).is(':checked')) {
            $("#changepass").removeClass("d-none");
            $("#changepass").find("input").attr("type", "password").show();
        } else {
            $("#changepass").addClass("d-none");
            $("#changepass").find("input").attr("type", "hidden").hide();
        }
    });
    $(document).on("click", '#btnsaveUser', function (event) {
        validateFormUser();
        if ($("#FormValidate").valid()) {
            $(this).parents("form").submit();
        }
    });

    $('.form-image').click(function () {
        $('#customFile').trigger('click');
    });

    setTimeout(function () {
        $('body').addClass('loaded');
    }, 200);

    jQueryModalGet = (url, title, check = true, size = "modal-lg") => {

        try {
            $.ajax({
                type: 'GET',
                url: url,
                contentType: false,
                processData: false,
                success: function (res) {
                    if (check) {
                        if (res.isValid) {
                            if (res.modelFooter) {
                                jQueryModal.Show(title, res.html, size);
                            } else {
                                jQueryModal.ShowNotFooter(title, res.html, size);
                            }
                            if (res.loadDataTable) {
                                LoadDataTable();
                            }
                        } else {
                            // alert lỗi khi false
                        }
                    } else {
                        if (res.isValid) {
                            $('#form-modal .modal-body').html(res.html);
                            $('#form-modal .modal-title').html(title);
                            $('#form-modal .modal-dialog').addClass(size);
                            if (res.modelFooter) {
                                $('#form-modal .modal-footer').removeClass("d-none");
                            }
                            $('#form-modal').modal('show');
                        } else {
                            $('#form-modal .modal-body').html(res);
                            $('#form-modal .modal-title').html(title);
                            $('#form-modal').modal('show');

                        }

                    }
                    //console.log(res);
                },
                error: function (err) {
                    console.log(err)
                }
            });
            //to prevent default form submit event
            return false;
        } catch (ex) {
            console.log(ex)
        }
    }
    jQueryModalGetRightToLeft = (url, title, size = "modal-lg") => {

        try {
            $.ajax({
                type: 'GET',
                url: url,
                contentType: false,
                processData: false,
                success: function (res) {
                    if (res.isValid) {
                        $('#form-modal').addClass('right');
                        $('#form-modal .modal-dialog').removeClass('modal-dialog-centered');
                        $('#form-modal .modal-body').html(res.html);
                        $('#form-modal .modal-title').html(title);
                        $('#form-modal .modal-dialog').addClass(size);
                        if (res.modelFooter) {
                            $('#form-modal .modal-footer').removeClass("d-none");
                        }


                        $('#form-modal').modal('show');
                    } else {
                        $('#form-modal').addClass('right');
                        $('#form-modal .modal-dialog').removeClass('modal-dialog-centered');
                        $('#form-modal .modal-body').html(res);
                        $('#form-modal .modal-title').html(title);
                        $('#form-modal').modal('show');
                    }
                    //console.log(res);
                },
                error: function (err) {
                    console.log(err)
                }
            });
            //to prevent default form submit event
            return false;
        } catch (ex) {
            console.log(ex)
        }
    }
    jQueryPost = (url) => {
        try {
            $.ajax({
                type: 'POST',
                url: url,
                contentType: false,
                processData: false,
                success: function (res) {
                    //console.log(res);
                },
                error: function (err) {
                    console.log(err)
                }
            })
            //to prevent default form submit event
            return false;
        } catch (ex) {
            console.log(ex)
        }
    }
    jQueryModalPost = form => {

        try {
            eventUnFormatTextnumber3();
            if (!$(form).valid()) {
                Ladda.stopAll();
                return false;
            }
            for (instance in CKEDITOR.instances) {
                CKEDITOR.instances[instance].updateElement();
                $('#' + instance).val(CKEDITOR.instances[instance].getData());
            }
            //$(document).find('textarea').each(function () {
            //    let idtex = $(this).attr('id');
            //    $('#' + idtex).val($(this).tinyMCE().getContent());
            //    // tinymce.execCommand('mceRemoveEditor', false, $(this).attr('id'));
            //});

            $.ajax({
                type: 'POST',
                url: form.action,
                data: new FormData(form),
                contentType: false,
                processData: false,
                success: function (res) {

                    if (res.isValid) {
                        if (res.loadTable) {
                            dataTableOut.ajax.reload(null, false);
                        } else {
                            $('#viewAll').html(res.html);
                            if (res.loadDataTable) {
                                LoadDataTable();
                            }
                        }
                        if (res.closeSwal) {
                            Swal.close();
                        }
                        $('#form-modal').modal('hide');
                        $('#myModalShowMess').modal('hide');
                        $('#myModalShowMess').unbind();
                        $('#myModalShowMess').remove();
                    } else {
                        Ladda.stopAll();
                    }
                    //loadevent();
                },
                error: function (err) {
                    console.log(err)
                }
            })
            return false;
        } catch (ex) {
            console.log(ex)
        }
    }

    jQueryModalDelete = form => {
        confirms("Xác nhận", "Bạn có chắc chắn muốn xóa không?", "<i class='far fa-times-circle'></i> Hủy", "<i class='fas fa-check-circle'></i> Đồng ý", function () {

            try {
                $.ajax({
                    type: 'POST',
                    url: form.action,
                    data: new FormData(form),
                    contentType: false,
                    processData: false,
                    success: function (res) {
                        if (res.isValid) {
                            if (res.loadTable) {
                                dataTableOut.ajax.reload(null, false);
                            } else if (res.loadTreeview) {
                                loadData()
                            } else {
                                $('#viewAll').html(res.html);
                            }
                        }
                    },
                    error: function (err) {
                        console.log(err)
                    }
                })
            } catch (ex) {
                console.log(ex)
            }
        });

        //prevent default form submit event
        return false;
    }

});



function AddProduct(sel) {

    var form = sel;
    if (!$("#create-formProduct").valid()) {
        Ladda.stopAll();
        return false;
    } else {
        if ($("#elfinder-input").val() == "" && parseInt($("#Id").val()) == 0) {
            $("#tab-img").click();
            toastr.error("Bạn chưa chọn hình ảnh đại diện");
            Ladda.stopAll();
            return false;
        } else {
            try {
                for (instance in CKEDITOR.instances) {
                    CKEDITOR.instances[instance].updateElement();
                    $('#' + instance).val(CKEDITOR.instances[instance].getData());
                }
                var isvalidtable = false;
                var IsAddingOptions = $("#IsAddingOptions").prop("checked");
                if ($("#tableStylePrice").length == 1 && IsAddingOptions) {
                    var lstArr = []; //list table style có đơn giá
                    var lstStyleArr = [];
                    $("#tableStylePrice").find("tbody").find("tr").each(function (index, element) {
                        let className = $(this).attr("class").replaceAll("even", "").replaceAll("odd", "");
                        let idOptionsname = $(this).attr("IdOptionsName");
                        let idData = $(this).data("id") || 0;
                        let name = $(this).find(".name").text();
                        let code = $(this).find(".code").val();
                        let price = $(this).find(".price").val().replaceAll(".", "");
                        let quantity = $(this).find(".quantity").val().replaceAll(".", "");
                        if (price == "" || price == 0) {
                            let htmlerr = "";
                            toastr.error("Đơn giá vui lòng không được bỏ trống");
                            $(this).find(".price").focus();
                            $(this).find(".price").addClass("error");
                            $(this).find(".price").after();
                            $(this).find(".price").keyup(function () {
                                if (this.length == 0 || this.value == 0) {
                                    $(this).find(".price").addClass("error");
                                } else {
                                    $(this).find(".price").removeClass("error");
                                }
                            });
                            Ladda.stopAll();
                            isvalidtable = true;
                            return false;
                        }
                        var itemDatatable = {}; //khởi tạo object
                        itemDatatable.IdOptionsName = idOptionsname;
                        itemDatatable.Id = idData;
                        itemDatatable.Name = name;
                        itemDatatable.SKU = code;
                        itemDatatable.Price = parseFloat(price);
                        itemDatatable.Quantity = quantity != "" ? parseInt(quantity) : 0;
                        itemDatatable.ClassName = className;

                        lstArr.push(itemDatatable); //thêm gán vào array
                    });

                    $("#items-input-addcontent").find(".input-addcontent").each(function (index, element) {
                        var _dlstStyleArr = {};
                        styleproduct1 = $(this).find("select").val();
                        _dlstStyleArr.IdStyleOptionsProduct = styleproduct1;
                        _dlstStyleArr.Sort = index;
                        //console.log("valuestyle:" + valuestyle);
                        var _lstStyleinput = [];
                        $(this).find("input").each(function (indexitem, element) {
                            var _style = {};
                            if (this.value.trim() != "") {
                                _id = $(this).data("id") || 0;
                                _style.Id = _id;
                                _style.Name = this.value;
                                _style.Sort = indexitem;
                                _lstStyleinput.push(_style);
                            }
                        });

                        _dlstStyleArr.OptionsNames = _lstStyleinput;
                        lstStyleArr.push(_dlstStyleArr);
                    });

                }
                if (isvalidtable) {
                    return false;
                } else {
                    let json = JSON.stringify(lstArr);
                    let jsonlstStyleArr = JSON.stringify(lstStyleArr);
                    $("#JsonTableByStylePro").val(json);
                    $("#JsonListStylePro").val(jsonlstStyleArr);
                }


                $.ajax({
                    type: 'POST',
                    url: form.action,
                    data: new FormData(form),
                    contentType: false,
                    processData: false,
                    success: function (res) {
                        if (res.isValid) {
                            dataTableOut.ajax.reload(null, false);

                        } else {
                            Ladda.stopAll();
                        }
                        // loadevent();
                    },
                    error: function (err) {
                        console.log(err)
                    }
                })
                return false;
            } catch (ex) {
                console.log(ex)
            }
        }
    }
}

function AddPost(sel) {
    var form = sel;
    $("#create-formPost").validate({
        //onfocusout: false,
        // onkeyup: false,
        // onclick: false,
        rules: {
            Name: {
                required: true,
                minlength: 5
            },
            idCategory: {
                required: true
            }
        },
        messages: {
            Name: {
                required: errrequired,
                minlength: errminlength5
            },
            idCategory: {
                required: errrequired,
            }
        },
        errorElement: 'span',
        errorPlacement: function errorPlacement(error, element) {
            Ladda.stopAll();
            // input.removeAttr('readonly').removeAttr('disabled');
            error.addClass('invalid-feedback');

            if (element.prop('type') === 'checkbox') {
                error.insertAfter(element.parent('label'));
                Ladda.stopAll();
                //input.removeAttr('readonly').removeAttr('disabled');
            } else {
                var a = element.parent();
                a = a.children().last();

                error.insertAfter(a.last());
                Ladda.stopAll();
                //input.removeAttr('readonly').removeAttr('disabled');
            }
            var elem = $(element);
            if (elem.hasClass("select2-hidden-accessible")) {
                // element = $("#select2-" + elem.attr("id") + "-container").parent();
                element = $("#select2-" + elem.attr("id") + "-container").parents(".form-group");
                //error.insertAfter(element);
                element.append(error);
            }
        },
        // eslint-disable-next-line object-shorthand
        highlight: function highlight(element) {
            let errorClass = "is-invalid";
            $(element).addClass('is-invalid').removeClass('is-valid');
            Ladda.stopAll();
            var elem = $(element);
            if (elem.hasClass("select2-hidden-accessible")) {
                // $("#select2-" + elem.attr("id") + "-container").parent().addClass(errorClass).removeClass('is-valid');
                $("#select2-" + elem.attr("id") + "-container").parents(".form-group").find("span.select2-selection--single").addClass(errorClass).removeClass('is-valid');
            }
            //input.removeAttr('readonly').removeAttr('disabled');
        },
        // eslint-disable-next-line object-shorthand
        unhighlight: function unhighlight(element) {
            let errorClass = "is-invalid";
            $(element).addClass('is-valid').removeClass('is-invalid');
            var elem = $(element);

            if (elem.hasClass("select2-hidden-accessible")) {
                // $("#select2-" + elem.attr("id") + "-container").parent().removeClass(errorClass).addClass('is-valid');
                $("#select2-" + elem.attr("id") + "-container").parents(".form-group").find("span.select2-selection--single").removeClass(errorClass).addClass('is-valid');
            }
            //Ladda.stopAll();
        },
        submitHandler: function (form) {
            // loadingStart();
            //  form.submit();
        }
    });
    $('#create-formPost .select2').on("change", function (e) {
        $(this).valid()
    });
    if (!$("#create-formPost").valid()) {
        Ladda.stopAll();
        return false;
    } else {
        if ($("#customFile").val() == "" && parseInt($("#Id").val()) == 0) {
            toastr.error("Bạn chưa chọn hình ảnh đại diện");
            Ladda.stopAll();
            return false;
        }
        try {
            for (instance in CKEDITOR.instances) {
                CKEDITOR.instances[instance].updateElement();
                $('#' + instance).val(CKEDITOR.instances[instance].getData());
            }

            $.ajax({
                type: 'POST',
                url: form.action,
                data: new FormData(form),
                contentType: false,
                processData: false,
                success: function (res) {

                    if (res.isValid) {
                        if (res.loadTable) {
                            dataTableOut.ajax.reload(null, false);
                        } else {
                            $('#viewAll').html(res.html);
                            if (res.loadDataTable) {
                                LoadDataTable();
                            }
                        }
                        $('#form-modal').modal('hide');
                        $('#myModalShowMess').modal('hide');
                        $('#myModalShowMess').unbind();
                        $('#myModalShowMess').remove();
                    } else {
                        Ladda.stopAll();
                    }
                    loadevent();
                },
                error: function (err) {
                    console.log(err)
                }
            })
            return false;
        } catch (ex) {
            console.log(ex)
        }
    }
}

$(function () {
    var url = window.location;
    $('.app-sidebar__inner a').filter(function () {
        return this.href == url;
    }).addClass('mm-active').parents('ul.mm-collapse').addClass("mm-show").parent("li").addClass('mm-active');
});
$('#form-modal').on('hidden.bs.modal', function (e) {
    $("div.modal-body").html("");
    $("div.modal-body").empty();
    //$(".select2").select2({
    //    placeholder: "Chọn giá trị",
    //    allowClear: true,
    //    language: {
    //        noResults: function () {
    //            return "Không tìm thấy dữ liệu";
    //        }
    //    },
    //});
});

function confirmcustom(sel, title) {
    var th = sel;
    confirms("Xác nhận", title, "<i class='far fa-times-circle'></i> Hủy", "<i class='fas fa-check-circle'></i> Đồng ý", function () {
        $(th).closest("form").submit();
        // $(th).parents.submit();
    });

    //prevent default form submit event
    return false;
}

function priceFormat() {
    $('.priceFormat').priceFormat({
        prefix: '',
        centsLimit: 0,
        centsSeparator: '.',
        thousandsSeparator: ','
    });
}
function numberFormat() {
    $('.numberformat').number(true, 3, '.', ',');
}
var errRegex = 'Vui lòng nhập đúng định dạng';
var errrequired = 'Dữ liệu không được để trống';
var errminlength5 = 'Trường dữ liệu ít nhất 5 ký tự';
var errminlength3 = 'Trường dữ liệu ít nhất 3 ký tự';
var errnumber = 'Trường dữ liệu định dạng phải là số';

//$("#create-formCompany").validate({
//    rules: {
//        Name: {
//            required: true,
//        },
//        CusTaxCode: {
//            required: true,
//        }, Address: {
//            required: true,
//        },
//        Email: {
//            required: true,
//            regex: /^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$/g,
//        }

//    },
//    messages: {
//        Name: {
//            required: errrequired,
//        },
//        CusTaxCode: {
//            required: errrequired,
//        },
//        Address: {
//            required: errrequired,
//        },
//        Email: {
//            regex: errRegex,
//        }

//    },
//    errorElement: 'em',
//    errorPlacement: function errorPlacement(error, element) {
//        Ladda.stopAll();
//        input.removeAttr('readonly').removeAttr('disabled');
//        error.addClass('invalid-feedback');

//        if (element.prop('type') === 'checkbox') {
//            error.insertAfter(element.parent('label'));
//            Ladda.stopAll();
//            input.removeAttr('readonly').removeAttr('disabled');
//        }
//        else {
//            var a = element.parent();
//            a = a.children().last();

//            error.insertAfter(a.last());
//            Ladda.stopAll();
//            input.removeAttr('readonly').removeAttr('disabled');
//        }
//    },
//    // eslint-disable-next-line object-shorthand
//    highlight: function highlight(element) {
//        $(element).addClass('is-invalid').removeClass('is-valid');
//        Ladda.stopAll();
//        input.removeAttr('readonly').removeAttr('disabled');
//    },
//    // eslint-disable-next-line object-shorthand
//    unhighlight: function unhighlight(element) {
//        $(element).addClass('is-valid').removeClass('is-invalid');
//        //Ladda.stopAll();
//    }
//});

function getObjectKey(obj, value) {
    return Object.keys(obj).find(key => obj[key] === value);
}

function pushState(url) {
    window.history.pushState({
        turbolinks: true,
        url: url
    }, null, url)
}
///////////////////admin
var eventConfigSaleParameters = {
    eventUpdatecheckbox: function () {
        //duyệt update
        $(".ConfigSaleParameter input").map(function (index, ele) {

            let key = $(this).data("key");
            let parent = $(this).data("parent");
            let type = $(this).data("type");
            let typevalue = $(this).data("typevalue");
            $(this).removeAttr("data-key");
            $(this).removeAttr("data-parent");
            $(this).removeAttr("data-type");
            $(this).removeAttr("data-typevalue");
            $(this).data("key", key)
            $(this).data("parent", parent)
            $(this).data("type", type)
            $(this).data("typevalue", typevalue)
        })
        $(".ConfigSaleParameter input").click(function () {
            var ConfigSaleParametersItems = [];
            let ConfigSaleParametersItem = {};
            let key = $(this).data("key");
            let parent = $(this).data("parent");
            let type = $(this).data("type");
            let typevalue = $(this).data("typevalue");
            let getValueKey = EnumConfigParameters[key];

            if (!$(this).prop('checked')) {
                //tắt

                switch (getValueKey) {
                    case EnumConfigParameters.DELETEINVOICENOPAYMENT:// Cho phép xóa đơn khi chưa xuất VAT

                        ConfigSaleParametersItem.Key = getObjectKey(EnumConfigParameters, EnumConfigParameters.ACCEPTPAYMENTDELETEINVOICE);
                        ConfigSaleParametersItem.Value = false;
                        ConfigSaleParametersItem.Parent = getObjectKey(EnumConfigParameters, EnumConfigParameters.DELETEINVOICENOPAYMENT);
                        ConfigSaleParametersItem.Type = type;
                        ConfigSaleParametersItem.TypeValue = "BOOL";//vì k chắc là cái con nó như cha nên truyền cứng vào nhé
                        ConfigSaleParametersItems.push(ConfigSaleParametersItem);
                        ConfigSaleParametersItem = {}
                        //eventConfigSaleParameters.updatedatachange($(this), ConfigSaleParametersItems);
                        break;
                    case EnumConfigParameters.PUBLISHMERGEINVOICE:// cho phép gộp hóa đơn

                        ConfigSaleParametersItem.Key = getObjectKey(EnumConfigParameters, EnumConfigParameters.DELETEIPUBLISHMERGEINVOICEAFTER);
                        ConfigSaleParametersItem.Value = false;
                        ConfigSaleParametersItem.Parent = getObjectKey(EnumConfigParameters, EnumConfigParameters.PUBLISHMERGEINVOICE);
                        ConfigSaleParametersItem.Type = type;
                        ConfigSaleParametersItem.TypeValue = "BOOL";//vì k chắc là cái con nó như cha nên truyền cứng vào nhé
                        ConfigSaleParametersItems.push(ConfigSaleParametersItem);
                        ConfigSaleParametersItem = {}
                        //eventConfigSaleParameters.updatedatachange($(this), ConfigSaleParametersItems);
                        break;
                    default:
                    // code block
                }
                ConfigSaleParametersItem.Key = key;
                ConfigSaleParametersItem.TypeValue = typevalue;
                ConfigSaleParametersItem.Value = false;
                ConfigSaleParametersItem.Type = type;
                ConfigSaleParametersItem.Parent = parent;
                ConfigSaleParametersItems.push(ConfigSaleParametersItem);

                eventConfigSaleParameters.updatedatachange($(this), ConfigSaleParametersItems);
            } else {
                //bật
                ConfigSaleParametersItem.Key = key;
                ConfigSaleParametersItem.TypeValue = typevalue;
                ConfigSaleParametersItem.Type = type;
                ConfigSaleParametersItem.Value = true;
                ConfigSaleParametersItem.Parent = parent;
                ConfigSaleParametersItems.push(ConfigSaleParametersItem);

                eventConfigSaleParameters.updatedatachange($(this), ConfigSaleParametersItems);
            }
        })
    },
    updatedatachange: function (sel, ConfigSaleParametersItems) {

        $.ajax({
            type: 'POST',
            url: "/Selling/ConfigSaleParameters/Update",
            async: true,

            data: {
                ConfigSaleParametersItems: ConfigSaleParametersItems
            },
            success: function (res) {
                if (res.isValid) {
                    if (!$(sel).prop('checked')) {
                        if ($(sel).parents(".item-col").next().hasClass("childrenconfig")) {
                            $(sel).parents(".item-col").next().addClass("d-none");
                        }
                    } else {
                        if ($(sel).parents(".item-col").next().hasClass("childrenconfig")) {
                            $(sel).parents(".item-col").next().removeClass("d-none");
                        }

                    }

                } else {

                    ConfigSaleParametersItems.forEach(function (item, index) {
                        if (!item.value) {
                            $('input[data-key=' + item.Key + ']').prop('checked', false)
                        } else {
                            $('input[data-key=' + item.Key + ']').prop('checked', false)
                        }
                    });
                }
            },
            error: function (err) {
                console.log(err)
            }
        })
    }
}
var Product = {
    eventbtnAddProduct: function () {
        $(".lst-action-addproduct a:nth-child(1)").click(function () {
            Product.Addproduct('/Selling/Product/CreateOrEdit?TypeProductCategory=' + EnumTypeProductCategory.PRODUCT, EnumTypeProductCategory.PRODUCT);
        })
        $(".lst-action-addproduct a:nth-child(2)").click(function () {
            Product.Addproduct('/Selling/Product/CreateOrEdit?TypeProductCategory=' + EnumTypeProductCategory.SERVICE, EnumTypeProductCategory.SERVICE);
        })
        $(".lst-action-addproduct a:nth-child(3)").click(function () {
            Product.Addproduct('/Selling/Product/CreateOrEdit?TypeProductCategory=' + EnumTypeProductCategory.COOKING, EnumTypeProductCategory.COOKING);
        })
        $(".lst-action-addproduct a:nth-child(4)").click(function () {
            Product.Addproduct('/Selling/Product/CreateOrEdit?TypeProductCategory=' + EnumTypeProductCategory.COMBO, EnumTypeProductCategory.COMBO);
        })
    },
    Addproduct: function (url, type) {

        $.ajax({
            type: 'GET',
            //global: false,
            url: url,
            // data: fomrdata,
            contentType: false,
            processData: false,
            success: function (res) {
                if (res.isValid) {
                    Swal.fire({
                        // icon: 'success',
                        title: res.title,
                        html: res.html,
                        position: 'top-end',
                        showClass: {
                            popup: `
                              popup-formcreate
                              popup-lg
                               animate__animated
                              animate__fadeInRight
                              animate__faster
                            `
                        },
                        hideClass: {
                            popup: "popup-formcreate popup-lg animate__animated animate__fadeOutRight animate__faster"
                        },
                        showCloseButton: true,
                        footer: "<button class='btn btn-primary btn-continue mr-3'><i class='icon-cd icon-add_cart icon'></i>Hủy bỏ</button><button class='btn btn-save btn-success'><i class='icon-cd icon-doneAll icon'></i>Lưu</button>",
                        allowOutsideClick: false,
                        showConfirmButton: false,
                        showCancelButton: false,
                        confirmButtonText: 'Lưu',
                        cancelButtonText: '<i class="fa fa-window-close"></i> Đóng',

                        didRender: () => {
                            if (res.idUnit == null) {
                                res.idUnit = 0;
                            }
                            evetnFormatnumber3();
                            evetnFormatTextnumber3decimal();
                            CKEDITOR.replace('Description');
                            $('[data-toggle="tooltip"]').tooltip()
                            validateForm.EditOrUpdateProduct();
                            loaddataSelect2CustomsTempalte("/Api/Handling/GetAllCategoryProduct?IsPos=true", "#idCategory", res.idcategory, "Chọn thực đơn");
                            loaddataSelect2Tempalte("/Api/Handling/GetAllUnit", "#IdUnit", res.idUnit, "Chọn đơn vị tính");
                            loaddaterangepicker();
                            loadEventIcheck();
                            loadeventAddSell();
                            Product.loadeEventCheckIsInventory();
                            //load tìm sản phẩm nếu là combo
                            if (res.typeProductCategory == EnumTypeProductCategory.COMBO) {
                                Product.autocompleteproduct(res.typeProductCategory);
                                Product.loadEvnetFormatTable();
                                Product.loadeventUpdateAmount();
                                Product.loadEventChangeQuantity();
                                Product.loadEventRemoveProduct();
                                Product.loadEventActionQuantity();

                            }
                            else if (res.typeProductCategory == EnumTypeProductCategory.COOKING) {
                                Product.autocompleteproduct(res.typeProductCategory);
                                Product.loadEvnetFormatTable();
                                Product.loadEventRemoveProduct();
                            }


                            //end
                            $(".btn-continue").click(function () {
                                Swal.close();
                            });

                            $(".btn-save").click(function () {
                                if (res.typeProductCategory == EnumTypeProductCategory.COMBO) {
                                    if ($(".table-combo tbody tr:not(.nodata)").length > 0) {
                                        Product.GetDataCombo();
                                    }
                                    else {
                                        toastrcus.error("Chưa có thành phần combo được chọn");
                                        return;
                                    }
                                }
                                if ($("form#create-formProduct").valid()) {

                                    jQueryModalPost($("form#create-formProduct")[0]);

                                }
                            });
                        }
                    }).then((result) => {
                        if (result.isConfirmed) {
                            $("#create-formProduct").submit();
                            return false;
                        }
                    });
                }
            },
            error: function (err) {
                console.log(err)
            }
        });
    },
    GetDataCombo: function () {
        let products = [];
        let checkok = true;
        $(".table-combo tbody tr").map(function () {
            let product = {};
            product.Id = parseInt($(this).find(".code").data("id")) || 0;
            product.IdPro = parseInt($(this).find(".code").data("idpro")) || 0;
            product.RetailPrice = parseInt($(this).find(".retailPrice").data("retailPrice")) || 0;
            product.Name = $(this).find(".name").data("name");
            if ($(this).find(".quantity").length > 0) {
                product.Quantity = parseFloat($(this).find(".quantity").val().replaceAll(",", "")) || 0;
            } else {
                product.Quantity = 0;
            }
            if (product.IdPro == 0) {
                toastrcus.error("Lỗi hàng hóa: " + product.Name + " đã bị xóa khỏi hệ thống, vui lòng tải lại trang để thử lại");
                checkok = false;
            }
            products.push(product);
        });
        if (!checkok) {
            toastrcus.error("Chưa có thành phần combo");
            // return;
        }
        if (products.length == 0) {
            toastrcus.error("Chưa chọn thành phần combo");
            // return;
        }
        let json = JSON.stringify(products);
        $("#JsonListComboProduct").val(json);
    },
    loadeEventCheckIsInventory: function () {
        $('.checkboxIsInventory').on('ifChecked', function (event) {
            $("#Quantity").attr("readonly", "readonly");
            $("#Quantity").parent(".form-group").addClass("d-none");
        });

        $('.checkboxIsInventory').on('ifUnchecked', function (event) {
            $("#Quantity").removeAttr("readonly", "readonly");
            $("#Quantity").parent(".form-group").removeClass("d-none");
        });
    },// evne check quản lý tồn kho hay k
    UpdatePriceProduct: function (sel) {

        let amount = $(sel).data("am");
        let iddata = $(sel).data("id");
        let price = parseFloat($(sel).val().replaceAll(",", "")) || 0;
        $.ajax({
            type: 'POST',
            url: "/Selling/Product/UpdatePrice",
            async: true,
            data: {
                id: iddata,
                price: price
            },
            success: function (res) {
                if (res.isValid) {
                    $(sel).data("am", price);
                } else {
                    $(sel).val(parseFloat(amount).format0VND(3, 3, ""));
                }
            },
            error: function (err) {
                console.log(err)
            }
        })
    },
    Delete: function (url, id, idProduct) {
        var xoa = false;
        try {
            $.ajax({
                type: 'POST',
                url: url,
                async: false,
                data: {
                    id: id,
                    idProduct: idProduct
                },
                success: function (res) {
                    if (res.isValid) {
                        xoa = true;
                    }
                },
                error: function (err) {
                    console.log(err)
                }
            })
        } catch (ex) {
            console.log(ex)
        }
        //prevent default form submit event
        return xoa;
    },
    autocompleteproduct: function (typeProductCategory) {
        $(".search-product").autocomplete(
            {
                appendTo: "#parentautocomplete",
                autoFocus: true,
                minLength: 1,
                delay: 0,
                source: function (request, response) {
                    $.ajax({
                        global: false,
                        url: "/Selling/Pos/SearchProductPos",
                        type: "GET",
                        dataType: "json",
                        data: {
                            text: request.term,
                            iSsell: false,
                            type: typeProductCategory
                        },
                        // html: true,
                        success: function (data) {
                            response($.map(data, function (item) {
                                let texthigh = __highlight(item.name, request.term);
                                let htmltonkho = "<span class='ton'>Tồn kho: " + parseFloat(item.quantity).format0VND(3, 3, '') + "<span>";
                                if (data.isInventory || data.typeProductCategory == EnumTypeProductCategory.COMBO || data.typeProductCategory == EnumTypeProductCategory.SERVICE) {
                                    htmltonkho = "<span class='ton'>Tồn kho: -- <span>";
                                }
                                let html =
                                    "<a href='javascript:void(0)'><div class='search-auto'>" +
                                    "<div class='img'><img src='../../../" + item.img + "'></div>" +
                                    "<div class='tk_name'><span>" + texthigh + " (" + item.code + ")</span>" +
                                    "<span class='price'>Giá: " + parseFloat(item.price).format0VND(3, 3, '') + "</span>" + htmltonkho + "</div></div></a>";
                                return {
                                    //label: html, value: item.code + " " + item.name, idProduct: item.id
                                    label: html, value: item.code, idProduct: item.id, price: item.price, retailPrice: item.retailPrice, quantity: item.quantity, name: item.name, typeProductCategory: item.typeProductCategory
                                };
                            }))
                            return { label: request.term, value: request.term };
                        },
                    })
                },
                html: true,
                select: function (e, ui) {
                    $(this).val(ui.item.value);
                    $(this).select();
                    if (ui.item.typeProductCategory == EnumTypeProductCategory.COMBO) {
                        Product.loadeventselectTable(ui.item.idProduct, ui.item.name, ui.item.price, ui.item.value, ui.item.retailPrice, ui.item.typeProductCategory);//retailPrice là  giá nhập vào giá vốn nhé
                    } else if (ui.item.typeProductCategory == EnumTypeProductCategory.SERVICE) {//dịch vụ có món thêm
                        Product.loadeventselectTable(ui.item.idProduct, ui.item.name, ui.item.price, ui.item.value, ui.item.retailPrice, ui.item.typeProductCategory);//retailPrice là  giá nhập vào giá vốn nhé
                    } else {
                        toastrcus("Dịch vụ chưa hỗ trợ chọn thành phần");
                    }

                },
                response: function () {
                    // $(this).select()
                }
            }).focus(function () {
                $(this).autocomplete("search");
                $(this).select();
            });
        $(".search-product").keypress(async function (event) {
            let _dt = $(this).val();
            if (event.keyCode == 13 && _dt.length > 0) {
                let check = await Product.loadEventEnterInputAddPro(_dt);
                $(this).select();
                if (!check) {
                    toastrcus.error("Hàng hóa không hợp lệ!");
                    return;
                }

            }
        });
    },

    loadEventEnterInputAddPro: async function (code) {
        let isValid = false;
        $(".table-combo tbody tr").map(function () {
            let _code = $(this).find(".code").text();
            if (_code.trim() == code) {
                isValid = true;
                $(this).find(".add-quantity").trigger("click");
            }
        });
        return isValid;
    },
    loadeventselectTable: async function (id, name, price, code, retailPrice, typeProductCategory) {//chọn select text khi sổ xuống lúc autocomplete
        let check = await Product.loadEventEnterInputAddPro(code);
        if (!check) {
            $(".table-combo tbody tr.nodata").remove();
            let dem = $(".table-combo tbody tr").length + 1;
            let html = "";
            if (typeProductCategory == EnumTypeProductCategory.COMBO) {
                html = `<tr>
                          <td><i class="fas fa-trash fa-remove"></i></td>
                          <td>`+ dem + `</td>
                          <td class="code" data-id="0" data-idpro="`+ id + `">` + code + `</td>
                          <td class="name" data-name="`+ name + `">` + name + `</td>
                          <td style="width: 175px;">
                            <div class="input-group">
                              <div class="input-group-prepend">
                                <span class="input-group-text minus-quantity" id="basic-addon1"><i class="fas fa-minus"></i></span>
                              </div>
                              <input type="text" class="form-control number3 text-center quantity" data-quantity="` + 1 + `" placeholder="Nhập số lượng" value="` + 1 + `">
                              <div class="input-group-prepend">
                                <span class="input-group-text add-quantity" id="basic-addon1"><i class="fas fa-plus"></i></span>
                              </div>
                            </div>
                          </td>
                           <td class=""><input type="text" readonly disabled class="text-right form-control number3 retailPrice" data-retailPrice="`+ parseFloat(retailPrice) + `" value="` + parseFloat(retailPrice) + `"/></td>
                          <td class=""><input type="text" readonly disabled class="text-right form-control number3 price" data-price="`+ parseFloat(price) + `" value="` + parseFloat(price) + `"/></td>
                          <td class=""><input type="text" readonly disabled class="text-right number3 form-control amount" data-amount="`+ parseFloat(price) + `"  value="` + parseFloat(retailPrice) + `"/></td>
                      </tr>`;
            } else if (typeProductCategory == EnumTypeProductCategory.SERVICE) {
                html = `<tr>
                          <td><i class="fas fa-trash fa-remove"></i></td>
                          <td>`+ dem + `</td>
                          <td class="code" data-id="0" data-idpro="`+ id + `">` + code + `</td>
                          <td class="name" data-name="`+ name + `">` + name + `</td>
                          <td class=""><input type="text" readonly disabled class="text-right form-control number3 retailPrice" data-retailPrice="`+ parseFloat(retailPrice) + `" value="` + parseFloat(retailPrice) + `"/></td>
                          <td class=""><input type="text" readonly disabled class="text-right form-control number3 price" data-price="`+ parseFloat(price) + `" value="` + parseFloat(price) + `"/></td>
                      </tr>`;
            }


            if (dem == 1) {
                $(".table-combo tbody").html(html);
            } else {
                $(".table-combo tbody tr:last").after(html);
            }
            Product.loadEvnetFormatTable();
            //thêm sự kiện xóa dòng
            Product.loadEventRemoveProduct();
            //end
            if (typeProductCategory == EnumTypeProductCategory.SERVICE) {

            }
            else if (typeProductCategory == EnumTypeProductCategory.COMBO) {
                Product.loadeventUpdateAmount();
                Product.loadEventChangeQuantity();
                Product.loadEventActionQuantity();
            }

        }
    },
    loadEventActionQuantity: function () {
        $(".table-combo tbody tr .minus-quantity").unbind();
        $(".table-combo tbody tr .minus-quantity").click(function () {
            Product.loadhighlightrow($(this).parents("tr"));
            let data_quantity = $(this).parents("tr").find(".quantity").data("quantity");
            let quantity = parseFloat($(this).parents("tr").find(".quantity").val().replaceAll(",", "")) || 0;
            quantity = quantity - 1;
            if (quantity > 0) {
                let price = parseFloat($(this).parents("tr").find(".price").val().replaceAll(",", "")) || 0;
                let retailPrice = parseFloat($(this).parents("tr").find(".retailPrice").val().replaceAll(",", "")) || 0;

                let amountgiaban = (quantity * price);
                let amount = (quantity * retailPrice);//lấy giá vón để tính
                $(this).parents("tr").find(".amount").val(amount.format0VND(3, 3, ""));
                $(this).parents("tr").find(".amount").data("amount", amountgiaban);//gán tiền giá bán
                $(this).parents("tr").find(".quantity").val(quantity.format0VND(3, 3, ""));
                $(this).data("quantity", quantity);
            } else {
                toastrcus.error("Vui lòng nhập số lượng lớn hơn 0");
                $(this).val(data_quantity);
                return;
            }
            Product.loadeventUpdateAmount();
        });
        $(".table-combo tbody tr .add-quantity").unbind();
        $(".table-combo tbody tr .add-quantity").click(function () {
            Product.loadhighlightrow($(this).parents("tr"));
            let data_quantity = $(this).parents("tr").find(".quantity").data("quantity");

            let quantity = parseFloat($(this).parents("tr").find(".quantity").val().replaceAll(",", "")) || 0;
            quantity = quantity + 1;



            if (quantity > 0) {
                let price = parseFloat($(this).parents("tr").find(".price").val().replaceAll(",", "")) || 0;
                let retailPrice = parseFloat($(this).parents("tr").find(".retailPrice").val().replaceAll(",", "")) || 0;
                let amount = (quantity * retailPrice);//giá vốn
                let amountgiaban = (quantity * price);
                $(this).parents("tr").find(".amount").val(amount.format0VND(3, 3, ""));
                $(this).parents("tr").find(".amount").data("amount", amountgiaban);
                $(this).parents("tr").find(".quantity").val(quantity.format0VND(3, 3, ""));
                $(this).parents("tr").find(".quantity").data("quantity", quantity);
            } else {
                toastrcus.error("Vui lòng nhập số lượng lớn hơn 0");
                $(this).val(data_quantity);
                return;
            }
            Product.loadeventUpdateAmount();
        });
    },//sự kiện thay đổi nút tăng giảm sl
    loadhighlightrow: function (sel) {
        if (setTimeoutremoveactiverow != null) {
            clearTimeout(setTimeoutremoveactiverow);
        }

        $(".table-combo tbody tr.active").removeClass("active");
        $(sel).addClass("active");
        setTimeoutremoveactiverow = setTimeout(() => {
            $(".table-combo tbody tr.active").removeClass("active");
        }, 1000);
    },
    loadeventUpdateAmount: function () {
        let _amount = 0;
        let _amountgiaban = 0;
        if ($(".table-combo tbody tr:not(.nodata)").length > 0) {
            $(".table-combo tbody tr").map(function () {
                // _quantity += parseFloat($(this).find(".quantity").val().replaceAll(",", "")) || 0;
                _amountgiaban += parseFloat($(this).find(".amount").data("amount")) || 0;
                _amount += parseFloat($(this).find(".amount").val().replaceAll(",", "")) || 0;
            });
        }
        $("#giavon").html(_amount.format0VND(3, 3, ""));
        $("#giaban").html(_amountgiaban.format0VND(3, 3, ""));

    },//sự kiện update lại tiền
    loadEventChangeQuantity: function () {
        $(".table-combo tbody tr input.quantity").unbind();
        $(".table-combo tbody tr input.quantity").change(function () {
            Product.loadhighlightrow($(this).parents("tr"));
            let data_quantity = $(this).data("quantity");
            let quantity = parseFloat($(this).val().replaceAll(",", "")) || 0;
            if (quantity > 0) {
                let price = parseFloat($(this).parents("tr").find(".price").val().replaceAll(",", "")) || 0;
                let retailPrice = parseFloat($(this).parents("tr").find(".retailPrice").val().replaceAll(",", "")) || 0;
                let amount = (quantity * retailPrice);//giá vốn
                let amountgiaban = (quantity * price);
                $(this).parents("tr").find(".amount").val(amount.format0VND(3, 3, ""));
                $(this).parents("tr").find(".amount").data("amount", amountgiaban);
                $(this).data("quantity", quantity);
            } else {
                toastrcus.error("Vui lòng nhập số lượng lớn hơn 0");
                $(this).val(data_quantity);
                return;
            }

            Product.loadeventUpdateAmount();
        });
    },//sự kiện thay đổi sl
    loadEventRemoveProduct: function () {
        $(".table-combo tbody tr .fa-remove").unbind();
        $(".table-combo tbody tr .fa-remove").click(function () {
            $(this).parents("tr").remove();
            Product.loadeventUpdateAmount();
        });
    },
    loadEvnetFormatTable: function () {
        $(".table-combo tbody tr input").unbind();
        $(".table-combo tbody tr input.number3").each(function (i, item) {
            var _val = $(this).val();
            $(this).val(_val.replaceAll(',', ''))

        });
        $(".table-combo tbody tr input.number3")
            .each(function (i, item) {
                var _val = $(this).val();
                $(this).val(_val.replaceAll(',', '.'));
                fnInitialFormatNumber(this);
            }).ForceNumericOnly()
            .focus(function () {
                var _val = $(this).val();
                $(this).val(_val.replaceAll(',', ''));
            })
            .focusout(function () {
                fnFocusOut(this);
            });
    },
}
///////////////
var JqueryModal = {
    ViewPDF: function (url) {
        loadingStart();
        let html = '<div id="ViewPDF"></div>';
        jQueryModal.Show("Thông tin chi tiết", html, "modal-70");
        setTimeout(function () {
            PDFObject.embed(url, "#ViewPDF");
            loadingStop()
        }, 1000);
    },
    Delete: function (url, ids) {

        confirms("Xác nhận", "Bạn có chắc chắn muốn xóa không?", "<i class='far fa-times-circle'></i> Hủy", "<i class='fas fa-check-circle'></i> Đồng ý", function () {
            try {
                $.ajax({
                    type: 'POST',
                    url: url,
                    async: false,
                    data: {
                        Id: parseInt(ids)
                    },
                    success: function (res) {
                        if (res.isValid) {
                            if (res.html != "") {
                                $('#viewAll').html(res.html)
                            }
                            return true;
                        }
                    },
                    error: function (err) {
                        console.log(err)
                    }
                });
                return false;
            } catch (ex) {
                console.log(ex)
            }
        });

        //prevent default form submit event
        return false;
    },
    DeleteRemove: function (url, ids, sel) {

        confirms("Xác nhận", "Bạn có chắc chắn muốn xóa không?", "<i class='far fa-times-circle'></i> Hủy", "<i class='fas fa-check-circle'></i> Đồng ý", function () {
            try {
                $.ajax({
                    type: 'POST',
                    url: url,
                    async: false,
                    data: {
                        Id: parseInt(ids)
                    },
                    success: function (res) {
                        if (res.isValid) {
                            if (res.html != "") {
                                $('#viewAll').html(res.html)
                            }
                            $(sel).parents("tr").remove();
                            return true;
                        }
                    },
                    error: function (err) {
                        console.log(err)
                    }
                });
                return false;
            } catch (ex) {
                console.log(ex)
            }
        });

        //prevent default form submit event
        return false;
    }
}



$(".btn-save-setting-inCustome-genager").click(function () {
    loadingElementStart("col-setting-genager");
    $.ajax({
        //global: false,
        type: "POST",
        data: $("#setting-genager").serialize(),
        url: "/admin/ConfigSystem/UpdateSettingGeneral",
        success: function (msg) {
            loadingStopElement("col-setting-genager");
            Ladda.stopAll();
        }
    });

});
$(".btn-save-setting-sell").click(function () {
    if ($("#setting-sellBannerheader").valid()) {
        loadingElementStart("col-setting-sell");
        $.ajax({
            //global: false,
            type: "POST",
            data: $("#setting-sellBannerheader").serialize(),
            url: "/admin/ConfigSystem/UpdateSettingSell",
            success: function (msg) {
                loadingStopElement("col-setting-sell");
                Ladda.stopAll();
            }
        });
    }
});
$(".btn-save-setting-headerlayout").click(function () {
    loadingElementStart("col-setting-genagerBannerheader");
    $.ajax({
        //global: false,
        type: "POST",
        data: $("#setting-genagerBannerheader").serialize(),
        url: "/admin/ConfigSystem/UpdateSettingGeneral",
        success: function (msg) {
            loadingStopElement("col-setting-genagerBannerheader");
            Ladda.stopAll();
        }
    });

});
$(".btn-save-setting-category-home").click(function () {

    const selectedNodes = $("#treeview").dxTreeView("getSelectedNodes");
    if (selectedNodes.length == 0) {
        toastr.error("Vui lòng chọn ít nhất 1 chuyên mục cha");
        Ladda.stopAll();
    } else {
        loadingElementStart("treeview");
        var pluginArrayArg = new Array();
        selectedNodes.forEach(function (key, index) {
            pluginArrayArg.push(key.itemData);
        });
        var dataJson = JSON.stringify(pluginArrayArg);
        $.ajax({
            //global: false,
            type: "POST",
            data: {
                jsonData: dataJson
            },
            url: "/admin/ConfigSystem/UpdateSettingCategoryInHome",
            success: function (msg) {
                loadingStopElement("treeview");
                Ladda.stopAll();
            }
        });
    }
});

function loadingElementStart(id) {
    $("#" + id).block({
        message: $('<div class="loader mx-auto">\n<div class="ball-pulse-sync">\n<div class="bg-white"></div>\n<div class="bg-white"></div>\n<div class="bg-white"></div>\n</div>\n</div>')
    })
}

function loadingStopElement(id) {
    $('#' + id).unblock();
}

function loadingStart() {
    $.blockUI({
        message: $('<div class="loader mx-auto">\n<div class="ball-pulse-sync">\n<div class=""></div>\n<div class=""></div>\n<div class=""></div>\n</div>\n</div>')
    })
}

function loadingStop() {
    $.unblockUI();
}

function loadDataAjax(url, id = "#viewAll") {
    $.ajax({
        global: false,
        type: "GET",
        url: url,
        beforeSend: function () {
            $(id).block({
                message: $('<div class="loader mx-auto">\n<div class="ball-pulse-sync">\n<div class="bg-white"></div>\n<div class="bg-white"></div>\n<div class="bg-white"></div>\n</div>\n</div>')
            })
        },
        success: function (msg) {

            if (typeof msg.isValid !== 'undefined' && msg.isValid) {
                $(id).html(msg.html);
                LoadDataTable();
            } else {
                $(id).html(msg);
            }

            $.unblockUI();
            Ladda.stopAll();
            //$("#loading-image").hide();
        }
    });
}

function loaddataSelect2(URL, id, idselectd = "", title = "") {
    $.ajax({
        global: true,
        type: "GET",
        url: URL,
        // async: true,
        data: {
            idselectd: idselectd
        },
        success: function (data) {
            var arr = JSON.parse(data);
            arr.push("");
            $(id).select2({
                data: JSON.parse(data),
                placeholder: (title != "" ? title : "Chọn giá trị"),
                allowClear: true,
                language: {
                    noResults: function () {
                        return "Không tìm thấy dữ liệu";
                    }
                },
            })
        }
    });
}

function sendmail(value, iddatamail) {
    $.ajax({
        // global: false,
        type: "POST",
        url: "/admin/email/SendEmail",
        async: false,
        data: {
            email: value,
            id: iddatamail,
        },
        success: function (data) {

        }
    });
}

function reSendEmail(id) {
    var iddatamail = id;
    $.ajax({
        type: "POST",
        url: "/admin/email/GetById?id=" + id,
        success: function (data) {
            if (data.isValid) {
                Swal.fire({
                    title: 'Địa chỉ email gửi',
                    input: 'text',
                    inputLabel: 'Nhập email gửi cách nhau bởi dấu phẩy',
                    inputValue: data.email,
                    showCancelButton: true,
                    inputValidator: (value) => {
                        if (!value) {
                            return 'Vui lòng nhập email!'
                        } else {
                            var pattern = /^\b[A-Z0-9._%-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\b$/i;
                            if (value.indexOf(",") != -1) {
                                var lst = value.split(",");
                                lst.forEach(function (element) {
                                    if (!pattern.test(element)) {
                                        return `Địa chỉ email ` + element + " không hợp lệ";
                                    }
                                })
                            } else {
                                if (!pattern.test(value)) {
                                    return `Địa chỉ email ` + value + " không hợp lệ";
                                }
                            }
                            sendmail(value, iddatamail);
                        }
                    }
                })
            }
        }
    });
}

function loaddataSelect2arrPost(id, URL, code, idbrand, idselectd = "") {
    $.ajax({
        global: false,
        type: "POST",
        url: URL,
        async: true,
        data: {
            idselectd: idselectd,
            code: code,
            idbrand: idbrand,
        },
        success: function (data) {
            var arr = JSON.parse(data);
            $(id).select2({
                data: arr,
                placeholder: "Chọn giá trị",
                allowClear: true,
                language: {
                    noResults: function () {
                        return "Không tìm thấy dữ liệu";
                    }
                },
            })
        }
    });
}

function loaddataCheckboxGET(URL, idselectd = "") {
    var html = "";
    $.ajax({
        global: false,
        type: "GET",
        url: URL,
        async: false,
        data: {
            idselectd: idselectd,
        },
        success: function (data) {
            var datajson = JSON.parse(data);
            datajson.forEach(loadArr);

            function loadArr(value, index, array) {
                let check = "";
                if (value.selected) {
                    check = "checked=checked";
                }
                html += `<div class="col-md-3 mb-2 flex">
                            <input name="idModel" class="mr-2" ` + check + ` value="` + value.id + `" type="checkbox" id="idModel"/> ` + value.text + `
                        </div>`;
            }
        }
    });
    return html;
}

///////////////////

function loaddaterangepicker(timePicker = true) {
    var today = new Date();
    var dd = String(today.getDate()).padStart(2, '0');
    var mm = String(today.getMonth() + 1).padStart(2, '0'); //January is 0!
    var yyyy = today.getFullYear();

    today = dd + '/' + mm + '/' + yyyy;
    let fotmat = "DD/MM/YYYY HH:mm";
    if (!timePicker) {
        fotmat = "DD/MM/YYYY";
    }

    try {
        $('.fc-datetimepicker').daterangepicker({
            "singleDatePicker": true,
            "showDropdowns": true,
            showTodayButton: true,
            minDate: 0,
            //startDate: today,
            autoApply: true,
            todayHighlight: true,
            "autoUpdateInput": false,
            timePicker: timePicker,
            timePicker24Hour: timePicker,
            "format": fotmat,
            "locale": {
                "format": fotmat,
                "separator": " - ",
                "applyLabel": "Đồng ý",
                "cancelLabel": "Hủy bỏ",
                "fromLabel": "From",
                "toLabel": "To",
                "customRangeLabel": "Custom",
                "weekLabel": "W",
                "daysOfWeek": [
                    "CN",
                    "T2",
                    "T3",
                    "T4",
                    "T5",
                    "T6",
                    "T7"
                ],
                "monthNames": [
                    "Tháng 1",
                    "Tháng 2",
                    "Tháng 3",
                    "Tháng 4",
                    "Tháng 5",
                    "Tháng 6",
                    "Tháng 7",
                    "Tháng 8",
                    "Tháng 9",
                    "Tháng 10",
                    "Tháng 11",
                    "Tháng 12"
                ],
                "firstDay": 1
            },
        });


        var isClick = 0;

        $(window).on('click', function () {
            isClick = 0;
        });
        var myCalendar = $('.fc-datetimepicker');
        $(myCalendar).on('apply.daterangepicker', function (ev, picker) {
            isClick = 0;
            $(this).val(picker.startDate.format(fotmat));

        });

        $('.js-btn-calendar').on('click', function (e) {
            e.stopPropagation();

            if (isClick === 1) isClick = 0;
            else if (isClick === 0) isClick = 1;

            if (isClick === 1) {
                myCalendar.focus();
            }
        });

        $(myCalendar).on('click', function (e) {
            e.stopPropagation();
            isClick = 1;
        });

        $('.daterangepicker').on('click', function (e) {
            e.stopPropagation();
        });


    } catch (er) {
        console.log(er);
    }

    try {
        $('.fc-datepicker').daterangepicker({
            "singleDatePicker": true,
            "showDropdowns": true,
            "autoUpdateInput": false,
            todayHighlight: true,
            "locale": {
                "format": "DD/MM/YYYY",
                "separator": " - ",
                "applyLabel": "Apply",
                "cancelLabel": "Cancel",
                "fromLabel": "From",
                "toLabel": "To",
                "customRangeLabel": "Custom",
                "weekLabel": "W",
                "daysOfWeek": [
                    "CN",
                    "T2",
                    "T3",
                    "T4",
                    "T5",
                    "T6",
                    "T7"
                ],
                "monthNames": [
                    "Tháng 1",
                    "Tháng 2",
                    "Tháng 3",
                    "Tháng 4",
                    "Tháng 5",
                    "Tháng 6",
                    "Tháng 7",
                    "Tháng 8",
                    "Tháng 9",
                    "Tháng 10",
                    "Tháng 11",
                    "Tháng 12"
                ],
                "firstDay": 1
            },
        });


        var isClick = 0;

        $(window).on('click', function () {
            isClick = 0;
        });
        var myCalendar = $('.fc-datepicker');
        $(myCalendar).on('apply.daterangepicker', function (ev, picker) {
            isClick = 0;
            $(this).val(picker.startDate.format('DD/MM/YYYY'));

        });

        $('.js-btn-calendar').on('click', function (e) {
            e.stopPropagation();

            if (isClick === 1) isClick = 0;
            else if (isClick === 0) isClick = 1;

            if (isClick === 1) {
                myCalendar.focus();
            }
        });

        $(myCalendar).on('click', function (e) {
            e.stopPropagation();
            isClick = 1;
        });

        $('.daterangepicker').on('click', function (e) {
            e.stopPropagation();
        });


    } catch (er) {
        console.log(er);
    }
}
//load datetiempicker
function loaddatetimerangepicker() {

    try {
        $('.fc-datepicker').daterangepicker({
            "singleDatePicker": true,
            "showDropdowns": true,
            "autoUpdateInput": false,
            todayHighlight: true,
            timePicker: true,
            timePicker24Hour: true,
            "locale": {
                "format": "DD/MM/YYYY HH:mm:ss",
                "separator": " - ",
                "applyLabel": "Đồng ý",
                "cancelLabel": "Hủy bỏ",
                "fromLabel": "From",
                "toLabel": "To",
                "customRangeLabel": "Custom",
                "weekLabel": "W",
                "daysOfWeek": [
                    "CN",
                    "T2",
                    "T3",
                    "T4",
                    "T5",
                    "T6",
                    "T7"
                ],
                "monthNames": [
                    "Tháng 1",
                    "Tháng 2",
                    "Tháng 3",
                    "Tháng 4",
                    "Tháng 5",
                    "Tháng 6",
                    "Tháng 7",
                    "Tháng 8",
                    "Tháng 9",
                    "Tháng 10",
                    "Tháng 11",
                    "Tháng 12"
                ],
                "firstDay": 1
            },
        });


        var isClick = 0;

        $(window).on('click', function () {
            isClick = 0;
        });
        var myCalendar = $('.fc-datepicker');
        $(myCalendar).on('apply.daterangepicker', function (ev, picker) {
            isClick = 0;
            $(this).val(picker.startDate.format('DD/MM/YYYY HH:mm:ss'));

        });

        $('.js-btn-calendar').on('click', function (e) {
            e.stopPropagation();

            if (isClick === 1) isClick = 0;
            else if (isClick === 0) isClick = 1;

            if (isClick === 1) {
                myCalendar.focus();
            }
        });

        $(myCalendar).on('click', function (e) {
            e.stopPropagation();
            isClick = 1;
        });

        $('.daterangepicker').on('click', function (e) {
            e.stopPropagation();
        });


    } catch (er) {
        console.log(er);
    }
}
//load check all
function loadcheckall() {

    $("#checkall").click(function () {
        $('#tableselect input.item-check').not(this).prop('checked', this.checked);

    });

    $('.item-check').change(function () {
        if ($('.item-check:checked').length == $('.item-check').length) {
            $('#checkall').prop('checked', true);
        } else {
            $('#checkall').prop('checked', false);
        }
    });
}
//load event checkall in sự kiện
function loadeventcheckall() {
    var _lists = [];
    $(".saveEvent").click(function () {

        if ($('input#checkall').is(':checked')) {
            //check all k cần xử lý
        } else {
            $('table#tableselect > tbody tr input[type=checkbox]:checked').each(function (index, tr) {
                let id = $(tr).data("id");
                _lists.push(parseInt(id));
            });
        }
        if (_lists.length > 0) {
            let json = JSON.stringify(_lists);
            $("#JsonProduct").val(json);
        }
        $(this).parents("form").submit();
    });

}

var btnsaveStatus = $("#btnsaveStatus");
var btncancelorder = $("#btn-cancelorder");
var iunpxChangepass = $("#ipxChangepass");

var search = "#btnSearch";
var addevincoie = $(".btn-addeinvoice");
var editeinvoice = $(".btn-editeinvoice");

$(document).on("click", '#btn-cancelorder', function (event) {
    let id = $(this).data("id");
    confirms("Xác nhận", "Bạn có chắc chắn hủy đơn hàng không?", "<i class='far fa-times-circle'></i> Hủy", "<i class='fas fa-check-circle'></i> Đồng ý", function () {
        try {
            $.ajax({
                type: 'GET',
                url: "/admin/order/Cancel?id=" + id,
                success: function (res) {
                    $('#form-modal .modal-body').html(res.html);
                    $('#form-modal .modal-title').html("Hủy đơn hàng");
                    $('#form-modal').modal('show');
                    loadeventnotecontetn();
                },
                error: function (err) {
                    console.log(err)
                }
            })
        } catch (ex) {
            console.log(ex)
        }
    });
    //prevent default form submit event
    return false;

})
var eventCreate = {
    viewSupplier: function (url) {//khu vực nhà hàng
        $.ajax({
            type: 'GET',
            //global: false,
            url: url,
            // data: fomrdata,
            contentType: false,
            processData: false,
            success: function (res) {
                if (res.isValid) {
                    Swal.fire({
                        // icon: 'success',
                        title: "Chi tiết nhà cung cấp",
                        html: res.html,
                        position: 'top-end',
                        showClass: {
                            popup: `
                              popup-formcreate
                              popup-lg
                               animate__animated
                              animate__fadeInRight
                              animate__faster
                            `
                        },
                        hideClass: {
                            popup: "popup-formcreate popup-lg animate__animated animate__fadeOutRight animate__faster"
                        }, showCloseButton: true,
                        // footer: "",
                        allowOutsideClick: true,
                        showConfirmButton: false,
                        showCancelButton: false,
                        cancelButtonText: '<i class="fa fa-window-close"></i> Đóng',
                        didRender: () => {

                        }
                    });
                }
            },
            error: function (err) {
                console.log(err)
            }
        });

    }, addOrEditSupplier: function (url) {//khu vực nhà hàng
        $.ajax({
            type: 'GET',
            //global: false,
            url: url,
            // data: fomrdata,
            contentType: false,
            processData: false,
            success: function (res) {
                if (res.isValid) {
                    Swal.fire({
                        // icon: 'success',
                        title: res.title,
                        html: res.html,
                        showClass: {
                            popup: `
                              popup-formcreate
                                popup-formleft-500
                               animate__animated
                              animate__fadeInRight
                              animate__faster
                            `
                        },
                        hideClass: {
                            popup: "popup-formcreate popup-formleft-500 animate__animated animate__fadeOutRight animate__faster"

                        },

                        footer: "<button class='btn btn-primary btn-continue mr-3'><i class='icon-cd icon-add_cart icon'></i>Hủy bỏ</button><button class='btn btn-save btn-success'><i class='icon-cd icon-doneAll icon'></i>Lưu</button>",
                        allowOutsideClick: true,
                        showConfirmButton: false,
                        showCancelButton: false,
                        cancelButtonText: '<i class="fa fa-window-close"></i> Đóng',
                        didRender: () => {
                            validateForm.EditAndAddSupplier();
                            evetnFormatnumber3();

                            $(".btn-continue").click(function () {
                                Swal.close();
                            });
                            $(".btn-save").click(function () {
                                if ($("form#create-form").valid()) {
                                    jQueryModalPost($("form#create-form")[0]);
                                }

                            });

                        }
                    });
                }
            },
            error: function (err) {
                console.log(err)
            }
        });

    },
    addOrEditArea: function (url) {//khu vực nhà hàng
        $.ajax({
            type: 'GET',
            //global: false,
            url: url,
            // data: fomrdata,
            contentType: false,
            processData: false,
            success: function (res) {
                if (res.isValid) {
                    Swal.fire({
                        // icon: 'success',
                        title: res.title,
                        html: res.html,
                        showClass: {
                            popup: 'popup-formcreate'
                        },

                        footer: "<button class='btn btn-primary btn-continue mr-3'><i class='icon-cd icon-add_cart icon'></i>Hủy bỏ</button><button class='btn btn-save btn-success'><i class='icon-cd icon-doneAll icon'></i>Lưu</button>",
                        allowOutsideClick: true,
                        showConfirmButton: false,
                        showCancelButton: false,
                        cancelButtonText: '<i class="fa fa-window-close"></i> Đóng',
                        didRender: () => {
                            validateForm.EditAndAddTableRoom();
                            $(".btn-continue").click(function () {
                                Swal.close();
                            });
                            $(".btn-save").click(function () {
                                if ($("form#create-form").valid()) {
                                    jQueryModalPost($("form#create-form")[0]);
                                }

                            });

                        }
                    });
                }
            },
            error: function (err) {
                console.log(err)
            }
        });

    },
    addOrEditUnit: function (url) {//khu vực nhà hàng
        $.ajax({
            type: 'GET',
            //global: false,
            url: url,
            // data: fomrdata,
            contentType: false,
            processData: false,
            success: function (res) {
                if (res.isValid) {
                    Swal.fire({
                        // icon: 'success',
                        title: res.title,
                        html: res.html,
                        showClass: {
                            popup: 'popup-formcreate'
                        },

                        footer: "<button class='btn btn-primary btn-continue mr-3'><i class='icon-cd icon-add_cart icon'></i>Hủy bỏ</button><button class='btn btn-save btn-success'><i class='icon-cd icon-doneAll icon'></i>Lưu</button>",
                        allowOutsideClick: true,
                        showConfirmButton: false,
                        showCancelButton: false,
                        cancelButtonText: '<i class="fa fa-window-close"></i> Đóng',
                        didRender: () => {
                            validateForm.EditAndAddUnit();
                            $(".btn-continue").click(function () {
                                Swal.close();
                            });
                            $(".btn-save").click(function () {
                                if ($("form#create-form").valid()) {
                                    jQueryModalPost($("form#create-form")[0]);
                                }

                            });

                        }
                    });
                }
            },
            error: function (err) {
                console.log(err)
            }
        });

    },

    addOrEditRoomTable: function (url) {
        $.ajax({
            type: 'GET',
            //global: false,
            url: url,
            // data: fomrdata,
            contentType: false,
            processData: false,
            success: function (res) {
                //if (res.isValid) {
                Swal.fire({
                    // icon: 'success',
                    position: 'top-end',
                    showClass: {
                        popup: `
                              popup-formcreate
                               animate__animated
                              animate__fadeInRight
                              animate__faster
                            `
                    },
                    hideClass: {
                        popup: "popup-formcreate animate__animated animate__fadeOutRight animate__faster"

                    },
                    showCloseButton: true,

                    title: "Thêm/Chỉnh sửa phòng/bàn",
                    html: res,
                    //showClass: {
                    //    popup: 'popup-formcreate'
                    //},

                    footer: "<button class='btn btn-primary btn-continue mr-3'><i class='icon-cd icon-add_cart icon'></i>Hủy bỏ</button><button class='btn btn-save btn-success'><i class='icon-cd icon-doneAll icon'></i>Lưu</button>",
                    allowOutsideClick: true,
                    showConfirmButton: false,
                    showCancelButton: false,
                    cancelButtonText: '<i class="fa fa-window-close"></i> Đóng',
                    didRender: () => {
                        validateForm.EditAndAddTableRoom();
                        $("#create-form .SelectListArea").select2({

                            placeholder: {
                                text: 'Chọn giá trị'
                            },
                            allowClear: true,
                            language: {
                                noResults: function () {
                                    return "Không tìm thấy dữ liệu";
                                }
                            },
                        });
                        $(".btn-continue").click(function () {
                            Swal.close();
                        });
                        $(".btn-save").click(function () {
                            if ($("form#create-form").valid()) {
                                jQueryModalPost($("form#create-form")[0]);
                            }

                        });
                        // sự kiến kích vào thêm nhiefu bàn
                        $("#IsCreateMuti").click(function () {
                            $(".fromMutitable").toggleClass("d-none");

                            if ($(".fromMutitable").hasClass("d-none")) {
                                $(".fromMutitable").find("input").attr("type", "hidden");
                            } else {
                                $(".fromMutitable").find("input").attr("type", "text");
                            }


                            $(".fromOnetable").toggleClass("d-none");
                            if ($(".fromOnetable").hasClass("d-none")) {
                                $(".fromOnetable").find("input").attr("type", "hidden");
                            } else {
                                $(".fromOnetable").find("input").attr("type", "text");
                            }
                            // validateForm.EditAndAddTableRoom();

                        })
                    }
                });
                //}
            },
            error: function (err) {
                console.log(err)
            }
        });

    },
    addOrEditCustomer: function (url) {
        $.ajax({
            type: 'GET',
            //global: false,
            url: url,
            // data: fomrdata,
            contentType: false,
            processData: false,
            success: function (res) {
                if (res.isValid) {
                    Swal.fire({
                        // icon: 'success',
                        title: res.title,
                        html: res.html,
                        showClass: {
                            popup: 'popup-formcreate popup-formcreate-600'
                        },

                        footer: "<button class='btn btn-primary btn-continue mr-3'><i class='icon-cd icon-add_cart icon'></i>Hủy bỏ</button><button class='btn btn-save btn-success'><i class='icon-cd icon-doneAll icon'></i>Lưu</button>",
                        allowOutsideClick: true,
                        showConfirmButton: false,
                        showCancelButton: false,
                        cancelButtonText: '<i class="fa fa-window-close"></i> Đóng',
                        didRender: () => {
                            loadEventIcheck();
                            validateForm.EditAndAddCustomer();
                            $(".btn-continue").click(function () {
                                Swal.close();
                            });
                            $(".btn-save").click(function () {
                                jQueryModalPost($("form#create-form")[0]);
                            });
                        }
                    });
                }
            },
            error: function (err) {
                console.log(err)
            }
        });

    },
    addOrEditTemplateInvoice: function (url) {
        $.ajax({
            type: 'GET',
            //global: false,
            url: url,
            // data: fomrdata,
            contentType: false,
            processData: false,
            success: function (res) {
                if (res.isValid) {
                    Swal.fire({
                        // icon: 'success',
                        title: res.title,
                        html: res.html,
                        showClass: {
                            popup: 'popup-formcreate'
                        },

                        footer: "<button class='btn btn-primary btn-continue mr-3'><i class='icon-cd icon-add_cart icon'></i>Hủy bỏ</button><button class='btn btn-save btn-success'><i class='icon-cd icon-doneAll icon'></i>Lưu</button>",
                        allowOutsideClick: true,
                        showConfirmButton: false,
                        showCancelButton: false,
                        cancelButtonText: '<i class="fa fa-window-close"></i> Đóng',
                        didRender: () => {
                            validateForm.EditAndAddTemplateInvoice();
                            $(".btn-continue").click(function () {
                                Swal.close();
                            });
                            $(".btn-save").click(function () {
                                if ($("form#create-form").valid()) {
                                    jQueryModalPost($("form#create-form")[0]);
                                }
                            });

                        }
                    });
                }
            },
            error: function (err) {
                console.log(err)
            }
        });

    },
    addOrEditPaymentmethod: function (url) {
        $.ajax({
            type: 'GET',
            //global: false,
            url: url,
            // data: fomrdata,
            contentType: false,
            processData: false,
            success: function (res) {
                if (res.isValid) {
                    Swal.fire({
                        // icon: 'success',
                        position: 'top-end',
                        showClass: {
                            popup: `
                              popup-formcreate
                                popup-formleft-500
                               animate__animated
                              animate__fadeInRight
                              animate__faster
                            `
                        },
                        hideClass: {
                            popup: "popup-formcreate popup-formleft-500 animate__animated animate__fadeOutRight animate__faster"

                        },
                        showCloseButton: true,
                        title: res.title,
                        html: res.html,
                        //showClass: {
                        //    popup: 'popup-formcreate'
                        //},

                        footer: "<button class='btn btn-primary btn-continue mr-3'><i class='icon-cd icon-add_cart icon'></i>Hủy bỏ</button><button class='btn btn-save btn-success'><i class='icon-cd icon-doneAll icon'></i>Lưu</button>",
                        allowOutsideClick: true,
                        showConfirmButton: false,
                        showCancelButton: false,
                        cancelButtonText: '<i class="fa fa-window-close"></i> Đóng',
                        didRender: () => {
                            validateForm.EditAndAddPaymentMethod();
                            $(".btn-continue").click(function () {
                                Swal.close();
                            });

                            $(".btn-save").click(function () {
                                if ($("form#create-form").valid()) {
                                    jQueryModalPost($("form#create-form")[0]);
                                }

                            });
                        }
                    });
                }
            },
            error: function (err) {
                console.log(err)
            }
        });

    },
    addOrEditUser: function (url) {
        $.ajax({
            type: 'GET',
            //global: false,
            url: url,
            // data: fomrdata,
            contentType: false,
            processData: false,
            success: function (res) {
                if (res.isValid) {
                    Swal.fire({
                        // icon: 'success',
                        position: 'top-end',
                        showClass: {
                            popup: `
                              popup-formcreate
                                popup-formleft-500
                               animate__animated
                              animate__fadeInRight
                              animate__faster
                            `
                        },
                        hideClass: {
                            popup: "popup-formcreate popup-formleft-500 animate__animated animate__fadeOutRight animate__faster"

                        },
                        showCloseButton: true,
                        title: "Cập nhật nhân viên",
                        html: res.html,
                        //showClass: {
                        //    popup: 'popup-formcreate'
                        //},

                        footer: "<button class='btn btn-primary btn-continue mr-3'><i class='icon-cd icon-add_cart icon'></i>Hủy bỏ</button><button class='btn btn-save btn-success'><i class='icon-cd icon-doneAll icon'></i>Lưu</button>",
                        allowOutsideClick: true,
                        showConfirmButton: false,
                        showCancelButton: false,
                        cancelButtonText: '<i class="fa fa-window-close"></i> Đóng',
                        didRender: () => {

                            validateFormUser();
                            $(".btn-continue").click(function () {
                                Swal.close();
                            });
                            $(".btn-save").click(function () {
                                if ($("form#FormValidate").valid()) {
                                    jQueryModalPost($("form#FormValidate")[0]);
                                }

                            });
                        }
                    });
                }
            },
            error: function (err) {
                console.log(err)
            }
        });

    },
    editTableInOrder: function (url) {
        $.ajax({
            type: 'GET',
            url: url,
            contentType: false,
            processData: false,
            success: function (res) {
                if (res.isValid) {
                    Swal.fire({
                        // icon: 'success',
                        position: 'top-end',
                        showClass: {
                            popup: `
                              popup-formcreate
                              popup-formleft-500
                              animate__animated
                              animate__fadeInRight
                              animate__faster
                            `
                        },
                        hideClass: {
                            popup: "popup-formcreate popup-formleft-500 animate__animated animate__fadeOutRight animate__faster"

                        },
                        showCloseButton: true,

                        title: "Thay đổi bàn",
                        html: res.html,
                        footer: "<button class='btn btn-primary btn-continue mr-3'><i class='icon-cd icon-add_cart icon'></i>Hủy bỏ</button><button class='btn btn-save btn-success'><i class='icon-cd icon-doneAll icon'></i>Lưu</button>",
                        allowOutsideClick: true,
                        showConfirmButton: false,
                        showCancelButton: false,
                        cancelButtonText: '<i class="fa fa-window-close"></i> Đóng',
                        didRender: () => {
                            $('#lstroomselect').append('<option selected></option>').select2({
                                placeholder: {
                                    id: '', // the value of the option
                                    text: "Chọn bàn"
                                },
                                allowClear: true,
                                language: {
                                    noResults: function () {
                                        return "Không tìm thấy dữ liệu";
                                    }
                                },
                            })
                            $('input.icheck-table').iCheck({
                                checkboxClass: 'icheckbox_square-green',
                                radioClass: 'iradio_square-green',
                                increaseArea: '20%' // optional
                            });
                            $('input.mangve').on('ifChanged', function (event) {

                                if (!$(".tabtachdon").hasClass("d-none")) {
                                    $(".tabtachdon").addClass("d-none");
                                    $('#lstroomselect').select2('close');
                                }
                            });
                            $('input.bankhac').on('ifChanged', function (event) {
                                if ($(".tabtachdon").hasClass("d-none")) {
                                    $(".tabtachdon").removeClass("d-none");
                                }
                            });
                            $(".btn-continue").click(function () {
                                Swal.close();
                            });
                            $(".btn-save").click(function () {
                                loadeventPos.updateChangetableOrder();
                            });

                        }
                    });
                }
            },
            error: function (err) {
                console.log(err)
            }
        });

    },
    addOrEditRevenueExpenditure: function (url) {
        $.ajax({
            type: 'GET',
            //global: false,
            url: url,
            // data: fomrdata,
            contentType: false,
            processData: false,
            success: function (res) {
                if (res.isValid) {
                    Swal.fire({
                        // icon: 'success',
                        position: 'top-end',
                        showClass: {
                            popup: `
                              popup-formcreate
                                popup-formleft-500
                               animate__animated
                              animate__fadeInRight
                              animate__faster
                            `
                        },
                        hideClass: {
                            popup: "popup-formcreate popup-formleft-500 animate__animated animate__fadeOutRight animate__faster"

                        },
                        showCloseButton: true,
                        title: res.title,
                        html: res.html,
                        footer: "<button class='btn btn-primary btn-continue mr-3'><i class='icon-cd icon-add_cart icon'></i>Hủy bỏ</button><button class='btn btn-save btn-success'><i class='icon-cd icon-doneAll icon'></i>Lưu</button>",
                        allowOutsideClick: true,
                        showConfirmButton: false,
                        showCancelButton: false,
                        cancelButtonText: '<i class="fa fa-window-close"></i> Đóng',
                        didRender: () => {
                            $(".btn-continue").click(function () {
                                Swal.close();
                            });
                            loadFormatnumber()
                            validateForm.EditAndAddRevenueExpenditure();
                            $("#formObjectRevenueExpenditures").select2({
                                dropdownParent: $("#create-form"),
                                placeholder: "Chọn đối tượng nộp",
                                allowClear: true,
                                language: {
                                    noResults: function () {
                                        return "Không tìm thấy dữ liệu";
                                    }
                                },

                            }).on('change', async function (e) {

                                var _curen = $(this).val();


                                if (_curen == EnumTypeObjectRevenueExpenditure.KHACHHANG) {
                                    let getCustomre = await axios.get("/Selling/Customer/GetJsonDataCustomer");
                                    html = `<select name="CustomerName" id="CustomerName" class="form-control" style="width: 100%;" required'>
                                                <option selected></option>
                                                </select>`
                                    $("#fomrCustomerName").html(html);
                                    $("#fomrCustomerName select").select2({
                                        dropdownParent: $("#create-form"),
                                        data: getCustomre.data,
                                        placeholder: {
                                            id: '', // the value of the option
                                            text: "Chọn khách hàng"
                                        },
                                        allowClear: true,
                                        language: {
                                            noResults: function () {
                                                return "Không tìm thấy dữ liệu";
                                            }
                                        },
                                    })
                                }
                                else if (_curen == EnumTypeObjectRevenueExpenditure.DOITAC) {
                                    let getCustomre = await axios.get("/Selling/Suppliers/GetJsonDataSuppliers");
                                    html = `<select name="CustomerName" id="CustomerName" class="form-control" style="width: 100%;" required'>
                                                <option selected></option>
                                                </select>`
                                    $("#fomrCustomerName").html(html);
                                    $("#fomrCustomerName select").select2({
                                        dropdownParent: $("#create-form"),
                                        data: getCustomre.data,
                                        placeholder: {
                                            id: '', // the value of the option
                                            text: "Chọn khách hàng"
                                        },
                                        allowClear: true,
                                        language: {
                                            noResults: function () {
                                                return "Không tìm thấy dữ liệu";
                                            }
                                        },
                                    })
                                } else if (_curen == EnumTypeObjectRevenueExpenditure.DOITUONGKHAC) {
                                    $("#fomrCustomerName").html('<input type="text" name="CustomerName" id="CustomerName" placeholder="Tên người nộp" class="form-control">');
                                }



                            });
                            $("#formselectCategoryCevenues").select2({
                                dropdownParent: $("#create-form"),
                                placeholder: "Chọn loại phiếu " + res.typeNme,
                                allowClear: true,
                                language: {
                                    noResults: function () {
                                        return "Không tìm thấy dữ liệu";
                                    }
                                },

                            })
                            $("#formPaymentMethod").select2({
                                dropdownParent: $("#create-form"),
                                placeholder: "Chọn hình thức thanh toán",
                                allowClear: true,
                                language: {
                                    noResults: function () {
                                        return "Không tìm thấy dữ liệu";
                                    }
                                },

                            });
                            loaddaterangepicker(false);
                            // dành cho danh mục thu/chi
                            if (res.isEditCategoryCevenue) {
                                $("#create-form").find("#Code").attr("readonly", "readonly");
                            }
                            // dành cho phiếu thu/chi
                            if (res.isEditRevenueExpenditure) {
                                $("#create-form").find("#Amount").attr("readonly", "readonly");
                                $("#create-form").find("#formselectCategoryCevenues").prop("disabled", true);
                                $("#create-form").find("#formObjectRevenueExpenditures").prop("disabled", true);
                                $("#create-form").find("#CustomerName").attr("readonly", "readonly");
                                $("#create-form").find("#formPaymentMethod").prop("disabled", true);
                            }
                            $(".btn-save").click(function () {
                                $('.number3').each(function () {
                                    let idtex = $(this).val().replaceAll(",", "");
                                    $(this).val(parseFloat(idtex))
                                });
                                if ($("form#create-form").valid()) {
                                    jQueryModalPost($("form#create-form")[0]);
                                }
                            });
                        }
                    });
                }
            },
            error: function (err) {
                console.log(err)
            }
        });

    },
}
var loadeventProduct = {
    loadEventbtnaction: function () {
        $(".btn-removeprouct").click(async function () {

            let title = "Bạn có chắc chắn muốn xóa các hóa đơn đã chọn không?";
            await Swal.fire({
                icon: 'warning',
                title: title,
                // showDenyButton: true,
                showCancelButton: true,
                confirmButtonText: 'Đồng ý',
                cancelButtonText: 'Đóng',
                // denyButtonText: `Don't save`,
            }).then((result) => {
                /* Read more about isConfirmed, isDenied below */
                if (result.isConfirmed) {
                    var lstid = new Array();
                    var rows_selected = dataTableOut.column(0).checkboxes.selected();
                    rows_selected.each(function (index, elem) {
                        lstid.push(parseInt(index));
                    });
                    //if (lstid.length > 20) {
                    //    let htlml = "Số lượng hóa đơn của bạn chọn " + lstid.length + " quá nhiều, trình duyệt không thể tải để in được, vui lòng chọn ít hơn 20 hóa đơn";
                    //    Swal.fire({
                    //        icon: 'error',
                    //        title: 'Thông báo',
                    //        html: htlml,
                    //        showCloseButton: true,
                    //        showCancelButton: true,
                    //        cancelButtonText: '<i class="fa fa-window-close"></i> Đã hiểu',
                    //    });
                    //}
                    $.ajax({
                        type: 'POST',
                        async: true,
                        url: '/Selling/Product/Delete',
                        traditional: true,
                        dataType: "json",
                        data: {
                            lstid: lstid
                        },
                        success: function (res) {
                            if (res.isValid) {
                                dataTableOut.columns().checkboxes.deselectAll(true)
                                dataTableOut.ajax.reload(null, false);
                            }
                        },
                        error: function (err) {
                            console.log(err)
                        }
                    });
                }
            });






        });
        $(".btn-stopbusiness").click(function () {
            var lstid = new Array();
            var rows_selected = dataTableOut.column(0).checkboxes.selected();
            rows_selected.each(function (index, elem) {
                lstid.push(parseInt(index));
            });
            if (lstid.length == 0) {
                let htlml = "Vui lòng chọn sản phẩm";
                Swal.fire({
                    icon: 'error',
                    title: 'Thông báo',
                    html: htlml,
                    showCloseButton: true,
                    showCancelButton: true,
                    cancelButtonText: '<i class="fa fa-window-close"></i> Đã hiểu',
                });
            }
            $.ajax({
                type: 'POST',
                async: true,
                url: '/Selling/Product/StopBusiness',
                traditional: true,
                dataType: "json",
                data: {
                    lstid: lstid
                },
                success: function (res) {
                    if (res.isValid) {
                        dataTableOut.ajax.reload(null, false);
                        title = "Kết quả cập nhật";
                        html = "Cập nhật thành công: " + res.data + " sản phẩm";
                        if (parseInt(res.data) != lstid.length) {
                            html = "<br/> Cập nhật thất bại " + (lstid.length - parseInt(res.data)) + " sản phẩm";
                        }
                        Swal.fire({
                            // icon: 'success',
                            position: 'center',
                            showClass: {
                                popup: `popup-formcreate eventpublisinvoice`
                            },

                            showCloseButton: true,

                            title: title,
                            html: html,
                            //showClass: {
                            //    popup: 'popup-formcreate'
                            //},

                            // footer: "<button class='btn btn-primary btn-continue mr-3'><i class='icon-cd icon-add_cart icon'></i>Hủy bỏ</button><button class='btn btn-save btn-success'><i class='icon-cd icon-doneAll icon'></i>Lưu</button>",
                            allowOutsideClick: true,
                            showConfirmButton: false,
                            showCancelButton: true,
                            cancelButtonText: '<i class="fa fa-window-close"></i> Đóng',
                            didRender: () => {
                                $(".btn-continue").click(function () {
                                    Swal.close();
                                });
                            }
                        });
                    }
                },
                error: function (err) {
                    console.log(err)
                }
            });

        });
        $(".btn-unstopbusiness").click(function () {
            var lstid = new Array();
            var rows_selected = dataTableOut.column(0).checkboxes.selected();
            rows_selected.each(function (index, elem) {
                lstid.push(parseInt(index));
            });
            if (lstid.length == 0) {
                let htlml = "Vui lòng chọn sản phẩm";
                Swal.fire({
                    icon: 'error',
                    title: 'Thông báo',
                    html: htlml,
                    showCloseButton: true,
                    showCancelButton: true,
                    cancelButtonText: '<i class="fa fa-window-close"></i> Đã hiểu',
                });
            }
            $.ajax({
                type: 'POST',
                async: true,
                url: '/Selling/Product/UnStopBusiness',
                traditional: true,
                dataType: "json",
                data: {
                    lstid: lstid
                },
                success: function (res) {
                    if (res.isValid) {
                        dataTableOut.ajax.reload(null, false);
                        title = "Kết quả cập nhật";
                        html = "Cập nhật thành công: " + res.data + " sản phẩm";
                        if (parseInt(res.data) != lstid.length) {
                            html = "<br/> Cập nhật thất bại " + (lstid.length - parseInt(res.data)) + " sản phẩm";
                        }
                        Swal.fire({
                            // icon: 'success',
                            position: 'center',
                            showClass: {
                                popup: `popup-formcreate eventpublisinvoice`
                            },

                            showCloseButton: true,

                            title: title,
                            html: html,
                            //showClass: {
                            //    popup: 'popup-formcreate'
                            //},

                            // footer: "<button class='btn btn-primary btn-continue mr-3'><i class='icon-cd icon-add_cart icon'></i>Hủy bỏ</button><button class='btn btn-save btn-success'><i class='icon-cd icon-doneAll icon'></i>Lưu</button>",
                            allowOutsideClick: true,
                            showConfirmButton: false,
                            showCancelButton: true,
                            cancelButtonText: '<i class="fa fa-window-close"></i> Đóng',
                            didRender: () => {
                                $(".btn-continue").click(function () {
                                    Swal.close();
                                });
                            }
                        });
                    }
                },
                error: function (err) {
                    console.log(err)
                }
            });

        });
        $(".btn-printbarcode").click(async function () {
            var lstid = new Array();
            var rows_selected = dataTableOut.column(0).checkboxes.selected();
            rows_selected.each(function (index, elem) {
                lstid.push(parseInt(index));
            });
            if (lstid.length == 0) {
                let htlml = "Vui lòng chọn sản phẩm";
                Swal.fire({
                    icon: 'error',
                    title: 'Thông báo',
                    html: htlml,
                    showCloseButton: true,
                    showCancelButton: true,
                    cancelButtonText: '<i class="fa fa-window-close"></i> Đã hiểu',
                });
            }
            $.ajax({
                type: 'POST',
                async: true,
                url: '/Selling/Product/PrintBarCode',
                traditional: true,
                dataType: "json",
                data: {
                    lstid: lstid
                },
                success: function (res) {
                    if (res.isValid) {
                        printDiv(res.html);
                    }
                },
                error: function (err) {
                    console.log(err)
                }
            });

        });


    }
}
function savefile(data, name) {
    var filename = name;
    var text = data;
    var pom = document.createElement('a');
    pom.setAttribute('href', 'data:text/plain;charset=utf-8,' + encodeURIComponent(text));
    pom.setAttribute('download', filename);
    if (document.createEvent) {
        var event = document.createEvent('MouseEvents');
        event.initEvent('click', true, true);
        pom.dispatchEvent(event);
    }
    else {
        pom.click();
    }
}
function Base64ToBytes(base64) {
    var s = window.atob(base64);
    var bytes = new Uint8Array(s.length);
    for (var i = 0; i < s.length; i++) {
        bytes[i] = s.charCodeAt(i);
    }
    return bytes;
};
var loadeventEinvoice = {

    exportXML: function (url) {
        $.ajax({
            type: "GET",
            url: url,
            success: function (res) {
                if (!res.isValid) {
                    return;
                }
                savefile(res.data, "data.xml");
            },
            error: function () {
                alert("Error occured!!");
            }
        });
    },
    exportPDF: function (url) {
        $.ajax({
            type: "POST",
            url: url,
            //data: {id:id},
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (r) {

                //Convert Base64 string to Byte Array.
                if (!r.isValid) {
                    return;
                }
                var bytes = Base64ToBytes(r.base64);
                //Convert Byte Array to BLOB.
                var blob = new Blob([bytes], { type: "application/octetstream" });

                //Check the Browser type and download the File.
                var isIE = false || !!document.documentMode;
                if (isIE) {
                    window.navigator.msSaveBlob(blob, fileName);
                } else {
                    var url = window.URL || window.webkitURL;
                    link = url.createObjectURL(blob);
                    var a = $("<a id='downloadfile' />");
                    a.attr("download", "invoice.pdf");
                    a.attr("href", link);
                    $("body").append(a);
                    a[0].click();
                    $("body").remove(a);
                    $("body").find("a#downloadfile").remove();
                }
            }
        });
    },
    printInvoice: function (url) {
        $.ajax({
            type: 'GET',
            async: true,
            url: url,
            data: {
                // secret : idinvoice
            },
            success: function (res) {
                if (res.isValid) {
                    printDiv(res.html);


                    //dataObject = {};
                    //dataObject.type = TypeEventWebSocket.PrintInvoice;
                    //dataObject.html = res.html;
                    //loadingStart();
                    //sposvietplugin.sendConnectSocket(listport[0]).then(function (data) {
                    //    console.log(data);
                    //    sposvietplugin.connectSignatureWebSocket(listport[0], JSON.stringify(dataObject)).then(function (data) {
                    //        loadingStop();
                    //        if (data == "-1") {
                    //            toastrcus.error("Có lỗi xảy ra");
                    //        }
                    //    });
                    //});

                }
            },
            error: function (err) {
                console.log(err)
            }
        });
    },
    viewInvoice: function (url) {
        $.ajax({
            type: 'GET',
            //global: false,
            url: url,
            // data: fomrdata,
            contentType: false,
            processData: false,
            success: function (res) {
                if (res.isValid) {
                    Swal.fire({
                        // icon: 'success',
                        position: 'top-end',
                        showClass: {
                            popup: `
                              popup-formcreate
                              popup-lg
                               animate__animated
                              animate__fadeInRight
                              animate__faster
                            `
                        },
                        hideClass: {
                            popup: "popup-formcreate popup-lg animate__animated animate__fadeOutRight animate__faster"
                        },
                        showCloseButton: true,

                        title: "Chi tiết hóa đơn điện tử",
                        html: res.html,
                        //showClass: {
                        //    popup: 'popup-formcreate'
                        //},

                        footer: "<button class='btn btn-warning btn-continue mr-3'><i class='fas fa-times mr-2'></i>Đóng</button><button type='button' class='btn btn-info btn-print mr-2'><i class='fas fa-print mr-2'></i> In hóa đơn</button>",
                        allowOutsideClick: true,
                        showConfirmButton: false,
                        showCancelButton: false,
                        cancelButtonText: '<i class="fa fa-window-close"></i> Đóng',
                        didRender: () => {
                            var idinvoice = $("#invoiceid").val();
                            let status = $("#Status").val();
                            var TypeEventInvoice = EnumTypeEventInvoice.Cancel;
                            let title = "Bạn có chắc chắn muốn xóa đơn không?";
                            if (status == "HUY_BO") {
                                TypeEventInvoice = EnumTypeEventInvoice.Restore;
                                title = "Bạn có chắc chắn muốn khôi phục đơn không?";
                                $(".btn-cancelinvoice").html("<i class='fas fa-undo mr-2'> Khôi phục hóa đơn");
                            }
                            $(".btn-continue").click(function () {
                                Swal.close();
                            });
                            //$(".btn-cancelinvoice").click(async function () {
                            //    eventInvocie.CancelInvoice(idinvoice, TypeEventInvoice);
                            //});javascript:void(0)
                            console.log(res.fkey);
                            $(".btn-print").click(function () {

                                $.ajax({
                                    type: 'GET',
                                    async: true,
                                    url: '/Selling/EInvoice/PrintInvoice?id=' + res.fkey,
                                    data: {
                                        // secret : idinvoice
                                    },
                                    success: function (res) {
                                        if (res.isValid) {
                                            Swal.close();
                                            printDiv(res.html);
                                        }
                                    },
                                    error: function (err) {
                                        console.log(err)
                                    }
                                });
                            });

                        }
                    });
                }
            },
            error: function (err) {
                console.log(err)
            }
        });
    },
    loadSupllerEinvoice: function (idselectd) {
        $.ajax({
            global: true,
            type: "GET",
            url: "/API/Handling/GetAllSupplierEInvoice",
            // async: true,
            data: {
                idselectd: idselectd
            },
            success: function (data) {
                var arr = JSON.parse(data);
                //arr.push("");
                $("#supplereinvoice").select2({
                    data: JSON.parse(data),
                    placeholder: "Chọn giá trị",
                    allowClear: true,
                    language: {
                        noResults: function () {
                            return "Không tìm thấy dữ liệu";
                        }
                    },
                }).on('select2:select', function (e) {
                    $(".btn-sendCQT").data("typesignature", e.params.data.type)
                    // console.log(data.type);
                }).on('select2:clear', function (e) {
                    toastrcus.error("Vui lòng chọn nhà cung cấp");
                    $(".btn-sendCQT").data("typesignature", -1)
                });
                let datas = null;
                arr.map(function (ele, ind) {
                    if (ind == 0) {
                        datas = ele;
                    }
                    if (ele.selected) {
                        datas = ele;
                    }
                })
                $('#supplereinvoice').trigger({
                    type: 'select2:select',
                    params: {
                        data: datas
                    }
                });

            }
        });
    },
    loadEventbtnaction: function () {

        $(".lstbtnaction .btn-printeinvoice:first").data("id", EnumTypePrinteInvoice.Printer);
        $(".lstbtnaction .btn-printeinvoice:last").data("id", EnumTypePrinteInvoice.PrintConvert);
        $(".btn-printeinvoice").click(function () {
            let TypeEventInvoice = $(this).data("id");
            var lstid = new Array();
            var rows_selected = dataTableOut.column(0).checkboxes.selected();
            rows_selected.each(function (index, elem) {
                lstid.push(parseInt(index));
            });
            if (lstid.length > 20) {
                let htlml = "Số lượng hóa đơn của bạn chọn " + lstid.length + " quá nhiều, trình duyệt không thể tải để in được, vui lòng chọn ít hơn 20 hóa đơn";
                Swal.fire({
                    icon: 'error',
                    title: 'Thông báo',
                    html: htlml,
                    showCloseButton: true,
                    showCancelButton: true,
                    cancelButtonText: '<i class="fa fa-window-close"></i> Đã hiểu',
                });
            }
            $.ajax({
                type: 'GET',
                async: true,
                url: '/Selling/EInvoice/PrintMutiEInvoice',
                traditional: true,
                dataType: "json",
                data: {
                    lstid: lstid,
                    typePrint: TypeEventInvoice
                },
                success: function (res) {
                    if (res.isValid) {
                        printDiv(res.html);
                    }
                },
                error: function (err) {
                    console.log(err)
                }
            });

        });
        $(".btn-SyncEinvoice").click(function () {
            let type = parseInt($(this).data("type")) || 0;
            var lstid = new Array();
            var rows_selected = dataTableOut.column(0).checkboxes.selected();
            rows_selected.each(function (index, elem) {
                lstid.push(parseInt(index));
            });
            if (lstid.length == 0) {
                let htlml = "Vui lòng chọn hóa đơn";
                Swal.fire({
                    icon: 'error',
                    title: 'Thông báo',
                    html: htlml,
                    showCloseButton: true,
                    showCancelButton: true,
                    cancelButtonText: '<i class="fa fa-window-close"></i> Đã hiểu',
                });
            }
            $.ajax({
                type: 'GET',
                async: true,
                url: '/Selling/EInvoice/SyncInvoice',
                traditional: true,
                dataType: "json",
                data: {
                    lstid: lstid,
                    type: type
                },
                success: function (res) {
                    if (res.isValid) {
                        dataTableOut.ajax.reload(null, false);
                        dataTableOut.columns().checkboxes.deselectAll(true)
                        Swal.close();
                        title = "Đồng bộ hóa đơn điện tử";
                        Swal.fire({
                            // icon: 'success',
                            position: 'center',
                            showClass: {
                                popup: `popup-formcreate eventpublisinvoice`
                            },

                            showCloseButton: true,

                            title: title,
                            html: res.html,
                            //showClass: {
                            //    popup: 'popup-formcreate'
                            //},

                            // footer: "<button class='btn btn-primary btn-continue mr-3'><i class='icon-cd icon-add_cart icon'></i>Hủy bỏ</button><button class='btn btn-save btn-success'><i class='icon-cd icon-doneAll icon'></i>Lưu</button>",
                            allowOutsideClick: true,
                            showConfirmButton: false,
                            showCancelButton: true,
                            cancelButtonText: '<i class="fa fa-window-close"></i> Đóng',
                            didRender: () => {

                                $(".btn-continue").click(function () {
                                    Swal.close();
                                });
                                $("#accordionkqpubinv a.text-dark").click(function () {
                                    alert("ok");
                                });

                            }
                        });
                    }
                },
                error: function (err) {
                    console.log(err)
                }
            });

        });

        $(".btn-cancelinvoice").click(async function () {

            let title = "Bạn có chắc chắn muốn hủy hóa đơn điện tử đã chọn không?";
            await Swal.fire({
                icon: 'warning',
                title: title,
                // showDenyButton: true,
                showCancelButton: true,
                confirmButtonText: 'Đồng ý',
                cancelButtonText: 'Đóng',
                // denyButtonText: `Don't save`,
            }).then((result) => {
                /* Read more about isConfirmed, isDenied below */
                if (result.isConfirmed) {
                    var lstid = new Array();
                    var rows_selected = dataTableOut.column(0).checkboxes.selected();
                    rows_selected.each(function (index, elem) {
                        lstid.push(parseInt(index));
                    });
                    if (lstid.length == 0) {
                        let htlml = "Vui lòng chọn hóa đơn";
                        Swal.fire({
                            icon: 'error',
                            title: 'Thông báo',
                            html: htlml,
                            showCloseButton: true,
                            showCancelButton: true,
                            cancelButtonText: '<i class="fa fa-window-close"></i> Đã hiểu',
                        });
                    }
                    $.ajax({
                        type: 'GET',
                        async: true,
                        url: '/Selling/EInvoice/CancelEInvoice',
                        traditional: true,
                        dataType: "json",
                        data: {
                            lstid: lstid
                        },
                        success: function (res) {
                            if (res.isValid) {
                                dataTableOut.ajax.reload(null, false);
                                dataTableOut.columns().checkboxes.deselectAll(true)
                                Swal.close();
                                title = "Hủy hóa đơn điện tử";
                                Swal.fire({
                                    // icon: 'success',
                                    position: 'center',
                                    showClass: {
                                        popup: `popup-formcreate eventpublisinvoice`
                                    },

                                    showCloseButton: true,

                                    title: title,
                                    html: res.html,
                                    //showClass: {
                                    //    popup: 'popup-formcreate'
                                    //},

                                    // footer: "<button class='btn btn-primary btn-continue mr-3'><i class='icon-cd icon-add_cart icon'></i>Hủy bỏ</button><button class='btn btn-save btn-success'><i class='icon-cd icon-doneAll icon'></i>Lưu</button>",
                                    allowOutsideClick: true,
                                    showConfirmButton: false,
                                    showCancelButton: true,
                                    cancelButtonText: '<i class="fa fa-window-close"></i> Đóng',
                                    didRender: () => {

                                        $(".btn-continue").click(function () {
                                            Swal.close();
                                        });
                                        $("#accordionkqpubinv a.text-dark").click(function () {
                                            alert("ok");
                                        });

                                    }
                                });
                            }
                        },
                        error: function (err) {
                            console.log(err)
                        }
                    });
                }
            });
        });
        $(".btn-sendCQT").click(async function () {
            let checktypesign = $(".btn-sendCQT").data("typesignature");
            if (checktypesign == -1) {
                let htlml = "Vui lòng chọn nhà cung cấp!";
                Swal.fire({
                    icon: 'error',
                    title: 'Thông báo',
                    html: htlml,
                    showCloseButton: true,
                    showCancelButton: true,
                    cancelButtonText: '<i class="fa fa-window-close"></i> Đã hiểu',
                });
                return false;
            }
            if (parseInt(checktypesign) == ENumTypeSeri.TOKEN) {
                url = '/Selling/EInvoice/GetHashToken';
                loadeventEinvoice.GetHashToken(url);
            } else if (parseInt(checktypesign) == ENumTypeSeri.VNPTSmartCA) {
                url = '/Selling/EInvoice/GetHashSmartCA';
                loadeventEinvoice.GetHashToken(url);
            } else {
                loadeventEinvoice.SendCQT(ENumTypeSeri.HSM);
            }
        });
        $(".btn-removeinvoice").click(async function () {

            let title = "Bạn có chắc chắn muốn xóa hóa đơn tạo mới đã chọn không?";
            await Swal.fire({
                icon: 'warning',
                title: title,
                // showDenyButton: true,
                showCancelButton: true,
                confirmButtonText: 'Đồng ý',
                cancelButtonText: 'Đóng',
                // denyButtonText: `Don't save`,
            }).then((result) => {
                /* Read more about isConfirmed, isDenied below */
                if (result.isConfirmed) {
                    var lstid = new Array();
                    var rows_selected = dataTableOut.column(0).checkboxes.selected();
                    rows_selected.each(function (index, elem) {
                        lstid.push(parseInt(index));
                    });
                    if (lstid.length == 0) {
                        let htlml = "Vui lòng chọn hóa đơn";
                        Swal.fire({
                            icon: 'error',
                            title: 'Thông báo',
                            html: htlml,
                            showCloseButton: true,
                            showCancelButton: true,
                            cancelButtonText: '<i class="fa fa-window-close"></i> Đã hiểu',
                        });
                    }
                    $.ajax({
                        type: 'GET',
                        async: true,
                        url: '/Selling/EInvoice/RemoveEInvoice',
                        traditional: true,
                        dataType: "json",
                        data: {
                            lstid: lstid
                        },
                        success: function (res) {
                            if (res.isValid) {
                                dataTableOut.ajax.reload(null, false);
                                dataTableOut.columns().checkboxes.deselectAll(true)
                                Swal.close();
                                title = "Hủy hóa đơn điện tử";
                                Swal.fire({
                                    // icon: 'success',
                                    position: 'center',
                                    showClass: {
                                        popup: `popup-formcreate eventpublisinvoice`
                                    },

                                    showCloseButton: true,

                                    title: title,
                                    html: res.html,
                                    //showClass: {
                                    //    popup: 'popup-formcreate'
                                    //},

                                    // footer: "<button class='btn btn-primary btn-continue mr-3'><i class='icon-cd icon-add_cart icon'></i>Hủy bỏ</button><button class='btn btn-save btn-success'><i class='icon-cd icon-doneAll icon'></i>Lưu</button>",
                                    allowOutsideClick: true,
                                    showConfirmButton: false,
                                    showCancelButton: true,
                                    cancelButtonText: '<i class="fa fa-window-close"></i> Đóng',
                                    didRender: () => {

                                        $(".btn-continue").click(function () {
                                            Swal.close();
                                        });
                                        $("#accordionkqpubinv a.text-dark").click(function () {
                                            alert("ok");
                                        });

                                    }
                                });
                            }
                        },
                        error: function (err) {
                            console.log(err)
                        }
                    });
                }
            });
        });
        $(".btn-publishEinvoice").click(function () {
            var lstid = new Array();
            var rows_selected = dataTableOut.column(0).checkboxes.selected();
            rows_selected.each(function (index, elem) {
                lstid.push(parseInt(index));
            });
            $.ajax({
                type: 'POST',
                url: '/Selling/EInvoice/PublishEInvoice',
                traditional: true,
                dataType: "json",
                data: {
                    lstid: lstid
                },
                success: function (res) {

                    if (res.isValid) {
                        dataTableOut.ajax.reload(null, false);

                        Swal.close();
                        title = "Phát hành hóa đơn điện tử";
                        Swal.fire({
                            // icon: 'success',
                            position: 'center',
                            showClass: {
                                popup: `popup-formcreate eventpublisinvoice`
                            },

                            showCloseButton: true,

                            title: title,
                            html: res.html,
                            //showClass: {
                            //    popup: 'popup-formcreate'
                            //},

                            // footer: "<button class='btn btn-primary btn-continue mr-3'><i class='icon-cd icon-add_cart icon'></i>Hủy bỏ</button><button class='btn btn-save btn-success'><i class='icon-cd icon-doneAll icon'></i>Lưu</button>",
                            allowOutsideClick: true,
                            showConfirmButton: false,
                            showCancelButton: true,
                            cancelButtonText: '<i class="fa fa-window-close"></i> Đóng',
                            didRender: () => {

                                $(".btn-continue").click(function () {
                                    Swal.close();
                                });
                                $("#accordionkqpubinv a.text-dark").click(function () {
                                    alert("ok");
                                });

                            }
                        });
                    }
                },

                error: function (err) {
                    console.log(err)
                }
            });
        });

    },
    loadDashboardEinvoice: function () {
        $.ajax({
            type: 'GET',
            async: true,
            url: "/Selling/EInvoice/DashboardEinvoice",
            data: {
                // secret : idinvoice
            },
            success: function (res) {
                if (res.isValid) {
                    $(".dashboardEinvoice .SignedInv").html(res.data.signedInv);
                    $(".dashboardEinvoice .UnSendInv").html(res.data.unSendInv);
                    $(".dashboardEinvoice .SentInv").html(res.data.sentInv);
                    $(".dashboardEinvoice .AcceptedInv").html(res.data.acceptedInv);
                    $(".dashboardEinvoice .RejectedInv").html(res.data.rejectedInv);
                }
            },
            error: function (err) {
                console.log(err)
            }
        });
    },
    GetHashToken: async function (url) {
        let title = "Bạn có chắc chắn muốn gửi hóa đơn đã chọn lên cơ quản thuế không?";
        await Swal.fire({
            icon: 'warning',
            title: title,
            // showDenyButton: true,
            showCancelButton: true,
            confirmButtonText: 'Đồng ý',
            cancelButtonText: 'Đóng',
            // denyButtonText: `Don't save`,
        }).then((result) => {
            /* Read more about isConfirmed, isDenied below */
            if (result.isConfirmed) {
                var lstid = new Array();
                var rows_selected = dataTableOut.column(0).checkboxes.selected();
                rows_selected.each(function (index, elem) {
                    lstid.push(parseInt(index));
                });

                if (lstid.length == 0) {
                    let htlml = "Vui lòng chọn hóa đơn";
                    Swal.fire({
                        icon: 'error',
                        title: 'Thông báo',
                        html: htlml,
                        showCloseButton: true,
                        showCancelButton: true,
                        cancelButtonText: '<i class="fa fa-window-close"></i> Đã hiểu',
                    });
                }
                $.ajax({
                    type: 'GET',
                    async: true,
                    url: url,
                    traditional: true,
                    dataType: "json",
                    data: {
                        lstid: lstid,
                    },
                    success: function (res) {

                        if (res.isValid) {
                            Swal.close();
                            loadingStart();
                            dataObject = {};
                            dataObject.type = TypeEventWebSocket.SignEInvoice;
                            dataObject.hash = res.hash;

                            sposvietplugin.sendConnectSocket(listport[0]).then(function () {
                                loadingStart();
                                sposvietplugin.connectSignatureWebSocket(listport[0], JSON.stringify(dataObject)).then(function (data) {
                                    if (data == "-1") {
                                        toastrcus.error("Có lỗi xảy ra");
                                    } else {
                                        loadeventEinvoice.SendCQTToken(lstid, data);
                                    }
                                });
                            });

                        }
                    },
                    error: function (err) {
                        console.log(err)
                    }
                });
            }
        });

    },
    SendCQTToken: function (lstid, data) {
        $.ajax({
            type: 'GET',
            async: true,
            url: '/Selling/EInvoice/SendCQTToken',
            traditional: true,
            dataType: "json",
            data: {
                lstid: lstid,
                data: data
            },
            success: function (res) {
                loadingStop();
                if (res.isValid) {
                    dataTableOut.ajax.reload(null, false);
                    Swal.close();
                    title = "Kết quả gửi CQT";
                    Swal.fire({
                        // icon: 'success',
                        position: 'center',
                        showClass: {
                            popup: `popup-formcreate eventpublisinvoice`
                        },

                        showCloseButton: true,

                        title: title,
                        html: res.html,
                        //showClass: {
                        //    popup: 'popup-formcreate'
                        //},

                        // footer: "<button class='btn btn-primary btn-continue mr-3'><i class='icon-cd icon-add_cart icon'></i>Hủy bỏ</button><button class='btn btn-save btn-success'><i class='icon-cd icon-doneAll icon'></i>Lưu</button>",
                        allowOutsideClick: true,
                        showConfirmButton: false,
                        showCancelButton: true,
                        cancelButtonText: '<i class="fa fa-window-close"></i> Đóng',
                        didRender: () => {

                            $(".btn-continue").click(function () {
                                Swal.close();
                            });
                            $("#accordionkqpubinv a.text-dark").click(function () {
                                alert("ok");
                            });

                        }
                    });
                }
            },
            error: function (err) {
                console.log(err)
            }
        });
    },
    SendCQT: async function (checktypesign) {
        let title = "Bạn có chắc chắn muốn gửi hóa đơn đã chọn lên cơ quản thuế không?";
        await Swal.fire({
            icon: 'warning',
            title: title,
            // showDenyButton: true,
            showCancelButton: true,
            confirmButtonText: 'Đồng ý',
            cancelButtonText: 'Đóng',
            // denyButtonText: `Don't save`,
        }).then((result) => {
            /* Read more about isConfirmed, isDenied below */
            if (result.isConfirmed) {
                var lstid = new Array();
                var rows_selected = dataTableOut.column(0).checkboxes.selected();
                rows_selected.each(function (index, elem) {
                    lstid.push(parseInt(index));
                });
                if (lstid.length == 0) {
                    let htlml = "Vui lòng chọn hóa đơn";
                    Swal.fire({
                        icon: 'error',
                        title: 'Thông báo',
                        html: htlml,
                        showCloseButton: true,
                        showCancelButton: true,
                        cancelButtonText: '<i class="fa fa-window-close"></i> Đã hiểu',
                    });
                }
                $.ajax({
                    type: 'GET',
                    async: true,
                    url: '/Selling/EInvoice/SendCQT',
                    traditional: true,
                    dataType: "json",
                    data: {
                        lstid: lstid,
                        // typesignature: checktypesign
                    },
                    success: function (res) {
                        if (res.isValid) {
                            dataTableOut.ajax.reload(null, false);
                            Swal.close();
                            title = "Kết quả gửi CQT";
                            Swal.fire({
                                // icon: 'success',
                                position: 'center',
                                showClass: {
                                    popup: `popup-formcreate eventpublisinvoice`
                                },

                                showCloseButton: true,

                                title: title,
                                html: res.html,
                                //showClass: {
                                //    popup: 'popup-formcreate'
                                //},

                                // footer: "<button class='btn btn-primary btn-continue mr-3'><i class='icon-cd icon-add_cart icon'></i>Hủy bỏ</button><button class='btn btn-save btn-success'><i class='icon-cd icon-doneAll icon'></i>Lưu</button>",
                                allowOutsideClick: true,
                                showConfirmButton: false,
                                showCancelButton: true,
                                cancelButtonText: '<i class="fa fa-window-close"></i> Đóng',
                                didRender: () => {

                                    $(".btn-continue").click(function () {
                                        Swal.close();
                                    });
                                    $("#accordionkqpubinv a.text-dark").click(function () {
                                        alert("ok");
                                    });

                                }
                            });
                        }
                    },
                    error: function (err) {
                        console.log(err)
                    }
                });
            }
        });


    }

}
var eventInvocie = {
    loadEventMutiaction: function () {
        $(".lstbtnaction .btn-publishinvoice").data("id", EnumTypeEventInvoice.PublishEInvoice);
        $(".lstbtnaction .btn-createinvoice").data("id", EnumTypeEventInvoice.CreateEInvoice);
        $(".lstbtnaction .btn-publishinvoice").click(async function () {
            let TypeEventInvoice = EnumTypeEventInvoice.PublishEInvoice;
            $.ajax({
                type: 'GET',
                //global: false,
                url: "/Selling/Invoice/SuppliersEInvoice",
                // data: fomrdata,
                contentType: false,
                processData: false,
                success: function (res) {
                    if (res.isValid) {
                        footer = "<button class='btn btn-primary btn-continue mr-3'><i class='icon-cd icon-add_cart icon'></i>Hủy bỏ</button><button class='btn btn-save btn-success'><i class='icon-cd icon-doneAll icon'></i>Lưu</button>";
                        if (!res.nodata) {
                            footer = "<button class='btn btn-primary btn-continue mr-3'><i class='icon-cd icon-add_cart icon'></i>Đã hiểu</button>";
                        }
                        Swal.fire({
                            // icon: 'success',
                            title: res.title,
                            html: res.html,
                            showClass: {
                                popup: 'popup-formcreate'
                            },

                            footer: footer,
                            allowOutsideClick: true,
                            showConfirmButton: false,
                            showCancelButton: false,
                            cancelButtonText: '<i class="fa fa-window-close"></i> Đóng',
                            didRender: () => {
                                $(".btn-continue").click(function () {
                                    Swal.close();
                                });
                                $('#ManagerPatternEInvoices').select2({
                                    placeholder: {
                                        id: '', // the value of the option
                                        text: "Chọn mẫu số và ký hiệu hóa đơn"
                                    },
                                    allowClear: true,
                                    language: {
                                        noResults: function () {
                                            return "Không tìm thấy dữ liệu";
                                        }
                                    },
                                })
                                $('#Vatrate').select2({
                                    placeholder: {
                                        id: '', // the value of the option
                                        text: "Chọn thuế suất"
                                    },
                                    allowClear: true,
                                    language: {
                                        noResults: function () {
                                            return "Không tìm thấy dữ liệu";
                                        }
                                    },
                                })
                                $(".btn-save").click(function () {
                                    let idpatern = $(".selectSupplerInoice").find("#ManagerPatternEInvoices").val();
                                    let Vatrate = $(".selectSupplerInoice").find("#Vatrate").val();
                                    if (idpatern == "") {
                                        toastrcus.error("Vui lòng chọn mẫu số ký hiệu hóa đơn");
                                        return;
                                    }
                                    if (Vatrate == "") {
                                        toastrcus.error("Vui lòng chọn thuế suất");
                                        return;
                                    }
                                    var lstid = new Array();
                                    var rows_selected = dataTableOut.column(0).checkboxes.selected();
                                    rows_selected.each(function (index, elem) {
                                        // console.log(index);
                                        lstid.push(parseInt(index));
                                    });
                                    if (lstid.length == 0) {
                                        let htlml = "Vui lòng chọn hóa đơn";
                                        Swal.fire({
                                            icon: 'error',
                                            title: 'Thông báo',
                                            html: htlml,
                                            showCloseButton: true,
                                            showCancelButton: true,
                                            cancelButtonText: '<i class="fa fa-window-close"></i> Đã hiểu',
                                        });
                                        return;
                                    }

                                    $.ajax({
                                        type: 'POST',
                                        url: '/Selling/Invoice/PublishEInvoice',
                                        traditional: true,
                                        dataType: "json",
                                        data: {
                                            lstid: lstid,
                                            idPattern: idpatern,
                                            Vatrate: Vatrate,
                                            TypeEventInvoice: TypeEventInvoice,
                                        },
                                        success: function (res) {

                                            dataTableOut.ajax.reload(null, false);

                                            if (res.isValid) {
                                                Swal.close();
                                                title = "Tạo mới và phát hành hóa đơn điện tử";
                                                Swal.fire({
                                                    // icon: 'success',
                                                    position: 'center',
                                                    showClass: {
                                                        popup: `popup-formcreate eventpublisinvoice`
                                                    },

                                                    showCloseButton: true,

                                                    title: title,
                                                    html: res.html,
                                                    //showClass: {
                                                    //    popup: 'popup-formcreate'
                                                    //},

                                                    // footer: "<button class='btn btn-primary btn-continue mr-3'><i class='icon-cd icon-add_cart icon'></i>Hủy bỏ</button><button class='btn btn-save btn-success'><i class='icon-cd icon-doneAll icon'></i>Lưu</button>",
                                                    allowOutsideClick: true,
                                                    showConfirmButton: false,
                                                    showCancelButton: true,
                                                    cancelButtonText: '<i class="fa fa-window-close"></i> Đóng',
                                                    didRender: () => {

                                                        $(".btn-continue").click(function () {
                                                            Swal.close();
                                                        });
                                                        $("#accordionkqpubinv a.text-dark").click(function () {
                                                            alert("ok");
                                                        });

                                                    }
                                                });
                                            }
                                        },

                                        error: function (err) {
                                            console.log(err)
                                        }
                                    });
                                });

                            }
                        });
                    }
                },
                error: function (err) {
                    console.log(err)
                }
            });
        });
        $(".lstbtnaction .btn-createinvoice").click(async function () {
            let TypeEventInvoice = EnumTypeEventInvoice.CreateEInvoice;
            $.ajax({
                type: 'GET',
                //global: false,
                url: "/Selling/Invoice/SuppliersEInvoice",
                // data: fomrdata,
                contentType: false,
                processData: false,
                success: function (res) {
                    if (res.isValid) {
                        footer = "<button class='btn btn-primary btn-continue mr-3'><i class='icon-cd icon-add_cart icon'></i>Hủy bỏ</button><button class='btn btn-save btn-success'><i class='icon-cd icon-doneAll icon'></i>Lưu</button>";
                        if (!res.nodata) {
                            footer = "<button class='btn btn-primary btn-continue mr-3'><i class='icon-cd icon-add_cart icon'></i>Đã hiểu</button>";
                        }
                        Swal.fire({
                            // icon: 'success',
                            title: res.title,
                            html: res.html,
                            showClass: {
                                popup: 'popup-formcreate'
                            },

                            footer: footer,
                            allowOutsideClick: true,
                            showConfirmButton: false,
                            showCancelButton: false,
                            cancelButtonText: '<i class="fa fa-window-close"></i> Đóng',
                            didRender: () => {
                                $(".btn-continue").click(function () {
                                    Swal.close();
                                });
                                $('#ManagerPatternEInvoices').select2({
                                    placeholder: {
                                        id: '', // the value of the option
                                        text: "Chọn mẫu số và ký hiệu hóa đơn"
                                    },
                                    allowClear: true,
                                    language: {
                                        noResults: function () {
                                            return "Không tìm thấy dữ liệu";
                                        }
                                    },
                                })
                                $('#Vatrate').select2({
                                    placeholder: {
                                        id: '', // the value of the option
                                        text: "Chọn mẫu số và ký hiệu hóa đơn"
                                    },
                                    allowClear: true,
                                    language: {
                                        noResults: function () {
                                            return "Không tìm thấy dữ liệu";
                                        }
                                    },
                                })
                                $(".btn-save").click(function () {
                                    let idpatern = $(".selectSupplerInoice").find("#ManagerPatternEInvoices").val();
                                    let Vatrate = $(".selectSupplerInoice").find("#Vatrate").val();
                                    if (idpatern == "") {
                                        toastrcus.error("Vui lòng chọn mẫu số ký hiệu hóa đơn");
                                        return;
                                    }
                                    if (Vatrate == "") {
                                        toastrcus.error("Vui lòng chọn thuế suất");
                                        return;
                                    }
                                    var lstid = new Array();
                                    var rows_selected = dataTableOut.column(0).checkboxes.selected();
                                    rows_selected.each(function (index, elem) {
                                        // console.log(index);
                                        lstid.push(parseInt(index));
                                    });
                                    if (lstid.length == 0) {
                                        let htlml = "Vui lòng chọn hóa đơn";
                                        Swal.fire({
                                            icon: 'error',
                                            title: 'Thông báo',
                                            html: htlml,
                                            showCloseButton: true,
                                            showCancelButton: true,
                                            cancelButtonText: '<i class="fa fa-window-close"></i> Đã hiểu',
                                        });
                                        return;
                                    }

                                    $.ajax({
                                        type: 'POST',
                                        url: '/Selling/Invoice/PublishEInvoice',
                                        traditional: true,
                                        dataType: "json",
                                        data: {
                                            lstid: lstid,
                                            idPattern: idpatern,
                                            Vatrate: Vatrate,
                                            TypeEventInvoice: TypeEventInvoice,
                                        },
                                        success: function (res) {

                                            dataTableOut.ajax.reload(null, false);

                                            if (res.isValid) {
                                                Swal.close();
                                                title = "Tạo mới và phát hành hóa đơn điện tử";
                                                Swal.fire({
                                                    // icon: 'success',
                                                    position: 'center',
                                                    showClass: {
                                                        popup: `popup-formcreate eventpublisinvoice`
                                                    },

                                                    showCloseButton: true,

                                                    title: title,
                                                    html: res.html,
                                                    //showClass: {
                                                    //    popup: 'popup-formcreate'
                                                    //},

                                                    // footer: "<button class='btn btn-primary btn-continue mr-3'><i class='icon-cd icon-add_cart icon'></i>Hủy bỏ</button><button class='btn btn-save btn-success'><i class='icon-cd icon-doneAll icon'></i>Lưu</button>",
                                                    allowOutsideClick: true,
                                                    showConfirmButton: false,
                                                    showCancelButton: true,
                                                    cancelButtonText: '<i class="fa fa-window-close"></i> Đóng',
                                                    didRender: () => {

                                                        $(".btn-continue").click(function () {
                                                            Swal.close();
                                                        });
                                                        $("#accordionkqpubinv a.text-dark").click(function () {
                                                            alert("ok");
                                                        });

                                                    }
                                                });
                                            }
                                        },

                                        error: function (err) {
                                            console.log(err)
                                        }
                                    });
                                });

                            }
                        });
                    }
                    else {
                        Swal.close();
                    }
                    
                },
                error: function (err) {
                    console.log(err)
                }
            });
        });
        $(".btn-cancelinvoice").click(async function () {
            let title = "Bạn có chắc chắn muốn hủy hóa đơn không?";
            var { value: note } = await Swal.fire({
                icon: 'warning',
                title: title,
                // showDenyButton: true,
                showCancelButton: true,
                confirmButtonText: 'Đồng ý',
                input: 'text',
                inputPlaceholder: 'Vui lòng nhập lý do',
                cancelButtonText: 'Đóng',
                // denyButtonText: `Don't save`,
                inputValidator: (value) => {
                    if (!value) {
                        return 'Vui lòng nhập lý do!'
                    }
                }
            }).then(async (result) => {
                /* Read more about isConfirmed, isDenied below */
                if (result.isConfirmed) {
                    var lstid = new Array();
                    var rows_selected = dataTableOut.column(0).checkboxes.selected();
                    rows_selected.each(function (index, elem) {
                        // console.log(index);
                        lstid.push(parseInt(index));
                    });
                    var notevalue = result.value;
                    await Swal.fire({
                        icon: 'warning',
                        title: "Bạn có muốn hủy phiếu thu liên quan không!",
                        // showDenyButton: true,
                        showCancelButton: true,
                        confirmButtonText: 'Đồng ý',
                        cancelButtonText: 'Không, chỉ hủy hóa đơn',
                    }).then((result2) => {
                        let DeletePT = false;
                        if (result2.isConfirmed) {
                            DeletePT = true;
                        }
                        $.ajax({
                            type: 'POST',
                            //global: false,
                            url: '/Selling/Invoice/CancelInvoiceList',
                            data: {
                                lstid: lstid,
                                Note: notevalue,
                                IsDeletePT: DeletePT,
                            },
                            success: function (res) {
                                if (res.isValid) {
                                    dataTableOut.ajax.reload(null, false);
                                    Swal.close();
                                    title = "Hủy hóa đơn";
                                    Swal.fire({
                                        // icon: 'success',
                                        position: 'center',
                                        showClass: {
                                            popup: `popup-formcreate eventpublisinvoice`
                                        },

                                        showCloseButton: true,

                                        title: title,
                                        html: res.html,
                                        //showClass: {
                                        //    popup: 'popup-formcreate'
                                        //},

                                        // footer: "<button class='btn btn-primary btn-continue mr-3'><i class='icon-cd icon-add_cart icon'></i>Hủy bỏ</button><button class='btn btn-save btn-success'><i class='icon-cd icon-doneAll icon'></i>Lưu</button>",
                                        allowOutsideClick: true,
                                        showConfirmButton: false,
                                        showCancelButton: true,
                                        cancelButtonText: '<i class="fa fa-window-close"></i> Đóng',
                                        didRender: () => {

                                            $(".btn-continue").click(function () {
                                                Swal.close();
                                            });
                                            $("#accordionkqpubinv a.text-dark").click(function () {
                                                alert("ok");
                                            });

                                        }
                                    });
                                }
                            },
                            error: function (err) {
                                console.log(err)
                            }
                        });
                    })
                }
            })
        });
        //CHẠY KIỂM TRA VÀ HIỂN THỊ BUTTON XÓA
        $.ajax({
            type: "GET",
            url: "/Selling/ConfigSaleParameters/GetConfigSell?key=" + getObjectKey(EnumConfigParameters, EnumConfigParameters.DELETEINVOICENOPAYMENT),
            success: function (res) {
                if (res.isValid) {
                    if (res.data.value == "true") {
                        let checkSavereport = 0;//check xem có lưu báo cáo không
                        let key = getObjectKey(EnumConfigParameters, EnumConfigParameters.ACCEPTPAYMENTDELETEINVOICE);
                        res.data.configSystems.forEach(function (item, index) {
                            if (key.toLocaleUpperCase() == item.key.toLocaleUpperCase()) {
                                if (item.value == "false") {
                                    checkSavereport = 1;
                                }
                            }
                        })
                        html = '<button type="button" disabled data-removereport="' + checkSavereport + '" class="btn btn-danger btn-deleteInvoice" data-toggle="tooltip" data-placement="top" title="Tính năng xóa hóa đơn khỏi hệ thống">Xóa hóa đơn</button>';
                        $(".btn-cancelinvoice").after(html);
                        $(".btn-deleteInvoice").click(async function () {
                            let title = "Bạn có chắc chắn muốn xóa hóa đơn khỏi hệ thống không?";
                            let content = "Bạn đang cấu hình <b class='text-primary'> ghi nhận doanh thu </b> cho các hóa đơn này!";
                            if (checkSavereport == 1) {
                                content = "Lưu ý: bạn đang cấu hình <b class='text-danger'> xóa hóa đơn nhưng không ghi nhận doanh thu</b>, bạn có chắc chắn muốn tiếp tục không?";
                            }
                            var { value: note } = await Swal.fire({
                                icon: 'warning',
                                title: title,
                                html: content,
                                // showDenyButton: true,
                                showCancelButton: true,
                                confirmButtonText: 'Đồng ý',
                                input: 'text',
                                inputPlaceholder: 'Bạn hãy cho biết lý do xóa hóa đơn?',
                                cancelButtonText: 'Đóng',
                                // denyButtonText: `Don't save`,
                                inputValidator: (value) => {
                                    if (!value) {
                                        return 'Vui lòng nhập lý do!'
                                    }
                                }
                            }).then((result) => {

                                /* Read more about isConfirmed, isDenied below */
                                if (result.isConfirmed) {
                                    var lstid = new Array();
                                    var rows_selected = dataTableOut.column(0).checkboxes.selected();
                                    rows_selected.each(function (index, elem) {
                                        // console.log(index);
                                        lstid.push(parseInt(index));
                                    });

                                    $.ajax({
                                        type: 'POST',
                                        //global: false,
                                        url: '/Selling/Invoice/DeleteInvoiceList',
                                        data: {
                                            lstid: lstid,
                                            Note: result.value,
                                            IsDelete: (checkSavereport == 1 ? true : false),
                                        },
                                        success: function (res) {
                                            if (res.isValid) {
                                                dataTableOut.ajax.reload(null, false);
                                                Swal.close();
                                                title = "Hủy hóa đơn";
                                                Swal.fire({
                                                    // icon: 'success',
                                                    position: 'center',
                                                    showClass: {
                                                        popup: `popup-formcreate eventpublisinvoice`
                                                    },

                                                    showCloseButton: true,

                                                    title: title,
                                                    html: res.html,
                                                    //showClass: {
                                                    //    popup: 'popup-formcreate'
                                                    //},

                                                    // footer: "<button class='btn btn-primary btn-continue mr-3'><i class='icon-cd icon-add_cart icon'></i>Hủy bỏ</button><button class='btn btn-save btn-success'><i class='icon-cd icon-doneAll icon'></i>Lưu</button>",
                                                    allowOutsideClick: true,
                                                    showConfirmButton: false,
                                                    showCancelButton: true,
                                                    cancelButtonText: '<i class="fa fa-window-close"></i> Đóng',
                                                    didRender: () => {

                                                        $(".btn-continue").click(function () {
                                                            Swal.close();
                                                        });

                                                    }
                                                });
                                            }
                                        },
                                        error: function (err) {
                                            console.log(err)
                                        }
                                    });
                                }
                            })
                        });
                    }
                }
            },
            error: function () {
                alert("Error occured!!");
            }
        });
        //chạy kiểm tra button gộp
        $.ajax({
            type: "GET",
            url: "/Selling/ConfigSaleParameters/GetConfigSell?key=" + getObjectKey(EnumConfigParameters, EnumConfigParameters.PUBLISHMERGEINVOICE),
            success: function (res) {
                if (res.isValid) {
                    if (res.data.value == "true") {
                        let checkSavereport = 0;//check xem có tự động xóa không
                        let key = getObjectKey(EnumConfigParameters, EnumConfigParameters.DELETEIPUBLISHMERGEINVOICEAFTER);
                        res.data.configSystems.forEach(function (item, index) {
                            if (key.toLocaleUpperCase() == item.key.toLocaleUpperCase()) {
                                if (item.value == "true") {
                                    checkSavereport = 1;
                                }
                            }
                        });
                        html = '<a class="dropdown-item btn-publishinvoicemerge" data-type="0" href="javascript:void(0)" data-toggle="tooltip" data-placement="top" title="Ghi chú: lựa chọn hóa đơn để phát hành gộp, nhiều hóa đơn bán hàng tương ứng một hóa đơn điện tử"><i class="fas fa-plus mr-2"></i> Phát hành gộp hóa đơn điện tử</a>';
                        $(".lst-action-buttonpublishinvoice .btn-publishinvoice").after(html);
                        $(".btn-publishinvoicemerge").click(async function () {
                            var lstid = new Array();
                            var rows_selected = dataTableOut.column(0).checkboxes.selected();
                            rows_selected.each(function (index, elem) {
                                // console.log(index);
                                lstid.push(parseInt(index));
                            });

                            htmlfooter = "<button class='btn btn-primary btn-continue mr-3'><i class='fas fa-power-off mr-2'></i>Hủy bỏ giao dịch</button><button class='btn btn-save btn-success mr-2'><i class='fas fa-save mr-2'></i>Phát hành không xóa đơn gộp</button><button class='btn btn-saveAndDelete btn-success'><i class='fas fa-share mr-2'></i>Phát hành và xóa đơn gộp</button>";
                            if (checkSavereport == 1) {
                                htmlfooter = "<button class='btn btn-primary btn-continue mr-3'><i class='fas fa-power-off mr-2'></i>Hủy bỏ giao dịch</button><button class='btn btn-save btn-success mr-2'><i class='fas fa-save mr-2'></i>Phát hành và xóa đơn gộp</button>";
                            }
                            $.ajax({
                                type: 'GET',
                                //global: false,
                                traditional: true,
                                url: '/Selling/Invoice/PublishEInvoiceMerge',
                                data: {
                                    lstid: lstid,
                                    IsDelete: (checkSavereport == 1 ? true : false),
                                },
                                success: function (res) {
                                    if (res.isValid) {
                                        dataTableOut.ajax.reload(null, false);
                                        Swal.close();
                                        title = "Phát hành hóa đơn điện tử gộp từ nhiều hóa đơn bán hàng (" + lstid.length + " hóa đơn)";
                                        Swal.fire({
                                            // icon: 'success',
                                            position: 'center',
                                            showClass: {
                                                popup: `popup-formcreate`
                                            },
                                            customClass: {
                                                container: 'eventpublisinvoice90p',
                                                //popup: 'your-popup-class',
                                                //header: 'your-header-class',
                                                //title: 'your-title-class',
                                                //closeButton: 'your-close-button-class',
                                                //icon: 'your-icon-class',
                                                //image: 'your-image-class',
                                                //content: 'your-content-class',
                                                //input: 'your-input-class',
                                                //actions: 'your-actions-class',
                                                //confirmButton: 'your-confirm-button-class',
                                                //cancelButton: 'your-cancel-button-class',
                                                footer: 'footer-Swal'
                                            },
                                            showCloseButton: true,

                                            title: title,
                                            html: res.html,
                                            //showClass: {
                                            //    popup: 'popup-formcreate'
                                            //},

                                            footer: htmlfooter,
                                            allowOutsideClick: false,
                                            showConfirmButton: false,
                                            showCancelButton: false,
                                            cancelButtonText: '<i class="fa fa-window-close"></i> Đóng',
                                            didRender: () => {
                                                evetnFormatnumber3();
                                                evetnFormatTextnumber3decimal();
                                                eventInvocie.eventChanegVATrate();//tính công thức
                                                $("#Vatrate").trigger("change");
                                                $('#ManagerPatternEInvoices').select2({
                                                    placeholder: {
                                                        id: '', // the value of the option
                                                        text: "Chọn mẫu số và ký hiệu hóa đơn"
                                                    },
                                                    allowClear: true,
                                                    language: {
                                                        noResults: function () {
                                                            return "Không tìm thấy dữ liệu";
                                                        }
                                                    },
                                                })
                                                $('#Vatrate').select2({
                                                    placeholder: {
                                                        id: '', // the value of the option
                                                        text: "Chọn thuế suất"
                                                    },
                                                    allowClear: true,
                                                    language: {
                                                        noResults: function () {
                                                            return "Không tìm thấy dữ liệu";
                                                        }
                                                    },
                                                });
                                                loaddataSelect2("/API/Handling/GetAllPaymentMethod", ".PaymentMethod", res.idPayment, "Chọn hình thức thanh toán");
                                                loadEventIcheck();
                                                eventInvocie.eventAutocomplete();
                                                eventInvocie.eventChangeKhachle();
                                                $('[data-toggle="tooltip"]').tooltip();
                                                validateForm.AddPublishInvoiceMerge();//validate nhé
                                                $('#create-formProduct .PaymentMethod').on("change", function (e) {
                                                    $(this).valid()
                                                });
                                                $(".btn-continue").click(function () {
                                                    Swal.close();
                                                });
                                                $(".btn-save").click(function () {
                                                    $("#IsDelete").val($("#IsDelete").data("value"));
                                                    eventInvocie.eventSaveInvoice();
                                                    if ($("form#create-formProduct").valid()) {
                                                        if ($("#CusName").val().trim() == "" && $("#Buyer").val().trim() == "" && $("#IsRetailCustomer").prop("checked") == false) {
                                                            toastrcus.error("Tên đơn vị mua hàng hoặc tên người hàng phải nhập một trong hai.");
                                                            return;
                                                        } else {
                                                            eventInvocie.savePublishEInvoiceMeger();
                                                        }
                                                    }
                                                });
                                                $(".btn-saveAndDelete").click(function () {
                                                    $("#IsDelete").val("true");
                                                    eventInvocie.eventSaveInvoice();
                                                    if ($("form#create-formProduct").valid()) {
                                                        if ($("#CusName").val().trim() == "" && $("#Buyer").val().trim() == "" && $("#IsRetailCustomer").prop("checked") == false) {
                                                            toastrcus.error("Tên đơn vị mua hàng hoặc tên người hàng phải nhập một trong hai.");
                                                            return;
                                                        } else {
                                                            eventInvocie.savePublishEInvoiceMeger();
                                                        }
                                                    }
                                                });

                                            }
                                        });
                                    }
                                },
                                error: function (err) {
                                    console.log(err)
                                }
                            });
                        });
                    }
                }
            },
            error: function () {
                alert("Error occured!!");
            }
        });
    },
    savePublishEInvoiceMeger: function () {
        try {
            for (instance in CKEDITOR.instances) {
                CKEDITOR.instances[instance].updateElement();
                $('#' + instance).val(CKEDITOR.instances[instance].getData());
            }
            form = $("#create-formProduct")[0];
            $.ajax({
                type: 'POST',
                url: form.action,
                data: new FormData(form),
                contentType: false,
                processData: false,
                success: function (res) {
                    if (res.isValid) {
                        dataTableOut.ajax.reload(null, false);
                        dataTableOut.columns().checkboxes.deselectAll(true);//tắt
                        $('[data-toggle="tooltip"]').tooltip();
                        title = "Phát hành gộp hóa đơn";
                        Swal.fire({
                            position: 'center',
                            showClass: {
                                popup: `popup-formcreate eventpublisinvoice`
                            },
                            showCloseButton: true,
                            title: title,
                            html: res.html,
                            allowOutsideClick: true,
                            showConfirmButton: false,
                            showCancelButton: true,
                            cancelButtonText: '<i class="fa fa-window-close"></i> Đóng',
                            didRender: () => {
                                $(".btn-continue").click(function () {
                                    Swal.close();
                                });
                            }
                        });
                    }
                },
                error: function (err) {
                    console.log(err)
                }
            })
            return false;
        } catch (ex) {
            console.log(ex)
        }
    },
    eventAutocomplete: function () {
        $("#CusName").blur(function () {
            $("#parentautocomplete").css({
                height: "0",
                top: "0",
                "z-index": "0"
            });
        });
        $("#Buyer").blur(function () {
            $("#parentautocomplete").css({
                height: "0",
                top: "0",
                "z-index": "0"
            });
        });
        $("#CusCode").blur(function () {
            $("#parentautocomplete").css({
                height: "0",
                top: "0",
                "z-index": "0"
            });
        });
        $("#Taxcode").blur(function () {
            $("#parentautocomplete").css({
                height: "0",
                top: "0",
                "z-index": "0"
            });
        });
        eventInvocie.Autocompletedata($("#CusName"), AutocompleteTypeCustomer.CUSNAME);
        eventInvocie.Autocompletedata($("#Buyer"), AutocompleteTypeCustomer.BUYER);
        eventInvocie.Autocompletedata($("#CusCode"), AutocompleteTypeCustomer.CUSCODE);
        eventInvocie.Autocompletedata($("#Taxcode"), AutocompleteTypeCustomer.TAXCODE);
    },
    Autocompletedata: function (id, type) {
        $(id).autocomplete(
            {
                appendTo: "#parentautocomplete",
                autoFocus: true,
                minLength: 3,
                delay: 0,

                source: function (request, response) {
                    $.ajax({
                        global: false,
                        url: "/Selling/Customer/SearchCustomerPos",
                        type: "GET",
                        dataType: "json",
                        data: {
                            text: request.term,
                            type: type
                        },
                        // html: true,
                        success: function (data) {
                            response($.map(data, function (item) {
                                let texthigh = "";
                                //if (item.name != "" && item.name != null) {
                                //     texthigh = __highlight(item.name, request.term);
                                //} else {
                                //    texthigh = __highlight(item.buyer, request.term);
                                //}
                                let value = "";
                                let name = "";
                                switch (type) {
                                    case AutocompleteTypeCustomer.BUYER:
                                        value = item.buyer;
                                        if (item.buyer != "" && item.buyer != null) {
                                            texthigh = __highlight(item.buyer, request.term);
                                            name += "<span>" + texthigh + "</span>";
                                            if (item.name != "" && item.name != null) {
                                                name += "<span>" + item.name + "</span>";
                                            }
                                        }
                                        break;
                                    case AutocompleteTypeCustomer.CUSNAME:
                                        value = item.name;
                                        if (item.name != "" && item.name != null) {
                                            texthigh = __highlight(item.name, request.term);
                                            name += "<span>" + texthigh + "</span>";
                                            if (item.buyer != "" && item.buyer != null) {
                                                name += "<span class='code'>Tên KH: " + item.buyer + "</span>";
                                            }
                                        }
                                        break;
                                    case AutocompleteTypeCustomer.CUSCODE:
                                        value = item.code;
                                        texthigh = __highlight(item.code, request.term);
                                        name += "<span class='code'>Mã KH: " + texthigh + " </span>";
                                        if (item.name != "" && item.name != null) {
                                            name += "<span>" + item.name + "</span>";
                                        }
                                        if (item.buyer != "" && item.buyer != null) {
                                            name += "<span>" + item.buyer + "</span>";
                                        }
                                        break;
                                    case AutocompleteTypeCustomer.TAXCODE:
                                        value = item.taxcode;
                                        texthigh = __highlight(item.taxcode, request.term);
                                        name += "<span>Mã số thuế: " + texthigh + " </span>";
                                        if (item.name != "" && item.name != null) {
                                            name += "<span>" + item.name + "</span>";
                                        }
                                        if (item.buyer != "" && item.buyer != null) {
                                            name += "<span>" + item.buyer + "</span>";
                                        }
                                        break;

                                    default:

                                }
                                if (item.address != "" && item.address != null) {
                                    name += "Địa chỉ: " + item.address + " <br/>";
                                }
                                if (type != AutocompleteTypeCustomer.TAXCODE) {
                                    if (item.taxcode != "" && item.taxcode != null) {
                                        name += " Mã số thuế: " + item.taxcode + " <br/>";
                                    }
                                }
                                if (type != AutocompleteTypeCustomer.CUSCODE) {
                                    name += "Mã KH: " + item.code + " <br/>";
                                }
                                if (item.phoneNumber != "" && item.phoneNumber != null) {
                                    name += " SĐT: " + item.phoneNumber;
                                }

                                let html =
                                    "<a href='javascript:void(0)'><div class='search-auto'>" +
                                    "<div class='img'><img src='../" + item.img + "'></div>" +
                                    "<div class='tk_name'>" + name + "</div></div></a>";

                                return {
                                    label: html,
                                    value: value,
                                    name: item.name,
                                    buyer: item.buyer,
                                    code: item.code,
                                    address: item.address,
                                    taxcode: item.taxcode,
                                    phoneNumber: item.phoneNumber,
                                    email: item.email,
                                    cccd: item.cccd,
                                    cusBankNo: item.cusBankNo,
                                    cusBankName: item.cusBankName,
                                    idCus: item.id
                                };
                            }))
                            return { label: request.term, value: request.term };
                        },
                    })

                },
                html: true,
                select: function (e, ui) {
                    eventInvocie.eventResetinput();
                    $(this).val("");
                    $("#CusName").val(ui.item.name)
                    $("#Address").val(ui.item.address)
                    $("#Buyer").val(ui.item.buyer)
                    $("#Taxcode").val(ui.item.taxcode)
                    $("#CCCD").val(ui.item.cccd)
                    $("#PhoneNumber").val(ui.item.phoneNumber)
                    $("#Email").val(ui.item.email)
                    $("#CusCode").val(ui.item.code)
                    $("#CusBankNo").val(ui.item.cusBankNo)
                    $("#CusBankName").val(ui.item.cusBankName)
                },
            }).focus(function () {
                $("#parentautocomplete").css({
                    height: "calc(100vh - 100px)",
                    top: "52px",
                    "z-index": "5"
                });

                //if (localStorage.getItem('Barcode') != 1) {
                $(this).autocomplete("search");
                $(this).select();
                //} else {
                //  $(this).select();
                // }
                //$(this).focus();
                // $(this).select()
            });
    },
    eventResetinput: function () {
        $("#CusName").val("")
        $("#Address").val("")
        $("#Buyer").val("")
        $("#Taxcode").val("")
        $("#CCCD").val("")
        $("#PhoneNumber").val("")
        $("#Email").val("")
        $("#CusCode").val("")
        $("#CusBankNo").val("")
        $("#CusBankName").val("")
    },
    eventChangeKhachle: function () {
        $('input#IsRetailCustomer').on('ifChanged', function (event) {
            if ($(this).prop("checked") == true) {
                $("#IsCreateCustomer").iCheck('uncheck');
                $('#IsCreateCustomer').iCheck('disable');

                $(".form-publishmegerinvoice").find("input").attr("disabled", true);
                $(".form-publishmegerinvoice").find("input").attr("readonly", true);
            } else if ($(this).prop("checked") == false) {
                $('#IsCreateCustomer').iCheck('enable');
                $(".form-publishmegerinvoice").find("input").removeAttr("disabled");
                $(".form-publishmegerinvoice").find("input").removeAttr("readonly");
            }
        });
        $('input#IsCreateCustomer').on('ifChanged', function (event) {
            if ($(this).prop("checked") == true) {
                if ($("#IsRetailCustomer").prop("checked")) {
                    toastrcus.error("Không thể thêm khách hàng khi bạn đã chọn là khách lẻ");
                    return false;

                }
            }
        });
    },
    eventSaveInvoice: function () {

        var Products = []; //list table style có đơn giá
        var InvoiceOlds = []; //các giá trị invoic cũ
        $(".number3").each(function (index, element) {
            let vat = parseInt($(this).val().replaceAll(",", "")) || 0;
            $(this).val(vat);
        });
        $(".codeinvoiceold").each(function (index, element) {
            let typeProductCategory = $(element).data("invoice");
            if (typeProductCategory == "") {
                alert("Các hóa đơn liên quan đã không còn tồn tại!");
                Swal.close();
                return false;
            }
            InvoiceOlds.push(typeProductCategory);
        });
        if (InvoiceOlds.length == 0) {
            alert("Các hóa đơn liên quan đã không còn tồn tại!")
            Swal.close();
            return false;
        } else {
            $("#JsonInvoiceOld").val(JSON.stringify(InvoiceOlds));
        }
        //---------------------//
        $("#tableinvoicemerge").find("tbody").find("tr").each(function (index, element) {
            let typeProductCategory = parseInt($(this).find(".name").data("typeProductCategory")) || EnumTypeProductCategory.PRODUCT;
            let code = $(this).find(".code").text();
            let name = $(this).find(".name").text();
            let unit = $(this).find(".unit").text();
            let price = $(this).find(".price").text().replaceAll(",", "") || 0;
            let quantity = $(this).find(".quantity").text().replaceAll(",", "") || 0;
            let total = $(this).find(".total").text().replaceAll(",", "") || 0;
            var Product = {}; //khởi tạo object
            Product.TypeProductCategory = typeProductCategory;
            Product.Code = code;
            Product.Unit = unit;
            Product.Price = parseFloat(price);
            Product.Total = parseFloat(total);
            Product.Quantity = parseFloat(quantity);
            if (code == "") {
                alert("Sản phẩm: " + name + " không hợp lệ!");
                Swal.close();
                return false;
            }
            Products.push(Product); //thêm gán vào array
        });
        if (Products.length == 0) {
            alert("Không tìm thấy sản phẩm, vui lòng thực hiện lại!");
            Swal.close();
            return false;
        } else {
            $("#JsonProduct").val(JSON.stringify(Products));
        }
    },
    eventChanegVATrate: function () {
        $("#Vatrate").change(function () {
            let total = parseFloat($(".Total").val().replaceAll(",", "")) || 0;
            let discountAmount = parseFloat($(".DiscountAmount").val().replaceAll(",", "")) || 0;
            if (discountAmount > 0) {
                total = total - discountAmount;
            }
            let Vatrate = parseInt($(this).val()) || 0;
            let vatamount = Math.round(total * (Vatrate / 100));
            let amount = total + vatamount;

            $(".VATAmount").val(vatamount.format0VND(0, 3));
            $(".Amount").val(amount.format0VND(0, 3));
        });
        $(".Total").keyup(function () {
            $("#Vatrate").trigger("change");
        });
        $(".DiscountAmount").keyup(function () {
            $("#Vatrate").trigger("change");
        });
        $(".VATAmount").keyup(function () {
            let total = parseFloat($(".Total").val().replaceAll(",", "")) || 0;
            let discountAmount = parseFloat($(".DiscountAmount").val().replaceAll(",", "")) || 0;
            let vatamount = parseFloat($(this).val().replaceAll(",", "")) || 0;
            let amount = total - discountAmount + vatamount;
            $(".Amount").val(amount.format0VND(0, 3));
        });
    },
    CancelInvoice: async function (idinvoice, TypeEventInvoice) {
        let title = "Bạn có chắc chắn muốn hủy hóa đơn không?";
        if (TypeEventInvoice == EnumTypeEventInvoice.Restore) {
            title = "Bạn có chắc chắn muốn khôi phục đơn không?";
        }
        const { value: note } = await Swal.fire({
            icon: 'warning',
            title: title,
            // showDenyButton: true,
            showCancelButton: true,
            confirmButtonText: 'Đồng ý',
            input: 'text',
            inputPlaceholder: 'Vui lòng nhập lý do',
            cancelButtonText: 'Đóng',
            // denyButtonText: `Don't save`,
            inputValidator: (value) => {
                if (!value) {
                    return 'Vui lòng nhập lý do!'
                }
            }
        }).then((result) => {

            /* Read more about isConfirmed, isDenied below */
            if (result.isConfirmed) {
                $.ajax({
                    type: 'POST',
                    //global: false,
                    url: '/Selling/Invoice/CancelInvoice?secret=' + idinvoice,
                    data: {
                        TypeEventInvoice: TypeEventInvoice,
                        Note: result.value,
                    },
                    success: function (res) {
                        if (res.isValid) {
                            dataTableOut.ajax.reload(null, false);
                        }
                    },
                    error: function (err) {
                        console.log(err)
                    }
                });
            }
        })

    },
    DeleteIsMerge: async function (idinvoice) {
        let title = "Bạn có chắc chắn muốn xóa các hóa đơn đã gộp cho hóa đơn này không?";
        const { value: note } = await Swal.fire({
            icon: 'warning',
            title: title,
            // showDenyButton: true,
            showCancelButton: true,
            confirmButtonText: 'Đồng ý xóa',
            cancelButtonText: 'Hủy bỏ',
            // denyButtonText: `Don't save`,
        }).then((result) => {

            /* Read more about isConfirmed, isDenied below */
            if (result.isConfirmed) {
                $.ajax({
                    type: 'POST',
                    //global: false,
                    url: '/Selling/Invoice/DeleteIsMerge?secret=' + idinvoice,
                    data: {
                    },
                    success: function (res) {
                        if (res.isValid) {
                            dataTableOut.ajax.reload(null, false);
                        }
                    },
                    error: function (err) {
                        console.log(err)
                    }
                });
            }
        })

    },
    printInvoice: function (url) {
        $.ajax({
            type: 'GET',
            async: true,
            url: url,
            data: {
                // secret : idinvoice
            },
            success: function (res) {
                if (res.isValid) {
                    printDiv(res.html);
                    //dataObject = {};
                    //dataObject.type = TypeEventWebSocket.PrintInvoice;
                    //dataObject.html = res.html;
                    //loadingStart();
                    //sposvietplugin.sendConnectSocket(listport[0]).then(function (data) {
                    //    console.log(data);
                    //    sposvietplugin.connectSignatureWebSocket(listport[0], JSON.stringify(dataObject)).then(function (data) {
                    //        loadingStop();
                    //        if (data == "-1") {
                    //            toastrcus.error("Có lỗi xảy ra");
                    //        }
                    //    });
                    //});
                }
            },
            error: function (err) {
                console.log(err)
            }
        });
    },
    viewInvoice: function (url) {
        $.ajax({
            type: 'GET',
            //global: false,
            url: url,
            // data: fomrdata,
            contentType: false,
            processData: false,
            success: function (res) {
                //if (res.isValid) {
                Swal.fire({
                    // icon: 'success',
                    position: 'top-end',
                    showClass: {
                        popup: `
                              popup-formcreate
                              popup-lg
                               animate__animated
                              animate__fadeInRight
                              animate__faster
                            `
                    },
                    hideClass: {
                        popup: "popup-formcreate popup-lg animate__animated animate__fadeOutRight animate__faster"

                    },
                    showCloseButton: true,

                    title: "Thêm/Chỉnh sửa phòng/bàn",
                    html: res,
                    //showClass: {
                    //    popup: 'popup-formcreate'
                    //},

                    footer: "<button class='btn btn-warning btn-continue mr-3'><i class='fas fa-times mr-2'></i>Đóng</button><button type='button' class='btn btn-info btn-print mr-2'><i class='fas fa-print mr-2'></i> In hóa đơn</button><button type='button' class='btn btn-danger btn-cancelinvoice'><i class='fas fa-power-off mr-2'></i>Hủy đơn</button>",
                    allowOutsideClick: true,
                    showConfirmButton: false,
                    showCancelButton: false,
                    cancelButtonText: '<i class="fa fa-window-close"></i> Đóng',
                    didRender: () => {
                        var idinvoice = $("#invoiceid").val();
                        let status = $("#Status").val();
                        var TypeEventInvoice = EnumTypeEventInvoice.Cancel;
                        let title = "Bạn có chắc chắn muốn xóa đơn không?";
                        if (status == "HUY_BO") {
                            TypeEventInvoice = EnumTypeEventInvoice.Restore;
                            title = "Bạn có chắc chắn muốn khôi phục đơn không?";
                            $(".btn-cancelinvoice").html("<i class='fas fa-undo mr-2'> Khôi phục hóa đơn");
                        }
                        $(".btn-continue").click(function () {
                            Swal.close();
                        });
                        $(".btn-cancelinvoice").click(async function () {
                            eventInvocie.CancelInvoice(idinvoice, TypeEventInvoice);
                        });

                        $(".btn-print").click(function () {

                            $.ajax({
                                type: 'GET',
                                async: true,
                                url: '/Selling/Invoice/PrintInvoice?secret=' + idinvoice,
                                data: {
                                    // secret : idinvoice
                                },
                                success: function (res) {
                                    if (res.isValid) {
                                        Swal.close();
                                        printDiv(res.html);
                                    }
                                },
                                error: function (err) {
                                    console.log(err)
                                }
                            });
                        });

                    }
                });
                //}
            },
            error: function (err) {
                console.log(err)
            }
        });
    },
}
var commonEventpost = {

    addOrEditPostOnePgae: function (url) {
        $.ajax({
            type: 'GET',
            //global: false,
            url: url,
            // data: fomrdata,
            contentType: false,
            processData: false,
            success: function (res) {
                if (res.isValid) {
                    Swal.fire({
                        // icon: 'success',
                        title: res.title,
                        html: res.html,
                        showClass: {
                            popup: 'popup-formcreate-post'
                        },

                        footer: "<button class='btn btn-primary btn-continue mr-3'><i class='icon-cd icon-add_cart icon'></i>Hủy bỏ</button><button class='btn btn-save btn-success'><i class='icon-cd icon-doneAll icon'></i>Lưu</button>",
                        allowOutsideClick: true,
                        showConfirmButton: false,
                        showCancelButton: false,
                        cancelButtonText: '<i class="fa fa-window-close"></i> Đóng',
                        didRender: () => {
                            // CKEDITOR.replace('Decription');
                            loadEventadmin.eventShowformAddPostOne();


                        }
                    });
                }
            },
            error: function (err) {
                console.log(err)
            }
        });

    }
}
var loadcentChitkent = {
    loadeventacceptAllnotify: function () {
        $("#next-allneworder").click(function () {
            let lstid = [];
            $("#dataChitkenNew .tab-pane.active .tab-Priorities li").map(function () {
                lstid.push($(this).data("id"));
            });

            $.ajax({
                type: 'POST',
                url: "/Selling/PosKitchen/NotifyOrderOrocessed",
                data: {
                    TypeNotifyKitchenOrder: _TypeNotifyKitchenOrder.Orocessed,
                    UpdateFull: true,
                    Status: StatusKitchenOrder.MOI,
                    ListIdChitken: lstid
                },
                dataType: 'json',
                success: function (res) {
                    if (res.isValid) {

                    }
                },
                error: function (err) {
                    console.log(err)
                }
            })
        });

    },
    loadeventacceptAllreadynotify: function () {

        $("#next-allreadyorder").click(function () {
            let lstid = [];
            $("#dataOrderReady .tab-pane.active .tab-Priorities li").map(function () {
                lstid.push($(this).data("id"));
            });

            $.ajax({
                type: 'POST',
                url: "/Selling/PosKitchen/NotifyOrderOrocessed",
                data: {
                    TypeNotifyKitchenOrder: _TypeNotifyKitchenOrder.Orocessed,
                    UpdateFull: true,
                    Status: StatusKitchenOrder.READY,
                    ListIdChitken: lstid
                },
                dataType: 'json',
                success: function (res) {
                    if (res.isValid) {

                    }
                },
                error: function (err) {
                    console.log(err)
                }
            })
        });
    },
    loadeventtimeago: function () {
        var dateAux;
        $("time.timeago").each(function (i, item) {
            dateAux = moment($(item).html(), 'DD-MM-YYYY hh:mm:ss');
            $(item).attr('datetime', dateAux.toISOString());
        });
        $("time.timeago").timeago();
    },
    loadDataFoodReady: async function (onload = true) {
        var loadOrder = await axios.get("/Selling/PosKitchen/DataFoodReady");
        if (loadOrder.data.isValid) {
            $("#dataOrderReady").html(loadOrder.data.data)

            let id = $(".tab-notifychitkenready a.active").attr("href");
            $("#dataOrderReady").find(".tab-pane").removeClass("active");
            $("#dataOrderReady").find(id).addClass("active");
            loadcentChitkent.loadeventtimeago();
            if (onload) {
                //load event các button và data-id
                loadcentChitkent.loadeventhideshoworderrooom($("#dataOrderReady"));
                loadcentChitkent.loadeventnextorder($("#dataOrderReady"));
                loadcentChitkent.loadeventacceptAllreadynotify();
                // $("#dataOrderReady").find(".tab-pane.active").find("li:first-child").addClass("active");
            }
        }


    },//danh sách món bếp đã làm xong chờ cung
    loadDataFoodNew: async function (onload = true) {
        // var loadOrder = await axios.get("/Selling/PosKitchen/DataFoodNew");
        $.ajax({
            type: 'GET',
            global: false,
            url: "/Selling/PosKitchen/DataFoodNew",
            dataType: 'json',
            success: function (res) {
                if (res.isValid) {
                    $("#dataChitkenNew").html(res.data);
                    let id = $(".tab-notifychitk a.active").attr("href");
                    $("#dataChitkenNew").find(".tab-pane").removeClass("active");
                    $("#dataChitkenNew").find(id).addClass("active");
                    loadcentChitkent.loadeventtimeago();
                    if (onload) {
                        //load event các button và data-id
                        loadcentChitkent.loadeventhideshoworderrooom($("#dataChitkenNew"));
                        loadcentChitkent.loadeventnextorder($("#dataChitkenNew"));
                        loadcentChitkent.loadeventacceptAllnotify();
                    }
                }
            },
            error: function (err) {
                console.log(err)
            }
        })



    },//danh sách món bếp đã làm xong chờ cung 
    loadeventhideshoworderrooom: function (sel) {
        $(sel).find(".tab-viewtable").find(".table-name").click(function () {
            if ($(this).children("i").hasClass("fa-minus")) {
                $(this).children("i").removeClass("fa-minus");
                $(this).children("i").addClass("fa-plus");
            } else {
                $(this).children("i").removeClass("fa-plus");
                $(this).children("i").addClass("fa-minus");
            }
            $(this).parents("li").children("ul").toggleClass("d-none");
        });
        //update lại data
        $(sel).find(" .item-data").map(function () {
            let iddata = $(this).data("id");
            if (iddata != "") {
                $(this).removeAttr("data-id");
                $(this).data("id", iddata);
            }
        })
    },// sự kiện của tab theo bàn bấm nút dấu cộng để show ra các item thôi nhé
    loadeventnextorder: function (sel) {
        $(sel).find(".tab-pane").find(".lst-action").find("button.btn-next").click(function () {
            $(this).attr("disabled", "disabled");
            let idchitken = $(this).parent().parent().data("id");
            let idPrduct = $(this).parent().parent().data("id");
            let upone = true;
            let updatedone = $(this).parent().parent("li").hasClass("item-ready") ? StatusKitchenOrder.DONE : StatusKitchenOrder.READY;


            if ($(this).parents(".tab-pane").hasClass("viewfood")) {
                //idPrduct = idchitken;
                idchitken = "";
                loadcentChitkent.eventNotifyOrderOrocessed(idchitken, upone, "", idPrduct, _TypeNotifyKitchenOrder.UPDATEBYFOOD, updatedone);

            } else {
                loadcentChitkent.eventNotifyOrderOrocessed(idchitken, upone, "", idPrduct, _TypeNotifyKitchenOrder.Orocessed, updatedone);
            }
            $(this).removeAttr("disabled");

        });

        $(sel).find(".tab-pane").find(".lst-action").find("button.btn-nextAll").click(function () {
            $(this).attr("disabled", "disabled");
            let idchitken = $(this).parent().parent().data("id");
            let idorder = $(this).parent().parent().data("order");
            let upone = false;
            let idPrduct = $(this).parents("li.item-product").data("id");
            let updatedone = $(this).parent().parent("li").hasClass("item-ready") ? StatusKitchenOrder.DONE : StatusKitchenOrder.READY;

            if ($(this).parents(".tab-pane").hasClass("viewfood")) {
                idchitken = "";
                loadcentChitkent.eventNotifyOrderOrocessed(idchitken, upone, "", idPrduct, _TypeNotifyKitchenOrder.UPDATEBYFOOD, updatedone);
            }
            else if (typeof idorder != "undefined") {
                loadcentChitkent.eventNotifyOrderOrocessed("", upone, idorder, idPrduct, _TypeNotifyKitchenOrder.UPDATEBYTABLE, updatedone);
            }
            else {
                loadcentChitkent.eventNotifyOrderOrocessed(idchitken, upone, idorder, idPrduct, _TypeNotifyKitchenOrder.Orocessed, updatedone);
            }
            $(this).removeAttr("disabled");
        });
        $(sel).find(".tab-pane").find(".lst-action").find("button.btn-delete").click(function () {
            $(this).attr("disabled", "disabled");
            let idchitken = $(this).parent().parent().data("id");
            loadcentChitkent.eventDeleteKitchen(idchitken, _TypeNotifyKitchenOrder.DELETE);
            $(this).removeAttr("disabled");
        });

    },
    eventDeleteKitchen: function (idkitchen, type) {
        $.ajax({
            type: 'POST',
            url: "/Selling/PosKitchen/DeleteKitchen",
            dataType: 'json',
            global: false,
            async: false,
            data: {
                TypeNotifyKitchenOrder: type,
                idChitken: idkitchen,
            },
            success: function (res) {
                if (res.isValid) {

                }
            },
            error: function (err) {
                console.log(err)
            }
        })

    },//đã chế biến
    eventNotifyOrderOrocessed: function (idkitchen, upone, idorder, idPrduct, Type, Status) {
        $.ajax({
            type: 'POST',
            url: "/Selling/PosKitchen/NotifyOrderOrocessed",
            dataType: 'json',
            global: false,
            async: false,
            data: {
                TypeNotifyKitchenOrder: Type,
                Status: Status,
                UpdateOne: upone,
                IdOrder: idorder,
                IdProduct: idPrduct,
                idChitken: idkitchen,
            },
            success: function (res) {
                if (res.isValid) {
                    //  loadcentChitkent.loadDataFoodReady(true);
                    //   loadcentChitkent.loadDataFoodNew(true);
                }
            },
            error: function (err) {
                console.log(err)
            }
        })

    }//đã chế biến
}
var thuchipos = {
    loadEventCategoryTabThuChi: function () {
        $("#tabthuchi li a").click(function () {
            if ($(this).hasClass("active")) {
                return;
            }
            let type = $(this).data("id");
            thuchipos.loadDataCategoryTabThuChi(type);
        });
    },

    loadDataCategoryTabThuChi: function (type) {//danh mcuj chi thu
        url = "/Selling/CategoryCevenue/CreateReceipts";
        if (type == TypeThuChi.CHI) {
            url = "/Selling/CategoryCevenue/CreatePayment";
        }
        html = ` <div class="card">
                        <div class="col-sm-12 pt-2">
                            <a onclick="eventCreate.addOrEditRevenueExpenditure('`+ url + `')" class="btn bg-success text-white">
                                <i class="fas fa-plus-circle"> </i> Thêm mới
                            </a>
                            <button class="ladda-button mr-2 btn btn-primary" id="reload" data-style="expand-left">
                                <span class="ladda-label"><i class="fa fas fa-bolt"></i>  Reload</span>
                                <span class="ladda-spinner"></span>
                            </button>
                        </div>
                        <div id="viewAll" class="card-body">
                            <table class="table table-bordered table-striped" style=" " id="dataTable">
                                <thead>
                                    <tr>
                                        <th>Mã nguồn</th>
                                        <th>Tên nguồn</th>
                                        <th>Mô tả</th>
                                        <th>Ngày tạo</th>
                                        <th>Người tạo</th>
                                        <th>Hành động</th>
                                    </tr>
                                </thead>
                                <tbody>
                                </tbody>
                            </table>
                        </div>
                    </div>`;
        $(".tab-datacontent").html(html);
        dataTableOut = $('#dataTable').DataTable({
            "responsive": true,
            "processing": true,
            stateSave: true,
            "pagingType": "full_numbers",
            "serverSide": true,
            "filter": true,
            "rowId": "id",
            "orderMulti": false,
            "ordering": false,
            // "order": [[0, "desc"]],
            "language": {
                "info": "Hiển thị _START_ đến _END_ dòng của tổng _TOTAL_ dòng",
                "infoEmpty": "Hiển thị 0 đến 0 dòng của tổng 0 dòng",
                "zeroRecords": "Không tìm thấy dữ liệu nào",
                "emptyTable": "Không có dữ liệu",
                "search": "Tìm kiếm:",
                "processing": "Đang xử lý...",
                "lengthMenu": "Hiển thị _MENU_ dòng",
                "loadingRecords": "Vui lòng chờ...",
                "paginate": {
                    "first": "Đầu tiên",
                    "last": "Cuối cùng",
                    "next": "Tiếp theo",
                    "previous": "Về trước"
                },
            },
            "pageLength": 10,
            "lengthMenu": [[10, 25, 50, 100, 200, -1], [10, 25, 50, 100, 200, "All"]],
            "buttons": [
                {
                    text: 'Reload',
                    action: function (e, dt, node, config) {
                        dataTableOut.ajax.reload();
                    }
                }
            ],
            "columnDefs": [


                { responsivePriority: 4, targets: [-1, 0] },////define hiển thị mặc định column 1

                {
                    targets: [1, 2, 3, 4, -1],
                    className: 'text-center'
                },
                {
                    "searchable": false,
                    "orderable": false,
                    "targets": [-1, 0],

                },
                { "width": "100px", "targets": -1 }

            ],
            ajax: {
                url: "/Selling/CategoryCevenue/LoadAll",
                type: "POST",
                datatype: "json",
                data: {
                    Name: function () { return $('#Code').val().trim() },
                    Code: function () { return $('#Code').val().trim() },
                    Type: function () {
                        return type;
                    },
                }
            },
            "columns": [
                {
                    "data": "code", "name": "code", "autoWidth": true

                }, {
                    "data": "name", "name": "name", "autoWidth": true

                }, {
                    "data": "content", "name": "content", "autoWidth": true

                }, {
                    "data": "createdOn", "name": "createdOn", "autoWidth": true,
                    "render": function (data, type, full, meta) {
                        // console.log(full.id);
                        if (full.createdOn != null) {
                            data = moment(full.createdOn).format('DD/MM/YYYY HH:MM:SS');
                            return data;
                        }
                        return "";
                    }
                }, {
                    "data": "createdBy", "name": "createdBy", "autoWidth": true

                },
                {
                    "data": null, "name": "",
                    "render": function (data, type, full, meta) {

                        let html = '<div class="btn-group dropleft">' +
                            '<button class="btn btn-light btn-sm dropdown-toggle" type="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"></button>' +
                            '<div class="dropdown-menu">' +
                            '<a class="dropdown-item" href="javascript:void(0)" onclick="eventCreate.addOrEditRevenueExpenditure(\'/Selling/CategoryCevenue/edit?secret=' + full.idString + '\')"><i class="fas fa-edit mr-2"></i> Sửa</a></a>' +

                            '<form id="form_delete_' + full.id + '" action="/Selling/CategoryCevenue/Delete?secret=' + full.idString + '"  asp-controller="RevenueExpenditure" asp-area="Selling" method="post" asp-action="Delete" onsubmit="return jQueryModalDelete(this)" class="d-inline">' +
                            '<a  class="dropdown-item"  href="javascript:$(\'#form_delete_' + full.id + '\').submit();"><i class="fas fa-trash-alt mr-2"></i> Xóa</a>' +
                            '</form>' +
                            '</div>' +
                            '</div>';

                        return html;
                    }
                },
            ]

        });
        $('#reload').on('click', function () {
            dataTableOut.ajax.reload(null, false);

        });
        $('input').keyup(function (e) {
            if (e.keyCode == 13) {
                dataTableOut.ajax.reload(null, false);
            }
        });
    },
    loadEventRevenueExpenditure: function () {
        $("#tabthuchi li a").click(function () {
            if ($(this).hasClass("active")) {
                return;
            }
            let type = $(this).data("id");
            thuchipos.loadDataRevenueExpenditure(type);
        });
    },
    loadEventQuyDauKy: function () {
        $.ajax({
            //global: false,
            url: "/Selling/RevenueExpenditure/GetDashboard",
            type: "GET",
            dataType: "json",
            data: {
                RangesDate: function () {
                    if ($('#rangesDate').val().trim() == "") {
                        return moment(new Date()).format("DD/MM/YYYY") + "-" + moment(new Date()).format("DD/MM/YYYY");
                    }
                    return $('#rangesDate').val().trim()
                },
            },
            // html: true,
            success: function (res) {
                if (res.isValid) {
                    $(".dashboardRevenueexpenditure ul li:nth-child(1)").find(".content").children("span").html(res.data.beginningFund.format0VND(0, 3));
                    $(".dashboardRevenueexpenditure ul li:nth-child(2)").find(".content").children("span").html(res.data.totalRevenue.format0VND(0, 3));
                    $(".dashboardRevenueexpenditure ul li:nth-child(3)").find(".content").children("span").html(res.data.totalExpenditure.format0VND(0, 3));
                    $(".dashboardRevenueexpenditure ul li:nth-child(4)").find(".content").children("span").html(res.data.fundsBalance.format0VND(0, 3));
                }
            },
        })
    },//tính quỹ đầu kỳ
    loadDataRevenueExpenditure: function (type) {
        url = "/Selling/RevenueExpenditure/CreateReceipts";
        if (type == TypeThuChi.CHI) {
            url = "/Selling/RevenueExpenditure/CreatePayment";
        }
        html = ` <div class="card">
                        <div class="col-sm-12 pt-2">
                            <a onclick="eventCreate.addOrEditRevenueExpenditure('`+ url + `')" class="btn bg-success text-white">
                                <i class="fas fa-plus-circle"> </i> Thêm mới
                            </a>
                            <button class="ladda-button mr-2 btn btn-primary" id="reload" data-style="expand-left">
                                <span class="ladda-label"><i class="fa fas fa-bolt"></i>  Reload</span>
                                <span class="ladda-spinner"></span>
                            </button>
                        </div>
                        <div id="viewAll" class="card-body">
                            <table class="table table-bordered table-striped" style=" " id="dataTable">
                                <thead>
                                    <tr>
                                        <th>Ngày tạo</th>
                                        <th>Mã phiếu</th>
                                        <th>Loại phiếu</th>
                                        
                                        <th>Số tiền</th>
                                        <th>Hình thức thanh toán</th>
                                        <th>Người nộp</th>
                                        <th>Mã chứng từ</th>
                                        <th>Trạng thái</th>
                                        <th>Hành động</th>
                                    </tr>
                                </thead>
                                <tbody>
                                </tbody>
                            </table>
                        </div>
                    </div>`;//<th>Người tạo</th>
        $(".tab-datacontent").html(html);
        dataTableOut = $('#dataTable').DataTable({
            "responsive": true,
            "processing": true,
            stateSave: true,
            "pagingType": "full_numbers",
            "serverSide": true,
            "filter": true,
            "rowId": "id",
            "orderMulti": false,
            "ordering": false,
            // "order": [[0, "desc"]],
            "language": {
                "info": "Hiển thị _START_ đến _END_ dòng của tổng _TOTAL_ dòng",
                "infoEmpty": "Hiển thị 0 đến 0 dòng của tổng 0 dòng",
                "zeroRecords": "Không tìm thấy dữ liệu nào",
                "emptyTable": "Không có dữ liệu",
                "search": "Tìm kiếm:",
                "processing": "Đang xử lý...",
                "lengthMenu": "Hiển thị _MENU_ dòng",
                "loadingRecords": "Vui lòng chờ...",
                "paginate": {
                    "first": "Đầu tiên",
                    "last": "Cuối cùng",
                    "next": "Tiếp theo",
                    "previous": "Về trước"
                },
            },
            "pageLength": 10,
            "lengthMenu": [[10, 25, 50, 100, 200, -1], [10, 25, 50, 100, 200, "All"]],
            "buttons": [
                {
                    text: 'Reload',
                    action: function (e, dt, node, config) {
                        dataTableOut.ajax.reload();
                    }
                }
            ],
            "columnDefs": [


                { responsivePriority: 4, targets: [-1, 0] },////define hiển thị mặc định column 1

                {
                    targets: [1, 2, 3, 4, -1],
                    className: 'text-center'
                },
                {
                    "searchable": false,
                    "orderable": false,
                    "targets": [-1, 0],

                },
                { "width": "100px", "targets": -1 }

            ],
            ajax: {
                url: "/Selling/RevenueExpenditure/LoadAll",
                type: "POST",
                datatype: "json",
                data: {
                    Name: function () { return $('#Code').val().trim() },
                    RangesDate: function () {
                        if ($('#rangesDate').val().trim() == "") {
                            return moment(new Date()).format("DD/MM/YYYY") + "-" + moment(new Date()).format("DD/MM/YYYY");
                        }
                        return $('#rangesDate').val().trim()
                    },
                    Type: function () {
                        return type;
                    },
                }
            },
            "columns": [
                {
                    "data": "date", "name": "date", "autoWidth": true

                }, {
                    "data": "code", "name": "code", "autoWidth": true

                }, {
                    "data": "categoryCevenueName", "name": "categoryCevenueName", "autoWidth": true

                },
                //{
                //    "data": "createdBy", "name": "createdBy", "autoWidth": true

                //},
                {
                    "data": "amount", "name": "amount", "autoWidth": true,
                    render: $.fn.dataTable.render.number(',', '.', 0)

                }, {
                    "data": "paymentName", "name": "paymentName", "autoWidth": true

                }, {
                    "data": "customerName", "name": "customerName", "autoWidth": true,
                    "render": function (data, type, full, meta) {
                        switch (full.objectRevenueExpenditure) {
                            case EnumTypeObjectRevenueExpenditure.KHACHHANG:
                                return '<div><span>' + full.customerName + '</span><br/><i>Khách hàng</i></div>';
                                break;
                            case EnumTypeObjectRevenueExpenditure.DOITAC:
                                return '<div><span>' + full.customerName + '</span><br/><i>Nhà cung cấp</i></div>';
                                break;
                            case EnumTypeObjectRevenueExpenditure.DOITUONGKHAC:
                                return '<div><span>' + full.customerName + '</span><br/><i>Đối tượng khác</i></div>';
                                break;
                            default:
                                return '';
                        }
                    }
                }, {
                    "data": "codeOriginaldocument", "name": "codeOriginaldocument", "autoWidth": true
                }, {
                    "data": "status", "name": "status", "autoWidth": true,
                    "render": function (data, type, full, meta) {
                        switch (full.status) {
                            case EnumStatusRevenueExpenditure.HUYBO:
                                return '<span class="badge badge-danger"><i class="fas fa-ban"></i> Đã hủy</span>';
                                break;
                            case EnumStatusRevenueExpenditure.HOANTHANH:
                                return '<span class="badge badge-success"><i class="fas fa-check-circle"></i> Hoàn thành</span>  ';
                                break;
                            default:
                                return '';
                        }
                    }
                },
                {
                    "data": null, "name": "",
                    "render": function (data, type, full, meta) {

                        let html = '<div class="btn-group dropleft">' +
                            '<button class="btn btn-light btn-sm dropdown-toggle" type="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"></button>' +
                            '<div class="dropdown-menu">' +
                            '<a class="dropdown-item" href="javascript:void(0)" onclick="eventCreate.addOrEditRevenueExpenditure(\'/Selling/RevenueExpenditure/edit?secret=' + full.idString + '\')"><i class="fas fa-edit mr-2"></i> Sửa</a></a>' +

                            '<form id="form_delete_' + full.id + '" action="/Selling/RevenueExpenditure/Delete?secret=' + full.idString + '"  asp-controller="RevenueExpenditure" asp-area="Selling" method="post" asp-action="Delete" onsubmit="return jQueryModalDelete(this)" class="d-inline">' +
                            '<a  class="dropdown-item"  href="javascript:$(\'#form_delete_' + full.id + '\').submit();"><i class="fas fa-trash-alt mr-2"></i> Xóa</a>' +
                            '</form>' +
                            '</div>' +
                            '</div>';

                        return html;
                    }
                },
            ]

        });
        $('#reload').on('click', function () {
            dataTableOut.ajax.reload(null, false);

        });
        $('input').keyup(function (e) {
            if (e.keyCode == 13) {
                dataTableOut.ajax.reload(null, false);
            }
        });
    }
}
var posStaff = {
    orderTable: function (dataObject) {
        $.ajax({
            type: 'POST',
            url: '/Selling/OrderTable/AddOrderTable',
            // contentType: 'application/json',
            dataType: 'json',
            data: dataObject,
            // traditional: true,
            success: async function (res) {
                //console.log(res.data);

                if (res.isValid) {
                    if (res.isbaobep) {
                        posStaff.eventKitchenNotice(res.dataHtml);

                    }
                    if (res.data.orderTableItems.length == 0) {
                        if (res.data.isBringBack) {
                            res.data.idRoomAndTableGuid = "-1";
                        }
                        await posStaff.getdatataorderbytable(res.data.idRoomAndTableGuid);
                    } else {
                        let html = ` `;
                        res.data.orderTableItems
                            .forEach(function (item, index) {
                                index = index + 1;
                                html += ` <tr data-id="` + item.idGuid + `" data-idpro="` + item.idProduct + `" data-slNotify="` + item.quantityNotifyKitchen + `" data-sl="` + item.quantity + `">
                                            <td>
                                                <div class="leftfood">
                                                    <i class="fas fa-trash-alt"></i>
                                                    <div class="content">
                                                        <span>` + index + ". " + item.name + `</span>
                                                        <i class="priceFormat">`+ (item.price) + `</i>
                                                    </div>
                                                </div>
                                            </td>
                                            <td>
                                                <div class="actionaddfood">
                                                    <i class="fas fa-minus"></i>
                                                    <input type="text" class="quantitynew " value="`+ (parseFloat(item.quantity.toString().replaceAll(",", "."))) + `" />
                                                    <i class="fas fa-plus"></i>
                                                </div>
                                            </td>
                                        </tr>`;

                            });

                        $(".bodybill table .itemorderbody").html(html);
                        $(".bodybill table .ordercode").html("Mã order:" + res.data.orderCode);
                        $("#id-order").data("id", res.data.idGuid);

                        posStaff.loadeventUpdatedataidtable();




                        posStaff.loadAmoutAndQuantity(res.data.amount, res.data.quantity);
                        posStaff.loadeventadditemorder();
                        posStaff.eventCheckBtnNotifyOrder();
                        posStaff.eventCheckActivetable();
                        // loadeventPos.loadEventClickIconAddAndMinus();
                        // loadeventPos.loadAddOrRemoveCurentClassTable(true);// xem có sản phẩm thì add class curen table
                        // console.log(dataObject.IdOrderItem);

                        //loadeventPos.loadactiveClickItemMon(dataObject.IdOrderItem, dataObject.IdProduct);// 


                        //// xử lý giữ lại khách hàng khi có nhiều tab dg mở nhưng kích vào tab active xong kích qua lại bị mất khách k hiển thị
                        //if (dataObject.TypeUpdate == _TypeUpdatePos.AddProduct && typeof dataObject.IdGuid == "undefined") {//xử lý với cái thêm mới

                        //    let findId = ListCusByOrderPos.find(x => x.idOrder == res.data.idGuid);
                        //    let cus = $(".search-customer").val();
                        //    if (typeof findId == "undefined" && cus != "") {
                        //        let arrCus = {};
                        //        arrCus.idOrder = res.data.idGuid;
                        //        arrCus.customerCode = cus.split(" ")[0];
                        //        arrCus.customerName = res.data.buyer;
                        //        ListCusByOrderPos.push(arrCus);
                        //    }
                        //}
                        numberFormat();
                        priceFormat();
                    }
                    if (parseFloat(dataObject.QuantityFloat) > 0) {
                        toastrcus.success("Thêm món thành công");
                    } else {
                        toastrcus.warning("Hủy món thành công");
                    }
                } else {

                    toastrcus.success("Cập nhật thất bại vui lòng kiểm tra lại internet");
                }
            },
            error: function (err) {
                console.log(err)
            }
        });
    },
    eventKitchenNotice: function (data) {
        connection.invoke('Printbaobep', data, "IN");
    },
    eventCheckActivetable: function () {
        let idatable = $(".topbuton button:first").data("idtable");
        if (typeof idatable != "undefined") {
            if ($(".bodybill table .itemorderbody").find("tr").not(".tr-nodata").length>0) {
                $("#lst-roomandtable li").map(function (ind, ele) {
                    // let idData = $(ele).;
                    if ($(ele).data("id") == idatable) {
                        $(ele).addClass("active");
                        //$(ele).find(".ribbon-two").remove();
                    }
                });
            }
        }
    },
    NotifyChitken: function () {
        let idOrder = $("#id-order").data("id");
        if (typeof idOrder == "undefined") {
            toastrcus.error("Chưa có đơn cần báo");
            return;
        }
        $(".btn-notif").attr("disabled", "disabled");
        $.ajax({
            type: 'POST',
            traditional: true,
            async: false,
            url: '/Selling/OrderTable/NotifyKitChen',
            data: {
                IdOrder: idOrder,
            },
            success: function (res) {
                if (res.isValid) {
                    console.log("Thông báo ok");
                    posStaff.eventUpdateQuantityItemMonOrder();
                    connection.invoke('Printbaobep', res.html,"IN");
                } else {

                    $(".btn-notif").removeAttr("disabled");
                }
            },
            error: function (err) {
                console.log(err)
            }
        });
    },//dành cho nhân viên phục vụ
    loadEventNoitfyStaff: function () {
        $(".btn-notif").click(function () {
            posStaff.NotifyChitken();
            //connection.invoke('sendNotifyPos', EnumTypeSignalRHub.CHITKEN, EnumTypeSignalRHub.KITCHENTOPOS);
            event.preventDefault();
        });
    },
    loadeventadditemorder: function () {
        $(".bodybill table tbody tr i.fa-minus").click(function () {
            let IdOrderItem = $(this).parents("tr").data("id");
            let idTable = $(".topbuton button:first").data("idtable");
            let cusocde = '';
            let slgoc = $(this).parents("tr").data("sl");

            let slnotify = $(this).parents("tr").data("slnotify");
            let IdCustomer = "";
            // if (cusocde.trim() != "") {
            IdCustomer = $("#searchcustomer").data("id");
            cusocde = $("#searchcustomer").data("code");
            // }
            //bấm giảm thì phải hỏi

            if (parseFloat(slnotify) > (parseFloat(slgoc) - 1)) {
                let namepro = $(this).parents("li").find(".name").find("b").html();
                let html = `
                            <div class="form-confirmremoveitem">
                                <span>Bạn có chắc chắn muốn hủy món <b>`+ namepro + `</b> không?</span>
                                <div class="slminus">
                                    <span>Số lượng hủy/giảm</span>
                                    <div class="item_action"><i class="fas fa-minus"></i><span data-slhuy="1" class="quantitynew">1</span><span class="quantityold">/`+ slnotify + `</span> <i class="fas fa-plus"></i></div>
                                </div>
                                <div class="input-note">
                                    <span class="text">Lý do: </span>
                                     <input type="text" id="noteminus" class="form-control"/>
                                </div>
                            </div>
                            `;
                Swal.fire({
                    // icon: 'success',
                    title: "Xác nhận giảm / Hủy món",
                    html: html,
                    showClass: {
                        popup: 'popup-formcreate'
                    },
                    footer: "<button class='swal2-cancel swal2-styled btn-cancel mr-3'><i class='icon-cd icon-add_cart icon'></i>Hủy bỏ</button><button class='swal2-styled btn btn-success btn-save'><i class='icon-cd icon-doneAll icon'></i>Đồng ý</button>",
                    allowOutsideClick: true,
                    showConfirmButton: false,
                    showCancelButton: false,
                    cancelButtonText: '<i class="fa fa-window-close"></i> Đóng',
                    didRender: () => {
                        $('.form-confirmremoveitem select').prepend('<option selected></option>').select2({

                            placeholder: {
                                id: '', // the value of the option
                                text: "Chọn bàn"
                            },
                            allowClear: true,
                            language: {
                                noResults: function () {
                                    return "Không tìm thấy dữ liệu";
                                }
                            },
                        })
                        $(".form-confirmremoveitem .item_action i:first-child").click(function () {

                            let slafter = parseFloat($(this).parent().children(".quantitynew").data("slhuy"));
                            if (slafter >= 0 && slafter < 1) {
                                slafter = parseDecimal(slafter - 0.1);
                            } else {
                                slafter = parseDecimal(slafter - 1);
                            }

                            if (slafter <= 0) {
                                toastr.error("Sô lượng hủy/giảm phải lớn hơn 0");
                                return false;
                            } else {

                                $(this).parent().children(".quantitynew").data("slhuy", slafter);
                                $(this).parent().children(".quantitynew").html(slafter);
                            }

                        });
                        $(".form-confirmremoveitem .item_action i:last-child").click(function () {
                            let slafter = parseFloat($(this).parent().children(".quantitynew").data("slhuy"));
                            if (slafter >= parseFloat(slnotify)) {
                                toastr.error("Sô lượng hủy/giảm không vượt quá số lượng gốc");
                                return false;
                            }
                            else {
                                if (slafter >= 0 && slafter < 1) {
                                    slafter = parseDecimal(slafter + 0.1);
                                } else {
                                    slafter = parseDecimal(slafter + 1);
                                }
                                $(this).parent().children(".quantitynew").data("slhuy", slafter);
                                $(this).parent().children(".quantitynew").html(slafter);
                            }
                        });


                        $(".btn-cancel").click(function () {
                            Swal.close();
                        });
                        $(".btn-save").click(function () {

                            let slafter = parseFloat($(".form-confirmremoveitem .item_action").children(".quantitynew").data("slhuy"));
                            let noteminus = $(".form-confirmremoveitem").find("#noteminus").val();
                            var dataObject = {
                                Note: noteminus,
                                IsCancel: true,
                                IdOrderItem: IdOrderItem,
                                CusCode: cusocde,
                                IdCustomer: IdCustomer,
                                QuantityFloat: (slafter * -1).toString(),
                                TypeUpdate: _TypeUpdatePos.UpdateQuantity,
                                IdRoomAndTableGuid: idTable != "-1" ? idTable : "",
                                IsBringBack: idTable == "-1" ? true : false,
                                IdGuid: $("#id-order").data("id")
                            };// giảm số lượng

                            posStaff.orderTable(dataObject);
                            Swal.close();
                        });

                    }
                });

            } else {
                var dataObject = {
                    IdOrderItem: IdOrderItem,
                    CusCode: cusocde,
                    IdCustomer: IdCustomer,
                    QuantityFloat: -1,
                    TypeUpdate: _TypeUpdatePos.UpdateQuantity,
                    IdRoomAndTableGuid: idTable != "-1" ? idTable : "",
                    IsBringBack: idTable == "-1" ? true : false,
                    IdGuid: $("#id-order").data("id")
                };// giảm số lượng
                posStaff.orderTable(dataObject);
            }
        });
        //tăng
        $(".bodybill table tbody tr i.fa-plus").click(function () {
            let IdOrderItem = $(this).parents("tr").data("id");
            let idTable = $(".topbuton button:first").data("idtable");
            let cusocde = '';
            let slgoc = $(this).parents("tr").data("sl");
            let slnotify = $(this).parents("tr").data("slnotify");
            let IdCustomer = "";
            // if (cusocde.trim() != "") {
            IdCustomer = $("#searchcustomer").data("id");
            cusocde = $("#searchcustomer").data("code");
            // }
            //bấm giảm thì phải hỏi
            var dataObject = {
                IdOrderItem: IdOrderItem,
                CusCode: cusocde,
                IdCustomer: IdCustomer,
                QuantityFloat: 1,
                TypeUpdate: _TypeUpdatePos.UpdateQuantity,
                IdRoomAndTableGuid: idTable != "-1" ? idTable : "",
                IsBringBack: idTable == "-1" ? true : false,
                IdGuid: $("#id-order").data("id")
            };// giảm số lượng
            posStaff.orderTable(dataObject);
        });
        //thay đổi
        $(".bodybill table tbody tr input.quantitynew").change(function () {
            let IdOrderItem = $(this).parents("tr").data("id");
            let idTable = $(".topbuton button:first").data("idtable");
            let cusocde = '';
            let slgoc = $(this).parents("tr").data("sl");
            let slnotify = $(this).parents("tr").data("slnotify");
            let IdCustomer = "";
            let slnew = $(this).val();
            //$(this).val(slgoc);
            // return;
            // if (cusocde.trim() != "") {
            IdCustomer = $("#searchcustomer").data("id");
            cusocde = $("#searchcustomer").data("code");
            // }
            //bấm giảm thì phải hỏi
            //bấm giảm thì phải hỏi
            if (parseFloat(slnew) <= 0) {
                $(this).val(slgoc);
                toastrcus.error("Số lượng không hợp lệ phải lớn hơn 0");
                return;
            }
            if (parseFloat(slnotify) > parseFloat(slnew)) {
                let slcanuychuabao = parseFloat((parseFloat(slgoc) - parseFloat(slnew)).toFixed(3));//sl góc bao gồm cả thông bsao và chưa báo

                let namepro = $(this).parents("li").find(".name").find("b").html();
                let html = `
                            <div class="form-confirmremoveitem">
                                <span>Bạn có chắc chắn muốn hủy món <b>`+ namepro + `</b> không?</span>
                                <div class="slminus">
                                    <span>Số lượng hủy/giảm</span>
                                    <div class="item_action"><i class="fas fa-minus"></i><span data-slhuy="`+ slcanuychuabao + `" class="quantitynew">` + slcanuychuabao + `</span><span class="quantityold">/` + slnotify + `</span> <i class="fas fa-plus"></i></div>
                                </div>
                                <div class="input-note">
                                    <span class="text">Lý do: </span>
                                    <input type="text" id="noteminus" class="form-control"/>
                                </div>
                            </div>
                            `;
                Swal.fire({
                    // icon: 'success',
                    title: "Xác nhận giảm / Hủy món",
                    html: html,
                    showClass: {
                        popup: 'popup-formcreate'
                    },
                    footer: "<button class='swal2-cancel swal2-styled btn-cancel mr-3'><i class='icon-cd icon-add_cart icon'></i>Hủy bỏ</button><button class='swal2-styled btn btn-success btn-save'><i class='icon-cd icon-doneAll icon'></i>Đồng ý</button>",
                    allowOutsideClick: true,
                    showConfirmButton: false,
                    showCancelButton: false,
                    cancelButtonText: '<i class="fa fa-window-close"></i> Đóng',
                    didRender: () => {
                        $(".form-confirmremoveitem .item_action i:first-child").click(function () {

                            let slafter = parseFloat($(this).parent().children(".quantitynew").data("slhuy"));
                            if (slafter >= 0 && slafter < 1) {
                                slafter = parseDecimal(slafter - 0.1);
                            } else {
                                slafter = parseDecimal(slafter - 1);
                            }

                            if (slafter <= 0) {
                                toastr.error("Số lượng hủy/giảm còn ít nhất là 0");
                                return false;
                            } else {
                                $(this).parent().children(".quantitynew").data("slhuy", slafter);
                                $(this).parent().children(".quantitynew").html(slafter);
                            }

                        });
                        $(".form-confirmremoveitem .item_action i:last-child").click(function () {
                            let slafter = parseFloat($(this).parent().children(".quantitynew").data("slhuy"));

                            if (slafter >= parseFloat(slnotify)) {
                                toastr.error("Sô lượng hủy/giảm không vượt quá số lượng gốc");
                                return false;
                            }
                            else {
                                if (slafter >= 0 && slafter < 1) {
                                    slafter = parseDecimal(slafter + 0.1);
                                } else {
                                    slafter = parseDecimal(slafter + 1);
                                }
                                $(this).parent().children(".quantitynew").data("slhuy", slafter);
                                $(this).parent().children(".quantitynew").html(slafter);
                            }
                        });


                        $(".btn-cancel").click(function () {
                            $(sel).val(slgoc);
                            Swal.close();
                        });
                        $(".btn-save").click(function () {
                            let slafter = parseFloat($(".form-confirmremoveitem .item_action").children(".quantitynew").data("slhuy"));
                            let noteminus = $(".form-confirmremoveitem").find("#noteminus").val();
                            var dataObject = {
                                Note: noteminus,
                                IsCancel: true,
                                IdOrderItem: IdOrderItem,
                                CusCode: cusocde,
                                IdCustomer: IdCustomer,
                                QuantityFloat: (slafter * -1).toString(),
                                TypeUpdate: _TypeUpdatePos.UpdateQuantity,
                                IdRoomAndTableGuid: idTable != "-1" ? idTable : "",
                                IsBringBack: idTable == "-1" ? true : false,
                                IdGuid: $("#id-order").data("id")
                            };// giảm số lượng
                            posStaff.orderTable(dataObject);
                            Swal.close();
                        });

                    }
                });

            }
            else {

                let sladdthem = parseFloat((parseFloat(slnew) - parseFloat(slgoc)).toFixed(3));
                if (sladdthem != 0) {
                    var dataObject = {
                        IdOrderItem: IdOrderItem,
                        CusCode: cusocde,
                        IdCustomer: IdCustomer,
                        QuantityFloat: sladdthem.toString(),
                        TypeUpdate: _TypeUpdatePos.UpdateQuantity,
                        IdRoomAndTableGuid: idTable != "-1" ? idTable : "",
                        IsBringBack: idTable == "-1" ? true : false,
                        IdGuid: $("#id-order").data("id")
                    };// giảm số lượng

                    posStaff.orderTable(dataObject);

                } else if (sladdthem == 0) {

                } else {
                    toastrcus.warning("Số lượng không hợp lệ");
                }

            }



        });
        //sự kiện xóa dòng đó luôn
        $(".bodybill table tbody tr .leftfood i").click(function () {

            let IdOrderItem = $(this).parents("tr").data("id");
            let idTable = $(".topbuton button:first").data("idtable");
            let Quantity = parseFloat($(this).parents("tr").data("sl")) || 0;
            let slgoc = parseFloat($(this).parents("tr").data("sl")) || 0;
            let slnotify = parseFloat($(this).parents("tr").data("slnotify")) || 0;

            if (parseFloat(slnotify) > 0) {
                let html = `
                            <div class="form-confirmremoveitem">
                                <span>Bạn có chắc chắn muốn hủy không?</span>
                                <div class="slminus">
                                    <span>Số lượng hủy/giảm</span>
                                    <div class="item_action"><i class="fas fa-minus"></i><span data-slhuy="`+ slnotify + `" data-goc="` + slgoc + `" data-slnotify="` + slnotify + `" class="quantitynew">` + slgoc + `</span><span class="quantityold">/` + slgoc + `</span> <i class="fas fa-plus"></i></div>
                                </div>
                                <div class="input-note">
                                    <span class="text">Lý do: </span>
                                     <input type="text" id="noteminus" class="form-control"/>
                                </div>
                            </div>
                            `;
                Swal.fire({
                    // icon: 'success',
                    title: "Xác nhận giảm / Hủy món",
                    html: html,
                    showClass: {
                        popup: 'popup-formcreate'
                    },
                    footer: "<button class='swal2-cancel swal2-styled btn-cancel mr-3'><i class='icon-cd icon-add_cart icon'></i>Hủy bỏ</button><button class='swal2-styled btn btn-success btn-save'><i class='icon-cd icon-doneAll icon'></i>Đồng ý</button>",
                    allowOutsideClick: true,
                    showConfirmButton: false,
                    showCancelButton: false,
                    cancelButtonText: '<i class="fa fa-window-close"></i> Đóng',
                    didRender: () => {
                        $(".form-confirmremoveitem .item_action i:first-child").click(function () {

                            let slafter = parseFloat($(this).parent().children(".quantitynew").data("slhuy"));
                            if (slafter >= 0 && slafter < 1) {
                                slafter = parseDecimal(slafter - 0.1);
                            } else {
                                slafter = parseDecimal(slafter - 1);
                            }
                            if (slafter <= 0) {
                                toastr.error("Sô lượng hủy/giảm phải lớn hơn 0");
                                return false;
                            } else {
                                $(this).parent().children(".quantitynew").data("slhuy", slafter);
                                $(this).parent().children(".quantitynew").html(slafter);
                            }

                        });
                        $(".form-confirmremoveitem .item_action i:last-child").click(function () {
                            let slafter = parseFloat($(this).parent().children(".quantitynew").data("slhuy"));
                            if (slafter >= parseFloat(slnotify)) {
                                toastr.error("Sô lượng hủy/giảm không vượt quá số lượng gốc");
                                return false;
                            }
                            else {
                                if (slafter >= 0 && slafter < 1) {
                                    slafter = parseDecimal(slafter + 0.1);
                                } else {
                                    slafter = parseDecimal(slafter + 1);
                                }

                                $(this).parent().children(".quantitynew").data("slhuy", slafter);
                                $(this).parent().children(".quantitynew").html(slafter);
                            }
                        });


                        $(".btn-cancel").click(function () {
                            Swal.close();
                        });
                        $(".btn-save").click(function () {

                            let slgoc = parseFloat($(".form-confirmremoveitem .item_action").children(".quantitynew").data("slgoc"));
                            let slafter = parseFloat($(".form-confirmremoveitem .item_action").children(".quantitynew").data("slhuy"));
                            let slnotify = parseFloat($(".form-confirmremoveitem .item_action").children(".quantitynew").data("slnotify"));
                            let noteminus = $(".form-confirmremoveitem").find("#noteminus").val();
                            let slnotNotify = slgoc - slnotify;

                            let type = _TypeUpdatePos.UpdateQuantity;
                            let IsCancel = false;
                            if (slafter == slgoc) {
                                type = _TypeUpdatePos.RemoveRowItem;
                            }
                            if (slnotNotify != slafter) {
                                IsCancel = true;
                            }
                            var dataObject = {
                                Note: noteminus,
                                IsCancel: IsCancel,
                                IdOrderItem: IdOrderItem,
                                QuantityFloat: (slafter * -1),
                                TypeUpdate: type,
                                IdRoomAndTableGuid: idTable != "-1" ? idTable : "",
                                IsBringBack: idTable == "-1" ? true : false,
                                IdGuid: $("#id-order").data("id")
                            };// giảm số lượng

                            posStaff.orderTable(dataObject);
                            Swal.close();
                        });

                    }
                });

            } else {
                var dataObject = {
                    QuantityFloat: Quantity,
                    IdOrderItem: IdOrderItem,
                    TypeUpdate: _TypeUpdatePos.RemoveRowItem,
                    IdRoomAndTableGuid: idTable != "-1" ? idTable : "",
                    IsBringBack: idTable == "-1" ? true : false,
                    IdGuid: $("#id-order").data("id")
                };// giảm số lượng
                posStaff.orderTable(dataObject);
            }
        });
    },//sự kiện item tăng giảm số lượng
    loadeventremoveOrder: function () {
        $(".btn-removerOrder").click(function () {
            let iddt = $("#id-order").data("id");
            Swal.fire({
                icon: 'warning',
                title: 'Bạn có chắc chắn muốn xóa đơn không?',
                // showDenyButton: true,
                showCancelButton: true,
                confirmButtonText: 'Đồng ý',
                cancelButtonText: 'Đóng',
                // denyButtonText: `Don't save`,
            }).then((result) => {
                /* Read more about isConfirmed, isDenied below */
                if (result.isConfirmed) {
                    $.ajax({
                        type: 'POST',
                        //global: false,
                        url: '/Selling/OrderTable/RemoveOder',
                        data: {
                            TypeUpdate: _TypeUpdatePos.RemoveOrder,
                            IdOrder: iddt
                        },

                        success: function (res) {
                            if (res.isValid) {

                                posStaff.checkHighlightTableInOrder();// check highlight
                                html = `<table class="table">
                                        <thead>
                                            <tr>
                                                <th class="ordercode">Mã đơn: </th>
                                                <th style="    width: 123px;" class="text-center"></th>
                                            </tr>
                                        </thead>
                                        <tbody class="itemorderbody">
                                            <tr class="tr-nodata">
                                                <td colspan="2">
                                                    <div class="no-order">
                                                        <img src="../images/ristorante_old.png" />
                                                        <b>Chưa có món nào được chọn</b>
                                                        <span>Vui lòng chọn món trong thực đơn</span>
                                                    </div>
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>`;
                                $(".bodybill").html(html);
                                $(".actionstaff").find(".btn-addNeworderStaff").remove();
                                $(".actionstaff").find(".btn-removerOrder").remove();
                            }
                        },
                        error: function (err) {
                            console.log(err)
                        }
                    });
                }
            })

        });
    },
    checkHighlightTableInOrder: function () {

        $("#lst-roomandtable li").map(function (ind, ele) {
            // let idData = $(ele).;
            if ($(ele).hasClass("CurentOrder")) {
                $(ele).removeClass("active");
                $(ele).find(".ribbon-two").remove();
            }
        });

    },
    loadAmoutAndQuantity: function (amount = null, quantity = null, idorder = "") {

        if (amount != null && quantity != null) {
            $(".amountFull").html(amount);
            $(".fullQuantity").html("(" + parseFloat(quantity.toString().replaceAll(",", ".")) + ")");
        } else {
            amount = 0;
            quantity = 0;
            //if (idorder != "") {

            //    $(".tab-content-order").find("div[data-id=" + idorder + "]").find("ul").find("li").map(function (item, index) {

            //        amount += parseFloat($(this).find(".amount").find(".priceFormat").html().replaceAll(".", ""));
            //        quantity += parseFloat($(this).find(".quantity").html().replaceAll(".", ""));
            //    });
            //} else {
            //    $(".tab-content-order").find(".tab-pane.active").find("ul").find("li").map(function (item, index) {
            //        amount += parseFloat($(this).find(".amount").find(".priceFormat").html().replaceAll(".", ""));
            //        quantity += parseFloat($(this).find(".quantity").html().replaceAll(".", ""));
            //    });
            //}

            $(".amountFull").html(amount);
            $(".fullQuantity").html("(" + 0 + ")");
            // $(".quantitySum").html(quantity);
        }
        priceFormat();
    },//load toongrquantity và tiền:
    loadeventUpdatedataidtable: function () {

        $('.bodybill table > tbody  > tr').each(function (index, tr) {
            let iddata = $(this).data("id");
            let idpro = $(this).data("idpro");
            let slNotify = $(this).data("slNotify");
            let sl = $(this).data("sl");
            $(this).removeAttr("id");
            $(this).removeAttr("idpro");
            $(this).removeAttr("slNotify");
            $(this).removeAttr("sl");
            $(this).data("id", iddata);
            $(this).data("idpro", idpro);
            $(this).data("slNotify", slNotify);
            $(this).data("sl", sl);
        });
    },// xử lý xóa data id
    loadeventClickThucDon: function () {
        $("#lstfood li").click(function () {
            //$("#lst-product li").on("click", function (event) {
            let idTable = $(".topbuton button:first").data("idtable");
            let IdGuid = $("#id-order").data("id");
            if (typeof idTable == "undefined") {
                toastrcus.error("Vui lòng chọn bàn/phòng");
            }
            var dataObject = {
                CusCode: "",// chỉ ở đây mới cập nhật khách vì là mới order món đầu tiên
                QuantityFloat: 1,
                TypeUpdate: _TypeUpdatePos.AddProduct,
                IdRoomAndTableGuid: idTable != "-1" ? idTable : "",
                IsBringBack: idTable == "-1" ? true : false,
                IdGuid: IdGuid,
                IdProduct: $(this).data("id")
            };
            posStaff.orderTable(dataObject);//lưu đơn
        });
    },
    getdatataorderbytable: async function (idTable) {
        var loadOrder = await axios.get("/Selling/OrderTable/LoadDataOrderStaff?idtable=" + idTable);
        if (loadOrder.data.isValid) {

            $(".bodybill").html(loadOrder.data.data);
            //loadeventPos.eventUpdatedataItemMonOrder();
            posStaff.loadeventUpdatedataidtable();
            posStaff.loadEventNoitfyStaff();

            posStaff.loadeventadditemorder();
            //if (loadOrder.data.dataNote.length > 0) {
            //    ListNoteOrder = loadOrder.data.dataNote;
            //}
            $(".actionstaff").find(".btn-addNeworderStaff").remove();
            $(".actionstaff").find(".btn-removerOrder").remove();
            if (loadOrder.data.active) {
                $(".actionstaff").append('<button type="button" class="btn-addNeworderStaff btn btn-primary"><i class="fas fa-plus"></i></button>')
                $(".actionstaff").append('<button type="button" class="btn-removerOrder btn btn-danger ml-2"><i class="fas fa-trash"></i></button>');
                posStaff.loadeventremoveOrder();
                $("#lst-roomandtable li.active").addClass("active");
                $(".topbuton button:first").trigger("click");
            } else {
                $(".topbuton button:last").trigger("click");
            }
            posStaff.eventCheckBtnNotifyOrder();
            evetnFormatnumber3();
            evetnFormatTextnumber3decimal();
        } else {
            return false;
        }
    },
    eventUpdateQuantityItemMonOrder: function () {

        $('.bodybill table > tbody  > tr').each(function (index, tr) {
            let sl = $(this).data("sl");
            $(this).data("slnotify", sl)
        });
        $(".btn-notif").attr("disabled", "disabled");
    },
    eventCheckBtnNotifyOrder: function () {
        let checkShowNotify = false;
        $('.bodybill table > tbody  > tr').each(function (index, tr) {
            let sl = $(this).data("sl");
            let slnotify = $(this).data("slnotify");
            if (slnotify != sl) {
                checkShowNotify = true;
            }
        });

        if (checkShowNotify) {
            $(".btn-notif").removeAttr("disabled");
        } else {
            $(".btn-notif").attr("disabled", "disabled");
        }
    },
    eventactiveTableloca: function (_cl) {
        if (typeof (Storage) !== "undefined") {
            // Store
            localStorage.setItem("activeTableStaff", _cl); // tham số active table
            // Retrieve
        } else {
            alert("Liên hệ đội ngủ hỗ  trợ lỗi trình duyệt của bạn không hỗ  trợ Storage");
        }
    },
    eventIntwindow: function () {
        $(".topbuton button:first").trigger("click");
        //loadTableActive();
        //function loadTableActive() { // load active

        //    if (typeof (Storage) !== "undefined") {
        //        // Store
        //        var _getActivetable = localStorage.getItem("activeTableStaff");
        //        if (!isNaN(parseInt(_getActivetable))) {
        //            _getActivetable = parseInt(_getActivetable) + 1;
        //            if ($("#lst-roomandtable li").length < _getActivetable) {
        //                _getActivetable = 1;
        //            }
        //            // $("#lst-roomandtable li:nth-child(" + _getActivetable + ")").trigger('click'); // là số thì active cái đó
        //            let ele = $("#lst-roomandtable li:nth-child(" + _getActivetable + ")"); // là số thì active cái đó
        //            ele.addClass("active");
        //            posStaff.getdatataorderbytable(ele.data("id"));

        //        } else {
        //            //nếu chưa chọn bàn lần nào thì show bàn ra cho chọn
        //            $(".topbuton button:first").trigger("click");
        //        }
        //        // Retrieve
        //    } else {
        //        alert("Liên hệ đội ngủ hỗ  trợ lỗi trình duyệt của bạn không hỗ  trợ Storage");
        //    }
        //}
    },
    loadeventClickbtnFood: function () { //kích vào thực đơn
        $(".topbuton button:last").click(function () {
            if (!$("#dataTableRoom").hasClass("d-none")) {
                $("#dataTableRoom").addClass("d-none");
                //$("#dataTableRoom").find(".dataArea").html("");
                //$("#dataTableRoom").find(".datatable").html("");
            }
            $(this).attr("disabled", "disabled");
            if ($("#dataListFood").hasClass("d-none")) {
                if ($("#dataListFood .lstcategoryfood li").length == 0) {
                    $.ajax({
                        type: 'GET',
                        //async: false,
                        url: '/Selling/Product/GetProductJson',
                        data: {

                        },

                        success: function (res) {
                            if (res.isValid) {
                                htmlcate = `<ul class="lstcategoryfood">`;
                                htmlcate += `<li class="active" data-id="0">
                                           <a href="javascript:void(0)" data-id="0">
                                                <span>`+ res.jsonPro.length + `</span> Tất cả
                                            </a>
                                         </li>`;

                                res.jsoncate.forEach(function (item, index) {
                                    htmlcate += `<li>
                                         <a href="javascript:void(0)" data-id="`+ item.id + `">
                                             <span>`+ item.countpro + `</span> ` + item.name + `
                                         </a>
                                      </li>`;
                                });
                                htmlcate += '</ul>';
                                $("#dataListFood .dataCategoryleft").html(htmlcate);
                                htmlcate = `<ul id="lstfood" class="ul-nonestyle">`;
                                res.jsonPro.forEach(function (item, index) {
                                    htmlimg = "./images/no-img.png";
                                    if (item.img != "") {
                                        htmlimg = "../" + item.img;
                                    }
                                    htmlcate += ` <li class="" style="display: flex;"  data-id="` + item.id + `" data-idcategory="` + item.idCategory + `">
                                                <div class="ribbon ribbon-primary float-end number3"> ` + item.retailPrice + `</div>
                                                <img src="`+ htmlimg + `" />
                                                <div class="conent-botom">
                                                    <span>` + item.name + `</span>
                                                </div>
                                        </li>`;
                                });
                                htmlcate += '</ul>';

                                $("#dataListFood .datatable").html(htmlcate);
                                //update lại iddata
                                $("#dataListFood .dataCategoryleft ul li").map(function () {
                                    //let idata = $(this).children("a").data("id");
                                    let idatacate = $(this).children("a").data("id");
                                    $(this).children("a").removeAttr("data-id");
                                    $(this).children("a").data("id", idatacate);
                                });
                                $("#dataListFood .datatable ul li").map(function () {
                                    let idata = $(this).data("id");
                                    let idatacate = $(this).data("idcategory");
                                    $(this).removeAttr("data-idcategory");
                                    $(this).removeAttr("data-id");
                                    $(this).data("idcategory", idatacate);
                                    $(this).data("id", idata);
                                });
                                $("#dataListFood .dataCategoryleft a").click(function () {
                                    let ididcategory = $(this).data("id");
                                    $("#dataListFood #lstfood li").filter(function () {
                                        var element = $(this);
                                        let idcategory = $(this).data("idcategory");
                                        if (ididcategory == idcategory || ididcategory == 0) {
                                            element.css('display', "flex");
                                        } else {
                                            element.css('display', "none");

                                        }
                                    });
                                    $(this).parents("ul.lstcategoryfood").find("li.active").removeClass("active");
                                    $(this).parent("li").addClass("active");
                                });
                                evetnFormatTextnumber3decimal();
                                posStaff.loadeventClickThucDon();
                                //sự kiện kích chọn bàn
                                // posStaff.evntSelectTabel();
                            }
                        },
                        error: function (err) {
                            console.log(err)
                        }

                    });
                }
                $("#dataListFood").toggleClass("d-none");
            } else {
                $("#dataListFood").toggleClass("d-none");
            }

            $(this).removeAttr("disabled");
        });
    },
    loadeventSelectttable: function () {
        $(".topbuton button:first").data("id", 0);
        $(".topbuton button:first").click(function () {
            $(this).attr("disabled", "disabled");
            if (!$("#dataListFood").hasClass("d-none")) {
                $("#dataListFood").addClass("d-none");
                //$("#dataListFood").find(".dataArea").html("");
                //$("#dataListFood").find(".datatable").html("");
            }
            if ($("#dataTableRoom").hasClass("d-none")) {
                if ($("#dataTableRoom .litstbale li").length == 0) {//nếu có rồi thì thôi
                    $.ajax({
                        type: 'GET',
                        //async: false,
                        url: '/Selling/RoomTable/GetTableJson',
                        data: {

                        },

                        success: function (res) {
                            if (res.isValid) {
                                htmlcate = `<ul class="litstbale">`;
                                htmlcate += `<li class="active" data-id="0">
                                   <a href="javascript:void(0)" data-id="0">
                                        <span>`+ res.jsonPro.length + `</span> Tất cả
                                    </a>
                                 </li>`;

                                res.jsoncate.forEach(function (item, index) {
                                    htmlcate += `<li>
                                         <a href="javascript:void(0)" data-id="`+ item.id + `">
                                             <span>`+ item.countpro + `</span> ` + item.name + `
                                         </a>
                                      </li>`;
                                });
                                htmlcate += '</ul>';
                                $("#dataTableRoom .dataArea").html(htmlcate);
                                htmlcate = `<ul id="lst-roomandtable" class="ul-nonestyle">`;
                                htmlcate += `<li class="mangve" style="display: flex;" data-id="-1" data-idcategory="-1">
                                        <i class="fas fa-baby-carriage"></i>
                                        <b>Mang về</b>
                                    </li>`;
                                res.jsonPro.forEach(function (item, index) {
                                    htmlMac = ` <li class="" style="display: flex;"  data-id="` + item.idtable + `" data-idcategory="` + item.idArea + `">`;
                                    if (item.numberProduct > 0) {
                                        htmlMac = `<li class="active" style="display: flex;"  data-id="` + item.idtable + `" data-idcategory="` + item.idArea + `"> <div class="ribbon-two ribbon-two-primary"><span>` + item.numberProduct + `</span></div>`;
                                    }
                                    htmlcate += htmlMac + `
                                            <i class="fas fa-utensils"></i>
                                            <b>` + item.tableName + `</b>
                                        </li>`;
                                });
                                htmlcate += '</ul>';
                                $("#dataTableRoom .datatable").html(htmlcate);
                                //update lại iddata
                                $("#dataTableRoom .dataArea ul li").map(function () {
                                    //let idata = $(this).children("a").data("id");
                                    let idatacate = $(this).children("a").data("id");
                                    $(this).children("a").removeAttr("data-id");
                                    $(this).children("a").data("id", idatacate);
                                });
                                $("#dataTableRoom .datatable ul li").map(function () {
                                    let idata = $(this).data("id");
                                    let idatacate = $(this).data("idcategory");
                                    $(this).removeAttr("data-idcategory");
                                    $(this).removeAttr("data-id");
                                    $(this).data("idcategory", idatacate);
                                    $(this).data("id", idata);
                                });
                                $("#dataTableRoom .dataArea a").click(function () {
                                    let ididcategory = $(this).data("id");
                                    $("#dataTableRoom #lst-roomandtable li").filter(function () {
                                        var element = $(this);
                                        let idcategory = $(this).data("idcategory");
                                        if (ididcategory == idcategory || ididcategory == 0) {
                                            element.css('display', "flex");
                                        } else {
                                            element.css('display', "none");

                                        }
                                    });
                                    $(this).parents("ul.litstbale").find("li.active").removeClass("active");
                                    $(this).parent("li").addClass("active");
                                });
                                //sự kiện kích chọn bàn
                                posStaff.evntSelectTabel();

                                //posStaff.eventCheckLoaddatatableOld();
                                // sự kiện kích vào bàn khi đã lưu trước đó.
                            }
                        },
                        error: function (err) {
                            console.log(err)
                        }
                    });
                }
                $("#dataTableRoom").toggleClass("d-none");
            }
            else {
                $("#dataTableRoom").toggleClass("d-none");
            }

            $(this).removeAttr("disabled");
        });
    },
    eventCheckLoaddatatableOld: function () {
        if (typeof (Storage) !== "undefined") {
            // Store
            var _getActivetable = localStorage.getItem("activeTableStaff");
            if (!isNaN(parseInt(_getActivetable))) {
                _getActivetable = parseInt(_getActivetable) + 1;
                if ($("#lst-roomandtable li").length < _getActivetable) {
                    _getActivetable = 1;
                }
                // $("#lst-roomandtable li:nth-child(" + _getActivetable + ")").trigger('click'); // là số thì active cái đó
                let ele = $("#lst-roomandtable li:nth-child(" + _getActivetable + ")"); // là số thì active cái đó
                ele.addClass("CurentOrder");
                posStaff.getdatataorderbytable(ele.data("id"));
                //load name

                posStaff.eventloadShownametable($(ele));//load tên table vàobutton

            }
            // Retrieve
        } else {
            alert("Liên hệ đội ngủ hỗ  trợ lỗi trình duyệt của bạn không hỗ  trợ Storage");
        }
    },
    eventloadShownametable: function (sel) {
        let idata = $(sel).data("id");
        let text = $(sel).children("b").html();
        $(".topbuton button:first").data("idtable", idata);
        $(".topbuton button:first").children(".tablename").html(text);
        $(".topbuton button:first").children(".tablename").removeClass("required");

    },
    evntSelectTabel: function () {//kích vào bàn
        $("#lst-roomandtable li").click(async function () {
            $(".bottomFix").remove();
            //if (!$(this).hasClass("CurentOrder")) {// chỉ cho kích 1 lần trên 1 table

            $("#lst-roomandtable").find("li.CurentOrder").not($(this)).removeClass("CurentOrder");
            $(this).addClass("CurentOrder");
            let id = $(this).data("id");
            await posStaff.getdatataorderbytable(id);// phải load dữ liệu trước


            posStaff.eventloadShownametable($(this));//load tên table vàobutton

            posStaff.eventactiveTableloca($(this).index());
            //$(".topbuton button:first").trigger("click");//kích lại cho ẩn đi
            // }

        });
    }
}

var PurchaseReturns = {
    loadEvnetFormatTable: function () {
        $("#tablePurchaseOrder tbody tr input").unbind();
        $("#tablePurchaseOrder tbody tr input.number3").each(function (i, item) {
            var _val = $(this).val();
            $(this).val(_val.replaceAll(',', ''))

        });
        $("#tablePurchaseOrder tbody tr input.number3")
            .each(function (i, item) {
                var _val = $(this).val();
                $(this).val(_val.replaceAll(',', '.'));
                fnInitialFormatNumber(this);
            }).ForceNumericOnly()
            .focus(function () {
                var _val = $(this).val();
                $(this).val(_val.replaceAll(',', ''));
            })
            .focusout(function () {
                fnFocusOut(this);
            });
    },
    autocompleteproduct: function () {
        $("#txtPurchaseOrderSearch").autocomplete(
            {
                appendTo: "#parentautocomplete",
                autoFocus: true,
                minLength: 1,
                delay: 0,
                source: function (request, response) {
                    $.ajax({
                        global: false,
                        url: "/Selling/Pos/SearchProductPos",
                        type: "GET",
                        dataType: "json",
                        data: {
                            text: request.term,
                        },
                        // html: true,
                        success: function (data) {
                            response($.map(data, function (item) {
                                let texthigh = __highlight(item.name, request.term);
                                let htmltonkho = "<span class='ton'>Tồn kho: " + parseFloat(item.quantity).format0VND(3, 3, '') + "<span>";
                                if (data.isInventory || data.typeProductCategory == EnumTypeProductCategory.COMBO || data.typeProductCategory == EnumTypeProductCategory.SERVICE) {
                                    htmltonkho = "<span class='ton'>Tồn kho: -- <span>";
                                }
                                let html =
                                    "<a href='javascript:void(0)'><div class='search-auto'>" +
                                    "<div class='img'><img src='../../../" + item.img + "'></div>" +
                                    "<div class='tk_name'><span>" + texthigh + " (" + item.code + ")</span>" +
                                    "<span class='price'>Giá: " + parseFloat(item.price).format0VND(3, 3, '') + "</span>" + htmltonkho + " </div></div></a>";
                                return {
                                    //label: html, value: item.code + " " + item.name, idProduct: item.id
                                    label: html, value: item.code, idProduct: item.id, price: item.price, quantity: item.quantity, name: item.name
                                };
                            }))
                            return { label: request.term, value: request.term };
                        },
                    })
                },
                html: true,
                select: function (e, ui) {
                    $(this).val(ui.item.value);
                    $(this).select();
                    PurchaseReturns.loadeventselectTable(ui.item.name, ui.item.price, ui.item.value, ui.item.quantity);
                },
                response: function () {
                    // $(this).select()
                }
            }).focus(function () {
                $(this).autocomplete("search");
                $(this).select();
            });
        $("#txtPurchaseOrderSearch").keypress(async function (event) {
            let _dt = $(this).val();
            if (event.keyCode == 13 && _dt.length > 0) {
                let check = await PurchaseOrder.loadEventEnterInputAddPro(_dt);
                $(this).select();
                if (!check) {
                    let id = parseInt($("#iddataPurchaseReturns").data("id")) || 0;
                    if (id == 0) {
                        toastrcus.error("Mã sản phẩm không hợp lệ!");
                    } else {
                        toastrcus.error("Hàng hóa không nằm trong đơn hàng nhập không thể trả hàng!");
                    }
                    return;
                }
            }
        });
    },
    loadeventselectTable: async function (name, price, code, quantity = 0) {//chọn select text khi sổ xuống lúc autocomplete
        let check = await PurchaseOrder.loadEventEnterInputAddPro(code)
        if (!check) {
            let getidtrahang = parseInt($("#iddataPurchaseReturns").data("id"));
            if (getidtrahang > 0) {
                toastrcus.error("Hàng hóa không nằm trong đơn hàng nhập không thể trả hàng!");
                return false;
            }
            $("#tablePurchaseOrder tbody tr.nodata").remove();
            let dem = $("#tablePurchaseOrder tbody tr").length + 1;
            //let htmlinput = "";
            //if (getidtrahang ==0) {
            //    htmlinput = `<div class="oldquantity">/<span class="number3">@item.Quantity.ToString()</span></div>`;
            //}
            let html = `<tr>
                          <td>`+ dem + `</td>
                          <td class="code">`+ code + `</td>
                          <td class="name" data-name="`+ name + `">` + name + `</td>
                          <td style="width: 175px;">
                            <div class="input-group">
                              <div class="input-group-prepend">
                                <span class="input-group-text minus-quantity" id="basic-addon1"><i class="fas fa-minus"></i></span>
                              </div>
                              <input type="text" class="form-control number3 text-center quantity" data-quanold="0" data-tonkho="`+ parseFloat(quantity.replaceAll(",", ".")) + `" data-quantity="` + 1 + `" placeholder="Nhập số lượng" value="` + 1 + `">
                              <div class="input-group-prepend">
                                <span class="input-group-text add-quantity" id="basic-addon1"><i class="fas fa-plus"></i></span>
                              </div>
                            </div>
                          </td>
                          <td class=""><input type="text" class="text-right form-control number3 price" data-price="`+ parseFloat(price) + `" value="` + parseFloat(price) + `"/></td>
                          <td class=""><input type="text" class="text-right form-control number3 discountpro" data-discount="0" value="0"/></td>
                          <td class=""><input type="text" readonly disabled class="text-right number3 form-control amount" value="`+ parseFloat(price) + `"/></td>
                      </tr>`;

            if (dem == 1) {
                $("#tablePurchaseOrder tbody").html(html);
            } else {
                $("#tablePurchaseOrder tbody tr:last").after(html);
            }


            PurchaseReturns.loadEvnetFormatTable();
            PurchaseReturns.loadeventUpdateAmount();
            PurchaseReturns.loadEventChangeQuantity();
            PurchaseReturns.loadEventChangePrice();
            PurchaseReturns.loadEventChangediscountProduct();
            PurchaseReturns.loadEventActionQuantity();
        }

    },
    loadeventbutonSubmit: function () {
        $(".btn-saves").click(function () {
            PurchaseReturns.loadeventSave(EnumStatusPurchaseOrder.DA_TRA_HANG);
        });
        $(".btn-temporarySave").click(function () {
            PurchaseReturns.loadeventSave(EnumStatusPurchaseOrder.PHIEU_TAM);
        });
    },
    loadEventChangeDisCount: function () {
        $(".discount").change(function () {

            let discountold = $(this).data("discount");
            let discountammount = parseFloat($(this).val().replaceAll(",", "")) || 0;
            if (discountammount >= 0) {
                $(this).data("discount", discountammount);
            } else {
                toastrcus.error("Vui lòng nhập số lượng lớn hơn 0");
                $(this).val(parseFloat(discountold).format0VND(3, 3, ""));
                return;
            }
            PurchaseReturns.loadeventUpdateAmount();
        });
        $(".paymentSupplier").change(function () {
            $(this).data("payment", 1);
            let _paymentSupplier = parseFloat($(this).val().replaceAll(",", "")) || 0;
            $(this).data("value", _paymentSupplier);
            $(".discount").trigger("change");

        });
    },//sự kiện thay đổi giảm giá bên ngoài
    loadeventSave: function (status) {
        let Products = [];
        if ($("#tablePurchaseOrder tbody tr:not(.nodata)").length == 0) {
            Swal.fire({
                title: 'Đơn trả hàng đang trống vui lòng chọn hàng hóa?',
                text: "Bạn có muốn tiếp tục nhập đơn hàng không?",
                icon: 'error',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Trở về danh sách trả hàng!',
                cancelButtonText: 'Đồng ý'
            }).then((result) => {
                if (result.isConfirmed) {
                    location.href = "/selling/purchaseorder/PurchaseReturns";
                }
            })
        } else {
            $("#tablePurchaseOrder tbody tr").map(function () {
                let _code = $(this).find(".code").text();
                let _name = $(this).find(".name").data("name");
                let _quantity = parseFloat($(this).find(".quantity").val().replaceAll(",", ""));
                let _price = parseFloat($(this).find(".price").val().replaceAll(",", ""));
                let _discountpro = parseFloat($(this).find(".discountpro").val().replaceAll(",", ""));
                let _amount = parseFloat($(this).find(".amount").val().replaceAll(",", ""));
                if (_quantity <= 0) {
                    toastrcus.error("Số lượng phải lớn hơn không của hàng hóa: " + _name);
                    return;
                }
                let Product = {};
                Product.Code = _code;
                Product.Name = _name;
                Product.Quantity = _quantity;
                Product.Price = _price;
                Product.DiscountAmount = _discountpro;
                Product.Total = _amount;
                Products.push(Product);
            });
            if (Products.length == 0) {
                toastrcus.error("Vui lòng lựa chọn hàng hóa cần nhập hàng");
                return;
            }
            let sel = $("form#create-formProduct");
            let _totalAll = parseFloat(sel.find(".totalAll").data("value")) || 0;//tiền chưa giảm
            let _amountAll = parseFloat(sel.find(".amountAll").data("value")) || 0;//tổng tiền đã giảm
            let _discount = parseFloat(sel.find(".discount").data("discount")) || 0;//tiền giảm
            let _quantityAll = parseFloat(sel.find(".quantityAll").data("value")) || 0;//tổng sl
            let _paymentSupplier = parseFloat(sel.find(".paymentSupplier").data("value")) || 0;//tố tiền nhà cung cấp trả
            let paydebt = parseFloat(sel.find(".paydebt").data("value")) || 0;//tiền ghi nọ

            let _note = sel.find(".notePurchaseOrder").val();
            let typePurchaseOrder = TypePurchaseOrder.TRA_HANG_NHAP;
            let iddataPurchaseReturns = $("#iddataPurchaseReturns").data("id");
            let createDate = $("#CreateDate").val();
            $.ajax({
                type: 'POST',
                url: '/Selling/PurchaseOrder/Create',
                data: {
                    IdPayment: $(".PaymentMethod").val(),
                    IdSuppliers: $(".Suppliersid").val(),
                    JsonItem: JSON.stringify(Products),
                    Quantity: _quantityAll,
                    Total: _totalAll,
                    DiscountAmount: _discount,
                    Amount: _amountAll,
                    AmountSuppliers: _paymentSupplier,
                    DebtAmount: paydebt,
                    Note: _note,
                    Type: typePurchaseOrder,
                    IdPurchaseOrder: iddataPurchaseReturns,
                    CreateDate: createDate,
                    Status: status
                    // Status: EnumStatusPurchaseOrder.DA_NHAP_HANG,
                },

                success: function (res) {
                    if (res.isValid) {
                        Swal.fire({
                            title: 'Trả hàng thành công?',
                            text: "Bạn có muốn tạo đơn trả hàng mới không?",
                            icon: 'success',
                            showCancelButton: true,
                            confirmButtonColor: '#3085d6',
                            cancelButtonColor: '#d33',
                            confirmButtonText: 'Trở về danh sách trả hàng!',
                            cancelButtonText: 'Đồng ý'
                        }).then((result) => {
                            if (result.isConfirmed) {
                                location.href = "/selling/purchaseorder/indexreturn";
                            } else {
                                location.href = "/selling/purchaseorder/PurchaseReturns";
                            }
                        })
                    }
                },
                error: function (err) {
                    console.log(err)
                }
            });
        }

    },
    loadCheckTrahangNhap: function () {
        if ($("#tablePurchaseOrder tbody tr:not(.nodata)").length == 0 && parseInt($("#iddataPurchaseReturns").data("id")) > 0) {
            Swal.fire({
                title: 'Đơn hàng rỗng?',
                text: "Đơn hàng hiện đã trả hết hàng, không có sản phẩm nào cần trả?",
                icon: 'error',
                allowOutsideClick: false,
                showCancelButton: false,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Trở về danh sách nhập hàng!',
                cancelButtonText: 'Đồng ý'
            }).then((result) => {
                if (result.isConfirmed) {
                    location.href = "/selling/purchaseorder";
                }
            })
        }
    },
    loadEventActionQuantity: function () {
        $("#tablePurchaseOrder tbody tr .minus-quantity").click(function () {
            let data_quantity = $(this).parents("tr").find(".quantity").data("quantity");


            let quantity = parseFloat($(this).parents("tr").find(".quantity").val().replaceAll(",", "")) || 0;
            quantity = quantity - 1;
            if (quantity > 0) {
                let price = parseFloat($(this).parents("tr").find(".price").val().replaceAll(",", "")) || 0;
                let discount = parseFloat($(this).parents("tr").find(".discountpro").val().replaceAll(",", "")) || 0;
                let amount = (quantity * price) - discount;
                $(this).parents("tr").find(".amount").val(amount.format0VND(3, 3, ""));
                $(this).parents("tr").find(".quantity").val(quantity.format0VND(3, 3, ""));
                $(this).data("quantity", quantity);
            } else {
                toastrcus.error("Vui lòng nhập số lượng lớn hơn 0");
                $(this).val(data_quantity);
                return;
            }
            PurchaseReturns.loadeventUpdateAmount();
        });
        $("#tablePurchaseOrder tbody tr .add-quantity").click(function () {

            let data_quantity = $(this).parents("tr").find(".quantity").data("quantity");
            let name = $(this).parents("tr").find(".name").data("name");
            let getidtrahang = parseInt($("#iddataPurchaseReturns").data("id"));
            let data_quantityold = parseInt($(this).parents("tr").find(".quantity").data("quanold").toString().replaceAll(",", "."));

            let quantity = parseFloat($(this).parents("tr").find(".quantity").val().replaceAll(",", "")) || 0;
            quantity = quantity + 1;

            if (getidtrahang > 0 && quantity > data_quantityold) {
                toastrcus.error("Vui lòng nhập số lượng hàng trả không vượt quá số lượng hàng nhập");
                $(this).val(data_quantity);
                return;
            }

            if (quantity > 0) {
                let tonkho = parseInt($(this).parents("tr").find(".quantity").data("tonkho"));
                if (quantity > tonkho) {
                    toastrcus.error("Sản phẩm " + name + " không đủ số lượng tồn kho để trả hàng, vui lòng kiểm tra lại");
                    $(this).val(data_quantity);
                    return;
                }
                let price = parseFloat($(this).parents("tr").find(".price").val().replaceAll(",", "")) || 0;
                let discount = parseFloat($(this).parents("tr").find(".discountpro").val().replaceAll(",", "")) || 0;
                let amount = (quantity * price) - discount;
                $(this).parents("tr").find(".amount").val(amount.format0VND(3, 3, ""));
                $(this).parents("tr").find(".quantity").val(quantity.format0VND(3, 3, ""));
                $(this).parents("tr").find(".quantity").data("quantity", quantity);
            } else {
                toastrcus.error("Vui lòng nhập số lượng lớn hơn 0");
                $(this).val(data_quantity);
                return;
            }
            PurchaseReturns.loadeventUpdateAmount();
        });
    },//sự kiện thay đổi nút tăng giảm sl
    loadeventUpdateAmount: function () {
        let _quantity = 0;
        let _amount = 0;
        if ($("#tablePurchaseOrder tbody tr:not(.nodata)").length > 0) {
            $("#tablePurchaseOrder tbody tr").map(function () {
                _quantity += parseFloat($(this).find(".quantity").val().replaceAll(",", "")) || 0;
                _amount += parseFloat($(this).find(".amount").val().replaceAll(",", "")) || 0;
            });
        }
        $(".quantityAll").html(_quantity.format0VND(3, 3, ""));
        $(".quantityAll").data("value", _quantity);
        $(".totalAll").html(_amount.format0VND(3, 3, ""));
        $(".totalAll").data("value", _amount);
        let _discount = parseFloat($(".discount").val().replaceAll(",", "")) || 0;
        let amountAll = _amount - _discount;
        $(".amountAll").html(amountAll.format0VND(3, 3, ""));
        $(".amountAll").data("value", amountAll);
        if (parseInt($(".paymentSupplier").data("payment")) == 0) {
            amountAll = amountAll * -1;
            $(".paydebt").html(amountAll.format0VND(3, 3, ""));
            $(".paydebt").data("value", amountAll);
            $(".paymentSupplier").val(0);
            $(".paymentSupplier").data("value", 0);
        } else {
            let _paymentSupplier = parseFloat($(".paymentSupplier").val().replaceAll(",", "")) || 0;
            $(".paydebt").html((_paymentSupplier - amountAll).format0VND(3, 3, ""));
            $(".paydebt").data("value", (_paymentSupplier - amountAll));
        }

    },//sự kiện update lại tiền
    loadEventChangeQuantity: function () {

        $("#tablePurchaseOrder tbody tr input.quantity").change(function () {

            let data_quantity = $(this).data("quantity");
            let quanold = parseFloat($(this).data("quanold").toString().replaceAll(",", "."));
            //let getidtrahang = parseInt($("#iddataPurchaseReturns").data("id"));
            let quantity = parseFloat($(this).val().replaceAll(",", "")) || 0;
            if (quantity > 0) {
                if (quantity > quanold) {
                    toastrcus.error("Số lượng hàng trả không vượt quá số lượng hàng nhập");
                    $(this).val(data_quantity);
                    return;
                } else {
                    let price = parseFloat($(this).parents("tr").find(".price").val().replaceAll(",", "")) || 0;
                    let discount = parseFloat($(this).parents("tr").find(".discountpro").val().replaceAll(",", "")) || 0;
                    let amount = (quantity * price) - discount;
                    $(this).parents("tr").find(".amount").val(amount.format0VND(3, 3, ""));
                    $(this).data("quantity", quantity);
                }
            } else {
                toastrcus.error("Vui lòng nhập số lượng lớn hơn 0");
                $(this).val(data_quantity);
                return;
            }

            PurchaseReturns.loadeventUpdateAmount();
        });
    },//sự kiện thay đổi sl
    loadEventChangePrice: function () {

        $("#tablePurchaseOrder tbody tr input.price").change(function () {
            let data_price = $(this).data("price");
            let price = parseFloat($(this).val().replaceAll(",", "")) || 0;
            if (price > 0) {
                let quantity = parseFloat($(this).parents("tr").find(".quantity").val().replaceAll(",", "")) || 0;
                let discount = parseFloat($(this).parents("tr").find(".discountpro").val().replaceAll(",", "")) || 0;
                let amount = (quantity * price) - discount;
                $(this).parents("tr").find(".amount").val(amount.format0VND(3, 3, ""));
                $(this).data("price", price);
            } else {
                toastrcus.error("Vui lòng nhập số lượng lớn hơn 0");
                $(this).val(parseFloat(data_price).format0VND(3, 3, ""));
                return;
            }
            PurchaseReturns.loadeventUpdateAmount();
        });
    },//sự kiện thay đổi giá
    loadEventChangediscountProduct: function () {
        $("#tablePurchaseOrder tbody tr input.discountpro").change(function () {
            let data_discount = $(this).data("discount");
            let discount = parseFloat($(this).val().replaceAll(",", "")) || 0;
            if (discount >= 0) {
                let quantity = parseFloat($(this).parents("tr").find(".quantity").val().replaceAll(",", "")) || 0;
                let price = parseFloat($(this).parents("tr").find(".price").val().replaceAll(",", "")) || 0;
                let amount = (quantity * price) - discount;
                $(this).parents("tr").find(".amount").val(amount.format0VND(3, 3, ""));
                $(this).data("discount", data_discount);
            } else {
                toastrcus.error("Vui lòng nhập số lượng lớn hơn 0");
                $(this).val(parseFloat(data_discount).format0VND(3, 3, ""));
                return;
            }
            PurchaseReturns.loadeventUpdateAmount();
        });
    },
    loadeventShowdetail: function (id, Type) {
        $.ajax({
            type: 'GET',
            //global: false,
            url: "/Selling/purchaseorder/View?secret=" + id + "&Type=" + Type,
            // data: fomrdata,
            contentType: false,
            processData: false,
            success: function (res) {
                if (res.isValid) {
                    Swal.fire({
                        // icon: 'success',
                        position: 'top-end',
                        showClass: {
                            popup: `
                              popup-formcreate
                                popupform-payment
                               animate__animated
                              animate__fadeInRight
                              animate__faster
                            `
                        },
                        hideClass: {
                            popup: "popup-formcreate animate__animated popupform-payment animate__fadeOutRight animate__faster"

                        },
                        showCloseButton: true,

                        title: "Chi tiết đơn hàng nhập",
                        html: res.html,
                        //showClass: {
                        //    popup: 'popup-formcreate'
                        //},

                        footer: "<button class='btn btn-primary btn-continue mr-3'><i class='fas fa-cancel'></i>Đóng</button><button class='mr-3 btn btn-purchaseorder btn-warning'><i class='fas fa-check mr-2'></i>Lưu</button>",
                        allowOutsideClick: true,
                        showConfirmButton: false,
                        showCancelButton: false,
                        cancelButtonText: '<i class="fa fa-window-close"></i> Đóng',
                        didRender: () => {
                            loaddaterangepicker(true);
                            $('.number3').each(function () {
                                let idtex = $(this).text().replaceAll(".", "");
                                $(this).html(parseFloat(idtex).format0VND(0, 3, ""))
                            });
                            if (res.purchaseOrderCode == null || res.purchaseOrderCode == "") {
                                loaddataSelect2("/API/Handling/GetAllSupller", ".Suppliersid", res.idSuppliers, "Chọn nhà cung cấp");
                            }
                            loaddataSelect2("/API/Handling/GetAllPaymentMethod", ".PaymentMethod", res.idPayment, "Chọn hình thức thanh toán");
                            $(".btn-purchaseorder").data("id", res.idData)
                            $(".btn-continue").click(function () {
                                Swal.close();
                            });

                            $(".btn-purchaseorder").click(function () {
                                PurchaseReturns.eventSaveIndetailt($(this).data("id"));
                            });
                        }
                    });
                }
            },
            error: function (err) {
                console.log(err)
            }
        });
    },
    eventSaveIndetailt: function (id) {
        let createDate = $("#CreateDate").val();
        if (createDate.trim() == "") {
            toastrcus.error("Vui lòng chọn ngày trả hàng");
            return;
        }
        let notePurchaseReturns = $("#NotePurchaseReturns").val();
        let typePurchaseOrder = TypePurchaseOrder.TRA_HANG_NHAP;
        $.ajax({
            type: 'POST',
            url: '/Selling/PurchaseOrder/Update',
            data: {
                IdPayment: $(".PaymentMethod").val(),
                IdSuppliers: $(".Suppliersid").val(),
                Id: id,
                Note: notePurchaseReturns,
                CreateDate: createDate,
                Type: typePurchaseOrder
            },

            success: function (res) {
                if (res.isValid) {
                    Swal.fire({
                        title: 'Cập nhật thành công?',
                        text: "Bạn có muốn tiếp tục chỉnh sửa không?",
                        icon: 'success',
                        showCancelButton: true,
                        confirmButtonColor: '#3085d6',
                        cancelButtonColor: '#d33',
                        confirmButtonText: 'Trở về danh sách trả hàng!',
                        cancelButtonText: 'Đồng ý'
                    }).then((result) => {
                        if (result.isConfirmed) {
                            dataTableOut.draw('page');
                            //dataTableOut.ajax.reload(null, false);
                            Swal.close();
                        }
                    })
                }
            },
            error: function (err) {
                console.log(err)
            }
        });
    }
}
var PurchaseOrder = {
    autocompleteproduct: function () {
        $("#txtPurchaseOrderSearch").autocomplete(
            {
                appendTo: "#parentautocomplete",
                autoFocus: true,
                minLength: 1,
                delay: 0,
                source: function (request, response) {
                    $.ajax({
                        global: false,
                        url: "/Selling/Pos/SearchProductPos",
                        type: "GET",
                        dataType: "json",
                        data: {
                            text: request.term,
                        },
                        // html: true,
                        success: function (data) {
                            response($.map(data, function (item) {
                                let texthigh = __highlight(item.name, request.term);
                                let htmltonkho = "<span class='ton'>Tồn kho: " + parseFloat(item.quantity).format0VND(3, 3, '') + "<span>";
                                if (data.isInventory || data.typeProductCategory == EnumTypeProductCategory.COMBO || data.typeProductCategory == EnumTypeProductCategory.SERVICE) {
                                    htmltonkho = "<span class='ton'>Tồn kho: -- <span>";
                                }
                                let html =
                                    "<a href='javascript:void(0)'><div class='search-auto'>" +
                                    "<div class='img'><img src='../../" + item.img + "'></div>" +
                                    "<div class='tk_name'><span>" + texthigh + " (" + item.code + ")</span>" +
                                    "<span class='price'>Giá: " + parseFloat(item.price).format0VND(3, 3, '') + "</span>" + htmltonkho + "</div></div></a>";
                                return {
                                    //label: html, value: item.code + " " + item.name, idProduct: item.id
                                    label: html, value: item.code, idProduct: item.id, price: item.price, quantity: item.quantity, name: item.name
                                };
                            }))
                            return { label: request.term, value: request.term };
                        },
                    })
                },
                html: true,
                select: function (e, ui) {
                    $(this).val(ui.item.value);
                    $(this).select();
                    PurchaseOrder.loadeventselectTable(ui.item.name, ui.item.price, ui.item.value);
                },
                response: function () {
                    // $(this).select()
                }
            }).focus(function () {
                $(this).autocomplete("search");
                $(this).select();
            });
        $("#txtPurchaseOrderSearch").keypress(function (event) {
            let _dt = $(this).val();
            if (event.keyCode == 13 && _dt.length > 0) {
                PurchaseOrder.loadEventEnterInputAddPro(_dt);
                $(this).select();
            } else if (_dt.length == 0) {
                idProductAutocom = 0;
            }
        });
    },
    loadEventEnterInputAddPro: async function (code) {
        let isValid = false;
        $("#tablePurchaseOrder tbody tr").map(function () {
            let _code = $(this).find(".code").text();
            if (_code.trim() == code) {
                isValid = true;
                $(this).find(".add-quantity").trigger("click");
            }
        });
        return isValid;
    },
    loadEvnetFormatTable: function () {
        $("#tablePurchaseOrder tbody tr input").unbind();
        $("#tablePurchaseOrder tbody tr input.number3").each(function (i, item) {
            var _val = $(this).val();
            $(this).val(_val.replaceAll(',', ''))

        });
        $("#tablePurchaseOrder tbody tr input.number3")
            .each(function (i, item) {
                var _val = $(this).val();
                $(this).val(_val.replaceAll(',', '.'));
                fnInitialFormatNumber(this);
            }).ForceNumericOnly()
            .focus(function () {
                var _val = $(this).val();
                $(this).val(_val.replaceAll(',', ''));
            })
            .focusout(function () {
                fnFocusOut(this);
            });
    },
    loadeventselectTable: async function (name, price, code) {//chọn select text khi sổ xuống lúc autocomplete
        $("#tablePurchaseOrder tbody tr.nodata").remove();
        let dem = $("#tablePurchaseOrder tbody tr").length + 1;

        let check = await PurchaseOrder.loadEventEnterInputAddPro(code)
        if (!check) {
            let html = `<tr>
                          <td>`+ dem + `</td>
                          <td class="code">`+ code + `</td>
                          <td class="name" data-name="`+ name + `">` + name + `</td>
                          <td style="width: 175px;">
                            <div class="input-group">
                              <div class="input-group-prepend">
                                <span class="input-group-text minus-quantity" id="basic-addon1"><i class="fas fa-minus"></i></span>
                              </div>
                              <input type="text" class="form-control number3 text-center quantity" data-quantity="`+ 1 + `" placeholder="Nhập số lượng" value="` + 1 + `">
                              <div class="input-group-prepend">
                                <span class="input-group-text add-quantity" id="basic-addon1"><i class="fas fa-plus"></i></span>
                              </div>
                            </div>
                          </td>
                          <td class=""><input type="text" class="text-right form-control number3 price" data-quantity="`+ parseFloat(price) + `" value="` + parseFloat(price) + `"/></td>
                          <td class=""><input type="text" class="text-right form-control number3 discountpro" data-discount="0" value="0"/></td>
                          <td class=""><input type="text" readonly disabled class="text-right number3 form-control amount" value="`+ parseFloat(price) + `"/></td>
                    </tr>`;

            if (dem == 1) {
                $("#tablePurchaseOrder tbody").html(html);
            } else {
                $("#tablePurchaseOrder tbody tr:last").after(html);
            }


            PurchaseOrder.loadEvnetFormatTable();
            PurchaseOrder.loadeventUpdateAmount();
            PurchaseOrder.loadEventChangeQuantity();
            PurchaseOrder.loadEventChangePrice();
            PurchaseOrder.loadEventChangediscountProduct();
            PurchaseOrder.loadEventActionQuantity();
        }

    },
    loadEventActionQuantity: function () {
        $("#tablePurchaseOrder tbody tr .minus-quantity").click(function () {
            let data_quantity = $(this).parents("tr").find(".quantity").data("quantity");
            let quantity = parseFloat($(this).parents("tr").find(".quantity").val().replaceAll(",", "")) || 0;
            quantity = quantity - 1;
            if (quantity > 0) {
                let price = parseFloat($(this).parents("tr").find(".price").val().replaceAll(",", "")) || 0;
                let discount = parseFloat($(this).parents("tr").find(".discountpro").val().replaceAll(",", "")) || 0;
                let amount = (quantity * price) - discount;
                $(this).parents("tr").find(".amount").val(amount.format0VND(3, 3, ""));
                $(this).parents("tr").find(".quantity").val(quantity.format0VND(3, 3, ""));
                $(this).data("quantity", quantity);
            } else {
                toastrcus.error("Vui lòng nhập số lượng lớn hơn 0");
                $(this).val(data_quantity);
                return;
            }
            PurchaseOrder.loadeventUpdateAmount();
        });
        $("#tablePurchaseOrder tbody tr .add-quantity").click(function () {
            let data_quantity = $(this).parents("tr").find(".quantity").data("quantity");
            let quantity = parseFloat($(this).parents("tr").find(".quantity").val().replaceAll(",", "")) || 0;
            quantity = quantity + 1;
            if (quantity > 0) {
                let price = parseFloat($(this).parents("tr").find(".price").val().replaceAll(",", "")) || 0;
                let discount = parseFloat($(this).parents("tr").find(".discountpro").val().replaceAll(",", "")) || 0;
                let amount = (quantity * price) - discount;
                $(this).parents("tr").find(".amount").val(amount.format0VND(3, 3, ""));
                $(this).parents("tr").find(".quantity").val(quantity.format0VND(3, 3, ""));
                $(this).parents("tr").find(".quantity").data("quantity", quantity);
            } else {
                toastrcus.error("Vui lòng nhập số lượng lớn hơn 0");
                $(this).val(data_quantity);
                return;
            }
            PurchaseOrder.loadeventUpdateAmount();
        });
    },//sự kiện thay đổi nút tăng giảm sl
    loadEventChangeQuantity: function () {

        $("#tablePurchaseOrder tbody tr input.quantity").change(function () {

            let data_quantity = $(this).data("quantity");
            let quantity = parseFloat($(this).val().replaceAll(",", "")) || 0;
            if (quantity > 0) {
                let price = parseFloat($(this).parents("tr").find(".price").val().replaceAll(",", "")) || 0;
                let discount = parseFloat($(this).parents("tr").find(".discountpro").val().replaceAll(",", "")) || 0;
                let amount = (quantity * price) - discount;
                $(this).parents("tr").find(".amount").val(amount.format0VND(3, 3, ""));
                $(this).data("quantity", quantity);
            } else {
                toastrcus.error("Vui lòng nhập số lượng lớn hơn 0");
                $(this).val(data_quantity);
                return;
            }

            PurchaseOrder.loadeventUpdateAmount();
        });
    },//sự kiện thay đổi sl
    loadEventChangePrice: function () {
        $("#tablePurchaseOrder tbody tr input.price").change(function () {
            let data_price = $(this).data("price");
            let price = parseFloat($(this).val().replaceAll(",", "")) || 0;
            if (price > 0) {
                let quantity = parseFloat($(this).parents("tr").find(".quantity").val().replaceAll(",", "")) || 0;
                let discount = parseFloat($(this).parents("tr").find(".discountpro").val().replaceAll(",", "")) || 0;
                let amount = (quantity * price) - discount;
                $(this).parents("tr").find(".amount").val(amount.format0VND(3, 3, ""));
                $(this).data("price", price);
            } else {
                toastrcus.error("Vui lòng nhập số lượng lớn hơn 0");
                $(this).val(parseFloat(data_price).format0VND(3, 3, ""));
                return;
            }
            PurchaseOrder.loadeventUpdateAmount();
        });
    },//sự kiện thay đổi giá
    loadEventChangediscountProduct: function () {
        $("#tablePurchaseOrder tbody tr input.discountpro").change(function () {
            let data_discount = $(this).data("discount");
            let discount = parseFloat($(this).val().replaceAll(",", "")) || 0;
            if (discount >= 0) {
                let quantity = parseFloat($(this).parents("tr").find(".quantity").val().replaceAll(",", "")) || 0;
                let price = parseFloat($(this).parents("tr").find(".price").val().replaceAll(",", "")) || 0;
                let amount = (quantity * price) - discount;
                $(this).parents("tr").find(".amount").val(amount.format0VND(3, 3, ""));
                $(this).data("discount", data_discount);
            } else {
                toastrcus.error("Vui lòng nhập số lượng lớn hơn 0");
                $(this).val(parseFloat(data_discount).format0VND(3, 3, ""));
                return;
            }
            PurchaseOrder.loadeventUpdateAmount();
        });
    },//sự kiện thay giảm giá trong sản phẩm

    loadEventChangeDisCount: function () {
        $(".discount").change(function () {

            let discountold = $(this).data("discount");
            let discountammount = parseFloat($(this).val().replaceAll(",", "")) || 0;
            if (discountammount >= 0) {
                $(this).data("discount", discountammount);
            } else {
                toastrcus.error("Vui lòng nhập số lượng lớn hơn 0");
                $(this).val(parseFloat(discountold).format0VND(3, 3, ""));
                return;
            }
            PurchaseOrder.loadeventUpdateAmount();
        });
        $(".paymentSupplier").change(function () {
            $(this).data("payment", 1);
            let _paymentSupplier = parseFloat($(this).val().replaceAll(",", "")) || 0;
            $(this).data("value", _paymentSupplier);
            $(".discount").trigger("change");

        });
    },//sự kiện thay đổi giảm giá bên ngoài

    loadeventUpdateAmount: function () {
        let _quantity = 0;
        let _amount = 0;
        if ($("#tablePurchaseOrder tbody tr:not(.nodata)").length > 0) {
            $("#tablePurchaseOrder tbody tr").map(function () {
                _quantity += parseFloat($(this).find(".quantity").val().replaceAll(",", "")) || 0;
                _amount += parseFloat($(this).find(".amount").val().replaceAll(",", "")) || 0;
            });

        }

        $(".quantityAll").html(_quantity.format0VND(3, 3, ""));
        $(".quantityAll").data("value", _quantity);
        $(".totalAll").html(_amount.format0VND(3, 3, ""));
        $(".totalAll").data("value", _amount);
        let _discount = parseFloat($(".discount").val().replaceAll(",", "")) || 0;
        let amountAll = _amount - _discount;
        $(".amountAll").html(amountAll.format0VND(3, 3, ""));
        $(".amountAll").data("value", amountAll);
        if (parseInt($(".paymentSupplier").data("payment")) == 0) {
            $(".paymentSupplier").val(amountAll.format0VND(3, 3, ""));
            $(".paymentSupplier").data("value", amountAll);
            $(".paydebt").data("value", 0);
        } else {
            let _paymentSupplier = parseFloat($(".paymentSupplier").val().replaceAll(",", "")) || 0;
            $(".paydebt").html((_paymentSupplier - amountAll).format0VND(3, 3, ""));
            $(".paydebt").data("value", (_paymentSupplier - amountAll));
        }

    },//sự kiện update lại tiền
    loadeventbutonSubmit: function () {
        $(".btn-saves").click(function () {
            PurchaseOrder.loadeventSave(EnumStatusPurchaseOrder.DA_NHAP_HANG);
        });
        $(".btn-temporarySave").click(function () {
            PurchaseOrder.loadeventSave(EnumStatusPurchaseOrder.PHIEU_TAM);
        });
    }
    ,
    loadeventSave: function (status) {
        let Products = [];
        if ($("#tablePurchaseOrder tbody tr:not(.nodata)").length == 0) {
            Swal.fire({
                title: 'Đơn nhập hàng đang trống vui lòng chọn hàng hóa?',
                text: "Bạn có muốn tiếp tục nhập đơn hàng không?",
                icon: 'error',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Trở về danh sách nhập hàng!',
                cancelButtonText: 'Đồng ý'
            }).then((result) => {
                if (result.isConfirmed) {
                    location.href = "/selling/purchaseorder";
                }
            })
        } else {
            if ($(".PaymentMethod").val() == "") {
                toastrcus.error("Vui lòng chọn hình thức thanh toán");
                return;
            }
            $("#tablePurchaseOrder tbody tr").map(function () {
                let _code = $(this).find(".code").text();
                let _name = $(this).find(".name").data("name");
                let _quantity = parseFloat($(this).find(".quantity").val().replaceAll(",", ""));
                let _price = parseFloat($(this).find(".price").val().replaceAll(",", ""));
                let _discountpro = parseFloat($(this).find(".discountpro").val().replaceAll(",", ""));
                let _amount = parseFloat($(this).find(".amount").val().replaceAll(",", ""));
                if (_quantity <= 0) {
                    toastrcus.error("Số lượng phải lớn hơn không của hàng hóa: " + _name);
                    return;
                }
                let Product = {};
                Product.Code = _code;
                Product.Name = _name;
                Product.Quantity = _quantity;
                Product.Price = _price;
                Product.DiscountAmount = _discountpro;
                Product.Total = _amount;
                Products.push(Product);
            });
            if (Products.length == 0) {
                toastrcus.error("Vui lòng lựa chọn hàng hóa cần nhập hàng");
                return;
            }
            let sel = $("form#create-formProduct");
            let _totalAll = parseFloat(sel.find(".totalAll").data("value")) || 0;//tiền chưa giảm
            let _amountAll = parseFloat(sel.find(".amountAll").data("value")) || 0;//tổng tiền đã giảm
            let _discount = parseFloat(sel.find(".discount").data("discount")) || 0;//tiền giảm
            let _quantityAll = parseFloat(sel.find(".quantityAll").data("value")) || 0;//tổng sl
            let _paymentSupplier = parseFloat(sel.find(".paymentSupplier").data("value")) || 0;//số tiền trả nhà cung cấp
            let paydebt = parseFloat(sel.find(".paydebt").data("value")) || 0;//tiền ghi nọ

            let _note = sel.find(".notePurchaseOrder").val();
            let typePurchaseOrder = TypePurchaseOrder.NHAP_HANG;
            //if ($("#EnumTypePurchaseOrder").data("id") == TypePurchaseOrder.TRA_HANG_NHAP) {
            //    TypePurchaseOrder = TypePurchaseOrder.TRA_HANG_NHAP;
            //}
            let createDate = $("#CreateDate").val();
            $.ajax({
                type: 'POST',
                //async: false,
                url: '/Selling/PurchaseOrder/Create',
                data: {
                    IdSuppliers: $(".Suppliersid").val(),
                    IdPayment: $(".PaymentMethod").val(),
                    JsonItem: JSON.stringify(Products),
                    Quantity: _quantityAll,
                    Total: _totalAll,
                    DiscountAmount: _discount,
                    Amount: _amountAll,
                    AmountSuppliers: _paymentSupplier,
                    DebtAmount: paydebt,
                    Note: _note,
                    CreateDate: createDate,
                    Type: typePurchaseOrder,
                    Status: status
                    // Status: EnumStatusPurchaseOrder.DA_NHAP_HANG,
                },

                success: function (res) {
                    if (res.isValid) {
                        Swal.fire({
                            title: 'Nhập hàng thành công?',
                            text: "Bạn có muốn tiếp tục nhập đơn hàng mới không?",
                            icon: 'success',
                            showCancelButton: true,
                            confirmButtonColor: '#3085d6',
                            cancelButtonColor: '#d33',
                            confirmButtonText: 'Trở về danh sách nhập hàng!',
                            cancelButtonText: 'Đồng ý'
                        }).then((result) => {
                            if (result.isConfirmed) {
                                location.href = "/selling/purchaseorder";
                            } else {
                                let tr =
                                    ` <tr class="nodata">
                                        <td colspan="7">Chưa có dữ liệu</td>
                                    </tr>`;
                                $("#txtPurchaseOrderSearch").val("");
                                $("#tablePurchaseOrder tbody").html(tr);
                                PurchaseOrder.loadeventUpdateAmount();

                            }
                        })
                    }
                },
                error: function (err) {
                    console.log(err)
                }
            });
        }

    }
    ,
    loadeventShowdetail: function (id, Type) {
        $.ajax({
            type: 'GET',
            //global: false,
            url: "/Selling/purchaseorder/View?secret=" + id + "&Type=" + Type,
            // data: fomrdata,
            contentType: false,
            processData: false,
            success: function (res) {
                if (res.isValid) {
                    Swal.fire({
                        // icon: 'success',
                        position: 'top-end',
                        showClass: {
                            popup: `
                              popup-formcreate
                                popupform-payment
                               animate__animated
                              animate__fadeInRight
                              animate__faster
                            `
                        },
                        hideClass: {
                            popup: "popup-formcreate animate__animated popupform-payment animate__fadeOutRight animate__faster"

                        },
                        showCloseButton: true,

                        title: "Chi tiết đơn hàng nhập",
                        html: res.html,
                        //showClass: {
                        //    popup: 'popup-formcreate'
                        //},

                        footer: "<button class='btn btn-primary btn-continue mr-3'><i class='fas fa-cancel'></i>Hủy bỏ</button><button class='mr-3 btn btn-save btn-success'><i class='fas fa-check mr-2'></i>Lưu</button><button class='mr-3 btn btn-purchaseorder btn-warning'><i class='fas fa-undo-alt mr-2'></i>Trả hàng</button>",
                        allowOutsideClick: true,
                        showConfirmButton: false,
                        showCancelButton: false,
                        cancelButtonText: '<i class="fa fa-window-close"></i> Đóng',
                        didRender: () => {
                            loaddaterangepicker(true);
                            $('.number3').each(function () {
                                let idtex = $(this).text().replaceAll(".", "");
                                $(this).html(parseFloat(idtex).format0VND(0, 3, ""))
                            });

                            $(".btn-continue").click(function () {
                                Swal.close();
                            });

                            $(".btn-purchaseorder").click(function () {
                                Swal.close();
                                location.href = "/selling/PurchaseOrder/PurchaseReturns/" + $("#PurchaseOrderId").val();
                            });
                            loaddataSelect2("/API/Handling/GetAllSupller", ".Suppliersid", res.idSuppliers);
                            loaddataSelect2("/API/Handling/GetAllPaymentMethod", ".PaymentMethod", res.idPayment, "Chọn hình thức thanh toán");
                            $(".btn-save").data("id", res.idData)
                            $(".btn-save").click(function () {
                                PurchaseOrder.eventSaveIndetailt($(this).data("id"));
                            });

                        }
                    });
                }
            },
            error: function (err) {
                console.log(err)
            }
        });
    },
    eventSaveIndetailt: function (id) {
        let createDate = $("#CreateDate").val();
        if (createDate.trim() == "") {
            toastrcus.error("Vui lòng chọn ngày nhận hàng");
            return;
        }
        let notePurchaseReturns = $("#NotePurchaseReturns").val();
        let typePurchaseOrder = TypePurchaseOrder.NHAP_HANG;
        $.ajax({
            type: 'POST',
            url: '/Selling/PurchaseOrder/Update',
            data: {
                IdSuppliers: $(".Suppliersid").val(),
                IdPayment: $(".PaymentMethod").val(),
                Id: id,
                Note: notePurchaseReturns,
                CreateDate: createDate,
                Type: typePurchaseOrder
            },

            success: function (res) {
                if (res.isValid) {
                    Swal.fire({
                        title: 'Cập nhật thành công?',
                        //text: "Bạn có muốn tiếp tục chỉnh sửa không?",
                        icon: 'success',
                        showCancelButton: false,
                        confirmButtonColor: '#3085d6',
                        cancelButtonColor: '#d33',
                        confirmButtonText: 'Trở về danh sách nhập hàng!',
                        cancelButtonText: 'Đồng ý'
                    }).then((result) => {
                        if (result.isConfirmed) {
                            dataTableOut.draw('page');
                            //dataTableOut.ajax.reload(null, false);
                            Swal.close();
                        }
                    })
                }
            },
            error: function (err) {
                console.log(err)
            }
        });
    }
}
var eventBanle = {
    enableOrDisnableSancanbarCode: function (is) {
        //https://github.com/kabachello/jQuery-Scanner-Detection
        if (is) {
            $(document).scannerDetection({
                timeBeforeScanTest: 200, // wait for the next character for upto 200ms
                avgTimeByChar: 100, // it's not a barcode if a character takes longer than 100ms
                onComplete: function (barcode, qty) {
                    if (barcode == "") {
                        toastrcus.error("Không tìm thấy mã barcode");
                    } else {
                        eventBanle.eventAddProductBySancanBarcode(barcode);
                    }

                }, // main callback function	  
                onError: function (barcode, qty) {

                } // main callback function	
            });
        } else {
            $(document).scannerDetection(false);
        }

    },
    changeTypeInvoice: function (sel) {
        $.ajax({
            type: 'POST',
            //async: false,
            url: '/Selling/SaleRetail/ConvertInvoice',
            data: {
                IdOrder: $(sel).parent().data("id"),
            },

            success: function (res) {
                if (res.isValid) {

                }
            },
            error: function (err) {
                console.log(err)
            }
        });
    },//chuyển đổi hóa đơn sang đặt hàng và ngược lại
    clearDataorder: function () {
        $(".action-inv li.active").data("id", 0);
        let html = `<div  role="tabpanel">
                             <div class="tab-pane " id="tab-order-0" role="tabpanel">
                                <div class="no-order">
                                    <img src="../images/shopping.png" />
                                    <b>Chưa có hàng hóa nào được chọn</b>
                                    <span>Vui lòng chọn hàng hóa</span>
                                </div>
                            </div>
                        </div>`;
        $('#container-tableOder').html(html);
        $(".fullamount").html(0);
        $(".cuspayment").html(0);
        $(".amoutchange").html(0);
        $(".discountamount").val(0);
        $(".cussendamount").val(0);
        if ($(".VATAmount").length > 0) {
            $(".VATAmount").val(0);
        }

        $(".box-data-customer .btn-cleardata").trigger("click");
        _classPosEvent.input_searchProduct.val("");
        // _classPosEvent.input_searchProduct.focus();
    }, //reset data
    loadeventchangeclickprice: function () {
        $("#item-mon li .price").unbind();
        $("#item-mon li .price").click(function () {
            let price = parseFloat($(this).data("price")) || 0;
            let giamoi = parseFloat($(this).data("pricenew")) || 0;
            let giamgia = parseFloat($(this).data("discountamount")) || 0;
            let discount = parseFloat($(this).data("discount")) || 0;
            //let a = $(this).data("typediscount");
            let typediscount = parseInt($(this).data("typediscount"));

            if (typediscount == 1) {
                giamgia = discount;
            }
            let html = `<div id="popupselectDiscount" class="popup-change-price">
                                                   <div class="overlazy"></div>
                                                   <div class="ele-discount ele-SaleRetail" id="ousSizeOutPopup">
                                                        <div class="item">
                                                            <span>Đơn giá bán: </span>
                                                            <div class="content">
                                                                <div class="text">
                                                                    <input class="data-value form-control number3 giaban" placeholder="Nhập số tiền" value=`+ price + ` />
                                                                </div>
                                                            </div>
                                                         </div>
                                                         <div class="item">
                                                            <span>Giảm giá: </span>
                                                            <div class="content">
                                                                <div class="text">
                                                                    <input class="data-value form-control number3 giamgia" value="`+ giamgia + `" data-value="` + giamgia + `" data-type="` + typediscount + `"/>
                                                                    <button type="button" data-value="0" class="btn btn-action  btn-amountselect">VND</button>
                                                                    <button type="button" data-value="1" class="btn btn-action btn-percent">%</button>
                                                                </div>
                                                            </div>
                                                         </div>
                                                         <div class="item">
                                                            <span>Giá bán mới: </span>
                                                            <div class="content">
                                                                <div class="text">
                                                                    <input class="data-value form-control number3 giamoi" placeholder="Nhập số tiền" value=`+ giamoi + ` />
                                                                </div>
                                                            </div>
                                                         </div>
                                                    </div>
                                                </div>`;
            $(this).after(html);
            if (typediscount == 0 || typediscount == -1) {
                $(".btn-amountselect").addClass("btn-primary");
            } else {
                $(".btn-percent").addClass("btn-primary");
            }
            evetnFormatTextnumber3();
            evetnFormatnumber3(false);
            eventBanle.loadeventChangePricenewAndDiscountItem();


            $(".popup-change-price").find("input.giamoi").select();//phải làm 2 lần vì lần 1 nó hủy do focus vafo mất format
            $(".popup-change-price").find("input.giamoi").select();
            eventBanle.loadEventSelectInputPopupPrice();
        });
        //$(".overlazy").click();
    },
    loadEventSelectInputPopupPrice: function () {
        $(".popup-change-price").find("input.giamoi").click(function () {
            $(this).select();
        });
        $(".popup-change-price").find("input.giaban").click(function () {
            $(this).select();
        });
        $(".popup-change-price").find("input.giamgia").click(function () {
            $(this).select();
        });
    },
    loadeventChangePricenewAndDiscountItem: function () {

        $("#ousSizeOutPopup .content button").on('click', function (e) {
            if (!$(this).hasClass("btn-primary")) {
                $(this).parent().find(".btn-primary").toggleClass("btn-primary");
                $(this).toggleClass("btn-primary");
                let getid = parseInt($(this).data("value")) || 0;
                let giamgiavalue = parseFloat($(".giamgia").data("value")) || 0;
                let giamgiatype = parseInt($(".giamgia").data("type"));

                let discountamount = 0;
                // 0 la vnd , 1 laf %
                if (getid == 0) {
                    let getvaluegiaban = parseFloat($(".giamgia").val().replaceAll(",", "")) || 0;
                    if (getvaluegiaban > 0) {
                        let discountamount = 0;
                        let getgiaban = parseFloat($(".giaban").val().replaceAll(",", "")) || 0;
                        if (giamgiatype == getid) {
                            discountamount = giamgiavalue;
                        } else {
                            discountamount = getgiaban * (getvaluegiaban / 100);
                        }
                        $(".giamgia").val(discountamount.format0VND(3, 3))
                        //$(".giamgia").data("value", discountamount);
                        loadcongthuc(3);
                    }
                } else if (getid == 1) {//1 là %

                    let getvaluegiaban = parseFloat($(".giamgia").val().replaceAll(",", "")) || 0;
                    if (getvaluegiaban > 0) {
                        let getgiaban = parseFloat($(".giaban").val().replaceAll(",", "")) || 0;
                        if (giamgiatype == getid) {
                            discountamount = giamgiavalue;
                        } else {
                            discountamount = parseFloat((parseFloat(getvaluegiaban / getgiaban) * 100).toFixed(2));
                        }
                        if (discountamount > 100) {
                            $(".giamgia").val(100);
                            loadcongthuc();
                            toastrcus.error("Giá trị % không hợp lệ");
                            return;
                        }
                        $(".giamgia").val(discountamount.format0VND(3, 3))
                        //$(".giamgia").data("value", discountamount);
                        loadcongthuc(3);
                    }
                } else {
                    toastrcus.error("Giá trị không hợp lệ");
                    return;
                }

            }
        });
        $(".giamgia").change(function () {//giảm giá
            let typegiamgia = parseInt($("#ousSizeOutPopup .content button.btn-primary").data("value"));
            let value = parseFloat($(this).val().replaceAll(",", ""));
            $(this).data("value", value);
            $(this).data("type", parseInt(typegiamgia));
            loadcongthuc();
        });
        $(".giaban").change(function () {//giá bán
            let gianban = parseFloat($(".giaban").val());
            $(".giamoi").val(parseFloat($(".giaban").val().replaceAll(",", "")).format0VND(3, 3));
            $(".giamgia").val("");
            $(".giamgia").data("value", 0);
            //-------------
            $(".giamoi").parents(".ele-price").find(".price").data('price', gianban);
            loadcongthuc();
        });
        $(".giamoi").keyup(function () {
            let giaban = parseFloat($(".giaban").val().replaceAll(",", "")) || 0;
            let giamoi = parseFloat($(this).val().replaceAll(",", "")) || 0;
            let giasautru = giamoi - giaban;//tức là lấy giảm giá
            if (giasautru > 0) {
                $(".giamgia").val(0);
                $(".giamgia").data("value", 0);
                $(".giamoi").parents(".ele-price").find(".price").data('discount', 0);//gán giá trị % giảm giá cho đơn giá tại dòng sản phẩm
                $(".giamoi").parents(".ele-price").find(".price").data('discountamount', 0);//gán giá trị giảm giá cho đơn giá tại dòng sản phẩm
                $(".giamoi").parents(".ele-price").find(".price").data('typediscount', -1);//gán loại giảm giá cho đơn giá tại dòng sản phẩm 
            } else {
                giasautru = giasautru * -1;

                let typegiamgia = parseInt($("#ousSizeOutPopup .content button.btn-primary").data("value"));
                if (typegiamgia == 0) {//vnd
                    $(".giamgia").val(giasautru.format0VND(3, 3));
                    $(".giamgia").data("value", giasautru);
                    $(".giamoi").parents(".ele-price").find(".price").data('discount', 0);//gán giá trị giảm giá cho đơn giá tại dòng sản phẩm
                }
                else {
                    let discount = parseFloat(((giasautru / giaban) * 100).toFixed(2));
                    //vì khách nhập giá mới nên giảm phải lấy gốc trừ giá mới
                    $(".giamgia").val(discount.format0VND(3, 3));
                    $(".giamgia").data("value", discount);
                    $(".giamoi").parents(".ele-price").find(".price").data('discount', discount);//gán giá trị giảm giá cho đơn giá tại dòng sản phẩm

                }
                $(".giamoi").parents(".ele-price").find(".price").data('discountamount', giasautru);//gán giá trị giảm giá cho đơn giá tại dòng sản phẩm
                $(".giamoi").parents(".ele-price").find(".price").data('typediscount', typegiamgia);//gán loại giảm giá cho đơn giá tại dòng sản phẩm 
            }

            $(".giamoi").parents(".ele-price").find(".price").val(giamoi.format0VND(3, 3));//thay đổi đơn giá gốc
            loadcongthuc(2)
        });
        $(".overlazy").on('click', function (e) {
            if ($(e.target).closest("#ousSizeOutPopup").length === 0) {
                $("#popupselectDiscount").remove();
                $(".overlazy").remove();
            }
        });

        function loadcongthuc(type = -1) {
            //0  giá bán: 1: giảm giá: 2 giá mới, 3: sự kiện chọn vnd or %
            let giaban = parseFloat($(".giaban").val().replaceAll(",", "")) || 0;
            let giamgia = parseFloat($(".giamgia").val().replaceAll(",", "")) || 0;
            let giamoi = parseFloat($(".giamoi").val().replaceAll(",", "")) || 0;
            let discountAmount = 0;
            let typegiamgia = parseInt($("#ousSizeOutPopup .content button.btn-primary").data("value"));
            let total = 0;
            if (typegiamgia == 0) {
                if (giamgia > giaban) {
                    $(".giamgia").val(giaban.format0VND(3, 3));
                    toastrcus.error("Giá trị giảm giá không được vượt quá giá gốc");
                    giamgia = giaban;
                }
                total = giaban - giamgia;
            } else if (typegiamgia == 1) {
                if (giamgia > 100) {
                    $(".giamgia").val(100);
                    giamgia = 100;
                    toastrcus.error("Giá trị % giảm giá không hợp lệ");
                }
                discountAmount = giaban * (parseFloat(giamgia / 100));
                total = giaban - discountAmount;

            } else {
                toastrcus.error("Giá trị không hợp lệ");
                return;
            }
            if (type == 2 || type == 3) {//khi sự kiện giá mơi dc gõ
                $(".giamoi").parents(".ele-price").find(".price").val(giamoi.format0VND(3, 3));//thay đổi đơn giá gốc
                $(".giamoi").parents(".ele-price").find(".price").data('pricenew', giamoi);//thay đổi đơn giá gốc
            } else {
                $(".giamoi").val(total.format0VND(3, 3));// load giá mới sau giảm giá trên popup
                $(".giamoi").parents(".ele-price").find(".price").val(total.format0VND(3, 3));//thay đổi đơn giá gốc
                $(".giamoi").parents(".ele-price").find(".price").data('pricenew', total);//thay đổi đơn giá gốc
            }

            $(".giamoi").parents(".ele-price").find(".discountAmountItemPro").remove();//Xóa khi có thay đổi
            if (giamgia > 0 || discountAmount > 0) {//khi có giảm giá mới thay đổi

                htmldiscount = "";
                if (typegiamgia == 0) {
                    htmldiscount = "<span class='discountAmountItemPro'>" + (giamgia * -1).format0VND(0, 3) + "</span>"
                    $(".giamoi").parents(".ele-price").find(".price").data('discount', 0);//gán giá trị giảm giá cho đơn giá tại dòng sản phẩm
                    $(".giamoi").parents(".ele-price").find(".price").data('discountamount', giamgia);//gán giá trị giảm giá cho đơn giá tại dòng sản phẩm
                } else if (typegiamgia == 1) {
                    htmldiscount = "<span class='discountAmountItemPro'>" + (giamgia * -1).format0VND(0, 3) + "%</span>"
                    $(".giamoi").parents(".ele-price").find(".price").data('discount', giamgia);//gán giá trị giảm giá cho đơn giá tại dòng sản phẩm
                    $(".giamoi").parents(".ele-price").find(".price").data('discountamount', discountAmount);//gán giá trị giảm giá cho đơn giá tại dòng sản phẩm
                }
                $(".giamoi").parents(".ele-price").find(".price").after(htmldiscount);//hiển thị text giảm giá
            } else {
                $(".giamoi").parents(".ele-price").find(".price").data('discount', 0);//gán giá trị giảm giá cho đơn giá tại dòng sản phẩm
                $(".giamoi").parents(".ele-price").find(".price").data('discountamount', 0);//gán giá trị giảm giá cho đơn giá tại dòng sản phẩm
            }


            //----------------

            $(".giamoi").parents(".ele-price").find(".price").data('typediscount', typegiamgia);//gán loại giảm giá cho đơn giá tại dòng sản phẩm 

            //------------
            updateTotalProduct();
        }

        function updateTotalProduct() {
            let quantity = parseFloat($(".giamoi").parents("li").find(".quantity").val().replaceAll(",", "")) || 0;
            let pricenew = parseFloat($(".giamoi").val().replaceAll(",", "")) || 0;



            $(".giamoi").parents("li").find(".total").children("b").html((quantity * pricenew).format0VND(3, 3));
            $(".giamoi").parents("li").find(".total").children("b").data('total', (quantity * pricenew));


            eventBanle.loadChangeItemOrderLoca();
        }
    },

    loadeventclickdiscount: function () {
        $(".discountamount").click(function () {
            let html = `<div id="popupselectDiscount">
                                                   <div class="overlazy"></div>
                                                   <div class="ele-discount" id="ousSizeOutPopup">
                                                        <span> Giảm giá theo: </span>
                                                        <div class="content">
                                                            <div class="form-check-inline">
                                                              <label class="form-check-label mr-3">
                                                                <input type="radio" class="form-check-input" id="radio1" name="optradiocheckboxdis" value="2" checked> Số tiền (VND)
                                                              </label>
                                                              </div>
                                                              <div class="form-check-inline">
                                                              <label class="form-check-label">
                                                                <input type="radio" class="form-check-input" id="radio2" name="optradiocheckboxdis" value="1"> Phần trăm (%)
                                                              </label>
                                                            </div>
                                                            <div class="text">
                                                                <input class="data-value form-control number3" id="discounttypevalue" placeholder="Nhập số tiền" />
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>`;
            $(".discountamount").after(html);
            evetnFormatnumber3(false);
            eventBanle.showPopupSelectDiscount();
        });
        $(".overlazy").click();
    },
    showPopupSelectDiscount: function () {

        var typeSelectDiscount = TypeSelectDiscount.Cash;
        let Cashkeup = 0;
        let Percentkeup = 0;
        let getdiscount = parseInt($(".discountamount").data("discount")) || 0;
        if (typeof $(".discountamount").data("type") == "undefined") {
            if (getdiscount > 0) {
                $(".discountamount").data("type", TypeSelectDiscount.Percent);
                $(".discountamount").data("value", getdiscount);
            } else {
                $(".discountamount").data("type", TypeSelectDiscount.Cash);
                $(".discountamount").data("value", $(".discountamount").val().replaceAll(",", ""));
            }
        }
        if (typeof $(".discountamount").data("type") != "undefined") {
            typeSelectDiscount = parseInt($(".discountamount").data("type")) || 0;
            value = parseInt($(".discountamount").data("value")) || 0;
            $("input:radio[value=" + typeSelectDiscount + "][name='optradiocheckboxdis']").prop('checked', true);
            $("#discounttypevalue").val(value);
            if (typeSelectDiscount == TypeSelectDiscount.Cash) {
                Cashkeup = value;
            }
            else if (typeSelectDiscount == TypeSelectDiscount.Percent) {
                Percentkeup = value;
            }
        }


        $('input:radio[name=optradiocheckboxdis]').change(function () {
            let gettype = $("input[name='optradiocheckboxdis']:checked").val();
            if (gettype == TypeSelectDiscount.Cash) {
                typeSelectDiscount = TypeSelectDiscount.Cash;
                $("#discounttypevalue").val(Cashkeup.format0VND(3, 3));
            }
            else if (gettype == TypeSelectDiscount.Percent) {
                typeSelectDiscount = TypeSelectDiscount.Percent;
                $("#discounttypevalue").val(Percentkeup.format0VND(3, 3));
            }
            loadCongthuc();
            $("#discounttypevalue").select();
            setTimeout(() => {
                $("#discounttypevalue").select();
            }, 150);
        });
        $("#discounttypevalue").keyup(function () {
            if (typeSelectDiscount == 0) {
                typeSelectDiscount = parseInt($(".discountamount").data("discount"));
            }
            loadCongthuc();

        });
        function loadCongthuc() {
            if (typeSelectDiscount == TypeSelectDiscount.Cash) {
                let discountamount = parseFloat($("#discounttypevalue").val().replaceAll(",", "")) || 0;
                Cashkeup = discountamount;//lưu để update lại số cũ
                $(".discountamount").val(discountamount.format0VND(3, 3));
                //-----gán giá trị tiền 
                $(".discountamount").data("value", discountamount);
                $(".discountamount").data("discount", 0);
            }
            else if (typeSelectDiscount == TypeSelectDiscount.Percent) {
                let totalPayment = parseFloat($(".fullamount").text().replaceAll(",", ""));
                let discountamount = parseFloat($("#discounttypevalue").val().replaceAll(",", ""));
                let _discountamount = Math.round(parseFloat(totalPayment * (discountamount / 100)));
                Percentkeup = discountamount;//lưu để update lại số cũ
                $(".discountamount").val(_discountamount.format0VND(3, 3));
                //-----gán giá trị tiền và %
                $(".discountamount").data("value", discountamount);
                $(".discountamount").data("discount", discountamount);
            }
            $(".discountamount").data("type", typeSelectDiscount);
            eventBanle.eventLoadCongThucTien();
            eventBanle.saveUpdateAmountLoca(typeSelectDiscount);
        }

        $(".overlazy").on('click', function (e) {
            if ($(e.target).closest("#ousSizeOutPopup").length === 0) {
                $("#popupselectDiscount").remove();

            }
        });

        setTimeout(() => {
            $("#discounttypevalue").select();
        }, 200);
    },
    eventLoadCongThucTien: function () {

        let totalPayment = $(".fullamount").text().replaceAll(",", "") || 0;
        let discountPayment = $(".discountamount").val().replaceAll(",", "") || 0;
        let totalsaudiscount = (parseFloat(totalPayment) - parseFloat(discountPayment)) || 0;
        let vatamount = 0;
        if ($("#Vatrate").length > 0) {
            vatamount = Math.round(totalsaudiscount * (parseFloat($("#Vatrate").val() || 0) / 100));
            $(".VATAmount").val(vatamount.format0VND(0, 3, ""));
        } else {
        }
        let khachtra = totalsaudiscount + vatamount;
        $(".cuspayment").html(khachtra.format0VND(0, 3, ""));
        let cuspay = $(".cussendamount").val().replaceAll(",", "") || 0;
        if ($(".cussendamount").data("select") == 1) {
            cuspay = parseFloat(cuspay.replaceAll(",", ""));
            $(".cussendamount").html((cuspay - khachtra).format0VND(0, 3, ""));
        } else {
            $(".cussendamount").val(khachtra.format0VND(0, 3, ""));
            $(".cuspay").val(khachtra.format0VND(0, 3, ""));
            $(".amoutchange").html(0);
        }

    },
    saveUpdateAmountLoca: function (typeSelectDiscount) {

        let discounttypevalue = 0;
        if (typeSelectDiscount != -1) {
            discounttypevalue = parseFloat($("#discounttypevalue").val().replaceAll(",", "")) || 0
        }
        let discountPayment = parseFloat($(".discountamount").val().replaceAll(",", "")) || 0;
        let getidhoadon = $("ul.action-inv li.active").data("id");
        let getloca = localStorage.getItem("ProductsArrays");
        if (typeof getloca != "undefined" && getloca != null) {
            let datas = JSON.parse(getloca);
            if (datas.length > 0) {
                let foundIndex = datas.findIndex(x => x.Id == getidhoadon);
                if (foundIndex != -1) {
                    let VATAmount = 0;
                    let VATRate = 0;
                    if ($("#Vatrate").length > NOVATRate) {
                        VATAmount = parseFloat($(".VATAmount").val().replaceAll(",", ""));
                        VATRate = parseFloat($("#Vatrate").val());
                    }
                    let getdata = datas[foundIndex];
                    getdata.VATAmount = VATAmount;
                    getdata.VATRate = VATRate;
                    getdata.DiscountAmount = discountPayment;
                    getdata.Amount = getdata.Total - discountPayment + VATAmount;
                    if (typeSelectDiscount != -1) {//sự kiện thay đổi chiết khấu
                        if (typeSelectDiscount == TypeSelectDiscount.Percent) {//nếu khách km %
                            getdata.Discount = discounttypevalue;
                        } else {
                            getdata.Discount = 0;
                        }
                    }
                    datas[foundIndex] = getdata;
                    localStorage.setItem("ProductsArrays", JSON.stringify(datas));
                } else {
                    toastrcus.error("Đơn hàng không tồn tại vui lòng tải lại trang!");
                }
            } else {
                toastrcus.error("Chưa có đơn được tạo!");
            }

        } else {
            toastrcus.error("Lỗi hệ thống, vui lòng tải lại trang");
        }
    },
    eventPaymentInvoice: async  function (model) {
        var Datainvoice = {};
        $.ajax({
            type: 'POST',
            global: false,
            async: false,
            url: '/Selling/OrderTable/PaymentSaleRatailt',
            data: {
                jsonData: JSON.stringify(model)
            },
            success: function (res) {
                if (res.isValid) {
                    Datainvoice.Html = res.data;
                    Datainvoice.IsSuccess = true;

                    //eventBanle.clearDataorder();
                    //htmlPrint = res.data;
                    //if (htmlPrint != "") {
                    //    printDiv(htmlPrint);
                    //}

                } else {
                    Datainvoice.IsSuccess = false;
                }
                
            },
            error: function (err) {
                console.log(err)
            }
        });
        return Datainvoice;
    },
    paymentInoffline: function () {
        $(".btn-payment").click( function () {
            let getidhoadon = $(".action-inv li.active").data("id");
            if (getidhoadon == "0") {
                toastrcus.error("Chưa có đơn hàng nào để thanh toán!");
                return false;
            }
            let getloca = localStorage.getItem("ProductsArrays");
            if (typeof getloca != "undefined" && getloca != null) {
                let datas = JSON.parse(getloca);
                if (datas.length > 0) {
                    let cuspayAmount = parseFloat($(".cussendamount").val().replaceAll(",", "")) || 0;//khách chọn thanh t
                    let amoutchange = parseFloat($(".amoutchange").html().replaceAll(",", "")) || 0;//tiền thừa
                    let foundIndex = datas.findIndex(x => x.Id == getidhoadon);
                    if (foundIndex != -1) {
                        let getdata = datas[foundIndex];
                        getdata.CusSendAmount = cuspayAmount;//tiền khashc đưa
                        getdata.AmountChangeCus = amoutchange;//tiền thừa
                        loadingStart();
                        if (getdata.VATMTT) {
                            $.ajax({
                                type: 'GET',
                                global: false,
                                async: false,
                                url: "/Selling/Invoice/SuppliersEInvoice?SaleRetail=1",
                                // data: fomrdata,
                                contentType: false,
                                processData: false,
                                success: function (res) {
                                    if (res.isValid) {
                                        footer = "<button class='btn btn-primary btn-continue mr-3'><i class='icon-cd icon-add_cart icon'></i>Hủy bỏ</button><button class='btn btn-save btn-success'><i class='icon-cd icon-doneAll icon'></i>Tiếp tục</button>";
                                        if (!res.nodata) {
                                            footer = "<button class='btn btn-primary btn-continue mr-3'><i class='icon-cd icon-add_cart icon'></i>Đã hiểu</button>";
                                        }
                                        Swal.fire({
                                            // icon: 'success',
                                            title: res.title,
                                            html: res.html,
                                            showClass: {
                                                popup: 'popup-formcreate'
                                            },

                                            footer: footer,
                                            allowOutsideClick: true,
                                            showConfirmButton: false,
                                            showCancelButton: false,
                                            cancelButtonText: '<i class="fa fa-window-close"></i> Đóng',
                                            didRender: () => {
                                                loadingStop();
                                                $(".btn-continue").click(function () {
                                                    Swal.close();
                                                });
                                                $('#ManagerPatternEInvoices').select2({
                                                    placeholder: {
                                                        id: '', // the value of the option
                                                        text: "Chọn mẫu số và ký hiệu hóa đơn"
                                                    },
                                                    allowClear: true,
                                                    language: {
                                                        noResults: function () {
                                                            return "Không tìm thấy dữ liệu";
                                                        }
                                                    },
                                                })
                                                $(".btn-save").click(async function () {
                                                    loadingStart();
                                                    let idpatern = $(".selectSupplerInoice").find("#ManagerPatternEInvoices").val();
                                                    if (idpatern == "") {
                                                        toastrcus.error("Vui lòng chọn mẫu số ký hiệu hóa đơn");
                                                        return;
                                                    }
                                                    getdata.IdPattern = parseInt(idpatern);
                                                    Swal.close();
                                                   await eventBanle.eventPaymentInvoice(getdata).then(function (data) {
                                                        loadingStop();
                                                        if (data.IsSuccess) {
                                                            eventBanle.clearDataorder();
                                                            Swal.close();
                                                            if (data.Html != "") {
                                                                printDiv(data.Html);
                                                            }
                                                            datas.splice(foundIndex, 1);//xóa đi đơn đó
                                                            localStorage.setItem("ProductsArrays", JSON.stringify(datas));
                                                        }
                                                    });
                                                });

                                            }
                                        });
                                    }
                                },
                                error: function (err) {
                                    console.log(err)
                                }
                            });
                        }
                        else {
                            eventBanle.eventPaymentInvoice(getdata).then(function (data) {
                                loadingStop();
                                if (data.IsSuccess) {
                                    eventBanle.clearDataorder();
                                    Swal.close();
                                    if (data.Html != "") {
                                        printDiv(data.Html);
                                    }
                                    datas.splice(foundIndex, 1);//xóa đi đơn đó
                                    localStorage.setItem("ProductsArrays", JSON.stringify(datas));
                                }
                            });
                        }
                        //---------xóa sau khi phát hành
                    } else {
                        toastrcus.error("Đơn hàng không tồn tại vui lòng tải lại trang!");
                    }
                }
                else {
                    toastrcus.error("Đơn hàng không tồn tại vui lòng tải lại trang!");
                }

            } else {
                toastrcus.error("Chưa có đơn hàng nào để thanh toán!");
                return false;
            }
        })
    },
    payment: function () {//thanh toán
        $(".btn-payment").click(function () {
            // bắt đầu thanh toán luận nhé
            let idorder = $(".action-inv li.active").data("id");
            if (idorder == "0") {
                toastrcus.error("Chưa có đơn hàng nào để thanh toán!");
                return false;
            }

            let discountPayment = parseFloat($(".discountamount").val().replaceAll(",", "")) || 0;
            let cuspayment = parseFloat($(".cuspayment").text().replaceAll(",", "")) || 0;
            let cuspayAmount = parseFloat($(".cussendamount").val().replaceAll(",", "")) || 0;
            let idpayment = $('input[name=idPaymentMethod]:checked', '.paymentMethod').data("id") || 0;
            var htmlPrint = "";
            var isValidVAT = false;
            //check có xuất hddt k
            //if ($('input.isCheckVAT').is(':checked')) {
            //    isValidVAT = true;
            //}
            if (localStorage.getItem("VATMTT") == "true") {
                isValidVAT = true;
            }
            //end
            if (isValidVAT) {
                let VATAmount = parseFloat($(".VATAmount").val().replaceAll(",", "")) || 0;
                let Vatrate = parseFloat($("#Vatrate").val()) || 0;
                $.ajax({
                    type: 'GET',
                    //global: false,
                    url: "/Selling/Invoice/SuppliersEInvoice?SaleRetail=1",
                    // data: fomrdata,
                    contentType: false,
                    processData: false,
                    success: function (res) {
                        if (res.isValid) {
                            footer = "<button class='btn btn-primary btn-continue mr-3'><i class='icon-cd icon-add_cart icon'></i>Hủy bỏ</button><button class='btn btn-save btn-success'><i class='icon-cd icon-doneAll icon'></i>Lưu</button>";
                            if (!res.nodata) {
                                footer = "<button class='btn btn-primary btn-continue mr-3'><i class='icon-cd icon-add_cart icon'></i>Đã hiểu</button>";
                            }
                            Swal.fire({
                                // icon: 'success',
                                title: res.title,
                                html: res.html,
                                showClass: {
                                    popup: 'popup-formcreate'
                                },

                                footer: footer,
                                allowOutsideClick: true,
                                showConfirmButton: false,
                                showCancelButton: false,
                                cancelButtonText: '<i class="fa fa-window-close"></i> Đóng',
                                didRender: () => {
                                    $(".btn-continue").click(function () {
                                        Swal.close();
                                    });
                                    $('#ManagerPatternEInvoices').select2({
                                        placeholder: {
                                            id: '', // the value of the option
                                            text: "Chọn mẫu số và ký hiệu hóa đơn"
                                        },
                                        allowClear: true,
                                        language: {
                                            noResults: function () {
                                                return "Không tìm thấy dữ liệu";
                                            }
                                        },
                                    })

                                    $(".btn-save").click(function () {
                                        let idpatern = $(".selectSupplerInoice").find("#ManagerPatternEInvoices").val();
                                        if (idpatern == "") {
                                            toastrcus.error("Vui lòng chọn mẫu số ký hiệu hóa đơn");
                                            return;
                                        }
                                        $.ajax({
                                            type: 'POST',
                                            //async: false,
                                            url: '/Selling/OrderTable/CheckOutOrder',
                                            data: {
                                                TypeUpdate: _TypeUpdatePos.CheckOutOrder,
                                                IdOrder: idorder,
                                                discountPayment: discountPayment,
                                                cuspayAmount: cuspayAmount,
                                                vat: true,
                                                Vatrate: Vatrate,
                                                Amount: cuspayment,
                                                VATAmount: VATAmount,
                                                Idpayment: idpayment,
                                                ManagerPatternEInvoices: idpatern
                                            },

                                            success: function (res) {

                                                if (res.isValid) {

                                                    eventBanle.clearDataorder();
                                                    Swal.close();
                                                    htmlPrint = res.data;
                                                    if (htmlPrint != "") {
                                                        printDiv(htmlPrint);

                                                        //dataObject = {};
                                                        //dataObject.type = TypeEventWebSocket.PrintInvoice;
                                                        //dataObject.html = res.html;
                                                        //loadingStart();
                                                        //sposvietplugin.sendConnectSocket(listport[0]).then(function (data) {
                                                        //    console.log(data);
                                                        //    sposvietplugin.connectSignatureWebSocket(listport[0], JSON.stringify(dataObject)).then(function (data) {
                                                        //        loadingStop();
                                                        //        if (data == "-1") {
                                                        //            toastrcus.error("Có lỗi xảy ra");
                                                        //        }
                                                        //    });
                                                        //});
                                                    }

                                                }
                                            },
                                            error: function (err) {
                                                console.log(err)
                                            }
                                        });
                                    });

                                }
                            });
                        }
                    },
                    error: function (err) {
                        console.log(err)
                    }
                });
            } else {
                $.ajax({
                    type: 'POST',
                    //async: false,
                    url: '/Selling/OrderTable/CheckOutOrder',
                    data: {
                        TypeUpdate: _TypeUpdatePos.CheckOutOrder,
                        IdOrder: idorder,
                        discountPayment: discountPayment,
                        cuspayAmount: cuspayAmount,
                        Idpayment: idpayment,
                    },

                    success: function (res) {

                        if (res.isValid) {

                            eventBanle.clearDataorder();
                            Swal.close();
                            htmlPrint = res.data;
                            if (htmlPrint != "") {
                                printDiv(htmlPrint);
                            }

                        }
                    },
                    error: function (err) {
                        console.log(err)
                    }
                });
            }

            //if (isValid) {
            //    let sel = $("#lst-roomandtable li.active");
            //    await loadeventPos.loadOrderByTable(sel.data("id"));
            //    let text = $(sel).find("b").text();
            //    $(".btn-showttable").attr("data-id", sel.data("id"));// hiển thị cái bàn hiện tại
            //    $(".btn-showttable").find(".showTableOrder").html(text); // hiển thị cái bàn hiện tại
            //    loadeventPos.checkHighlightTableInOrder();// check highlight


            //}
        })

    },
    loadAddTabOrderInvoice: function () { // sự kiện  tạo và xóa đơn của bàn 
        $('#sellting-banle .btn-addInvoice').click(function (e) {
            // e.preventDefault();
            let isCreate = true;
            $(".action-inv").find("li").not($(this)).map(function (el, index) {
                let getTitle = $(this).data("id");
                if (getTitle == "0") {
                    isCreate = false;
                }
            });
            if (!isCreate) {
                return false;
            }
            if ($(".action-inv").find("li").not($(this)).length > 4) {
                toastr.warning("Mỗi bàn tối đa 4 đơn chưa thanh toán, vui lòng thanh toán bớt để order thêm!");
                return false;
            }
            let _class = makeid(5);
            var nextTab = $('.action-inv li.item-order').length + 1;
            $(".action-inv").find("li.active").removeClass("active");
            if ($('.action-inv li.item-order').length == 0) {
                $(this).parents(".action-inv").find("li").before('<li data-id="0" class="active item-order ' + _class + '">' +
                    '<i class="fa fa-exchange"></i>' +
                    '<span>Hóa đơn 1</span>' +
                    ' <i class="fas fa-times"></i>' +
                    '</li>');
            } else {

                $(this).parents(".action-inv").find("li:nth-child(" + (nextTab - 1) + ")").after('<li data-id="0" class="active item-order ' + _class + '">' +
                    '<i class="fa fa-exchange"></i>' +
                    '<span>Hóa đơn ' + nextTab + '</span>' +
                    ' <i class="fas fa-times"></i>' +
                    '</li>');
            }

            let html = `<div  role="tabpanel">
                             <div class="tab-pane " id="tab-order-0" role="tabpanel">
                                <div class="no-order">
                                    <img src="../images/shopping.png" />
                                    <b>Chưa có hàng hóa nào được chọn</b>
                                    <span>Vui lòng chọn hàng hóa</span>
                                </div>
                            </div>
                        </div>`;
            $('#container-tableOder').html(html);
            eventBanle.loadEventClickTabInvoiceShowData(_class);
            eventBanle.loadEventRemoveTaborder(_class, true);
            eventBanle.loadEventResetCusomter();
            eventBanle.loadAmoutAndQuantity(0, 0);
            $(".discountamount").data("discount", 0)
            $(".discountamount").data("value", 0);
            $(".discountamount").data("type", TypeSelectDiscount.Cash);
            //eventBanle.loadEventRemoveTaborder(_class, true);
        });

    },// sự kiện  tạo và xóa đơn của bàn 
    loadEventClickTabInvoiceShowData: function (sel) {
        $(".action-inv").find("li." + sel).click(async function () {

            if (!$(this).hasClass("active")) {// chỉ cho kích 1 lần trên 1 table
                $(".action-inv").find("li.active").not($(this)).removeClass("active");
                $(this).addClass("active");
                let id = $(this).data("id");
                await eventBanle.loadDataInvoiceOffline(id);// phải load dữ liệu trước
                localStorage.setItem("activeInvoicetab", $(this).index());
            }
        });
    },
    loadEventRemoveTaborder: function (sel, child = true) {
        if (child) {
            $(".action-inv").find("li." + sel).find("i:last-child").click(async function () {
                let idOrder = $(this).parent('li.item-order').data("id");
                if (idOrder == "0") {
                    $(this).parent('li.item-order').remove();
                    let html = `<div role="tabpanel">
                             <div class="tab-pane " id="tab-order-0" role="tabpanel">
                                <div class="no-order">
                                    <img src="../images/shopping.png" />
                                    <b>Chưa có hàng hóa nào được chọn</b>
                                    <span>Vui lòng chọn hàng hóa</span>
                                </div>
                            </div>
                        </div>`;
                    $('#container-tableOder').html(html);
                    if ($('.action-inv').find(".item-order").length == 0) { //not($(this).parents(".nav-item"))
                        $('.action-inv').find(".btn-addInvoice").trigger('click');
                    } else {
                        $('.action-inv li:first').trigger('click');

                    }
                } else {
                    //eventBanle.removeOder($(this), idOrder);
                    eventBanle.removeOderOffline($(this), idOrder);
                }
            });
        } else {
            $(sel).find("li").not(".add-tab").find("i:last-child").click(async function () {
                let idOrder = $(this).parent('li.item-order').data("id");
                if (idOrder == "0") {
                    $(this).parent('li.item-order').remove();
                    let html = `<div role="tabpanel">
                             <div class="tab-pane " id="tab-order-0" role="tabpanel">
                                <div class="no-order">
                                    <img src="../images/shopping.png" />
                                    <b>Chưa có hàng hóa nào được chọn</b>
                                    <span>Vui lòng chọn hàng hóa</span>
                                </div>
                            </div>
                        </div>`;
                    $('#container-tableOder').html(html);
                    if ($('.action-inv').find(".item-order").length == 0) { //not($(this).parents(".nav-item"))
                        $('.action-inv').find(".btn-addInvoice").trigger('click');
                    } else {
                        $('.action-inv li:first').trigger('click');

                    }
                } else {
                    //eventBanle.removeOder($(this), idOrder);
                    eventBanle.removeOderOffline($(this), idOrder);
                }
            });
        }
    },
    removeOder: function (sel, idOrder) {

        let iddt = idOrder;
        Swal.fire({
            icon: 'warning',
            title: 'Bạn có chắc chắn muốn xóa đơn không?',
            // showDenyButton: true,
            showCancelButton: true,
            confirmButtonText: 'Đồng ý',
            cancelButtonText: 'Đóng',
            // denyButtonText: `Don't save`,
        }).then((result) => {
            /* Read more about isConfirmed, isDenied below */
            if (result.isConfirmed) {
                $.ajax({
                    type: 'POST',
                    //global: false,
                    url: '/Selling/OrderTable/RemoveOderInvoice',
                    data: {
                        TypeUpdate: _TypeUpdatePos.RemoveOrder,
                        IdOrder: iddt
                    },

                    success: function (res) {
                        if (res.isValid) {
                            $(sel).parent().remove();
                            if ($(".action-inv li:not(.add-tab)").length == 0) {
                                $(".action-inv li.add-tab .btn-addInvoice").trigger("click");
                            } else {
                                $(".action-inv li:first").trigger("click");
                            }
                        }
                    },
                    error: function (err) {
                        console.log(err)
                    }
                });
            }
        });
    },// sự kiện xóa order
    loadEventCheckEnableBarcode: function () {
        if (localStorage.getItem('Barcode') == 1) {
            $(".btn-barcode").addClass("active")
            //$("#parentautocomplete").html("")
            toastrcus.success("Bật chế độ quét mã QRcode/Barcode");
            eventBanle.enableOrDisnableSancanbarCode(true);
            //$(".search-product").unbind("focus");
            // $(_classPosEvent.input_searchProduct).autocomplete("disable");
            //_classPosEvent.input_searchProduct.focus();
        }
    },//kiểm tra bật chế độ quét mã
    loadeventClickBarcode: function () {
        $(".btn-barcode").click(function () {
            if ($(this).hasClass("active")) {
                localStorage.setItem('Barcode', 0)
                $(this).removeClass("active")
                toastrcus.warning("Hủy chế độ quét mã QRcode/Barcode");
                eventBanle.enableOrDisnableSancanbarCode(false);
            } else {
                localStorage.setItem('Barcode', 1)
                $(this).addClass("active")
                toastrcus.success("Bật chế độ quét mã QRcode/Barcode");
                eventBanle.enableOrDisnableSancanbarCode(true);
            }
        });
    },
    loadRemovetab: function () {
        if ($(".action-inv li:not(.add-tab):not(.active)").length == 0) {
            $(".action-inv li.active").data("id", 0);
            eventBanle.loadDataInvoice(0);
        } else {
            $(".action-inv li.active").remove();
            $(".action-inv li:first-child").trigger("click");
        }
    },
    eventresetpaymentinput: function () {
        $(".fullamount").html(0);
        $(".discountamount").val(0);
        $(".VATAmount").val(0);
        $(".cuspayment").html(0);
        $(".amoutchange").html(0);
        $(".cussendamount").val(0);
    },

    loadChangeItemOrderLoca: function (idorder) {
        ProductsArrays = [];
        ProductsArray = {};
        Products = [];
        var totals = 0;
        var totalsaudiscount = 0;
        var Vatrate = NOVATRate;
        var vatamount = 0;
        var amounts = 0;
        var amountchange = 0;//tiền trả khách
        if ($("ul#item-mon").find("li").length == 0) {
            html = `<div role="tabpanel">
                             <div class="tab-pane " id="tab-order-0" role="tabpanel">
                                <div class="no-order">
                                    <img src="/images/shopping.png">
                                    <b>Chưa có hàng hóa nào được chọn</b>
                                    <span>Vui lòng chọn hàng hóa</span>
                                </div>
                            </div>
                        </div>`;
            $("#container-tableOder").html(html);
            eventBanle.eventresetpaymentinput();
            let getloca = localStorage.getItem("ProductsArrays");
            if (typeof getloca != "undefined" && getloca != null) {
                let datas = JSON.parse(getloca);
                if (datas.length > 0) {
                    // let getidhoadon = $("ul.action-inv li.active").data("id");
                    let getidhoadon = idorder;// test lại tức là xóa đơn thì xóa loca đi nhé, 
                    var foundIndex = datas.findIndex(x => x.Id == getidhoadon);
                    datas.splice(foundIndex, 1);
                    localStorage.setItem("ProductsArrays", JSON.stringify(datas));
                } else {
                    localStorage.setItem("ProductsArrays", "[]");
                }

            } else {
                localStorage.setItem("ProductsArrays", "[]");
            }
        } else {
            let getidhoadon = $("ul.action-inv li.active").data("id");
            $("ul#item-mon").find("li").map(function (index, ele) {
                product = {};
                let id = $(this).data("idpro");
                let code = $(this).data("code");
                let name = $(this).find(".name").data("name");
                let quantity = parseFloat($(this).find(".quantity").val()) || 0;
                let price = parseFloat($(this).find(".price").data("price")) || 0;
                let pricenew = parseFloat($(this).find(".price").data("pricenew")) || 0;
                let retailPrice = parseFloat($(this).find(".price").data("retailPrice")) || 0;
                let typediscount = parseFloat($(this).find(".price").data("typediscount"));
                let discountamount = parseFloat($(this).find(".price").data("discountamount")) || 0;
                let discount = parseFloat($(this).find(".price").data("discount")) || 0;
                let amount = parseFloat($(this).find(".total").find("b").data("total")) || 0;
                product.Id = id;
                product.Code = code;
                product.Name = name;
                product.Price = price;
                product.PriceNew = pricenew;
                product.RetailPrice = retailPrice;
                product.Quantity = quantity;
                product.Typediscount = typediscount;
                product.Discount = discount;
                product.DiscountAmount = discountamount;
                product.Amount = amount;
                Products.push(product);
                totals += amount;
            });
            //--------------tính tiền------------
            $(".fullamount").html(totals.format0VND(3, 3));
            let discountamount = parseFloat($(".discountamount").val().replaceAll(",", "")) || 0;
            let discount = parseFloat($(".discountamount").data("discount")) || 0;
            //------------
            if (discountamount > 0) {
                totalsaudiscount = totals - discountamount;
            } else {
                $(".discountamount").val(0);
                totalsaudiscount = totals;
            }
            if ($("#Vatrate").length > 0) {
                Vatrate = parseFloat($("#Vatrate").val());
                vatamount = totalsaudiscount * parseFloat(Vatrate / 100).toFixed(2);
                $(".VATAmount").val(vatamount.format0VND(3, 3));
            } 
            amounts = vatamount + totalsaudiscount;
            $(".cuspayment").html(amounts.format0VND(3, 3));
            //let cussendamount = parseFloat($("ul.action-inv li.active").data("cussendamount")) || 0;
            //if (cussendamount > 0) {
            //    let kkhacthanhtoan = parseFloat($(".cussendamount").val().replaceAll(",", "")) || 0;
            //    amountchange = amounts - kkhacthanhtoan;
            //    $(".amoutchange").val(amountchange.format0VND(0, 3, ""));
            //} else {
            //    $(".cussendamount").val(amounts.format0VND(0, 3, ""));
            //    $(".amoutchange").val(0);
            //}
            $(".cussendamount").val(amounts.format0VND(0, 3, ""));
            $(".amoutchange").val(0);
            //--------------add khách hàng----------//
            let cusocde = $(".search-customer").val();
            let IdCustomer = "";
            if (cusocde.trim() != "") {
                cusocde = cusocde.split(" ")[0];
                name = cusocde.split(" ")[1];
                IdCustomer = $(".search-customer").data("id");
                Customer = {};
                Customer.Id = IdCustomer;
                Customer.Name = name;
                Customer.CusCode = cusocde;

            } else {
                Customer = {};
            }
            //----------------lưu tiền mặt CK hay có xuất hóa đơn hay không
            if (localStorage.getItem("VATMTT") == "true") {
                ProductsArray.VATMTT = true;
            } else {
                ProductsArray.VATMTT = false;
            }
            let idpayment = $('input[name=idPaymentMethod]:checked', '.paymentMethod').data("id") || 0;
            ProductsArray.IdPaymentMethod = idpayment;
            //--------------------đóng gói----
            ProductsArray.Customer = Customer;//add khách ahfng
            ProductsArray.Items = Products;//sản phẩm

            ProductsArray.Total = totals;
            ProductsArray.DiscountAmount = discountamount;
            ProductsArray.Discount = discount;
            ProductsArray.VATRate = Vatrate;
            ProductsArray.VATAmount = vatamount;
            ProductsArray.Amount = amounts;
            ProductsArray.CusSendAmount = 0;//tiền khách thanh toán
            ProductsArray.Amoutchange = 0;//tiền thừa
            ProductsArray.IdPattern = null;//tiền thừa
            ProductsArray.Id = getidhoadon;
            ProductsArrays.push(ProductsArray);//add vào list lớn
            let getloca = localStorage.getItem("ProductsArrays");
            if (typeof getloca != "undefined" && getloca != null) {
                let datas = JSON.parse(getloca);
                if (datas.length > 0) {
                    var foundIndex = datas.findIndex(x => x.Id == getidhoadon);
                    if (foundIndex != -1) {
                        datas[foundIndex] = ProductsArray;//chỉ update lại index nên phải để ProductsArray chứ k phải ProductsArrays
                        localStorage.setItem("ProductsArrays", JSON.stringify(datas));
                    } else {//tức là tìm k có thì thêm mới hóa đơn
                        datas.push(ProductsArray);
                        localStorage.setItem("ProductsArrays", JSON.stringify(datas));
                    }

                } else {
                    localStorage.setItem("ProductsArrays", JSON.stringify(ProductsArrays));
                }

            } else {
                localStorage.setItem("ProductsArrays", JSON.stringify(ProductsArrays));
            }

        }
        eventBanle.loadSugetionPayment();
    },//thay đổi chiết khấu giảm giả của mỗi mặt hàng
    //orderTable: function (dataObject) {
    //    $.ajax({
    //        type: 'POST',
    //        url: '/Selling/OrderTable/AddOrderInvoice',
    //        dataType: 'json',
    //        data: dataObject,
    //        success: async function (res) {
    //            //console.log(res.data);

    //            if (res.isValid) {
    //                if (res.data.orderTableItems.length == 0) {

    //                    if (res.data.isBringBack) {
    //                        res.data.idRoomAndTableGuid = "-1";
    //                    }
    //                    await eventBanle.loadRemovetab(0);
    //                } else {
    //                    let html = `<ul id="item-mon"> `;
    //                    res.data.orderTableItems
    //                        .forEach(function (item, index) {
    //                            index = index + 1;
    //                            html += ` <li data-code="` + item.code + `" data-id="` + item.idGuid + `" data-idpro ="` + item.idProduct + `" data-slNotify=` + item.quantityNotifyKitchen + ` data-sl=` + item.quantity + ` >
    //                                        <div  class="btn-remove" data-idquan="`+ item.quantity + `"><i data-idquan="` + item.quantity + `" class="fas fa-trash-alt"></i></div>
    //                                        <div class="name" data-name="`+ item.name +`"><b>` + index + ". " + item.name + `</b></div>
    //                                        <div class="item_action"><i class="fas fa-minus"></i><input class="quantity numberformat" value="`+ item.quantity + `"><i class="fas fa-plus"></i></div>
    //                                        <div class="ele-price"><input type="text" class="form-control priceFormat price"  data-price="`+ item.price +`" data-discount="0" data-discountamount="0" readonly value="`+ (item.price) + `" /></div>
    //                                        <div class="total"><b class="priceFormat" data-total="0">`+ (item.total) + `</b></div>
    //                                    </li>`;
    //                        });
    //                    html += `</ul>`;
    //                    $("#container-tableOder").html(html);
    //                    $(".action-inv").find("li.active").data("id", res.data.idGuid);
    //                    eventBanle.eventUpdatedataItemMonOrder();

    //                    eventBanle.loadAmoutAndQuantity(res.data.amount, res.data.quantity);

    //                    eventBanle.loadEventClickIconAddAndMinus();
    //                    eventBanle.loadSugetionPayment();
    //                    eventBanle.loadeventchangeclickprice();
    //                    //eventBanle.loadTienthuakhach();// sự kiện tiền thừa cho khách.


    //                }
    //            }
    //        },
    //        error: function (err) {
    //            console.log(err)
    //        }
    //    });
    //},// sự kiện order
    eventUpdatedataItemMonOrder: function () {
        $("#container-tableOder ul#item-mon").children("li").map(function () {
            let idpro = $(this).data("idpro");
            let iditem = $(this).data("id");
            let sl = $(this).data("sl");
            let slnotify = $(this).data("slnotify");
            $(this).removeAttr("data-id");
            $(this).removeAttr("data-idpro");
            $(this).removeAttr("data-sl");
            $(this).removeAttr("data-slnotify");
            $(this).data("idpro", idpro)
            $(this).data("id", iditem)
            $(this).data("sl", sl)
            $(this).data("slnotify", slnotify)
        });

    },
    loadEventCheckCodeItem: async function (code) {

        let isValid = false;
        $("ul#item-mon").find("li").map(function () {
            let _code = $(this).data("code") || "";
            if (_code.trim() == code) {
                isValid = true;
                $(this).find(".item_action").find("i:last-child").trigger("click");
            }
        });
        return isValid;
    },
    eventselectProductCompelteOffline: async function (id, price, retailPrice, code, name) {
        var checkcode = await eventBanle.loadEventCheckCodeItem(code);
        if (!checkcode) {
            let html = `<ul id="item-mon"> `;
            let htmlli = "";
            htmlli += `<li data-code="` + code + `" data-idpro ="` + id + `" data-sl=` + 1 + `>
                            <div  class="btn-remove" data-idquan="1"><i data-idquan="1" class="fas fa-trash-alt"></i></div>
                            <div class="name" data-name="`+ name + `"><b>` + name + `</b></div>
                            <div class="item_action"><i class="fas fa-minus"></i><input data-quantity="1" class="quantity number3" value="1"><i class="fas fa-plus"></i></div>
                            <div class="ele-price"><input type="text" class="form-control number3 price"  data-price="`+ price + `" data-pricenew="` + price + `"  data-retailPrice="` + retailPrice + `" data-typediscount="-1" data-discountamount="0" data-discount="0" readonly value="` + (price) + `" /></div>
                            <div class="total"><b class="number3" data-total="`+ (price * 1) + `">` + (price * 1) + `</b></div>
                        </li>`;
            html += htmlli;
            html += `</ul>`;
            if ($("ul#item-mon").length > 0) {
                $("ul#item-mon li:last-child").after(htmlli);
            } else {

                $(".action-inv").find("li.active").removeAttr("data-id");
                $(".action-inv").find("li.active").data("id", GUID());// add id vào
                $("#container-tableOder").html(html);
            }
        }
        eventBanle.loadeventchangeclickprice();//kích vào giá
        eventBanle.loadChangeItemOrderLoca();
        evetnFormatTextnumber3();
        eventBanle.loadEventClickIconAddAndMinusoffline();
        if ($(".search-customer").val() != "") {
            eventBanle.updateCustomerOrderOffline(0);//update khách hàng
        }
    },
    loadEventClickIconAddAndMinusoffline: function () {
        $("#item-mon  li .item_action").find("input.quantity").unbind();
        $("#item-mon  li .item_action").find("input.quantity").change(function () {
            if ($(this).val() <= 0) {
                toastrcus.error("Số lượng không hợp lệ");
                $(this).val($(this).data("quantity"));
                return;
            }

            let quantity = parseFloat($(this).val().replaceAll(",", ""));
            let price = parseFloat($(this).parents("li").find(".price").val().replaceAll(",", ""));
            $(this).val(quantity.format0VND(3, 3));
            $(this).data("quantity", quantity);

            $(this).parents("li").find(".total").find("b").html((quantity * price).format0VND(3, 3));
            $(this).parents("li").find(".total").find("b").data("total", (quantity * price));

            eventBanle.loadChangeItemOrderLoca();

        });
        $("#item-mon  li .item_action").find("i").unbind();
        $("#item-mon  li .item_action").find("i:first-child").click(function () {

            let quantity = parseFloat($(this).parent().find(".quantity").val().replaceAll(",", ""));
            _quan = quantity - 1;
            if (_quan <= 0) {
                toastrcus.error("Số lượng không hợp lệ");
                $(this).val($(this).data("quantity"));
                return;
            }

            let price = parseFloat($(this).parents("li").find(".price").val().replaceAll(",", ""));
            $(this).parents("li").find(".quantity").val(_quan.format0VND(3, 3));
            $(this).parents("li").find(".quantity").data("quantity", _quan);

            $(this).parents("li").find(".total").find("b").html((_quan * price).format0VND(3, 3));
            $(this).parents("li").find(".total").find("b").data("total", (_quan * price));

            eventBanle.loadChangeItemOrderLoca();

        });// dùng để giảm món
        $("#item-mon  li .item_action").find("i:last-child").click(function () {

            let quantity = parseFloat($(this).parent().find(".quantity").val().replaceAll(",", ""));
            _quan = quantity + 1;
            if (_quan <= 0) {
                toastrcus.error("Số lượng không hợp lệ");
                $(this).val($(this).data("quantity"));
                return;
            }

            let price = parseFloat($(this).parents("li").find(".price").val().replaceAll(",", ""));
            $(this).parents("li").find(".quantity").val(_quan.format0VND(3, 3));
            $(this).parents("li").find(".quantity").data("quantity", _quan);

            $(this).parents("li").find(".total").find("b").html((_quan * price).format0VND(3, 3));
            $(this).parents("li").find(".total").find("b").data("total", (_quan * price));

            eventBanle.loadChangeItemOrderLoca();

        });
        $("#item-mon  li .btn-remove").unbind();
        $("#item-mon  li .btn-remove").click(function () {
            $(this).parent("li").remove();
            eventBanle.loadChangeItemOrderLoca();
        });
    },
    //loadEventClickIconAddAndMinus: function () {
    //    $("#item-mon  li .item_action").find("input").unbind();
    //    $("#item-mon  li .item_action").find("input").change(function () {
    //        if ($(this).val() <= 0) {
    //            toastrcus.error("Số lượng không hợp lệ");
    //            $(this).val(1);
    //        }
    //        let IdOrderItem = $(this).parent().parent("li").data("id");
    //        let idTable = "-1";
    //        let cusocde = $(".search-customer").val();
    //        let sluongmon = $(this).val();
    //        let IdCustomer = "";
    //        if (cusocde.trim() != "") {
    //            cusocde = cusocde.split(" ")[0];
    //            IdCustomer = $(".search-customer").data("id");
    //        }
    //        //bấm giảm thì phải hỏi
    //        if (sluongmon <= 0) {
    //            toastrcus.error("Sô lượng ít nhất của sản phẩm phải là 1");
    //        } else {
    //            var dataObject = {
    //                IdOrderItem: IdOrderItem,
    //                CusCode: cusocde,
    //                IdCustomer: IdCustomer,
    //                QuantityFloat: sluongmon,
    //                TypeUpdate: _TypeUpdatePos.ReplaceQuantity,
    //                IdRoomAndTableGuid: idTable != "-1" ? idTable : "",
    //                IsBringBack: idTable == "-1" ? true : false,
    //                IdGuid: $(".action-inv").find("li.active").data("id")
    //            };// giảm số lượng
    //            eventBanle.orderTable(dataObject);
    //        }

    //    });
    //    $("#item-mon  li .item_action").find("i").unbind();
    //    $("#item-mon  li .item_action").find("i:first-child").click(function () {
    //        let IdOrderItem = $(this).parent().parent("li").data("id");
    //        let idTable = "-1";
    //        let cusocde = $(".search-customer").val();
    //        let sluongmon = parseInt($(this).parent().parent().data("sl"));
    //        let IdCustomer = "";
    //        if (cusocde.trim() != "") {
    //            cusocde = cusocde.split(" ")[0];
    //            IdCustomer = $(".search-customer").data("id");
    //        }
    //        //bấm giảm thì phải hỏi
    //        if (sluongmon <= 1) {
    //            toastrcus.error("Sô lượng ít nhất của sản phẩm phải là 1");
    //        } else {
    //            var dataObject = {
    //                IdOrderItem: IdOrderItem,
    //                CusCode: cusocde,
    //                IdCustomer: IdCustomer,
    //                QuantityFloat: -1,
    //                TypeUpdate: _TypeUpdatePos.UpdateQuantity,
    //                IdRoomAndTableGuid: idTable != "-1" ? idTable : "",
    //                IsBringBack: idTable == "-1" ? true : false,
    //                IdGuid: $(".action-inv").find("li.active").data("id")
    //            };// giảm số lượng
    //            eventBanle.orderTable(dataObject);
    //        }


    //        //  loadeventPos.loadactiveClickItemMon($(this));

    //    });// dùng để giảm món
    //    $("#item-mon  li .item_action").find("i:last-child").click(function () {

    //        let IdOrderItem = $(this).parent().parent("li").data("id");
    //        let idTable = "-1";
    //        let cusocde = $(".search-customer").val();
    //        let IdCustomer = "";
    //        if (cusocde.trim() != "") {
    //            cusocde = cusocde.split(" ")[0];
    //            IdCustomer = $(".search-customer").data("id");
    //        }
    //        var dataObject = {
    //            IdOrderItem: IdOrderItem,
    //            CusCode: cusocde,
    //            IdCustomer: IdCustomer,
    //            QuantityFloat: 1,
    //            TypeUpdate: _TypeUpdatePos.UpdateQuantity,
    //            IdRoomAndTableGuid: idTable != "-1" ? idTable : "",
    //            IsBringBack: idTable == "-1" ? true : false,
    //            IdGuid: $(".action-inv").find("li.active").data("id")
    //        };// giảm số lượng
    //        eventBanle.orderTable(dataObject);

    //    });
    //    $("#item-mon  li .btn-remove").unbind();
    //    $("#item-mon  li .btn-remove").click(function () {
    //        let IdOrderItem = $(this).parent("li").data("id");
    //        let Quantity = $(this).data("idquan");
    //        let idTable = "-1";

    //        var dataObject = {
    //            QuantityFloat: Quantity,
    //            IdOrderItem: IdOrderItem,
    //            TypeUpdate: _TypeUpdatePos.RemoveRowItem,
    //            IdRoomAndTableGuid: idTable != "-1" ? idTable : "",
    //            IsBringBack: idTable == "-1" ? true : false,
    //            IdGuid: $(".action-inv").find("li.active").data("id")
    //        };// giảm số lượng
    //        eventBanle.orderTable(dataObject);

    //    });


    //},// sự kiện giảm đi số lượng món
    loadAmoutAndQuantity: function (amount = null, quantity = null) {

        if (amount != null && quantity != null) {
            $(".fullamount").html(parseFloat(amount).format0VND(3, 3, ""));
            if ($(".discountamount").val() == "") {
                $(".discountamount").val(0);
            }

            //$(".cuspayment").html(parseFloat(amount).format0VND(3, 3, ""));

            //let checkclicksend = $(".cussendamount").hasClass("active");
            //cuspay = parseInt($(".cussendamount").val() || 0);
            //if (!checkclicksend) {
            //    $(".cussendamount").val(parseFloat(amount));

            //} else {


            //}

        } else {
            $(".fullamount").html(0);
            $(".discountamount").val(0);
            //  $(".cuspayment").html(0);
            // $(".cussendamount").val(0);
        }
        eventBanle.eventLoadTFullAmount();
        priceFormat();
        numberFormat();

    },//load toongrquantity và tiền:
    eventAddProductBySancanBarcode: function (ProductCode) {
        let cusocde = $(".search-customer").val();
        if (cusocde.trim() != "") {
            cusocde = cusocde.split(" ")[0];
        }
        idProduct = "";
        var dataObject = {
            CusCode: cusocde,// chỉ ở đây mới cập nhật khách vì là mới order món đầu tiên
            QuantityFloat: 1,
            TypeUpdate: _TypeUpdatePos.AddProduct,
            IdRoomAndTableGuid: "-1",
            IsBringBack: true,
            IdGuid: $(".action-inv").find("li.active").data("id"),
            IdProduct: idProduct,
            ProductCode: ProductCode,
        };
        eventBanle.orderTable(dataObject);//lưu đơn
    },//add sản phẩm khi quét mã barcode
    eventselectProductCompelte: function (idProduct) {

        let ProductCode = $(".search-product").val();
        let cusocde = $(".search-customer").val();
        if (cusocde.trim() != "") {
            cusocde = cusocde.split(" ")[0];
        }
        if (ProductCode.trim() != "") {
            ProductCode = ProductCode.split(" ")[0];
        }
        var dataObject = {
            CusCode: cusocde,// chỉ ở đây mới cập nhật khách vì là mới order món đầu tiên
            QuantityFloat: 1,
            TypeUpdate: _TypeUpdatePos.AddProduct,
            IdRoomAndTableGuid: "-1",
            IsBringBack: true,
            IdGuid: $(".action-inv").find("li.active").data("id"),
            IdProduct: idProduct,
            ProductCode: ProductCode,
        };
        eventBanle.orderTable(dataObject);//lưu đơn
    },
    loadEventChangePaumentDiscount: function () {

        var amoutn = 0;

        //$(".discountamount").keyup(function () {
        //    $("ul.action-inv li.active").data("discountamount", parseFloat($(this).val().replaceAll(",", "")));
        //    eventBanle.eventLoadTFullAmount();
        //    eventBanle.loadSugetionPayment();

        //});

        if (amoutn == 0) {
            amoutn = parseInt($(".cuspayment").html().replaceAll(",", ""));
        }
        //
        $(".cussendamount").keyup(function () {
            //$(this).data("value", 1);
            datathis = parseFloat($(this).val().replaceAll(",", ""));
            amoutn = parseFloat($(".cuspayment").html().replaceAll(",", ""));

            $("ul.action-inv li.active").data("cussendamount", datathis);
            let cuspayamount = parseFloat($(".cussendamount").val().replaceAll(",", "")) || 0;
            let _vl = cuspayamount || 0;
            let tienthua = _vl - amoutn;

            $(".amoutchange").html(tienthua.format0VND(0, 3, ""));
            //if (_vl > amoutn) {
            //    $(".amoutchange").html(tienthua.format0(0, 3, ""));
            //} else {
            //    $(".amoutchange").html(0);
            //}
        });
    },
    loadEventChangeVATRateVATAmount: function () {
        $(".VATAmount").keyup(function () {
            let fullamount = parseFloat($(".fullamount").text().replaceAll(",", "")) || 0;
            let discountamount = parseFloat($(".discountamount").val().replaceAll(",", "")) || 0;
            let Total = fullamount - discountamount;
            let VATAmount = parseFloat($(this).val().replaceAll(",", "")) || 0;
            let Amount = Total + VATAmount;
            $(".cuspayment").html(Amount.format0VND(3, 3, ""));
            let cussendamount = parseFloat($("ul.action-inv li.active").data("cussendamount")) || 0;
            if (cussendamount != 0) {
                //if ($(".cussendamount").data("value") == 1) {
                let cuspayment = parseFloat(cussendamount) || 0;//số tieefn khách thanh toán
                $(".amoutchange").html((cuspayment - Amount).format0VND(3, 3, ""));
            } else {
                $(".cuspayment").html(Amount.format0VND(3, 3, ""));
                $(".cussendamount").val(Amount.format0VND(3, 3, ""));
                $(".amoutchange").html(0);
            }
            eventBanle.saveUpdateAmountLoca(-1);
            eventBanle.loadSugetionPayment();
        });
        $("#Vatrate").change(function () {
            eventBanle.eventLoadTFullAmount();
            eventBanle.saveUpdateAmountLoca(-1);
            eventBanle.loadSugetionPayment();

        });
    },
    loadTienthuakhach: function () {
        amoutn = parseFloat($(".cuspayment").html().replaceAll(",", ""));
        let cuspayamount = parseFloat($("ul.action-inv li.active").data("cussendamount")) || 0;
        //let cuspayamount = parseFloat($(".cussendamount").val().replaceAll(",", "")) || 0;
        let _vl = cuspayamount || 0;
        let tienthua = _vl - amoutn;
        $(".amoutchange").html(tienthua.format0VND(0, 3, ""));
    },
    loadEventChangeSelectAmoutpaycus: function () {
        // lựa chọn số tiền khách đưa
        $(".lst-amount-paycus span").click(function () {
            let _value = parseInt($(this).data("value"));
            $(".cussendamount").val(_value.format0VND(0, 3, ""));
            $(".cussendamount").addClass("active");
            $(".cussendamount").trigger("keyup");
            // $(".cussendamount").select();

        });
    },
    //sự keienj remove cả dòng bấm nút xóa
    loadSugetionPayment: function () {
        function placeValues(someNumber) {
            x = [...someNumber.toString()].reverse().reduce((p, c, i) => {
                p[10 ** i] = c;
                return p;
            }, {});
            return x; // {1000:1, 100:2, 10:3, 1:4}
        }
        let html = "";
        let _ntot = $(".cuspayment").text().replaceAll(",", "");
        let total = placeValues(Math.ceil(parseFloat(_ntot)));
        let v = 0;
        let z = Math.round(parseFloat(_ntot));
        _ntot = z.toString();
        if (_ntot.length === 7) {
            v = Math.ceil(z / 1000000);
            if (v == 2 || v == 3 || v == 4) {

                if ((v * 1000000) - 500000 >= z) {
                    html += '<span class="priceFormat" data-value="' + ((v * 1000000) - 500000) + '">' + ((v * 1000000) - 500000) + '</span>';
                }
                html += '<span class="priceFormat" data-value="' + (v * 1000000) + '">' + (v * 1000000) + '</span>';
                html += '<span class="priceFormat" data-value="' + 5000000 + '">' + 5000000 + '</span>';
            } else {

                if ((v * 1000000) - 500000 >= z) {
                    html += '<span class="priceFormat" data-value="' + ((v * 1000000) - 500000) + '">' + ((v * 1000000) - 500000) + '</span>';
                }
                html += '<span class="priceFormat" data-value="' + (v * 1000000) + '">' + (v * 1000000) + '</span>';
                if (v != 1) {

                    html += '<span class="priceFormat" data-value="' + 10000000 + '">' + 10000000 + '</span>';
                }
            }
        } else if (_ntot.length === 6) {
            v = Math.ceil(z / 100000);
            if (v == 2 || v == 3 || v == 4) {

                if ((v * 100000) - 50000 >= z) {
                    html += '<span class="priceFormat" data-value="' + ((v * 100000) - 50000) + '">' + ((v * 100000) - 50000) + '</span>';
                }
                html += '<span class="priceFormat" data-value="' + (v * 100000) + '">' + (v * 100000) + '</span>';
                html += '<span class="priceFormat" data-value="' + 500000 + '">' + 500000 + '</span>';
            } else {

                if ((v * 100000) - 50000 >= z) {
                    html += '<span class="priceFormat" data-value="' + ((v * 100000) - 50000) + '">' + ((v * 100000) - 50000) + '</span>';
                }
                html += '<span class="priceFormat" data-value="' + (v * 100000) + '">' + (v * 100000) + '</span>';
                html += '<span class="priceFormat" data-value="' + 1000000 + '">' + 1000000 + '</span>';
            }
        }
        else if (_ntot.length === 5) {
            v = Math.ceil(z / 10000);
            if (v == 2 || v == 3 || v == 4) {
                html += '<span class="priceFormat" data-value="' + (v * 1000000) + '">' + (v * 10000) + '</span>';
                if ((v * 10000) - 5000 >= z) {
                    html += '<span class="priceFormat" data-value="' + ((v * 10000) - 5000) + '">' + ((v * 10000) - 5000) + '</span>';
                }
                html += '<span class="priceFormat" data-value="' + 50000 + '">' + 50000 + '</span>';
                html += '<span class="priceFormat" data-value="' + 100000 + '">' + 100000 + '</span>';
                html += '<span class="priceFormat" data-value="' + 200000 + '">' + 200000 + '</span>';
                html += '<span class="priceFormat" data-value="' + 500000 + '">' + 500000 + '</span>';
            } else {
                html += '<span class="priceFormat" data-value="' + (v * 10000) + '">' + (v * 10000) + '</span>';
                if ((v * 10000) - 5000 >= z) {
                    html += '<span data-value="' + ((v * 10000) - 5000) + '">' + ((v * 10000) - 5000) + '</span>';
                }
                html += '<span class="priceFormat" data-value="' + 100000 + '">' + 100000 + '</span>';
                html += '<span class="priceFormat" data-value="' + 200000 + '">' + 200000 + '</span>';
                html += '<span class="priceFormat" data-value="' + 500000 + '">' + 500000 + '</span>';
            }
        }
        $(".lst-amount-paycus").html(html);
        eventBanle.loadEventChangeSelectAmoutpaycus();
        priceFormat();
    },
    loadAutocomplete: function (isProduct = true, isCustomer = true) { //sự kiện autocomple sản phẩm và khách hàng
        if (isProduct) {
            let idProductAutocom = 0;
            _classPosEvent.input_searchProduct.blur(function () {
                $("#parentautocomplete").css({
                    height: "0",
                    top: "0",
                    "z-index": "0"
                });
            });
            _classPosEvent.input_searchProduct.autocomplete(
                {
                    appendTo: "#parentautocomplete",
                    autoFocus: true,
                    minLength: 0,
                    delay: 0,

                    source: function (request, response) {
                        $.ajax({
                            global: false,
                            url: "/Selling/Pos/SearchProductPos",
                            type: "GET",
                            dataType: "json",
                            data: {
                                text: request.term,
                                iSsell: true
                            },
                            // html: true,
                            success: function (data) {
                                response($.map(data, function (item) {
                                    let texthigh = __highlight(item.name, request.term);
                                    let htmltonkho = "<span class='quantity'>Tồn kho: <b>" + parseFloat(item.quantity).format0VND(3, 3, '') + "</b></span>";
                                    if (data.isInventory || data.typeProductCategory == EnumTypeProductCategory.COMBO || data.typeProductCategory == EnumTypeProductCategory.SERVICE) {
                                        htmltonkho = "<span class='quantity'>Tồn kho: -- <span>";
                                    }
                                    let html =
                                        "<a href='javascript:void(0)'><div class='search-auto'>" +
                                        "<div class='img'><img src='../" + item.img + "'></div>" +
                                        "<div class='tk_name'><span>" + texthigh + " (" + item.code + ")</span><span class='price'> Giá: " + parseFloat(item.price).format0VND(3, 3, '') + "</span>" +
                                        htmltonkho + "</div></div></a>";
                                    return {
                                        //label: html, value: item.code + " " + item.name, idProduct: item.id
                                        label: html, value: item.code, idProduct: item.id, code: item.code, name: item.name, retailPrice: item.retailPrice, price: item.price
                                    };
                                }))
                                return { label: request.term, value: request.term };
                            },
                        })

                    },
                    html: true,
                    select: async function (e, ui) {

                        // console.log(ui);
                        $(this).val(ui.item.value);
                        $(this).data("price", parseFloat(ui.item.price));
                        $(this).data("retailPrice", parseFloat(ui.item.retailPrice));
                        $(this).data("code", ui.item.code);
                        $(this).data("name", ui.item.name);
                        $(this).select();
                        // _varIdGuidSelectproductAutocomplete = ui.item.idProduct;
                        idProductAutocom = ui.item.idProduct;

                        //  eventBanle.eventselectProductCompelte(idProductAutocom);//dùng cho bán hàng gọi online
                        await eventBanle.eventselectProductCompelteOffline(ui.item.idProduct,
                            parseFloat(ui.item.price),
                            parseFloat(ui.item.retailPrice),
                            ui.item.code,
                            ui.item.name);
                    },
                    response: function () {
                        // $(this).select()
                    }
                }).focus(function () {
                    $("#parentautocomplete").css({
                        height: "calc(100vh - 100px)",
                        top: "52px",
                        "z-index": "5"
                    });

                    //if (localStorage.getItem('Barcode') != 1) {
                    $(this).autocomplete("search");
                    $(this).select();
                    //} else {
                    //  $(this).select();
                    // }
                    //$(this).focus();
                    // $(this).select()
                });
            _classPosEvent.input_searchProduct.keypress(async function (event) {
                let _dt = $(this).val();
                if (event.keyCode == 13 && _dt.length > 0) {
                    //eventBanle.eventselectProductCompelte(idProductAutocom);
                    let price = $(this).data("price");
                    let retailPrice = $(this).data("retailPrice");
                    let code = $(this).data("code");
                    let name = $(this).data("name");
                    await eventBanle.eventselectProductCompelteOffline(idProductAutocom,
                        parseFloat(price),
                        parseFloat(retailPrice),
                        code,
                        name);
                    $(this).select();
                } else if (_dt.length == 0) {
                    idProductAutocom = 0;
                }
            });
            //$(_classPosEvent.input_searchProduct).focus(function () {
            //    if (localStorage.getItem('Barcode') != 1) {
            //        $(this).autocomplete("search");
            //        $(this).select()
            //    }

            //});
        }
        if (isCustomer) {
            _classPosEvent.input_searchProduct.blur(function () {
                $("#parentautocomplete").css({
                    height: "0",
                    top: "0",
                    "z-index": "0"
                });
            });
            _classPosEvent.input_searchCustomer.autocomplete(
                {
                    appendTo: "#parentautocomplete",
                    autoFocus: true,
                    minLength: 0,
                    delay: 0,

                    source: function (request, response) {
                        $.ajax({
                            global: false,
                            url: "/Selling/Customer/SearchCustomerPos",
                            type: "GET",
                            dataType: "json",
                            data: {
                                text: request.term,
                            },
                            // html: true,
                            success: function (data) {
                                response($.map(data, function (item) {
                                    let texthigh = __highlight(item.name, request.term);
                                    let html =
                                        "<a href='javascript:void(0)'><div class='search-auto'>" +
                                        "<div class='img'><img src='../" + item.img + "'></div>" +
                                        "<div class='tk_name'><span>" + texthigh + "</span><span class='code'>Mã KH: " + item.code + " <br/> SĐT: " + item.phoneNumber + "</div></div></a>";
                                    return {
                                        label: html, value: item.name, idCus: item.id, code: item.code
                                    };
                                }))
                                return { label: request.term, value: request.term };
                            },
                        })

                    },
                    html: true,
                    select: async function (e, ui) {
                        // console.log(ui);
                        $(this).val(ui.item.value);
                        $(this).data("idCus", ui.item.idCus);
                        $(this).data("name", ui.item.value);
                        $(this).data("code", ui.item.code);
                        // $(this).select();
                        $(this).parents(".box-data-customer").find(".btn-cleardata").show();
                        $(this).parents(".box-data-customer").find(".btn-addCus").hide();
                        $(this).parents(".box-data-customer").find("input").attr("readonly", "readonly");
                        $(this).parents(".box-data-customer").addClass("readonly");
                        // xử lý nếu đơn đã có mà chọn khách thay đổi thì update db
                        let checkOrder = $(".action-inv li.active").data("id");
                        if ($('#item-mon').length > 0 && typeof checkOrder != "undefined") {
                            // await eventBanle.updateCustomerOrder(ui.item.idCus, checkOrder);
                            await eventBanle.updateCustomerOrderOffline(0);
                        }
                    },
                }).focus(function () {
                    $("#parentautocomplete").css({
                        height: "calc(100vh - 100px)",
                        top: "52px",
                        "z-index": "5"
                    });

                    $(this).autocomplete("search");
                    $(this).select()
                });

        }



    },//sự kiện autocomple sản phẩm và khách hàng, và select chọn sản phẩm
    eventLoadTFullAmount: async function () {
        let fullamount = parseFloat($(".fullamount").text().replaceAll(",", "")) || 0;

        let discountamount = parseFloat($(".discountamount").val().replaceAll(",", "")) || 0;
        $(".discountamount").val(discountamount.format0VND(3, 3, ""));
        let Total = fullamount - discountamount;

        let Vatrate = parseFloat($("#Vatrate").val()) || 0;
        let VATAmount = parseFloat($(".VATAmount").data("VATAmount")) || 0;
        if (VATAmount == 0) {
            VATAmount = Math.round((Total) * (Vatrate / 100));
        }
        let Amount = Total + VATAmount;
        $(".VATAmount").val(VATAmount.format0VND(3, 3, ""));
        $(".cuspayment").html(Amount.format0VND(3, 3, ""));

        let cussendamount = parseFloat($("ul.action-inv li.active").data("cussendamount")) || 0;
        // if ($(".cussendamount").data("value") == 1) {
        if (cussendamount != 0) {
            //let cuspayment = parseFloat($(".cussendamount").val().replaceAll(",", "")) || 0;//số tieefn khách thanh toán
            let cuspayment = parseFloat(cussendamount) || 0;//số tieefn khách thanh toán
            $(".amoutchange").html((cuspayment - Amount).format0VND(3, 3, ""));
        } else {
            $(".cuspayment").html(Amount.format0VND(3, 3, ""));
            $(".cussendamount").val(Amount.format0VND(3, 3, ""));
            $(".amoutchange").html(0);
        }
    },
    updateCustomerOrder: async function (idcus, idorder) {//update khách hàng khi chọn
        const response = await axios({
            method: 'post',
            url: '/Selling/OrderTable/UpdateCustomOrderTable',
            headers: {
                'Content-Type': `multipart/form-data`,
            },
            data: {
                TypeUpdate: _TypeUpdatePos.ChangedCustomer,
                IdCustomer: idcus,
                IdGuid: idorder
            }
        });
        if (response.data.isValid) {


        } else {
            $(".box-data-customer").find(".btn-cleardata").trigger("click");
        }

    },// update lại khách hàng khi chọn hoặc xóa 
    updateCustomerOrderOffline: function (type = 0) {//update khách hàng khi chọn 0 là thêm, 1 là xóa
        let idCus = parseInt($(".search-customer").data("idCus")) || 0;
        let idOrder = $(".action-inv li.active").data("id");
        if (idCus > 0 && type == 0) {
            let name = $(".search-customer").data("name");
            let code = $(".search-customer").data("code");
            //-------------
            let getloca = localStorage.getItem("ProductsArrays");
            if (typeof getloca != "undefined" && getloca != null) {
                let datas = JSON.parse(getloca);
                if (datas.length > 0) {
                    let foundIndex = datas.findIndex(x => x.Id == idOrder);
                    if (foundIndex != -1) {
                        Customer = {};
                        Customer.Id = idCus;
                        Customer.Name = name;
                        Customer.Code = code;
                        let getdata = datas[foundIndex];
                        getdata.Customer = Customer;
                        datas[foundIndex] = getdata;
                        localStorage.setItem("ProductsArrays", JSON.stringify(datas));
                    } else {
                        toastrcus.error("Đơn hàng không tồn tại vui lòng tải lại trang!");
                        $(".box-data-customer").find(".btn-cleardata").trigger("click");
                    }
                } else {
                    toastrcus.error("Chưa có đơn được tạo!");
                    $(".box-data-customer").find(".btn-cleardata").trigger("click");
                }

            } else {
                toastrcus.error("Lỗi hệ thống, vui lòng tải lại trang");
                $(".box-data-customer").find(".btn-cleardata").trigger("click");
            }
        } else if (type == 1) {
            let getloca = localStorage.getItem("ProductsArrays");
            if (typeof getloca != "undefined" && getloca != null) {
                let datas = JSON.parse(getloca);
                if (datas.length > 0) {
                    let foundIndex = datas.findIndex(x => x.Id == idOrder);
                    if (foundIndex != -1) {
                        Customer = {};
                        let getdata = datas[foundIndex];
                        getdata.Customer = Customer;
                        datas[foundIndex] = getdata;
                        localStorage.setItem("ProductsArrays", JSON.stringify(datas));
                    } else {
                        toastrcus.error("Đơn hàng không tồn tại vui lòng tải lại trang!");
                        $(".box-data-customer").find(".btn-cleardata").trigger("click");
                    }
                } else {
                    toastrcus.error("Chưa có đơn được tạo!");
                    $(".box-data-customer").find(".btn-cleardata").trigger("click");
                }

            } else {
                toastrcus.error("Lỗi hệ thống, vui lòng tải lại trang");
                $(".box-data-customer").find(".btn-cleardata").trigger("click");
            }
        }
    },// update lại khách hàng khi chọn hoặc xóa 
    loadEventremoveCustomer: function () {
        $(".box-data-customer").find(".btn-cleardata").unbind();
        $(".box-data-customer").find(".btn-cleardata").click(async function () {
            $(this).parents(".box-data-customer").find(".btn-cleardata").hide();
            $(this).parents(".box-data-customer").find(".btn-addCus").show();
            $(this).parents(".box-data-customer").find("input").removeAttr("readonly");
            $(this).parents(".box-data-customer").removeClass("readonly");
            $(this).parents(".box-data-customer").find("input").val("");
            $(this).parents(".box-data-customer").find("input").focus();
            // xử lý nếu đơn đã có mà xóa khách thay đổi thì update db
            let checkOrder = $(".action-inv li.active").data("id");
            if ($('#item-mon').length > 0 && typeof checkOrder != "undefined") {
                // await eventBanle.removeCustomer(checkOrder);
                await eventBanle.updateCustomerOrderOffline(1);
            }
            $(this).parents(".box-data-customer").find("input").removeAttr("data-id");
        });
    },// sự  kiện icon xóa khách hàng trong ô nhập để chọn lại khách mới
    removeCustomer: async function (idorder) {
        const response = await axios({
            method: 'post',
            url: '/Selling/OrderTable/UpdateCustomOrderTable',
            headers: {
                'Content-Type': `multipart/form-data`,
            },
            data: {
                TypeUpdate: _TypeUpdatePos.ChangedCustomer,
                IsRemoveCustomer: true,
                IdGuid: idorder
            }
        });
        if (response.data.isValid) {


        }

    },// update lại khách hàng khi chọn hoặc xóa 
    eventAddHtmlVAT: function () {
        let html = ` <div class="input-group mb-3 showselectboxvat selectbox">
                                <div class="input-group-prepend">
                                    <span class="input-group-text" id="basic-addon1" style="display: flex; white-space: nowrap;align-items: center;">Thuế GTGT:
                                            <select id="Vatrate" name="Vatrate" class="Vatrate">
                                                <option value="0">0%</option>
                                                <option value="5">5%</option>
                                                <option value="8">8%</option>
                                                <option value="10" selected>10%</option>
                                            </select>
                                    </span>
                                   
                                </div>
                                <input type="text" class="form-control VATAmount number3" placeholder="Tiền thuế" aria-label="" aria-describedby="basic-addon1">
                            </div>`;
        if ($(".list-inputpayment .showselectboxvat").length == 0) {
            $(".elemdiscount").after(html);
            loadFormatnumber(".showselectboxvat");
        }
    },
    eventChangeCheckboxHDDTVAT: function () {

        let isLoadevent = false;
        if (typeof (Storage) !== "undefined") {
            // Store
            // Retrieve
        } else {
            alert("Liên hệ đội ngủ hỗ  trợ lỗi trình duyệt của bạn không hỗ  trợ Storage");
        }
        if (!isLoadevent) {
            $('.vatmtt-ele input.isCheckVAT').unbind();
            $('.vatmtt-ele input.isCheckVAT').on('ifChecked', function (event) {
                debugger
                localStorage.setItem("VATMTT", true);
                eventBanle.eventAddHtmlVAT();
                eventBanle.loadEventChangeVATRateVATAmount();
                eventBanle.eventLoadTFullAmount();
                eventBanle.saveUpdateDataVATMTT();//lưu loca
            });

            $('.vatmtt-ele input.isCheckVAT').on('ifUnchecked', function (event) {
                debugger
                localStorage.setItem("VATMTT", false);
                $(".showselectboxvat").remove();
                eventBanle.eventLoadTFullAmount();
                eventBanle.saveUpdateDataVATMTT();//lưu loca
            });

            if (typeof localStorage.getItem("VATMTT") != "undefined") {
                if (localStorage.getItem("VATMTT") == "true") {
                    eventBanle.eventAddHtmlVAT();
                    eventBanle.loadEventChangeVATRateVATAmount();
                    //eventBanle.eventLoadTFullAmount();
                    $('.vatmtt-ele input').iCheck('check');

                }
            }
        }

        isLoadevent = true;
    },// action xuất h
    loadEventChangePaymentMethod: function () {
        $('input.icheckpayment').unbind();
        $('input.icheckpayment').on('ifChecked', function (event) {
            eventBanle.saveUpdateDataPaymentMethod();
        });

        $('.input.icheckpayment').on('ifUnchecked', function (event) {

        });
    },// action xuất hDDT
    saveUpdateDataPaymentMethod: function () {
        var getippayment = parseInt($('input[name=idPaymentMethod]:checked', '.paymentMethod').data("id"));
        let getidhoadon = $("ul.action-inv li.active").data("id");
        let getloca = localStorage.getItem("ProductsArrays");
        if (typeof getloca != "undefined" && getloca != null) {
            let datas = JSON.parse(getloca);
            if (datas.length > 0) {
                let foundIndex = datas.findIndex(x => x.Id == getidhoadon);
                if (foundIndex != -1) {
                    let getdata = datas[foundIndex];
                    getdata.IdPaymentMethod = getippayment;
                    datas[foundIndex] = getdata;
                    localStorage.setItem("ProductsArrays", JSON.stringify(datas));
                } else {
                    toastrcus.error("Đơn hàng không tồn tại vui lòng tải lại trang!");
                }
            } else {
                toastrcus.error("Chưa có đơn được tạo!");
            }
        } else {
            toastrcus.error("Lỗi hệ thống, vui lòng tải lại trang");
        }
    },
    saveUpdateDataVATMTT: function () {
        let VATMTT = false;
        if (localStorage.getItem("VATMTT") == "true") {
            VATMTT = true;
        }
        let getidhoadon = $("ul.action-inv li.active").data("id");
        let getloca = localStorage.getItem("ProductsArrays");
        if (typeof getloca != "undefined" && getloca != null) {
            let datas = JSON.parse(getloca);
            if (datas.length > 0) {
                let foundIndex = datas.findIndex(x => x.Id == getidhoadon);
                if (foundIndex != -1) {
                    let getdata = datas[foundIndex];
                    getdata.VATMTT = VATMTT;
                    if (VATMTT) {
                        getdata.VATRate = parseInt($("#Vatrate").val());
                        getdata.VATAmount = parseFloat($(".VATAmount").val().replaceAll(",", ""));
                    } else {
                        getdata.VATRate = NOVATRate;//
                        getdata.VATAmount = 0;
                    }
                    datas[foundIndex] = getdata;
                    localStorage.setItem("ProductsArrays", JSON.stringify(datas));
                } else {
                    toastrcus.error("Đơn hàng không tồn tại vui lòng tải lại trang!");
                }
            } else {
                toastrcus.error("Chưa có đơn được tạo!");
            }
        } else {
            toastrcus.error("Lỗi hệ thống, vui lòng tải lại trang");
        }
    },
    loadOtherTabInvoice: function () {
        let getJsondata = localStorage.getItem("ProductsArrays");
        let loadOrder = JSON.parse(getJsondata);
        let html = "";
        if (typeof loadOrder != "undefined" && loadOrder != null && loadOrder.length > 0) {

            loadOrder.map(function (ele, ind) {

                html += `<li data-id="` + ele.Id + `" class="item-order">
                            <i class="fa fa-exchange"></i>
                            <span>Hóa đơn `+ (ind + 1) + `</span>
                            <i class="fas fa-times"></i>
                        </li>`;
            });
        } else {
            html += `<li data-id="0" class="active item-order">
                        <i class="fa fa-exchange"></i>
                        <span>Hóa đơn 1</span>
                        <i class="fas fa-times"></i>
                    </li>`;
        }
        $(".action-inv").find("li.add-tab").before(html);

        eventBanle.loadEventRemoveTaborderOffline();

    },//mới đầu load các đơn ra
    removeOderOffline: function (sel, idOrder) {
        let iddt = idOrder;
        Swal.fire({
            icon: 'warning',
            title: 'Bạn có chắc chắn muốn xóa đơn không?',
            // showDenyButton: true,
            showCancelButton: true,
            confirmButtonText: 'Đồng ý',
            cancelButtonText: 'Đóng',
            // denyButtonText: `Don't save`,
        }).then((result) => {
            /* Read more about isConfirmed, isDenied below */
            if (result.isConfirmed) {
                $(sel).parent().remove();
                if ($(".action-inv li:not(.add-tab)").length == 0) {
                    $(".action-inv li.add-tab .btn-addInvoice").trigger("click");
                } else {
                    $(".action-inv li:first").trigger("click");
                }
                eventBanle.loadremoveDatalocaOffline(iddt);
            }
        });
    },// sự kiện xóa order
    loadremoveDatalocaOffline: function (getidhoadon) {
        let getloca = localStorage.getItem("ProductsArrays");
        if (typeof getloca != "undefined" && getloca != null) {
            let datas = JSON.parse(getloca);
            if (datas.length > 0) {
                var foundIndex = datas.findIndex(x => x.Id == getidhoadon);
                if (foundIndex != -1) {
                    datas.splice(foundIndex, 1);
                    localStorage.setItem("ProductsArrays", JSON.stringify(datas));
                }
                if (datas.length == 0) {
                    eventBanle.eventresetpaymentinput();
                }
            } else {
                localStorage.setItem("ProductsArrays", "[]");
            }
        } else {
            localStorage.setItem("ProductsArrays", "[]");
        }
    },
    loadEventRemoveTaborderOffline: function (sel) {
        $(".action-inv").find("li").not(".add-tab").find("i:last-child").unbind();
        $(".action-inv").find("li").not(".add-tab").find("i:last-child").click(async function () {
            let idOrder = $(this).parent('li.item-order').data("id");
            if (idOrder != "0") {
                eventBanle.removeOderOffline($(this), idOrder);
            } else {
                if ($(".action-inv li:not(.add-tab)").not($(this).parent(".item-order")).length > 0) {
                    $(this).parent(".item-order").remove();
                    $(".action-inv li:first-child").trigger("click");
                }
            }
        });
    },
    loadClicktabInvoice: function () { // tự load sau khi loadpage và sự kiện click vào bàn lưu lại giá trị đó
        //load các invocie dg có ở máy khách hàng
        eventBanle.loadOtherTabInvoice();
        //check xem có lưu localStorage có lưu table k để active
        // suwj kieensj forcus khách thanh toán
        $(".cussendamount").focus(function () {
            $(this).select();
        });
        $(".action-inv li.item-order").click(async function () {
            debugger
            if (!$(this).hasClass("active")) {// chỉ cho kích 1 lần trên 1 table
                $(".action-inv").find("li.active").not($(this)).removeClass("active");
                $(this).addClass("active");
                let id = $(this).data("id");
                if (id != "0") {
                    await eventBanle.loadDataInvoiceOffline(id);
                    //await eventBanle.loadDataInvoice(id);// phải load dữ liệu trước
                }
                saveTableActive($(this).index());
                //$(".search-customer").focus(function () {
                //    $(this).parent(".input-group").addClass("focus-input");
                //});
                //$(".search-customer").blur(function () {
                //    $(this).parent(".input-group").removeClass("focus-input");
                //});
            }

        });
        loadTableActive();
        function saveTableActive(_cl) { // lưu lại giá trị mới localStorage
            if (typeof (Storage) !== "undefined") {
                // Store
                localStorage.setItem("activeInvoicetab", _cl); // tham số active table
                // Retrieve
            } else {
                alert("Liên hệ đội ngủ hỗ  trợ lỗi trình duyệt của bạn không hỗ  trợ Storage");
            }
        }
        function loadTableActive() { // load active load lần đầu

            if (typeof (Storage) !== "undefined") {
                // Store
                var _getActivetable = localStorage.getItem("activeInvoicetab");
                if (!isNaN(parseInt(_getActivetable))) {
                    _getActivetable = parseInt(_getActivetable) + 1;
                    if ($(".action-inv li").length < _getActivetable) {
                        _getActivetable = 1;
                    }
                    let ele = $(".action-inv li.item-order:nth-child(" + _getActivetable + ")"); // là số thì active cái đó
                    if (ele.length == 0) {
                        ele = $(".action-inv li.item-order:nth-child(1)");// k thì phải active cái đầu
                    }
                    // eventBanle.loadDatatable(ele);
                    eventBanle.loadDatatableOffline(ele);
                } else {
                    // let ele = $("#lst-roomandtable li:nth-child(1)").trigger("click");// k thì phải active cái đầu
                    let ele = $(".action-inv li.item-order:nth-child(1)");// k thì phải active cái đầu
                    // eventBanle.loadDatatable(ele);
                    eventBanle.loadDatatableOffline(ele);
                }
                // Retrieve
            } else {
                alert("Liên hệ đội ngủ hỗ  trợ lỗi trình duyệt của bạn không hỗ  trợ Storage");
            }
        }

    },
    eventChangeCheckbocVAT: function () {

        $.ajax({
            type: "GET",
            global: false,
            url: "/Selling/ConfigSaleParameters/GetConfigSell?key=" + getObjectKey(EnumConfigParameters, EnumConfigParameters.AUTOVATINPAYMENT),
            success: function (res) {

                if (res.isValid) {

                    if (res.data.typeValue == EnumTypeValue.BOOL && res.data.value == "false") {

                        if ($(".vatmtt-ele").length > 0) {
                            $(".vatmtt-ele").removeClass("d-none");
                        }

                    } else {
                        if ($(".vatmtt-ele").length > 0) {
                            $(".vatmtt-ele").remove();
                        }
                        localStorage.setItem("VATMTT", "true");

                    }
                } else {
                    if ($(".vatmtt-ele").length > 0) {
                        $(".vatmtt-ele").removeClass("d-none");
                    }
                }
            },
            error: function () {
                alert("Error occured!!");
            }
        });

        $(".isCheckVAT").click(function () {
            let vat = $(this).is(':checked');
            localStorage.setItem("VATMTT", vat);
        });
        if (typeof localStorage.getItem("VATMTT") != "undefined") {
            //$(".paymentvatmtt").attr('checked', localStorage.getItem("VATMTT"))

            if (localStorage.getItem("VATMTT") == "true") {
                $(".isCheckVAT").attr('checked', 'checked');
            }

        }
    },
    loadDatatable: async function (sel) {
        sel.addClass("active");
        id = $(sel).data("id");
        if (id != 0) {
            await eventBanle.loadDataInvoice(id);// phải load dữ liệu trước
        } else {
            eventBanle.loadAmoutAndQuantity(0, 0);
            eventBanle.eventChangeCheckboxHDDTVAT();
            return false;
        }
        $(".search-customer").focus(function () {
            $(this).parent(".input-group").addClass("focus-input");
        });
        $(".search-customer").blur(function () {
            $(this).parent(".input-group").removeClass("focus-input");
        });
    },
    loadDatatableOffline: async function (sel) {
        sel.addClass("active");
        id = $(sel).data("id");
        if (id != 0) {
            await eventBanle.loadDataInvoiceOffline(id);// phải load dữ liệu trước
        } else {
            eventBanle.loadAmoutAndQuantity(0, 0);
            eventBanle.eventChangeCheckboxHDDTVAT();
            return false;
        }
        $(".search-customer").focus(function () {
            $(this).parent(".input-group").addClass("focus-input");
        });
        $(".search-customer").blur(function () {
            $(this).parent(".input-group").removeClass("focus-input");
        });
    },
    loadDataInvoiceOffline: function (idinvoice) {

        if (idinvoice == 0) {
            let html = `<div role="tabpanel">
                             <div class="tab-pane " id="tab-order-0" role="tabpanel">
                                <div class="no-order">
                                    <img src="../images/shopping.png" />
                                    <b>Chưa có hàng hóa nào được chọn</b>
                                    <span>Vui lòng chọn hàng hóa</span>
                                </div>
                            </div>
                        </div>`;
            $('#container-tableOder').html(html);
            eventBanle.loadAmoutAndQuantity(0, 0);
            $(".discountamount").data("discount", 0)
            $(".discountamount").data("value", 0);
            $(".discountamount").data("type", TypeSelectDiscount.Cash)
            return false;
        }
        let getJsondata = localStorage.getItem("ProductsArrays");
        let loadOrder = JSON.parse(getJsondata);


        if (typeof loadOrder != "undefined" && loadOrder != null && loadOrder.length > 0) {
            let getindex = loadOrder.findIndex(x => x.Id == idinvoice);
            eventBanle.loadHtmloffline(loadOrder[getindex]);
        } else {
            eventBanle.loadAmoutAndQuantity(0, 0)
            return false;
        }

    },
    loadHtmloffline: async function (json) {
        let product = json.Items;
        let html = `<ul id="item-mon"> `;
        product.map(function (ele, index) {
            let htmldiscount = "";
            let pricenew = ele.PriceNew;
            if (ele.Typediscount != -1 && ele.DiscountAmount > 0) {
                if (ele.Typediscount == 1) {
                    htmldiscount = "<span class='discountAmountItemPro'>" + (ele.Discount * -1) + "%</span>"
                } else {
                    htmldiscount = "<span class='discountAmountItemPro'>" + (ele.DiscountAmount * -1).format0VND(0, 3) + "</span>"
                }
            }
            html += `<li data-code="` + ele.Code + `" data-idpro ="` + ele.Id + `" data-sl=` + ele.Quantity + `>
                            <div  class="btn-remove" data-idquan="1"><i data-idquan="1" class="fas fa-trash-alt"></i></div>
                            <div class="name" data-name="`+ ele.Name + `"><b>` + (index + 1) + `.` + ele.Name + `</b></div>
                            <div class="item_action"><i class="fas fa-minus"></i><input data-quantity="`+ ele.Quantity + `" class="quantity number3" value="` + ele.Quantity + `"><i class="fas fa-plus"></i></div>
                            <div class="ele-price"><input type="text" class="form-control number3 price"  data-price="`+ ele.Price + `"  data-pricenew="` + ele.PriceNew + `"  data-retailPrice="` + ele.RetailPrice + `" data-typediscount="` + ele.Typediscount + `" data-discountamount="` + ele.DiscountAmount + `" data-discount="` + ele.Discount + `" readonly value="` + (pricenew) + `" />` + htmldiscount + `</div>
                            <div class="total"><b class="number3" data-total="`+ (ele.Amount) + `">` + ele.Amount.format0VND(0, 3) + `</b></div>
                        </li>`;

        });
        html += `</ul>`;
        $("#container-tableOder").html(html);
        $(".fullamount").text(json.Total.format0VND(3, 3));
        $(".discountamount").val(json.DiscountAmount.format0VND(3, 3));

        $(".discountamount").data("discount", json.Discount);
        if (json.Discount > 0) {
            $(".discountamount").data("value", json.Discount)
            $(".discountamount").data("type", TypeSelectDiscount.Percent);
        } else {
            $(".discountamount").data("value", json.DiscountAmount)
            $(".discountamount").data("type", TypeSelectDiscount.Cash);
        }


        //$("ul.action-inv li.active").data("discountamount", json.DiscountAmount)
        //------
        eventBanle.loadeventchangeclickprice();//kích vào giá
        evetnFormatTextnumber3();
        eventBanle.loadEventClickIconAddAndMinusoffline();

        eventBanle.eventChangeCheckboxHDDTVAT();
        eventBanle.loadShowOrHideCheckVATMTT(json.VATMTT);//sau cái sự kiện

        $('#Vatrate option').each(function (i, e) {
            e.selected = false
        });
        $("#Vatrate option[value='" + json.VATRate + "']").prop("selected", "selected");

        //---------thuế suất
        if (json.VATMTT) {
            $(".VATAmount").data("VATAmount", json.VATAmount);//đưa xuống vì phải load html vatamoutn ra mới có cái để đưa vào
        }
        await eventBanle.eventLoadTFullAmount().then(() => {
            if (json.VATMTT) {
                $(".VATAmount").removeData("VATAmount");
            }
        });
        //------------- load hình thức thanh toán
        var getippayment = parseInt($('input[name=idPaymentMethod]:checked', '.paymentMethod').data("id"));
        if (getippayment != json.IdPaymentMethod) {
            $('input[name=idPaymentMethod]:checked', '.paymentMethod').iCheck("uncheck");
            $('input[data-id=' + json.IdPaymentMethod + ']', '.paymentMethod').iCheck("check")
        }
        //---------
        _classPosEvent = {
            input_searchProduct: $(".search-product"),
            input_searchCustomer: $(".search-customer"),
        }
        loadeventPos.loadEventkeyCode();
        eventBanle.loadCustomerByOrderOfline(json.Customer);//load khách hàng
        eventBanle.loadSugetionPayment();

    },
    loadShowOrHideCheckVATMTT: function (isShow = true) {
        if (!isShow) {
            $("input[name=isCheckVAT]").iCheck('uncheck');
        } else {
            $("input[name=isCheckVAT]").iCheck('check');
        }
    },
    loadCustomerByOrderOfline: function (Customer) {
        // thêm điều kiện nút xóa khách
        let JSeach = $(".search-customer");
        if (Customer.Id != "" && Customer.Id != null) {
            JSeach.val(Customer.Name);
            JSeach.parents(".box-data-customer").find(".btn-cleardata").show();
            JSeach.parents(".box-data-customer").find(".btn-addCus").hide();
            JSeach.parents(".box-data-customer").find("input").attr("readonly", "readonly");
            JSeach.parents(".box-data-customer").addClass("readonly");
            JSeach.parents(".box-data-customer").data("idCus", Customer.Id);
            JSeach.parents(".box-data-customer").data("code", Customer.Code);
            JSeach.parents(".box-data-customer").data("name", Customer.Name);
        } else {
            eventBanle.loadEventResetCusomter();
        }
    },
    loadEventResetCusomter: function () {
        let JSeach = $(".search-customer");
        JSeach.val("");
        JSeach.parents(".box-data-customer").find(".btn-cleardata").hide();
        JSeach.parents(".box-data-customer").find(".btn-addCus").show();
        JSeach.parents(".box-data-customer").find("input").removeAttr("readonly");
        JSeach.parents(".box-data-customer").removeClass("readonly");
        JSeach.parents(".box-data-customer").data("idCus", 0);
        JSeach.parents(".box-data-customer").data("code", "");
        JSeach.parents(".box-data-customer").data("name", "");
    },// sự  kiện icon xóa khách hàng trong ô nhập để chọn lại khách mới
    loadDataInvoice: async function (idinvoice) {
        if (idinvoice == 0) {
            let html = `<div role="tabpanel">
                             <div class="tab-pane " id="tab-order-0" role="tabpanel">
                                <div class="no-order">
                                    <img src="../images/shopping.png" />
                                    <b>Chưa có hàng hóa nào được chọn</b>
                                    <span>Vui lòng chọn hàng hóa</span>
                                </div>
                            </div>
                        </div>`;
            $('#container-tableOder').html(html);
            eventBanle.loadAmoutAndQuantity(0, 0)
            return false;
        }
        var loadOrder = await axios.get("/Selling/OrderTable/LoadDataByOrderInvoice?idinvoice=" + idinvoice);
        if (loadOrder.data.isValid) {
            $("#container-tableOder").html(loadOrder.data.data);
            //loadeventPos.eventUpdatedataItemMonOrder();
            if (loadOrder.data.dataCus != null && loadOrder.data.dataCus != "") {
                CusByOrderIvnoice = loadOrder.data.dataCus;
            }

            if (loadOrder.data.dataNote != null && loadOrder.data.dataNote != "") {
                NoteByOrderIvnoice = loadOrder.data.dataNote;
            }

            eventBanle.loadAmoutAndQuantity(loadOrder.data.amount, loadOrder.data.quantity)
            eventBanle.loadTienthuakhach()
            eventBanle.loadSugetionPayment()
            eventBanle.loadeventchangeclickprice();//kích vào giá


        } else {
            eventBanle.loadAmoutAndQuantity(0, 0)
            return false;
        }
        loadeventPos.loadEventAddCustomer();// load sự kiện thêm khách hàng
        _classPosEvent = {
            input_searchProduct: $(".search-product"),
            input_searchCustomer: $(".search-customer"),
        }
        eventBanle.loadEventClickIconAddAndMinusoffline();// sử lý ở đây tạm thời ẩn
        loadeventPos.loadEventkeyCode();
        // loadeventPos.loadAddTabOrder();
        eventBanle.loadAutocomplete(false, true);
        // loadeventPos.loadEventClose();

        //loadeventPos.loadClicktabOrder();
        // $('#ul-tab-order a:first').trigger('click');
        //loadeventPos.loadAmoutAndQuantity();// load tong tien

        eventBanle.loadCustomerByOrder();// load khasch hang
        eventBanle.eventChangeCheckboxHDDTVAT();

    }, // sự kiện load dữ liệu order  của 1 bàn bất kỳ
    loadCustomerByOrder: function () {
        // thêm điều kiện nút xóa khách
        let JSeach = $(".search-customer");
        if (CusByOrderIvnoice.idCustomer != "" && CusByOrderIvnoice.idCustomer != null) {
            JSeach.val(CusByOrderIvnoice.customerCode + " " + CusByOrderIvnoice.customerName);
            JSeach.parents(".box-data-customer").find(".btn-cleardata").show();
            JSeach.parents(".box-data-customer").find(".btn-addCus").hide();
            JSeach.parents(".box-data-customer").find("input").attr("readonly", "readonly");
            JSeach.parents(".box-data-customer").addClass("readonly");
            JSeach.parents(".box-data-customer").data("id", CusByOrderIvnoice.idCustomer);
        } else {
            JSeach.parents(".box-data-customer").find(".btn-cleardata").hide();
            JSeach.parents(".box-data-customer").find(".btn-addCus").show();
            JSeach.parents(".box-data-customer").find("input").removeAttr("readonly");
            JSeach.parents(".box-data-customer").removeClass("readonly");
            JSeach.parents(".box-data-customer").data("id", 0);
        }
    },

    loadEventAddCustomer: function () {
        $(".btn-addCus").click(function () {
            eventCreate.addOrEditCustomer('/Selling/Customer/Create?IsPos=true');
        });
    },
}
var loadeventPos = {
    eventShowTabProductByCategory: function () {
        $.ajax({
            global: false,
            type: "GET",
            url: "/Selling/Product/GetProductJson",
            dataType: 'json',
            beforeSend: function () {

            },
            success: function (res) {
                if (res.isValid) {
                    htmlcate = `<ul class="ltscategoryProduct">`;
                    htmlcate += `<li class="active">
                                   <a href="javascript:void(0)" data-id="0">
                                        <b>`+ res.jsonPro.length + `</b> <span>Tất cả</span>
                                    </a>
                                 </li>`;

                    res.jsoncate.forEach(function (item, index) {
                        htmlcate += `<li>
                                         <a href="javascript:void(0)" data-id="`+ item.id + `">
                                             <b>`+ item.countpro + `</b> <span>` + item.name + `</span>
                            
                                         </a>
                                         <span class="tooltipshow">` + item.name + `</span>
                                      </li>`;
                    });
                    htmlcate += '</ul>';
                    $(".bodyDataproduct .leftcolumthucdon").html(htmlcate);
                    htmlcate = `<ul id="lst-product" class="ul-nonestyle">`;

                    res.jsonPro.forEach(function (item, index) {
                        console.log(item.img);
                        htmlimg = '<img src="../' + item.img + '">';

                        if (item.img == "" || item.img == null) {
                            //htmlimg = '<i class="fas fa-utensils"></i>'; 
                            htmlimg = '<img class="ristorante" src="../images/ristorante_old.png">';
                        }
                        htmlcate += ` <li data-id="` + item.id + `" data-idcategory="` + item.idCategory + `">
                                            <div class="head_pro">
                                                    `+ htmlimg + `
                                                </div>
                                             <div class="footer_pro">
                                                 <span>`+ item.name + `</span>
                                                 <b class="required priceFormat">`+ item.retailPrice + `</b>
                                          </div>
                                      </li>`;
                    });
                    htmlcate += '</ul>';
                    $(".bodyDataproduct .rightcolumthucdon").html(htmlcate);
                    //update lại iddata
                    $(".bodyDataproduct .leftcolumthucdon ul li").map(function () {
                        //let idata = $(this).children("a").data("id");
                        let idatacate = $(this).children("a").data("id");
                        $(this).children("a").removeAttr("data-id");
                        $(this).children("a").data("id", idatacate);
                    });
                    $(".bodyDataproduct .rightcolumthucdon ul li").map(function () {
                        let idata = $(this).data("id");
                        let idatacate = $(this).data("idcategory");
                        $(this).removeAttr("data-idcategory");
                        $(this).removeAttr("data-id");
                        $(this).data("idcategory", idatacate);
                        $(this).data("id", idata);
                    });

                    $(".bodyDataproduct .leftcolumthucdon ul.ltscategoryProduct a").click(function () {
                        let ididcategory = $(this).data("id");
                        $(".bodyDataproduct .rightcolumthucdon ul li").filter(function () {
                            var element = $(this);
                            let idcategory = $(this).data("idcategory");
                            if (ididcategory == idcategory || ididcategory == 0) {
                                element.css('display', "flex");
                            } else {
                                element.css('display', "none");

                            }
                        });
                        $(this).parents("ul.ltscategoryProduct").find("li").removeClass("active");
                        $(this).parent("li").addClass("active");
                    });
                    loadeventPos.loadClickItemProduct();
                    priceFormat();
                }
            }
        });
    },
    updateChangetableOrder: function () {
        _dtselect = parseInt($("input[name=orderActions]:checked").data("id"))//0 mang về,1 chọn bàn
        _dtIdTabale = $("#lstroomselect").val();
        _dtOldIdTable = $("#OldIdTable").val();
        if (isNaN(_dtselect)) {
            toastrcus.error("Bạn chưa chọn bàn cần chuyển!");
            return;
        }
        else if (_dtselect == 1 && _dtIdTabale.replaceAll(" ", "") == "") {
            toastrcus.warning("Vui lòng chọn bàn cần chuyển!");
            return;
        }
        idOrder = $("#ul-tab-order a.active").data("id");
        dataObject = {
            IdOrder: idOrder,
            TypeSelectTable: _dtselect,
            IdTable: _dtIdTabale,
            OldIdTable: _dtOldIdTable,
            TypeUpdate: _TypeUpdatePos.UpdateRoomOrTableInOrder
        };
        $.ajax({
            type: 'POST',
            url: '/Selling/OrderTable/ChangeTableInOrder',
            // contentType: 'application/json',
            dataType: 'json',
            data: dataObject,
            // traditional: true,
            success: async function (res) {
                //console.log(res.data);
                if (res.isValid) {
                    checkRooom = 0;///kiểm tra xem bàn đó còn đơn nào k
                    $("#ul-tab-order li:not(.add-tab)").map(function () {
                        if (!$(this).children("a").hasClass("active") && typeof $(this).children("a").data("id") != "undefined" && $(this).children("a").data("id") != "") {
                            checkRooom += 1;
                        }
                    });

                    if (checkRooom > 0) {// tức là còn đơn khác trên bàn
                        $('#ul-tab-order li').filter(function () {
                            var element = $(this);
                            if (element.find("a").hasClass("active")) {
                                element.remove();
                                $('#ul-tab-order a:first').trigger('click');
                            }
                        });

                        $("#lst-roomandtable li").map(function () {
                            idata = $(this).data("id");
                            if (idata == res.idTable) {
                                $(this).addClass("CurentOrder");
                            }
                        });
                    } else {
                        let liele;
                        $("#lst-roomandtable li.active").removeAttr("class");
                        $("#lst-roomandtable li").map(function () {
                            idata = $(this).data("id");
                            if (idata == res.idTable) {
                                liele = $(this);
                            }
                        });
                        liele.trigger("click")
                    }
                    //if (res.IsBringback) {
                    //    $(".btn-showttable").data("id", "-1");
                    //} else {
                    //    $(".btn-showttable").data("id", res.idTable);
                    //}

                    //$(".showTableOrder").html(res.tableName);
                    loadeventPos.eventloadNumberStatusTable();// load đếm bàn
                    Swal.close();
                }
            },
            error: function (err) {
                console.log(err)
            }
        });

    },
    loadeventChangetableOrder: function () {
        $(".btn-showttable").click(function () {
            idtable = $(this).data("id");
            idOrder = $("#ul-tab-order a.active").data("id");
            isbringback = false;
            if (idtable == "-1") {
                isbringback = true;
                idtable = "";
            }
            if (idOrder != "" && typeof idOrder != "undefined") {
                eventCreate.editTableInOrder("/Selling/OrderTable/ChangeTableInOrder?IdOrder=" + idOrder + "&IdRoomAndTable=" + idtable + "&isbringback=" + isbringback);
            }
        });
    },//thay đổi bàn cho đơn
    loadAddOrRemoveCurentClassTable: function (isAdd = false) {
        if (!isAdd) {
            $("#lst-roomandtable li.active").removeClass("CurentOrder");// nếu đã hết sản phẩm tức là không đặt nữa thì xóa curen đi
        } else {
            if (!$("#lst-roomandtable li.active").hasClass("CurentOrder")) {
                $("#lst-roomandtable li.active").addClass("CurentOrder");// nếu theem sản phẩm thi active CurentOrder
            }
        }
        loadeventPos.eventloadNumberStatusTable();
    },
    checkHighlightTableInOrder: function () {
        let isExit = false;
        $("#ul-tab-order li:not(.add-tab)").map(function (ind, ele) {
            let idData = $(ele).children("a").data("id");
            if (typeof idData != "undefined" && idData != "") {
                isExit = true;
            }
        });
        if (!isExit) {
            loadeventPos.loadAddOrRemoveCurentClassTable(false);
        }
    },
    eventUpdateQuantityItemMonOrder: function () {
        $(".tab-content-order .tab-pane.active ul#item-mon").children("li").map(function () {
            let sl = $(this).data("sl");
            $(this).data("slnotify", sl)
        });
        $(".btn-notif").attr("disabled", "disabled");
    },
    eventCheckBtnNotifyOrder: function (sel) {
        let checkShowNotify = false;
        let id = $(sel).attr("href");
        $(id).find("ul#item-mon").children("li").map(function () {

            let sl = $(this).data("sl");
            let slnotify = $(this).data("slnotify");
            if (slnotify != sl) {
                checkShowNotify = true;
            }
        });

        if (checkShowNotify) {
            $(".btn-notif").removeAttr("disabled");
        } else {
            $(".btn-notif").attr("disabled", "disabled");
        }
    },

    eventUpdatedataItemMonOrder: function () {
        let checkShowNotify = false;
        $("#container-tableOder ul#item-mon").children("li").map(function () {
            let idpro = $(this).data("idpro");
            let iditem = $(this).data("id");
            let sl = $(this).data("sl");
            let slnotify = $(this).data("slnotify");
            $(this).removeAttr("data-id");
            $(this).removeAttr("data-idpro");
            $(this).removeAttr("data-sl");
            $(this).removeAttr("data-slnotify");
            $(this).data("idpro", idpro)
            $(this).data("id", iditem)
            $(this).data("sl", sl)
            $(this).data("slnotify", slnotify)

            // check show bnt notify
            if (slnotify != sl) {
                checkShowNotify = true;
            }
        });

        if (checkShowNotify) {
            $(".btn-notif").removeAttr("disabled");
        } else {
            $(".btn-notif").attr("disabled", "disabled");
        }
    },
    orderTable: function (dataObject) {
        $.ajax({
            type: 'POST',
            url: '/Selling/OrderTable/AddOrderTable',
            // contentType: "application/json; charset=utf-8",
            dataType: 'json',
            data: dataObject,
            // traditional: true,
            success: async function (res) {
                //console.log(res.data);

                if (res.isValid) {
                    if (res.data.orderTableItems.length == 0) {
                        if (res.data.isBringBack) {
                            res.data.idRoomAndTableGuid = "-1";
                        }
                        await loadeventPos.loadOrderByTable(res.data.idRoomAndTableGuid);

                        loadeventPos.checkHighlightTableInOrder();// check highlight
                        // sau khi load lại gán order mới thì update bàn active qua tab order
                        let GetTable = $("#lst-roomandtable").find("li.active");
                        $(".btn-showttable").attr("data-id", GetTable.data("id"));
                        $(".btn-showttable").find(".showTableOrder").html(GetTable.find("b").html());
                    } else {
                        let html = `<ul id="item-mon"> `;
                        res.data.orderTableItems
                            .forEach(function (item, index) {
                                index = index + 1;
                                html += ` <li data-id="` + item.idGuid + `" data-idpro ="` + item.idProduct + `" data-slNotify=` + item.quantityNotifyKitchen + ` data-sl=` + item.quantity + ` >
                                            <div  class="btn-remove" data-idquan="`+ item.quantity + `"><i data-idquan="` + item.quantity + `" class="fas fa-trash-alt"></i></div>
                                            <div class="name"><b>` + index + ". " + item.name + `</b></div>
                                            <div class="item_action"><i class="fas fa-minus"></i><input class="quantity numberformat" value="`+ item.quantity + `"> <i class="fas fa-plus"></i></div>
                                            <div><input type="text" class="form-control priceFormat" readonly value="`+ (item.price) + `" /></div>
                                            <div class="amount"><b class="priceFormat">`+ (item.total) + `</b></div>
                                        </li>`;
                                // <div class="item_action"><i class="fas fa-minus"></i><span class="quantity">`+ item.quantity + `</span> <i class="fas fa-plus"></i></div>
                            });
                        html += `</ul>`;

                        $("#ul-tab-order").find("a.active").data("id", res.data.idGuid);
                        $("#ul-tab-order").find("a.active").parent("li").data("title", "");// phải gán  = "" mới có hiệu lực chỉ xóa đi thì k dc
                        // $("#ul-tab-order").find("a.active").parent("li").removeAttr("data-title");
                        $("#ul-tab-order").find("a.active").find("span.orderCode").html(res.data.orderCode);
                        $(".tab-content-order").find(".tab-pane.active").html(html);


                        loadeventPos.eventUpdatedataItemMonOrder();


                        $(".tab-content-order").find(".tab-pane.active").attr("data-id", res.data.idGuid);
                        loadeventPos.loadAmoutAndQuantity(res.data.amount, res.data.quantity);
                        loadeventPos.loadEventClickIconAddAndMinus();
                        loadeventPos.loadAddOrRemoveCurentClassTable(true);// xem có sản phẩm thì add class curen table
                        // console.log(dataObject.IdOrderItem);

                        loadeventPos.loadactiveClickItemMon(dataObject.IdOrderItem, dataObject.IdProduct);// 


                        // xử lý giữ lại khách hàng khi có nhiều tab dg mở nhưng kích vào tab active xong kích qua lại bị mất khách k hiển thị
                        if (dataObject.TypeUpdate == _TypeUpdatePos.AddProduct && typeof dataObject.IdGuid == "undefined") {//xử lý với cái thêm mới

                            let findId = ListCusByOrderPos.find(x => x.idOrder == res.data.idGuid);
                            let cus = $(".search-customer").val();
                            if (typeof findId == "undefined" && cus != "") {
                                let arrCus = {};
                                arrCus.idOrder = res.data.idGuid;
                                arrCus.customerCode = cus.split(" ")[0];
                                arrCus.customerName = res.data.buyer;
                                ListCusByOrderPos.push(arrCus);
                            }
                        }

                    }
                }
            },
            error: function (err) {
                console.log(err)
            }
        });
    },// sự kiện order
    loadactiveClickItemMon: function (idorder, IdProduct) {
        $("ul#item-mon").children("li").map(function () {
            if ($(this).data("id") == idorder || $(this).data("idpro") == IdProduct) {
                $(this).addClass("animatino");
                $(this).addClass("active");
                sel = $(this);
                setTimeout(function () {
                    $(sel).removeClass("animatino");
                }, 300);

                // $(sel).parents("ul#item-mon").find("li").not($(sel).parents("li")).removeClass("active");
            }
        });
    },
    loadeventclickitemmon: function () {
        $("#item-mon li").click(function () {
            $("#item-mon li.active").removeClass("active");
            $(this).addClass("active");
        })
    },  /// sự kiện kích item

    loadEventClickIconAddAndMinus: function () {
        $("#item-mon  li .item_action").find("i").unbind();
        $("#item-mon  li .item_action").find("i:first-child").click(function () {
            let IdOrderItem = $(this).parent().parent("li").data("id");
            let idTable = $(".btn-showttable").data("id");
            let cusocde = $(".search-customer").val();
            let slgoc = $(this).parent().parent("li").data("sl");
            let slnotify = $(this).parent().parent("li").data("slnotify");
            let IdCustomer = "";
            if (cusocde.trim() != "") {
                cusocde = cusocde.split(" ")[0];
                IdCustomer = $(".search-customer").data("id");
            }
            //bấm giảm thì phải hỏi

            if (parseFloat(slnotify) > (parseFloat(slgoc) - 1)) {
                let namepro = $(this).parents("li").find(".name").find("b").html();
                let html = `
                            <div class="form-confirmremoveitem">
                                <span>Bạn có chắc chắn muốn hủy món <b>`+ namepro + `</b> không?</span>
                                <div class="slminus">
                                    <span>Số lượng hủy/giảm</span>
                                    <div class="item_action"><i class="fas fa-minus"></i><span data-slhuy="1" class="quantitynew">1</span><span class="quantityold">/`+ parseFloat(slnotify) + `</span> <i class="fas fa-plus"></i></div>
                                </div>
                                <div class="input-note">
                                    <span class="text">Lý do: </span>
                                     <input type="text" id="noteminus" class="form-control"/>
                                </div>
                            </div>
                            `;
                Swal.fire({
                    // icon: 'success',
                    title: "Xác nhận giảm / Hủy món",
                    html: html,
                    showClass: {
                        popup: 'popup-formcreate'
                    },
                    footer: "<button class='swal2-cancel swal2-styled btn-cancel mr-3'><i class='icon-cd icon-add_cart icon'></i>Hủy bỏ</button><button class='swal2-styled btn btn-success btn-save'><i class='icon-cd icon-doneAll icon'></i>Đồng ý</button>",
                    allowOutsideClick: true,
                    showConfirmButton: false,
                    showCancelButton: false,
                    cancelButtonText: '<i class="fa fa-window-close"></i> Đóng',
                    didRender: () => {
                        //$('.form-confirmremoveitem select').prepend('<option selected></option>').select2({

                        //    placeholder: {
                        //        id: '', // the value of the option
                        //        text: "Chọn bàn"
                        //    },
                        //    allowClear: true,
                        //    language: {
                        //        noResults: function () {
                        //            return "Không tìm thấy dữ liệu";
                        //        }
                        //    },
                        //})
                        $(".form-confirmremoveitem .item_action i:first-child").click(function () {

                            let slafter = parseFloat($(this).parent().children(".quantitynew").data("slhuy"));
                            if (slafter >= 0 && slafter < 1) {
                                slafter = parseDecimal(slafter - 0.1);
                            } else {
                                slafter = parseDecimal(slafter - 1);
                            }
                            if (slafter <= 0) {
                                toastr.error("Sô lượng hủy/giảm phải lớn hơn 0");
                                return false;
                            } else {

                                $(this).parent().children(".quantitynew").data("slhuy", slafter);
                                $(this).parent().children(".quantitynew").html(slafter);
                            }

                        });
                        $(".form-confirmremoveitem .item_action i:last-child").click(function () {
                            let slafter = parseFloat($(this).parent().children(".quantitynew").data("slhuy"));
                            if (slafter >= parseFloat(slnotify)) {
                                toastr.error("Sô lượng hủy/giảm không vượt quá số lượng gốc");
                                return false;
                            }
                            else {
                                if (slafter >= 0 && slafter < 1) {
                                    slafter = parseDecimal(slafter + 0.1);
                                } else {
                                    slafter = parseDecimal(slafter + 1);
                                }
                                $(this).parent().children(".quantitynew").data("slhuy", slafter);
                                $(this).parent().children(".quantitynew").html(slafter);
                            }
                        });


                        $(".btn-cancel").click(function () {
                            Swal.close();
                        });
                        $(".btn-save").click(function () {

                            let slafter = parseFloat($(".form-confirmremoveitem .item_action").children(".quantitynew").data("slhuy"));
                            let noteminus = $(".form-confirmremoveitem").find("#noteminus").val();
                            var dataObject = {
                                Note: noteminus,
                                IsCancel: true,
                                IdOrderItem: IdOrderItem,
                                CusCode: cusocde,
                                IdCustomer: IdCustomer,
                                QuantityFloat: (slafter * -1),
                                TypeUpdate: _TypeUpdatePos.UpdateQuantity,
                                IdRoomAndTableGuid: idTable != "-1" ? idTable : "",
                                IsBringBack: idTable == "-1" ? true : false,
                                IdGuid: $("#ul-tab-order").find("a.active").data("id")
                            };// giảm số lượng

                            loadeventPos.orderTable(dataObject);
                            Swal.close();
                        });

                    }
                });

            }
            else {
                var dataObject = {
                    IdOrderItem: IdOrderItem,
                    CusCode: cusocde,
                    IdCustomer: IdCustomer,
                    QuantityFloat: -1,
                    TypeUpdate: _TypeUpdatePos.UpdateQuantity,
                    IdRoomAndTableGuid: idTable != "-1" ? idTable : "",
                    IsBringBack: idTable == "-1" ? true : false,
                    IdGuid: $("#ul-tab-order").find("a.active").data("id")
                };// giảm số lượng
                loadeventPos.orderTable(dataObject);
            }


            //  loadeventPos.loadactiveClickItemMon($(this));

        });// dùng để giảm món
        $("#item-mon  li input.quantity").unbind();
        $("#item-mon  li .item_action").find("input.quantity").change(function () {
            let sel = ($(this));
            let IdOrderItem = $(this).parent().parent("li").data("id");
            let idTable = $(".btn-showttable").data("id");
            let cusocde = $(".search-customer").val();
            let slgoc = $(this).parent().parent("li").data("sl");
            // $(this).val(slgoc);
            // return;
            let slnew = $(this).val();
            let slnotify = $(this).parent().parent("li").data("slnotify");
            let IdCustomer = "";
            if (cusocde.trim() != "") {
                cusocde = cusocde.split(" ")[0];
                IdCustomer = $(".search-customer").data("id");
            }

            //bấm giảm thì phải hỏi
            if (parseFloat(slnew) <= 0) {
                $(this).val(slgoc);
                toastrcus.error("Số lượng không hợp lệ phải lớn hơn 0");
                return;
            }
            if (parseFloat(slnotify) > parseFloat(slnew)) {
                let slcanuychuabao = parseFloat((parseFloat(slgoc) - parseFloat(slnew)).toFixed(3));//sl góc bao gồm cả thông bsao và chưa báo
                let slcanuy = parseFloat(slnotify) - parseFloat(slnew);
                let namepro = $(this).parents("li").find(".name").find("b").html();
                let html = `
                            <div class="form-confirmremoveitem">
                                <span>Bạn có chắc chắn muốn hủy món <b>`+ namepro + `</b> không?</span>
                                <div class="slminus">
                                    <span>Số lượng hủy/giảm</span>
                                    <div class="item_action"><i class="fas fa-minus"></i><span data-slhuy="`+ slcanuychuabao + `" class="quantitynew">` + slcanuychuabao + `</span><span class="quantityold">/` + slnotify + `</span> <i class="fas fa-plus"></i></div>
                                </div>
                                <div class="input-note">
                                    <span class="text">Lý do: </span>
                                    <input type="text" id="noteminus" class="form-control"/>
                                </div>
                            </div>
                            `;
                Swal.fire({
                    // icon: 'success',
                    title: "Xác nhận giảm / Hủy món",
                    html: html,
                    showClass: {
                        popup: 'popup-formcreate'
                    },
                    footer: "<button class='swal2-cancel swal2-styled btn-cancel mr-3'><i class='icon-cd icon-add_cart icon'></i>Hủy bỏ</button><button class='swal2-styled btn btn-success btn-save'><i class='icon-cd icon-doneAll icon'></i>Đồng ý</button>",
                    allowOutsideClick: true,
                    showConfirmButton: false,
                    showCancelButton: false,
                    cancelButtonText: '<i class="fa fa-window-close"></i> Đóng',
                    didRender: () => {
                        $(".form-confirmremoveitem .item_action i:first-child").click(function () {

                            let slafter = parseFloat($(this).parent().children(".quantitynew").data("slhuy"));
                            if (slafter >= 0 && slafter < 1) {
                                slafter = parseDecimal(slafter - 0.1);
                            } else {
                                slafter = parseDecimal(slafter - 1);
                            }

                            if (slafter <= 0) {
                                toastr.error("Số lượng hủy/giảm còn ít nhất là 0");
                                return false;
                            } else {
                                $(this).parent().children(".quantitynew").data("slhuy", slafter);
                                $(this).parent().children(".quantitynew").html(slafter);
                            }

                        });
                        $(".form-confirmremoveitem .item_action i:last-child").click(function () {
                            let slafter = parseFloat($(this).parent().children(".quantitynew").data("slhuy"));

                            if (slafter >= parseFloat(slnotify)) {
                                toastr.error("Sô lượng hủy/giảm không vượt quá số lượng gốc");
                                return false;
                            }
                            else {
                                if (slafter >= 0 && slafter < 1) {
                                    slafter = parseDecimal(slafter + 0.1);
                                } else {
                                    slafter = parseDecimal(slafter + 1);
                                }
                                $(this).parent().children(".quantitynew").data("slhuy", slafter);
                                $(this).parent().children(".quantitynew").html(slafter);
                            }
                        });


                        $(".btn-cancel").click(function () {
                            $(sel).val(slgoc);
                            Swal.close();
                        });
                        $(".btn-save").click(function () {
                            let slafter = parseFloat($(".form-confirmremoveitem .item_action").children(".quantitynew").data("slhuy"));
                            let noteminus = $(".form-confirmremoveitem").find("#noteminus").val();
                            var dataObject = {
                                Note: noteminus,
                                IsCancel: true,
                                IdOrderItem: IdOrderItem,
                                CusCode: cusocde,
                                IdCustomer: IdCustomer,
                                QuantityFloat: (slafter * -1).toString(),
                                TypeUpdate: _TypeUpdatePos.UpdateQuantity,
                                IdRoomAndTableGuid: idTable != "-1" ? idTable : "",
                                IsBringBack: idTable == "-1" ? true : false,
                                IdGuid: $("#ul-tab-order").find("a.active").data("id")
                            };// giảm số lượng
                            loadeventPos.orderTable(dataObject);
                            Swal.close();
                        });

                    }
                });

            }
            else {

                let sladdthem = parseFloat((parseFloat(slnew) - parseFloat(slgoc)).toFixed(3));
                if (sladdthem != 0) {
                    var dataObject = {
                        IdOrderItem: IdOrderItem,
                        CusCode: cusocde,
                        IdCustomer: IdCustomer,
                        QuantityFloat: sladdthem.toString(),
                        TypeUpdate: _TypeUpdatePos.UpdateQuantity,
                        IdRoomAndTableGuid: idTable != "-1" ? idTable : "",
                        IsBringBack: idTable == "-1" ? true : false,
                        IdGuid: $("#ul-tab-order").find("a.active").data("id")
                    };// giảm số lượng

                    loadeventPos.orderTable(dataObject);

                } else if (sladdthem == 0) {

                } else {
                    toastrcus.warning("Số lượng không hợp lệ");
                }

            }

            //  loadeventPos.loadactiveClickItemMon($(this));

        });// dùng để giảm món
        $("#item-mon  li .item_action").find("i:last-child").click(function () {
            let IdOrderItem = $(this).parent().parent("li").data("id");
            let idTable = $(".btn-showttable").data("id");
            let cusocde = $(".search-customer").val();
            let IdCustomer = "";
            if (cusocde.trim() != "") {
                cusocde = cusocde.split(" ")[0];
                IdCustomer = $(".search-customer").data("id");
            }
            var dataObject = {
                IdOrderItem: IdOrderItem,
                CusCode: cusocde,
                IdCustomer: IdCustomer,
                QuantityFloat: 1,
                TypeUpdate: _TypeUpdatePos.UpdateQuantity,
                IdRoomAndTableGuid: idTable != "-1" ? idTable : "",
                IsBringBack: idTable == "-1" ? true : false,
                IdGuid: $("#ul-tab-order").find("a.active").data("id")
            };// giảm số lượng
            loadeventPos.orderTable(dataObject);

        });
        $("#item-mon  li .btn-remove").unbind();
        $("#item-mon  li .btn-remove").click(function () {
            let IdOrderItem = $(this).parent("li").data("id");
            let Quantity = $(this).data("idquan");
            let idTable = $(".btn-showttable").data("id");

            let slgoc = $(this).parent("li").data("sl");
            let slnotify = $(this).parent("li").data("slnotify");

            if (parseFloat(slnotify) > 0) {
                let namepro = $(this).parents("li").find(".name").find("b").html();
                let html = `
                            <div class="form-confirmremoveitem">
                                <span>Bạn có chắc chắn muốn hủy món <b>`+ namepro + `</b> không?</span>
                                <div class="slminus">
                                    <span>Số lượng hủy/giảm</span>
                                    <div class="item_action"><i class="fas fa-minus"></i><span data-slhuy="`+ slnotify + `" data-goc="` + slgoc + `" data-slnotify="` + slnotify + `" class="quantitynew">` + slgoc + `</span><span class="quantityold">/` + slgoc + `</span> <i class="fas fa-plus"></i></div>
                                </div>
                                <div class="input-note">
                                    <span class="text">Lý do: </span>
                                     <input type="text" id="noteminus" class="form-control"/>
                                </div>
                            </div>
                            `;
                Swal.fire({
                    // icon: 'success',
                    title: "Xác nhận giảm / Hủy món",
                    html: html,
                    showClass: {
                        popup: 'popup-formcreate'
                    },
                    footer: "<button class='swal2-cancel swal2-styled btn-cancel mr-3'><i class='icon-cd icon-add_cart icon'></i>Hủy bỏ</button><button class='swal2-styled btn btn-success btn-save'><i class='icon-cd icon-doneAll icon'></i>Đồng ý</button>",
                    allowOutsideClick: true,
                    showConfirmButton: false,
                    showCancelButton: false,
                    cancelButtonText: '<i class="fa fa-window-close"></i> Đóng',
                    didRender: () => {
                        $('.form-confirmremoveitem select').prepend('<option selected></option>').select2({

                            placeholder: {
                                id: '', // the value of the option
                                text: "Chọn bàn"
                            },
                            allowClear: true,
                            language: {
                                noResults: function () {
                                    return "Không tìm thấy dữ liệu";
                                }
                            },
                        })
                        $(".form-confirmremoveitem .item_action i:first-child").click(function () {

                            let slafter = parseFloat($(this).parent().children(".quantitynew").data("slhuy"));
                            if (slafter >= 0 && slafter < 1) {
                                slafter = parseDecimal(slafter - 0.1);
                            } else {
                                slafter = parseDecimal(slafter - 1);
                            }
                            if (slafter <= 0) {
                                toastr.error("Sô lượng hủy/giảm phải lớn hơn 0");
                                return false;
                            } else {
                                $(this).parent().children(".quantitynew").data("slhuy", slafter);
                                $(this).parent().children(".quantitynew").html(slafter);
                            }

                        });
                        $(".form-confirmremoveitem .item_action i:last-child").click(function () {
                            let slafter = parseFloat($(this).parent().children(".quantitynew").data("slhuy"));
                            if (slafter >= parseFloat(slnotify)) {
                                toastr.error("Sô lượng hủy/giảm không vượt quá số lượng gốc");
                                return false;
                            }
                            else {
                                if (slafter >= 0 && slafter < 1) {
                                    slafter = parseDecimal(slafter + 0.1);
                                } else {
                                    slafter = parseDecimal(slafter + 1);
                                }

                                $(this).parent().children(".quantitynew").data("slhuy", slafter);
                                $(this).parent().children(".quantitynew").html(slafter);
                            }
                        });


                        $(".btn-cancel").click(function () {
                            Swal.close();
                        });
                        $(".btn-save").click(function () {

                            let slgoc = parseFloat($(".form-confirmremoveitem .item_action").children(".quantitynew").data("slgoc"));
                            let slafter = parseFloat($(".form-confirmremoveitem .item_action").children(".quantitynew").data("slhuy"));
                            let slnotify = parseFloat($(".form-confirmremoveitem .item_action").children(".quantitynew").data("slnotify"));
                            let noteminus = $(".form-confirmremoveitem").find("#noteminus").val();
                            let slnotNotify = slgoc - slnotify;

                            let type = _TypeUpdatePos.UpdateQuantity;
                            let IsCancel = false;
                            if (slafter == slgoc) {
                                type = _TypeUpdatePos.RemoveRowItem;
                            }
                            if (slnotNotify != slafter) {
                                IsCancel = true;
                            }
                            var dataObject = {
                                Note: noteminus,
                                IsCancel: IsCancel,
                                IdOrderItem: IdOrderItem,
                                QuantityFloat: (slafter * -1),
                                TypeUpdate: type,
                                IdRoomAndTableGuid: idTable != "-1" ? idTable : "",
                                IsBringBack: idTable == "-1" ? true : false,
                                IdGuid: $("#ul-tab-order").find("a.active").data("id")
                            };// giảm số lượng

                            loadeventPos.orderTable(dataObject);
                            Swal.close();
                        });

                    }
                });

            } else {
                var dataObject = {
                    QuantityFloat: Quantity,
                    IdOrderItem: IdOrderItem,
                    TypeUpdate: _TypeUpdatePos.RemoveRowItem,
                    IdRoomAndTableGuid: idTable != "-1" ? idTable : "",
                    IsBringBack: idTable == "-1" ? true : false,
                    IdGuid: $("#ul-tab-order").find("a.active").data("id")
                };// giảm số lượng
                loadeventPos.orderTable(dataObject);
            }
        });

        $("#item-mon li").unbind();
        loadeventPos.loadeventclickitemmon();
    },// sự kiện giảm đi số lượng món
    loadeventselectautocompleteproduct: function (idProduct) {
        let cusocde = $(".search-customer").val();
        if (cusocde.trim() != "") {
            cusocde = cusocde.split(" ")[0];
        }
        let idTable = $(".btn-showttable").data("id");
        var dataObject = {
            CusCode: cusocde,// chỉ ở đây mới cập nhật khách vì là mới order món đầu tiên
            QuantityFloat: 1,
            TypeUpdate: _TypeUpdatePos.AddProduct,
            IdRoomAndTableGuid: idTable != "-1" ? idTable : "",
            IsBringBack: idTable == "-1" ? true : false,
            IdGuid: $("#ul-tab-order").find("a.active").data("id"),
            IdProduct: idProduct
        };
        loadeventPos.orderTable(dataObject);//lưu đơn
    },
    loadClickItemProduct: function () { // kích vào menu thực đơn chọn sản phẩm

        $("#lst-product li").click(function () {
            //$("#lst-product li").on("click", function (event) {
            let cusocde = $(".search-customer").val();
            if (cusocde.trim() != "") {
                cusocde = cusocde.split(" ")[0];
            }
            let idTable = $(".btn-showttable").data("id");
            var dataObject = {
                CusCode: cusocde,// chỉ ở đây mới cập nhật khách vì là mới order món đầu tiên
                QuantityFloat: 1,
                TypeUpdate: _TypeUpdatePos.AddProduct,
                IdRoomAndTableGuid: idTable != "-1" ? idTable : "",
                IsBringBack: idTable == "-1" ? true : false,
                IdGuid: $("#ul-tab-order").find("a.active").data("id"),
                IdProduct: $(this).data("id")
            };
            loadeventPos.orderTable(dataObject);//lưu đơn

        });

    },// kích vào menu thực đơn chọn sản phẩm
    loadAmoutAndQuantity: function (amount = null, quantity = null, idorder = "") {

        if (amount != null && quantity != null) {
            $(".fullamount").html(amount);
            $(".quantitySum").html(quantity);
        } else {
            amount = 0;
            quantity = 0;
            if (idorder != "") {

                $(".tab-content-order").find("div[data-id=" + idorder + "]").find("ul").find("li").map(function (item, index) {

                    amount += parseFloat($(this).find(".amount").find(".priceFormat").html().replaceAll(",", ""));
                    quantity += parseFloat($(this).find(".quantity").val().replaceAll(",", ""));
                });
            } else {
                $(".tab-content-order").find(".tab-pane.active").find("ul").find("li").map(function (item, index) {
                    amount += parseFloat($(this).find(".amount").find(".priceFormat").html().replaceAll(",", ""));
                    quantity += parseFloat($(this).find(".quantity").val().replaceAll(",", ""));
                });
            }

            $(".fullamount").html(amount);
            $(".quantitySum").html(quantity);
        }
        priceFormat();
    },//load toongrquantity và tiền:
    loadAutocomplete: function (isProduct = true, isCustomer = true) { //sự kiện autocomple sản phẩm và khách hàng
        if (isProduct) {

            _classPosEvent.input_searchProduct.blur(function () {
                $("#parentautocomplete").css({
                    height: "0",
                    top: "0",
                    "z-index": "0"
                });
            });

            _classPosEvent.input_searchProduct.autocomplete(
                {
                    appendTo: "#parentautocomplete",
                    autoFocus: true,
                    minLength: 0,
                    delay: 0,

                    source: function (request, response) {

                        $.ajax({
                            global: false,
                            url: "/Selling/Pos/SearchProductPos",
                            type: "GET",
                            dataType: "json",
                            data: {
                                text: request.term,
                                iSsell: true
                            },
                            // html: true,
                            success: function (data) {
                                response($.map(data, function (item) {
                                    let texthigh = __highlight(item.name, request.term);
                                    var htmltonkho = "<span>Tồn kho: <b>" + parseFloat(item.quantity).format0VND(3, 3, '') + "</b></span>";
                                    if (item.isInventory || item.typeProductCategory == EnumTypeProductCategory.COMBO || item.typeProductCategory == EnumTypeProductCategory.SERVICE) {
                                        htmltonkho = "<span>Tồn kho: <b>--</b></span>";
                                    }
                                    var html =
                                        "<a href='javascript:void(0)'><div class='search-auto'>" +
                                        "<div class='img'><img src='../" + item.img + "'></div>" +
                                        "<div class='tk_name'><span>" + texthigh + " (" + item.code + ")</span><span class='price'>" + parseFloat(item.price).format0VND(3, 3, '') + "</span>"
                                        + htmltonkho + "</div></div></a>";
                                    return {
                                        // label: html, value: item.code + " " + item.name, idProduct: item.id
                                        label: html, value: item.code, idProduct: item.id
                                    };
                                }))
                                return { label: request.term, value: request.term };
                            },
                        })

                    },
                    html: true,
                    select: function (e, ui) {
                        // console.log(ui);
                        $(this).val(ui.item.value);
                        $(this).select();
                        _varIdGuidSelectproductAutocomplete = ui.item.idProduct;
                        loadeventPos.loadeventselectautocompleteproduct(_varIdGuidSelectproductAutocomplete);
                    },
                }).focus(function () {
                    $("#parentautocomplete").css({
                        height: "calc(100vh - 100px)",
                        top: "52px",
                        "z-index": "5"
                    });
                    $(this).autocomplete("search");
                    $(this).select()
                });
            _classPosEvent.input_searchProduct.keypress(function (event) {
                let _dt = $(this).val();
                if (event.keyCode == 13 && _dt.length > 0) {
                    $("#lst-product li").map(function () {
                        let idDt = $(this).data("id");
                        if (idDt == _varIdGuidSelectproductAutocomplete) {
                            $(this).trigger("click");
                        }
                    });
                } else if (_dt.length == 0) {
                    _varIdGuidSelectproductAutocomplete = "";
                }
            });
        }
        if (isCustomer) {
            _classPosEvent.input_searchCustomer.blur(function () {
                $("#parentautocomplete").css({
                    height: "0",
                    top: "0",
                    "z-index": "0"
                });
            });
            _classPosEvent.input_searchCustomer.autocomplete(
                {
                    appendTo: "#parentautocomplete",
                    autoFocus: true,
                    minLength: 0,
                    delay: 0,

                    source: function (request, response) {
                        $.ajax({
                            global: false,
                            url: "/Selling/Customer/SearchCustomerPos",
                            type: "GET",
                            dataType: "json",
                            data: {
                                text: request.term,
                            },
                            // html: true,
                            success: function (data) {
                                response($.map(data, function (item) {
                                    let texthigh = __highlight(item.name, request.term);
                                    let html =
                                        "<a href='javascript:void(0)'><div class='search-auto'>" +
                                        "<div class='img'><img src='../" + item.img + "'></div>" +
                                        "<div class='tk_name'><span>" + texthigh + "</span><span class='code'>Mã KH: " + item.code + " <br/> SĐT: " + item.phoneNumber + "</div></div></a>";
                                    return {
                                        label: html, value: item.code + " " + item.name, idCus: item.id
                                    };
                                }))
                                return { label: request.term, value: request.term };
                            },
                        })

                    },
                    html: true,
                    select: async function (e, ui) {


                        // console.log(ui);
                        $(this).val(ui.item.value);
                        $(this).attr("data-id", ui.item.idCus);
                        // $(this).select();
                        $(this).parents(".box-data-customer").find(".btn-cleardata").show();
                        $(this).parents(".box-data-customer").find(".btn-addCus").hide();
                        $(this).parents(".box-data-customer").find("input").attr("readonly", "readonly");
                        $(this).parents(".box-data-customer").addClass("readonly");
                        // xử lý nếu đơn đã có mà chọn khách thay đổi thì update db
                        let checkOrder = $("#ul-tab-order li a.active").data("id");
                        if ($('#item-mon').length > 0 && typeof checkOrder != "undefined") {
                            await loadeventPos.updateCustomerOrder(ui.item.value);
                        }
                    },
                }).focus(function () {
                    $("#parentautocomplete").css({
                        height: "calc(100vh - 100px)",
                        top: "52px",
                        "z-index": "5"
                    });

                    $(this).autocomplete("search");
                    $(this).select()
                });

        }



    },//sự kiện autocomple sản phẩm và khách hàng, và select chọn sản phẩm
    updateCustomerOrder: async function (value) {
        cusocde = "";
        let idTable = $("#ul-tab-order").find("a.active").data("id");
        if (value != "") {
            cusocde = value.split(" ")[0];
        }
        let iDorder = $("#ul-tab-order a.active").data("id");

        const response = await axios({
            method: 'post',
            url: '/Selling/OrderTable/UpdateCustomOrderTable',
            headers: {
                'Content-Type': `multipart/form-data`,
            },
            data: {
                TypeUpdate: _TypeUpdatePos.ChangedCustomer,
                CusCode: cusocde,
                IdGuid: idTable
            }
        });
        if (response.data.isValid) {

            let findId = ListCusByOrderPos.find(x => x.idOrder == iDorder);
            let cus = $(".search-customer").val();
            if (typeof findId == "undefined" && cus != "") {
                let arrCus = {};
                arrCus.idOrder = iDorder;
                arrCus.customerCode = cus.split(" ")[0];
                arrCus.customerName = response.data.data;
                ListCusByOrderPos.push(arrCus);
            }
            else if (findId && cus == "") {
                let index = ListCusByOrderPos.findIndex(x => x.idOrder == iDorder);
                ListCusByOrderPos.splice(index, 1);
            }
        }

    },// update lại khách hàng khi chọn hoặc xóa 
    loadEventClose: function () {
        $(".box-data-customer").find(".btn-cleardata").unbind();
        $(".box-data-customer").find(".btn-cleardata").click(async function () {
            $(this).parents(".box-data-customer").find(".btn-cleardata").hide();
            $(this).parents(".box-data-customer").find(".btn-addCus").show();
            $(this).parents(".box-data-customer").find("input").removeAttr("readonly");
            $(this).parents(".box-data-customer").removeClass("readonly");
            $(this).parents(".box-data-customer").find("input").val("");
            $(this).parents(".box-data-customer").find("input").focus();
            // xử lý nếu đơn đã có mà xóa khách thay đổi thì update db
            let checkOrder = $("#ul-tab-order li a.active").data("id");
            if ($('#item-mon').length > 0 && typeof checkOrder != "undefined") {
                await loadeventPos.updateCustomerOrder("");
            }
            $(this).parents(".box-data-customer").find("input").removeAttr("data-id");
        });
    },// sự  kiện icon xóa khách hàng trong ô nhập để chọn lại khách mới
    loadEventAddCustomer: function () {
        $(".btn-addCus").click(function () {
            eventCreate.addOrEditCustomer('/Selling/Customer/Create?IsPos=true');
        });
    },
    loadEventkeyCode: function () {// sự kiện nhấn phím
        document.onkeyup = KeyCheck;
        function KeyCheck(e) {
            var KeyID = (window.event) ? event.keyCode : e.keyCode;

            if (KeyID == 113) {//f2
                _classPosEvent.input_searchProduct.focus();
            }
            if (KeyID == 115) {
                _classPosEvent.input_searchCustomer.focus();
            }
            if (KeyID == 81) {//ctrl + q thah toán
                if ($(".btn-payment").length > 0) {
                    $(".btn-payment").trigger("click");
                }
            }
            if (KeyID == 66) {//ctrl + b 
                if ($(".btn-barcode").length > 0) {

                    if ($(".btn-barcode").hasClass("active")) {
                        localStorage.setItem('Barcode', 0)
                        $(".btn-barcode").removeClass("active")
                        toastrcus.warning("Hủy chế độ quét mã QRcode/Barcode");
                        //$(_classPosEvent.input_searchProduct).autocomplete("enable");

                        //if ($(_classPosEvent.input_searchProduct).val() != "") {
                        //    $(_classPosEvent.input_searchProduct).autocomplete("search");
                        //    _classPosEvent.input_searchProduct.select();
                        //}
                        //else if (_classPosEvent.input_searchProduct.is(":focus")) {
                        //    $(_classPosEvent.input_searchProduct).autocomplete("search");
                        //} 
                        eventBanle.enableOrDisnableSancanbarCode(false);
                    } else {
                        localStorage.setItem('Barcode', 1)
                        $(".btn-barcode").addClass("active")
                        toastrcus.success("Bật chế độ quét mã QRcode/Barcode");
                        //$(_classPosEvent.input_searchProduct).autocomplete("close");
                        //$(_classPosEvent.input_searchProduct).autocomplete("disable");

                        //if (!_classPosEvent.input_searchProduct.is(":focus")) {
                        //    _classPosEvent.input_searchProduct.focus();
                        //} 
                        //if ($(_classPosEvent.input_searchProduct).val() != "") {
                        //    _classPosEvent.input_searchProduct.select();
                        //}
                        eventBanle.enableOrDisnableSancanbarCode(true);
                    }
                }
            }

            if (KeyID == 119) {
                $(".btn-payment").trigger("click");
                if ($('.discountamount').length > 0) {
                    $(".discountamount").focus()
                    $(".discountamount").select()
                }

            } if (KeyID == 120) {//f9
                $(".cussendamount").focus()
                $(".cussendamount").select()
            }


        }
    },// sự kiện nhấn phím
    loadDatatable: async function (sel) {
        sel.addClass("active");
        id = $(sel).data("id");
        await loadeventPos.loadOrderByTable(id);// phải load dữ liệu trước
        let text = $(sel).find("b").text();
        $(".btn-showttable").attr("data-id", id);// hiển thị cái bàn hiện tại
        $(".btn-showttable").find(".showTableOrder").html(text); // hiển thị cái bàn hiện tại

        $(".search-customer").focus(function () {
            $(this).parent(".input-group").addClass("focus-input");
        });
        $(".search-customer").blur(function () {
            $(this).parent(".input-group").removeClass("focus-input");
        });
    },
    loadClickTableroom: function () { // tự load sau khi loadpage và sự kiện click vào bàn lưu lại giá trị đó
        //check xem có lưu localStorage có lưu table k để active

        $("#lst-roomandtable li").click(async function () {

            if (!$(this).hasClass("active")) {// chỉ cho kích 1 lần trên 1 table
                $("#lst-roomandtable").find("li.active").not($(this)).removeClass("active");
                $(this).addClass("active");
                let id = $(this).data("id");
                await loadeventPos.loadOrderByTable(id);// phải load dữ liệu trước
                let text = $(this).find("b").text();
                $(".btn-showttable").attr("data-id", id);// hiển thị cái bàn hiện tại
                $(".btn-showttable").find(".showTableOrder").html(text); // hiển thị cái bàn hiện tại

                saveTableActive($(this).index());
                $(".search-customer").focus(function () {
                    $(this).parent(".input-group").addClass("focus-input");
                });
                $(".search-customer").blur(function () {
                    $(this).parent(".input-group").removeClass("focus-input");
                });
            }

            let getshowMenuInSelectTable = localStorage.getItem("showMenuInSelectTable");


            if (typeof getshowMenuInSelectTable != "undefined") {
                if (parseInt(getshowMenuInSelectTable) == 1) {
                    $(".ele-tab-leftRoomMenu .tab-menuOrder a").trigger("click");
                    localStorage.setItem("menuOrderShow", 1);
                }
            }

            // load dữ liệu order

        });
        loadTableActive();
        function saveTableActive(_cl) { // lưu lại giá trị mới localStorage
            if (typeof (Storage) !== "undefined") {
                // Store
                localStorage.setItem("activeTable", _cl); // tham số active table
                // Retrieve
            } else {
                alert("Liên hệ đội ngủ hỗ  trợ lỗi trình duyệt của bạn không hỗ  trợ Storage");
            }
        }
        function loadTableActive() { // load active

            if (typeof (Storage) !== "undefined") {
                // Store
                var _getActivetable = localStorage.getItem("activeTable");
                if (!isNaN(parseInt(_getActivetable))) {
                    _getActivetable = parseInt(_getActivetable) + 1;
                    if ($("#lst-roomandtable li").length < _getActivetable) {
                        _getActivetable = 1;
                    }
                    // $("#lst-roomandtable li:nth-child(" + _getActivetable + ")").trigger('click'); // là số thì active cái đó
                    let ele = $("#lst-roomandtable li:nth-child(" + _getActivetable + ")"); // là số thì active cái đó
                    loadeventPos.loadDatatable(ele);

                } else {
                    // let ele = $("#lst-roomandtable li:nth-child(1)").trigger("click");// k thì phải active cái đầu
                    let ele = $("#lst-roomandtable li:nth-child(1)");// k thì phải active cái đầu
                    loadeventPos.loadDatatable(ele);
                }
                // Retrieve
            } else {
                alert("Liên hệ đội ngủ hỗ  trợ lỗi trình duyệt của bạn không hỗ  trợ Storage");
            }
        }

    },// sự kiện click vào bàn lưu lại giá trị đó
    loadEventActiveTabMenuLeft: function () {
        let menuOrderShow = localStorage.getItem("menuOrderShow");

        if (menuOrderShow == "1") {
            $(".ele-tab-leftRoomMenu li.tab-menuOrder a").trigger("click");
        }

        $(".ele-tab-leftRoomMenu li").click(function () {
            if ($(this).hasClass("tab-menuOrder")) {
                localStorage.setItem("menuOrderShow", 1);
            } else {
                localStorage.setItem("menuOrderShow", 0);
            }
        });

    },
    loadClicktabOrder: function () {
        $("#ul-tab-order li a").unbind();
        $("#ul-tab-order li a").click(function () {

            let idorder = $(this).data("id");
            let findNote = ListNoteOrder.find(x => x.idOrder == idorder);
            if (typeof findNote != "undefined") {
                loadeventPos.eventUpdateClassNote(true);
            } else {
                loadeventPos.eventUpdateClassNote(false);
            }

            let getItem = ListCusByOrderPos.find(x => x.idOrder == idorder);
            let JSeach = $(".search-customer");
            if (typeof getItem != "undefined") {
                JSeach.val(getItem.customerCode + " " + getItem.customerName);
                JSeach.parents(".box-data-customer").find(".btn-cleardata").show();
                JSeach.parents(".box-data-customer").find(".btn-addCus").hide();
                JSeach.parents(".box-data-customer").find("input").attr("readonly", "readonly");
                JSeach.parents(".box-data-customer").addClass("readonly");
            } else {
                // $(".box-data-customer").find(".btn-cleardata").trigger('click');

                JSeach.parents(".box-data-customer").find(".btn-cleardata").hide();
                JSeach.parents(".box-data-customer").find(".btn-addCus").show();
                JSeach.parents(".box-data-customer").find("input").removeAttr("readonly");
                JSeach.parents(".box-data-customer").removeClass("readonly");
                JSeach.parents(".box-data-customer").find("input").val("");
            }

            if (typeof idorder == "undefined") {
                loadeventPos.loadAmoutAndQuantity(0, 0);
            } else {
                loadeventPos.loadAmoutAndQuantity(null, null, idorder);
            }

            loadeventPos.eventCheckBtnNotifyOrder($(this));

        });
    }, // sự kiện click tab
    loadOrderByTable: async function (idTable) {
        var loadOrder = await axios.get("/Selling/OrderTable/LoadDataOrder?idtable=" + idTable);
        if (loadOrder.data.isValid) {

            $("#container-tableOder").html(loadOrder.data.data);
            loadeventPos.eventUpdatedataItemMonOrder();
            if (loadOrder.data.dataCus.length > 0) {
                ListCusByOrderPos = loadOrder.data.dataCus;
            }

            if (loadOrder.data.dataNote.length > 0) {
                ListNoteOrder = loadOrder.data.dataNote;
            }
            if (loadOrder.data.active) {
                $("#lst-roomandtable li.active").addClass("CurentOrder");
            }
            numberFormat();
        } else {
            return false;
        }
        loadeventPos.loadEventAddCustomer();// load sự kiện thêm khách hàng
        _classPosEvent = {
            input_searchProduct: $(".search-product"),
            input_searchCustomer: $(".search-customer"),

        }

        loadeventPos.loadeventChangetableOrder();
        loadeventPos.loadEventClickIconAddAndMinus();
        loadeventPos.loadEventkeyCode();
        loadeventPos.loadAddTabOrder();
        loadeventPos.loadAutocomplete(false, true);
        loadeventPos.loadEventClose();

        loadeventPos.loadClicktabOrder();
        $('#ul-tab-order a:first').trigger('click');
        loadeventPos.loadAmoutAndQuantity();// load tong tien
        loadEventadmin.evnetFullscreen();// sự kiện full màn hình
        // $(".btnfullScreen").trigger("click");
        // loadeventPos.loadCustomerByOrder($('#ul-tab-order a:first').data("id"));// load khasch hang

    }, // sự kiện load dữ liệu order  của 1 bàn bất kỳ
    loadAddTabOrder: function () { // sự kiện  tạo và xóa đơn của bàn 
        $('.add-tab').click(function (e) {
            // e.preventDefault();
            let isCreate = true;
            $("#ul-tab-order").find("li").not($(this)).map(function (el, index) {
                let getTitle = $(this).data("title");
                if (getTitle == "newtab") {
                    isCreate = false;
                }
            });
            if (!isCreate) {
                return false;
            }
            if ($("#ul-tab-order").find("li").not($(this)).length > 6) {
                toastr.warning("Mỗi bàn tối đa 6 đơn chưa thanh toán, vui lòng thanh toán bớt để order thêm!");
                return false;
            }
            let _class = makeid(5);
            var nextTab = $('#ul-tab-order li.nav-item').length + 1;
            if ($('#ul-tab-order li.nav-item').length == 0) {
                $(this).parents("ul#ul-tab-order").find("li").before('<li class="nav-item"><a class="nav-link" href="#tab' + nextTab + '" data-toggle="tab"><span class="orderCode">OD-New</span> <span class="btn-removeTab ' + _class + '"><i class="fas fa-times"></i></span></a></li>');
            } else {
                $(this).parents("ul#ul-tab-order").find("li:nth-child(" + (nextTab - 1) + ")").after('<li class="nav-item"><a class="nav-link" href="#tab' + nextTab + '" data-toggle="tab"><span class="orderCode">OD-New</span> <span class="btn-removeTab ' + _class + '"><i class="fas fa-times"></i></span></a></li>');
            }

            let html = `<div class="tab-pane" id="tab` + nextTab + `" role="tabpanel">
                             <div class="tab-pane " id="tab-order-0" role="tabpanel">
                                <div class="no-order">
                                    <img src="../images/ristorante_old.png" />
                                    <b>Chưa có món nào được chọn</b>
                                    <span>Vui lòng chọn món trong thực đơn</span>
                                </div>
                            </div>
                        </div>`;
            $(html).appendTo('.tab-content-order');
            loadeventPos.loadClicktabOrder();
            $('#ul-tab-order a:last').trigger('click');
            $('#ul-tab-order a:last').parent("li").data("title", "newtab");
            loadeventPos.loadEventRemoveTaborder("." + _class);
        });
        loadeventPos.loadEventRemoveTaborder();
    },// sự kiện  tạo và xóa đơn của bàn 
    loadeventShowactionmobi: function () {
        $(".btn-moreaction-pos").click(function () {
            html = "<div class='showindex'></div>";
            $(".action-left-footer").after(html);
            $(".action-left-footer").toggleClass("show");
            if (!$(".action-left-footer").hasClass("show")) {
                $(".showindex").remove();
            }
            $(".showindex").click(function () {
                $(".action-left-footer").toggleClass("show");
                $(".showindex").remove();
            });
        });
    },//sự kiến tgasch bàn ở mobi
    loadEventRemoveTaborder: function (sel = "") { // sự kiện xóa đơn của bàn
        if (sel == "") {
            sel = $("#ul-tab-order .btn-removeTab");
        }

        $(sel).click(function () {

            //$(this).parents('.nav-item').remove();
            if ($(sel).parent().parent().data("title") == "newtab") {
                $(this).parents('.nav-item').remove();
                $('.tab-content-order > div.active').remove();

                if ($('#ul-tab-order').find(".nav-item").length == 0) { //not($(this).parents(".nav-item"))
                    $('#ul-tab-order').find(".add-tab").trigger('click');
                } else {
                    $('#ul-tab-order a:first').trigger('click');
                }
                loadeventPos.checkHighlightTableInOrder();// check highlight
            } else {
                loadeventPos.removeOder($(this).parents('.nav-item'));
            }


            // $("#ul-tab-order li.nav-item").first().trigger("click");
        });


    },
    CheckOutOrder: async function () {
        let typeSelectDiscount = parseInt($("#discountPayment").data("type"));//phần chiết khấu
        let valuediscount = parseInt($("#discountPayment").data("value"));
        let discount = 0;
        if (typeSelectDiscount == TypeSelectDiscount.Percent) {
            discount = valuediscount;
        }
        let idorder = $("#dataTablePayment").data("id");
        let discountPayment = $("#discountPayment").val().replaceAll(",", "");
        let cuspayAmount = $(".cuspay").val().replaceAll(",", "") || 0;
        let idpayment = $('input[name=idPaymentMethod]:checked', '.paymentMethod').data("id") || 0;
        var htmlPrint = "";
        var vatrate = $("#Vatrate").val();
        var VATAmount = $(".VATAmount").text().replaceAll(",", "") || 0;
        var Amount = $(".amountPayment").text().replaceAll(",", "") || 0;
        let isVAT = false;
        if (localStorage.getItem("VATMTT") == "true") {
            isVAT = true;
        }
        //var isValid = false;

        $.ajax({
            type: 'POST',
            async: true,
            url: '/Selling/OrderTable/CheckOutOrder',
            data: {
                TypeUpdate: _TypeUpdatePos.CheckOutOrder,
                IdOrder: idorder,
                VATAmount: VATAmount,
                Amount: Amount,
                discountPayment: discountPayment,
                discount: discount,
                cuspayAmount: cuspayAmount,//tiền khách đưa
                Idpayment: idpayment,
                vat: isVAT,
                Vatrate: vatrate,
                ManagerPatternEInvoices: $("#ManagerPatternEInvoices").val(),
            },

            success: async function (res) {
                isValid = res.isValid;
                if (res.isValid) {
                    Swal.close();
                    htmlPrint = res.data;
                    let sel = $("#lst-roomandtable li.active");
                    await loadeventPos.loadOrderByTable(sel.data("id"));
                    let text = $(sel).find("b").text();
                    $(".btn-showttable").attr("data-id", sel.data("id"));// hiển thị cái bàn hiện tại
                    $(".btn-showttable").find(".showTableOrder").html(text); // hiển thị cái bàn hiện tại
                    loadeventPos.checkHighlightTableInOrder();// check highlight

                    if (htmlPrint != "") {
                        printDiv(htmlPrint);
                        //dataObject = {};
                        //dataObject.type = TypeEventWebSocket.PrintInvoice;
                        //dataObject.html = htmlPrint;
                        //loadingStart();
                        //sposvietplugin.sendConnectSocket(listport[0]).then(function (data) {
                        //    console.log(data);
                        //    sposvietplugin.connectSignatureWebSocket(listport[0], JSON.stringify(dataObject)).then(function (data) {
                        //        loadingStop();
                        //        if (data == "-1") {
                        //            toastrcus.error("Có lỗi xảy ra");
                        //        }
                        //    });
                        //});
                    }
                }
            },
            error: function (err) {
                console.log(err)
            }
        });
        //if (isValid) {

        //}

    },
    eventChangeCheckbocVAT: function () {
        $.ajax({
            type: "GET",
            global: false,
            url: "/Selling/ConfigSaleParameters/GetConfigSell?key=" + getObjectKey(EnumConfigParameters, EnumConfigParameters.AUTOVATINPAYMENT),
            success: function (res) {

                if (res.isValid) {

                    if (res.data.typeValue == EnumTypeValue.BOOL && res.data.value == "false") {
                        if ($(".actionpaymentMTTpos").length > 0) {
                            $(".actionpaymentMTTpos").removeClass("d-none");
                        }
                    } else {
                        if ($(".actionpaymentMTTpos").length > 0) {
                            $(".actionpaymentMTTpos").remove();
                        }

                        localStorage.setItem("VATMTT", "true");

                    }
                } else {
                    if ($(".actionpaymentMTTpos").length > 0) {
                        $(".actionpaymentMTTpos").removeClass("d-none");
                    }

                }
            },
            error: function () {
                alert("Error occured!!");
            }
        });

        $(".paymentvatmtt").click(function () {
            let vat = $(this).is(':checked');
            localStorage.setItem("VATMTT", vat);
        });
        if (typeof localStorage.getItem("VATMTT") != "undefined") {
            //$(".paymentvatmtt").attr('checked', localStorage.getItem("VATMTT"))

            if (localStorage.getItem("VATMTT") == "true") {
                $(".paymentvatmtt").attr('checked', 'checked');
            }

        }
    },
    eventLoadCongThucTien: function () {

        let totalPayment = $(".totalPayment").text().replaceAll(",", "") || 0;
        let discountPayment = $("#discountPayment").val().replaceAll(",", "") || 0;
        let totalsaudiscount = (parseFloat(totalPayment) - parseFloat(discountPayment)) || 0;
        let vatamount = 0;
        if (!$(".ele-vatrate").hasClass("d-none")) {

            vatamount = Math.round(totalsaudiscount * (parseFloat($("#Vatrate").val() || 0) / 100));

            $(".VATAmount").html(vatamount.format0VND(0, 3, ""));
            $("#dataTablePayment").data("isVat", 1);
        } else {
            $("#dataTablePayment").data("isVat", 0);

        }
        let khachtra = totalsaudiscount + vatamount;
        $(".amountPayment").html(khachtra.format0VND(0, 3, ""));
        let cuspay = $(".cuspay").val().replaceAll(",", "");
        if ($(".cuspay").data("select") == 1) {
            cuspay = parseFloat(cuspay.replaceAll(",", ""));
            $(".amoutchange").html((cuspay - khachtra).format0VND(0, 3, ""));
        } else {
            $(".cuspay").val(khachtra.format0VND(0, 3, ""));
            $(".amoutchange").html(0);
        }

    },
    eventprint: function () {
        $(".btn-printbill").click(function () {
            let vat = false;
            if (localStorage.getItem("VATMTT") == "true") {
                vat = true;
            }
            let idOder = $("#ul-tab-order > li").find("a.active").data("id");
            if (vat) {
                let html = `<select id="Vatrate" name="Vatrate" class="form-control Vatrate">
                                        <option value="0">0%</option>
                                        <option value="5">5%</option>
                                        <option value="8">8%</option>
                                        <option value="10" selected>10%</option>
                                    </select>`;
                Swal.fire({
                    icon: 'warning',
                    title: 'Lựa chọn thuế suất?',
                    html: html,
                    // showDenyButton: true,
                    showCancelButton: true,
                    confirmButtonText: '<i class="fas fa-print"></i> Tiếp tục in',
                    cancelButtonText: '<i class="fas fa-power-off"></i> Hủy bỏ không in nữa',
                    // denyButtonText: `Don't save`,
                }).then((result) => {
                    /* Read more about isConfirmed, isDenied below */
                    if (result.isConfirmed) {
                        let vatrate = parseInt($("#Vatrate").val()) || -1;
                        $.ajax({
                            type: 'GET',

                            async: true,
                            url: "/Selling/OrderTable/PrintOrderTale",
                            data: {
                                id: idOder,
                                vat: vat,
                                Vatrate: vatrate,

                            },
                            success: function (res) {
                                if (res.isValid) {
                                    printDiv(res.html);


                                    //dataObject = {};
                                    //dataObject.type = TypeEventWebSocket.PrintInvoice;
                                    //dataObject.html = res.html;
                                    //loadingStart();
                                    //sposvietplugin.sendConnectSocket(listport[0]).then(function (data) {
                                    //    console.log(data);
                                    //    sposvietplugin.connectSignatureWebSocket(listport[0], JSON.stringify(dataObject)).then(function (data) {
                                    //        loadingStop();
                                    //        if (data == "-1") {
                                    //            toastrcus.error("Có lỗi xảy ra");
                                    //        }
                                    //    });
                                    //});

                                }
                            },
                            error: function (err) {
                                console.log(err)
                            }
                        });
                    }
                })
            } else {
                let vatrate = -1;
                $.ajax({
                    type: 'GET',
                    async: true,
                    url: "/Selling/OrderTable/PrintOrderTale",
                    data: {
                        id: idOder,
                        vat: vat,
                        Vatrate: vatrate,
                    },
                    success: function (res) {
                        if (res.isValid) {
                            printDiv(res.html);


                            //dataObject = {};
                            //dataObject.type = TypeEventWebSocket.PrintInvoice;
                            //dataObject.html = res.html;
                            //loadingStart();
                            //sposvietplugin.sendConnectSocket(listport[0]).then(function (data) {
                            //    console.log(data);
                            //    sposvietplugin.connectSignatureWebSocket(listport[0], JSON.stringify(dataObject)).then(function (data) {
                            //        loadingStop();
                            //        if (data == "-1") {
                            //            toastrcus.error("Có lỗi xảy ra");
                            //        }
                            //    });
                            //});

                        }
                    },
                    error: function (err) {
                        console.log(err)
                    }
                });
            }

        })

    },//in tạm tính
    eventPayment: function () {
        //$(".btn-notif").click(function () {
        //    loadeventPos.NotifyChitken();
        //});
        // thông báo cho bếp
        $(".btn-payment").click(function () {
            let idOder = $("#ul-tab-order > li").find("a.active").data("id");
            // let vat = $(".paymentvatmtt").is(':checked');
            let vat = false;
            if (localStorage.getItem("VATMTT") == "true") {
                vat = true;
            }
            $.ajax({
                type: 'GET',
                //global: false,
                url: "/Selling/OrderTable/Payment",
                data: {
                    TypeUpdate: _TypeUpdatePos.Payment,
                    vat: vat,
                    IdOrder: idOder
                },

                success: function (res) {
                    if (res.isValid) {
                        Swal.fire({
                            // icon: 'success',
                            title: res.title,
                            html: res.data,
                            showClass: {
                                popup: 'popup-formcreate popupform-payment'
                            },
                            onOpen: () => Swal.getConfirmButton().focus(),
                            footer: "<button class='btn btn-primary btn-continue mr-3'><i class='icon-cd icon-add_cart icon'></i>Hủy bỏ</button><button class='btn btn-save btn-success'><i class='icon-cd icon-doneAll icon'></i>Thanh toán</button>",
                            allowOutsideClick: true,
                            showConfirmButton: false,
                            showCancelButton: false,
                            cancelButtonText: '<i class="fa fa-window-close"></i> Đóng',
                            didOpen: () => {
                                Swal.getHtmlContainer().querySelector('.cuspay').select()
                            },
                            didRender: () => {
                                document.onkeyup = KeyCheckpayment;
                                function KeyCheckpayment(e) {
                                    var KeyID = (window.event) ? event.keyCode : e.keyCode;
                                    if (KeyID == 13) {
                                        loadeventPos.CheckOutOrder();
                                    }
                                }

                                $('select#ManagerPatternEInvoices').on('change', function () {
                                    var optionsText = this.options[this.selectedIndex].text;
                                    type = parseInt(optionsText.split('/')[0]);
                                    if (type == TypeEInvoice.GTGT) {
                                        $("#dataTablePayment").data("isVat", 1);
                                        $(".table-info-pay tr.ele-vatrate").removeClass("d-none");
                                    } else {
                                        $("#dataTablePayment").data("isVat", 0);
                                        if (!$(".table-info-pay tr.ele-vatrate").hasClass("d-none")) {
                                            $(".table-info-pay tr.ele-vatrate").addClass("d-none");
                                        }
                                    }
                                    loadeventPos.eventLoadCongThucTien();//load lại công thức
                                });

                                $('select#Vatrate').on('change', function () {
                                    loadeventPos.eventLoadCongThucTien();//load lại công thức
                                });
                                $('input.icheckpayment').iCheck({
                                    checkboxClass: 'icheckbox_square-green',
                                    radioClass: 'iradio_square-green',
                                    increaseArea: '20%' // optional
                                });
                                //numberFormat();
                                priceFormat();
                                $('.number3').each(function () {
                                    let idtex = $(this).text().replaceAll(",", ".");
                                    $(this).html(parseFloat(idtex).format0VND(0, 3, ""))
                                });
                                $("#dataTablePayment").data("id", res.idOrder);
                                var amoutn = 0;
                                //$("#discountPayment").keyup(function () {
                                //    loadeventPos.eventLoadCongThucTien();
                                //});

                                $("#discountPayment").click(function () {

                                    let html = `<div id="popupselectDiscount">
                                                   <div class="overlazy"></div>
                                                   <div class="ele-discount" id="ousSizeOutPopup">
                                                        <span> Giảm giá theo: </span>
                                                        <div class="content">
                                                            <div class="form-check-inline">
                                                              <label class="form-check-label mr-3">
                                                                <input type="radio" class="form-check-input" id="radio1" name="optradiocheckboxdis" value="2" checked> Số tiền (VND)
                                                              </label>
                                                              </div>
                                                              <div class="form-check-inline">
                                                              <label class="form-check-label">
                                                                <input type="radio" class="form-check-input" id="radio2" name="optradiocheckboxdis" value="1"> Phần trăm (%)
                                                              </label>
                                                            </div>
                                                            <div class="text">
                                                                <input class="data-value form-control number3" id="discounttypevalue" placeholder="Nhập số tiền" />
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>`;
                                    $("#discountPayment").after(html);
                                    evetnFormatnumber3();
                                    loadeventPos.showPopupSelectDiscount();
                                });

                                $(".overlazy").click();

                                if (amoutn == 0) {
                                    amoutn = parseInt($(".amountPayment").html().replaceAll(",", "")) || 0;
                                }
                                // lựa chọn số tiền khách đưa
                                $(".lst-amount-paycus span").click(function () {
                                    let _value = parseInt($(this).data("value"));
                                    $(".cuspay").val(_value.format0VND(0, 3, ""));
                                    $(".cuspay").data("select", 1);
                                    $(".cuspay").trigger("keyup");
                                });
                                //
                                $(".cuspay").keyup(function () {
                                    amoutn = parseInt($(".amountPayment").html().replaceAll(",", ""));
                                    let _vl = parseInt($(this).val().replaceAll(",", "")) || 0;
                                    cuspayamount = _vl;
                                    let tienthua = _vl - amoutn;


                                    $(".amoutchange").html(tienthua.format0VND(0, 3, ""));
                                    //if (_vl > amoutn) {
                                    //    $(".amoutchange").html(tienthua.format0(0, 3, ""));
                                    //} else {
                                    //    $(".amoutchange").html(0);
                                    //}
                                });
                                loadeventPos.eventLoadCongThucTien();//load lại công thức
                                $(".btn-continue").click(function () {
                                    Swal.close();
                                });
                                $(".btn-save").click(function () {
                                    loadeventPos.CheckOutOrder();
                                }); // thanh toán hóa đơn


                            }
                        });
                    }
                },
                error: function (err) {
                    toastr.error(err);
                    console.log(err)
                }
            });

        });
    },
    showPopupSelectDiscount: function () {

        let typeSelectDiscount = TypeSelectDiscount.Cash;
        let Cashkeup = 0;
        let Percentkeup = 0;

        if (typeof $("#discountPayment").data("type") != "undefined") {
            typeSelectDiscount = parseInt($("#discountPayment").data("type"));
            value = parseInt($("#discountPayment").data("value"));
            $("input:radio[value=" + typeSelectDiscount + "][name='optradiocheckboxdis']").prop('checked', true);
            $("#discounttypevalue").val(value);
            if (typeSelectDiscount == TypeSelectDiscount.Cash) {
                Cashkeup = value;
            }
            else if (typeSelectDiscount == TypeSelectDiscount.Percent) {
                Percentkeup = value;
            }
        }


        $('input:radio[name=optradiocheckboxdis]').change(function () {
            let gettype = $("input[name='optradiocheckboxdis']:checked").val();
            if (gettype == TypeSelectDiscount.Cash) {
                typeSelectDiscount = TypeSelectDiscount.Cash;
                $("#discounttypevalue").val(Cashkeup.format0VND(3, 3));
            }
            else if (gettype == TypeSelectDiscount.Percent) {
                typeSelectDiscount = TypeSelectDiscount.Percent;
                $("#discounttypevalue").val(Percentkeup.format0VND(3, 3));
            }
            loadCongthuc();
            $("#discounttypevalue").select();
            setTimeout(() => {
                $("#discounttypevalue").select();
            }, 150);
        });
        $("#discounttypevalue").keyup(function () {
            loadCongthuc();
        });
        function loadCongthuc() {
            if (typeSelectDiscount == TypeSelectDiscount.Cash) {
                let discountPayment = parseFloat($("#discounttypevalue").val().replaceAll(",", ""));
                Cashkeup = discountPayment;//lưu để update lại số cũ
                $("#discountPayment").val(discountPayment.format0VND(3, 3));
                $("#discountPayment").data("value", discountPayment);
            }
            else if (typeSelectDiscount == TypeSelectDiscount.Percent) {
                let totalPayment = parseFloat($(".totalPayment").text().replaceAll(",", ""));
                let discountPayment = parseFloat($("#discounttypevalue").val().replaceAll(",", ""));
                let _discountamount = Math.round(parseFloat(totalPayment * (discountPayment / 100)));
                Percentkeup = discountPayment;//lưu để update lại số cũ
                $("#discountPayment").val(_discountamount.format0VND(3, 3));

                $("#discountPayment").data("value", discountPayment);// gán giá trị
            }
            $("#discountPayment").data("type", typeSelectDiscount);
            loadeventPos.eventLoadCongThucTien();
        }

        $(".overlazy").on('click', function (e) {
            if ($(e.target).closest("#ousSizeOutPopup").length === 0) {
                $("#popupselectDiscount").remove();

            }
        });

        setTimeout(() => {
            $("#discounttypevalue").select();
        }, 200);
    },
    NotifyChitken: function () {
        let idOrder = $("#ul-tab-order a.active").data("id");
        $(".btn-notif").attr("disabled", "disabled");
        $.ajax({
            type: 'POST',
            traditional: true,
            async: false,
            url: '/Selling/OrderTable/NotifyKitChen',
            data: {
                IdOrder: idOrder,
            },
            success: function (res) {
                if (res.isValid) {
                    loadeventPos.eventUpdateQuantityItemMonOrder();
                    loadeventPos.eventinbaobep(res.html);
                } else {
                    $(".btn-notif").removeAttr("disabled");
                }
            },
            error: function (err) {
                console.log(err)
            }
        });
    },
    eventinbaobep: function (html) {
        dataObject = {};
                    dataObject.type = TypeEventWebSocket.PrintBep;
        dataObject.html = html;
                   // loadingStart();
                    sposvietplugin.sendConnectSocket(listport[0]).then(function (data) {
                        console.log(data);
                        sposvietplugin.connectSignatureWebSocket(listport[0], JSON.stringify(dataObject)).then(function (data) {
                            //loadingStop();
                            if (data == "-1") {
                                toastrcus.error("Có lỗi xảy ra");
                            }
                        });
                    });
    },
    eventShowHistory: function () {
        $(".btn-history").click(async function () {
            let idorder = $("#ul-tab-order a.active").data("id");
            const ipAPI = '/Selling/OrderTable/GetHistoryOrder?IdOrder=' + idorder;
            jQueryModalGetRightToLeft(ipAPI, "Lịch sử gọi món (Đã thông báo bếp)");
            //var loadhis = await axios.get(ipAPI);
            //if (loadhis.data.isValid) {

            //}

        });
    },
    eventSplitOrder: function () {
        $(".btn-splitOder").click(async function () {
            let checkInorder = $(".tab-content-order .tab-pane.active").find("#item-mon").length;
            if (checkInorder == 0) {
                toastr.error("Bàn chưa có món nào không thể tách đơn")
                return false;
            }
            let idorder = $("#ul-tab-order a.active").data("id");
            let idtable = $(".btn-showttable").data("id");
            if (idtable = !1) {
                idtable = true;
            } else {
                idtable = false;
            }
            lstRom = [];
            $("#lst-roomandtable li.CurentOrder").map(function () {
                let room = {};
                room.id = $(this).data("id");
                room.text = $(this).find("b").html();
                lstRom.push(room);
            })
            const ipAPI = '/Selling/OrderTable/SplitOrder?TypeUpdate=' + _TypeUpdatePos.SplitOrder + "&IdOrder=" + idorder + "&IsBringBack=" + idtable;
            var loadOrder = await axios.get(ipAPI);
            await Swal.fire({
                // icon: 'success',
                // title: res.title,
                html: loadOrder.data,
                showClass: {
                    popup: 'popup-formcreate popupform-payment'
                },
                // onOpen: () => Swal.getConfirmButton().focus(),
                footer: "<button class='swal2-cancel swal2-styled btn-continue swal2-styled mr-3'><i class='icon-cd icon-add_cart icon'></i>Hủy bỏ</button><button class='swal2-confirm swal2-styled btn-save btn-success'><i class='icon-cd icon-doneAll icon'></i>Lưu</button>",
                allowOutsideClick: true,
                showConfirmButton: false,
                showCancelButton: false,
                closeOnConfirm: false,
                confirmButtonText: '<i class="fa fa-save"></i> Lưu',
                cancelButtonText: '<i class="fa fa-window-close"></i> Đóng',
                didOpen: () => {
                    //  Swal.getHtmlContainer().querySelector('.cuspay').select()
                },
                didRender: async () => {
                    //  evetnFormatTextnumber3()
                    let idtable = $("#lst-roomandtable li.active").data("id");
                    let IsBringBack = false;
                    if (idtable == "-1") {
                        IsBringBack = true;
                    }

                    // tạo list table tất cả
                    lstRomAll = [];
                    $("#lst-roomandtable li").map(function () {
                        let room = {};
                        room.id = $(this).data("id");
                        room.text = $(this).find("b").html();
                        lstRomAll.push(room);
                    })
                    //
                    $('#lstroomselectTach').prepend('<option selected></option>').select2({
                        data: lstRomAll,
                        placeholder: {
                            id: '', // the value of the option
                            text: "Chọn bàn"
                        },
                        allowClear: true,
                        language: {
                            noResults: function () {
                                return "Không tìm thấy dữ liệu";
                            }
                        },
                    });
                    $('#lstroomselect').prepend('<option selected></option>').select2({
                        data: lstRom,
                        placeholder: {
                            id: '', // the value of the option
                            text: "Chọn bàn"
                        },
                        allowClear: true,
                        language: {
                            noResults: function () {
                                return "Không tìm thấy dữ liệu";
                            }
                        },
                    }).on('change', async function (e) {
                        var _curen = $(this).val();
                        if (_curen == "-1") {
                            IsBringBack = true;
                        } else {
                            IsBringBack = false;
                        }
                        var loadtable = await axios.get('/Selling/OrderTable/LoadDataOrderByTableInSplit?idtable=' + _curen + "&IdOrder=" + idorder + "&Type=" + _TypeSpitOrderPos.Graft + "&IsBringBack=" + IsBringBack);
                        $(".tableOrderRoom").html(loadtable.data.html);
                        evetnFormatTextnumber3decimal()
                        $('.tableOrderRoom input.icheck').iCheck({
                            checkboxClass: 'icheckbox_square-green',
                            radioClass: 'iradio_square-green',
                            increaseArea: '20%' // optional
                        });
                        //this.value 
                    })

                    $('#flexCheckDefault').on('ifChanged', function (event) {
                        $(".tabtachdon").toggleClass("d-none");
                        $(".tabghepdon").toggleClass("d-none");

                    });
                    //lấy các item của dơn hiện tại load ra cho việc tách đơn
                    var loadtable = await axios.get('/Selling/OrderTable/LoadDataOrderByTableInSplit?idtable=' + idtable + "&IdOrder=" + idorder + "&Type=" + _TypeSpitOrderPos.Separate + "&IsBringBack=" + IsBringBack);
                    $(".table-tachdon").html(loadtable.data.html);

                    var listOrderNew = await axios.get('/Selling/OrderTable/GetOrderStatusNew?Status=' + 0 + '&NotOrder=' + idorder);

                    // $("#listOrderNew").html(loadtable.data.html);
                    $('#listOrderNew').prepend('<option value="0">Tạo đơn mới</option>').select2({
                        data: listOrderNew.data.json,
                        placeholder: {
                            id: '', // the value of the option
                            text: "Chọn đơn"
                        },
                        allowClear: true,
                        language: {
                            noResults: function () {
                                return "Không tìm thấy dữ liệu";
                            }
                        },
                    }).on('change', async function (e) {
                        var _curen = $(this).val();
                        if (_curen == "0") {
                            $(".elepaternlstroomselectTach").show();// 0 laf tạo đơn mới
                        } else {
                            $(".elepaternlstroomselectTach").hide();
                        }
                        //this.value 
                    })

                    evetnFormatTextnumber3decimal()
                    ///

                    // load sự kiện tách số lượng
                    loadeventPos.eventSpitQuntity()
                    //
                    $('.form-splitOrder input.icheck').iCheck({
                        checkboxClass: 'icheckbox_square-green',
                        radioClass: 'iradio_square-green',
                        increaseArea: '20%' // optional
                    });

                    $(".btn-continue").click(function () {
                        Swal.close();
                    });
                    $(".btn-save").click(function () {
                        lstdataOrderNew = [];
                        lstdataOrderOld = [];

                        let checktachorghep = $('input[name="orderActions"]:checked').val();// là tách hay ghép

                        if (checktachorghep == _TypeSpitOrderPos.Separate) { // là tách
                            let IsNewOrder = false;
                            let idtach = $("#listOrderNew").val();// tách đến đơn nào
                            if (idtach == "0") {
                                IsNewOrder = true;
                            }
                            let IsBringBack = false;
                            let idtablenew = $("#lstroomselectTach").val(); // nếu tạo mới thì chọn bàn nào
                            if (idtach == "") {
                                toastr.error("Vui lòng chọn bàn cần tách đến!")
                                return;
                            } else if (idtach == "0" && idtablenew == "") {
                                toastr.error("Vui lòng chọn bàn cần tách, khi tạo đơn mới!")
                                return;
                            }
                            if (idtablenew == "-1") {
                                IsBringBack = true;
                            }

                            $(".table-tachdon table tr").map(function () {
                                let quantityGoc = $(this).children("td.quantity").data("id");
                                let quantityNew = $(this).children("td.quantitySplit").find(".quantyti-new").data("id");

                                if (parseInt(quantityNew) > 0) {
                                    dataOrderNew = {};
                                    dataOrderNew.quantity = quantityNew;
                                    dataOrderNew.idOrderitem = $(this).data("id");
                                    lstdataOrderNew.push(dataOrderNew);
                                }
                                if (parseInt(quantityGoc) > 0) {
                                    dataOrderNewold = {};
                                    dataOrderNewold.quantity = quantityGoc;
                                    lstdataOrderOld.push(dataOrderNewold);
                                }
                            });

                            if (lstdataOrderNew.length == 0) {
                                toastr.error("Vui lòng chọn món cần tách!")
                                return;
                            }
                            if (lstdataOrderOld.length == 0) {
                                toastr.error("Đơn gốc còn lại có ít nhất một món hàng!")
                                return;
                            }
                            things = JSON.stringify(lstdataOrderNew);
                            $.ajax({
                                type: 'POST',
                                traditional: true,
                                // async: false,
                                url: '/Selling/OrderTable/SplitOrder',
                                data: {
                                    TypeUpdate: _TypeSpitOrderPos.Separate,
                                    IsNewOrder: IsNewOrder,
                                    IdOrderOld: idorder,
                                    IdOrderNew: idtach,
                                    IdTable: idtablenew,
                                    IsBringBack: IsBringBack,
                                    json: things,
                                },

                                success: function (res) {
                                    isValid = res.isValid;
                                    if (res.isValid) {
                                        Swal.close();
                                        let idTableCurent = $("#lst-roomandtable li.active").data("id");
                                        $("#lst-roomandtable li.active").removeClass("active");
                                        $("#lst-roomandtable li:not(.CurentOrder)").map(function () {
                                            if ($(this).data("id") == idtablenew) {
                                                $(this).addClass("CurentOrder");
                                            }
                                        });

                                        $("#lst-roomandtable li.CurentOrder").map(function () {
                                            if ($(this).data("id") == idTableCurent) {
                                                $(this).trigger("click");
                                            }
                                        });

                                    }
                                },
                                error: function (err) {
                                    console.log(err)
                                }
                            });

                            //  toastr.success("Đã ok!")
                        } else if (checktachorghep == _TypeSpitOrderPos.Graft) { // là ghép
                            let getlstOrderGhep = [];// lấy danh sách các đơn cần ghép vào nếu chọn nhiều sẽ ghép toàn bộ thành 1 đơn. 
                            let idtablenew = $("#lstroomselect").val(); // nếu tạo mới thì chọn bàn nào
                            if (idtablenew == "") {
                                toastr.error("Vui lòng chọn bàn cần ghép")
                                return;
                            }
                            $(".tableOrderRoom table tbody tr").map(function () {
                                if ($(this).find('input[name="orderName"]:checked').length > 0) {

                                    if (typeof $(this).data("id") != "undefined") {
                                        item = {};
                                        item.idOrder = $(this).data("id");
                                        getlstOrderGhep.push(item);
                                    }
                                }
                            });

                            if (getlstOrderGhep.length == 0) {
                                toastr.error("Vui lòng chọn đơn cần ghép!")
                                return;
                            }
                            things = JSON.stringify(getlstOrderGhep);
                            $.ajax({
                                type: 'POST',
                                traditional: true,
                                // async: false,
                                url: '/Selling/OrderTable/SplitOrder',
                                data: {
                                    TypeUpdate: _TypeSpitOrderPos.Graft,
                                    IdOrderOld: idorder,
                                    json: things,
                                },

                                success: function (res) {
                                    isValid = res.isValid;
                                    if (res.isValid) {
                                        Swal.close();

                                        $("#lst-roomandtable li.active").removeClass("CurentOrder");
                                        //$("#lst-roomandtable li.active").removeClass("active");
                                        // console.log(idorder);
                                        $("#lst-roomandtable li.CurentOrder").map(function () {

                                            if ($(this).data("id") == idtablenew) {
                                                $(this).trigger("click");
                                            }
                                        });
                                    }
                                },
                                error: function (err) {
                                    console.log(err)
                                }
                            });
                        }
                    });
                }
            })
        });
    },//sự kiện tách ghép bàn
    eventSpitQuntity: function () {
        $(".table-tachdon table tr .lst-action i:last-child").click(function () {// tăng số lượng mới
            let _tr = $(this).parents("tr");
            let quantityGocOld = _tr.children("td.quantity").data("old");
            let quantityGoc = _tr.children("td.quantity").data("id");
            let quantityNew = _tr.children("td.quantitySplit").find(".quantyti-new").data("id");
            if (parseInt(quantityGoc) == 0) {
                //
            } else {
                let _nquan = parseInt(quantityNew) + 1;
                quantityNew = _nquan;
                _tr.children("td.quantitySplit").find(".quantyti-new").data("id", _nquan);
                _tr.children("td.quantitySplit").find(".quantyti-new").html(_nquan);

                let _oldquan = parseInt(quantityGoc) - 1;
                quantityGoc = _oldquan;
                _tr.children("td.quantity").data("id", _oldquan);
                _tr.children("td.quantity").html(_oldquan);
            }
            if (parseInt(quantityGocOld) != parseInt(quantityGoc)) {
                _tr.addClass("change-quantity");
            } else {
                _tr.removeClass("change-quantity");
            }
        });
        $(".table-tachdon table tr .lst-action i:first-child").click(function () {
            let _tr = $(this).parents("tr");
            let quantityGocOld = _tr.children("td.quantity").data("old");
            let quantityGoc = _tr.children("td.quantity").data("id");
            let quantityNew = _tr.children("td.quantitySplit").find(".quantyti-new").data("id");
            if (parseInt(quantityNew) == 0) {
                //
            } else {
                let _nquan = parseInt(quantityNew) - 1;
                quantityNew = _nquan;
                _tr.children("td.quantitySplit").find(".quantyti-new").data("id", _nquan);
                _tr.children("td.quantitySplit").find(".quantyti-new").html(_nquan);
                let _oldquan = parseInt(quantityGoc) + 1;
                quantityGoc = _oldquan;
                _tr.children("td.quantity").data("id", _oldquan);
                _tr.children("td.quantity").html(_oldquan);
            }
            if (parseInt(quantityGocOld) != parseInt(quantityGoc)) {
                _tr.addClass("change-quantity");
            } else {
                _tr.removeClass("change-quantity");
            }
        });

    },// sự kiện load số lượng kích vào bàn
    eventloadNumberStatusTable: function () {
        let _iAll = 0;
        let _iActive = 0;
        let _iNo = 0;
        $('#lst-roomandtable li').map(function () {
            _iAll += 1;
            var element = $(this);
            if (element.hasClass("CurentOrder")) {
                _iActive += 1;
            } else if (element.not(".CurentOrder")) {
                _iNo += 1;
            }
        });

        $('.list-other-table input[name="optiontablename"]').map(function () {
            let value = $(this);
            switch (parseInt(value.val())) {
                case _TypeSelectTableRadio.All:
                    value.parents(".form-check").find(".value").html(_iAll);
                    break;
                case parseInt(_TypeSelectTableRadio.IsActive):
                    value.parents(".form-check").find(".value").html(_iActive);
                    break;
                case _TypeSelectTableRadio.No:
                    value.parents(".form-check").find(".value").html(_iNo);
                    break;
            }
        })
        $('.list-other-table input[name="optiontablename"]:checked').trigger("ifChecked")
    },
    eventLoadCheckradiotable: function () {


        $('.list-other-table input[name="optiontablename"]').on('ifChecked', function (event) {
            switch (parseInt(event.target.value)) {
                case _TypeSelectTableRadio.All:

                    $('#lst-roomandtable li').filter(function () {
                        var element = $(this);
                        element.css('display', "flex");
                    });
                    break;
                case parseInt(_TypeSelectTableRadio.IsActive):

                    $('#lst-roomandtable li').filter(function () {
                        var element = $(this);
                        if (element.hasClass("CurentOrder")) {
                            element.css('display', "flex");
                        } else {
                            element.css('display', "none");
                        }

                    });
                    break;
                case _TypeSelectTableRadio.No:
                    $('#lst-roomandtable li').filter(function () {
                        var element = $(this);
                        if (element.hasClass("CurentOrder")) {
                            element.css('display', "none");
                        } else {
                            element.css('display', "flex");
                        }
                    });
                    break;
                default:
                    console.log(_TypeSelectTableRadio.All)
                // code block
            }
        });

    },
    eventAddNoteOrder: function () {
        $(".btn-noteOder").click(function () {
            let checkInorder = $(".tab-content-order .tab-pane.active").find("#item-mon").length;
            if (checkInorder == 0) {
                toastr.error("Bàn chưa có món nào không thể tạo ghi chú")
                return false;
            }
            let idorder = $("#ul-tab-order a.active").data("id");
            let inputValue = "";
            let findId = ListNoteOrder.find(x => x.idOrder == idorder);
            if (typeof findId != "undefined") {
                inputValue = findId.note;
            }

            Swal.fire({
                title: 'Nhập ghi chú đơn',
                input: 'textarea',
                inputValue: inputValue,
                inputPlaceholder: 'Nhập ghi chú đơn',
                inputAttributes: {
                    autocapitalize: 'off'
                },
                showCancelButton: true,
                confirmButtonText: 'Lưu ghi chú',
                cancelButtonText: 'Đóng',
                showLoaderOnConfirm: true,
                inputValidator: (value) => {
                    let findId = ListNoteOrder.find(x => x.idOrder == idorder);
                    if (typeof findId == "undefined") {
                        if (!value) {
                            return 'Vui lòng nhập ghi chú!'
                        }
                    }
                },
                preConfirm: (note) => {

                    $.ajax({
                        type: 'POST',
                        //global: false,
                        url: '/Selling/OrderTable/AddNote',
                        data: {
                            TypeUpdate: _TypeUpdatePos.AddNoteOrder,
                            IdOrder: idorder,
                            Note: note,
                        },

                        success: function (res) {
                            if (res.isValid) {
                                let index = ListNoteOrder.findIndex(x => x.idOrder == idorder);
                                ListNoteOrder.splice(index, 1);
                                if (note == "") {

                                    loadeventPos.eventUpdateClassNote(false)
                                } else {
                                    let _nnote = {};
                                    _nnote.idOrder = idorder;
                                    _nnote.note = note;
                                    ListNoteOrder.push(_nnote);
                                    loadeventPos.eventUpdateClassNote(true)
                                }

                            }
                        },
                        error: function (err) {
                            console.log(err)
                        }
                    });
                },
                allowOutsideClick: () => !Swal.isLoading()
            })
        });

    },//sự kiến ghi chú đơn hàng
    eventUpdateClassNote: function (add = true) {
        if (add) {
            if (!$(".btn-noteOder").hasClass("curent-note")) {

                $(".btn-noteOder").addClass("curent-note");
            }
        } else {
            $(".btn-noteOder").removeClass("curent-note");
        }

    },// theemclass vào note khi có note
    removeOder: function (sel) {

        let iddt = $(sel).children("a").data("id");
        Swal.fire({
            icon: 'warning',
            title: 'Bạn có chắc chắn muốn xóa đơn không?',
            // showDenyButton: true,
            showCancelButton: true,
            confirmButtonText: 'Đồng ý',
            cancelButtonText: 'Đóng',
            // denyButtonText: `Don't save`,
        }).then((result) => {
            /* Read more about isConfirmed, isDenied below */
            if (result.isConfirmed) {
                $.ajax({
                    type: 'POST',
                    //global: false,
                    url: '/Selling/OrderTable/RemoveOder',
                    data: {
                        TypeUpdate: _TypeUpdatePos.RemoveOrder,
                        IdOrder: iddt
                    },

                    success: function (res) {
                        if (res.isValid) {
                            $(sel).remove();
                            $('.tab-content-order > div.active').remove();

                            if ($('#ul-tab-order').find(".nav-item").length == 0) { //not($(this).parents(".nav-item"))
                                $('#ul-tab-order').find(".add-tab").trigger('click');
                            } else {
                                $('#ul-tab-order a:first').trigger('click');
                            }
                            loadeventPos.checkHighlightTableInOrder();// check highlight
                            // xóa note trong list
                            let index = ListNoteOrder.findIndex(x => x.idOrder == iddt);
                            ListNoteOrder.splice(index, 1);
                        }
                    },
                    error: function (err) {
                        console.log(err)
                    }
                });
            }
        })






        //axios.post('/Selling/OrderTable/RemoveOder', )


        //const response = await axios({
        //    method: 'post',
        //    url: '/Selling/OrderTable/RemoveOder',
        //    headers: {
        //        'Content-Type': `multipart/form-data`,
        //    },
        //    data: {
        //        TypeUpdate: _TypeUpdatePos.RemoveOrder,
        //        IdOrder: iddt
        //    }
        //});

        //if (response.data.isValid) {
        //    $(sel).remove();
        //    $('.tab-content-order > div.active').remove();
        //    $('#ul-tab-order a:first').trigger('click');
        //    if ($('#ul-tab-order').find(".nav-item").length == 0) { //not($(this).parents(".nav-item"))
        //        $('#ul-tab-order').find(".add-tab").trigger('click');
        //    }
        //}
    },// sự kiện xóa order
    loadActiveTableInOrder: async function () {
        var loadOrder = await axios.get("/Selling/OrderTable/LoadCheckOrderInTable");
        //console.log(loadOrder);
        if (loadOrder.data.isValid) {
            $("#lst-roomandtable li").map(function (item, index) {
                let idData = $(this).data("id");
                if (idData == "-1") {
                    let getIN = loadOrder.data.data.find(x => x.isBringBack);
                    if (typeof getIN != "undefined" && getIN.isOrder) {
                        $(this).addClass("CurentOrder");
                    }
                } else {
                    let getIN = loadOrder.data.data.find(x => x.idtable == idData);
                    if (typeof getIN != "undefined" && getIN.isOrder) {
                        $(this).addClass("CurentOrder");
                    }
                }
            });
            loadeventPos.eventloadNumberStatusTable();
        }
    },// mới đầu load dữ liệu tô màu các bàn đang có khách
    loadDataAttributeProductAndTable: function () {
        $('#lst-product li').map(function () {
            let idata = $(this).data("id");
            $(this).removeAttr("data-id");
            $(this).data("id", idata);
        });
        $('#lst-roomandtable li').map(function () {
            let idata = $(this).data("id");
            $(this).removeAttr("data-id");
            $(this).data("id", idata);
        });
    },//load gán lại data id
    loadEventCheckbox: function () {
        let getshowMenuInSelectTable = localStorage.getItem("showMenuInSelectTable");
        if (typeof getshowMenuInSelectTable != "undefined") {
            if (parseInt(getshowMenuInSelectTable) == 1) {
                $('.showmenuClickroom input').iCheck("check");
            }
        }

        $('.showmenuClickroom input').on('ifChecked', function (event) {
            //if ($("input#CreatePart").is(':checked')) {
            //    alert('checked');
            //}
            localStorage.setItem("menuOrderShow", 1);
            if (typeof (Storage) !== "undefined") {
                // Store
                localStorage.setItem("showMenuInSelectTable", 1); //
                // Retrieve
            } else {
                alert("Liên hệ đội ngủ hỗ  trợ lỗi trình duyệt của bạn không hỗ  trợ Storage");
            }
        });

        $('.showmenuClickroom input').on('ifUnchecked', function (event) {
            //if ($("input#CreatePart").is(':checked')) {
            //    alert('un-checked');
            //}
            localStorage.setItem("menuOrderShow", 0);
            if (typeof (Storage) !== "undefined") {
                // Store
                localStorage.setItem("showMenuInSelectTable", 0); //
                // Retrieve
            } else {
                alert("Liên hệ đội ngủ hỗ  trợ lỗi trình duyệt của bạn không hỗ  trợ Storage");
            }
        });
    }
}
var loadEventadmin = {
    evnetFullscreen: function () {
        $(".btnfullScreen").click(function () {
            $(this).find("i").toggleClass("fa-compress");
            $(this).find("i").toggleClass("fa-expand");
            toggleFullScreen();
        });
    },
    eventShowformAddPostOne: function () {
        loadevent();
        var initEditor = function () {
            return CKEDITOR.replace('Decription', {});
        }
        loaddataSelect2CustomsTempalte("/Api/Handling/GetAllCategoryPost", "#IdCategory", $("#Category").val(), "Tất cả");
        $("#elfinder-input").change(function () {
            $("#Imgdata").attr("src", "/" + $(this).val());
        })
        //CKEDITOR.replace('Title');
        initEditor();
        $(".btn-continue").click(function () {
            Swal.close();
        });
        $(".btn-save").click(function () {

            jQueryModalPost($("form#PostOnePage")[0]);
        });
    },
    event_removerowsortable: function (sel) {
        $(sel).find(".fa-trash").click(function () {
            $(this).parents(".input-addcontent").remove();
            if ($("#items-input-addcontent").html().trim() == "") {
                $("input[id=IsAddingOptions]").iCheck('uncheck');
            }
            if ($("#items-input-addcontent").find(".input-addcontent").length < 2) {
                if ($(".btn-addoption").hasClass("disabled")) {
                    $(".btn-addoption").removeClass("disabled")
                }
            }
            loadEventadmin.eventGenerateDataPriceproduct($("#add-option-body"));
        });
    },
    event_loadajaxdataselect2: async function (sel, idselectd = "") {
        var gethtml = await axios.get('/API/Handling/GetDataJsonStypeProduct?idselectd=' + idselectd);
        $(sel).prepend('<option selected></option>').select2({
            data: gethtml.data,
            placeholder: {
                id: '', // the value of the option
                text: "Chọn giá trị"
            },
            allowClear: true,
            language: {
                noResults: function () {
                    return "Không tìm thấy dữ liệu";
                }
            },
        }).on('change', function (e) {
            var _curen = $(this);
            var checkvalid = false;
            $($(this).parents("#items-input-addcontent").find("select").not($(this))).each(function () {
                // alert(_val + "_" + this.value)
                if (_curen.val() == $(this).val()) {
                    checkvalid = true;
                    // $(_curen).addClass("isvalid");
                    return;
                }
            });
            if (!checkvalid) {
                _curen.parents(".input-addcontent").find("input").removeAttr("readonly").removeAttr("disabled");
                _curen.parents(".input-addcontent").find(".validate-select").remove();
                $(e.currentTarget).data().select2.$container.removeClass('isvalid');
            } else {
                if (_curen.parents(".input-addcontent").find(".validate-select").length > 0) {
                    _curen.parents(".input-addcontent").find(".validate-select").remove();
                }
                $(_curen).parent(".header-select").after("<div class='validate-select'><label class='isvalid'>Tên tùy chọn này đã được sử dụng</label></div>");
                _curen.parents(".input-addcontent").find("input").attr("readonly", "readonly").attr("disabled", "disabled");
                $(e.currentTarget).data().select2.$container.addClass('isvalid');
            }
            //this.value
        })
    },
    eventupdatesortableproduct: function (id) {
        $(id).sortable("destroy");
        //$('#items-input-addcontent .body-addoption').sortable({
        $(id).sortable({
            //scrollSensitivity: 2,
            handle: 'button.Sortable-body-addoption',
            // helper: 'clone',
            items: "div:not(.hidden-Sortable)",
            cancel: "textarea",
            axis: "y",
            zIndex: 9999,
            distance: 10,
            scrollSpeed: 0,
            cursor: "move",
            tolerance: "pointer",
            connectWith: ".m-card-sortable",
            cancel: '.tile-option-choose',
            start: function start(event, ui) {
                $(ui.item).addClass("active-sortable");
            },
            stop: function stop(event, ui) {
                $(ui.item).removeClass("active-sortable");
                //changeLocalCard();
            },
            change: function change(event, ui) { },
            update: function update(event, ui) {
                loadEventadmin.eventGenerateDataPriceproduct($("#add-option-body"));
            }
        });
    },
    eventaddstypeoptionproduct: function (input) {
        $(input.find("input")).keyup(function () {
            let isvalid = false;
            let sel = $(this);
            let _nclass = makeid(6);
            let htmlinput = `<div class="item-grid-sort hidden-Sortable ` + _nclass + `">
                                    <button type="button" class="Sortable-body-addoption" role="button"><svg viewBox="0 0 20 20" class="Polaris-Icon__Svg_375hu" focusable="false" aria-hidden="true"><path d="M7 2a2 2 0 1 0 .001 4.001 2 2 0 0 0-.001-4.001zm0 6a2 2 0 1 0 .001 4.001 2 2 0 0 0-.001-4.001zm0 6a2 2 0 1 0 .001 4.001 2 2 0 0 0-.001-4.001zm6-8a2 2 0 1 0-.001-4.001 2 2 0 0 0 .001 4.001zm0 2a2 2 0 1 0 .001 4.001 2 2 0 0 0-.001-4.001zm0 6a2 2 0 1 0 .001 4.001 2 2 0 0 0-.001-4.001z"></path></svg></button>
                                    <input type="text" class="form-control" name="" data-id="0" placeholder="Nhập giá trị" />
                                    <i class="fa fa-trash"></i>
                               </div>`;
            if ($(this).val().length > 0) {
                if (!sel.parents(".item-grid-sort").hasClass("data-success")) {
                    sel.parents(".item-grid-sort").addClass("data-success");
                    sel.parents(".item-grid-sort").find(".fa-trash").click(function () { // kích hoạt sự kiện cho nút xóa của dòng hiện tại là dòng cuối cùng
                        $(this).parents(".item-grid-sort.data-success").remove();
                        loadEventadmin.eventGenerateDataPriceproduct($("#add-option-body"));
                    });
                }
                if (sel.parents(".item-grid-sort").hasClass("hidden-Sortable")) {
                    sel.parents(".item-grid-sort").removeClass("hidden-Sortable");
                    loadEventadmin.eventupdatesortableproduct('#items-input-addcontent .body-addoption');
                }
                let i = 0;

                $($(this).parents(".body-addoption").find("input").not($(this))).each(function () {

                    if ($(this).val().length == 0) {
                        i += 1;
                    } else if ($(this).val() == sel.val()) {

                        isvalid = true;
                        return;
                    }
                    //else if (sel.hasClass("isvalid")) {
                    //    sel.removeClass("isvalid");
                    //    isvalid = false;
                    //}
                })
                if (!isvalid) {
                    $(this).parents(".body-addoption").find("span.isvalid").remove();
                    sel.removeClass("isvalid");
                } else {
                    sel.removeClass("is-valid");
                    sel.addClass("isvalid");
                    if ($(this).parents(".body-addoption").find("span.isvalid").length > 0) {
                        $(this).parents(".body-addoption").find("span.isvalid").remove();
                    }
                    sel.after("<span class='isvalid'>Tên đã bị trùng</span>");

                }
                if (i == 0) {
                    $(this).parents(".body-addoption").append(htmlinput);
                    loadEventadmin.eventaddstypeoptionproduct($("." + _nclass));
                }
            }
            else {
                sel.parents(".item-grid-sort").removeClass("data-success");
                if (!sel.parents(".item-grid-sort").hasClass("hidden-Sortable")) {
                    sel.parents(".item-grid-sort").addClass("hidden-Sortable");
                    loadEventadmin.eventupdatesortableproduct('#items-input-addcontent .body-addoption');
                }
                $($(this).parents(".body-addoption").find("input").not($(this))).each(function () {
                    if ($(this).val().length == 0) {
                        $(this).parents(".item-grid-sort").remove();
                    }
                })
            }

            loadEventadmin.eventGenerateDataPriceproduct($(this));
        });
        $("#add-option-body input").blur(function () {
            if ($(this).hasClass("isvalid")) {
                let newvl = $(this).val();
                $(this).parents(".body-addoption").find("span.isvalid").remove();
                if (newvl.substring(0, newvl.length - 1) == "") {
                    $(this).parents(".item-grid-sort").remove();
                } else {
                    $(this).val(newvl.substring(0, newvl.length - 1));
                    $(this).removeClass("isvalid");
                }
            }

        });

    },
    eventGenerateDataPriceproduct: function (sel) {
        // xử lý việc lấy dữ liệu từ các ô nhập của các cấp ở đây có thể có 2 cấp rồi sinh ra table 
        var _lstDataOne = []; // cấp 1
        var styleproduct1 = ""; // tên loại cấp 1
        var styleproduct2 = ""; // tên loại cấp 2
        var _lstDataTow = []; // cấp 2
        var _lstDatatables = [];
        var btn_rollback = false;
        $("#items-input-addcontent").find(".input-addcontent").each(function (index, element) {
            if (index == 0) {
                styleproduct1 = $(this).find("select").val();
                //console.log("valuestyle:" + valuestyle);
                $(this).find("input").each(function () {
                    var _style = {};
                    if (this.value.trim() != "") {
                        _style.name = this.value;
                        _lstDataOne.push(_style);
                    }
                });
            } else if (index == 1) {
                styleproduct2 = $(this).find("select").val();
                $(this).find("input").each(function () {
                    var _style = {};
                    if (this.value.trim() != "") {
                        _style.name = this.value;
                        _lstDataTow.push(_style);
                    }
                });
            }

        });
        if (styleproduct1 != "" && _lstDataOne.length > 0) {
            _lstDataOne.forEach(function (item) {

                var _value = item.name;
                var _code = styleproduct1 + "$" + removeVietnameseTones(item.name).replaceAll(" ", "-").toLowerCase();
                var _rowDatatable = {};
                if (_lstDataTow.length > 0 && styleproduct2 != "") {
                    _lstDataTow.forEach(function (item2) {
                        var _rowDatatable = {};
                        var _value2 = _value + "/" + item2.name;
                        var _code2 = _code + "_" + styleproduct2 + "$" + removeVietnameseTones(item2.name).replaceAll(" ", "-").toLowerCase();
                        _rowDatatable.name = _value2;
                        _rowDatatable.code = _code2;
                        _lstDatatables.push(_rowDatatable);
                    });
                } else {
                    _rowDatatable.name = _value;
                    _rowDatatable.code = _code;
                    _lstDatatables.push(_rowDatatable);
                }
            });

            if (_lstDatatables.length > 0) {
                let htmlbody = "";


                //let getitemreadData = ListArrNameStyleProductinDb;//lấy danh sách các item đã lưu trong db
                let lstArrayremoveDb = [];
                if (ListArrNameStyleProductinDb.length > 0) { // kiểm tra db trước có hay k ms xử lý tiếp
                    lstArrayremoveDb = ListArrNameStyleProductinDb.filter(function (item) {
                        let get = _lstDatatables.find(x => x.name == item.name);
                        if (typeof get === "undefined") {
                            return true;
                        }
                        return false;
                    });
                }
                _lstDatatables.forEach(function (item) {
                    let _classProductEdit = "";
                    let _htmlProductEdit = "";
                    //xử lý load table  _lstDatatables là table mới, ListArrNameStyleProduct là table cũ mục đích để check cái cũ có xóa thì giữ lại
                    var _val = item.name;
                    let className = "";
                    let Name = "";
                    let Code = "";
                    let Id = "0";
                    let Price = "";
                    let Quantity = "";
                    let lstbtn = `<button type="button" class="btn btn-sm btn-primary mr-2 btn-edit">Sửa</button>
                                  <button type="button" class="btn btn-sm btn-danger btn-delete">Xóa</button>`;
                    if (ListArrNameStyleProductinDb.length > 0) {
                        let getitemdb = ListArrNameStyleProductinDb.find(i => i.name.toLowerCase() == _val.toLowerCase());//tìm tên trong db
                        if (typeof getitemdb == "undefined") {
                            _htmlProductEdit = `<span class="newrowedit">Mới</span>`; // k có là mới phù hợp vs lúc edit sản phẩm
                            _classProductEdit = `newrowedit`; //k có là mới phù hợp vs lúc edit sản phẩm
                        }
                    }
                    if (ListArrNameStyleProduct.length > 0) {
                        let getitemindex = ListArrNameStyleProduct.find(i => i.Name.toLowerCase() == _val.toLowerCase()); //lấy item tương ứng trong arr cũ để cập nhập lại table mới,
                        if (typeof getitemindex !== "undefined") {
                            // ví vụ khi xóa site L của A mà L của B còn thì k dc xóa L A
                            className = getitemindex.ClassName;
                            Name = getitemindex.Name;
                            Code = getitemindex.Code;
                            Price = getitemindex.Price;
                            Id = getitemindex.Id;
                            Quantity = getitemindex.Quantity;
                            if (className.includes(_classremoteTr)) {
                                btn_rollback = true;
                                let _nclass = makeid(5); //sinh class
                                lstbtn = `<button type="button" class="btn btn-sm btn-danger btn-rollback ` + _nclass + `">Khôi phục</button>`;
                            }
                        }
                    }
                    if (className.includes(_classProductEdit)) {
                        _classProductEdit = "";
                    }

                    htmlbody += `
                                    <tr class="` + className + " " + _classProductEdit + `" data-id="` + Id + `">
                                      <td><input type="checkbox" class="icheck item-check" /></td>
                                      <td>
                                         <div class="imgstypedetailt"></div>
                                      </td>
                                      <td><span class="name">` + _val + "</span>" + _htmlProductEdit + `</td>
                                      <td><input type="text" ` + (className.includes(_classremoteTr) ? "disabled readonly" : "") + ` class="form-control code" value="` + Code + `" /></td>
                                      <td><input type="text" ` + (className.includes(_classremoteTr) ? "disabled readonly" : "") + ` class="form-control price priceFormat" value="` + Price + `"/></td>
                                      <td><input type="text" ` + (className.includes(_classremoteTr) ? "disabled readonly" : "") + ` class="form-control quantity priceFormat" value="` + Quantity + `"/></td>
                                      <td>
                                         <div class="lst-button">
                                           ` + lstbtn + `
                                         </div>
                                      </td>
                                   </tr>
                            `;
                });
                if (lstArrayremoveDb.length > 0) {
                    // dánh sách list đã bị remove
                    btn_rollback = true;
                    let _nclass = makeid(5); //sinh class
                    _lstbtnRemove = `<button type="button" class="btn btn-sm btn-danger btn-rollback ` + _nclass + `">Khôi phục</button>`;
                    lstArrayremoveDb.forEach(function (element, index) {

                        htmlbody += `
                                    <tr class="readData ` + _classremoteTr + `"  data-id="` + element.id + `">
                                      <td><input type="checkbox" disabled class="icheck item-check" /></td>
                                      <td>
                                         <div class="imgstypedetailt"></div>
                                      </td>
                                      <td><span class="name">` + element.name + `</td>
                                      <td><input type="text" disabled readonly class="form-control code" value="` + element.sku + `" /></td>
                                      <td><input type="text" disabled readonly class="form-control price priceFormat" value="` + element.price + `"/></td>
                                      <td><input type="text" disabled readonly class="form-control quantity priceFormat" value="` + element.quantity + `"/></td>
                                      <td>
                                         <div class="lst-button">
                                           ` + _lstbtnRemove + `
                                         </div>
                                      </td>
                                   </tr>`;
                    });
                }
                let html = `<div class="pt-3 pb-3 w-100" id="add-table-styleproduct" style= "background-color: #f3f3f6;" >
                            <div class="card" style="box-shadow:none">
                                <div class="card-header">
                                    <div>
                                        <h3 style="font-size: 18px;font-weight:bold" class="mb-0">Mẫu mã</h3>
                                    </div>
                                </div>
                                <div class="card-body">
                                    <table class="table table-bordered" id="tableStylePrice">
                                        <thead>
                                            <tr>
                                                <th><input type="checkbox" class="icheck" id="check-all" /></th>
                                                <th>Hình ảnh</th>
                                                <th>Mẫu mã</th>
                                                <th>Mã hàng</th>
                                                <th>Giá</th>
                                                <th style="width:100px">Số lượng</th>
                                                <th>Công cụ</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            ` + htmlbody + `
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                           </div >
    `;

                loadEventadmin.enventloadeventTablestyleproduct(sel, html, btn_rollback);
                //loadEventadmin.eventloadremovetablestyleproduct();

                //$(sel).parents(".option-add").after(html);
                //priceFormat();
                //if (btn_rollback) {//nếu có rollback thì mới chạy sự kiện
                //    $("table#tableStylePrice").find("tbody").find("tr").find(".btn-rollback").click(function (item) {
                //        // add sự kiện cho nút khôi phục đó
                //        let _nclass = makeid(5);
                //        htmllstbutton = `< button type = "button" class="btn btn-sm btn-primary mr-2 btn-edit" > Sửa</button >
                //                         <button type="button" class="btn btn-sm btn-danger btn-delete `+ _nclass + `">Xóa</button>`;
                //        $(this).parent(".lst-button").html(htmllstbutton);
                //        loadEventadmin.eventBtnRollbackItemtableStyleProduct($("." + _nclass));// add sự kiện cho nút xóa sửa lại sau khi update
                //    });
                //}

                //$('#add-table-styleproduct input.icheck').iCheck({
                //    checkboxClass: 'icheckbox_square-green',
                //    radioClass: 'iradio_square-green',
                //    increaseArea: '20%' // optional
                //});
                /////event delete
                //loadEventadmin.eventdeletebtnStyletableproduct($("#add-table-styleproduct"));
                //$('#tableStylePrice').DataTable({
                //    scrollY: "300px",
                //    scrollX: true,
                //    scrollCollapse: true,
                //    paging: false,
                //    searching: false,
                //    ordering: false, "info": false,
                //    fixedColumns: {
                //        left: 2
                //    }
                //});
                //loadEventadmin.eventloadIcheck();

            }
        }
    },
    eventBtnRollbackItemtableStyleProduct: function (item) {
        $(item).parent().parents("tr." + _classremoteTr).find("input").removeAttr("readonly");
        $(item).parent().parents("tr." + _classremoteTr).find("input").removeAttr("disabled");
        $(item).parents("tr").find(".item-check").iCheck('enable');
        $(item).parents("tr").removeClass(_classremoteTr);
        loadEventadmin.eventdeletebtnStyletableproduct($(item).parents("tr"));
        if (ListArrNameStyleProductinDb.length > 0) {
            if ($(item).parents("tr").hasClass(_classloadTrDb)) {
                // nếu là dg edit mà có dũ liệu thì roolback lại
                let getName = $(item).parents("tr").find(".name").text();
                let nameStyle1 = "";
                let nameStyle2 = "";
                if (getName.indexOf("/") != -1) {
                    // là có cấp 2
                    nameStyle1 = getName.split('/')[0];
                    nameStyle2 = getName.split('/')[1];
                } else {
                    nameStyle1 = getName;
                }

                $("#items-input-addcontent").find(".input-addcontent").each(function (index, element) {
                    if (index == 0) { // cấp 1
                        isValid = false;
                        $(this).find("input").each(function () {
                            if (this.value == nameStyle1) {
                                isValid = true;
                            }
                        });

                        if (!isValid) {
                            let _nclass = makeid(5);
                            htmlInput = `<div class="item-grid-sort data-success ` + _nclass + `" >
                                           <button type="button" class="Sortable-body-addoption" role="button"><svg viewBox="0 0 20 20" class="Polaris-Icon__Svg_375hu" focusable="false" aria-hidden="true"><path d="M7 2a2 2 0 1 0 .001 4.001 2 2 0 0 0-.001-4.001zm0 6a2 2 0 1 0 .001 4.001 2 2 0 0 0-.001-4.001zm0 6a2 2 0 1 0 .001 4.001 2 2 0 0 0-.001-4.001zm6-8a2 2 0 1 0-.001-4.001 2 2 0 0 0 .001 4.001zm0 2a2 2 0 1 0 .001 4.001 2 2 0 0 0-.001-4.001zm0 6a2 2 0 1 0 .001 4.001 2 2 0 0 0-.001-4.001z"></path></svg></button>
                                           <input type="text" class="form-control" name="" data-id="" placeholder="Nhập giá trị" value="` + nameStyle1 + `" />
                                           <i class="fa fa-trash"></i>
                                        </div>`;
                            $(this).find(".body-addoption").find("div.item-grid-sort.data-success").last().after(htmlInput);
                            loadEventadmin.eventaddstypeoptionproduct($("." + _nclass));
                            loadEventadmin.eventupdatesortableproduct($(this).find(".body-addoption"));
                            loadEventadmin.eventGetDataStyletable();
                            loadEventadmin.eventGenerateDataPriceproduct($("#add-option-body"));
                        }
                    }
                    else if (index == 1) {
                        // cấp 2
                        isValid = false;
                        $(this).find("input").each(function () {
                            if (this.value == nameStyle2) {
                                isValid = true;
                            }
                        });
                        if (!isValid) {
                            let _nclass = makeid(5);
                            htmlInput = `<div class="item-grid-sort data-success ` + _nclass + `" >
                                           <button type="button" class="Sortable-body-addoption" role="button"><svg viewBox="0 0 20 20" class="Polaris-Icon__Svg_375hu" focusable="false" aria-hidden="true"><path d="M7 2a2 2 0 1 0 .001 4.001 2 2 0 0 0-.001-4.001zm0 6a2 2 0 1 0 .001 4.001 2 2 0 0 0-.001-4.001zm0 6a2 2 0 1 0 .001 4.001 2 2 0 0 0-.001-4.001zm6-8a2 2 0 1 0-.001-4.001 2 2 0 0 0 .001 4.001zm0 2a2 2 0 1 0 .001 4.001 2 2 0 0 0-.001-4.001zm0 6a2 2 0 1 0 .001 4.001 2 2 0 0 0-.001-4.001z"></path></svg></button>
                                           <input type="text" class="form-control" name="" data-id="" placeholder="Nhập giá trị" value="` + nameStyle2 + `" />
                                           <i class="fa fa-trash"></i>
                                        </div>`;
                            $(this).find(".body-addoption").find("div.item-grid-sort.data-success").last().after(htmlInput);
                            loadEventadmin.eventaddstypeoptionproduct($("." + _nclass));
                            loadEventadmin.eventupdatesortableproduct($(this).find(".body-addoption"));
                            loadEventadmin.eventGetDataStyletable();
                            loadEventadmin.eventGenerateDataPriceproduct($("#add-option-body"));
                        }
                    }
                });
            }
        }
    },
    loadEventBtnRollBack: function (sel) {

        let _nclass = makeid(5); //sinh class
        var htmllstbutton = `<button type="button" class="btn btn-sm btn-danger btn-rollback ` + _nclass + `">Khôi phục</button>`;
        $(sel).parent(".lst-button").html(htmllstbutton); //đổi thành nút khôi phục
        // $("table#tableStylePrice").find("tbody").find("tr").find("." + _nclass).click(function (item) {
        $("." + _nclass).click(function (item) {
            // add sự kiện cho nút khôi phục đó

            _nclass = makeid(5);
            htmllstbutton = `<button type="button" class="btn btn-sm btn-primary mr-2 btn-edit">Sửa</button>
                                         <button type="button" class="btn btn-sm btn-danger btn-delete ` + _nclass + `">Xóa</button>`;
            $(this).parent(".lst-button").html(htmllstbutton);
            loadEventadmin.eventBtnRollbackItemtableStyleProduct($("." + _nclass)); // add sự kiện cho nút xóa sửa lại
        });
    },
    eventdisabledRow: function (sel) {
        $(sel).parents("tr").addClass(_classremoteTr); //add class
        $(sel).parents("tr").find("input").attr("readonly", "readonly").attr("disabled", "disabled"); //add attr
        $(sel).parents("tr").find(".item-check").iCheck('uncheck'); //update uncheck
        $(sel).parents("tr").find(".item-check").iCheck('disable'); //add disable
    },
    eventdeletebtnStyletableproduct: function (sel) {
        $(sel).find(".btn-delete").click(function () {
            var _this = $(this).parents("tr");
            var _name = $(this).parents("tr").find(".name").html().trim(); //lấy tên trong tr đó có name
            if (_name.indexOf("/") == -1) { //xem là 1 cấp hay 2 cấp,-1 là 1 cấp
                $(this).parents("tr").remove(); // 1 cấp remove luôn
                $("#items-input-addcontent").find(".body-addoption").find("input").each(function (index, element) {
                    if (this.value == _name) {
                        $(this).parents(".item-grid-sort").remove();
                        return;
                    }
                });
            }
            else {
                var arr = _name.split("/"); // 2 cấp lấy tên sau dấu /
                var isvalidtow = false; //cấp 2
                var isvalidone = false; //cấp 1
                // lấy các tên còn lại của các dòng k phải dòng hiện tại để kiểm tra
                $(this).parents("table#tableStylePrice").find("tbody").find("tr").not($(this).parents("tr")).not("tr.remove").each(function (index, ele) {
                    var getname = $(this).find(".name").html().split("/");
                    if (getname[1] == arr[1].trim()) { // nếu còn dòng đó thì đặt true cấp 2
                        isvalidtow = true;
                    }
                    if (getname[0] == arr[0].trim()) { // nếu còn dòng đó thì đặt true cấp 1

                        isvalidone = true;
                    }
                });
                if (isvalidtow) { // true thì chỉ add class và thay đổi butotn dòng đó chứ k xóa ở cấp 2

                    loadEventadmin.eventdisabledRow($(this));
                    loadEventadmin.loadEventBtnRollBack($(this));// add vaf load btn rooll back

                }
                else { // cấp 2 xóa trước khi không tìm dc dòng nào còn nữa
                    // xóa các dòng đó điều kiện là phải cùng tên cấp 2
                    if (ListArrNameStyleProductinDb.length > 0) {
                        // dành cho lúc edit có dữ liệu 
                        _this.parents("table#tableStylePrice").find("tbody").find("tr.newrowedit").filter(function () {
                            return $(this).find(".name").html().split("/")[1] == arr[1];
                        }).remove(); // sẽ xóa các dòng là newrowedit tức là dòng mới, các dòng trong db thì chỉ add class remove
                        // xử lý các dòng trong db thì add class remove thôi k dc xóa
                        _this.parents("table#tableStylePrice").find("tbody").find("tr.readData").not("." + _classremoteTr).each(function () {
                            if ($(this).find(".name").html().split("/")[1] == arr[1]) {
                                loadEventadmin.eventdisabledRow($(this).find(".btn-delete"));
                                loadEventadmin.loadEventBtnRollBack($(this).find(".btn-delete"));
                            }
                        }); // sẽ xóa các dòng là newrowedit tức là dòng mới, các dòng trong db thì chỉ add class remove
                    } else {
                        // khi tạo mới thì xóa thoải mái
                        _this.parents("table#tableStylePrice").find("tbody").find("tr").filter(function () {
                            return $(this).find(".name").html().split("/")[1] == arr[1];
                        }).remove(); // sẽ xóa các dòng nếu k tìm thấy nữa lưu ý là các dòng k có class remove, nếu có thì khi xóa sẽ xóa các dòng đó vì đã hết dòng k xóa rồi
                    }

                    // xử lý xóa input trên phần nhập sau khi xóa dưới đây
                    $("#items-input-addcontent").find(".input-addcontent").each(function (index, element) {
                        if (index == 1) {// cấp2
                            $(this).find("input").each(function () {
                                if (this.value == arr[1]) {
                                    $(this).parents(".item-grid-sort").remove();
                                }
                            });
                        }
                    });
                }
                if (!isvalidone) {
                    // xóa dòng đó điều kiện là phải cùng tên cấp 1
                    if (ListArrNameStyleProductinDb.length > 0) {
                        // dành cho lúc edit có dữ liệu 
                        _this.parents("table#tableStylePrice").find("tbody").find("tr.newrowedit").filter(function () {
                            return $(this).find(".name").html().split("/")[0] == arr[0];
                        }).remove(); // sẽ xóa các dòng là newrowedit tức là dòng mới, các dòng trong db thì chỉ add class remove
                        // xử lý các dòng trong db thì add class remove thôi k dc xóa
                        _this.parents("table#tableStylePrice").find("tbody").find("tr.readData").not("." + _classremoteTr).each(function () {
                            if ($(this).find(".name").html().split("/")[0] == arr[0]) {
                                $(this).addClass(_classremoteTr);
                                loadEventadmin.eventdisabledRow($(this).find(".btn-delete"));
                                loadEventadmin.loadEventBtnRollBack($(this).find(".btn-delete"));
                            }
                        }); // sẽ xóa các dòng là newrowedit tức là dòng mới, các dòng trong db thì chỉ add class remove
                    } else {
                        // khi tạo mới thì xóa thoải mái
                        _this.parents("table#tableStylePrice").find("tbody").find("tr").filter(function () {
                            return $(this).find(".name").html().split("/")[0] == arr[0];
                        }).remove(); // sẽ xóa các dòng nếu k tìm thấy nữa lưu ý là các dòng k có class remove, nếu có thì khi xóa sẽ xóa các dòng đó vì đã hết dòng k xóa rồi
                    }

                    // xử lý xóa input trên phần nhập sau khi xóa dưới đây
                    $("#items-input-addcontent").find(".input-addcontent").each(function (index, element) {
                        if (index == 0) { // cấp 1
                            $(this).find("input").each(function () {
                                if (this.value == arr[0]) {
                                    $(this).parents(".item-grid-sort").remove();
                                }
                            });
                        }
                    });
                }
            }
            ListArrNameStyleProduct = []; // mỗi lần update gì thì update lại list mới
            if ($("#add-table-styleproduct").find("table").find("tbody").find("tr").length == 0) {
                $("#add-table-styleproduct").remove();
                loadEventadmin.eventRemoveAllStyle();
            }
            else if ($("#add-table-styleproduct").find("table").find("tbody").find("tr." + _classremoteTr).length == $("#add-table-styleproduct").find("table").find("tbody").find("tr").length) {
                $("#add-table-styleproduct").remove();
                loadEventadmin.eventRemoveAllStyle();
            }
            else {
                // xử lý update lại list ListArrNameStyleProduct
                loadEventadmin.eventGetDataStyletable();
            }
        });

    },
    eventRemoveAllStyle: function () {
        $('input#IsAddingOptions').iCheck('uncheck')
    },
    eventGetDataStyletable: function (isdb = false) { // lấy danh sách table hiện tại
        ListArrNameStyleProduct = [];
        $("table#tableStylePrice").find("tbody").find("tr").each(function (index, element) {
            let className = $(this).attr("class");
            let idData = $(this).data("id");
            let name = $(this).find(".name").text();
            let code = $(this).find(".code").val();
            let price = $(this).find(".price").val().replaceAll(".", "");
            let quantity = $(this).find(".quantity").val().replaceAll(".", "");
            var itemDatatable = {}; //khởi tạo object
            itemDatatable.Name = name;
            itemDatatable.Id = idData;
            itemDatatable.Code = code;
            itemDatatable.Price = price;
            itemDatatable.Quantity = quantity;
            itemDatatable.ClassName = className;
            ListArrNameStyleProduct.push(itemDatatable); //thêm gán vào array
        });
        if (isdb) {
            ListArrNameStyleProductinDb = ListArrNameStyleProduct;
        }
    },
    eventloadremovetablestyleproduct: function () {
        $("#add-table-styleproduct").remove();
    },
    eventloadIcheck: function () {
        var triggeredByChild = false;
        var checkalltriggered = false;
        var uncheckalltriggered = false;
        $('#add-table-styleproduct .dataTables_scrollHead #check-all').on('ifChecked', function (event) {

            checkalltriggered = true;
            $(this).parents("#add-table-styleproduct").find(".dataTables_scrollBody").find("tbody").find('.item-check').each(function () {
                $(this).iCheck('check');
            });
            triggeredByChild = false;
            checkalltriggered = false;
        });

        $('#add-table-styleproduct .dataTables_scrollHead #check-all').on('ifUnchecked', function (event) {
            uncheckalltriggered = true;
            if (!triggeredByChild) {
                $(this).parents("#add-table-styleproduct").find(".dataTables_scrollBody").find("tbody").find('.item-check').each(function () {
                    $(this).iCheck('uncheck');
                });
            }
            uncheckalltriggered = false;
            triggeredByChild = false;

        });
        // Removed the checked state from "All" if any checkbox is unchecked
        $("#add-table-styleproduct").find(".dataTables_scrollBody").find("tbody").find('.item-check').on('ifUnchecked', function (event) {
            triggeredByChild = true;
            $(this).parents('#add-table-styleproduct').find('.dataTables_scrollHead #check-all').iCheck('uncheck');
        });
        $("#add-table-styleproduct").find(".dataTables_scrollBody").find("tbody").find('.item-check').on('ifChecked', function (event) {
            triggeredByChild = true;
            var countitem = $(this).parents("#add-table-styleproduct").find(".dataTables_scrollBody").find("tbody").find('.item-check').length;
            var dem = 0;
            $(this).parents("#add-table-styleproduct").find(".dataTables_scrollBody").find("tbody").find('.item-check').each(function () {
                var checkboxChecked = $(this).is(':checked');
                if (checkboxChecked) {
                    dem += 1;
                }
            });
            if (dem == countitem) {
                $(this).parents('#add-table-styleproduct').find('.dataTables_scrollHead #check-all').iCheck('check');
            }
        });
    },
    eventbtnaddstypeoptionproduct: function () {
        $(".btn-addoption").click(async function () {
            if ($("#items-input-addcontent").find(".input-addcontent").length > 1) {
                $(this).addClass("disabled");
                return;
            }
            if ($(this).hasClass("disabled")) {
                $(this).removeClass("disabled");
            }
            let _nclass = makeid(5);
            let html = `<div class="input-addcontent pr-3 pt-3 pb-3 ` + _nclass + `">
                                                       <div class="header-select mb-3 item-grid-sort">
                                                            <label for="">Tên tùy chọn<span class="required">(*)</span></label>

                                                            <button type="button" class="Sortable" role="button"><svg viewBox="0 0 20 20" class="Polaris-Icon__Svg_375hu" focusable="false" aria-hidden="true"><path d="M7 2a2 2 0 1 0 .001 4.001 2 2 0 0 0-.001-4.001zm0 6a2 2 0 1 0 .001 4.001 2 2 0 0 0-.001-4.001zm0 6a2 2 0 1 0 .001 4.001 2 2 0 0 0-.001-4.001zm6-8a2 2 0 1 0-.001-4.001 2 2 0 0 0 .001 4.001zm0 2a2 2 0 1 0 .001 4.001 2 2 0 0 0-.001-4.001zm0 6a2 2 0 1 0 .001 4.001 2 2 0 0 0-.001-4.001z"></path></svg></button>
                                                              
                                                                <select class="form-control">
                                                                    <opiton></opiton>
                                                                </select>
                                                            <i class="fa fa-trash"></i>
                                                        </div>
                                                        <div class="body-addoption">
                                                            <div class="tile-option-choose hidden-Sortable">
                                                                <label for="">Giá trị tùy chọn<span class="required">(*)</span></label>
                                                            </div>
                                                           <div class="item-grid-sort">
                                                               <button type="button" class="Sortable-body-addoption" role="button"><svg viewBox="0 0 20 20" class="Polaris-Icon__Svg_375hu" focusable="false" aria-hidden="true"><path d="M7 2a2 2 0 1 0 .001 4.001 2 2 0 0 0-.001-4.001zm0 6a2 2 0 1 0 .001 4.001 2 2 0 0 0-.001-4.001zm0 6a2 2 0 1 0 .001 4.001 2 2 0 0 0-.001-4.001zm6-8a2 2 0 1 0-.001-4.001 2 2 0 0 0 .001 4.001zm0 2a2 2 0 1 0 .001 4.001 2 2 0 0 0-.001-4.001zm0 6a2 2 0 1 0 .001 4.001 2 2 0 0 0-.001-4.001z"></path></svg></button>
                                                               <input type="text" class="form-control" name="" data-id="" placeholder="Nhập giá trị" />
                                                               <i class="fa fa-trash"></i>
                                                            </div>
                                                        </div>
                                                        <button class="btn btn-success" type="button" data-style="expand-left">
                                                            <span class="ladda-label"><i class="fas fa-check"></i> Xong</span>
                                                        </button>
                                                    </div>
                                              `;

            $(".card-style-option").append(html);
            $("." + _nclass + " .header-select").addClass("new-input-data");
            loadEventadmin.eventaddstypeoptionproduct($("." + _nclass));
            loadEventadmin.eventsortableproduct();
            loadEventadmin.event_removerowsortable($(".header-select"));
            await loadEventadmin.event_loadajaxdataselect2($("." + _nclass + " .header-select select"));
            // check nếu thêm thì check có 2 csai rồi thì làm mờ button đi
            if ($("#items-input-addcontent").find(".input-addcontent").length > 1) {
                if (!$(".btn-addoption").hasClass("disabled")) {
                    $(".btn-addoption").addClass("disabled")
                }
            }
        });
    },
    enventloadeventTablestyleproduct: async function (sel, html, btn_rollback) {
        loadEventadmin.eventloadremovetablestyleproduct();

        $(sel).parents(".option-add").after(html);
        priceFormat();
        if (btn_rollback) { //nếu có rollback thì mới chạy sự kiện
            $("table#tableStylePrice").find("tbody").find("tr").find(".btn-rollback").click(function (item) {
                // add sự kiện cho nút khôi phục đó
                let _nclass = makeid(5);
                htmllstbutton = `<button type="button" class="btn btn-sm btn-primary mr-2 btn-edit">Sửa</button>
                                         <button type="button" class="btn btn-sm btn-danger btn-delete ` + _nclass + `">Xóa</button>`;
                $(this).parent(".lst-button").html(htmllstbutton);
                loadEventadmin.eventBtnRollbackItemtableStyleProduct($("." + _nclass)); // add sự kiện cho nút xóa sửa lại sau khi update
            });
        }

        $('#add-table-styleproduct input.icheck').iCheck({
            checkboxClass: 'icheckbox_square-green',
            radioClass: 'iradio_square-green',
            increaseArea: '20%' // optional
        });
        ///event delete
        loadEventadmin.eventdeletebtnStyletableproduct($("#add-table-styleproduct"));
        $('#tableStylePrice').DataTable({
            scrollY: "300px",
            scrollX: true,
            scrollCollapse: true,
            paging: false,
            searching: false,
            ordering: false,
            "info": false,
            fixedColumns: {
                left: 2
            }
        });
        loadEventadmin.eventloadIcheck();
        loadEventadmin.eventGetDataStyletable();
    },
    InitloadStyleproduct: async function (url) {//ban đầu là load table và style ra
        var getdata = await axios.get(url);

        if (getdata.data.isValid) {
            let html = `<div class="card-style-option" id="items-input-addcontent">`;
            getdata.data.data.styleProductModels.forEach(async function (element, index) {

                let htmlinput = "";
                if ($("#add-option-body").html().trim() != "") {
                    html = "";
                }
                element.optionsNames.forEach(function (element, index) {
                    htmlinput += `<div class="item-grid-sort data-success">
                                    <button type="button" class="Sortable-body-addoption" role="button"><svg viewBox="0 0 20 20" class="Polaris-Icon__Svg_375hu" focusable="false" aria-hidden="true"><path d="M7 2a2 2 0 1 0 .001 4.001 2 2 0 0 0-.001-4.001zm0 6a2 2 0 1 0 .001 4.001 2 2 0 0 0-.001-4.001zm0 6a2 2 0 1 0 .001 4.001 2 2 0 0 0-.001-4.001zm6-8a2 2 0 1 0-.001-4.001 2 2 0 0 0 .001 4.001zm0 2a2 2 0 1 0 .001 4.001 2 2 0 0 0-.001-4.001zm0 6a2 2 0 1 0 .001 4.001 2 2 0 0 0-.001-4.001z"></path></svg></button>
                                     <input type="text" class="form-control" name="" placeholder="Nhập giá trị" data-id="` + element.id + `" value="` + element.name + `" />
                                     <i class="fa fa-trash"></i>
                                 </div>`;
                });
                htmlinput += `<div class="item-grid-sort hidden-Sortable">
                                    <button type="button" class="Sortable-body-addoption" role="button"><svg viewBox="0 0 20 20" class="Polaris-Icon__Svg_375hu" focusable="false" aria-hidden="true"><path d="M7 2a2 2 0 1 0 .001 4.001 2 2 0 0 0-.001-4.001zm0 6a2 2 0 1 0 .001 4.001 2 2 0 0 0-.001-4.001zm0 6a2 2 0 1 0 .001 4.001 2 2 0 0 0-.001-4.001zm6-8a2 2 0 1 0-.001-4.001 2 2 0 0 0 .001 4.001zm0 2a2 2 0 1 0 .001 4.001 2 2 0 0 0-.001-4.001zm0 6a2 2 0 1 0 .001 4.001 2 2 0 0 0-.001-4.001z"></path></svg></button>
                                     <input type="text" class="form-control" data-id="" name="" placeholder="Nhập giá trị"  />
                                     <i class="fa fa-trash"></i>
                                 </div>`;
                let _nclass = makeid(5);

                html += `<div class="input-addcontent pr-3 pt-3 pb-3">
                           <div class="header-select mb-3 item-grid-sort ` + _nclass + `">
                              <label for="">Tên tùy chọn<span class="required">(*)</span></label>
                              <button type="button" class="Sortable" role="button">
                                 <svg viewBox="0 0 20 20" class="Polaris-Icon__Svg_375hu" focusable="false" aria-hidden="true">
                                    <path d="M7 2a2 2 0 1 0 .001 4.001 2 2 0 0 0-.001-4.001zm0 6a2 2 0 1 0 .001 4.001 2 2 0 0 0-.001-4.001zm0 6a2 2 0 1 0 .001 4.001 2 2 0 0 0-.001-4.001zm6-8a2 2 0 1 0-.001-4.001 2 2 0 0 0 .001 4.001zm0 2a2 2 0 1 0 .001 4.001 2 2 0 0 0-.001-4.001zm0 6a2 2 0 1 0 .001 4.001 2 2 0 0 0-.001-4.001z"></path>
                                 </svg>
                              </button>
                              <select class="form-control">
                                 <opiton></opiton>
                              </select>
                              <i class="fa fa-trash"></i>
                           </div>
                           <div class="body-addoption">
                              <div class="tile-option-choose hidden-Sortable">
                                 <label for="">Giá trị tùy chọn<span class="required">(*)</span></label>
                              </div>
                              ` + htmlinput + `
                           </div>
                           <button class="btn btn-success" type="button" data-style="expand-left">
                           <span class="ladda-label"><i class="fas fa-check"></i> Xong</span>
                           </button>
                        </div>`;
                if ($("#add-option-body").html().trim() == "") {
                    html += "</div>";
                    $("#add-option-body").append(html);
                } else {
                    $("#items-input-addcontent").append(html);
                }

                $("#items-input-addcontent ." + _nclass).addClass("new-input-data");
                loadEventadmin.event_removerowsortable($("." + _nclass));
                /// sự kiện các input nhập
                $("#items-input-addcontent").find(".item-grid-sort.data-success").find(".fa-trash").click(function () { // kích hoạt sự kiện cho nút xóa của dòng hiện tại là dòng cuối cùng
                    $(this).parents(".item-grid-sort.data-success").remove();
                    loadEventadmin.eventGenerateDataPriceproduct($("#add-option-body"));
                });
                await loadEventadmin.event_loadajaxdataselect2($("." + _nclass + " select"), element.idStyleOptionsProduct);
            });

            loadEventadmin.eventsortableproduct();
            let htmlfooter = `<div class="card-footer">
                                   <div class="btn-addoption">
                                        <i class="fas fa-solid fa-plus mr-2"></i> Thêm tùy chọn khác
                                  </div>
                             </div>`;
            $(".option-add .card").append(htmlfooter);
            if ($("#items-input-addcontent").find(".input-addcontent").length > 1) {
                if (!$(".btn-addoption").hasClass("disabled")) {
                    $(".btn-addoption").addClass("disabled")
                }
            }
            loadEventadmin.eventaddstypeoptionproduct($("#items-input-addcontent"));
            loadEventadmin.eventbtnaddstypeoptionproduct();
            // load table style
            if (getdata.data.data.optionsDetailtProductModels.length > 0) {
                let htmlbody = "";
                ListArrNameStyleProductinDb = getdata.data.data.optionsDetailtProductModels;// lưu lại list db
                getdata.data.data.optionsDetailtProductModels.forEach(function (element, index) {
                    htmlbody += `
                                <tr class="`+ _classloadTrDb + `" IdOptionsName="` + element.idOptionsName + `" data-id="` + element.id + `">
                                      <td><input type="checkbox" class="icheck item-check" /></td>
                                      <td>
                                         <div class="imgstypedetailt"></div>
                                      </td>
                                      <td class="name">` + element.name + `</td>
                                      <td><input type="text"  class="form-control code" value="` + element.sku + `" /></td>
                                      <td><input type="text" class="form-control price priceFormat" value="` + element.price + `"/></td>
                                      <td><input type="text" class="form-control quantity priceFormat" value="` + element.quantity + `"/></td>
                                      <td>
                                         <div class="lst-button">
                                            <button type = "button" class="btn btn-sm btn-primary mr-2 btn-edit"> Sửa</button >
                                            <button type="button" class="btn btn-sm btn-danger btn-delete">Xóa</button>
                                         </div>
                                      </td>
                                </tr>
                            `;
                });
                let html = `<div class="pt-3 pb-3 w-100" id="add-table-styleproduct" style="background-color:#f3f3f6;">
                               <div class="card" style="box-shadow:none">
                                   <div class="card-header">
                                      <div>
                                       <h3 style="font-size: 18px;font-weight:bold" class="mb-0">Mẫu mã</h3>
                                     </div>
                                  </div>
                                <div class="card-body">
                                <table class="table table-bordered" id="tableStylePrice">
                                    <thead>
                                       <tr>
                                          <th><input type="checkbox" class="icheck" id="check-all" /></th>
                                          <th>Hình ảnh</th>
                                          <th>Mẫu mã</th>
                                          <th>Mã hàng</th>
                                          <th>Giá</th>
                                          <th style="width:100px">Số lượng</th>
                                          <th>Công cụ</th>
                                       </tr>
                                    </thead>
                                    <tbody>
                                        ` + htmlbody + `
                                    </tbody>
                                 </table>
                               </div>
                              </div>
                           </div>
                           `;
                loadEventadmin.enventloadeventTablestyleproduct($("#items-input-addcontent"), html, false);
            }

        }
    },
    eventsortableproduct: function () {
        $('#items-input-addcontent .body-addoption').sortable({
            //scrollSensitivity: 2,
            handle: 'button.Sortable-body-addoption',
            // helper: 'clone',
            items: "div:not(.hidden-Sortable)",
            cancel: "textarea",
            axis: "y",
            zIndex: 9999,
            distance: 10,
            scrollSpeed: 0,
            cursor: "move",
            tolerance: "pointer",
            connectWith: ".m-card-sortable",
            cancel: '.tile-option-choose',
            start: function start(event, ui) {
                $(ui.item).addClass("active-sortable");
            },
            stop: function stop(event, ui) {
                $(ui.item).removeClass("active-sortable");

            },
            change: function change(event, ui) {

            },
            update: function update(event, ui) {
                loadEventadmin.eventGenerateDataPriceproduct($("#add-option-body"));
            }
        });
        $('#items-input-addcontent').sortable({
            //scrollSensitivity: 2,
            handle: 'button.Sortable',
            // helper: 'clone',
            //revert: true,
            cancel: "textarea",
            axis: "y",
            zIndex: 9999,
            distance: 10,
            scrollSpeed: 0,
            cursor: "move",
            tolerance: "pointer",
            connectWith: ".m-card-sortable",
            cancel: '.m-add-card, .modal',
            start: function start(event, ui) {
                $(ui.item).addClass("active-sortable");
            },
            stop: function stop(event, ui) {
                $(ui.item).removeClass("active-sortable");

            },
            change: function change(event, ui) {

            },
            update: function update(event, ui) {
                loadEventadmin.eventGenerateDataPriceproduct($("#add-option-body"));
            }
        });
        //$('#items-input-addcontent input').bind('click.sortable mousedown.sortable', function (ev) {
        //    ev.target.focus();
        //});
    }
}
var supplierEInvoice = {
    eventcheckConnect: function () {
        $(".btn-checkconncetinview").click(function () {//CheckConnectWebservice
            let dataid = $(this).data("id");
            $.ajax({
                type: 'POST',
                //global: false,
                url: "/Selling/SupplierEInvoice/CheckConnectWebservice?secret=" + dataid,

                success: function (res) {
                    if (res.isValid) {

                    }
                },
                error: function (err) {
                    console.log(err)
                }
            })
        });
    },
    removeSupplierEInvoice: function () {
        $(".btn-removeSupplierEInvoice").click(function () {//CheckConnectWebservice
            let dataid = $(this).data("id");
            sel = $(this).parents("li.item-infoEinvoice");
            Swal.fire({
                title: 'Bạn có chắc chắn muốn gỡ bỏ nhà cung cấp không?',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Đồng ý',
                cancelButtonText: 'Hủy bỏ',
                showLoaderOnConfirm: true,
                preConfirm: (text) => {
                    $.ajax({
                        type: 'POST',
                        //global: false,
                        url: "/Selling/SupplierEInvoice/Remove?secret=" + dataid,
                        success: function (res) {
                            if (res.isValid) {
                                html = `<img class="disnable" src="../images/VNPT_Logo.png" />
                                <p class="mt-3">Của hàng của bạn chưa kết nối hóa đơn điện tử VNPT với SposViet</p>
                                <button class="btn btn-primary btn-addeinvoice" data-id="`+ ENumSupplierEInvoice.VNPT + `">Kết nối</button>`;
                                sel.html(html);
                                addevincoie = $(".btn-addeinvoice");
                                supplierEInvoice.eventaddEinvoice();
                            }
                        },
                        error: function (err) {
                            console.log(err)
                        }
                    })
                },
                // allowOutsideClick: () => !Swal.isLoading()
            })

        });
    },
    loadeventremoveidatata: function (sel) {
        $(".lstSuplerinvoice li").each(function () {
            $(this).find("button").filter(function (sel) {
                let dataid = $(this).data("id");
                $(this).removeAttr("data-id");
                $(this).data("id", dataid);
            });
        });

    },
    eventaddEinvoice: function () {
        addevincoie.click(function () {
            sel = $(this).parents("li.item-infoEinvoice");
            let id = $(this).data("id");
            $.ajax({
                type: 'GET',
                //global: false,
                url: "/Selling/SupplierEInvoice/Create?TypeSupplierEInvoice=" + id,
                // data: fomrdata,
                contentType: false,
                processData: false,
                success: function (res) {
                    if (res.isValid) {
                        Swal.fire({
                            // icon: 'success',
                            position: 'top-end',
                            showClass: {
                                popup: `
                              popup-formcreate
                               animate__animated
                              animate__fadeInRight
                              animate__faster
                            `
                            },
                            hideClass: {
                                popup: "popup-formcreate animate__animated animate__fadeOutRight animate__faster"

                            },
                            showCloseButton: true,

                            title: "Cấu hình kết nối hóa đơn điện tử VNPT",
                            html: res.html,
                            //showClass: {
                            //    popup: 'popup-formcreate'
                            //},

                            footer: "<button class='btn btn-primary btn-continue mr-3'><i class='fas fa-cancel'></i>Hủy bỏ</button><button class='mr-3 btn btn-save btn-success'><i class='fas fa-check mr-2'></i>Lưu</button><button class='btn btn-checkconncet btn-success'><i class='fas fa-check mr-2'></i>Kiểm tra kết nối</button>",
                            allowOutsideClick: true,
                            showConfirmButton: false,
                            showCancelButton: false,
                            cancelButtonText: '<i class="fa fa-window-close"></i> Đóng',
                            didRender: () => {
                                validateForm.formEivnoiceVNPT();

                                $(".btn-continue").click(function () {
                                    Swal.close();
                                });
                                $(".btn-checkconncet").click(function () {//CheckConnectWebservice
                                    if ($("form#create-form").valid()) {
                                        $.ajax({
                                            type: 'POST',
                                            //global: false,
                                            url: "/Selling/SupplierEInvoice/CheckConnectWebservice",
                                            data: $("#create-form").serialize(),

                                            success: function (res) {
                                                if (res.isValid) {



                                                }
                                            },
                                            error: function (err) {
                                                console.log(err)
                                            }
                                        })
                                    }

                                });
                                $(".btn-save").click(function () {
                                    if ($("form#create-form").valid()) {
                                        $.ajax({
                                            type: 'POST',
                                            //global: false,
                                            url: $("form#create-form")[0].action,
                                            data: $("#create-form").serialize(),
                                            success: function (res) {
                                                if (res.isValid) {
                                                    DomainName = $("#DomainName").val();
                                                    UserNameAdmin = $("#UserNameAdmin").val();
                                                    PassWordAdmin = $("#PassWordAdmin").val();
                                                    UserNameService = $("#UserNameService").val();
                                                    PassWordService = $("#PassWordService").val();
                                                    html = `<div class="row">
                                                                    <div class="col-md-5">
                                                                        <img class="" src="../images/VNPT_Logo.png" />
                                                                        <p class="mt-3 text-success">Của hàng của bạn  kết nối thành công hóa đơn điện tử VNPT với SposViet</p>
                      
                                                                    </div>
                                                                    <div class="col-md-7 infoaccount">
                                                                        <label><a target="_blank" href="`+ DomainName + `">` + DomainName + `</a></label>
                                                                        <span><b>Tài khoản admin:</b> `+ UserNameAdmin + `/******</span>
                                                                        <span><b>Tài khoản webservice:</b> `+ UserNameService + `/******</span>
                                                                        <a href="/Selling/ManagerPatternEInvoice?secret=`+ res.secrettype + `" class="btn btn-success btn-sm">Quản lý mẫu số ký hiệu hóa đơn VNPT</a>
                                                                    </div>
                                                                    <div class="col-md-12 text-center">
                                                                          <div class="sltbtn">
                                                                            <button class="btn btn-danger btn-removeSupplierEInvoice" data-id="`+ res.secret + `"><i class="fas fa-trash mr-2"></i>Gỡ bỏ</button>
                                                                            <button class="btn btn-primary btn-editeinvoice" data-id="`+ res.secret + `"><i class="fas fa-edit mr-2"></i>Chỉnh sửa</button>
                                                                            <button class="btn btn-info btn-checkconncetinview" data-id="`+ res.secret + `"><i class="fas fa-wifi mr-2"></i>Kiểm tra kết nối</button>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            `;

                                                    sel.html(html);
                                                    editeinvoice = $(".btn-editeinvoice");
                                                    supplierEInvoice.showViewConnect();
                                                    supplierEInvoice.eventcheckConnect();
                                                    supplierEInvoice.removeSupplierEInvoice();
                                                    Swal.close();
                                                }
                                            },
                                            error: function (err) {
                                                console.log(err)
                                            }
                                        })
                                    }

                                });

                            }
                        });
                    }
                },
                error: function (err) {
                    console.log(err)
                }
            });
        });
    },
    showViewConnect: function () {

        supplierEInvoice.loadeventremoveidatata();

        editeinvoice.click(function () {
            sel = $(this).parents("li.item-infoEinvoice");
            let id = $(this).data("id");
            $.ajax({
                type: 'GET',
                //global: false,
                url: "/Selling/SupplierEInvoice/Edit?secret=" + id,
                // data: fomrdata,
                contentType: false,
                processData: false,
                success: function (res) {
                    if (res.isValid) {
                        Swal.fire({
                            // icon: 'success',
                            position: 'top-end',
                            showClass: {
                                popup: `
                              popup-formcreate
                               animate__animated
                              animate__fadeInRight
                              animate__faster
                            `
                            },
                            hideClass: {
                                popup: "popup-formcreate animate__animated animate__fadeOutRight animate__faster"

                            },
                            showCloseButton: true,

                            title: "Cấu hình kết nối hóa đơn điện tử VNPT",
                            html: res.html,
                            //showClass: {
                            //    popup: 'popup-formcreate'
                            //},

                            footer: "<button class='btn btn-primary btn-continue mr-3'><i class='fas fa-cancel'></i>Hủy bỏ</button><button class='mr-3 btn btn-save btn-success'><i class='fas fa-check mr-2'></i>Lưu</button><button class='btn btn-checkconncet btn-success'><i class='fas fa-check mr-2'></i>Kiểm tra kết nối</button>",
                            allowOutsideClick: true,
                            showConfirmButton: false,
                            showCancelButton: false,
                            cancelButtonText: '<i class="fa fa-window-close"></i> Đóng',
                            didRender: () => {
                                validateForm.formEivnoiceVNPT();

                                $(".btn-continue").click(function () {
                                    Swal.close();
                                });
                                $(".btn-checkconncet").click(function () {//CheckConnectWebservice
                                    if ($("form#create-form").valid()) {
                                        $.ajax({
                                            type: 'POST',
                                            //global: false,
                                            url: "/Selling/SupplierEInvoice/CheckConnectWebservice",
                                            data: $("#create-form").serialize(),

                                            success: function (res) {
                                                if (res.isValid) {

                                                }
                                            },
                                            error: function (err) {
                                                console.log(err)
                                            }
                                        })
                                    }

                                });
                                $(".btn-save").click(function () {
                                    if ($("form#create-form").valid()) {
                                        $.ajax({
                                            type: 'POST',
                                            //global: false,
                                            url: $("form#create-form")[0].action,
                                            data: $("#create-form").serialize(),
                                            success: function (res) {
                                                if (res.isValid) {
                                                    DomainName = $("#DomainName").val();
                                                    UserNameAdmin = $("#UserNameAdmin").val();
                                                    PassWordAdmin = $("#PassWordAdmin").val();
                                                    UserNameService = $("#UserNameService").val();
                                                    PassWordService = $("#PassWordService").val();
                                                    html = `<div class="row">
                                                                    <div class="col-md-5">
                                                                        <img class="" src="../images/VNPT_Logo.png" />
                                                                        <p class="mt-3 text-success">Của hàng của bạn  kết nối thành công hóa đơn điện tử VNPT với SposViet</p>
                      
                                                                    </div>
                                                                    <div class="col-md-7 infoaccount">
                                                                        <label><a target="_blank" href="`+ DomainName + `">` + DomainName + `</a></label>
                                                                        <span><b>Tài khoản admin:</b> `+ UserNameAdmin + `/******</span>
                                                                        <span><b>Tài khoản webservice:</b> `+ UserNameService + `/******</span>
                                                                    </div>
                                                                    <div class="col-md-12 text-center">
                                                                          <div class="sltbtn">
                                                                            <button class="btn btn-danger btn-removeSupplierEInvoice" data-id="`+ res.secret + `"><i class="fas fa-trash mr-2"></i>Gỡ bỏ</button>
                                                                            <button class="btn btn-primary btn-editeinvoice" data-id="`+ res.secret + `"><i class="fas fa-edit mr-2"></i>Chỉnh sửa</button>
                                                                            <button class="btn btn-info btn-checkconncetinview" data-id="`+ res.secret + `"><i class="fas fa-wifi mr-2"></i>Kiểm tra kết nối</button>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            `;

                                                    sel.html(html);
                                                    editeinvoice = $(".btn-editeinvoice");
                                                    supplierEInvoice.showViewConnect();
                                                    supplierEInvoice.eventcheckConnect();
                                                    supplierEInvoice.removeSupplierEInvoice();
                                                    Swal.close();
                                                }
                                            },
                                            error: function (err) {
                                                console.log(err)
                                            }
                                        })
                                    }

                                });

                                $("#SelectlistSeri").select2({
                                    dropdownParent: $("#create-form"),
                                    placeholder: "Chọn loại chữ ký số",
                                    allowClear: true,
                                    language: {
                                        noResults: function () {
                                            return "Không tìm thấy dữ liệu";
                                        }
                                    },

                                })
                            }
                        });
                    }
                },
                error: function (err) {
                    console.log(err)
                }
            });
        });
    },
}
var ManagerPatternEInvoice = {
    addOredit: function (url) {
        $.ajax({
            type: 'GET',
            //global: false,
            url: url,
            // data: fomrdata,
            contentType: false,
            processData: false,
            success: function (res) {
                if (res.isValid) {
                    Swal.fire({
                        // icon: 'success',
                        position: 'top-end',
                        showClass: {
                            popup: `
                              popup-formcreate
                               animate__animated
                              animate__fadeInRight
                              animate__faster
                            `
                        },
                        hideClass: {
                            popup: "popup-formcreate animate__animated animate__fadeOutRight animate__faster"

                        },
                        showCloseButton: true,

                        title: res.title,
                        html: res.html,
                        //showClass: {
                        //    popup: 'popup-formcreate'
                        //},

                        footer: "<button class='btn btn-primary btn-continue mr-3'><i class='fas fa-cancel'></i>Hủy bỏ</button><button class='mr-3 btn btn-save btn-success'><i class='fas fa-check mr-2'></i>Lưu</button>",
                        allowOutsideClick: true,
                        showConfirmButton: false,
                        showCancelButton: false,
                        cancelButtonText: '<i class="fa fa-window-close"></i> Đóng',
                        didRender: () => {
                            validateForm.addOrEditMangerPatternEInvoice();

                            $(".btn-continue").click(function () {
                                Swal.close();
                            });

                            $(".btn-save").click(function () {
                                if ($("form#create-form").valid()) {
                                    jQueryModalPost($("form#create-form")[0]);
                                }

                            });

                        }
                    });
                }
            },
            error: function (err) {
                console.log(err)
            }
        });
    }
}
var commonfunc = {
    cancelOrder: function (sel) {
        var form = $(sel).parent("form#form_delete")[0];
        Swal.fire({
            title: 'Bạn có chắc chắn muốn hủy không?',
            input: 'textarea',
            inputPlaceholder: 'Vui lòng nhập lý do bạn muốn hủy, nội dung này sẽ được gửi đến khách hàng...',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Đồng ý',
            cancelButtonText: 'Hủy bỏ',
            showLoaderOnConfirm: true,
            inputValidator: (value) => {
                if (!value) {
                    return 'Vui lòng nhập lý do!'
                }
            },
            preConfirm: (text) => {
                try {
                    var fomrdata = new FormData(form);
                    fomrdata.append('content', text);
                    $.ajax({
                        type: 'POST',
                        //global: false,
                        url: form.action,
                        data: fomrdata,
                        contentType: false,
                        processData: false,
                        success: function (res) {
                            if (res.isValid) {
                                if (res.loadTable) {
                                    dataTableOut.ajax.reload(null, false);
                                } else if (res.loadTreeview) {
                                    loadData()
                                } else {
                                    $('#viewAll').html(res.html);
                                }
                            }
                        },
                        error: function (err) {
                            console.log(err)
                        }
                    })
                } catch (ex) {
                    console.log(ex)

                }
            },
            // allowOutsideClick: () => !Swal.isLoading()
        })
        //.then((result) => {
        //    if (result.isConfirmed) {
        //        Swal.fire({
        //            title: `$s avatar`
        //        })
        //    }
        //})
    },
    confirmDelete: function (url) {
        Swal.fire({
            title: 'Bạn có chắc chắn muốn tiếp tục không?',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Đồng ý',
            cancelButtonText: 'Hủy bỏ',
            showLoaderOnConfirm: true,
            preConfirm: (text) => {
                try {
                    var fomrdata = new FormData();
                    $.ajax({
                        type: 'POST',
                        //global: false,
                        url: url,
                        //data: { secret: secret },
                        contentType: false,
                        processData: false,
                        success: function (res) {
                            if (res.isValid) {
                                if (res.loadTable) {
                                    dataTableOut.ajax.reload(null, false);
                                } else {
                                    $('#viewAll').html(res.html);
                                }
                            }
                        },
                        error: function (err) {
                            console.log(err)
                        }
                    })
                } catch (ex) {
                    console.log(ex)

                }
            },
            // allowOutsideClick: () => !Swal.isLoading()
        })
        //.then((result) => {
        //    if (result.isConfirmed) {
        //        Swal.fire({
        //            title: `$s avatar`
        //        })
        //    }
        //})
    }
}
$(document).on("click", '#btnsubmitcancelorder', function (event) {
    var contentnote = $("#contentnote");
    var iserror = $(".iserror");
    if ($.trim(contentnote.val()) == "") {
        iserror.show();
        return false;
    }
    $(this).parent("form").submit();
});

function makeid(length) {
    var result = '';
    var characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
    var charactersLength = characters.length;
    for (var i = 0; i < length; i++) {
        result += characters.charAt(Math.floor(Math.random() * charactersLength));
    }
    return result;
}

function popupCenter(url, title, w, h) {
    var left = (screen.width / 2) - (w / 2);
    var top = (screen.height / 2) - (h / 2);
    const myWindow = window.open(url, title, 'toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=no, resizable=no, copyhistory=no, width=' + w + ', height=' + h + ', top=' + top + ', left=' + left);
}

function elFinderBrowser(callback, value, meta) {



    tinymce.activeEditor.windowManager.openUrl({
        title: 'Custom Dialog',
        url: '/tiny-mce/browse',
        width: 920,
        height: 540,
        oninsert: function (url, objVals) {

            callback(url, objVals);
        },
        onselect: function (file, fm) {

            var url, reg, info;

            // URL normalization
            url = fm.convAbsUrl(file.url);

            // Make file info
            info = file.name + ' (' + fm.formatSize(file.size) + ')';

            // Provide file and text for the link dialog
            if (meta.filetype == 'file') {
                callback(url, {
                    text: info,
                    title: info
                });
            }

            // Provide image and alt text for the image dialog
            if (meta.filetype == 'image') {
                callback(url, {
                    alt: info
                });
            }

            // Provide alternative source and posted for the media dialog
            if (meta.filetype == 'media') {
                callback(url);
            }
        }
    }, {
        oninsert: function (url) {

            callback(url);
        }
    }

    );
    return false;
}

function loadeventnotecontetn() {
    var contentnote = $("#contentnote");
    var iserror = $(".iserror");
    contentnote.keyup(function () {
        if ($.trim(contentnote.val()) == "") {
            iserror.show();
            return false;
        } else {
            iserror.hide();
            return true;
        }
    });
}

function removeVietnameseTones(str) {
    str = str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
    str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
    str = str.replace(/ì|í|ị|ỉ|ĩ/g, "i");
    str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
    str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
    str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
    str = str.replace(/đ/g, "d");
    str = str.replace(/À|Á|Ạ|Ả|Ã|Â|Ầ|Ấ|Ậ|Ẩ|Ẫ|Ă|Ằ|Ắ|Ặ|Ẳ|Ẵ/g, "A");
    str = str.replace(/È|É|Ẹ|Ẻ|Ẽ|Ê|Ề|Ế|Ệ|Ể|Ễ/g, "E");
    str = str.replace(/Ì|Í|Ị|Ỉ|Ĩ/g, "I");
    str = str.replace(/Ò|Ó|Ọ|Ỏ|Õ|Ô|Ồ|Ố|Ộ|Ổ|Ỗ|Ơ|Ờ|Ớ|Ợ|Ở|Ỡ/g, "O");
    str = str.replace(/Ù|Ú|Ụ|Ủ|Ũ|Ư|Ừ|Ứ|Ự|Ử|Ữ/g, "U");
    str = str.replace(/Ỳ|Ý|Ỵ|Ỷ|Ỹ/g, "Y");
    str = str.replace(/Đ/g, "D");
    // Some system encode vietnamese combining accent as individual utf-8 characters
    // Một vài bộ encode coi các dấu mũ, dấu chữ như một kí tự riêng biệt nên thêm hai dòng này
    str = str.replace(/\u0300|\u0301|\u0303|\u0309|\u0323/g, ""); // ̀ ́ ̃ ̉ ̣  huyền, sắc, ngã, hỏi, nặng
    str = str.replace(/\u02C6|\u0306|\u031B/g, ""); // ˆ ̆ ̛  Â, Ê, Ă, Ơ, Ư
    // Remove extra spaces
    // Bỏ các khoảng trắng liền nhau
    str = str.replace(/ + /g, " ");
    str = str.trim();
    // Remove punctuations
    // Bỏ dấu câu, kí tự đặc biệt
    str = str.replace(/!|@|%|\^|\*|\(|\)|\+|\=|\<|\>|\?|\/|,|\.|\:|\;|\'|\"|\&|\#|\[|\]|~|\$|_|`|-|{|}|\||\\/g, " ");
    return str;
}

function templateSelect2(data) {
    let html = "<span class='select-option-noParent-" + data.Level + "'>" + data.text + "</span>";
    if (data.isParent) {
        html = "<span class='select-option-isParent'>" + data.text + "</span>";
    }
    return html;
}
function loaddataSelect2Tempalte(URL, id, idselectd, placeholder = "", iddata = "", iscreateCategory = false) {

    IsPos = false;
    if (URL.includes("IsPos")) {
        IsPos = true;
    }
    $.ajax({
        global: false,
        async: true,
        type: "GET",
        dataType: 'JSON',
        url: URL,
        data: {
            IsPos: IsPos,
            idselectd: idselectd,
            iddata: iddata,
            iscreateCategory: iscreateCategory
        },
        success: function (data) {
            debugger
            $(id).append("<opiton value=''></opiton>").select2({
                data: data,
                placeholder: placeholder != "" ? placeholder : "Chọn giá trị",
                allowClear: true,
                language: {
                    noResults: function () {
                        return "Không tìm thấy dữ liệu";
                    }
                },

            })
        }
    });
}
function loaddataSelect2CustomsTempalte(URL, id, idselectd, placeholder = "", iddata = "", iscreateCategory = false) {

    IsPos = false;
    if (URL.includes("IsPos")) {
        IsPos = true;
    }
    $.ajax({
        global: false,
        async: true,
        type: "GET",
        dataType: 'JSON',
        url: URL,
        data: {
            IsPos: IsPos,
            idselectd: idselectd,
            iddata: iddata,
            iscreateCategory: iscreateCategory
        },
        success: function (data) {

            $(id).append("<opiton></opiton>").select2({
                data: data,
                placeholder: placeholder != "" ? placeholder : "Chọn giá trị",
                allowClear: true,
                templateResult: templateSelect2,
                escapeMarkup: function (m) {
                    return m;
                },
                language: {
                    noResults: function () {
                        return "Không tìm thấy dữ liệu";
                    }
                },

            })
        }
    });
}



function checkform() {
    $("#fomrvalid").validate({
        rules: {
            // "Name": {
            //  required: true,
            //    minlength: 5
            //  },
            //"Code": {
            //    required: true,
            //    minlength: 8
            //},
            //"re-password": {
            //    equalTo: "#password",
            //    minlength: 8

            //}
        },
        messages: {
            "Name": {
                required: "Vui lòng nhập tên",
                maxlength: "Hãy nhập ít nhất 8 ký tự"
            },
            "Code": {
                required: "Vui lòng nhập mã",
                minlength: "Hãy nhập ít nhất 8 ký tự"
            }
        }
    });

}
$(document).on("click", '#btnupdatepayment', function (event) {
    let status = $("#updatepayment input[type='radio']:checked").val();
    if (status == undefined) {
        toastr.error("Bạn chưa chọn loại thanh toán hay đặt cọc");
        return false;
    }
    if ($("#Amount").val() == "") {
        toastr.error("Bạn chưa nhập số tiền");
        return false;
    }
    $(this).parents("form").submit();
    // let Amount = $("#Amount").val();
    //let note = $("#note").val();
    //$.ajax({
    //    type: "POST",
    //    url: "/Admin/Payment/Updatepaymentorder",
    //    async: true,
    //    data: {
    //        Amount: Amount,
    //        note: note,
    //        status: status,
    //    },
    //    success: function (data) {

    //    }
    //});
});
$(document).on("click", '#btnsaveStatus', function (event) {
    var idorder = $(".litsstep").find(".active").data("id");
    var status = $(".litsstep").find(".active").data("status");
    var code = $(".litsstep").find(".active").data("code");
    var note = $(".bodyContentStatus").find("#step-" + status).find("#note").val();
    // var dataIdNote = $(".bodyContentStatus").find("#step-" + status).find("#note").data("id");

    var addnew = true;
    if ($('input[name="checknew"]').is(':checked')) {
        addnew = false;
    } else {
        addnew = true;
    }

    $.ajax({
        global: false,
        type: "POST",
        url: "/Admin/Order/UpdateStatus",
        async: true,
        data: {
            status: status,
            code: code,
            note: note,
            idorder: idorder,
            // IdNote: dataIdNote,
            // addnew: addnew
        },
        success: function (data) {
            if (data.isValid) {
                if (status == 4) {
                    $(".bodyContentStatus").find("#step-" + status).find("textarea").attr("disabled", "disabled").attr("readonly", "readonly").removeClass("textcurrent");
                    $(".btnsaveStatus").remove();
                }
                var listdata = $(".bodyContentStatus").find("#step-" + status).find(".listdata");
                listdata.find("li.active").removeClass("active");
                listdata.find("li.nodata").remove();

                let html = `<li  data-id="` + data.idnote + `" class="active">
                                            <img src="../images/logo.png"/>
                                            <div class="content">
                                                 <span class="create-user"> ` + data.fullname + `</span>    <span>- ` + data.note + `</span><br />
                                                <small>(` + data.date + `)</small>
                                                </div>
                                        </li>`;
                listdata.prepend(html);

                $(".bodyContentStatus").find("#step-" + status).find("#note").attr("data-id", data.idnote);

                var clon = $(".bodyContentStatus").find("#step-" + status).find("#note").clone();

                $(".bodyContentStatus").find("#step-" + status).find(".bodytextarea").html(clon);

                $(".litsstep").find(".active").addClass("datastatus");


                var textcurrent = $(".bodyContentStatus").find('.textcurrent');
                $(".bodyContentStatus").find("#step-" + status).find(".checkAddnew").removeClass("d-none");
                if (textcurrent.data("status") != status) {

                    $(".litsstep").find(".currentOrder").removeClass("currentOrder"); /// xóa cái k phải cái đã update
                    $(".litsstep").find(".status-" + status).children("a.nav-link").addClass("currentOrder"); // add lại cái mới
                    let statushidden = textcurrent.data("status");
                    textcurrent.attr("disabled", "disabled").attr("readonly", "readonly").removeClass("textcurrent");

                    $(".bodyContentStatus").find("#step-" + status).find("textarea").addClass("textcurrent");


                    $(".bodyContentStatus").find("#step-" + statushidden).find(".checkAddnew").remove();
                    $(".bodyContentStatus").removeAttr("style");
                    $(".bodyContentStatus").animate({
                        height: $(".bodyContentStatus").height()
                    }, 400)
                }
                //$(".bodyContentStatus").find('.textcurrent').attr("disabled", "disabled").attr("readonly", "readonly").removeClass(".textcurrent");
                //$(".bodyContentStatus").find("#step-" + status).find("textarea").addClass("textcurrent");
            }
        }
    });
});

function updateSatusContractFinal(idorder, value, sel) {
    confirms("Xác nhận", "Bạn có chắc chắn muốn cập nhật hay không?", "<i class='far fa-times-circle'></i> Hủy", "<i class='fas fa-check-circle'></i> Đồng ý", function () {
        try {
            $.ajax({
                async: false,
                type: 'POST',
                url: "/admin/order/UpdateSatusContractFinal",
                data: {
                    idOrder: idorder,
                    check: value
                },
                success: function (res) {
                    if (res.isValid) {
                        $(sel).attr("disabled", "disabled");
                    } else {

                    }

                },
                error: function (err) {
                    console.log(err)
                }
            })
        } catch (ex) {
            console.log(ex)
        }
    },
        function () {

            $(sel).prop('checked', false);
        }
    );

    // $(sel).attr('checked', false);
    //prevent default form submit event
    return false;
}
$(search).click(function () {
    loadLadda();
    dataTableOut.draw();
    Ladda.stopAll();
});
// upload exxcel


function readURL(input) {
    if (input.files && input.files[0]) {

        var reader = new FileReader();

        reader.onload = function (e) {
            $('.image-upload-wrap').hide();

            $('.file-upload-image').attr('src', e.target.result);
            $('.file-upload-content').show();

            $('.image-title').html(input.files[0].name);
        };

        reader.readAsDataURL(input.files[0]);

    } else {
        removeUpload();
    }
}
function printDiv(html) {
    //if (isAndroid) {
    //    //https://stackoverflow.com/questions/26684190/using-window-print-or-alternative-on-android-devices
    //    // https://developers.google.com/cloud-print/docs/gadget
    //    var gadget = new cloudprint.Gadget();
    //    gadget.setPrintDocument("url", html, window.location.href, "utf-8");
    //    gadget.openPrintDialog();
    // } else {
    // window.print();
    $("#printDocx").remove();
    Be = document.createElement("div");
    Be.setAttribute("id", "printDocx");
    // Be.style.position = "absolute",
    Be.style.display = "none",
        //  Be.style.top = "-9999px",
        document.body.appendChild(Be);

    $("#printDocx").html(html);
    var printElement = document.getElementById("printDocx");
    $(printElement).printArea({
        mode: "iframe",
        popWd: 900,
        popHt: 600, tagPrint: "#printDocx",
        popClose: false
    });
    window.onafterprint = function () {
        console.log("Printing completed...");
    }
    //}

    //Be.contentWindow && (Be.contentWindow.focus(),
    //Be.contentWindow.print(),
    //    Be.contentWindow.close())





    //Be = document.createElement("div");
    //Be.setAttribute("id", "printDocx");
    //Be.style.position = "absolute",
    //    Be.style.top = "-9999px",
    //    Be.innerHTML = html,
    //    document.body.appendChild(Be),
    //    Be.contentWindow && (Be.contentWindow.focus(),
    //        Be.contentWindow.print(),
    //        Be.contentWindow.close())
    //w = window.open();
    //w.document.write(html);
    //w.print();
    //w.close();
}
function __highlight(s, t) {
    //  t = t.replace(/[-\/\\^$*+?.()|[\]{}]/g, '\\$&');
    // var matcher = new RegExp("(" + t.split(' ').join('|') + ")", "gi");

    var matcher = new RegExp("(" + $.ui.autocomplete.escapeRegex(t) + ")", "ig");
    return s.replace(matcher, "<strong>$1</strong>");
}
function evetnFormatTextnumber3() {
    $('.number3').each(function () {
        if ($(this).is('input:text')) {
            let idtex = $(this).val().replaceAll(",", "");
            $(this).val(parseFloat(idtex).format0VND(0, 3, ""))
        } else {
            let idtex = $(this).text().replaceAll(",", "");
            $(this).html(parseFloat(idtex).format0VND(0, 3, ""))
        }
    });
} function eventUnFormatTextnumber3() {
    $('.number3').each(function () {
        if ($(this).is('input:text')) {
            let idtex = $(this).val().replaceAll(",", "") || 0;
            $(this).val(parseFloat(idtex))
        }
    });
}
function evetnFormatTextnumber3decimal() {
    $('.number3').each(function () {
        let idtex = $(this).text().replaceAll(",", ".");
        $(this).html(parseFloat(idtex).format0VND(0, 3, ""))
    });
}
function evetnFormatnumber3(updatevalue = true) {
    $("input.number3")
        .each(function (i, item) {
            if (updatevalue) {
                var _val = $(this).val().replaceAll(',', '.');
                $(this).val(_val);
            }
            fnInitialFormatNumber(this);
        }).ForceNumericOnly()
        .focus(function () {
            var _val = $(this).val();
            $(this).val(_val.replaceAll(',', ''));
        })
        .focusout(function () {
            fnFocusOut(this);
        });
}
function loadFormatnumber(sel = "") {
    if (sel != "") {

        $(sel).find(".number3")
            .each(function (i, item) {

                fnInitialFormatNumber(this);
            }).ForceNumericOnly()
            .focus(function () {
                var _val = $(this).val();
                $(this).val(_val.replaceAll(',', ''));
            })
            .focusout(function () {
                fnFocusOut(this);
            });
    }
    else {
        $('.number3')
            .each(function (i, item) {

                var _val = $(this).val().replaceAll(',', '.');
                $(this).val(_val);
                fnInitialFormatNumber(this);
            }).ForceNumericOnly()
            .focus(function (i, item) {

                var _val = $(this).val();
                $(this).val(_val.replaceAll(',', ''));
            })
            .focusout(function (i, item) {
                fnFocusOut(this);
            });
    }
}
function removeUpload() {
    $('.file-upload-input').replaceWith($('.file-upload-input').val('').clone(true));
    $('.file-upload-content').hide();
    $('.image-upload-wrap').show();
}
$('.image-upload-wrap').bind('dragover', function () {
    $('.image-upload-wrap').addClass('image-dropping');
});
$('.image-upload-wrap').bind('dragleave', function () {
    $('.image-upload-wrap').removeClass('image-dropping');
});
loadFormatnumber();
loaddaterangepicker();
//end
// add default
$.validator.addMethod('minStrict', function (value, el, param) {
    value = parseInt(value.replaceAll(",", ""));
    return value > param;
});
const startbtn = document.getElementById("startbtnaudio");
const audio = document.getElementById("audio");
startbtn.addEventListener("click", () => {

    audio.play().then(() => {
    });
});
function playaudioNotifyChitchen() {
    //var audio = new Audio('../audio/chuongbao.mp3');

    //audio.play();
    startbtn.click();
    // var audio = document.getElementById("audio");
    // audio.muted = false;
    //audio.muted = false;
    //  audio.pause();
    // await audio.play();
}
function checkautido() {
    Swal.fire({
        title: 'Nhấn Ok để kiểm tra chuông báo!',
        showDenyButton: false,
        showCancelButton: false,
        confirmButtonText: 'OK',
        // denyButtonText: `Don't save`,
    }).then((result) => {
        /* Read more about isConfirmed, isDenied below */
        if (result.isConfirmed) {
            Swal.close();
            startbtn.click();
        }
    })
}
function testConnetWebSocket() {
    sposvietplugin.sendConnectSocket(listport[0]).then(function (data) {
        console.log(data);
    });
}
function parseDecimal(equation, precision = 9) {
    //return Math.floor(equation * (10 ** precision)) / (10 ** precision);
    return Math.round(equation * Math.pow(10, precision)) / Math.pow(10, precision);
} function GUID() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}
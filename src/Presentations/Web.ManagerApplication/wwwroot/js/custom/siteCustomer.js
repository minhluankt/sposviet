var dataTableOut;
var idcategory = 0;
var idPrice = 0;
var sortby = "";
var urlGetCategoryPost = "/API/Handling/GetAllCategoryPost";
var initParams = {
    clearParams: function () {
        idPrice = 0;
        sortby = "";
    }
};
function CustomePopupAddCart(title, result) {
    Swal.fire({
        // icon: 'success',
        title: title,
        html: result,
        showClass: {
            popup: 'popup-cart'
        },
        footer: "<button class='btn btn-primary btn-continue mr-3'><i class='icon-cd icon-add_cart icon'></i>Tiếp tục mua hàng</button><button class='btn btn-gocart btn-success'><i class='icon-cd icon-doneAll icon'></i>Tiến hành đặt hàng</button>",
        allowOutsideClick: true,
        showConfirmButton: false,
        showCancelButton: false,
        cancelButtonText:
            '<i class="fa fa-window-close"></i> Đóng',
        didRender: () => {
            $(".btn-continue").click(function () {
                Swal.close();
            });
            $(".btn-gocart").click(function () {
                Swal.close();
                window.location.href = "/Cart/Mycart";
            });

        },
    });
}
var loadnEvent_init = {
    loadeventmobi: function () {
        if ($(window).width() < 769) {
            $(".careerCenterMenu").click(function () {
                $(this).find(".list-menu-profile").toggleClass("hide");
                $(this).find(".list-menu-profile").toggleClass("show");
            });
        }
    },
    loadeventcardmobi: function () {
        if ($(window).width() < 769) {
            $("html").css("overflow", "hidden");
            $(".btn-back-mobi").click(function () {
                history.back();
            })

            $(".remove-mobi").remove();
        } else {

            $(".remove-pc").remove();
        }

    },
    loadeventnotifyuser: function () {
        $(".lstPaginatedListcontent .delete").click(function () {
            var sel = $(this);
            var id = $(this).data("id");
            $.ajax({
                url: "/Account/DeleteDataNotifmy",
                type: 'POST',
                cache: false,
                data: { id: id },
                success: function (result) {
                    if (result.isValid) {
                        sel.parent("li").remove();
                    }
                }
            });
        });
    },
    loadeventcountnotifyuser: function () {

        $.ajax({
            url: "/Account/CountNotifmyUser",
            type: 'GET',
            cache: false,
            success: function (result) {
                if (result.isValid) {

                    $("b#count-notify").html(result.count);
                }
            }
        });

    },
    loadAddCart: function () {
        var btnadd_to_cart = $(".add-to-cart");
        var btnaddCart_detaiprod = $(".btnaddCart-detaiprod");
        $(btnadd_to_cart).click(function () {


            nameProAddCart = $(this).data("name");
            imgProAddCart = $(this).data("img");
            priceProAddCart = $(this).data("price");
            addCart($(this).data("id"), 1, true);
        });

        $(btnaddCart_detaiprod).click(function () {
            nameProAddCart = $(this).data("name");
            imgProAddCart = $(this).data("img");
            priceProAddCart = $(this).data("price");
            addCart($(this).data("id"), parseInt($("#qty").val()), true);
        });
    },
    loadicheckpriceinCateogry: function () {
        var url_string = window.location.href;
        var url = new URL(url_string);
        var params = new URLSearchParams(url.search);
        let ordersortby = params.get("sortby");
        $("input[order='" + ordersortby + "']").iCheck('check');

    },
    loadevenradiocheck: function (page) {
        $('input#order').on('ifChecked', async function (event) {

            let idcate = $(this).parents(".index-block-product").find("#IdCode").val();
            let order = $(this).attr("order");
            let slug = $(this).data("slug-cate");
            await loadatahtmlsell(this, idcate, page, order, slug);
        });
    },
    fetdataProductSellByCategory: async function (slug) {
        var gethtml = await axios.get('/Search/GetProductSell?slug=' + slug);
        $("#dataProductSell").html(gethtml.data);

        loadevent.load_sort_productmobi(); // load sự kiện sort

        loadisCheck();
        setTimeout(() => {
            loadnEvent_init.loadevenradiocheck(1);
            loadnEvent_init.loadAddCart();
        }, 500)

        $(document).on('click', '.pagination  a', async function (event) {
            event.preventDefault();
            var page = $(this).attr('href').split('page=')[1];
            let slug = $(this).attr('href').split('page=')[0];
            var page2 = page.split('&')[0];
            var cate = $(this).attr('href').split('txt=')[1];
            var txtcate = cate.split('&')[0];
            await loadatahtmlsell(this, txtcate, page2, slug.replaceAll("?", ""));

        })

    },
    fetdataProductSellByOneCategory: async function (idcate, sortby) {

        // var gethtml = await axios.get('/Search/GetProductByCategory?isPromotion=true&loadmore=true&page=1&idcate=' + idcate + '&sortby=' + sortby);
        // $("#dataProduct").append(gethtml.data);
        // loadisCheck();
        //var pageIndex = 2;
        //var _incallback = false;
        //var pages = 2;
        loadnEvent_init.loadicheckpriceinCateogry();
        setTimeout(function () {
            $('input#order').on('ifChecked', function (event) {
                sortby = $(this).attr("order");
                SortAndSeartInCategoryProduct(1);
            });
            $('input#price').on('ifChecked', function (event) {
                idPrice = $(this).val();
                SortAndSeartInCategoryProduct(1);
            });


        }, 500);
        loadevent.load_tab_show_incategoryproduct();
        initParams.clearParams();
        $(window).scroll(function () {
            var hT = $('#progressmarker').offset().top,
                hH = $('#progressmarker').outerHeight(),
                wH = $(window).height(),
                wS = $(window).scrollTop();
            // don't do it if we have reached last page OR we are still grabbing items
            if (pages >= pageIndex && !_incallback) {
                if (wS > (hT + hH - wH - 600)) {
                    loadnEvent_init.fetdataProductSellByOneCategoryLoadmore(pageIndex, idcate, sortby);
                }
            }
        })

    },
    fetdataProductSellByOneCategoryLoadmore: async function (page, idcate, sortby) {
        _incallback = true;
        var gethtml = await axios.get('/Search/GetProductByCategory?isPromotion=true&loadmore=true&idcate=' + idcate + '&page=' + page + '&sortby=' + sortby);

        $("#ListProduct").append(gethtml.data);
        _incallback = false;
        pageIndex++;
        loadnEvent_init.loadAddCart();
    },
    fetListDataPost: async function (page, idcategory, text) {

        var gethtml = await axios.get('/Search/GetListPost?page=' + page + '&text=' + text + '&idcategory=' + idcategory);
        if (gethtml.data.isValid) {
            $("#dataPost").html(gethtml.data.html);
        }
    }
}
async function loadatahtmlsell(sel, txtcate, page, sortby = "", slug = "") {
    var elmentparent = $(sel).parents(".index-block-product");
    var gethtml = await axios.get('/Search/GetProductByCategory?isPromotion=true&page=' + page + "&txtcategory=" + encodeURI(txtcate) + "&sortby=" + sortby + "&slugcate=" + slug);
    // elmentparent.parent().html(gethtml.data);
    $(gethtml.data).insertAfter(elmentparent);
    //elmentparent.after(gethtml.data);
    loadisCheck();
    setTimeout(() => {
        loadnEvent_init.loadevenradiocheck(page);
    }, "500");
    elmentparent.remove();
    loadevent.load_sort_productmobi(); // load sự kiện sort
    loadnEvent_init.loadAddCart();
}
var commonfunc = {
    cancelOrder: function (sel, isdetailt = 0) {
        var form = $(sel).parent("form#form_delete")[0];
        Swal.fire({
            title: 'Bạn có chắc chắn muốn hủy không?',
            input: 'textarea',
            inputPlaceholder: 'Vui lòng nhập lý do bạn muốn hủy...',
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
                                if (isdetailt == 1) {
                                    location.reload();
                                }
                                else if (res.loadTable) {
                                    dataTableOut.ajax.reload(null, false);
                                }
                                else if (res.loadTreeview) {
                                    loadData()
                                }
                                else {
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
    }
}

var popupResult = {
    success: function (result, title) {

        Swal.fire({
            icon: 'success',
            title: (title != "" ? title : "Thông báo"),
            html: result,
            // text: html,
            //  timer: 3000,
            allowOutsideClick: true,
            showConfirmButton: false,
            showCancelButton: true,
            cancelButtonText:
                '<i class="fa fa-window-close"></i> Đóng',
        });
    },
    error: function (result, title) {

        Swal.fire({
            icon: 'error',
            title: (title != "" ? title : "Thông báo"),
            html: result,
            // text: html,
            allowOutsideClick: true,
            showConfirmButton: false,
            showCancelButton: true,
            cancelButtonText:
                '<i class="fa fa-window-close"></i> Đóng',
        });
    },
    info: function (result, title) {
        Swal.fire({
            icon: 'info',
            title: (title != "" ? title : "Thông báo"),
            html: result,
            // text: html,
            allowOutsideClick: true,
            showConfirmButton: false,
            showCancelButton: true,
            cancelButtonText:
                '<i class="fa fa-window-close"></i> Đóng',
        });
    },
    warning: function (result, title) {
        Swal.fire({
            icon: 'warning',
            title: (title != "" ? title : "Thông báo"),
            html: result,
            // text: html,
            allowOutsideClick: true,
            showConfirmButton: false,
            showCancelButton: true,
            cancelButtonText:
                '<i class="fa fa-window-close"></i> Đóng',
        });
    }
}
setTimeout(function () {
    $(".alert-success").hide(1000);
    $(".alert-danger").hide(1000);
}, 3000);
toastr.options = {
    "closeButton": true,
    "newestOnTop": false,
    "progressBar": true,
    "positionClass": "toast-top-right",
    "preventDuplicates": false,
    "onclick": null,
    "showDuration": "300",
    "hideDuration": "1000",
    "timeOut": "5000",
    "extendedTimeOut": "1000",
    "showEasing": "swing",
    "hideEasing": "linear",
    "showMethod": "fadeIn",
    "hideMethod": "fadeOut"
}

var remove_item_cart = $(".remove-item-cart");
var btn_minus = $(".mycart .btn-minus");
var btn_plus = $(".mycart .btn-plus");
var countCart = $(".count_item_pr");

var btn_login = $("#btn_login");
var selectProductType = $("#ProductType");
var txtsearchindex = $("#txtsearchindex");
var consultation = $("#CodeConsultation");
var btnsearch = $("#btnsearch");
var btnUpdateimgavata = $(".edit_imgavata");
var addcomment = $(".addcomment button");
var replycomment = $(".answerComment .btn-reply");

var _myModalCustom = $("#myModalCustom");
var _bodymodalCustom = $(".bodymodalCustom");
var _listcategoryindetailpost = $(".detail-post .list-category");
//var btnaddcart = document.getElementById("btn-addCart");
var loginby = $(".login-by");
// Get the modal
var myModalCustom = document.getElementById("myModalCustom");

// Get the button that opens the modal
//var btn = document.getElementById("myBtn");

// Get the <span> element that closes the modal
var span = document.getElementsByClassName("closemodalCustom")[0];

// When the user clicks the button, open the modal 
//btn.onclick = function () {
//    myModalCustom.style.display = "block";
//}

// When the user clicks on <span> (x), close the modal
span.onclick = function () {
    myModalCustom.style.display = "none";
}

// When the user clicks anywhere outside of the modal, close it
window.onclick = function (event) {
    if (event.target == myModalCustom) {
        myModalCustom.style.display = "none";
    }
}
function ChangeToSlug(title) {
    var slug;
    //Đổi chữ hoa thành chữ thường
    slug = title.toLowerCase();

    //Đổi ký tự có dấu thành không dấu
    slug = slug.replace(/á|à|ả|ạ|ã|ă|ắ|ằ|ẳ|ẵ|ặ|â|ấ|ầ|ẩ|ẫ|ậ/gi, 'a');
    slug = slug.replace(/é|è|ẻ|ẽ|ẹ|ê|ế|ề|ể|ễ|ệ/gi, 'e');
    slug = slug.replace(/i|í|ì|ỉ|ĩ|ị/gi, 'i');
    slug = slug.replace(/ó|ò|ỏ|õ|ọ|ô|ố|ồ|ổ|ỗ|ộ|ơ|ớ|ờ|ở|ỡ|ợ/gi, 'o');
    slug = slug.replace(/ú|ù|ủ|ũ|ụ|ư|ứ|ừ|ử|ữ|ự/gi, 'u');
    slug = slug.replace(/ý|ỳ|ỷ|ỹ|ỵ/gi, 'y');
    slug = slug.replace(/đ/gi, 'd');
    //Xóa các ký tự đặt biệt
    slug = slug.replace(/\`|\~|\!|\@|\#|\||\$|\%|\^|\&|\*|\(|\)|\+|\=|\,|\.|\/|\?|\>|\<|\'|\"|\:|\;|_/gi, '');
    //Đổi khoảng trắng thành ký tự gạch ngang
    slug = slug.replace(/ /gi, "-");
    //Đổi nhiều ký tự gạch ngang liên tiếp thành 1 ký tự gạch ngang
    //Phòng trường hợp người nhập vào quá nhiều ký tự trắng
    slug = slug.replace(/\-\-\-\-\-/gi, '-');
    slug = slug.replace(/\-\-\-\-/gi, '-');
    slug = slug.replace(/\-\-\-/gi, '-');
    slug = slug.replace(/\-\-/gi, '-');
    //Xóa các ký tự gạch ngang ở đầu và cuối
    slug = '@' + slug + '@';
    slug = slug.replace(/\@\-|\-\@|\@/gi, '');
    //In slug ra textbox có id “slug”
    return slug;
}
// add even button
Number.prototype.format = function (n, x, s, c) {
    var re = '\\d(?=(\\d{' + (x || 3) + '})+' + (n > 0 ? '\\D' : '$') + ')',
        num = this.toFixed(Math.max(0, ~~n));

    return (c ? num.replace('.', c) : num).replace(new RegExp(re, 'g'), '$&' + (s || ','));
};

remove_item_cart.click(function () {
    var iditem = $(this);
    $.ajax({
        type: "POST",
        data: {
            IdItemCart: iditem.data("id")
        },
        dataType: "JSON",
        url: "/Cart/RemoveItemCart",
        beforeSend: function () {
        },
        success: function (data) {

            if (data.isValid) {
                countCart.html(data.data.quantity);
                let total = data.data.amount.format(0, 3, '.', ',');
                $(".value-total").html(total + " vnđ");
                $(".value-amount").html(total + " vnđ");
                iditem.parents(".item-cart").remove();
            }
            else if (data.login == true) {
                popupResult.warning(data.mess);
            }
            else {
                toastr.error(data.mess);
            }
        }
    });

});

btn_minus.click(function () {

    var sel = $(this);
    sel.attr("disabled", "disabled");
    sel.parent().find("input").attr("disabled", "disabled");
    var result = sel.parent().find("#qty");
    var amount = sel.parents(".wrapper-title").find(".text-amount");
    var price = sel.parent().find("#qty").data("price");
    var idpro = sel.parent().find("#qty").data("id");
    let qty = parseInt(result.val());
    if (!isNaN(qty) & qty > 1) {
        let addVl = qty - 1;
        result.val(addVl);
        let total = parseInt(price) * addVl;
        let format = total.format(0, 3, '.', ',');
        amount.html(format);
        addCart(idpro, addVl, false);
    } else {
        return false;
    }
    sel.removeAttr("disabled");
    sel.parent().find("input").removeAttr("disabled");
});

btn_plus.click(function () {
    var sel = $(this);
    sel.attr("disabled", "disabled");
    sel.parent().find("input").attr("disabled", "disabled");
    var result = sel.parent().find("#qty");
    var amount = sel.parents(".wrapper-title").find(".text-amount");
    var price = sel.parent().find("#qty").data("price");
    var idpro = sel.parent().find("#qty").data("id");
    let qty = parseInt(result.val());
    if (!isNaN(qty)) {
        //  result.value--; 
        let addVl = qty + 1;
        result.val(addVl);
        let total = parseInt(price) * addVl;
        let format = total.format(0, 3, '.', ',');
        amount.html(format);
        addCart(idpro, addVl, false);
    } else {
        return false;
    }
    sel.removeAttr("disabled");
    sel.parent().find("input").removeAttr("disabled");
});



replycomment.click(function () {
    var sel = $(this);
    if (sel.parent().find("textarea").length == 0) {
        let idpro = sel.data("pro");
        let pattenid = sel.data("pattenid");
        let fullname = sel.data("name");
        html = `<div class="ele-content-reply mt-2">
                <textarea class="form-control txtreply" rows="3" id="commentproduct" placeholder="Nhập nội dung của bạn!"></textarea>
                <div class="mar-top clearfix">
                   <button class="btn btn-sm btn-primary pull-right btn-replycomment" data-id-page-comment="`+ idpro + `"  data-pattenid="` + pattenid + `" type="button"><i class="fa fa-pencil fa-fw"></i> Gửi bình luận</button>
                   <button class="btn btn-sm btn-danger pull-right btn-cancelreplycomment mr-3"  type="button"><i class="fa fa-ban fa-fw"></i> Hủy bỏ</button>
                </div>
             </div>`;
        $(html).insertAfter(sel);
        sel.parent().find("textarea").focus();
        sel.parent().find("textarea").val("#" + fullname + "  ");
        loadevent.eventCancelComment();
        loadevent.eventReplyComment();

    } else {

    }
})
addcomment.click(async function () {

    var sel = $(this);
    idproduct = sel.data("id-page-comment");
    let htmlform = `<form>
                    <div class="form-group">
                        <p class="pmd-lb">Tên: <span class="required"><sup>*</sup></span></p>
                        <input type="text" class="form-control" name="txtNameRating" id="txtNameRating" aria-describedby="emailHelp" placeholder="Nhập họ và tên" value="">
                        <p class="m-err required mt-1" style="display: none;">Tên phải có 2 ký tự trở lên</p>
                    </div>
                    <div class="form-group">
                        <p class="pmd-ln2">Để nhận thông báo khi có trả lời. Hãy nhập email và số điện thoại (Không bắt buộc)</p>
                        <p class="pmd-lb">Email:</p><input type="text" class="form-control" name="txtEmailRating" id="txtEmailRating" placeholder="Nhập email" value="">
                        <p class="m-err required mt-1" style="display: none;">Nhập đúng định dạng email</p>
                    </div>
                    <div class="form-group">
                        <p class="pmd-lb">Số điện thoại:</p>
                        <input type="text" class="form-control" name="txtPhoneRating" id="txtPhoneRating" placeholder="Nhập số điện thoại" value="">
                        <p class="m-err required mt-1" style="display: none;">Số điện thoại không hợp lệ</p>
                    </div>
                </form>`;
    var commentproduct = $(".addcomment textarea").val();
    if (commentproduct.trim() == 0) {
        toastr.error("Vui lòng nhập nội dung!");
        return false;
    }
    else {
        var checklogin = await axios.get('/Account/CheckLogin');
        if (!checklogin.data.isValid) {
            Swal.fire({
                title: 'Cập nhật thông tin!',
                // input: 'textarea',
                // inputPlaceholder: 'Vui lòng nhập lý do bạn muốn hủy...',
                html: htmlform,
                //icon: 'warning',
                animation: "zoomIn",
                showCancelButton: true,
                showClass: {
                    popup: 'popup-add-comment animate__animated animate__zoomIn'
                },
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Gửi nhận xét',
                cancelButtonText: 'Hủy bỏ',
                showLoaderOnConfirm: true,
                //inputValidator: (value) => {
                //    if (!value) {
                //        return 'Vui lòng nhập lý do!'
                //    }
                //},
                didRender: () => {

                    $('input#txtNameRating').on("input", function () {
                        var dInput = this.value;
                        if (dInput.length < 2) {
                            $(this).parent().find(".m-err").show();
                        } else {
                            $(this).parent().find(".m-err").hide();
                        }
                    });
                    var pattern = /^\b[A-Z0-9._%-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\b$/i;
                    $('#txtEmailRating').on("input", function () {
                        var dInput = this.value;
                        if (!pattern.test(dInput) && dInput.length > 0) {
                            $(this).parent().find(".m-err").show();
                        } else {
                            $(this).parent().find(".m-err").hide();
                        }
                    });
                },
                preConfirm: async (text) => {
                    loadingStart();

                    txtNameRating = $('#txtNameRating').val();
                    txtEmailRating = $('#txtEmailRating').val();
                    txtPhoneRating = $('#txtPhoneRating').val();
                    if (txtNameRating.trim().length == 0) {
                        $('#txtNameRating').parent().find(".m-err").show();
                        //swal.showValidationMessage("Vui lòng nhập tên của bạn!"); // Show error when validation fails.
                        return false;
                        // swal.enableConfirmButton(); // Enable the confirm button again.
                    }//
                    //swal.resetValidationMessage();
                    // else {
                    // Reset the validation message.
                    try {
                        // var fomrdata = new FormData(form);
                        //  fomrdata.append('content', text);
                        var datajson = {
                            IdProduct: idproduct,
                            Comment: commentproduct,
                            CusName: txtNameRating,
                            CusEmail: txtEmailRating,
                            CusPhone: txtPhoneRating
                        };
                        await addCommentproductAsync(datajson).then(
                            function (value) {
                                if (value) {
                                    popupResult.success("Bình luận thành công, hệ thống đang kiểm tra và sẽ phản hồi lại sau!");
                                } else {
                                    popupResult.error("Bình luận thất bại, bạn vui lòng thử lại sau!");
                                }
                                loadingStop();
                            }
                        );

                    } catch (ex) {

                        console.log(ex);
                        loadingStop()

                    }
                    // }

                },
                // allowOutsideClick: () => !Swal.isLoading()
            })

        }
        else {
            loadingStart();
            var datajson = {
                IdProduct: idproduct,
                Comment: commentproduct
            };
            await addCommentproductAsync(datajson).then(
                function (value) {
                    if (value) {
                        popupResult.success("Bình luận thành công, hệ thống đang kiểm tra và sẽ phản hồi lại sau!");
                    } else {
                        popupResult.error("Bình luận thất bại, bạn vui lòng thử lại sau!");
                    }
                    loadingStop();
                }
            );
        }

    }
});

async function addCommentproductAsync(datajson) {
    value = false;
    const response = await axios({
        method: 'post',
        url: '/API/Handling/AddCommentProduct',
        headers: {
            'Content-Type': `multipart/form-data`,
        },
        data: datajson
    });
    value = response.data.isValid;


    //await axios({
    //    method: 'post',
    //    url: '/API/Handling/AddCommentProduct',
    //    data: datajson
    //})
    //    .then(function (response) {
    //        
    //        value = response.isValid;
    //    })
    //    .catch(function (error) {
    //        console.log(error);
    //    });
    //$.ajax({
    //    type: 'POST',
    //    global: false,
    //    async: false,
    //    url: "/API/Handling/AddCommentProduct",
    //    data: datajson,
    //    success: function (res) {
    //        value = res.isValid;
    //    },
    //    error: function (err) {
    //        console.log(err);
    //        return false;
    //        // popupResult.error("Bình luận thất bại, bạn vui lòng thử lại sau!");
    //    }
    //});
    return value;
}



$(".show-seach-home input").focus(function () {
    $(".form-search").toggleClass("fixed_top");
    $("html").css("overflow", "hidden");
});

$(".show-seach-home .input-group-append").click(function () {
    $(".form-search").toggleClass("fixed_top");
    $("html").css("overflow", "hidden");
});
$(".title-seach img").click(function () {
    $(".form-search").toggleClass("fixed_top");
    $("html").css("overflow", "");
});
$("#formSalesRegistration .btn-update").click(async function () {

    let type = $(this).data("type");
    var secret = $(this).data("secret");
    var iddata = $(this).data("id");
    if (type == "email") {

        const { value: email } = await Swal.fire({
            title: 'Cập nhật địa chỉ email',
            input: 'email',
            showCancelButton: true,
            confirmButtonText: 'Cập nhật',
            cancelButtonText: 'Hủy bỏ',
            inputLabel: 'Email của bạn',
            inputPlaceholder: 'Nhập email của bạn',
            inputValidator: (value) => {
                if (!value) {
                    return 'Vui lòng nhập đúng định dạng email'
                }
            }
        })
        if (email) {
            $.ajax({
                type: 'POST',
                url: "/Account/ComplementaryEmail",
                //async: false, 
                data: {
                    email: email,
                    Id: iddata,
                    modal: true,
                },
                success: function (res) {
                    if (res.isValid) {
                        //  $(".text-email .value").html(email);
                        //$("#Email").val(email);
                        //$(".text-email .value").removeClass("no-value");
                        popupResult.success(res.html);
                    } else {
                        popupResult.error(res.html);
                    }
                },
                error: function (err) {
                    popupResult.error(err);
                }
            })
        }
    }
    else if (type == "phone") {
        // Swal.fire({
        //    title: 'Cập nhật số điện thoại',
        //    html: html,
        //    showConfirmButton: false,
        //    showCloseButton: true,
        //    didRender: () => {
        //        UploadAvatar();
        //    },

        //})
        const { value: phone } = await Swal.fire({
            title: 'Cập nhật số điện thoại',
            input: 'text',
            showCancelButton: true,
            confirmButtonText: 'Cập nhật',
            cancelButtonText: 'Hủy bỏ',
            inputLabel: 'Số điện thoại của bạn',
            inputPlaceholder: 'Nhập số điện thoại của bạn',
            inputValidator: (value) => {
                let text = value.replaceAll("_", "");
                if (!value || text.length < 14) {
                    return 'Vui lòng nhập đúng số điện thoại'
                }
            },
            customClass: {
                input: 'txt-phonenumer',
            },
            didRender: () => {
                $(".txt-phonenumer").inputmask("(999) 999-9999");
            },
        })
        if (phone) {
            $.ajax({
                type: 'POST',
                url: "/Account/UpdatePhone",
                //async: false, 
                data: {
                    phone: phone,
                    id: iddata
                },
                success: function (res) {
                    if (res.isValid) {
                        $(".text-phone .value").html(phone);
                        $("#PhoneNumber").val(phone);
                        $(".text-phone .value").removeClass("no-value");
                        popupResult.success(res.html);

                    } else {
                        popupResult.error(res.html);
                    }
                },
                error: function (err) {
                    popupResult.error(err);
                }
            })
        }
    }
});
var loadevent = {
    eventReplyComment: function () {
        $(".answerComment .btn-replycomment").click(async function () {
            var sel = $(this);
            idproduct = sel.data("id-page-comment");
            idpattern = sel.data("pattenid");

            var commentproduct = sel.parents(".ele-content-reply").find("textarea#commentproduct").val();
            let htmlform = `<form>
                                <div class="form-group">
                                    <p class="pmd-lb">Tên: <span class="required"><sup>*</sup></span></p>
                                    <input type="text" class="form-control" name="txtNameRating" id="txtNameRating" aria-describedby="emailHelp" placeholder="Nhập họ và tên" value="">
                                    <p class="m-err required mt-1" style="display: none;">Tên phải có 2 ký tự trở lên</p>
                                </div>
                                <div class="form-group">
                                    <p class="pmd-ln2">Để nhận thông báo khi có trả lời. Hãy nhập email và số điện thoại (Không bắt buộc)</p>
                                    <p class="pmd-lb">Email:</p><input type="text" class="form-control" name="txtEmailRating" id="txtEmailRating" placeholder="Nhập email" value="">
                                    <p class="m-err required mt-1" style="display: none;">Nhập đúng định dạng email</p>
                                </div>
                                <div class="form-group">
                                    <p class="pmd-lb">Số điện thoại:</p>
                                    <input type="text" class="form-control" name="txtPhoneRating" id="txtPhoneRating" placeholder="Nhập số điện thoại" value="">
                                    <p class="m-err required mt-1" style="display: none;">Số điện thoại không hợp lệ</p>
                                </div>
                            </form>`;

            if (commentproduct.trim() == 0) {
                toastr.error("Vui lòng nhập nội dung!");
                return false;
            }
            else {
                var checklogin = await axios.get('/Account/CheckLogin');
                if (!checklogin.data.isValid) {
                    Swal.fire({
                        title: 'Cập nhật thông tin!',
                        // input: 'textarea',
                        // inputPlaceholder: 'Vui lòng nhập lý do bạn muốn hủy...',
                        html: htmlform,
                        //icon: 'warning',
                        animation: "zoomIn",
                        showCancelButton: true,
                        showClass: {
                            popup: 'popup-add-comment animate__animated animate__zoomIn'
                        },
                        confirmButtonColor: '#3085d6',
                        cancelButtonColor: '#d33',
                        confirmButtonText: 'Gửi nhận xét',
                        cancelButtonText: 'Hủy bỏ',
                        showLoaderOnConfirm: true,
                        //inputValidator: (value) => {
                        //    if (!value) {
                        //        return 'Vui lòng nhập lý do!'
                        //    }
                        //},
                        didRender: () => {

                            $('input#txtNameRating').on("input", function () {
                                var dInput = this.value;
                                if (dInput.length < 2) {
                                    $(this).parent().find(".m-err").show();
                                } else {
                                    $(this).parent().find(".m-err").hide();
                                }
                            });
                            var pattern = /^\b[A-Z0-9._%-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\b$/i;
                            $('#txtEmailRating').on("input", function () {
                                var dInput = this.value;
                                if (!pattern.test(dInput) && dInput.length > 0) {
                                    $(this).parent().find(".m-err").show();
                                } else {
                                    $(this).parent().find(".m-err").hide();
                                }
                            });
                        },
                        preConfirm: async (text) => {

                            txtNameRating = $('#txtNameRating').val();
                            txtEmailRating = $('#txtEmailRating').val();
                            txtPhoneRating = $('#txtPhoneRating').val();
                            if (txtNameRating.trim().length == 0) {
                                $('#txtNameRating').parent().find(".m-err").show();
                                //swal.showValidationMessage("Vui lòng nhập tên của bạn!"); // Show error when validation fails.
                                return false;
                                // swal.enableConfirmButton(); // Enable the confirm button again.
                            }//
                            //swal.resetValidationMessage();
                            // else {
                            // Reset the validation message.
                            try {
                                // var fomrdata = new FormData(form);
                                //  fomrdata.append('content', text);

                                var datajson = {
                                    IdPattern: idpattern,
                                    IdProduct: idproduct,
                                    Comment: commentproduct,
                                    CusName: txtNameRating,
                                    CusEmail: txtEmailRating,
                                    CusPhone: txtPhoneRating
                                };
                                await addCommentproductAsync(datajson).then(
                                    function (value) {
                                        if (value) {
                                            popupResult.success("Bình luận thành công, hệ thống đang kiểm tra và sẽ phản hồi lại sau!");
                                        } else {
                                            popupResult.error("Bình luận thất bại, bạn vui lòng thử lại sau!");
                                        }
                                    }
                                );

                            } catch (ex) {

                                console.log(ex)

                            }
                            // }

                        },
                        // allowOutsideClick: () => !Swal.isLoading()
                    })

                } else {
                    var datajson = {
                        IdProduct: idproduct,
                        IdPattern: idpattern,
                        Comment: commentproduct
                    };
                    await addCommentproductAsync(datajson).then(
                        function (value) {
                            if (value) {
                                popupResult.success("Bình luận thành công, hệ thống đang kiểm tra và sẽ phản hồi lại sau!");
                            } else {
                                popupResult.error("Bình luận thất bại, bạn vui lòng thử lại sau!");
                            }
                        }
                    );
                }

            }
        })
    },
    eventCancelComment: function () {
        $(".answerComment .btn-cancelreplycomment").click(function () {
            $(this).parents("div.ele-content-reply").remove();
        })
    },
    load_tab_show_incategoryproduct: function (type = 0) {
        $(".index-block-product .button_show_tab").click(function () {
            var sel = $(this);
            sel.toggleClass("active");
            sel.parent().find(".viewmore").toggleClass("show");
        });
        //$(".index-block-product .button_show_sortby").click(function () {

        //    $("html").css("overflow", "hidden");
        //    var sel = $(this);
        //    //sel.toggleClass("active");

        //    if ($(".block-product .row-order").hasClass("d-none")) {
        //        $(".block-product .row-order").toggleClass("d-none");
        //    }
        //    if (!$(".block-product .row-order").hasClass("show")) {
        //        $(".block-product .row-order").toggleClass("show");
        //    }


        //});
        loadevent.load_sort_productmobi(type);

    },
    load_sort_productmobi: function (type = 0) {
        $(".index-block-product .button_show_sortby").click(function () {
            var elehml = document.querySelector("html");


            // $("html").css("overflow", "hidden");
            var sel = $(this);
            //sel.toggleClass("active");

            if (sel.parents(".index-block-product").find(".row-order").hasClass("d-none")) {
                sel.parents(".index-block-product").find(".row-order").toggleClass("d-none");
            }
            if (!sel.parents(".index-block-product").find(".row-order").hasClass("show")) {
                sel.parents(".index-block-product").find(".row-order").toggleClass("show");
            }
            if ($(window).width() < 769) {

                sel.parents(".index-block-product").find(".row-order").click(function () {
                    var _sel = $(this);
                    _sel.addClass("d-none");
                    _sel.removeClass("show");
                    _sel.parents("html").css("overflow", "");
                    $("html").css("overflow", "");
                    elehml.setAttribute("style", "");
                    // document.getElementsByTagName("html")[0].setAttribute("style", "");
                });

            }

        });
        $(".index-block-product .button_show_search").click(function () {
            let datatype = parseInt($(this).data("type"));
            if (datatype != 1) {
                datatype = 0;
            }

            if ($(window).width() < 769) {
                $('input#price').iCheck('destroy');
                $("html").css("overflow", "hidden");
                d = document.createElement('div');
                $(d).addClass("show-popup-search")
                    .html("<div class='close-popup-search'><button class='btn btn-primary'>Đóng</button></div>")
                    .append($("#sort-price").html())
                    .append("<div class='close-popup-success'><button class='btn btn-success btn-block'>Xem kết quả</button></div>")
                    .appendTo("body"); //main div
                $(".close-popup-search").click(function () {
                    $(".show-popup-search").remove();
                    $("html").css("overflow", "");
                });

                $('input#price').iCheck('uncheck');
                var url_string = window.location.href;
                var url = new URL(url_string);
                var params = new URLSearchParams(url.search);
                var idorderprice = params.get("idPrice");
                idPrice = idorderprice;
                $('.show-popup-search input[type=radio]').each(function () {

                    if ($(this).val() == idorderprice) {
                        $(this).iCheck('check');
                    }
                });
                loadisCheck();
                $('.show-popup-search input#price').on('ifChecked', function (event) {
                    //SortAndSeartInCategoryProduct();
                    // $(".close-popup-search").trigger("click");

                });
                $(".close-popup-success").click(function () {
                    $("html").css("overflow", "");
                    SortAndSeartInCategoryProduct(datatype);

                    $(".close-popup-search").trigger("click");

                });

            }

        });
    },

    loadautocomplete: function () {
        //https://api.jqueryui.com/autocomplete/#method-_resizeMenu
        jQuery.ui.autocomplete.prototype._resizeMenu = function () {
            var ul = this.menu.element;
            ul.outerWidth(this.element.outerWidth());
        }
    },
    eventcollapsecategorypost: function () {
        $(".detail-post .list-category i").click(function () {
            //$(this).parent().find(".dropdownlist").toggleClass("show", 1000);
            if ($(this).parents("li").hasClass("active")) {
                if ($(this).children("i").hasClass("fa-angle-down")) {
                    $(this).parent().parent().find(".dropdownlist").show("slow");

                    $(this).addClass("fa-angle-up");
                    $(this).removeClass("fa-angle-down");
                } else {
                    $(this).parent().parent().find(".dropdownlist").hide("slow");
                    $(this).addClass("fa-angle-down");
                    $(this).removeClass("fa-angle-up");
                }
                $(this).parent().parent().removeClass("active");

            } else {
                $(this).closest(".list-category").find("li.active").find("span").children("i").removeClass("fa-angle-up").addClass("fa-angle-down");
                $(this).closest(".list-category").find("li.active").find(".dropdownlist").hide("slow");
                $(this).closest(".list-category").find("li.active").removeClass("active");

                $(this).parent().parent("li").addClass("active");
                $(this).parent().parent().find(".dropdownlist").show("slow");
                $(this).addClass("fa-angle-up");
                $(this).removeClass("fa-angle-down");
            }

            //end reset

        });

        $(".detail-post .list-category .select").parents("ul").parent("li").addClass("select").addClass("active");
        $(".detail-post .list-category .select").parents("ul").parent("li").find(".dropdownlist").show();
        $(".detail-post .list-category .select").parents("ul").parent("li").parent("span").children("i").removeClass("fa-angle-dow").addClass("fa-angle-up");
    },
    eventScrollPostCategory: function () {
        widthr_r = $('.detail-post .right-ct').width();
        height_h = document.getElementById("myheader").offsetHeight;
        if (window.matchMedia('(min-width: 992px)').matches) {
            $(window).bind('scroll', function () {

                if ($(window).scrollTop() > 160) {
                    $('.detail-post .left-ct .card').css('width', widthr_r + "px");
                    $('.detail-post .left-ct').addClass('fixed-bar');

                    $('.detail-post .right-ct').addClass('fixed-bar');
                    $('.detail-post .right-ct .card').css('width', widthr_r + "px");

                    $('.detail-post .body.fixed-bar').css('top', height_h + "px");
                } else {
                    $('.detail-post .left-ct').removeClass('fixed-bar');
                    $('.detail-post .right-ct').removeClass('fixed-bar');

                    $('.detail-post .right-ct').css('width', "");
                    $('.detail-post .left-ct .card').css('width', "");

                }
            });

        }

    }

}
function loadWard(idDistrict, IdWard = 0) {
    let url_seri = "/API/Handling/GetDataWartByIdDistrict?idDistrict=" + idDistrict;
    loaddataSelect2(url_seri, "#IdWard", IdWard, "Chọn phường/xã");
}
function loadEventRegisterCustomer() {
    let idCity = $("#IdCity").val();

    let url = "/API/Handling/GetDataDistrictByIdCity?idcity=" + idCity;
    loaddataSelect2(url, "#IdDistrict", 0, "Chọn quận/chuyện");

    $("#IdCity").select2({
        placeholder: "Chọn tỉnh thành",
        allowClear: true,
        language: {
            noResults: function () {
                return "Không tìm thấy dữ liệu";
            }
        },
    }).on('change', function (e) {
        $('#IdDistrict').empty().append('<option selected="selected" value=""></option>');
        $('#IdWard').empty().append('<option selected="selected" value=""></option>');
        let url_seri = "/API/Handling/GetDataDistrictByIdCity?idcity=" + this.value;
        loaddataSelect2(url_seri, "#IdDistrict", 0, "Chọn quận/chuyện", false);
        $('#IdDistrict').trigger("change");
    });

    $("#IdDistrict").change(function () {
        $('#IdWard').empty().append('<option selected="selected" value=""></option>');
        loadWard($(this).val());
    });
}

function loadEventButonlogin() {
    $(".btn_login").click(function () {
        LoginFom();
    });

    $(".btn-registration").click(function () {
        registrationFom();
    });
}


function loginPostAjax() {
    $.ajax({
        type: 'POST',
        url: "/Account/Login",
        data: $('#loginformLoginPopup').serialize(),
        //contentType: false,
        //processData: false,
        success: function (res) {
            if (res.isValid) {
                $('#loginformLoginPopup').modal('hide');
                window.location.reload();
            } else {
                Ladda.stopAll();
            }
        },
        error: function (err) {
            console.log(err)
        }
    })
}
// hàm đăng ký post
function registrationAjax(sel) {
    // $("#RegisterModal").modal('hide');

    $.ajax({
        type: 'POST',
        url: "/Account/Register",
        data: $('#Registerform').serialize(),

        //contentType: false,
        //processData: false,
        success: function (res) {

            if (res.isValid) {
                $('#RegisterModal').modal('hide');
                // $('#RegisterModal').remove();
                let text = "Chúc mừng bạn đã đăng ký tài khoản thành công. Vui lòng vào Email để xác nhận và tiếp tục sử dụng dịch vụ";
                jQueryModal.MessCustom("Đăng ký thành công", text, "Đóng", function () {
                    jQueryModal.MessCustomCloes;
                })

            } else {
                Ladda.stopAll();
            }
        },
        error: function (err) {
            console.log(err)
        }
    })
}

var minlength6 = "Hãy nhập ít nhất 6 ký tự";
// hàm load fomr đăng ký
function registrationFom() {
    $.ajax({
        type: 'GET',
        url: "/Account/Register?popup=true",
        // contentType: false,
        // processData: false,
        success: function (res) {
            if (res.isValid) {
                $("#RegisterModal").remove();
                $("body").append(res.html);
                $("#RegisterModal").modal('show');
                $('#RegisterModal').on('hidden.bs.modal', function (e) {
                    $('#RegisterModal').remove();
                });
                eventValidateFormRegister();
                $("input#Email").change(function () {
                    let a = $(this).value;
                    $.ajax({
                        type: 'POST',
                        url: '/Account/ValidateEmail',
                        data: {
                            Email: $("input#Email").val()
                        },
                        success: function (res) {

                            if (res.isValid) {
                                $("#UserName").val(res.name);
                                $("#UserName").focus();
                            }
                            $("#Email").focus();
                        },
                        error: function (err) {
                            console.log(err)
                        }
                    })
                });

                $("input.inputmask").inputmask();
                $('#PhoneNumber').inputmask("(999) 999-9999");
                loaddaterangepicker();
                $("#Registerform button").click(function () {
                    if ($("#Registerform").valid()) {
                        registrationAjax(this);
                    }
                });
            }
        },
        error: function (err) {
            console.log(err)
        }
    });
}


function eventValidateFormRegister(idform = "Registerform") {
    _form = $("#" + idform);
    _form.validate({
        rules: {
            "UserName": {
                required: true,
                minlength: 6
            },
            "Name": {
                required: true,
                minlength: 6
            },
            "Email": {
                required: true,
                email: true
            },
            "Address": {
                required: true,
                minlength: 6
            },
            "PhoneNumber": {
                required: true,
                minlength: 14
            },
            "Password": {
                required: true,
                minlength: 6
            },
            "ConfirmPassword": {
                required: true,
                equalTo: "#Password",
                minlength: 6

            }
        },
        messages: {
            "UserName": {
                required: "Vui lòng nhập tên đăng nhập",
                minlength: minlength6
            },
            "Name": {
                required: "Vui lòng nhập họ và tên",
                minlength: minlength6
            },
            "Address": {
                required: "Vui lòng nhập địa chỉ",
                minlength: minlength6
            },
            "Email": {
                required: "Vui lòng nhập địa chỉ email",
                email: "Email nhập lại không khớp"
            },
            "PhoneNumber": {
                required: "Vui lòng nhập số điện thoại",
                minlength: "Vui lòng nhập đúng định dạng"
            },
            "Password": {
                required: "Vui lòng nhập mật khẩu",
                minlength: minlength6
            },
            "ConfirmPassword": {
                equalTo: "Mật khẩu nhập lại không khớp",
                required: "Vui lòng nhập lại mật khẩu",
                minlength: minlength6
            }
        },

        errorElement: 'span',
        errorPlacement: function errorPlacement(error, element) {
            Ladda.stopAll();
            // input.removeAttr('readonly').removeAttr('disabled');
            error.addClass('invalid-feedback');


            var a = element.parents(".form-group").children();
            error.insertAfter(a.last());
            Ladda.stopAll();
            //input.removeAttr('readonly').removeAttr('disabled');

        },
        // eslint-disable-next-line object-shorthand
        highlight: function highlight(element) {
            // let errorClass = "is-invalid";
            $(element).addClass('is-invalid').removeClass('is-valid');
            Ladda.stopAll();

        },
        // eslint-disable-next-line object-shorthand
        unhighlight: function unhighlight(element) {
            // let errorClass = "is-invalid";
            $(element).addClass('is-valid').removeClass('is-invalid');

        },
        submitHandler: function (form) {
            // loadingStart();
            form.submit();
        }
    });
}


function validatefomrlogin(loginfomr = "") {
    _loginfomr = $("#loginform");
    if (loginfomr != "") {
        _loginfomr = $("#" + loginfomr);
    }
    _loginfomr.validate({
        rules: {
            "UserName": {
                required: true,
                minlength: 6
            },
            "Password": {
                required: true,
                minlength: 6
            }
        },
        messages: {
            "UserName": {
                required: "Vui lòng nhập tên đăng nhập",
                minlength: minlength6
            },
            "Password": {
                required: "Vui lòng nhập mật khẩu",
                minlength: minlength6
            }
        },

        errorElement: 'span',
        errorPlacement: function errorPlacement(error, element) {
            Ladda.stopAll();
            // input.removeAttr('readonly').removeAttr('disabled');
            error.addClass('invalid-feedback');


            var a = element.parents(".form-group").children();
            error.insertAfter(a.last());
            Ladda.stopAll();
            //input.removeAttr('readonly').removeAttr('disabled');

        },
        // eslint-disable-next-line object-shorthand
        highlight: function highlight(element) {
            // let errorClass = "is-invalid";
            $(element).addClass('is-invalid').removeClass('is-valid');
            Ladda.stopAll();

        },
        // eslint-disable-next-line object-shorthand
        unhighlight: function unhighlight(element) {
            // let errorClass = "is-invalid";
            $(element).addClass('is-valid').removeClass('is-invalid');

        },
        submitHandler: function (form) {
            // loadingStart();
            form.submit();
        }
    });

}

var errrequired = 'Dữ liệu không được để trống';
var errminlength5 = 'Trường dữ liệu ít nhất 5 ký tự';
// hàm load form đăng nhập
function LoginFom() {
    $.ajax({
        type: 'GET',
        url: "/Account/Login?popup=true",
        // contentType: false,
        // processData: false,
        success: function (res) {
            if (res.isValid) {
                $("#loginModal").remove();
                $("body").append(res.html);
                $("#loginModal").modal('show');

                validatefomrlogin("loginformLoginPopup");

                $('#loginModal').on('hidden.bs.modal', function (e) {
                    $('#loginModal').remove();
                });
                $("#loginformLoginPopup button").click(function () {
                    if ($("#loginformLoginPopup").valid()) {
                        loginPostAjax();
                    }

                });
            }
        },
        error: function (err) {
            console.log(err)
        }
    });
}

var jQueryModal = {
    MessCustom: function (title, content, titlebutton = "Đóng", callback = "") {

        var confirmModal = $(`<div class="popup" data-pd-popup="popupNew">
                        <div class="popup-inner">
                            <img src="../../icon/icon_luantm/success.svg"/>
                            <h1>`+ title + `</h1>
                          <p>`+ content + `</p>
                            <p><a data-pd-popup-close="popupNew" id="okButton" href="#" class="btn btn-danger">`+ titlebutton + `</a></p>
                            <a class="popup-close" data-pd-popup-close="popupNew" href="#"> </a>
                        </div>
                    </div>
                  </div>`);
        let targeted_popup_class = "popupNew";
        $("body").append(confirmModal);
        $('[data-pd-popup="' + targeted_popup_class + '"]').fadeIn(200);
        // confirmModal.modal("show");
        $("body").addClass("popup-open");
        confirmModal.find('#okButton').click(function (event) {
            callback();
            // confirmModal.modal("show");
            $("body").removeClass("popup-open");
            $(this).unbind();
            $('[data-pd-popup="' + targeted_popup_class + '"]').fadeOut(200);
            $(".popup").remove();
            $(this).remove();
            confirmModal.remove();
        });

    },
    MessCustomCloes: function () {
        let targeted_popup_class = "popupNew";
        $('[data-pd-popup="' + targeted_popup_class + '"]').fadeOut(200);
        $(".popup").remove();
    },
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
            '<div class="modal-body">'
            + mess +
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
function loadeventCheckallCart() {

    var triggeredByChild = false;
    var checkalltriggered = false;
    var uncheckalltriggered = false;

    $('#check-all').on('ifChecked', function (event) {
        checkalltriggered = true;

        $('.item-check').each(function () {
            $(this).iCheck('check');
        });
        if (!triggeredByChild) {
            updateSelectArray();
        }
        triggeredByChild = false;
        checkalltriggered = false;
    });

    $('#check-all').on('ifUnchecked', function (event) {
        uncheckalltriggered = true;
        if (!triggeredByChild) {
            $('.item-check').each(function () {
                $(this).iCheck('uncheck');
            });
            updateSelectArray();
        }
        uncheckalltriggered = false;
        triggeredByChild = false;

    });
    // Removed the checked state from "All" if any checkbox is unchecked
    $('.item-check').on('ifUnchecked', function (event) {

        let IdItemCart = $(this).data("id");
        let IdCart = $(this).data("idcart");
        if (!uncheckalltriggered) {

            let list = [];
            list.push(parseInt(IdItemCart));
            updateSelectItemCart(list, false);
        }
        triggeredByChild = true;
        $('#check-all').iCheck('uncheck');

    });

    $('.item-check').on('ifChecked', function (event) {

        let IdItemCart = $(this).data("id");
        let list = [];
        list.push(parseInt(IdItemCart));
        let IdCart = $(this).data("idcart");

        if (!checkalltriggered) {
            updateSelectItemCart(list, true);
        }
        triggeredByChild = true;
        if ($('.item-check').filter(':checked').length == $('.item-check').length) {
            $('#check-all').iCheck('check');
            triggeredByChild = false;
        }
    });
}
function updateSelectArray() {
    var list = $(".item-check:checked").map(function () {
        return parseInt($(this).val());
    }).toArray();

    if (list.length > 0) {
        updateSelectItemCart(list, true, false, true);
    } else {
        updateSelectItemCart(list, false, true);
    }
}

function updateSelectItemCart(IdItemCart, select, removeAll = false, checkAll = false) {
    $.ajax({
        type: "POST",
        data: {
            IdItemCart: IdItemCart,
            Select: select,
            removeAll: removeAll,
            checkAll: checkAll,
        },
        dataType: "JSON",
        url: "/Cart/UpdateItemCart",
        beforeSend: function () {
            //  btnaddcart.attr("disabled", "disabled");
            //   loadingStart("#dataProduct");
        },
        success: function (data) {

            if (data.isValid) {
                countCart.html(data.data.quantity);

                let total = data.data.amount.format(0, 3, '.', ',');
                $(".value-total").html(total + " vnđ");
                $(".value-amount").html(total + " vnđ");

            }
            else if (data.login == true) {
                popupResult.warning(data.mess);
            }
            else {
                toastr.error(data.mess);
            }
        }
    });
}
function confirms(heading, question, cancelButtonTxt, okButtonTxt, callback) {
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
            '<button type="button" class="btn btn-secondary" data-dismiss="modal">' +
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
    confirmModal.on('hide.bs.modal', function () {
        $("div#myModal").empty();
        $(this).unbind();
        $(this).remove();

    });
    confirmModal.modal('show');
}
$(document).ajaxStart(function () {

    $.blockUI({
        message: $('<div class="loader mx-auto">\n <div class="line-scale-pulse-out">\n   <div class="bg-success"></div>\n <div class="bg-success"></div>\n  <div class="bg-success"></div>\n <div class="bg-success"></div>\n                                <div class="bg-success"></div>\n                            </div>\n  <span>Đang xử lý.....</span>                      </div>')
    })
}).ajaxStop(function () {
    Ladda.stopAll();
    $.unblockUI();
});


$("#TableData").DataTable({
    "lengthMenu": [[10, 25, 50, 100, 200, -1], [10, 25, 50, 100, 200, "All"]],
    "pageLength": 10,
    "order": [[0, 'desc']],
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

function LoadDataTable() {
    $("#TableData").DataTable({
        "lengthMenu": [[10, 25, 50, 100, 200, -1], [10, 25, 50, 100, 200, "All"]],
        "pageLength": 10,
        "order": [[0, 'desc']],
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
$(function () {
    var url = window.location;
    /*  $('#sidebar-menu a[href="' + url + '"]').parent('li').addClass('current-page');*/
    $('#menu_portal a').filter(function () {
        return this.href == url;
    }).addClass("active").parents('li.nav-item').addClass('active');
    $('.menu-info-user a').filter(function () {
        return this.href == url;
    }).addClass("active").parent('li').addClass('active');
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
//validate password
$.validator.addMethod("pwcheck",
    function (value, element) {
        let password = value;
        if (!(/^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[@#$%&])(.{8,20}$)/.test(password))) {
            return false;
        }
        return true;
    }, function (value, element) {
        let password = $(element).val();
        if (!(/^(.{8,20}$)/.test(password))) {
            return 'Mật khẩu phải dài từ 8 đến 20 ký tự';
        }
        else if (!(/^(?=.*[A-Z])/.test(password))) {
            return 'Mật khẩu phải chứa ít nhất một chữ hoa.';
        }
        else if (!(/^(?=.*[a-z])/.test(password))) {
            return 'Mật khẩu phải chứa ít nhất một chữ thường';
        }
        else if (!(/^(?=.*[0-9])/.test(password))) {
            return 'Mật khẩu phải chứa ít nhất một chữ số';
        }
        else if (!(/^(?=.*[@#$%&])/.test(password))) {
            return "Mật khẩu phải chứa một trong các ký tự đặc biệt từ @#$%&.";
        }
        return false;
    });
function validatechangepass() {
    var validator = $("#formChangePass").bind("invalid-form.validate", function () {
        // $("#summary").html("Your form contains " + validator.numberOfInvalids() + " errors, see details below.");
    }).validate({
        rules: {
            Email: {
                required: true,
                email: true,
            },
            Password: {
                required: true,
                pwcheck: true
            },
            PasswordOld: {
                required: true,
                minlength: 6
            },
            ConfirmPassword: {
                required: true,
                minlength: 6,
                equalTo: "#Password"
            }
        },
        messages: {
            Email: {
                required: errrequired,
                email: "Email không đúng định dạng",
            },
            Password: {
                required: errrequired,
                pwcheck: "Mật khẩu ít nhất 8 ký tự, có chứa chữ hoa, chữ thường và ký tự đặt biệt"
            },
            PasswordOld: {
                required: errrequired,
                minlength: minlength6
            },
            ConfirmPassword: {
                required: errrequired,
                minlength: minlength6,
                equalTo: "Mật khẩu nhập lại không đúng"
            }

        },
        errorElement: 'span',
        errorPlacement: function errorPlacement(error, element) {
            $(".field-validation-error").hide();
            Ladda.stopAll();
            // input.removeAttr('readonly').removeAttr('disabled');
            error.addClass('invalid-feedback');

            if (element.prop('type') === 'checkbox') {
                error.insertAfter(element.parent('label'));
                Ladda.stopAll();
                // input.removeAttr('readonly').removeAttr('disabled');
            }
            else {
                var a = element.parent();
                a = a.children().last();

                error.insertAfter(a.last());
                Ladda.stopAll();
                // input.removeAttr('readonly').removeAttr('disabled');
            }
        },
        // eslint-disable-next-line object-shorthand
        highlight: function highlight(element) {
            $(element).addClass('error');
            Ladda.stopAll();
            // input.removeAttr('readonly').removeAttr('disabled');
        },
        // eslint-disable-next-line object-shorthand
        unhighlight: function unhighlight(element) {
            $(element).removeClass('error');
            Ladda.stopAll();
        },
        submitHandler: function (form) {
            //alert("s");
            form.submit();
        }
    });
}
$(document).ready(function () {

    $('[data-toggle="tooltip"]').tooltip();
    /*  $.fn.modal.Constructor.prototype.enforceFocus = function () { };*/

    $('.form-image').click(function () { $('#customFile').trigger('click'); });

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
                            $('#form-modal .modal-dialog').addClass(size);
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
            if (!$(form).valid()) {
                Ladda.stopAll();
                return false;
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
                        }
                        else {
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
        confirms("Xác nhận", "Bạn có chắc chắn muốn tiếp tục không?", "<i class='far fa-times-circle'></i> Hủy", "<i class='fas fa-check-circle'></i> Đồng ý", function () {

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
                            }
                            else if (res.loadTreeview) {
                                loadData()
                            }
                            else {
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
$().ready(function () {

});
$(function () {
    var url = window.location;

    $('.app-sidebar__inner a').filter(function () {
        return this.href == url;
    }).addClass('mm-active').parents().parents('ul').addClass("mm-show").addClass('mm-activee');
});
$('#form-modal').on('hidden.bs.modal', function (e) {
    $("div.modal-body").html("");
    $("div.modal-body").empty();
    $(".select2").select2({
        placeholder: "Chọn giá trị",
        allowClear: true,
        language: {
            noResults: function () {
                return "Không tìm thấy dữ liệu";
            }
        },
    });
});
$("input.inputmask").inputmask();


function confirmcustom(sel, title) {
    confirms("Xác nhận", title, "<i class='far fa-times-circle'></i> Hủy", "<i class='fas fa-check-circle'></i> Đồng ý", function () {
        $(sel).closest("form").submit();
        $(sel).form.submit();
    });

    //prevent default form submit event
    return false;
}
var errRegex = 'Vui lòng nhập đúng định dạng';
var emailformat = $("#emailformat").text();

$("#formSalesRegistrationss").validate({
    rules: {
        Name: {
            required: true,
        },
        CusTaxCode: {
            required: true,
        }, Address: {
            required: true,
        },
        Email: {
            required: true,
            regex: /^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$/g,
        }

    },
    messages: {
        Name: {
            required: errrequired,
        },
        CusTaxCode: {
            required: errrequired,
        },
        Address: {
            required: errrequired,
        },
        Email: {
            regex: errRegex,
        }

    },

    errorElement: 'em',
    errorPlacement: function errorPlacement(error, element) {
        Ladda.stopAll();
        // input.removeAttr('readonly').removeAttr('disabled');
        error.addClass('invalid-feedback');

        if (element.prop('type') === 'checkbox') {
            error.insertAfter(element.parent('label'));
            Ladda.stopAll();
            //input.removeAttr('readonly').removeAttr('disabled');
        }
        else {
            var a = element.parent();
            a = a.children().last();

            error.insertAfter(a.last());
            Ladda.stopAll();
            //input.removeAttr('readonly').removeAttr('disabled');
        }
    },
    // eslint-disable-next-line object-shorthand
    highlight: function highlight(element) {
        $(element).addClass('is-invalid').removeClass('is-valid');
        Ladda.stopAll();
        //input.removeAttr('readonly').removeAttr('disabled');
    },
    // eslint-disable-next-line object-shorthand
    unhighlight: function unhighlight(element) {
        $(element).addClass('is-valid').removeClass('is-invalid');
        //Ladda.stopAll();
    },
    submitHandler: function (form) {
        //   alert("s");
        form.submit();
    }
});
$(".select2-2").select2({
    placeholder: "Chọn giá trị",
    allowClear: true,
    language: {
        noResults: function () {
            return "Không tìm thấy dữ liệu";
        }
    },
});

var form = $("#create-formCompany");
function save() {
    if ($("#create-formCompany").valid()) {

    }
}
function pushState(url) {
    window.history.pushState({
        turbolinks: true,
        url: url
    }, null, url)
}
///////////////////admin
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
                    data: {
                        Id: parseInt(ids)
                    },
                    contentType: 'application/json',

                    success: function (res) {
                        if (res.isValid) {
                            $('#viewAll').html(res.html)
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
    , DeleteRemove: function (url, ids, sel) {

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
        message: '<div class="loader"></div>',
        css: {
            /*  border: '3px solid #a00'*/
        }
    })
}
function loadingStop() {
    $.unblockUI();
}
async function loadDataAjaxHtml(url, id = "#viewAll", _async = true) {

    $.ajax({
        global: false,
        async: _async,
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
jQuery('#txtsearchindex').bind('focusin focus', function (e) {
    e.preventDefault();
})
$("input#txtsearchindex").focus(function () {

    $(this).autocomplete("search");
    $("<div id='showzindex'></div>").insertAfter("header");
    $("html").css("overflow", "hidden");
});
$("input#txtsearchindex").blur(function () {
    $("#showzindex").remove();
    $("html").css("overflow", "");
});
selectProductType.change(function () {
    $("input#txtsearchindex").autocomplete("search");
});
function __highlight(s, t) {
    //  t = t.replace(/[-\/\\^$*+?.()|[\]{}]/g, '\\$&');
    // var matcher = new RegExp("(" + t.split(' ').join('|') + ")", "gi");

    var matcher = new RegExp("(" + $.ui.autocomplete.escapeRegex(t) + ")", "ig");
    return s.replace(matcher, "<strong>$1</strong>");
}





function LoadDataTableById(id) {
    dataTable = $("#" + id).DataTable({
        "lengthMenu": [[10, 25, 50, 100, 200, -1], [10, 25, 50, 100, 200, "All"]],
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
//$('input').on('ifChecked', function (event) {
//    alert(event.type + ' callback');
//});


function loadeventchillmenu() {
    $('.modal-menu .header i').click(function () {
        $(this).closest(".modal-menu").find("*").off();
        $(this).closest(".modal-menu").remove();
        $("html, body").css("overflow", "");
    });
    $('.modal-menu-category .menu-pattern i').click(function () {
        $(this).closest("li.item-menu").toggleClass("active");
        if ($(this).hasClass("fa-plus")) {
            $(this).removeClass("fa-plus")
            $(this).addClass("fa-minus")
        } else {
            $(this).removeClass("fa-minus")
            $(this).addClass("fa-plus")
        }
    });
}
function loadDataAjax(url) {
    var dataJson = "";
    $.ajax({
        global: false,
        async: false,
        dataType: "JSON",
        type: "GET",
        url: url,
        success: function (data) {
            dataJson = data;
        }
    });
    return dataJson;
}
function LoadEventMenuMobi() {


    // $(".menu-category").click(async function () {
    $(".menu-infouser").on('click', async function () {
        var json = await loadDataAjax("/Account/GetUser");
        var data = json.data;
        var html = "";
        if (data != "") {

            html = `<div class="modal-menu-infouser modal-menu">
                            <div class="header">
                                <i class="fas fa-long-arrow-alt-left"></i>
                                <h4>Thông tin cá nhân</h4>
                            </div>
                            <div class="body">
                               <div class="col1">
                                   <img src="../images/user.png" />
                                   <div class="right-inf">
                                     <ul>
                                         <li>
                                              <span class="first">Họ và tên</span>
                                              <p class="last">: `+ data.name + `</p>
                                         </li>
                                         <li>
                                              <span class="first">Tài khoản</span>
                                              <p class="last">: `+ data.userName + `</p>
                                         </li>
                                         <li>
                                              <span class="first">Email</span>
                                              <p class="last">: `+ (data.email != null ? data.email : "") + `</p>
                                         </li>
                                         
                                     </ul>
                                   </div>
                               </div>
                               <div class="item-category">
                                   <ul class="col-menu">
                                       <li><a href="/quan-ly-tin-dang"><span class="icon-manager icon-cd"></span> Quản lý tin đăng</a> </li>
                                       <li><a href="/dang-tin"><span class="icon-create icon-cd"></span> Đăng tin</a></li>
                                   </ul>
                                     <ul class="col-menu">
                                       <li><a href="/Account/Profile"><span class="icon-user icon-cd"></span> Thông tin cá nhân</a></li>
                                       <li><a href="/Account/ChangePass"><span class="icon-lock_open icon-cd"></span> Đổi mật khẩu</a></li>
                                       <li> <a href="/Account/Logout"><span class="icon-logout icon-cd"></span> Đăng xuất</a> </li>
                                   </ul>
                               </div>
                            </div>
                        </div>`;
        } else {
            html = `<div class="modal-menu-infouser modal-menu">
                            <div class="header">
                                <i class="fas fa-long-arrow-alt-left"></i>
                                <h4>Thông tin cá nhân</h4>
                            </div>
                            <div class="body">
                               <div class="login">
                                      <a href="javascript:void(0)" class="btn_reg btn-registration">Đăng ký</a>
                                      <a href="javascript:void(0)" class="btn_login">Đăng nhập</a>
                                </div>
                            </div>
                        </div>`;
        }
        $("html, body").css("overflow", "hidden");
        $("body").append(html);
        if (!json.isLogin) {
            loadEventButonlogin();
        }
        await loadeventchillmenu();
        html = "";
    });
    $(".menu-alert").on('click', async function () {
        let htmlcategory = "";
        var json = await loadDataAjax("/Sell/GetAllAlert");
        var dataJson = JSON.parse(json.data);
        if (!json.isLogin) {
            htmlcategory =
                `<div class="login">
                      <a href="javascript:void(0)" class="btn_reg btn-registration">Đăng ký</a>
                      <a href="javascript:void(0)" class="btn_login">Đăng nhập</a>
                </div>`;

        } else {
            if (json.data.length > 0) {
                htmlcategory = '<ul class="list-alert">';
                $.each(dataJson, function (i, item) {
                    htmlcategory += `<li class="item-alert">
                                      <a href="/" class="no-review"> (24/24/1211) Thông báo tin đăng đã được phê duyệt</a>
                                      <small>Tin đăng "Bất động sản quảng nam" đã được phê duyệt</small>
                                    </li>`;
                });
                htmlcategory += '</ul>';
            } else {
                htmlcategory = `<p class="no-alert">Chưa có thông báo nào</p>`;
            }
        }
        let html = `<div class="modal-menu-alert modal-menu">
                        <div class="header">
                            <i class="fas fa-long-arrow-alt-left"></i>
                            <h4>Thông báo (`+ dataJson.length + `)</h4>
                        </div>
                        <div class="body">
                            `+ htmlcategory + `
                        </div>
                  </div>`

        $("html, body").css("overflow", "hidden");
        $("body").append(html);
        if (!json.isLogin) {
            loadEventButonlogin();
        }
        await loadeventchillmenu();
    });

    // load menu danh mục
    $(".menu-category").on('click', async function () {
        $(".modal-menu-category").remove();
        let htmlcategory = "";
        var json = await loadDataAjax("/API/Handling/GetAllCategoryProduct");

        var htmlmenuChil = "";
        var chillmenu = false;

        $.each(json, function (i, item) {
            if (item.text != "") {
                if (item.isParent) {
                    if (htmlcategory != "") {
                        if (chillmenu == true) {
                            htmlcategory += `<ul class="dropdown-menu-item">` + htmlmenuChil + "</ul>"
                        }
                        htmlcategory += "</li>";
                    }
                    let icon = '<i class="fas fa-minus"></i>';
                    let checkchill = json.find(x => x.idPattern == item.id);
                    if (checkchill != null && checkchill != "") {
                        icon = '<i class="fas fa-plus"></i>';
                    }

                    htmlcategory += `<li class="item-menu">
                                    <div class="menu-pattern">
                                        `+ icon + `
                                         <a href="/`+ item.slug + `">
                                           `+ item.text + `
                                        </a>
                                    </div>
                                   `;
                    chillmenu = false;
                    htmlmenuChil = "";
                }
                else {
                    htmlmenuChil += ` <li>
                                           <i class="fas fa-minus"></i>
                                            <a href="/`+ item.slug + `">
                                            `+ item.text + `
                                            </a>
                                        </li>`;
                    chillmenu = true;
                }
            }
        });
        if (chillmenu == true) {
            htmlcategory += `<ul class="dropdown-menu-item">` + htmlmenuChil + "</ul>"
        }
        htmlcategory += "</li>";
        let html = `<div class="modal-menu-category modal-menu">
                    <div class="header">
                        <i class="icon-arrow_back_white icon-cd"></i>
                        <h4>Danh mục</h4>
                    </div>
                    <div class="body">
                        <ul class="list-category">`;
        html += htmlcategory;
        html += `</ul></div></div>`;
        $("html, body").css("overflow", "hidden");
        $("body").append(html);
        await loadeventchillmenu();
    });
}
function checkloadcontentProduct() {
    var getwidthdec = $(".body-Description").height();
    if (getwidthdec > 700) {
        $(".body-Description").addClass("hide-content");
        $(".body-Description").parent().append("<span class='show-more'>Xem thêm</span>");
    }
    $(".show-more").click(function () {
        $(this).remove();
        $(".body-Description").removeClass("hide-content");
    });

}
async function loadProductInCategory(idCategory) {
    var json = await loadDataAjax("/API/Handling/GetAllProductInCategory?idCategory=" + idCategory);
    if (json.isValid) {
        htmlcategory = '';
        let dataJson = JSON.parse(json.data);
        $.each(dataJson, function (i, item) {

            let htmlshowDiscount = "";
            let htmlshowpriceDiscount = `<span class="current_price">` + item.Price + `₫</span>`;
            if (item.isRunPromotion) {
                if (item.DiscountRun > 0) {
                    htmlshowDiscount = `<div class="label_product">
                                  <span class="label_sale"> `+ (item.DiscountRun) + `% </span>
                                </div>`;
                }

                htmlshowpriceDiscount = ` <span class="current_price">` + item.PriceDiscountRun + `₫</span>
                                                <span class="old_price">` + item.Price + `₫</span>`;
            } else if (item.isPromotion) {
                if (item.Discount > 0) {
                    htmlshowDiscount = `<div class="label_product">
                                  <span class="label_sale"> `+ item.Discount + `% </span>
                                </div>`;
                }


                htmlshowpriceDiscount = ` <span class="current_price">` + item.PriceDiscount + `₫</span>
                                                <span class="old_price">` + item.Price + `₫</span>`;
            }
            htmlcategory += `<div class="col-product col-item">
                                <div class="body">
                                    <div class="product_thumb">
                                        <a href="`+ item.Slug + `">
                                            <img src="../`+ item.Img + `" />
                                        </a>
                                        `+ htmlshowDiscount + `
                                    </div>
                                    <div class="product_content">
                                        <a class="product_name" href="`+ item.Slug + `" title="` + item.Name + `" tabindex="0"> ` + item.Name + `</a>
                                        <div class="price-container">

                                          `+ htmlshowpriceDiscount + `
                                        </div>
                                    </div>
                                </div>
                            </div>`;
        });
        if (htmlcategory != "") {
            $(".load-list-product-lq").html(htmlcategory);
        } else {
            $(".load-list-product-lq").html("<p class='productNotExit'>Không có sản phẩm nào</p>");
        }

    } else {
        $(".load-list-product-lq").html("<p class='productNotExit'>Không có sản phẩm nào</p>");
    }

}
function loadCategoryPostIn(id) {
    $.ajax({
        dataType: "JSON",
        type: "GET",
        url: urlGetCategoryPost,
        success: function (data) {
            html = `<ul>`;
            var i = 0;
            var j = 0;
            let classactive = "";
            var idpatern = null;
            var IdLevel = 0;
            var z = 0;
            itemchill = "";
            data.forEach(function (value, index) {

                if (value.text != "") {
                    if (value.id == id) {
                        classactive = "select";
                    } else {
                        classactive = "";
                    }
                    if (i == 0) {
                        html += "<li class='" + classactive + "'><span><a class='collapse' href='/" + value.slug + "'>" + value.text;
                    } else if (value.idPattern == null && idpatern == null) {
                        html += "</a><li><span><a class='collapse " + classactive + "' href='" + value.slug + "'>" + value.text;
                    }
                    else if (value.idPattern == null) {
                        if (idpatern > 0) {
                            itemchill += "</li></ul>";
                            html += itemchill;
                        }
                        html += `</li> <li><span><a class='collapse ` + classactive + `' href='/` + value.slug + `'>` + value.text;
                        z = 0;
                        IdLevel = 0;
                    } else if (value.idPattern != null && value.idPattern != idpatern && value.Level > IdLevel) {
                        itemchill = "</a><i class='fas fa-angle-down'></i></span><ul class='dropdownlist'>";
                        itemchill += "<li><span><a class='" + classactive + "' href='/" + value.slug + "'>" + value.text + "</span></a>";
                        z += 1;
                    } else if (value.idPattern != null && value.idPattern == idpatern) {
                        itemchill += `</li><li><span><a class="` + classactive + `" href='/` + value.slug + `'>` + value.text + "</span></a>";
                    } else if (value.idPattern != null && value.idPattern != idpatern && value.Level < IdLevel) {

                    }
                    IdLevel = value.Level;
                    idpatern = value.idPattern;
                    i += 1;








                    //if (value.idPattern == null && i == 0) {
                    //    if (j == 1) {
                    //        itemchill += "</li></ul>";
                    //        html += itemchill;
                    //        j = 0;
                    //    }
                    //    html += `<li>` + value.text;
                    //    i = 1;
                    //    j = 0;

                    //} else if (value.idPattern == null && i > 0) {
                    //    html += `</li><li>` + value.text;
                    //    i = 1;
                    //    j = 0;
                    //} else if (value.idPattern != null && j == 0) {
                    //    itemchill = "<ul>";
                    //    itemchill += "<li>" + value.text;
                    //    j = 1;
                    //    i = 0;

                    //} else if (value.idPattern != null && j > 0) {
                    //    itemchill += `</li><li>` + value.text;
                    //    j = 1;
                    //    i = 0;
                    //}
                } else if (itemchill != "") {
                    html += itemchill;
                }
            });
            html += "</li></ul>"
            _listcategoryindetailpost.html(html);
            loadevent.eventcollapsecategorypost();
        }
    });
}
function priceFormat() {
    $('.priceFormat').priceFormat({
        prefix: '',
        centsLimit: 0,
        centsSeparator: ',',
        thousandsSeparator: '.'
    });
}
$(function () {
    $('.priceFormat').priceFormat({
        prefix: '',
        centsLimit: 0,
        centsSeparator: ',',
        thousandsSeparator: '.'
    });
    var url = window.location;
    // $('.listitembar a[href="' + url + '"]').parent('li').addClass('current-page');
    $('.listitembar a').filter(function () {
        return this.href == url;
    }).parent('li').addClass('active');
});

if (window.matchMedia('(max-width:  991.98px)').matches) {
    LoadEventMenuMobi();
    loadeventchillmenu();

    $(".banner_headerRight").remove();
    $("#element-search").remove();
    $(".search_logo .logo").remove();
    $(".mobile-search .select_seach").remove();
    $(".barmaninapp").show();
    $(window).bind('scroll', function () {
        if ($(window).scrollTop() > 30) {
            $('#myheader').addClass('fixed-top');
            $('.header-search').addClass('fixed-top');
        } else {
            $('#myheader').removeClass('fixed-top');
            $('.header-search').removeClass('fixed-top');
        }
    });

}
if (window.matchMedia('(min-width: 992px)').matches) {
    //$(".mobileLanguae").remove();
    $(".title-seach").remove();
    $(".btn-sreach").remove();
    $(".icon-seach-mb").remove();
    $(window).bind('scroll', function () {
        if ($(window).scrollTop() > 112) {
            $('#myheader').addClass('fixed-top');
            $('.header-search').addClass('fixed-top');
        } else {
            $('#myheader').removeClass('fixed-top');
            $('.header-search').removeClass('fixed-top');
        }
    });

}
function showcategorymobi() {
    $(".modalcatemobile").each(function () {
        var data = $(this);
        data.addClass("d-none");

    });
    $("html, body").css("overflow", "hidden");
}
function activeMenuApp() {
    $(".listitembar li").each(function () {
        var data = $(this);
        data.removeClass("active");
    });
}
/// cart
var totalitem = $(".intended__final-prices");
var priceitem = $(".intended__real-prices");
var AmountTamtinhOrder = $(".prices__value--tamtinh");
var totalOrder = $(".final-total");
var VATAmountOrder = $(".final-VATAmount");
var amountOrder = $(".final-amount");

btnsearch.click(function () {
    if (txtsearchindex.val() != "") {
        $(this).parents("form").submit();
    }
});

btnUpdateimgavata.click(function () {


    // let html = $("#upload-avata").html();
    let html = `<div id="upload-avata" class="upload-avata-user" >
                                <div class="body">
                                    <div class="upload-msg">
                                        <input type="file" id="upload" value="Choose a file" accept="image/*">
                                            <span>Kích vào đây để chọn hình ảnh</span>
                                    </div>
                                        <div class="upload-avata-user-wrap">
                                            <div id="upload-avata-user"></div>
                                        </div>

                                    </div>
                                    <button class="cancel-result btn btn-default mr-2">Hủy bỏ</button>
                                    <button class="upload-result btn btn-primary">Lưu thay đổi</button>

                                </div>`;
    Swal.fire({
        title: 'Cập nhật ảnh đại diện',
        html: html,
        showConfirmButton: false,
        showCloseButton: true,
        didRender: () => {
            UploadAvatar();
        },

    })
    //$('#form-modal .modal-body').html(html);
    //$('#form-modal .modal-title').html("Upload avata");


});

function UploadAvatar() {
    var $uploadCrop;

    function readFile(input) {
        if (input.files && input.files[0]) {
            var reader = new FileReader();

            reader.onload = function (e) {
                $('.upload-avata-user').addClass('ready');
                $uploadCrop.croppie('bind', {
                    url: e.target.result
                }).then(function () {
                    console.log('jQuery bind complete');
                });

            }

            reader.readAsDataURL(input.files[0]);
        }
        else {
            Swal.fire("Sorry - you're browser doesn't support the FileReader API");
        }
    }

    $uploadCrop = $('#upload-avata-user').croppie({
        viewport: {
            width: 300,
            height: 300,
            type: 'circle'
        },
        enableExif: true
    });

    $('#upload').on('change', function () { readFile(this); $(".swal2-container").addClass("change-avata"); });
    $('.upload-result').on('click', function (ev) {
        var imgbase = "";
        $uploadCrop.croppie('result', {
            type: 'canvas',
            size: 'viewport'
        }).then(function (resp) {
            imgbase = resp;
            //popupResult({
            //    src: resp  // để như vậy thì hàm nhận dc dạng json
            //});
            $.ajax({
                type: 'POST',
                url: "/Account/UpdateAvatar",
                //async: false,
                data: {
                    base64: imgbase
                },
                success: function (res) {
                    if (res.isValid) {
                        popupResult.success(res.data);
                        $(".form-avatar img.default").attr("src", res.src);
                        $(".card-profile-image img.rounded-circle").attr("src", res.src);
                        $(".main-head .login-cus img").attr("src", res.src).removeClass("icon-default");
                        $(".form-avatar img.default").addClass("active");
                    } else {
                        popupResult.error(res.data);
                    }
                },
                error: function (err) {
                    popupResult.error(err);
                }
            })
        });


    });
    $('.cancel-result').on('click', function (ev) {
        Swal.close();
    });
}
//https://sweetalert2.github.io/#configuration

//$("select").on("select2:close", function (e) {
//    $(this).valid();
//});
$("#formvalid").validate({
    ignore: 'input[type=hidden]',
    rules: {
        Name: {
            required: true,
            minlength: 2
        },
        CodeConsultation: {
            required: true
        }
        , CarCompany: {
            required: true
        },
        Email: {
            required: true,
            email: true
        },
    },
    messages: {
        FullName: {
            required: errrequired
        },
        CodeConsultation: {
            required: errrequired
        },
        PhoneNumber: {
            required: errrequired
        },
        Email: {
            required: errrequired,
            email: emailformat
        },
        CarCompany: {
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
        }
        else {
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
        loadingStart();
        form.submit();
    }
});

$('#myForm').validate({
    ignore: 'input[type=hidden], .select2-input, .select2-focusser'
});
$('#PhoneNumber').usPhoneFormat({
    format: '(xxx) xxx-xxxx'
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

//load form search



///////////////////////
function fetContentHtmlprice(idselectd) {
    $.ajax({
        global: false,
        type: "GET",
        data: {
            idselectd: idselectd
        },
        contentType: "Json",
        url: "/API/Handling/GetAllPrice",

        //beforeSend: function () {
        //    loadingStart("#dataProduct");
        //},
        success: function (data) {
            if (data.isValid) {
                let html = ` <ul class="ul-item-price">`;
                let arr = JSON.parse(data.data);

                var url_string = window.location.href;
                var url = new URL(url_string);
                var params = new URLSearchParams(url.search);
                var idorderprice = params.get("idPrice");
                arr.forEach(myFunction);
                function myFunction(value, index, array) {
                    let checkhtml = "";
                    if (value.id == parseInt(idorderprice)) {
                        checkhtml = "checked";
                    }
                    html += `<li>
                                   <input type="radio" `+ checkhtml + `  class="icheckradio_square-green" name="price" id="price" data-price=` + value.id + ` value= ` + value.id + ` />
                                   `+ value.text + `
                               </li>`;
                }
                html += `</ul>`;

                $(".block-list-price .content-item").html(html);
                loadisCheck();
                //setTimeout(function () {
                //    $('input#price').on('ifChecked', function (event) {
                //        SortAndSeartInCategoryProduct();
                //    });
                //}, 500); 

            }
        }
    });
}
function SortAndSeartInCategoryProduct(type = 0) {
    //0 là mặc định danh mục, 1 là danh mục sell
    var url_string = window.location.href;
    var url = new URL(url_string);
    var anew = new URLSearchParams(url.search);
    let order = $('input[name=order]:checked').attr("order");
    let price = $('input[name=price]:checked').val();

    if (typeof price == 'undefined') {
        price = anew.get('idPrice');
    }

    const cate = anew.get('cate')
    anew.delete('keyword');
    anew.delete('sortby');
    anew.delete('idPrice');
    // anew.delete('search');
    // anew.append("search", 1);
    if (typeof price !== 'undefined') {
        anew.append("idPrice", price);
        idPrice = price;
    }
    if (typeof order !== 'undefined') {
        anew.append("sortby", order);
        sortby = order;
    }

    // window.location.href = window.location.pathname + "?" + encodeURI(anew.toString());
    pushState(window.location.pathname + "?" + encodeURI(anew.toString()));
    if (type == 1) {
        $(".row-order").trigger("click"); // hide sắp xếp
        Product.fetch_dataSell(1, idcategory, cate);
    } else {
        Product.fetch_data(1, idcategory, keyword);
    }
}

function loadUser() {
    var user;
    $.ajax({
        global: false,
        async: false,
        type: "GET",
        url: "/Account/GetUser",
        success: function (data) {
            user = data.data;
        }
    });
    return user;
}
function loadeventmenumobi() {
    $("#menu_portal > li > a > i").click(function (event) {
        event.preventDefault();
        $(this).closest("li").toggleClass("active");
        return false;
    });
}

if (window.matchMedia('(min-width: 992px)').matches) {

}
async function loadatahistori(text, Task = 0) {
    var datas;
    $.ajax({
        global: false,
        async: false,
        url: "/Search/autoSearch",
        type: "GET",
        dataType: "json",
        data: {
            keyword: text,
            ProductType: parseInt($("#ProductType").val()),
            history: true,
            Task: Task

        },
        // html: true,
        success: function (data) {
            datas = data;
        },
    });
    return datas;
}
$("#txtsearchindex").autocomplete(
    {
        autoFocus: true,
        minLength: 0,
        delay: 500,

        source: function (request, response) {

            if (request.term == "") {
                var listhis = localStorage.getItem("search-keyword");

                if (listhis != null && listhis.length > 0) {

                    var data = JSON.parse(listhis);
                    var getdataprotype = data.filter(m => m.ProductType == parseInt($("#ProductType").val()));
                    if (getdataprotype.length > 0) {
                        response($.map(getdataprotype, function (item) {
                            // let texthigh = __highlight(item.name, request.term);
                            let texthigh = __highlight(item.name, request.term);
                            let html = "<a href='javascript:void(0)'><div class='search-auto'>" +
                                "<div class='img'><i class='fas fa-history'></i></div>" +
                                "<div class='tk_name'><span>" + texthigh + "</span></div></div></a>";

                            return {
                                label: html, value: item.name
                            };
                        }));
                    } else {
                        loadatahistori(request.term).then(responses => {
                            response($.map(responses, function (item) {
                                // let texthigh = __highlight(item.name, request.term);
                                let texthigh = __highlight(item.name, request.term);
                                let html = "<a href='javascript:void(0)'><div class='search-auto'>" +
                                    "<div class='img'><i class='fas fa-search'></i></div>" +
                                    "<div class='tk_name'><span>" + texthigh + "</span></div></div></a>";

                                return {
                                    label: html, value: item.name
                                };
                            }))
                        });
                    }
                } else {
                    loadatahistori(request.term).then(responses => {
                        response($.map(responses, function (item) {
                            // let texthigh = __highlight(item.name, request.term);
                            let texthigh = __highlight(item.name, request.term);
                            let html = "<a href='javascript:void(0)'><div class='search-auto'>" +
                                "<div class='img'><i class='fas fa-search'></i></div>" +
                                "<div class='tk_name'><span>" + texthigh + "</span></div></div></a>";

                            return {
                                label: html, value: item.name
                            };
                        }))
                    });
                }
            } else {
                $.ajax({
                    global: false,
                    // url: "/Site/autoSearch",
                    url: "/Search/autoSearch",
                    type: "GET",
                    dataType: "json",
                    data: {
                        keyword: request.term,
                        // ProductType: parseInt($("#ProductType").val())
                    },
                    // html: true,
                    success: function (data) {
                        response($.map(data, function (item) {
                            // let texthigh = __highlight(item.name, request.term);
                            let texthigh = __highlight(item.name, request.term);
                            let html = "<a href='javascript:void(0)'><div class='search-auto'>" +
                                "<div class='img'><i class='fas fa-search'></i></div>" +
                                "<div class='tk_name'><span>" + texthigh + "</span></div></div></a>";

                            return {
                                label: html, value: item.name
                            };
                        }))
                        return { label: request.term, value: request.term };
                    },
                })
            }

        },
        html: true,
        select: function (e, ui) {
            // console.log(ui);
            $(this).val(ui.item.value);
            //let url = "/Search?keyword=" + ui.value + "&ProductType=" + parseInt($("#ProductType").val());
            //window.location.href = url;
            var datakeyword = ui.item.value.trim();
            var _lists = new Array();
            var listhis = localStorage.getItem("search-keyword");
            if (listhis == null || listhis == "") {
                var list = new Object();
                list.name = datakeyword;
                list.HistoryLoca = true;
                list.ProductType = parseInt($("#ProductType").val());
                _lists.push(list);
                localStorage.setItem("search-keyword", JSON.stringify(_lists));
            } else {
                listhis = JSON.parse(listhis);
                if (listhis.length > 10) {
                    listhis.pop();
                }
                var getname = listhis.filter(x => x.name.toLowerCase().trim() == datakeyword.toLowerCase().trim());
                if (getname.length == 0) {
                    var list = new Object();
                    list.name = datakeyword;
                    list.HistoryLoca = true;
                    list.ProductType = parseInt($("#ProductType").val());
                    listhis.unshift(list);
                    localStorage.setItem("search-keyword", JSON.stringify(listhis));
                }
            }
            $(this).parents('form').submit();
            //  select.val(ui.item.name);

        },
    });


function loaddataSelect2arrPost(id, URL, code, idbrand, idselectd = "", placeholder = "") {
    let _placeholder = placeholder != "" ? placeholder : "Chọn giá trị";
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
                placeholder: _placeholder,
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
function loaddataSelect2Ajax(URL, id, idselectd, placeholder = "") {

    $(id).select2({
        ajax: {
            url: URL,
            type: "GET",
            dataType: 'json',
            placeholder: placeholder,
            //delay: 250,
            data: function (params) {
                return {
                    // idselectd: params.term, // search term
                    idselectd: idselectd
                };
            },
            templateSelection: function (container) {

                $(container.element).attr("source", container.source);
                return container.text;
            },
            processResults: function (data) {

                return {
                    results: $.map(data, function (item) {
                        return {
                            text: item.text,
                            source: item.text,
                            selected: item.selected,
                            id: item.id
                        }
                    })
                };
            },
            cache: true
        },
        //define min input length
        //minimumInputLength: 3,
    });


}
function loaddataSelect2(URL, id, idselectd, placeholder = "", async = true) {

    $.ajax({
        global: false,
        async: async,
        type: "GET",
        dataType: 'JSON',
        url: URL,
        data: { idselectd: idselectd },
        success: function (data) {
            $(id).select2({
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
function templateSelect2(data) {
    let html = "<span class='select-option-noParent'>" + data.text + "</span>";
    if (data.isParent) {
        html = "<span class='select-option-isParent'>" + data.text + "</span>";
    }
    return html;
}
function templateBoxSelect2(data) {

    let html = "<span class=''>" + data.text + "</span>";
    if (data.Code == "box") {
        html = "<span class='select2-option-box'><input type='text' class='form-control'/></span>";
    }
    return html;
}


function loaddataSelect2CustomsTempalteBox(URL, id, idselectd, placeholder = "", _fromPrice, _toPrice) {

    fromPrice = _fromPrice;
    toPrice = _toPrice;
    $.ajax({
        global: false,
        async: true,
        type: "GET",
        dataType: 'JSON',
        url: URL,
        data: { idselectd: idselectd },
        success: function (data) {
            $(id).select2({
                data: data,
                placeholder: placeholder != "" ? placeholder : "Chọn giá trị",
                allowClear: true,
                //templateResult: boxchoose != "" ? templateBoxSelect2: templateSelect2,
                //escapeMarkup: function (m) {
                //    return m;
                //},
                language: {
                    noResults: function () {
                        return "Không tìm thấy dữ liệu";
                    }
                }, closeOnSelect: true,
                //dropdownParent: $("#myModal")
                dropdownAdapter: $.fn.select2.amd.require("SingleSelectSearchAdapter")
            });

            if (toPrice > 0) {
                loadSelectMucGia();
            }
        }
    });
}
var fromPrice = 0;
var toPrice = 0;
$.fn.select2.amd.define("SingleSelectSearchAdapter", [
    "select2/utils",
    "select2/dropdown",
    "select2/dropdown/attachBody",
    "select2/dropdown/attachContainer",
    "select2/dropdown/search",
    "select2/dropdown/minimumResultsForSearch",
    "select2/dropdown/closeOnSelect",
],
    function (Utils, Dropdown, AttachBody, AttachContainer, Search, CloseOnSelect, MinimumResultsForSearch) {

        // Decorate Dropdown with Search features
        let dropdownWithHeader = Utils.Decorate(Dropdown, Search);
        // CloseOnSelect = function () { return true; };
        dropdownWithHeader.prototype.render = function () {
            let $rendered = Dropdown.prototype.render.call(this);

            // let headerTitle = this.options.options.headerTitle ? this.options.options.headerTitle : "Sélectionnez une option";
            //let $search = $(
            //    '<span class="select2-custom-header">' +
            //    //'<span class="select2-custom-header__title">' + headerTitle + '</span>' +
            //    '<span class="select2-custom-header__close"></span>' +
            //    '</span>' +
            //    '<span class="select2-search select2-search--dropdown">' +
            //    '<input class="select2-search__field" type="search" tabindex="-1"' +
            //    ' autocomplete="off" autocorrect="off" autocapitalize="none"' +
            //    ' spellcheck="false" role="textbox" />' +
            //    '</span>'
            //);
            let $search = $(
                '<span class="select2-custom-header">' +
                //'<span class="select2-custom-header__title">' + headerTitle + '</span>' +
                '<span class="select2-custom-header__close"></span>' +
                '</span>' +
                '<span class="select2-search select2-search--dropdown price-slider-range slider-range">' +
                '<input placeholder="Từ" value="' + fromPrice + '" class="form-control" id="fromvalue" type="text" tabindex="-1"' +
                ' autocomplete="off" autocorrect="off" autocapitalize="none"' +
                ' spellcheck="false" role="textbox" />' +
                '<span><img src="/icon/ic_arrow_left.png"></span>' +
                '<input placeholder="Đến" value="' + toPrice + '" class="form-control" id="tovalue" type="text"' +
                ' autocomplete="off" autocorrect="off" autocapitalize="none"' +
                ' spellcheck="false" role="textbox" />' +
                '<button type="button" class="btn btn-success" id="btn-apply-range"><i class="fas fa-check"></i></button>' +
                '</span>' +
                '<div class="slideprice"><div id="slideprice-range"></div></div>'
            );

            // mở ra là tìm kiếm
            this.$searchContainer = $search;
            this.$search = $search.find('input.search');

            $rendered.prepend($search);

            return $rendered;
        };

        // Decorate the dropdown+search with necessary containers
        let adapter = Utils.Decorate(dropdownWithHeader, AttachContainer);
        adapter = Utils.Decorate(adapter, AttachBody, CloseOnSelect, MinimumResultsForSearch);

        return adapter;
    }
);
function loadnoUiSlider() {
    var slider = document.getElementById('slideprice-range');
    slider.noUiSlider.updateOptions({
        step: 100,
        range: { min: 0, max: 20000 },
    });
    //noUiSlider.create(slider, {
    //    start: [0, 1000],
    //    range: { min: 0, max: 20000 },
    //    step: 100,
    //    format: wNumb({
    //        decimals: 0,
    //        //thousand: '.'
    //    }),
    //    connect: true
    //});
}
function loadsliderange() {
    var slider = document.getElementById('slideprice-range');
    var sliderjqu = $('#slideprice-range');
    var target = sliderjqu.children(".noUi-base");

    if (target.length == 0) {
        let start = 0;
        let end = 1000;
        let step = 100;
        let _start = $('.mucgia option:selected').attr("data-leftValue");
        let _end = $('.mucgia option:selected').attr("data-rightValue");

        if (_start) {
            start = parseInt(_start);
        }
        else if (fromPrice > 0) {
            start = fromPrice / 1000000;
        }
        if (_end) {
            end = parseInt(_end);
        }
        else if (toPrice > 0) {
            end = toPrice / 1000000;
        }
        if (_start && _end) {
            step = 1;
        }

        noUiSlider.create(slider, {
            start: [start, end],
            range: { min: 0, max: 20000 },
            step: step,
            format: wNumb({
                decimals: 0,
                //thousand: '.'
            }),
            connect: true
        });
        var leftValue = $('#fromvalue'),
            rightValue = $('#tovalue');
        function convertValuesToTime(values, handle) {

            if (handle === 0) {
                leftValue.val(values[0]);
                return;
            };
            rightValue.val(values[1]);

        };

        slider.noUiSlider.on("change.tap", function () {

            slider.noUiSlider.updateOptions({
                step: 100,
                range: { min: 0, max: 20000 },
            });
            //loadSelectMucGia()
        });
        $(".price-slider-range input").keyup(function () {
            let from = leftValue.val();
            let to = rightValue.val();

            if (parseInt(from) >= 0 && parseInt(to) >= 0) {
                slider.noUiSlider.updateOptions({
                    step: 1,
                    range: { min: 0, max: 20000 },
                });
                slider.noUiSlider.set([from, to]);
                //slider.noUiSlider.updateOptions({
                //    start: [from, to],
                //    range: { min: 0, max: 40000 },
                //});
            }

        });
        slider.noUiSlider.on('update', function (values, handle) {

            convertValuesToTime(values, handle);
        });
        document.getElementById("btn-apply-range").addEventListener("click", loadSelectMucGia);
        // $(".option-value-custom").remove();
    }
}
function ConvertAmountToWord(amount) {
    amount = 1000000 * amount;
    let amountword = "";
    if ((amount < 0) || (amount > 999999999999999)) {
        return "Số quá lớn";
    }
    else {

        if ((amount >= 1000000000) && (amount <= 999999999999999))    //TỶ
        {
            // let ty = Math.round((amount / 1000000000), 2);
            let ty = (amount / 1000000000);
            amountword = `${ty} tỷ`;
        }
        if ((amount >= 1000000) && (amount < 1000000000)) //TRIỆU
        {
            let trieu = (amount / 1000000);
            amountword = `${trieu} triệu`;
        }
        if (amount == 0) {
            amountword = "0 đồng";
        }
    }
    return amountword;
}
function loadSelectMucGia() {

    $(".mucgia option.option-value-custom").remove();
    var leftValue = $('#fromvalue').val(),
        rightValue = $('#tovalue').val();
    if (toPrice > 0 && !rightValue) {
        rightValue = toPrice / 1000000;
        leftValue = fromPrice / 1000000;
    }
    //localStorage.setItem("leftValue", leftValue);
    //localStorage.setItem("rightValue", rightValue);
    let text = ConvertAmountToWord(parseInt(leftValue)) + " - " + ConvertAmountToWord(parseInt(rightValue));
    $('.mucgia').select2('destroy');
    $('.mucgia').append('<option selected="selected" data-leftValue="' + leftValue + '" data-rightValue="' + rightValue + '" value="' + leftValue + '-' + rightValue + '" class="option-value-custom">' + text + '</option>');
    $('.mucgia').select2({
        placeholder: "Tất cả",
        allowClear: true,
        //templateResult: boxchoose != "" ? templateBoxSelect2: templateSelect2,
        //escapeMarkup: function (m) {
        //    return m;
        //},
        templateResult: resultState,
        language: {
            noResults: function () {
                return "Không tìm thấy dữ liệu";
            }
        }, closeOnSelect: true,
        //dropdownParent: $("#myModal")
        dropdownAdapter: $.fn.select2.amd.require("SingleSelectSearchAdapter")

    });
}
function resultState(data, container) {

    if (data.element) {
        $(container).addClass($(data.element).attr("class"));
    }
    return data.text;
}

function loaddataSelect2CustomsTempalte(URL, id, idselectd, placeholder = "") {

    $.ajax({
        global: false,
        async: true,
        type: "GET",
        dataType: 'JSON',
        url: URL,
        data: { idselectd: idselectd },
        success: function (data) {
            $(id).select2({
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

function loaddataSelect2Append(URL, id, idselectd, name, placeholder = "") {
    $.ajax({
        global: false,
        async: true,
        type: "GET",
        url: URL,
        dataType: 'JSON',
        data: { idselectd: idselectd },
        success: function (data) {
            var htmlselect2 = "<select name='" + name + "' id='" + name + "' class='form-control'>";
            htmlselect2 += '<option></option>';

            for (i = 0; i < data.length; i++) {
                if (data[i].id == idselectd) {
                    htmlselect2 += '<option  selected="selected"  value="' + data[i].id + '" data-code=' + data[i].slug + '>' + data[i].text + '</option>';
                } else {
                    htmlselect2 += '<option value="' + data[i].id + '" data-code=' + data[i].slug + '>' + data[i].text + '</option>';
                }

            }
            htmlselect2 += "</select>";
            //alert(data.msg);
            //  localStorage.setItem("messDeleteOK", lang.length);
            // location.reload();
            $(id).html(htmlselect2);

            $(id).select2({
                placeholder: placeholder != "" ? placeholder : "Tất cả",
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
///////////////////
function loaddaterangepicker() {

    try {
        $('.fc-datepicker').daterangepicker({
            // defaultDate: new Date(),//

            //defaultDate: "null",
            "singleDatePicker": true,
            "showDropdowns": true,
            todayHighlight: true,
            "autoUpdateInput": false,
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

        var myCalendar = $('.fc-datepicker');
        var isClick = 0;

        $(window).on('click', function () {
            isClick = 0;
        });

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


    } catch (er) { console.log(er); }
}
var Product = {
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
    fetch_data: function (page, idcategory, text) {

        $.ajax({
            global: false,
            type: "GET",
            data: {
                idcategory: idcategory,
                idPrice: idPrice,
                sortby: sortby,
                keyword: text,
                //TypeSerach: typeSearch, // tìm theo danh mục hay từ khoas
                // typeproduct: typeproduct, // loại tìm kiếm sản phẩm hay vài viết
                pagenumber: page,
            },
            url: "/Search/GetProduct",

            //beforeSend: function () {
            //    loadingStart("#dataProduct");
            //},
            success: function (data) {

                $('#dataProduct').html(data.html);
                loadisCheck();
                loadnEvent_init.loadicheckpriceinCateogry();
                setTimeout(function () {
                    $('input#order').on('ifChecked', function (event) {
                        sortby = $(this).attr("order");
                        SortAndSeartInCategoryProduct();
                    });
                    $('input#price').on('ifChecked', function (event) {
                        idPrice = $(this).val();
                        SortAndSeartInCategoryProduct();
                    });

                }, 500);

                loadevent.load_tab_show_incategoryproduct();
                initParams.clearParams();
                loadnEvent_init.loadAddCart();
            }
        });
    },
    fetch_dataSell: function (page, idcategory, cate) {

        $.ajax({
            global: false,
            type: "GET",
            data: {
                cate: cate,
                isPromotion: true,
                loadmore: true,
                idcate: idcategory,
                idPrice: idPrice,
                sortby: sortby,
                page: page,
            },
            url: "/Search/GetProductByCategory",

            //beforeSend: function () {
            //    loadingStart("#dataProduct");
            //},
            success: function (data) {

                $('#ListProduct').html(data);
                loadisCheck();
                loadnEvent_init.loadicheckpriceinCateogry();
                setTimeout(function () {
                    $('input#order').on('ifChecked', function (event) {

                        sortby = $(this).attr("order");
                        SortAndSeartInCategoryProduct(1);

                    });//
                    $('input#price').on('ifChecked', function (event) {
                        idPrice = $(this).val();
                        SortAndSeartInCategoryProduct(1);
                    });

                }, 200);
                loadevent.load_tab_show_incategoryproduct();
                initParams.clearParams();
                loadnEvent_init.loadAddCart();
            }
        });
    }
}

var keyword = "#keyword";
var search = "#btnSearchtable";
$(search).click(function () {
    loadLadda();
    dataTableOut.draw();
    Ladda.stopAll();
});
//$(keyword).keyup(function (event) {

//    if ($(this).val().length > 0) {
//        $(".form-search select").each(function () {
//            $(this).prop("disabled", true);
//            $(this).parent(".col").addClass("disabled");
//        });

//    } else {
//        $(".form-search select").each(function () {
//            $(this).prop("disabled", false);
//            $(this).parent(".col").removeClass("disabled");
//        });
//    }

//});

var nameProAddCart = "";
var imgProAddCart = "";
var priceProAddCart = "";

function addCart(id, quantity, AddCart) {
    $.ajax({
        //global: false,
        type: "POST",
        // async: true,
        data: {
            IdProduct: id,
            Quantity: quantity,
            AddCart: AddCart
        },
        dataType: "JSON",
        url: "/Cart/AddToCart",
        beforeSend: function () {
            //  btnaddcart.attr("disabled", "disabled");
            //   loadingStart("#dataProduct");
        },
        success: function (data) {

            if (data.isValid) {
                countCart.html(data.data.quantity);

                if (AddCart) {
                    let html = "<div class='body-add-pro-cart'><img src='" + imgProAddCart + "'/><div class='product-n'><span>" + nameProAddCart + "</span><span class='price'>" + priceProAddCart + " vnđ</span></div></div>";
                    // popupResult.success(html, "Đã thêm " + quantity+" sản phẩm vào giỏ hàng");
                    CustomePopupAddCart("Đã thêm " + quantity + " sản phẩm vào giỏ hàng.<b style='color:red'>(" + data.data.quantity + ")</b>", html);
                } else {
                    let total = data.data.amount.format(0, 3, '.', ',');
                    $(".value-total").html(total + " vnđ");
                    $(".value-amount").html(total + " vnđ");
                }
            } else if (data.login == true) {

                popupResult.warning(data.mess);
            }
            else {
                toastr.error(data.mess);
            }
        }
    });
}
function getCartUser() {
    $.ajax({
        global: false,
        type: "GET",
        url: "/Cart/GetQuantityCartByUser",
        beforeSend: function () {
        },
        success: function (data) {
            if (data.isValid) {
                countCart.html(data.data);
            }
            else {
                console.log(data.mess);
            }
        }
    });
}
function animationacart(quantity, initHeight, initWidth, posInitTop, posInitLeft, posFinalTop, posFinalLeft) {
    $('body').append("<div class='fly btn btn-primary'><i class='fas fa-check-circle'></i></div>");
    $('.fly').css({

        overflow: 'hidden',
        position: 'fixed',
        height: initHeight,
        width: initWidth,
        top: posInitTop,
        left: posInitLeft,
        display: 'flex',
        "align-items": 'center',
        "z-index": '1',
        "padding-left": "14px"
    });
    setTimeout(function () {
        $('.fly')
            .animate({ opacity: 1 }, 10)
            .animate({
                borderRadius: '50%',
                width: 40,
                height: 40,
                marginLeft: initWidth / 2 - initHeight / 2
            }, 300)
            .animate({
                top: posFinalTop - 10,
                left: posFinalLeft - 130
            }, 600)
            .animate({
                opacity: 0
            }, 600)

    }, 20);
    setTimeout(function () {
        $('.fly').remove();
    }, 1600);
    setTimeout(function () {
        quantity_cart.addClass('shake').html(quantity);
        setTimeout(function () {
            quantity_cart.removeClass('shake');
        }, 500)
    }, 1000);
}

$("#increasePro button:first-child").click(function () {
    let _quantity = parseInt($("#quantity").val());
    let _minus = _quantity - 1;
    if (_quantity > 1) {
        $("#quantity").val(_minus);
        if (_minus == 1) {
            $(this).addClass("disable");
        }
    } else {
        return false;
    }

});
$("#increasePro button:last-child").click(function () {
    let _quantity = parseInt($("#quantity").val());
    let _minus = _quantity + 1;
    if (_quantity > 0) {
        $("#quantity").val(_minus);
        if (_minus > 1) {
            $(this).parent().find(".disable").removeClass("disable");
        }
    } else {
        return false;
    }

});

$(window).bind('scroll', function () {
    if ($(window).scrollTop() > 126) {
        $('.header').addClass('fixed-top');
    } else {
        $('.header').removeClass('fixed-top');
    }
});
$("#headerSlidebarLT").hover(function () {
    $("<div id='showzindex'></div>").insertAfter("header");
}, function () {
    $("#showzindex").remove();
});



function ShowCart(check, returnurl) {
    var url = "";
    if (check == 0) {
        url = returnurl;
        window.location.href = "/Account/Login?ReturnUrl=" + url;
    } else {
        window.location.href = "/Cart";
    }
}
function loadListCategory() {
    let id = parseInt($("#idIdPatternCategory").val());
    let type = $("#NamePatternCategory").val();
    if (id > 0) {
        $.ajax({
            global: false,
            type: "GET",
            data: {
                IdPattern: id
            },
            url: "/Site/GetListCategoryByPattern",

            //beforeSend: function () {
            //    loadingStart("#dataProduct");
            //},
            success: function (data) {
                if (data.isValid) {
                    let html = "";
                    let arr = JSON.parse(data.data);
                    arr.forEach(myFunction);
                    function myFunction(value, index, array) {

                        html += '<a class="item item--category" style="padding-left: 0;" href="/' + type + '/' + value.Code + '.html">' + value.Name + '</a>';
                    }
                    $(".listCategory").html(html);
                }
            }
        });
    }
}
function loadListCountry() {

    $.ajax({
        global: false,
        type: "GET",
        data: {
            //IdPattern: id
        },
        url: "/Site/GetListCountry",

        //beforeSend: function () {
        //    loadingStart("#dataProduct");
        //},
        success: function (data) {
            if (data.isValid) {
                let html = "";
                let arr = JSON.parse(data.data);
                arr.forEach(myFunction);
                function myFunction(value, index, array) {

                    html += `<label data-view-id="search_filter_item" data-view-index="0" class="item item--filter_mobile_rom">
                                    <label class=" bHTjmp">
                                        <input type="checkbox" id="idCountry" value="`+ value.Id + `" />
                                        <span>`+ value.Name + `</span>
                                    </label>
                                </label>`;
                }
                $(".listContry").html(html);
            }
        }
    });

}
function loadmain() {
    loadEventButonlogin();
    loaddaterangepicker();
    loadevent.loadautocomplete();
    loadnEvent_init.loadAddCart();
    loadnEvent_init.loadeventcountnotifyuser();
    loadnEvent_init.loadeventmobi();

}

function loadSelect2Home() {

}
function loadhistorisearch() {
    $.ajax({
        global: false,
        type: "GET",
        url: "/Search/GetHistoris",
        beforeSend: function () {
        },
        success: function (data) {
            var html = "";
            if (data.isValid) {
                data.data.forEach(myFunction);

                function myFunction(value, index, array) {

                    html += `<a href='/Search?keyword=` + encodeURI(value.name) + `&ProductType=` + value.productType + `'>` + value.name + `</a>`;
                }
                $("#listhotkeysearch").html(html);

            }
            else {
                console.log("fail Search/GetHistoris");
            }
        }
    });
}

$(document).ready(function () {
    // getCartUser();

    getCartUser();
    // loadhistorisearch();
    loadmain();
    loadisCheck()
});
function loadisCheck() {
    $('input').iCheck({
        checkboxClass: 'icheckbox_square-green',
        radioClass: 'iradio_square-green',
        increaseArea: '20%' // optional
    });
}
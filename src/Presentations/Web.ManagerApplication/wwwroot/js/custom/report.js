var enumTypeReportRevenue = {
    NONE: -1,//
    DOANHTHU: 0,// 
    HUYDON: 1, //
    HINHTHUCPHUVU: 3,
    HOADONCHUATHANHTOAN: 4,
}
var enumTypeReportDashboard = {
    
    DOANHTHU: 0,// 
  
}
var enumTypeReportProduct = {
    NONE: -1,//
    DANHMUCMATHANG: 0,// 
    MATHANGBANCHAY: 1, //
}
var enumTypeReportProduct = {
    NONE: -1,//
    DANHMUCMATHANG: 0,// 
    MATHANGBANCHAY: 1, //
}

var report = {
    getReportProduct: function () {
        $(".btn-report").click(function () {
            TypeReport = $("#TypeReport").val();
            rangesDate = $("#rangesDate").val();
            $.ajax({
                type: 'GET',
                //global: false,
                url: "/Selling/ReportPos/GetReportProducts",
                data: {
                    typeReportProduct: TypeReport,
                    rangesDate: rangesDate
                },
                success: function (res) {
                    if (res.isValid) {
                        if (res.typeReportPos == enumTypeReportProduct.DANHMUCMATHANG) {
                        
                        
                        // table gôm theo ngày
                        htmltable = `<div class="card"><div class="card-body">
                                        <table class="table table-bordered table-striped" id="datatable" style="width:100%">
                                           <thead>
                                               <tr>
                                                   <th  class='text-center'>Danh mục</th>
                                                   <th  class='text-center'>Giá bán</th>
                                                   <th  class='text-center'>Đơn vị</th>
                                                   <th  class='text-center'>Số lượng</th>
                                                   <th  class='text-center'>Tổng tiền</th>
                                               </tr>
                                           </thead>
                                           <tbody>
                                          
                                            `;
                        let quantitys = 0;
                        let amounts = 0;
                        // nhóm theo danh mục
                        const groupByCategory = res.data.listItemReports.reduce((group, product) => {
                            const { categoryName } = product;
                            group[categoryName] = group[categoryName] ?? [];
                            group[categoryName].push(product);
                            return group;
                        }, {});
                        $.each(groupByCategory, function (key, item) {
                            //console.log(i);
                           // console.log(item);

                            
                            let quantity = 0;
                            let amount = 0;
                            htmlcontent = "";
                            for (var i = 0; i < item.length; i++) {
                                
                               
                                var it = item[i];
                                if (it) {
                                    htmlcontent += "<tr>";
                                    quantity += parseFloat(it.quantity);
                                    amount += parseFloat(it.total);
                                    console.log(parseFloat(it.quantity));
                                    htmlcontent += "<td class='text-left'>" + it.productName + "</td>";
                                    htmlcontent += "<td class='text-right number3'>" + it.price + "</td>";
                                    htmlcontent += "<td class='text-center'>" + it.unit + "</td>";
                                    htmlcontent += "<td class='text-right'>" + it.quantity + "</td>";
                                    htmlcontent += "<td class='text-right number3'>" + it.total + "</td>";
                                    htmlcontent += "</tr>";
                                }
                               
                            }

                            htmltbodyheader = `<tr class="category">
                                                <td class="d-none"></td>
                                                <td class="d-none"></td>
                                                <td colspan="3">`+ key + `</td>
                                                <td class='text-right number3'>`+ quantity + `</td>
                                                <td class='text-right number3'>`+ amount + `</td>
                                            </tr>`

                            htmltbodyheader += htmlcontent;
                            htmltable += htmltbodyheader;

                            quantitys += quantity;
                            amounts += amount;
                        });
                       
                        htmltable += `</tbody>
                                        <tfoot>
                                        <tr>
                                            <th colspan="3">Tổng cộng</th>
                                            <th class='text-right number3'>`+ quantitys + `</th>
                                            <th class='text-right number3'>`+ amounts + `</th>
                                        </tr>
                                     </tfoot>`;
                        htmltable += "</table></div></div>";


                        // table chi tiết theo ngày của hóa đơn
                        htmltableinvoice = `<div class="card"><div class="card-body">
                                        <table class="table table-bordered table-striped" id="datatableDetail" style="width:100%">
                                           <thead>
                                               <tr>
                                                   <th  class='text-center'>Tên hàng</th>
                                                   <th  class='text-center'>ĐVT</th>
                                                   <th  class='text-center'>Mã hóa đơn</th>
                                                   <th  class='text-center'>Ngày bán</th>
                                                   <th  class='text-center'>Khách hàng</th>
                                                   <th  class='text-center'>Mã khách hàng</th>
                                                   <th  class='text-center'>SL</th>
                                                   <th  class='text-center'>Giá bán</th>
                                                   <th  class='text-center'>Chiết khấu</th>
                                                   <th  class='text-center'>Tổng thu</th>
                                               </tr>
                                           </thead>
                                           <tbody>
                                          
                                            `;
                        totals = 0;
                        discountAmounts = 0;
                        quantitys = 0;
                        for (var i = 0; i < res.data.listItemReportDetails.length; i++) {
                            var it = res.data.listItemReportDetails[i];
                          
                            discountAmounts += it.discountAmount;
                            totals += it.total;
                            quantitys += it.quantity;

                            htmltableinvoice += "<td class='text-left'>" + it.productName + "</td>";
                            htmltableinvoice += "<td class='text-center'>" + it.unit + "</td>";
                            htmltableinvoice += "<td class='text-center'>" + it.invoiceNo + "</td>";
                            htmltableinvoice += "<td class='text-center'>" + it.createDate + "</td>";
                            htmltableinvoice += "<td class='text-center'>" + it.buyer + "</td>";
                            htmltableinvoice += "<td class='text-center'>" + it.cusCode + "</td>";
                            htmltableinvoice += "<td class='text-center number3'>" + it.quantity + "</td>";
                            htmltableinvoice += "<td class='text-right number3'>" + it.price + "</td>";
                            htmltableinvoice += "<td class='text-right number3'>" + it.discountAmount + "</td>";
                            htmltableinvoice += "<td class='text-right number3'>" + it.total + "</td>";
                            htmltableinvoice += "</tr>";
                        }
                        htmltableinvoice += `</tbody>
                                        <tfoot>
                                        <tr>
                                            <th colspan="6">Tổng cộng</th>
                                            <th class='text-right number3'>`+ quantitys + `</th>
                                            <th class='text-right'></th>
                                            <th class='text-right number3'>`+ discountAmounts + `</th>
                                            <th class='text-right number3'>`+ totals + `</th>
                                        </tr>
                                     </tfoot>`;
                        htmltableinvoice += "</table></div></div>";
                        // kết thúc
                        // tab
                        htmltabnavtable = `<ul class="nav nav-pills mb-3 mt-3" id="pills-tab"  role="tablist">
                                              <li class="nav-item ">
                                                <a class="nav-link active" id="pills-home-tab" data-toggle="pill" href="#pills-home" role="tab" aria-controls="pills-home" aria-selected="true">Báo cáo theo hàng hóa</a>
                                              </li>
                                              <li class="nav-item reportdate">
                                                <a class="nav-link" id="pills-profile-tab" data-toggle="pill" href="#pills-profile" role="tab" aria-controls="pills-profile" aria-selected="false">Báo cáo theo hàng hóa chi tiết</a>
                                              </li>
                                             
                                            </ul>
                                            <div class="tab-content" id="pills-tabContent">
                                              <div class="tab-pane fade show active" id="pills-home" role="tabpanel" aria-labelledby="pills-home-tab">`+ htmltable + `</div>
                                              <div class="tab-pane fade" id="pills-profile" role="tabpanel" aria-labelledby="pills-profile-tab">`+ htmltableinvoice + `</div>
                                              
                                            </div>`
                            ;
                        //html += "<div id='containerchart' class='card'></div>";
                       
                       
                        $("#contentDatareport").html(htmltabnavtable);
                        
                        //end
                            priceFormat();
                            evetnFormatTextnumber3();
                        // datatable js


                        oTable = $('#datatable').DataTable({
                            "columnDefs": [{ "targets": 1, "type": "date-eu-pre" }],
                            "destroy": true,
                            fixedHeader: true,
                            scrollY: "500px",
                            scrollX: true,
                            scrollCollapse: true,
                            paging: false,
                            searching: true,
                            ordering: false,
                            "info": false,
                            order: [[1, 'desc']],
                            "language": {
                                "emptyTable": "Không tìm thấy dữ liệu"
                            },
                            dom: 'Bfrtip',
                            buttons: [
                                'excel', 'pdf', 'print'
                            ]

                        });
                        $('#pills-tab li').click(function () {
                            if ($(this).hasClass("reportdate") && !$(this).hasClass("showdatatable")) {
                                setTimeout(function () {
                                    $('#datatableDetail').DataTable({
                                        fixedHeader: true,
                                        scrollY: "500px",
                                        scrollX: true,
                                        scrollCollapse: true,
                                        paging: true,
                                        searching: true,
                                        ordering: true,
                                        "info": false,
                                        "columnDefs": [{ "targets": 1, "type": "date-eu-pre" }],
                                        order: [[1, 'desc']],
                                        dom: 'Bfrtip',
                                        buttons: [
                                            'excel', 'pdf', 'print'
                                        ]
                                    }); 
                                }, 200);
                                $(this).addClass("showdatatable");
                            }
                        });
                        }
                        else if (res.typeReportPos == enumTypeReportProduct.MATHANGBANCHAY) {
                            // table gôm theo ngày
                            htmltable = "<div id='containerchart' class='card'></div>";
                            htmltable += `<div class="card mt-3"><div class="card-body">
                                        <table class="table table-bordered table-striped" id="datatable" style="width:100%">
                                           <thead>
                                               <tr>
                                                   <th  class='text-center'>Tên mặt hàng</th>
                                                   <th  class='text-center'>Đơn vị</th>
                                                   <th  class='text-center'>Danh mục</th>
                                                   <th  class='text-center'>Giá bán</th>
                                                   <th  class='text-center'>Số lượng</th>
                                                   <th  class='text-center'>Tổng tiền</th>
                                               </tr>
                                           </thead>
                                           <tbody>
                                            `;
                        
                            for (var i = 0; i < res.data.listItemReports.length; i++) {


                                var it = res.data.listItemReports[i];
                                if (it) {
                                    htmltable += "<tr>";
                                    htmltable += "<td class='text-left'>" + it.productName + "</td>";
                                    htmltable += "<td class='text-center'>" + it.unit + "</td>";
                                    htmltable += "<td class='text-left'>" + it.categoryName + "</td>";
                                    htmltable += "<td class='text-right priceFormat'>" + it.price + "</td>";
                                    htmltable += "<td class='text-center'>" + it.quantity + "</td>";
                                    htmltable += "<td class='text-right priceFormat'>" + it.total + "</td>";
                                    htmltable += "</tr>";
                                }

                            }
                            htmltable += "</tbody></table></div></div>";
                           
                            
                            // chart

                            $("#contentDatareport").html(htmltable);
                            report.showchartsanphambanchay(res.data.charts);
                            //end
                            priceFormat();
                        }
                    }
                },
                error: function (err) {
                    toastr.error(err);
                    console.log(err)
                }
            });

        });
    },
    getreport: function () {
        $(".btn-report").click(function () {
            TypeReport = $("#TypeReport").val();
            rangesDate = $("#rangesDate").val();
            $.ajax({
                type: 'GET',
                //global: false,
                url: "/Selling/ReportPos/GetReportRevenue",
                data: {
                    typeReportPos: TypeReport,
                    rangesDate: rangesDate
                },

                success: function (res) {
                    if (res.isValid) {
                        if (res.typeReportPos == enumTypeReportRevenue.DOANHTHU) {
                             html = ` <div class="row">
                                    <div class="col-lg-12 col-xl-12">
                                        <div class="main-card mb-3 card">
                                            <div class="grid-menu grid-menu-2col">
                                                <div class="no-gutters row">
                                                    <div class="col-sm-3">
                                                        <div class="widget-chart widget-chart-hover">
                                                            <div class="icon-wrapper rounded-circle">
                                                                <div class="icon-wrapper-bg bg-primary">
                                                                </div>
                                                                <i class="fas fa-file-alt text-primary"></i>
                                                            </div>
                                                            <div class="widget-numbers priceFormat">`+ res.data.invoiceAll + `</div>
                                                            <div class="widget-subheading">Tổng hóa đơn</div>
                                                        </div>
                                                    </div>
                                                    <div class="col-sm-3">
                                                        <div class="widget-chart widget-chart-hover">
                                                            <div class="icon-wrapper rounded-circle">
                                                                <div class="icon-wrapper-bg bg-info"></div>
                                                                <i class="far fa-trash-alt text-danger"></i>
                                                            </div>
                                                              <div class="widget-numbers priceFormat">`+ res.data.invoiceCancel + `</div>
                                                            <div class="widget-subheading">Hóa đơn hủy</div>
                                                        </div>
                                                    </div>
                                                    <div class="col-sm-3">
                                                        <div class="widget-chart widget-chart-hover">
                                                            <div class="icon-wrapper rounded-circle">
                                                                <div class="icon-wrapper-bg bg-danger"></div>
                                                                <i class="fas fa-utensils text-info"></i>
                                                            </div>
                                                             <div class="widget-numbers priceFormat">`+ res.data.product + `</div>
                                                            <div class="widget-subheading">Số lượng mặt hàng</div>

                                                        </div>
                                                    </div>
                                                    <div class="col-sm-3">
                                                        <div class="widget-chart widget-chart-hover br-br">
                                                            <div class="icon-wrapper rounded-circle">
                                                                <div class="icon-wrapper-bg bg-success"></div>
                                                                <i class="fas fa-donate"></i>
                                                            </div>
                                                              <div class="widget-numbers priceFormat">`+ res.data.amount + `</div>
                                                            <div class="widget-subheading">Tổng doanh thu</div>

                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>`;
                        // table gôm theo ngày
                        htmltable = `<div class="card"><div class="card-body">
                                        <table class="table table-bordered table-striped" id="datatable" style="width:100%">
                                           <thead>
                                               <tr>
                                                   <th  class='text-center'>Thứ</th>
                                                   <th  class='text-center'>Ngày</th>
                                                   <th  class='text-center'>Tiền hàng</th>
                                                   <th  class='text-center'>Tiền giảm</th>
                                                   <th  class='text-center'>Tiền hủy đơn</th>
                                                   <th  class='text-center'>Phí phụ thu</th>
                                                   <th  class='text-center'>Thuế GTGT</th>
                                                   <th  class='text-center'>Tổng thu</th>
                                               </tr>
                                           </thead>
                                           <tbody>
                                          
                                            `;
                        let totals = 0;
                        let discountAmounts = 0;
                        let totalCancels = 0;
                        let serviceChargeAmounts = 0;
                        let amounts = 0;
                            let vatAmounts = 0;
                        for (var i = 0; i < res.data.listItemReports.length; i++) {

                            htmltable += "<tr>";

                            var it = res.data.listItemReports[i];
                            totals += it.total;
                            discountAmounts += it.discountAmount;
                            totalCancels += it.totalCancel;
                            serviceChargeAmounts += it.serviceChargeAmount;
                            amounts += it.amount;
                            vatAmounts += it.vatAmount;

                            htmltable += "<td class='text-center'>" + it.rankName + "</td>";
                            htmltable += "<td class='text-center'>" + it.createDate + "</td>";
                            htmltable += "<td class='text-right priceFormat'>" + it.total + "</td>";
                            htmltable += "<td class='text-right priceFormat'>" + (it.discountAmount*-1) + "</td>";
                            htmltable += "<td class='text-right priceFormat'>" + (it.totalCancel*-1) + "</td>";
                            htmltable += "<td class='text-right priceFormat'>" + it.serviceChargeAmount + "</td>";
                            htmltable += "<td class='text-right priceFormat'>" + it.vatAmount + "</td>";
                            htmltable += "<td class='text-right priceFormat'>" + it.amount + "</td>";
                            htmltable += "</tr>";
                        }
                        htmltable += `</tbody>
                                        <tfoot>
                                        <tr>
                                            <th colspan="2">Tổng cộng</th>
                                            <th class='text-right priceFormat'>`+ totals + `</th>
                                            <th class='text-right priceFormat'>`+ (discountAmounts*-1) + `</th>
                                            <th class='text-right priceFormat'>`+ (totalCancels*-1) + `</th>
                                            <th class='text-right priceFormat'>`+ serviceChargeAmounts + `</th>
                                            <th class='text-right priceFormat'>`+ vatAmounts + `</th>
                                            <th class='text-right priceFormat'>`+ amounts + `</th>
                                        </tr>
                                     </tfoot>`;
                        htmltable += "</table></div></div>";


                        // table chi tiết theo ngày của hóa đơn
                        htmltableinvoice = `<div class="card"><div class="card-body">
                                        <table class="table table-bordered table-striped" id="datatableDetail" style="width:100%">
                                           <thead>
                                               <tr>
                                                   <th  class='text-center'>Thứ</th>
                                                   <th  class='text-center'>Ngày</th>
                                                   <th  class='text-center'>Mã hóa đơn</th>
                                                   <th  class='text-center'>Khách hàng</th>
                                                   <th  class='text-center'>Bàn/phòng</th>
                                                   <th  class='text-center'>Nhân viên bán</th>
                                                   <th  class='text-center'>Trạng thái đơn</th>
                                                   <th  class='text-center'>Tiền hàng</th>
                                                   <th  class='text-center'>Tiền giảm</th>
                                                   <th  class='text-center'>Phí phụ thu</th>
                                                   <th  class='text-center'>Thuế GTGT</th>
                                                   <th  class='text-center'>Tổng thu</th>
                                               </tr>
                                           </thead>
                                           <tbody>
                                          
                                            `;
                        totals = 0;
                        discountAmounts = 0;
                        serviceChargeAmounts = 0;
                        amounts = 0;
                         vatAmounts = 0;
                        for (var i = 0; i < res.data.listItemReportsByInvoice.length; i++) {
                            var it = res.data.listItemReportsByInvoice[i];
                            if (parseInt(it.status) == 3 || parseInt(it.status) == 4) {
                                htmltableinvoice += "<tr class='cancel'>";
                            } else {
                                htmltableinvoice += "<tr>";
                                discountAmounts += it.discountAmount;
                                serviceChargeAmounts += it.serviceChargeAmount;
                                amounts += it.amount;
                                vatAmounts += it.vatAmount;
                            }
                            totals += it.total;




                            htmltableinvoice += "<td class='text-center'>" + it.rankName + "</td>";
                            htmltableinvoice += "<td class='text-center'>" + it.createDate + "</td>";
                            htmltableinvoice += "<td class='text-center'><a href='javascript:void(0)' data-id=" + it.invoiceId + ">" + it.invoiceCode + "</a></td>";
                            htmltableinvoice += "<td class='text-center'>" + it.buyer + "</td>";
                            htmltableinvoice += "<td class='text-center'>" + it.roomName + "</td>";
                            htmltableinvoice += "<td class='text-center'>" + it.casherName + "</td>";
                            if (parseInt(it.status) == 3 || parseInt(it.status) == 4 || parseInt(it.status) == 6) {
                                htmltableinvoice += "<td class='text-center'><span class='badge badge-danger'>" + it.statusName + "</span></td>";
                            }
                            else if (parseInt(it.status) == 5) {
                                htmltableinvoice += "<td class='text-center'><span class='badge badge-warning'>" + it.statusName + "</span></td>";
                            }

                            else {
                                htmltableinvoice += "<td class='text-center'><span class='badge badge-success'>" + it.statusName + "</span></td>";
                            }


                            htmltableinvoice += "<td class='text-right priceFormat'>" + it.total + "</td>";
                            htmltableinvoice += "<td class='text-right priceFormat'>" + it.discountAmount + "</td>";
                            htmltableinvoice += "<td class='text-right priceFormat'>" + it.serviceChargeAmount + "</td>";
                            htmltableinvoice += "<td class='text-right priceFormat'>" + it.vatAmount + "</td>";
                            htmltableinvoice += "<td class='text-right priceFormat'>" + it.amount + "</td>";
                            htmltableinvoice += "</tr>";
                        }
                        htmltableinvoice += `</tbody>
                                        <tfoot>
                                        <tr>
                                            <th colspan="7">Tổng cộng</th>
                                            <th class='text-right priceFormat'>`+ totals + `</th>
                                            <th class='text-right priceFormat'>`+ discountAmounts + `</th>
                                            <th class='text-right priceFormat'>`+ serviceChargeAmounts + `</th>
                                            <th class='text-right priceFormat'>`+ vatAmounts + `</th>
                                            <th class='text-right priceFormat'>`+ amounts + `</th>
                                        </tr>
                                     </tfoot>`;
                        htmltableinvoice += "</table></div></div>";
                        // kết thúc
                        // tab
                        htmltabnavtable = `<ul class="nav nav-pills mb-3 mt-3" id="pills-tab"  role="tablist">
                                              <li class="nav-item reportdate">
                                                <a class="nav-link" id="pills-home-tab" data-toggle="pill" href="#pills-home" role="tab" aria-controls="pills-home" aria-selected="true">Báo cáo gộp theo ngày</a>
                                              </li>
                                              <li class="nav-item">
                                                <a class="nav-link  active" id="pills-profile-tab" data-toggle="pill" href="#pills-profile" role="tab" aria-controls="pills-profile" aria-selected="false">Báo cáo theo hóa đơn</a>
                                              </li>
                                             
                                            </ul>
                                            <div class="tab-content" id="pills-tabContent">
                                              <div class="tab-pane fade " id="pills-home" role="tabpanel" aria-labelledby="pills-home-tab">`+ htmltable + `</div>
                                              <div class="tab-pane fade show active" id="pills-profile" role="tabpanel" aria-labelledby="pills-profile-tab">`+ htmltableinvoice + `</div>
                                              
                                            </div>`
                            ;
                        html += "<div id='containerchart' class='card'></div>";
                        html += htmltabnavtable;
                        // chart

                        $("#contentDatareport").html(html);
                        report.showchartdoanhthu(res.data.charts);
                        //end
                        priceFormat();
                        // datatable js

                        $('#datatableDetail').DataTable({
                            fixedHeader: true,
                            scrollY: "500px",
                            scrollX: true,
                            scrollCollapse: true,
                            paging: false,
                            searching: true,
                            ordering: false,
                            "info": false,
                            "columnDefs": [{ "targets": 1, "type": "date-eu-pre" }],
                            order: [[1, 'desc']],
                            dom: 'Bfrtip',
                            buttons: [
                                'excel', 'pdf', 'print'
                            ]
                        });
                        $('#pills-tab li').click(function () {
                            if ($(this).hasClass("reportdate") && !$(this).hasClass("showdatatable")) {
                                setTimeout(function () {
                                    oTable = $('#datatable').DataTable({
                                        "columnDefs": [{ "targets": 1, "type": "date-eu-pre" }],
                                        "destroy": true,
                                        fixedHeader: true,
                                        scrollY: "500px",
                                        scrollX: true,
                                        scrollCollapse: true,
                                        paging: false,
                                        searching: true,
                                        ordering: false,
                                        "info": false,
                                        order: [[1, 'desc']],
                                        dom: 'Bfrtip',
                                        buttons: [
                                            'excel', 'pdf', 'print'
                                        ],
                                        "language": {
                                            "emptyTable": "Không tìm thấy dữ liệu"
                                        }

                                    });
                                }, 200);
                                $(this).addClass("showdatatable");
                            }
                        });
                        }
                        else if (res.typeReportPos == enumTypeReportRevenue.HUYDON) {
                            html = "<div id='containerchart' class='card'></div>";
                            htmltable = `<div class="card"><div class="card-body">
                                        <table class="table table-bordered table-striped" id="datatable" style="width:100%">
                                           <thead>
                                               <tr>
                                                   <th  class='text-center'>Thời gian</th>
                                                   <th  class='text-center'>Mã hóa đơn</th>
                                                   <th  class='text-center'>Người hủy</th>
                                                   <th  class='text-center'>Lý do</th>
                                                   <th  class='text-center'>Tổng tiền</th>
                                               </tr>
                                           </thead>
                                           <tbody>
                                          
                                            `;

                            let amounts = 0;
                            for (var i = 0; i < res.data.listItemReports.length; i++) {

                                htmltable += "<tr>";
                                var it = res.data.listItemReports[i];
                                amounts += it.amount;
                                htmltable += "<td class='text-center'>" + it.creatDate + "</td>";
                                htmltable += "<td class='text-center'><a href='javascript:void(0)' data-id=" + it.invoiceId + ">" + it.invoiceNo + "</a></td>";
                                htmltable += "<td class='text-center'>" + it.casherName + "</td>";
                                htmltable += "<td class='text-left'>" + it.note + "</td>";
                                htmltable += "<td class='text-right priceFormat'>" + it.amount + "</td>";
                                htmltable += "</tr>";
                            }
                            htmltable += `</tbody>
                                        <tfoot>
                                        <tr>
                                            <th colspan="4">Tổng cộng</th>
                                            <th class='text-right priceFormat'>`+ amounts + `</th>
                                        </tr>
                                     </tfoot>`;
                            htmltable += "</table></div></div>";
                            html += htmltable;

                            $("#contentDatareport").html(html);
                            if (res.isShowChart) {
                                report.showcharthuydon(res.data.charts);
                            }
                            //end
                            priceFormat();
                            // datatable js

                            $('#datatable').DataTable({
                                fixedHeader: true,
                                scrollY: "500px",
                                scrollX: true,
                                scrollCollapse: true,
                                paging: true,
                                searching: false,
                                ordering: true,
                                dom: 'Bfrtip',
                                buttons: [
                                    'excel', 'pdf', 'print'
                                ],
                                "info": false
                            });
                        }
                        else if (res.typeReportPos == enumTypeReportRevenue.HINHTHUCPHUVU) {
                            html = "<div id='containerchart' class='card'></div>";
                            htmltable = `<div class="card"><div class="card-body">
                                        <table class="table table-bordered table-striped" id="datatable" style="width:100%">
                                           <thead>
                                               <tr>
                                                   <th  class='text-center'>Hình thức</th>
                                                   <th  class='text-center'>Hóa đơn</th>
                                                   <th  class='text-center'>Hóa đơn hủy</th>
                                                   <th  class='text-center'>Tổng thu</th>
                                               </tr>
                                           </thead>
                                           <tbody>
                                          
                                            `;
                         
                            let amounts = 0;
                            for (var i = 0; i < res.data.listItemReports.length; i++) {

                                htmltable += "<tr>";

                                var it = res.data.listItemReports[i];
                                amounts += it.amount;
                                htmltable += "<td class='text-right'><b>" + it.name + "</b></td>";
                                htmltable += "<td class='text-right priceFormat'>" + it.invoicePaymentCount + "</td>";
                                htmltable += "<td class='text-right priceFormat'>" + it.invoiceCancelCount + "</td>";
                                htmltable += "<td class='text-right priceFormat'>" + it.amount + "</td>";
                                htmltable += "</tr>";
                            }
                            htmltable += `</tbody>
                                        <tfoot>
                                        <tr>
                                            <th colspan="3">Tổng cộng</th>
                                            <th class='text-right priceFormat'>`+ amounts + `</th>
                                        </tr>
                                     </tfoot>`;
                            htmltable += "</table></div></div>";
                            html += htmltable;

                            $("#contentDatareport").html(html);
                            report.showcharthinhthucphucvu(res.data.charts);
                            //end
                            priceFormat();
                            // datatable js

                            $('#datatable').DataTable({
                                fixedHeader: true,
                                scrollY: "500px",
                                scrollX: true,
                                scrollCollapse: true,
                                paging: false,
                                searching: false,
                                ordering: true,
                                "info": false,
                                dom: 'Bfrtip',
                                buttons: [
                                    'excel', 'pdf', 'print'
                                ]
                            });
                        }
                    }
                },
                error: function (err) {
                    toastr.error(err);
                    console.log(err)
                }
            });

        });
    },
   showcharthinhthucphucvu: function (data) {
        var newArrKey = new Array();
        var newArrvalue = new Array();
        $.each(data, function (i, item) {
            newArrKey.push(item.key);
            newArrvalue.push(item.value);
        });

        Highcharts.chart('containerchart', {
            chart: {
                type: 'bar'
            },
            title: {
                text: 'Thống kê doanh thu'
            },
            subtitle: {
                text: ''
            },
            xAxis: {
                categories: newArrKey
            },
            yAxis: {
                min: 0,
                title: {
                    text: ''
                }
            },
            legend: {
                reversed: true
            },
            plotOptions: {
                line: {
                    dataLabels: {
                        enabled: true
                    },
                    enableMouseTracking: false
                }
            },
            series: [{
                name: 'Doanh thu',
                data: newArrvalue
            }]
        });

    },
    showchartsanphambanchay: function (data) {
        var newArrKey = new Array();
        var newArrvalue = new Array();
        $.each(data, function (i, item) {
            newArrKey.push(item.key);
            newArrvalue.push(item.value);
        });
        
        Highcharts.chart('containerchart', {
            chart: {
                type: 'bar'
            },
            title: {
                text: 'Báo cáo mặt hàng bán chạy',
                align: 'center'
            },
            subtitle: {
                text: 'SposViet.vn',
                align: 'left'
            },
            xAxis: {
                categories: newArrKey,
                title: {
                    text: null
                }
            },
            yAxis: {
                min: 0,
                title: {
                    text: 'Doanh thu (vnđ)',
                    align: 'high'
                },
                labels: {
                    overflow: 'justify'
                }
            },
            tooltip: {
                valueSuffix: 'đ'
            },
            plotOptions: {
                bar: {
                    dataLabels: {
                        enabled: true
                    }
                }
            },
            legend: {
                reversed: true
                //layout: 'vertical',
                //align: 'right',
                //verticalAlign: 'top',
                //x: -40,
                //y: 80,
                //floating: true,
                //borderWidth: 1,
                //backgroundColor:
                //    Highcharts.defaultOptions.legend.backgroundColor || '#FFFFFF',
                //shadow: true
            },
            credits: {
                enabled: false
            },
            series: [{
                name: 'Doanh thu',
                data: newArrvalue
            }]
        });

    },
    showcharthuydon: function (data) {
        var newArrKey = new Array();
        var newArrvalue = new Array();
        $.each(data, function (i, item) {
            newArrKey.push(item.key);
            newArrvalue.push(item.value);
        });
        
        Highcharts.chart('containerchart', {
            chart: {
                type: 'column'
            },
            title: {
                text: 'Thống kê doanh thu'
            },
            subtitle: {
                text: ''
            },
            xAxis: {
                categories: newArrKey,
                crosshair: true
            },
            yAxis: {
                min: 0,
                title: {
                    text: ''
                },
                
            }, tooltip: {
                headerFormat: '<span style="font-size:10px">{point.key}</span><table>',
                pointFormat: '<tr><td style="color:{series.color};padding:0">{series.name}: </td>' +
                    '<td style="padding:0"><b>{point.y:.1f} mm</b></td></tr>',
                footerFormat: '</table>',
                shared: true,
                useHTML: true
            },
            legend: {
                reversed: true
            },
            plotOptions: {
                line: {
                    dataLabels: {
                        enabled: true
                    },
                    enableMouseTracking: false
                }
            },
            series: [{
                name: 'Doanh thu',
                data: newArrvalue
            }]
        });

    },
    showchartdoanhthu: function (data) {
        var newArrKey = new Array();
        var newArrvalue = new Array();
        $.each(data, function (i, item) {
            newArrKey.push(item.key);
            newArrvalue.push(item.value);
        });

        Highcharts.chart('containerchart', {
            chart: {
                type: 'line'
            },
            title: {
                text: 'Thống kê doanh thu'
            },
            subtitle: {
                text: ''
            },
            xAxis: {
                categories: newArrKey
            },
            yAxis: {
                title: {
                    text: ''
                }
            },
            plotOptions: {
                line: {
                    dataLabels: {
                        enabled: true
                    },
                    enableMouseTracking: false
                }
            },
            series: [{
                name: 'Doanh thu',
                data: newArrvalue
            }]
        });

    },
    getReportEinvoice: function () {
       
        $("#btnSearchEinvoice").click(function (e) {
            e.preventDefault();
            $.ajax({
                //cache: false,
                type: "POST",
                //contentType: 'application/json; charset=utf-8',
                url: '/selling/reportpos/PostReportEInvoiceMonth',
               // contentType: false,
               // processData: false,
                data: {
                    TypeReportEInvoice: $("#typereportEinvoice").val(),
                    rangesDate: $("#rangesDate").val(),
                },
                //contentType: false,
               // contentType: 'application/json; charset=utf-8',
               // processData: false,
                dataType: 'json',
                success: function (r) {
                    if (!r.isValid) {
                        return;
                    }
                    var bytes = Base64ToBytes(r.data);
                    //Convert Byte Array to BLOB.
                  //  var blob = new Blob([bytes], { type: "application/octetstream" });

                    //Check the Browser type and download the File.
                    var blob = new Blob([bytes], { type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
                    var link = document.createElement('a');
                    link.href = window.URL.createObjectURL(blob);
                    link.download = "applicationFileName.xlsx";
                    link.click();

                    link.remove();

                },
                error: function (error) {
                    console.log(JSON.stringify(error));
                }
            });
        })
    }
}
var dashboardreport = {
    eventChangrInput: function () {
        $("#rangesDate").change(function () {
            dashboardreport.reportdoanhthu();
        })
     },
    GetDashBoardIndex: function () {
        
        $.ajax({
            type: 'GET',
            global: false,
            url: "/Selling/Dashboard/GetDashBoardIndex",
            success: function (res) {
                if (res.isValid) {

                   let custang = 0;
                    if (res.customer > 0 && res.customerHomQua > 0) {
                        custang = (res.customer / res.customerHomQua) * 100;
                        if (custang < 0) {
                            custang = 0;
                        }
                    } else if(res.customer > 0 && res.customerHomQua == 0) {
                        custang = 100;
                    }
                    let DONDAXONGtang = 0;
                    if (res.dondaxong > 0 && res.donxonghomqua > 0) {
                        DONDAXONGtang = (res.dondaxong / res.donxonghomqua) * 100;
                        if (res.dondaxong < res.donxonghomqua) {
                            DONDAXONGtang = 0;
                        }
                    }
                    let  DOANHSOtang = 0;
                    if (res.doanhso > 0 && res.doanhsohomqua > 0) {
                        DOANHSOtang = Math.round((res.doanhso / res.doanhsohomqua) * 100);
                        if (res.doanhso < res.doanhsohomqua) {
                            DOANHSOtang = 0;
                        }
                    }
                    let html = `
                            <div class="grid-card gridDashboard">
                                <ul>
                                    <li class="item-grid">
                                        <div class="bg">
                                            <span></span>
                                            <span></span>
                                            <span></span>
                                        </div>
                                        <div class="body">
                                            <div class="header">
                                             <i class="fas fa-users"></i>
                                               <div class="content">
                                                    <h3>Khách hàng</h3>
                                                    <span>`+ res.customer + `</span>
                                                </div>
                                            </div>
                                            <div class="bottom">
                                                <p class="">Hôm qua: `+ res.customerHomQua + ` <span class="float-right"><i class="fas fa-chart-line"></i>` + custang + `%</span></p>
                                            </div>
                                        </div>
                                    </li>
                                    <li class="item-grid">
                                        <div class="bg">
                                            <span></span>
                                            <span></span>
                                            <span></span>
                                        </div>
                                        <div class="body">
                                            <div class="header">
                                                <i class="fas fa-user-edit"></i>
                                                <div class="content">
                                                    <h3>Đơn đang phục vụ</h3>
                                                    <span>`+ res.dondangphucvu + `</span>
                                                </div>
                                            </div>
                                            <div class="bottom">
                                                <p class="">Tổng dự thu: <span class="float-right amount">`+ res.doanhthudondangphucvu.format0VND(3,3)+`</span></p>
                                            </div>
                                        </div>
                                       
                                    </li>
                                    <li class="item-grid">
                                        <div class="bg">
                                            <span></span>
                                            <span></span>
                                            <span></span>
                                        </div>
                                        <div class="body">
                                            <div class="header">
                                               <i class="fas fa-shopping-cart"></i>
                                                <div class="content">
                                                    <h3>Đơn đã thanh toán</h3>
                                                    <span>`+ res.dondaxong.format0VND(0, 3) + `</span>
                                                </div>
                                            </div>
                                            <div class="bottom">
                                                <p class="">Hôm qua: `+ res.donxonghomqua.format0VND(0, 3) + ` <span class="float-right"><i class="fas fa-chart-line"></i>` + DONDAXONGtang + `%</span></p>
                                            </div>
                                        </div>
                                    </li>

                                    <li class="item-grid">
                                        <div class="bg">
                                            <span></span>
                                            <span></span>
                                            <span></span>
                                        </div>
                                        <div class="body">
                                            <div class="header">
                                                <i class="fas fa-donate"></i>
                                                <div class="content">
                                                    <h3>Tổng thu</h3>
                                                    <span>`+ res.doanhso.format0VND(0, 3) + `</span>
                                                </div>
                                            </div>
                                            <div class="bottom">
                                                <p class="">Hôm qua: `+ res.doanhsohomqua.format0VND(0, 3) + ` vnđ <span class="float-right"><i class="fas fa-chart-line"></i>` + DOANHSOtang + `%</span></p>
                                            </div>
                                        </div>
                                    </li>
                                </ul>
                            </div>
                            `;
                    //let html = `<div class="row mt-3">
                    //            <div class="col-xl-3 col-md-6">
                    //               <div class="card">
                    //                  <div class="card-body">
                    //                     <i class="fa fa-info-circle text-muted float-right"></i>
                    //                     <h4 class="mt-0 font-16">Khách hàng</h4>
                    //                        <h2 class="text-primary my-3 text-center"><span><span>`+ res.customer + `</span></span></h2>
                    //                        <p class="text-muted mb-0">Hôm qua:`+ res.customerHomQua + ` <span class="float-right"><i class="me-1 fa fa-caret-up text-success"></i>` + custang +`%</span></p>
                    //                  </div>
                    //               </div>
                    //            </div>
                    //            <div class="col-xl-3 col-md-6">
                    //                <div class="card">
                    //                    <div class="card-body">
                    //                        <i class="fa fa-info-circle text-muted float-right"></i>
                    //                        <h4 class="mt-0 font-16">Đơn đang phục vụ</h4>
                    //                        <h2 class="text-primary my-3 text-center"><span><span>`+ res.dondangphucvu + `</span></span></h2>
                    //                        <p class="mb-0 text-white">Hôm qua: </p>
                    //                    </div>
                    //                </div>
                    //            </div>
                    //            <div class="col-xl-3 col-md-6">
                    //                <div class="card">
                    //                    <div class="card-body">
                    //                        <i class="fa fa-info-circle text-muted float-right"></i>
                    //                        <h4 class="mt-0 font-16">Đơn đã thanh toán</h4>
                    //                        <h2 class="text-primary my-3 text-center"><span><span>`+ res.dondaxong.format0VND(0, 3) + `</span></span></h2>
                    //                        <p class="text-muted mb-0">Hôm qua: `+ res.donxonghomqua.format0VND(0, 3) + ` <span class="float-right"><i class="me-1 fa fa-caret-up text-success"></i>` + DONDAXONGtang + `%</span></p>
                    //                    </div>
                    //                </div>
                    //            </div>
                    //            <div class="col-xl-3 col-md-6">
                    //                <div class="card">
                    //                    <div class="card-body">
                    //                        <i class="fa fa-info-circle text-muted float-right"></i>
                    //                        <h4 class="mt-0 font-16">Tổng thu</h4>
                    //                        <h2 class="text-primary my-3 text-center"><span><span>`+ res.doanhso.format0VND(0, 3) + `vnđ</span></span></h2>
                    //                        <p class="text-muted mb-0">Hôm qua: `+ res.doanhsohomqua.format0VND(0, 3) + `vnđ <span class="float-right"><i class="me-1 fa fa-caret-up text-success"></i>` + DOANHSOtang + `%</span></p>
                    //                    </div>
                    //                </div>
                    //            </div>
                    //        </div>`;
                    $(".indexdashboad").html(html);
                }
            },
            error: function (err) {
                toastr.error(err);
                console.log(err)
            }
        });
    }, reportdoanhthu: function () {
        
        rangesDate = $("#rangesDate").val();
        $.ajax({
            type: 'GET',
            global: false,
            url: "/Selling/ReportPos/GetReportDashBoard",
            data: {
                typeReportPos: enumTypeReportDashboard.DOANHTHU,
                rangesDate: rangesDate
            },
            success: function (res) {
                if (res.isValid) {
                    if (res.typeReportPos == enumTypeReportDashboard.DOANHTHU) {
                        chartDashBoard.chartDoanhthu(res.data.charts);
                    }
                }
            },
            error: function (err) {
                toastr.error(err);
                console.log(err)
            }
        });
    }
}
var chartDashBoard = {
    chartDoanhthu: function (data) {
        var newArrKey = new Array();
        var newArrvalue = new Array();//daonh thu
        var newArrvalue2 = new Array();//tiền thuế
        $.each(data, function (i, item) {
            newArrKey.push(item.key);
            newArrvalue.push(item.value);
            newArrvalue2.push(item.value2);
        });
        Highcharts.setOptions({
            global: {
                useUTC: false,

            },
            lang: {
                decimalPoint: ',',
                thousandsSep: '.'
            }
        });
        Highcharts.chart('reportAverage', {
            chart: {
                type: 'column'
            },
            title: {
                text: 'Báo cáo doanh thu theo ngày'
            },
            subtitle: {
                text: 'Doanh thu'
            },
            xAxis: {
                categories: newArrKey,
                crosshair: true
            },
            yAxis: {
                min: 0,
                title: {
                    text: 'Doanh thu'
                }
            },
            tooltip: {
                headerFormat: '<span style="font-size:10px">{point.key}</span><table>',
                pointFormat: '<tr><td style="color:{series.color};padding:0">{series.name}: </td>' +
                    '<td style="padding:0"><b>{point.y} vnđ</b></td></tr>',
                footerFormat: '</table>',
                shared: true,
                useHTML: true
            },
            plotOptions: {
                column: {
                    pointPadding: 0.2,
                    borderWidth: 0
                }
            },
            series: [{
                name: 'Tổng doanh thu',
                data: newArrvalue

            }, {
                name: 'Tổng tiền thuế',
                data: newArrvalue2
            }]
        });
    }
}

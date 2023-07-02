
//code cho sapo
var cassoScript = document.currentScript;
setTimeout(function () {
    let publicAPI = 'https://pay.casso.vn'
    let url = cassoScript.src;
    const params = new URL(url).searchParams;
    let code = params.get('p');

    let isGetPhone = params.get('phone');
    let isGetFormat = params.get('format');
    let FormatUSER = 'vietqr_net_2';
    if(isGetFormat){
        FormatUSER  = params.get('format');
    }
    let phone = null;
    let dataInfoPurchage = document.querySelector(".section__content--bordered").innerHTML;
    if (!dataInfoPurchage.toLowerCase().includes('vietqr')) return;
    if (isGetPhone && isGetPhone == 'true') {
        console.log(dataInfoPurchage);
        var regexNumber = /\d+/g;
        var matchesNumber = dataInfoPurchage.match(regexNumber);
        for (let p of matchesNumber) {
            if (isVietnamesePhoneNumber(p)) {
                phone = p;
                break;

            }
        }
    }
    let dataUser = {};
    if (code) {
        try {
            dataUser = JSON.parse(decodeBase58(code));
        } catch (error) {
            return;
        }
    }
    let prefix = params.get('prefix') || dataUser.prefix;
    let bankName = params.get('bank_name') || dataUser.bank_name;

    let bankFirst = bankName;
    if (bankName.includes(',')) {
        bankFirst = (bankName.split(',')[0]).toLowerCase();
    }
    if (bankName.includes(' | ')) {
        bankFirst = (bankName.split(' | ')[0]).toLowerCase();
    }
    if (dataUser.bin) {
        bankFirst = dataUser.bin;
        let bank = banks.filter(el => el.bin == dataUser.bin);
        bankName = bank[0].short_name;

    }

    let accountNumber = params.get('account_number') || dataUser.account_number;
    let accountName = params.get('account_name') || dataUser.account_name || "Đang cập nhật";
    let amount = document.querySelector(".payment-due__price").innerHTML;
    let code_orders = document.querySelector(".order-summary__title").innerHTML;
    code_orders = prefix + code_orders.match(/\d+/)[0];
    if (phone) {
        code_orders = code_orders + ' ' + phone;
    }
    let amountNumber = amount.split('.').join('').replace('₫', '');
    let imageUrl = encodeURI(`https://api.vietqr.io/${bankFirst}/${accountNumber.replace(/ +/g, "")}/${amountNumber}/${code_orders}/${FormatUSER}.jpg?accountName=${accountName}`);
    var div = document.createElement('div');
    div.innerHTML = `
    <style>
    .box {
        width: 50%;
        margin: 0 auto;
        background: rgba(255, 255, 255, 0.2);
        padding: 35px;
        border: 2px solid #fff;
        border-radius: 20px/50px;
        background-clip: padding-box;
        text-align: center;
    }
    .button {
        font-size: 1em;
        padding: 10px;
        color: #fff;
        border: 2px solid #06D85F;
        border-radius: 20px/50px;
        text-decoration: none;
        cursor: pointer;
        transition: all 0.3s ease-out;
    }
    .button:hover {
        background: #06D85F;
    }
    .overlay {
        position: fixed;
        top: 0;
        bottom: 0;
        left: 0;
        right: 0;
        background: rgba(0, 0, 0, 0.7);
        transition: opacity 500ms;
        visibility: hidden;
        opacity: 0;
        z-index: 100;
    }
    .overlay:target {
        visibility: visible;
        opacity: 1;
    }
    .vietqr__popup {
        margin: 70px auto;
        padding: 20px;
        border-radius: 5px;
        width: 50%;
        position: relative;
        transition: all 2s ease-in-out;
    }
    .vietqr__popup h2 {
        margin-top: 0;
        color: #333;
        font-family: Tahoma, Arial, sans-serif;
    }
    .vietqr__popup .close {
        position: absolute;
        top: 20px;
        right: 30px;
        transition: all 200ms;
        font-size: 30px;
        font-weight: bold;
        text-decoration: none;
        color: #333;
    }
    .badge {
        position: absolute;
        top: -25px;
        right: -5px;
        display:inline-block;
        min-width:10px;
        padding:3px 7px;
        font-size:12px;
        font-weight:700;
        line-height:1;
        color:#fff;
        text-align:center;
        white-space:nowrap;
        vertical-align:baseline;
        background-color:#fb3333;
        border-radius:10px;
    }
    .vietqr__popup .close:hover {
        color: #06D85F;
    }
    .vietqr__popup .content {
        max-height: 30%;
        overflow: auto;
    }
    .modal{
        max-width: 700px;
        width: 100%;
        background-color: #fdfdfd !important;
    }
    .qrcode__header{
        margin-top: 15px;
    }
    .qrcode__footer{
        margin-bottom: 15px;
    }
    .btn_copy__cassso2{
        font-size: 12px;
        cursor: pointer;
        width: 90px;
        margin-bottom: 13px;
        background: #96ce9d33;
        padding: 6px;
    }
    .border__payment_casso_left{
        border: 1px solid #0000000f;
        border-radius: 8px;
        margin-right: 10px;
    }
    .border__payment_casso_right{
        border: 1px solid #0000000f;
        border-top-right-radius: 8px;
        border-radius: 8px;
    }
    .icon__copy_content_casso{
        cursor: pointer;
        color: #12a212;
    }
    .show_fix_download_image_casso{
        position: fixed;
        bottom: 0;
        left: 0;
        width: 100%;
        height: 30px;
        z-index: 10;
        background: #eeeeee;
        display: none;
    }
    .data__in_table__cassso{
        font-weight: bold;
        margin-bottom: 13px;
        float: left;
    }
    .loader {
        border: 8px solid #f3f3f3; /* Light grey */
        border-top: 8px solid #3498db; /* Blue */
        border-radius: 50%;
        width: 20px;
        height: 20px;
        animation: spin 2s linear infinite;
      }

      
      @keyframes spin {
        0% { transform: rotate(0deg); }
        100% { transform: rotate(360deg); }
      }
      @media screen and (max-width: 1200px) {
        .vietqr__popup {
            width: 60%;
        }
      }
    @media screen and (max-width: 1100px) {
        .border__payment_casso_left{
            margin-right: 0px;
            margin-bottom: 10px;
        }
        .modal-body{
            display: block !important;
         }
        .vietqr__popup {
            height: 290%;
            overflow: auto;
            width: 65%;
        }
    }
    @media screen and (max-width: 700px) {
        .box {
            width: 70%;
        }
        #temp__casso {
            max-width: 90px;
        }
        .vietqr__popup {
            height: 300%;
            overflow: auto;
            width: 100%;
        }
        .modal{
            width: 90% !important;
        }
        .border__payment_casso_right{
            border-top-right-radius: 0px;

        }
        .border__payment_casso_left{
            border: 1px solid #0000000f;
            border-bottom-left-radius: 0px;
    
        }
        .show_fix_download_image_casso{
            display: block;
        }
        .fix__button_paid{
            margin-bottom: 25px
        }
        
    }
</style>
    <a class="btn btn-default" id="onShowPaymentCasso" href="#popup2">Hiển thị thông tin chuyển khoản</a>
<div id="popup2" class="overlay">
    <div class="vietqr__popup">

        <div class="content" >
        
            <div class="modal" style="display: inline-block; margin-bottom: 20px;">
           
                <div class="modal-header">
                    <h2 class="modal-title">Chuyển khoản ngân hàng</h2>
                    <a class="close" href="#">&times;</a>
                </div>
                <div class="modal-body qr">
                    <div class="qrcode border__payment_casso_left">
                 
                        <div class="qrcode__header" style="position: relative">
                        <span class="badge">NEW</span>
                            <h4 style="margin-bottom: 5px;"><span style="font-weight: bold;">Cách 1:</span> Chuyển khoản bằng mã QR
                            
                            </h4>
                          
                            <div style = "margin-bottom: 10px;font-weight: 320;font-size: 95%;">Mở App Ngân Hàng Quét QRCode</div>
                            
                        </div>
                        <div class="qrcode__main" style="margin-bottom: 5px;">
                            <img src="${imageUrl}"
                                alt="qr code" class="qrcode__image"></br>
                                <a href="${imageUrl}" download target="_blank">Tải xuống</a>
                        </div>
                        <div class="qrcode__footer">21 App ngân hàng hỗ trợ Quét Mã VietQR</div>
                        
                    </div>

                    <div class="qrcode border__payment_casso_right">
                        <div class="qrcode__header">
                            <h4 style="margin-bottom: 5px;text-align: -webkit-auto; margin-bottom: 20px"><span style="font-weight: bold;">Cách 2: </span> Chuyển khoản THỦ CÔNG theo thông tin
                            </h4>
                        </div>


                        <table>
                        <tr>
                            <td style="width: 90px;"> <div style="text-align: -webkit-auto;margin-bottom: 13px;font-weight: 320;">Ngân hàng:</div></td>
                            <td><div class='data__in_table__cassso'>${bankName.toUpperCase()} </div></td>
                            <td></td>
                            <td></td>
                        </tr>
                        <tr>
                            <td style="width: 90px;"> <div style="text-align: -webkit-auto;margin-bottom: 13px;font-weight: 320;">Chủ tài khoản: </div></td>
                            <td><div class='data__in_table__cassso' style="text-align: left;">${accountName.toUpperCase()}</div></td>
                            <td></td>
                            <td></td>
                        </tr>

                        <tr>
                            <td style="width: 90px;"> <div style="text-align: -webkit-auto;margin-bottom: 13px;font-weight: 320;">Số tài khoản: </div></td>
                            <td><div class='data__in_table__cassso'>${accountNumber}</div></td>
                            <td><span class="icon__copy_content_casso" id="copy_acc_num_banking_casso"><button type="button" class="btn_copy__cassso2" id="copy_acc_num_banking_casso_btn">Sao chép</button></span></td>
                            <td></td>
                        </tr>
                        <tr>
                            <td style="width: 90px;"> <div style="text-align: -webkit-auto;margin-bottom: 13px;font-weight: 320;">Số tiền: </div></td>
                            <td><div class='data__in_table__cassso'>${amount}</div></td>
                            <td><span class="icon__copy_content_casso" id="copy_amount_banking_casso"><button type="button" class="btn_copy__cassso2" id="copy_amount_banking_casso_btn">Sao chép</button></span></td>
                            <td></td>
                        </tr>
                        <tr>
                            <td style="width: 90px;" class="fix__casso_width_content"> <div style="text-align: -webkit-auto;margin-bottom: 13px;font-weight: 320;">Nội dung: </div></td>
                            <td style="float: left;"><div style="
                            padding: 2px;
                            border: 2px solid #06dc52;
                            color: #06dc52;
                            border-radius: 3px;
                            /* max-width: 90px; */
                            text-align: left;
                            margin-bottom: 13px;
                            display: inline-block;
                            " id="temp__casso">${code_orders}</div></td>
                            <td><span id="copy_content_banking_casso" class="icon__copy_content_casso"><button type="button" id="copy_content_banking_casso_btn" class="btn_copy__cassso2">Sao chép</button></span></td>
                            <td></td>
                        </tr>
                        </table>
                        
                        
                        
                        <div  style="text-align: -webkit-auto;margin-bottom: 13px;font-weight: 320;margin-top: 10px;">Lưu ý: Nhập chính xác nội dung <span style="font-weight: bold;display: inline-block;">${code_orders}</span> khi chuyển khoản. Bạn sẽ nhận được email (hoặc SMS) xác nhận khi giao dịch thành công</div>
                        <div style="padding: 10px 30px; display:none;" id="users__paid_casso">
                        <div class="loader" style = "margin: auto;"></div>
                        <p style="padding: 10px;
                        background: aliceblue;
                        border-radius: 7px;">
                        Đơn hàng của bạn sẽ được xác nhận thanh toán tự động trong 1-5 phút tới, chúng tôi sẽ gửi email xác nhận khi đã hoàn tất
                        </p>
                        </div>
                        </div>
                    <div class="qrcode__footer"></div>
                </div>

                <div class="modal-footer fix__button_paid" id="button__paid_casso" style="${dataUser.bin && dataUser.bin == '970415' ? '' : 'display:none'}">
                    <a class="btn btn-default">Tôi đã thanh toán</a>
                </div>
                <div class="modal-footer fix__button_paid" style="${dataUser.bin && dataUser.bin == '970415' ? 'display:none' : ''}">
                <a class="btn btn-default" href="#">Đóng</a>
            </div>
            </div>
        </div>
    </div>
</div>


    `;
    document.getElementsByClassName('thankyou-message-container')[0].appendChild(div);
    setTimeout(function () {
        document.getElementById("onShowPaymentCasso").click();
        document.getElementById("copy_content_banking_casso").addEventListener("click", function () {
            navigator.clipboard.writeText(code_orders);
            document.getElementById('copy_content_banking_casso_btn').innerHTML = 'Đã sao chép';
            setTimeout(function () {
                document.getElementById('copy_content_banking_casso_btn').innerHTML = 'Sao chép';
            }, 1500);
        });
        document.getElementById("copy_acc_num_banking_casso").addEventListener("click", function () {
            navigator.clipboard.writeText(accountNumber);
            document.getElementById('copy_acc_num_banking_casso_btn').innerHTML = 'Đã sao chép';
            setTimeout(function () {
                document.getElementById('copy_acc_num_banking_casso_btn').innerHTML = 'Sao chép';
            }, 1500);
        });
        document.getElementById("copy_amount_banking_casso").addEventListener("click", function () {
            navigator.clipboard.writeText(amountNumber);
            document.getElementById('copy_amount_banking_casso_btn').innerHTML = 'Đã sao chép';
            setTimeout(function () {
                document.getElementById('copy_amount_banking_casso_btn').innerHTML = 'Sao chép';
            }, 1500);
        });
        document.getElementById("button__paid_casso").addEventListener("click", function () {
            document.getElementById('users__paid_casso').style.display = "block";
            fetchStatusAPI();
        });
        function fetchStatusAPI() {
            let hasTrans = false;
            let timeTemp = 0;
            setInterval(function () {
                if (timeTemp > 100 || hasTrans) {
                    return;
                }
                let data = {
                    account_number: parseInt(accountNumber),
                    content: code_orders,
                    amount: parseInt(amountNumber)
                };
                //   console.log(data);
                jQuery.ajax({
                    url: `${publicAPI}/public/check_new_transaction`,
                    type: "post",
                    data: data,
                    error: function (response) {
                        // console.log(response);
                    },
                    success: function (response) {
                        console.log(response);
                        let { data } = response;
                        data = JSON.parse(decodeBase58(data));
                        if (data.hasTransaction == true) {
                            let message__casso = 'Bạn đã thanh toán thành công.';
                            let image__casso = 'https://icon-library.com/images/418329c59c.png';
                            document.getElementById('users__paid_casso').innerHTML = ""
                            var newDiv = document.createElement('div');
                            if (data.difference > 2000) {
                                message__casso = `Bạn đã thanh toán thiếu ${data.difference}. Có thể liên hệ với chúng tôi để được hỗ trợ`;
                                image__casso = 'https://image.flaticon.com/icons/png/512/63/63436.png';
                            }
                            newDiv.innerHTML = `<div>
                            <img src="${image__casso}" width="50" height="50">
                            <p style="padding: 10px 4px;background: aliceblue;border-radius: 7px;color: #06dc52; border: 1.5px solid;">${message__casso}</p > 
                            </div > `;
                            document.getElementById("users__paid_casso").appendChild(newDiv);
                            document.getElementById('button__paid_casso').style.display = "none";
                            hasTrans = true;
                        }
                    }
                });
                console.log(hasTrans);
                timeTemp = timeTemp + 3;
            }, 3000);
        }
    }, 200);


}, 200);
const banks = [{ "id": 2, "name": "Ngân hàng TMCP Á Châu", "code": "ACB", "bin": "970416", "short_name": "ACB", "logo": "https://vietqr.net/img/ACB.6e7fe025.png", "vietqr": 3 }, { "id": 47, "name": "Ngân hàng TMCP Việt Nam Thịnh Vượng", "code": "VPB", "bin": "970432", "short_name": "VPBank", "logo": "https://vietqr.net/img/VPB.ca2e7350.png", "vietqr": 3 }, { "id": 21, "name": "Ngân hàng TMCP Quân đội", "code": "MB", "bin": "970422", "short_name": "MBBank", "logo": "https://vietqr.net/img/MB.f9740319.png", "vietqr": 3 }, { "id": 39, "name": "Ngân hàng TMCP Tiên Phong", "code": "TPB", "bin": "970423", "short_name": "TPBank", "logo": "https://vietqr.net/img/TPB.883b6135.png", "vietqr": 3 }, { "id": 22, "name": "Ngân hàng TMCP Hàng Hải", "code": "MSB", "bin": "970426", "short_name": "MSB", "logo": "https://vietqr.net/img/MSB.1b076e2a.png", "vietqr": 3 }, { "id": 23, "name": "Ngân hàng TMCP Nam Á", "code": "NAB", "bin": "970428", "short_name": "NamABank", "logo": "https://vietqr.net/img/NAB.f74b0fa8.png", "vietqr": 3 }, { "id": 20, "name": "Ngân hàng TMCP Bưu Điện Liên Việt", "code": "LPB", "bin": "970449", "short_name": "LienVietPostBank", "logo": "https://vietqr.net/img/LPB.07a7c83b.png", "vietqr": 3 }, { "id": 43, "name": "Ngân hàng TMCP Ngoại Thương Việt Nam", "code": "VCB", "bin": "970436", "short_name": "Vietcombank", "logo": "https://vietqr.net/img/VCB.237d4924.png", "vietqr": 2 }, { "id": 17, "name": "Ngân hàng TMCP Công thương Việt Nam", "code": "ICB", "bin": "970415", "short_name": "VietinBank", "logo": "https://vietqr.net/img/ICB.3d4d6760.png", "vietqr": 2 }, { "id": 44, "name": "Ngân hàng TMCP Bản Việt", "code": "VCCB", "bin": "970454", "short_name": "Timo", "logo": "https://vietqr.net/img/VCCB.654a3506.png", "vietqr": 2 }, { "id": 4, "name": "Ngân hàng TMCP Đầu tư và Phát triển Việt Nam", "code": "BIDV", "bin": "970418", "short_name": "BIDV", "logo": "https://vietqr.net/img/BIDV.862fd58b.png", "vietqr": 1 }, { "id": 36, "name": "Ngân hàng TMCP Sài Gòn Thương Tín", "code": "STB", "bin": "970403", "short_name": "Sacombank", "logo": "https://vietqr.net/img/STB.a03fef2c.png", "vietqr": 1 }, { "id": 33, "name": "Ngân hàng TMCP Đông Nam Á", "code": "SEAB", "bin": "970440", "short_name": "SeABank", "logo": "https://vietqr.net/img/SEAB.1864a665.png", "vietqr": 1 }, { "id": 45, "name": "Ngân hàng TMCP Quốc tế Việt Nam", "code": "VIB", "bin": "970441", "short_name": "VIB", "logo": "https://vietqr.net/img/VIB.4ecb28e6.png", "vietqr": 1 }, { "id": 42, "name": "Ngân hàng Nông nghiệp và Phát triển Nông thôn Việt Nam", "code": "VBA", "bin": "970405", "short_name": "Agribank", "logo": "https://vietqr.net/img/VBA.d72a0e06.png", "vietqr": 0 }, { "id": 38, "name": "Ngân hàng TMCP Kỹ thương Việt Nam", "code": "TCB", "bin": "970407", "short_name": "Techcombank", "logo": "https://vietqr.net/img/TCB.b2828982.png", "vietqr": 0 }, { "id": 34, "name": "Ngân hàng TMCP Sài Gòn Công Thương", "code": "SGICB", "bin": "970400", "short_name": "SaigonBank", "logo": "https://vietqr.net/img/SGICB.5886546f.png", "vietqr": 0 }, { "id": 9, "name": "Ngân hàng TMCP Đông Á", "code": "DOB", "bin": "970406", "short_name": "DongABank", "logo": "https://vietqr.net/img/DOB.92bbf6f4.png", "vietqr": 0 }, { "id": 11, "name": "Ngân hàng Thương mại TNHH MTV Dầu Khí Toàn Cầu", "code": "GPB", "bin": "970408", "short_name": "GPBank", "logo": "https://vietqr.net/img/GPB.29bd127d.png", "vietqr": 0 }, { "id": 3, "name": "Ngân hàng TMCP Bắc Á", "code": "BAB", "bin": "970409", "short_name": "BacABank", "logo": "https://vietqr.net/img/BAB.75c3a8c2.png", "vietqr": 0 }, { "id": 32, "name": "Ngân hàng TNHH MTV Standard Chartered Bank Việt Nam", "code": "SCVN", "bin": "970410", "short_name": "StandardChartered", "logo": "https://vietqr.net/img/SCVN.a53976be.png", "vietqr": 0 }, { "id": 30, "name": "Ngân hàng TMCP Đại Chúng Việt Nam", "code": "PVCB", "bin": "970412", "short_name": "PVcomBank", "logo": "https://vietqr.net/img/PVCB.6129f342.png", "vietqr": 0 }, { "id": 27, "name": "Ngân hàng Thương mại TNHH MTV Đại Dương", "code": "Oceanbank", "bin": "970414", "short_name": "Oceanbank", "logo": "https://vietqr.net/img/OCEANBANK.f84c3119.png", "vietqr": 0 }, { "id": 24, "name": "Ngân hàng TMCP Quốc Dân", "code": "NCB", "bin": "970419", "short_name": "NCB", "logo": "https://vietqr.net/img/NCB.7d8af057.png", "vietqr": 0 }, { "id": 48, "name": "Ngân hàng Liên doanh Việt - Nga", "code": "VRB", "bin": "970421", "short_name": "VRB", "logo": "https://vietqr.net/img/VRB.9d6d40f3.png", "vietqr": 0 }, { "id": 37, "name": "Ngân hàng TNHH MTV Shinhan Việt Nam", "code": "SHBVN", "bin": "970424", "short_name": "ShinhanBank", "logo": "https://vietqr.net/img/SHBVN.b6c0e806.png", "vietqr": 0 }, { "id": 1, "name": "Ngân hàng TMCP An Bình", "code": "ABB", "bin": "970425", "short_name": "ABBANK", "logo": "https://vietqr.net/img/ABB.9defb03d.png", "vietqr": 0 }, { "id": 41, "name": "Ngân hàng TMCP Việt Á", "code": "VAB", "bin": "970427", "short_name": "VietABank", "logo": "https://vietqr.net/img/VAB.9bf85d8e.png", "vietqr": 0 }, { "id": 31, "name": "Ngân hàng TMCP Sài Gòn", "code": "SCB", "bin": "970429", "short_name": "SCB", "logo": "https://vietqr.net/img/SCB.5ca4bec4.png", "vietqr": 0 }, { "id": 29, "name": "Ngân hàng TMCP Xăng dầu Petrolimex", "code": "PGB", "bin": "970430", "short_name": "PGBank", "logo": "https://vietqr.net/img/PGB.825cbbda.png", "vietqr": 0 }, { "id": 10, "name": "Ngân hàng TMCP Xuất Nhập khẩu Việt Nam", "code": "EIB", "bin": "970431", "short_name": "Eximbank", "logo": "https://vietqr.net/img/EIB.ae2f0252.png", "vietqr": 0 }, { "id": 46, "name": "Ngân hàng TMCP Việt Nam Thương Tín", "code": "VIETBANK", "bin": "970433", "short_name": "VietBank", "logo": "https://vietqr.net/img/VIETBANK.bb702d50.png", "vietqr": 0 }, { "id": 18, "name": "Ngân hàng TNHH Indovina", "code": "IVB", "bin": "970434", "short_name": "IndovinaBank", "logo": "https://vietqr.net/img/IVB.ee79782c.png", "vietqr": 0 }, { "id": 12, "name": "Ngân hàng TMCP Phát triển Thành phố Hồ Chí Minh", "code": "HDB", "bin": "970437", "short_name": "HDBank", "logo": "https://vietqr.net/img/HDB.4256e826.png", "vietqr": 0 }, { "id": 5, "name": "Ngân hàng TMCP Bảo Việt", "code": "BVB", "bin": "970438", "short_name": "BaoVietBank", "logo": "https://vietqr.net/img/BVB.2b7aab15.png", "vietqr": 0 }, { "id": 28, "name": "Ngân hàng TNHH MTV Public Việt Nam", "code": "PBVN", "bin": "970439", "short_name": "PublicBank", "logo": "https://vietqr.net/img/PBVN.67dbc9af.png", "vietqr": 0 }, { "id": 35, "name": "Ngân hàng TMCP Sài Gòn - Hà Nội", "code": "SHB", "bin": "970443", "short_name": "SHB", "logo": "https://vietqr.net/img/SHB.665daa27.png", "vietqr": 0 }, { "id": 6, "name": "Ngân hàng Thương mại TNHH MTV Xây dựng Việt Nam", "code": "CBB", "bin": "970444", "short_name": "CBBank", "logo": "https://vietqr.net/img/CBB.5b47e56f.png", "vietqr": 0 }, { "id": 26, "name": "Ngân hàng TMCP Phương Đông", "code": "OCB", "bin": "970448", "short_name": "OCB", "logo": "https://vietqr.net/img/OCB.84d922d1.png", "vietqr": 0 }, { "id": 19, "name": "Ngân hàng TMCP Kiên Long", "code": "KLB", "bin": "970452", "short_name": "KienLongBank", "logo": "https://vietqr.net/img/KLB.23902895.png", "vietqr": 0 }, { "id": 7, "name": "Ngân hàng TNHH MTV CIMB Việt Nam", "code": "CIMB", "bin": "422589", "short_name": "CIMB", "logo": "https://vietqr.net/img/CIMB.70b35f80.png", "vietqr": 0 }, { "id": 14, "name": "Ngân hàng TNHH MTV HSBC (Việt Nam)", "code": "HSBC", "bin": "458761", "short_name": "HSBC", "logo": "https://vietqr.net/img/HSBC.6fa79196.png", "vietqr": 0 }, { "id": 8, "name": "DBS Bank Ltd - Chi nhánh Thành phố Hồ Chí Minh", "code": "DBS", "bin": "796500", "short_name": "DBSBank", "logo": "https://vietqr.net/img/DBS.83742b1e.png", "vietqr": 0 }, { "id": 25, "name": "Ngân hàng Nonghyup - Chi nhánh Hà Nội", "code": "NHB HN", "bin": "801011", "short_name": "Nonghyup", "logo": "https://vietqr.net/img/NHB%20HN.6a3f7952.png", "vietqr": 0 }, { "id": 13, "name": "Ngân hàng TNHH MTV Hong Leong Việt Nam", "code": "HLBVN", "bin": "970442", "short_name": "HongLeong", "logo": "https://vietqr.net/img/HLBVN.4a284a9a.png", "vietqr": 0 }, { "id": 15, "name": "Ngân hàng Công nghiệp Hàn Quốc - Chi nhánh Hà Nội", "code": "IBK - HN", "bin": "970455", "short_name": "IBK Bank", "logo": "https://vietqr.net/img/IBK%20-%20HN.eee4e569.png", "vietqr": 0 }, { "id": 16, "name": "Ngân hàng Công nghiệp Hàn Quốc - Chi nhánh TP. Hồ Chí Minh", "code": "IBK - HCM", "bin": "970456", "short_name": "IBK Bank", "logo": "https://vietqr.net/img/IBK%20-%20HN.eee4e569.png", "vietqr": 0 }, { "id": 49, "name": "Ngân hàng TNHH MTV Woori Việt Nam", "code": "WVN", "bin": "970457", "short_name": "Woori", "logo": "https://vietqr.net/img/WVN.45451999.png", "vietqr": 0 }, { "id": 40, "name": "Ngân hàng United Overseas - Chi nhánh TP. Hồ Chí Minh", "code": "UOB", "bin": "970458", "short_name": "United Overseas", "logo": "https://vietqr.net/img/UOB.e6a847d2.png", "vietqr": 0 }, { "id": 50, "name": "Ngân hàng Kookmin – Chi nhánh Hà Nội", "code": "KBHN", "bin": "970462", "short_name": "Kookmin", "logo": "https://vietqr.net/img/KBHN.5126abce.png", "vietqr": 0 }];
function decodeBase58(strDecode) {
    var ALPHABET, ALPHABET_MAP, Base58, i;
    Base58 = (typeof module !== "undefined" && module !== null ? module.exports : void 0) || (window.Base58 = {});
    ALPHABET = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";
    ALPHABET_MAP = {};
    i = 0;
    while (i < ALPHABET.length) {
        ALPHABET_MAP[ALPHABET.charAt(i)] = i;
        i++;
    }
    Base58.decode = function (string) {
        var bytes, c, carry, j;
        if (string.length === 0) {
            return [];
        }
        i = void 0;
        j = void 0;
        bytes = [0];
        i = 0;
        while (i < string.length) {
            c = string[i];
            if (!(c in ALPHABET_MAP)) {
                throw "Base58.decode received unacceptable input. Character '" + c + "' is not in the Base58 alphabet.";
            }
            j = 0;
            while (j < bytes.length) {
                bytes[j] *= 58;
                j++;
            }
            bytes[0] += ALPHABET_MAP[c];
            carry = 0;
            j = 0;
            while (j < bytes.length) {
                bytes[j] += carry;
                carry = bytes[j] >> 8;
                bytes[j] &= 0xff;
                ++j;
            }
            while (carry) {
                bytes.push(carry & 0xff);
                carry >>= 8;
            }
            i++;
        }
        i = 0;
        while (string[i] === "1" && i < string.length - 1) {
            bytes.push(0);
            i++;
        }
        return bytes.reverse();
    };
    for (var r = Base58.decode(strDecode), o = "", e = 0; e < r.length; e++)o += String.fromCharCode(r[e]);
    return o
}
function isVietnamesePhoneNumber(number) {
    return /([\+84|84|0]+(3|5|7|8|9|1[2|6|8|9]))+([0-9]{8})\b/.test(number);
}





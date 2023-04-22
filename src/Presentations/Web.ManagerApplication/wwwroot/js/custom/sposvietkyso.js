var timeOut = 2000;

var webSocket = null;
var IdPort = 0;
var timer;

function getbrowse() {
    var ua = navigator.userAgent;
    var tem;
    var M = ua.match(/(opera|chrome|safari|firefox|msie|trident(?=\/))\/?\s*(\d+)/i) || [];
    if (/trident/i.test(M[1])) {
        tem = /\brv[ :]+(\d+)/g.exec(ua) || [];
        return 'IE ' + (tem[1] || '');
    }
    if (M[1] === 'Chrome') {
        tem = ua.match(/\b(OPR|Edge)\/(\d+)/);
        if (tem != null) return tem.slice(1).join(' ').replace('OPR', 'Opera');
    }
    M = M[2] ? [M[1], M[2]] : [navigator.appName, navigator.appVersion, '-?'];
    if ((tem = ua.match(/version\/(\d+)/i)) != null) M.splice(1, 1, tem[1]);
    return {
        name: M[0],
        version: M[1]
    };
};

function getosname() {
    var OSName = "Unknown";
    console.log(window.navigator.userAgent)
    if (window.navigator.userAgent.indexOf("Windows NT 10.0") != -1) OSName = "Windows 10";
    if (window.navigator.userAgent.indexOf("Windows NT 6.2") != -1) OSName = "Windows 8";
    if (window.navigator.userAgent.indexOf("Windows NT 6.1") != -1) OSName = "Windows 7";
    if (window.navigator.userAgent.indexOf("Windows NT 6.0") != -1) OSName = "Windows Vista";
    if (window.navigator.userAgent.indexOf("Windows NT 5.1") != -1) OSName = "Windows XP";
    if (window.navigator.userAgent.indexOf("Windows NT 5.0") != -1) OSName = "Windows 2000";
    if (window.navigator.userAgent.indexOf("Mac") != -1) OSName = "MAC";
    if (window.navigator.userAgent.indexOf("X11") != -1) OSName = "UNIX";
    if (window.navigator.userAgent.indexOf("Linux") != -1) OSName = "Linux";
    return OSName;
};
function getOS() {
    var userAgent = window.navigator.userAgent,
        platform = window.navigator.platform,
        macosPlatforms = ['Macintosh', 'MacIntel', 'MacPPC', 'Mac68K'],
        windowsPlatforms = ['Win32', 'Win64', 'Windows', 'WinCE'],
        iosPlatforms = ['iPhone', 'iPad', 'iPod'],
        os = null;

    if (macosPlatforms.indexOf(platform) !== -1) {
        os = 'Mac OS';
    } else if (iosPlatforms.indexOf(platform) !== -1) {
        os = 'iOS';
    } else if (windowsPlatforms.indexOf(platform) !== -1) {
        os = 'Windows';
    } else if (/Android/.test(userAgent)) {
        os = 'Android';
    } else if (!os && /Linux/.test(platform)) {
        os = 'Linux';
    }

    return os;
}

var sposvietplugin = {
    sendConnectSocket: function (port) {
        data = {};
        data.type = TypeEventWebSocket.SendTestConnect;
        data.hash = "";
        var json = JSON.stringify(data);
        return sposvietplugin.connectSignatureWebSocket(port, json);
    },
    againConnect: function (port,data, resolve) {
        if (IdPort < 5) {
            sposvietplugin.connectSignatureWebSocket(port, data).then(function (data) {
                resolve(data);
            });
        } else {
            clearTimeout(timer); // clear 	  
        }
    },
    connectSignatureWebSocket: function (port, data) {
        return new Promise((resolve, reject) => {
            if (webSocket == null) {
                webSocket = new WebSocket("ws://127.0.0.1:" + port + "/SposVietPlugin");
           
                //if (webSocket == null) {
                timer = setTimeout(function () {
                    IdPort++;
                    sposvietplugin.againConnect(port, data, resolve);
                }, timeOut);
               // }
            }
            else {
                //  ddaonj này là gửi lại lần sau
               
                webSocket.send(data);
            }

            // kết nối thành công
            webSocket.onopen = function () {
                
                StatusSposVietPlugin = 1;
                clearTimeout(timer); // xóa đi nhé 	 
                webSocket.send(data);

            };
            // khi đóng
            webSocket.onclose = function () {
                StatusSposVietPlugin = 0;
                webSocket = null;
               
               
            };
            // nhận dự liệu
            webSocket.onmessage = function (message) {
                var json = JSON.parse(message.data);
                if (json.isSuccess) {
                    console.log(json.Message);
                    if (parseInt(json.type) == TypeEventWebSocket.SignEInvoice) {
                        if (json.Data == "") {
                            toastrcus.error("Không ký số được dữ liệu hash");
                            resolve("-1");
                        } 
                        resolve(json.Data);
                    }
                    else if (parseInt(json.type) == TypeEventWebSocket.PrintInvoice) {
                        resolve("In hóa đơn bán hàng (bill)");
                    }
                    else if (parseInt(json.type) == TypeEventWebSocket.PrintBep) {
                        resolve("In bêp OK");
                    }
                    else if (parseInt(json.type) == TypeEventWebSocket.SendTestConnect) {
                        resolve("test");
                    }
                    
                } else {
                    toastrcus.error(json.Message);
                }
            };
            // khi lỡi
            webSocket.onerror = function () {
                StatusSposVietPlugin = 0;
                loadingStop();
                //reject("-1");
                resolve("-1");
                webSocket.close();
            };
        });
        

    }
}

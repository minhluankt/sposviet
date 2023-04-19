var toastr = {
    success: function (mess) {
        toastr.success(
            mess,
            { showMethod: "fadeIn", hideMethod: "fadeOut", timeOut: 2000 }
        );
    },
    info: function (mess) {
        toastr.info(
            mess,
            { showMethod: "fadeIn", hideMethod: "fadeOut", timeOut: 2000 }
        );
    },
    error: function (mess) {
        toastr.error(
            mess,
            { showMethod: "fadeIn", hideMethod: "fadeOut", timeOut: 2000 }
        );
    }

}

var company = {
    remove: function (sel) {
        let idcompany = $(sel).data("id");
        Swal.fire({
            icon: 'warning',
            title: 'Bạn có chắc chắn muốn công ty không, dữ liệu liên quan đến công ty cũng sẽ được xóa toàn bộ?',
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
                    url: '/company/delete?secret=' + idcompany,
                    data: {
                        //secret: idcompany
                    },

                    success: function (res) {
                        if (res.isValid) {
                            table
                                .row($(sel).parents('tr'))
                                .remove()
                                .draw();
                        }
                    },
                    error: function (err) {
                        console.log(err)
                    }
                });
            }
        })

    },// sự kiện xóa order
}
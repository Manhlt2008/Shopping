
$(document).ready(function () {
    // Init table
    initDatatable();
    // Init select
    initSelect();
    // Init date
    initDate();
    //set form submit
    var options = {
        target: "#result-search",
        beforeSubmit: showRequest,
        success: showResponse,
        type: 'post',
        resetForm: false
    };

    $('#form-search').ajaxForm(options); // id of the form we wish to submit
    $("#form-search").submit();
});

function initDate() {
    if (($('#dpStart').length) && ($('#dpEnd').length)) {
        $('#dpStart').datepicker({
            format: 'dd-mm-yyyy'
        }).on('changeDate', function (e) {
            $('#dpEnd').datepicker('setStartDate', e.date);
        });
        $('#dpEnd').datepicker({
            format: 'dd-mm-yyyy'
        }).on('changeDate', function (e) {
            $('#dpStart').datepicker('setEndDate', e.date)
        });
    }

    var d = new Date();
    var month = d.getMonth() + 1;
    var day = d.getDate();
    $('#dpEnd').val((('' + day).length < 2 ? '0' : '') + day + '-' + (('' + month).length < 2 ? '0' : '') + month + '-' + d.getFullYear());
    var d2 = new Date(d.setDate(d.getDate()));
    month = d2.getMonth() + 1;
    day = d2.getDate();
    $('#dpStart').val((('' + day).length < 2 ? '0' : '') + day + '-' + (('' + month).length < 2 ? '0' : '') + month + '-' + d.getFullYear());
}

function initDatatable() {
    if ($('#dt_scroll').length) {
        $('#dt_scroll').dataTable({
            "sScrollX": "100%",
            "sScrollXInner": '150%',
            "sPaginationType": "bootstrap",
            "bScrollCollapse": true
        });
    }
};

function initSelect() {
    if ($("select").length > 0) {
        $("select").select2({
            allowClear: true,
            minimumResultsForSearch: -1,
            placeholder: "Select..."
        })
    }
};

function showRequest(formData, jqForm, options) {
    $("#form-search :input").attr("disabled", true);//lock input
    $("#form-search button").attr("disabled", true);//lock button
};

function showResponse(responseText, statusText, xhr, $form) {
    //InitDatatable(); tạm thời đóng lại để theo dõi sau
    $("#form-search :input").attr("disabled", false);//lock input
    $("#form-search button").attr("disabled", false);//lock button
    //scrollToDiv("result-search");
};

function scrollToDiv(id) {
    $('html,body').animate({
        scrollTop: $("#" + id).offset().top - ($(window).height() - $("#" + id).outerHeight(true)) / 2
    }, 200);
};

// Transaction Detail
function getTransactionHistoryDetail(element) {
    var tid = $(element).data("id");
    var orderId = $(element).data("order-id");
    var orderCode = $(element).data("order-code");

    $("#span-tid").text("Mã GD :" + tid + "/ Mã HĐ : " + orderCode);
    var options = {
        url: "/Transaction/GetTransactionHistoryDetailForAdmin",
        data: {
            orderId: parseInt(orderId)
        },
        beforeSubmit: showRequest,
        success: function (response) {
            $("#form-search :input").attr("disabled", false);//lock input
            $("#form-search button").attr("disabled", false);//lock button
            $("#result-transaction-detail").empty();
            $("#result-transaction-detail").html(response);

            scrollToDiv("result-transaction-detail");
        },
        type: 'post'
    };

    $.ajax(options);
}
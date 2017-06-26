function scrollToDiv(id) {
    $('html,body').animate({
        scrollTop: $("#" + id).offset().top - ($(window).height() - $("#" + id).outerHeight(true)) / 2
    }, 200);
}
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
        success: function (response) {
            $("#form-search :input").attr("disabled", false);//lock input
            $("#form-search button").attr("disabled", false);//lock button
            $("#result-transaction-detail").empty();
            $("#result-transaction-detail").html(response);

            scrollToDiv("result-search");
        },
        type: 'post'
    };

    $.ajax(options);
}